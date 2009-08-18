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

		const int CDisposeBackgroundThreadWaitTimeMS = 200;

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
			Suspending = 2,
			Suspended = 3,
			StopSignaled = 4,
			Stopping = 5,
			Stopped = 6,
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
				: (IListener)new BackgroundThreadListener();

			listener.Start(db, endpoint, finishSocket, (e) =>
					{
						if (OnEventArrival != null)
							OnEventArrival(this, e);
					});
			_listener = listener;
		}

		#endregion Methods

		#region Nested Types

		/// <remarks>
		/// Uses background threads to receive events from LWES. This class uses two
		/// threads, one to listen and deserialize the events and another to perform
		/// the notifications.
		/// </remarks>
		class BackgroundThreadListener : IListener
		{
			#region Fields

			EndPoint _anyEP;
			byte[] _buffer;
			Action<Event> _callback;
			IEventTemplateDB _db;
			SimpleLockFreeQueue<Event> _eventQueue = new SimpleLockFreeQueue<Event>();
			UdpEndpoint _listenEP;
			Thread _notifier;
			Status<ListenerState> _notifierState;
			Object _notifierWaitObject;
			Thread _reciever;
			Status<ListenerState> _recieverState;

			#endregion Fields

			#region Constructors

			~BackgroundThreadListener()
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
				// Start a dedicated background thread to handle the receiving...
				_reciever = new Thread(Background_Receiver);
				_reciever.IsBackground = true;
				_reciever.Start();

				// Start a dedicated background thread to perform event notification...
				_notifierWaitObject = new Object();
				_notifier = new Thread(Background_Notifier);
				_notifier.IsBackground = true;
				_notifier.Start();
			}

			internal void Stop()
			{
				if (_recieverState.TrySetState(ListenerState.StopSignaled, ListenerState.Active))
				{
					// Close the listener, this will cause the receiver thread to wakeup
					// if it is blocked waiting for IO on the socket.
					Util.Dispose(ref _listenEP);
					_reciever.Join();
				}
			}

			private void Background_Notifier(object unused_state)
			{
				_notifierState.SetState(ListenerState.Active);
				while (_notifierState.CurrentState < ListenerState.StopSignaled)
				{
					Event ev;
					if (!_eventQueue.Dequeue(out ev))
					{
						lock (_notifierWaitObject)
						{ // double-check that the queue is empty
							// this strategy catches the race condition when the
							// reciever queue's an event while we're acquiring the lock.
							_notifierState.SetState(ListenerState.Suspending);
							if (!_eventQueue.Dequeue(out ev))
							{
								_notifierState.SetState(ListenerState.Suspended);
								Monitor.Wait(_notifierWaitObject);
							}
							// If the stop signal arrived during a wait then bail out...
							if (_notifierState.CurrentState == ListenerState.StopSignaled)
							{
								_notifierState.SetState(ListenerState.Stopped);
								break;
							}
							// otherwise we're active again
							_notifierState.SetState(ListenerState.Active);
						}
					}
					_callback(ev);
				}
			}

			private void Background_Receiver(object unused_state)
			{
				if (_recieverState.TrySetState(ListenerState.Active, ListenerState.Unknown))
				{
					try
					{
						// Continue until signaled to stop...
						while (_recieverState.CurrentState == ListenerState.Active)
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
					if (_recieverState.TrySetState(ListenerState.Stopping, ListenerState.StopSignaled))
					{
						// Cascade the stop signal to the notifier and wait for it to exit...
						_notifierState.SetState(ListenerState.StopSignaled);
						_notifier.Join();
					}
				}
			}

			private void Dispose(bool disposing)
			{
				// Signal background threads...
				_recieverState.TrySetStateWithAction(ListenerState.StopSignaled, ListenerState.Active, () =>
					{
						Util.Dispose(ref _listenEP);
						_reciever.Join(CDisposeBackgroundThreadWaitTimeMS);
						Buffers.ReleaseBuffer(_buffer);
						_buffer = null;
					});
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

				if (_notifierState.CurrentState > ListenerState.Active)
				{
					// notifier thread is either suspending or suspended;
					// wake it up...
					lock (_notifierWaitObject)
					{
						Monitor.Pulse(_notifierWaitObject);
					}
				}
			}

			#endregion Methods
		}

		/// <remarks>
		/// Uses the threadpool and overlapped IO on the recieving socket. This listener
		/// will consume between 0 and 3 threads from the threadpool, depending on which
		/// jobs are active. The jobs may consist of the following:
		/// <ul>
		/// <li>Receiver - invoked by the socket on a threadpool thread when input is received</li>
		/// <li>Deserializer - scheduled for a threadpool thread and runs as long as buffers are in the receive queue</li>
		/// <li>Notifier - scheduled for a threadpool thread and runs as long as Events are in the notification queue</li>
		/// </ul>
		/// </remarks>
		class ParallelListener : IListener
		{
			#region Fields

			EndPoint _anyEP;
			Action<Event> _callback;
			IEventTemplateDB _db;
			Status<ListenerState> _deserializerState = new Status<ListenerState>(ListenerState.Suspended);
			SimpleLockFreeQueue<Event> _eventQueue = new SimpleLockFreeQueue<Event>();
			UdpEndpoint _listenEP;
			Status<ListenerState> _notifierState = new Status<ListenerState>(ListenerState.Suspended);
			SimpleLockFreeQueue<ReceiveCapture> _receiveQueue;
			Status<ListenerState> _recieverState;

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

				_receiveQueue = new SimpleLockFreeQueue<ReceiveCapture>();

				_listenEP = new UdpEndpoint(listenEP).Initialize(finishSocket);
				ParallelReceiver();
			}

			internal void Stop()
			{
				_recieverState.TrySetStateWithAction(ListenerState.StopSignaled, ListenerState.Active, () =>
				{
					Util.Dispose(ref _listenEP);

					_recieverState.SpinWaitForState(ListenerState.Stopped, () => Thread.Sleep(CDisposeBackgroundThreadWaitTimeMS));
				});
			}

			private void Background_Deserializer(object unused_state)
			{
				// Called within the thread pool:
				//
				// Drains the recieve queue of capture records and
				// transforms those records into Event objects by deserialization.
				//
				//
				if (_deserializerState.SetStateIfLessThan(ListenerState.Active, ListenerState.StopSignaled))
				{
					ReceiveCapture input;
					while (_receiveQueue.Dequeue(out input))
					{
						PerformEventDeserializationAndQueueForNotification(input.RemoteEndPoint, input.Buffer, 0, input.BytesTransferred);
					}
					_deserializerState.TrySetStateWithAction(ListenerState.Suspending, ListenerState.Active, () =>
						{
							// We may temporarily have a thread active and a thread suspending
							// this will be temporary and together they will drain the queue
							while (_receiveQueue.Dequeue(out input))
							{
								PerformEventDeserializationAndQueueForNotification(input.RemoteEndPoint, input.Buffer, 0, input.BytesTransferred);
							}
							_deserializerState.TrySetState(ListenerState.Suspended, ListenerState.Suspending);
						});
				}
			}

			private void Background_Notifier(object unused_state)
			{
				//
				// Drains the event queue and performs notification
				//
				if (_notifierState.SetStateIfLessThan(ListenerState.Active, ListenerState.StopSignaled))
				{
					Event ev;
					while (_eventQueue.Dequeue(out ev))
					{
						_callback(ev);
					}
					_notifierState.TrySetStateWithAction(ListenerState.Suspending, ListenerState.Active, () =>
						{
							// We may temporarily have a thread active and a thread suspending
							// this will be temporary and together they will drain the queue
							while (_eventQueue.Dequeue(out ev))
							{
								_callback(ev);
							}
							_notifierState.TrySetState(ListenerState.Suspended, ListenerState.Suspending);
						});
				}
			}

			private void ParallelReceiver()
			{
				if (_recieverState.TrySetState(ListenerState.Active, ListenerState.Unknown))
				{
					Background_ParallelReceiver(null);
				}
			}

			private void Background_ParallelReceiver(object unused_state)
			{
				// Continue until signalled to stop...
				if (_recieverState.CurrentState == ListenerState.Active)
				{
					// Acquiring a buffer may block until a buffer
					// becomes available.
					byte[] buffer = Buffers.AcquireBuffer(() => _recieverState.IsGreaterThan(ListenerState.Active));

					// If the buffer is null then the stop-signal was received while acquiring a buffer
					if (buffer != null)
					{
						_listenEP.ReceiveFromAsync(_anyEP, buffer, 0, buffer.Length, (op) =>
						{
							if (op.SocketError == SocketError.Success)
							{
								// Reschedule the receiver before pulling the buffer out, we want to catch receives
								// in the tightest loop possible, excepting this little ditty; we don't want to keep
								// a threadpool thread *for ever* so we continually put the job back in the queue - this
								// way our parallelism plays nicely with other jobs - now, if only the other jobs were
								// programmed to give up their threads periodically too... hmmm!
								ThreadPool.QueueUserWorkItem(new WaitCallback(Background_ParallelReceiver));
								if (op.BytesTransferred > 0)
								{
									_receiveQueue.Enqueue(new ReceiveCapture(op.RemoteEndPoint, op.Buffer, op.BytesTransferred));

									EnsureDeserializerIsActive();
								}
							}
							else if (op.SocketError == SocketError.OperationAborted)
							{
								// This is the dispose or stop call. fall through
								CascadeStopSignal();
							}

							return false;
						}, null);
						return;
					}
				}

				// We get here if the receiver is signaled to stop.
				CascadeStopSignal();
			}

			private void CascadeStopSignal()
			{
				_recieverState.TrySetStateWithAction(ListenerState.Stopping, ListenerState.StopSignaled, () =>
				{
					_deserializerState.SetState(ListenerState.StopSignaled);
					_notifierState.SetState(ListenerState.StopSignaled);
					_recieverState.SetState(ListenerState.Stopped);
				});
			}

			private void Dispose(bool disposing)
			{
				if (_recieverState.CurrentState == ListenerState.Active)
					Stop();
			}

			private void EnsureDeserializerIsActive()
			{
				if (_deserializerState.IsGreaterThan(ListenerState.Active))
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(Background_Deserializer));
				}
			}

			private void EnsureNotifierIsActive()
			{
				if (_notifierState.IsGreaterThan(ListenerState.Active))
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
				Buffers.ReleaseBuffer(buffer);
				EnsureNotifierIsActive();
			}

			#endregion Methods

			#region Nested Types

			struct ReceiveCapture
			{
				#region Fields

				public byte[] Buffer;
				public int BytesTransferred;
				public EndPoint RemoteEndPoint;

				#endregion Fields

				#region Constructors

				public ReceiveCapture(EndPoint ep, byte[] data, int transferred)
				{
					this.RemoteEndPoint = ep;
					this.Buffer = data;
					this.BytesTransferred = transferred;
				}

				#endregion Constructors
			}

			#endregion Nested Types
		}

		#endregion Nested Types

		#region Other

		//bool _parallel;

		#endregion Other
	}
}