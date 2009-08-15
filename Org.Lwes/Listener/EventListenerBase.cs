namespace Org.Lwes.Listener
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Threading;

	using Org.Lwes.DB;
	using Org.Lwes.Properties;

	/// <summary>
	/// Base class for event listeners.
	/// </summary>
	public class EventListenerBase : IEventListener
	{
		#region Fields

		IEventTemplateDB _db;
		IPEndPoint _endpoint;
		IListener _listener;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Destructor ensuring dispose is called.
		/// </summary>
		~EventListenerBase()
		{
			Dispose(false);
		}

		#endregion Constructors

		#region Enumerations

		enum ListenerState
		{
			Unknown = 0,
			Active = 1,
			StopSignaled = 2,
			Stopped = 3,
			Stopping = 4,
		}

		#endregion Enumerations

		#region Nested Interfaces

		interface IListener : IDisposable
		{
			#region Methods

			void Start(IEventTemplateDB db
				, IPEndPoint listenEP
				, Action<Socket, IPEndPoint> finishSocket
				, Action<Event> callback);

			#endregion Methods
		}

		#endregion Nested Interfaces

		#region Events

		/// <summary>
		/// .NET language event signaled when a new LWES event arrives at the listener.
		/// </summary>
		public event HandleEventArrival OnEventArrival;

		#endregion Events

		#region Properties

		/// <summary>
		/// Indicates whether the listener has been initialized.
		/// </summary>
		public virtual bool IsInitialized
		{
			get { return _listener != null; }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Disposes of the emitter and frees any resources held.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Ensures the emitter has been initialized.
		/// </summary>
		/// <exception cref="InvalidOperationException">thrown if the emitter has not yet been initialized.</exception>
		protected void CheckInitialized()
		{
			if (IsInitialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
		}

		/// <summary>
		/// Disposes of the emitter.
		/// </summary>
		/// <param name="disposing">Indicates whether the object is being disposed</param>
		protected virtual void Dispose(bool disposing)
		{
			Util.Dispose(ref _listener);
		}

		/// <summary>
		/// Initializes the base class.
		/// </summary>
		/// <param name="db">template database used when creating events</param>
		/// <param name="endpoint">an IP endpoint where listening will occur</param>
		/// <param name="parallel">whether the listener will listen and dispatch events in parallel</param>
		/// <param name="finishSocket">callback method used to complete the setup of the socket
		/// connected to the given <paramref name="endpoint"/></param>
		protected void Initialize(IEventTemplateDB db, IPEndPoint endpoint, bool parallel, Action<Socket, IPEndPoint> finishSocket)
		{
			if (db == null) throw new ArgumentNullException("db");
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (finishSocket == null) throw new ArgumentNullException("finishSocket");

			_db = db;
			_endpoint = endpoint;
			IListener listener = (parallel)
				? (IListener)new ParallelListener()
				: (IListener)new MultiThreadedListener();

			listener.Start(db, endpoint, finishSocket, (e) =>
					{
						if (OnEventArrival != null)
							OnEventArrival(this, e);
					});
			_listener = listener;
		}

		#endregion Methods

		#region Nested Types

		class MultiThreadedListener : IListener
		{
			#region Fields

			EndPoint _anyEP;
			byte[] _buffer;
			Action<Event> _callback;
			IEventTemplateDB _db;
			SimpleLockFreeQueue<Event> _eventQueue = new SimpleLockFreeQueue<Event>();
			UdpEndpoint _listenEP;
			int _notifierState;
			int _recieverState;

			#endregion Fields

			#region Constructors

			~MultiThreadedListener()
			{
				Dispose(false);
			}

			#endregion Constructors

			#region Methods

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Starts the listener in multi-threaded mode. In this mode the listener
			/// consumes from 1 to 2 threads from the threadpool. A thread is used for
			/// receiving bytes and deserializing LWES events and another thread is 
			/// scheduled to perform event notification only when LWES events have
			/// been received.
			/// </summary>
			/// <param name="db">event template DB used during deserialization</param>
			/// <param name="listenEP">a IP endpoint where listening should occur</param>
			/// <param name="finishSocket"></param>
			/// <param name="callback"></param>
			public void Start(IEventTemplateDB db
				, IPEndPoint listenEP
				, Action<Socket, IPEndPoint> finishSocket
				, Action<Event> callback)
			{
				_db = db;
				_callback = callback;
				_anyEP = (listenEP.AddressFamily == AddressFamily.InterNetworkV6)
					? new IPEndPoint(IPAddress.IPv6Any, listenEP.Port)
					: new IPEndPoint(IPAddress.Any, listenEP.Port);
				_buffer = Buffers.AcquireBuffer(null);
				_listenEP = new UdpEndpoint(listenEP).Initialize(finishSocket);
				ThreadPool.QueueUserWorkItem(new WaitCallback(Background_Receiver));
			}

			internal void Stop()
			{
				if (Interlocked.CompareExchange(ref _recieverState, (int)ListenerState.StopSignaled, (int)ListenerState.Active) == (int)ListenerState.Active)
				{
					Util.Dispose(ref _listenEP);
					while (Thread.VolatileRead(ref _recieverState) != (int)ListenerState.Stopped)
					{
						Thread.Sleep(20);
					}
				}
			}

			private void Background_Notifier(object unused_state)
			{
				Thread.VolatileWrite(ref _notifierState, (int)ListenerState.Active);
				Event ev;
				while (_eventQueue.Dequeue(out ev))
				{
					_callback(ev);
				}
				Thread.VolatileWrite(ref _notifierState, (int)ListenerState.Stopping);
				// Catch queued input during race condition -
				// With this strategy it is possible for two threads
				// to be temporarily competing to drain the queue,
				// but as the queue is lock-free and thread-safe
				// the competition is mostly benign and short lived.
				while (_eventQueue.Dequeue(out ev))
				{
					_callback(ev);
				}
			}

			private void Background_Receiver(object unused_state)
			{
				if (Interlocked.CompareExchange(ref _recieverState, (int)ListenerState.Active, (int)ListenerState.Unknown) == (int)ListenerState.Unknown)
				{
					try
					{
						// Continue until signaled to stop...
						while (Thread.VolatileRead(ref _recieverState) == (int)ListenerState.Active)
						{
							EndPoint rcep = _anyEP;
							// Perform a blocking receive...
							int bytes = _listenEP.ReceiveFrom(ref rcep, _buffer, 0, _buffer.Length);
							if (bytes > 0)
							{
								// Data was received, deserialize the event...
								PerformEventDeserializationAndQueueForNotification(rcep, _buffer, 0, bytes);
							}
						}
					}
					catch (SocketException se)
					{
						if (se.ErrorCode != 10004)
							throw se;
					}
					if (Interlocked.CompareExchange(ref _recieverState, (int)ListenerState.Stopping, (int)ListenerState.StopSignaled) == (int)ListenerState.StopSignaled)
					{
						Thread.VolatileWrite(ref _recieverState, (int)ListenerState.Stopped);
					}
				}
			}

			private bool CheckForStopSignal()
			{
				// Check for stop signal
				if (Interlocked.CompareExchange(ref _recieverState, (int)ListenerState.Stopping, (int)ListenerState.StopSignaled) == (int)ListenerState.StopSignaled)
				{
					// When signaled, stop rescheduling.
					Thread.VolatileWrite(ref _recieverState, (int)ListenerState.Stopped);
					return true;
				}
				return false;
			}

			private void Dispose(bool disposing)
			{
				// Signal background threads...
				if (Interlocked.CompareExchange(ref _recieverState, (int)ListenerState.StopSignaled, (int)ListenerState.Active) == (int)ListenerState.Active)
				{
					Thread.Sleep(0);
					Util.Dispose(ref _listenEP);
					Buffers.ReleaseBuffer(_buffer);
					_buffer = null;
				}
			}

			private void EnsureNotifierIsActive()
			{
				if (Thread.VolatileRead(ref _notifierState) != (int)ListenerState.Active)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(Background_Notifier));
				}
			}

			private void PerformEventDeserializationAndQueueForNotification(EndPoint rcep
				, byte[] buffer
				, int p, int bytes)
			{
				IPEndPoint ep = (IPEndPoint)rcep;
				// For received events, set MetaEventInfo.ReciptTime, MetaEventInfo.SenderIP, and MetaEventInfo.SenderPort...
				Event ev = Event.BinaryDecode(_db, buffer, 0, bytes)
					.SetValue(Constants.MetaEventInfoAttributes.ReceiptTime.Name, Constants.DateTimeToLwesTimeTicks(DateTime.UtcNow))
					.SetValue(Constants.MetaEventInfoAttributes.SenderIP.Name, ep.Address)
					.SetValue(Constants.MetaEventInfoAttributes.SenderPort.Name, ep.Port);

				_eventQueue.Enqueue(ev);
				EnsureNotifierIsActive();
			}

			#endregion Methods
		}

		class ParallelListener : IListener
		{
			#region Fields

			EndPoint _anyEP;
			Action<Event> _callback;
			IEventTemplateDB _db;
			int _deserializerState;
			SimpleLockFreeQueue<Event> _eventQueue = new SimpleLockFreeQueue<Event>();
			UdpEndpoint _listenEP;
			int _notifierState;
			SimpleLockFreeQueue<EndPointOpState> _receiveQueue;
			int _recieverState;

			#endregion Fields

			#region Constructors

			~ParallelListener()
			{
				Dispose(false);
			}

			#endregion Constructors

			#region Methods

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Starts the listener.
			/// </summary>
			/// <param name="db">an event template DB</param>
			/// <param name="listenEP">the listening endpoint</param>
			/// <param name="finishSocket">a callback method that is called upon to finish the listening socket</param>
			/// <param name="callback">a callback method to receive events</param>
			public void Start(IEventTemplateDB db
				, IPEndPoint listenEP
				, Action<Socket, IPEndPoint> finishSocket
				, Action<Event> callback)
			{
				_db = db;
				_callback = callback;
				_anyEP = (listenEP.AddressFamily == AddressFamily.InterNetworkV6)
					? new IPEndPoint(IPAddress.IPv6Any, 0)
					: new IPEndPoint(IPAddress.Any, 0);

				_receiveQueue = new SimpleLockFreeQueue<EndPointOpState>();

				_listenEP = new UdpEndpoint(listenEP).Initialize(finishSocket);
				ParallelReceiver();
			}

			internal void Stop()
			{
				if (Interlocked.CompareExchange(ref _recieverState, (int)ListenerState.StopSignaled, (int)ListenerState.Active) == (int)ListenerState.Active)
				{
					Util.Dispose(ref _listenEP);
					while (Thread.VolatileRead(ref _recieverState) != (int)ListenerState.Stopped)
					{
						Thread.Sleep(20);
					}
				}
			}

			private void Background_Deserializer(object unused_state)
			{
				// Called within the thread pool:
				//
				// Drains the recieve queue of opstate records and
				// transforms those records into Event objects by deserialization.
				//
				//
				Thread.VolatileWrite(ref _deserializerState, (int)ListenerState.Active);
				EndPointOpState input;
				while (_receiveQueue.Dequeue(out input))
				{
					PerformEventDeserializationAndQueueForNotification(input.RemoteEndPoint, input.Buffer, 0, input.BytesTransferred);
				}
				Thread.VolatileWrite(ref _deserializerState, (int)ListenerState.Stopping);
				// Catch queued input during race condition -
				// With this strategy it is possible for two threads
				// to be temporarily competing to drain the queue,
				// but as the queue is lock-free and thread-safe
				// the competition is mostly benign and short lived.
				while (_receiveQueue.Dequeue(out input))
				{
					PerformEventDeserializationAndQueueForNotification(input.RemoteEndPoint, input.Buffer, 0, input.BytesTransferred);
				}
			}

			private void Background_Notifier(object unused_state)
			{
				Thread.VolatileWrite(ref _notifierState, (int)ListenerState.Active);
				Event ev;
				while (_eventQueue.Dequeue(out ev))
				{
					_callback(ev);
				}
				Thread.VolatileWrite(ref _notifierState, (int)ListenerState.Stopping);
				// Catch queued input during race condition -
				// With this strategy it is possible for two threads
				// to be temporarily competing to drain the queue,
				// but as the queue is lock-free and thread-safe
				// the competition is mostly benign and short lived.
				while (_eventQueue.Dequeue(out ev))
				{
					_callback(ev);
				}
			}

			private bool CheckForStopSignal()
			{
				// Check for stop signal
				if (Interlocked.CompareExchange(ref _recieverState, (int)ListenerState.Stopping, (int)ListenerState.StopSignaled) == (int)ListenerState.StopSignaled)
				{
					// When signaled, stop rescheduling.
					Thread.VolatileWrite(ref _recieverState, (int)ListenerState.Stopped);
					return true;
				}
				return false;
			}

			private void Dispose(bool disposing)
			{
				// Signal background threads...
				if (Interlocked.CompareExchange(ref _recieverState, (int)ListenerState.StopSignaled, (int)ListenerState.Active) == (int)ListenerState.Active)
				{
					Thread.Sleep(0);
					Util.Dispose(ref _listenEP);
				}
			}

			private void EnsureDeserializerIsActive()
			{
				if (Thread.VolatileRead(ref _deserializerState) != (int)ListenerState.Active)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(Background_Deserializer));
				}
			}

			private void EnsureNotifierIsActive()
			{
				if (Thread.VolatileRead(ref _notifierState) != (int)ListenerState.Active)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(Background_Notifier));
				}
			}

			private void ParallelReceiver()
			{
				if (CheckForStopSignal()) return;

				// Acquiring a buffer may block until a buffer
				// becomes available.
				byte[] buffer = Buffers.AcquireBuffer(() =>
					{
						return Thread.VolatileRead(ref _recieverState) > (int)ListenerState.Active;
					});

				if (buffer == null)
				{
					// Fall through; stop was signaled before a buffer
					// could be acquired.
					return;
				}

				// Use overlapped receive - the lambda callback will fire in a background thread when bytes arrive.
				_listenEP.ReceiveFromAsync(_anyEP, buffer, 0, buffer.Length, (a) =>
				{
					if (a.SocketError == SocketError.Success)
					{
						if (a.BytesTransferred > 0)
						{
							// Bytes were received; schedule for deserialization...
							_receiveQueue.Enqueue(a);
							EnsureDeserializerIsActive();
						}
					}

					// Always restart the receiver - stop signaling is checked on re-entry.
					ParallelReceiver();

					return false; // not reusing EndPointOpState
				}, null);
			}

			private void PerformEventDeserializationAndQueueForNotification(EndPoint rcep
				, byte[] buffer
				, int p, int bytes)
			{
				IPEndPoint ep = (IPEndPoint)rcep;
				// For received events, set MetaEventInfo.ReciptTime, MetaEventInfo.SenderIP, and MetaEventInfo.SenderPort...
				Event ev = Event.BinaryDecode(_db, buffer, 0, bytes)
					.SetValue(Constants.MetaEventInfoAttributes.ReceiptTime.Name, Constants.DateTimeToLwesTimeTicks(DateTime.UtcNow))
					.SetValue(Constants.MetaEventInfoAttributes.SenderIP.Name, ep.Address)
					.SetValue(Constants.MetaEventInfoAttributes.SenderPort.Name, ep.Port);

				_eventQueue.Enqueue(ev);
				Buffers.ReleaseBuffer(buffer);
				EnsureNotifierIsActive();
			}

			#endregion Methods
		}

		#endregion Nested Types

		#region Other

		//bool _parallel;

		#endregion Other
	}
}