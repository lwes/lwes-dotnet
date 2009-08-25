//
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT© 2009, Phillip Clark (phillip[at*flitbit[dot*org)
//	 original .NET implementation
//
// LWES.net is free software: you can redistribute it and/or modify
// it under the terms of the Lesser GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// LWES.net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// Lesser GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes.Listener
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Threading;

	using Org.Lwes.DB;
	using Org.Lwes.Properties;

	/// <summary>
	/// Base class for event listeners.
	/// </summary>
	public class EventListenerBase : IEventListener, ITraceable
	{
		#region Fields

		const int CDisposeBackgroundThreadWaitTimeMS = 200;
		const int LeadNotifier = 1;

		List<RegistrationKey> _additions = new List<RegistrationKey>();
		Action<RegistrationKey, Exception> _cacheHandleErrorsDelegate;
		int _consolidationVotes = 0;
		IEventTemplateDB _db;
		IPEndPoint _endpoint;
		ListenerGarbageHandling _garbageHandling;
		Dictionary<TrafficTrackingKey, TrafficTrackingRec> _garbageTracking;
		Object _garbageTrackingLock;
		IListener _listener;
		ReaderWriterLockSlim _notifications = new ReaderWriterLockSlim();
		int _notifiers = 0;
		RegistrationKey[] _registrations = new RegistrationKey[0];

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		protected EventListenerBase()
		{
			_cacheHandleErrorsDelegate = new Action<RegistrationKey, Exception>(HandleErrorsOnEventSink);
		}

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
			void Start(IEventTemplateDB db
				, IPEndPoint listenEP
				, Action<Socket, IPEndPoint> finishSocket
				, EventListenerBase listener);
		}

		#endregion Nested Interfaces

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
		/// Registers an event sink and activates it.
		/// </summary>
		/// <param name="sink">the event sink to register</param>
		/// <param name="handback">a handback object - this object is opaque to the listener
		/// and will be attached to the registration key prior to activation</param>
		/// <returns>A registration key for the event sink.</returns>
		public IEventSinkRegistrationKey RegisterAndActivateEventSink(IEventSink sink, object handback)
		{
			IEventSinkRegistrationKey key = RegisterEventSink(sink);
			key.Handback = handback;
			key.Activate();
			return key;
		}

		/// <summary>
		/// Registers an event sink with the listener without activating the
		/// event sink.
		/// </summary>
		/// <param name="sink">the event sink to register</param>
		/// <returns>A registration key for the event sink</returns>		
		public IEventSinkRegistrationKey RegisterEventSink(IEventSink sink)
		{
			if (sink == null) throw new ArgumentNullException("sink");
			RegistrationKey key = new RegistrationKey(this, sink);
			AddRegistration(key);
			return key;
		}

		internal void PerformEventArrival(Event ev)
		{
			int notifier = Interlocked.Increment(ref _notifiers);
			try
			{
				if (notifier == LeadNotifier) _notifications.EnterUpgradeableReadLock();
				else _notifications.EnterReadLock();
				try
				{
					foreach (var r in _registrations)
					{
						if (r.PerformEventArrival(ev, _cacheHandleErrorsDelegate))
						{
							Interlocked.Increment(ref _consolidationVotes);
						}
					}
					if (notifier == LeadNotifier && Thread.VolatileRead(ref _consolidationVotes) > 0)
					{
						SafeConsolidateRegistrations();
					}
				}
				finally
				{
					if (notifier == LeadNotifier) _notifications.ExitUpgradeableReadLock();
					else _notifications.ExitReadLock();
				}
			}
			finally
			{
				Interlocked.Decrement(ref _notifiers);
			}
		}

		internal GarbageHandlingVote PerformGarbageArrival(EndPoint remoteEndPoint, int priorGarbageCountForEndpoint, byte[] garbage)
		{
			GarbageHandlingVote strategy = GarbageHandlingVote.None;
			int notifier = Interlocked.Increment(ref _notifiers);
			try
			{
				if (notifier == LeadNotifier) _notifications.EnterUpgradeableReadLock();
				else _notifications.EnterReadLock();
				try
				{
					foreach (var r in _registrations)
					{
						GarbageHandlingVote strategyVote = r.PerformGarbageArrival(
							remoteEndPoint,
							priorGarbageCountForEndpoint,
							garbage,
							_cacheHandleErrorsDelegate
							);
						if (strategyVote > strategy)
						{
							strategy = strategyVote;
						}
					}
					if (notifier == LeadNotifier && Thread.VolatileRead(ref _consolidationVotes) > 0)
					{
						SafeConsolidateRegistrations();
					}
				}
				finally
				{
					if (notifier == LeadNotifier) _notifications.ExitUpgradeableReadLock();
					else _notifications.ExitReadLock();
				}
			}
			finally
			{
				Interlocked.Decrement(ref _notifiers);
			}
			return strategy;
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
		/// <param name="garbageHandling">indicates the garbage handling strategy the listener will use</param>
		/// <param name="finishSocket">callback method used to complete the setup of the socket
		/// connected to the given <paramref name="endpoint"/></param>
		protected void Initialize(IEventTemplateDB db
			, IPEndPoint endpoint
			, bool parallel
			, ListenerGarbageHandling garbageHandling
			, Action<Socket, IPEndPoint> finishSocket)
		{
			if (db == null) throw new ArgumentNullException("db");
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (finishSocket == null) throw new ArgumentNullException("finishSocket");

			_db = db;
			_endpoint = endpoint;
			IListener listener = (parallel)
				? (IListener)new ParallelListener()
				: (IListener)new BackgroundThreadListener();

			_garbageHandling = garbageHandling;
			if (_garbageHandling > ListenerGarbageHandling.FailSilently)
			{
				_garbageTracking = new Dictionary<TrafficTrackingKey, TrafficTrackingRec>();
				_garbageTrackingLock = new Object();
			}

			listener.Start(db, endpoint, finishSocket, this);
			_listener = listener;
		}

		private void AddRegistration(RegistrationKey key)
		{
			int notifier = Interlocked.Increment(ref _notifiers);
			try
			{
				if (notifier == LeadNotifier)
				{
					if (_notifications.TryEnterWriteLock(20))
					{
						try
						{
							lock (_additions)
							{
								_additions.Add(key);
								UnsafeConsolidateRegistrations();
							}
							return;
						}
						finally
						{
							_notifications.ExitWriteLock();
						}
					}
				}

				// We couldn't get the writelock so we're gonna have to schedule
				// the key to be added later...
				lock (_additions)
				{
					_additions.Add(key);
					Interlocked.Increment(ref _consolidationVotes);
				}
			}
			finally
			{
				Interlocked.Decrement(ref _notifiers);
			}
		}

		private GarbageHandlingVote GetTrafficStrategyForEndpoint(EndPoint ep)
		{
			if (_garbageHandling == ListenerGarbageHandling.FailSilently)
			{
				return GarbageHandlingVote.None;
			}
			else
			{
				IPEndPoint ipep = (IPEndPoint)ep;
				TrafficTrackingKey key = new TrafficTrackingKey(ep);
				TrafficTrackingRec tracking;
				lock (_garbageTrackingLock)
				{
					if (_garbageTracking.TryGetValue(key, out tracking))
					{
						return tracking.Strategy;
					}
				}
				return GarbageHandlingVote.Default;
			}
		}

		private void HandleErrorsOnEventSink(RegistrationKey key, Exception e)
		{
			this.TraceData(TraceEventType.Error, Resources.Error_EventSinkThrewException, key, e);
			// TODO: Strategies for event sinks that cause exceptions.
		}

		private void HandleGarbageData(EndPoint ep, byte[] buffer, int offset, int bytesTransferred)
		{
			this.TraceData(TraceEventType.Verbose, new Func<object[]>(() =>
				{
					return new object[] { ((IPEndPoint)ep).ToString(), Util.BytesToOctets(buffer, offset, bytesTransferred) };
				})
				);

			if (_garbageHandling > ListenerGarbageHandling.FailSilently)
			{
				IPEndPoint ipep = (IPEndPoint)ep;
				TrafficTrackingKey key = new TrafficTrackingKey(ep);
				TrafficTrackingRec tracking;
				lock (_garbageTrackingLock)
				{
					if (!_garbageTracking.TryGetValue(key, out tracking))
					{
						tracking = new TrafficTrackingRec(ep);
						_garbageTracking.Add(key, tracking);
					}
				}
				if (_garbageHandling == ListenerGarbageHandling.AskEventSinksToVoteOnStrategy
					&& tracking.Strategy != GarbageHandlingVote.IgnoreAllTrafficFromEndpoint)
				{
					PerformGarbageDataNotification(tracking, ep, buffer, offset, bytesTransferred);
				}
			}
		}

		private void PerformGarbageDataNotification(TrafficTrackingRec tracking, EndPoint rcep, byte[] buffer, int offset, int bytesTransferred)
		{
			byte[] copy = new byte[bytesTransferred];
			Array.Copy(buffer, copy, bytesTransferred);
			tracking.Strategy = PerformGarbageArrival(rcep, tracking.IncrementGarbageCount(), copy);
		}

		private void SafeConsolidateRegistrations()
		{
			_notifications.EnterWriteLock();
			try
			{
				lock (_additions)
				{
					UnsafeConsolidateRegistrations();
				}
			}
			finally
			{
				_notifications.ExitWriteLock();
			}
		}

		private void UnsafeConsolidateRegistrations()
		{
			#if DEBUG
			Debug.Assert(_notifications.IsWriteLockHeld);
			#endif
			_registrations = (from r in _registrations
												where r.Status != EventSinkStatus.Canceled
												select r).Concat(from r in _additions
																				 where r.Status != EventSinkStatus.Canceled
																				 select r).ToArray();
			_additions.Clear();

			Thread.VolatileWrite(ref _consolidationVotes, 0);
		}

		#endregion Methods

		#region Nested Types

		struct TrafficTrackingKey
		{
			#region Fields

			public uint Address;
			public int AddressFamily;
			public int Port;

			#endregion Fields

			#region Constructors

			public TrafficTrackingKey(EndPoint ep)
			{
				IPEndPoint ipep = (IPEndPoint)ep;
				Address = BitConverter.ToUInt32(ipep.Address.GetAddressBytes(), 0);
				Port = ipep.Port;
				AddressFamily = (int)ipep.AddressFamily;
			}

			#endregion Constructors
		}

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
			IEventTemplateDB _db;
			SimpleLockFreeQueue<Event> _eventQueue = new SimpleLockFreeQueue<Event>();
			UdpEndpoint _listenEP;
			EventListenerBase _listener;
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
			/// <param name="listener"></param>
			public void Start(IEventTemplateDB db
				, IPEndPoint listenEP
				, Action<Socket, IPEndPoint> finishSocket
				, EventListenerBase listener)
			{
				_db = db;
				_listener = listener;
				_anyEP = (listenEP.AddressFamily == AddressFamily.InterNetworkV6)
					? new IPEndPoint(IPAddress.IPv6Any, listenEP.Port)
					: new IPEndPoint(IPAddress.Any, listenEP.Port);
				_buffer = BufferManager.AcquireBuffer(null);
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
				if (_recieverState.TryTransition(ListenerState.StopSignaled, ListenerState.Active))
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
					if (!_eventQueue.TryDequeue(out ev))
					{
						lock (_notifierWaitObject)
						{ // double-check that the queue is empty
							// this strategy catches the race condition when the
							// reciever queue's an event while we're acquiring the lock.
							_notifierState.SetState(ListenerState.Suspending);
							if (!_eventQueue.TryDequeue(out ev))
							{
								_notifierState.SetState(ListenerState.Suspended);
								Monitor.Wait(_notifierWaitObject);
								continue;
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
					_listener.PerformEventArrival(ev);
				}
			}

			private void Background_Receiver(object unused_state)
			{
				if (_recieverState.TryTransition(ListenerState.Active, ListenerState.Unknown))
				{
					try
					{
						// Continue until signaled to stop...
						while (_recieverState.CurrentState == ListenerState.Active)
						{
							EndPoint rcep = _anyEP;
							// Perform a blocking receive...
							int bytesTransferred = _listenEP.ReceiveFrom(ref rcep, _buffer, 0, _buffer.Length);
							if (bytesTransferred > 0)
							{
								GarbageHandlingVote handling = _listener.GetTrafficStrategyForEndpoint(rcep);
								if (handling == GarbageHandlingVote.None)
								{
									PerformEventDeserializationAndQueueForNotification(rcep, _buffer, 0, bytesTransferred);
								}
								else if (handling == GarbageHandlingVote.TreatTrafficFromEndpointAsGarbage)
								{
									_listener.HandleGarbageData(rcep, _buffer, 0, bytesTransferred);
								}
								// Otherwise the handling was GarbageHandlingStrategy.FailfastForTrafficOnEndpoint
								// and we're going to ignore it altogether.
							}
						}
					}
					catch (SocketException se)
					{
						if (se.ErrorCode != 10004)
							throw se;
					}
					if (_recieverState.TryTransition(ListenerState.Stopping, ListenerState.StopSignaled))
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
				_recieverState.TryTransition(ListenerState.StopSignaled, ListenerState.Active, () =>
					{
						Util.Dispose(ref _listenEP);
						_reciever.Join(CDisposeBackgroundThreadWaitTimeMS);
						BufferManager.ReleaseBuffer(_buffer);
						_buffer = null;
					});
			}

			private void PerformEventDeserializationAndQueueForNotification(EndPoint rcep
				, byte[] buffer
				, int offset, int bytesTransferred)
			{
				IPEndPoint ep = (IPEndPoint)rcep;
				try
				{
					// For received events, set MetaEventInfo.ReciptTime, MetaEventInfo.SenderIP, and MetaEventInfo.SenderPort...
					Event ev = Event.BinaryDecode(_db, buffer, offset, bytesTransferred);
						ev.SetValue(Constants.MetaEventInfoAttributes.ReceiptTime.Name, Constants.DateTimeToLwesTimeTicks(DateTime.UtcNow));
						ev.SetValue(Constants.MetaEventInfoAttributes.SenderIP.Name, ep.Address);
						ev.SetValue(Constants.MetaEventInfoAttributes.SenderPort.Name, ep.Port);
					_eventQueue.Enqueue(ev);
				}
				catch (BadLwesDataException)
				{
					_listener.HandleGarbageData(rcep, buffer, offset, bytesTransferred);
				}

				if (_notifierState.CurrentState > ListenerState.Active)
				{
					// notifier thread is suspended;
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
			IEventTemplateDB _db;
			int _deserializers;
			SimpleLockFreeQueue<Event> _eventQueue = new SimpleLockFreeQueue<Event>();
			UdpEndpoint _listenEP;
			EventListenerBase _listener;
			Status<ListenerState> _listenerState;
			int _notifiers;
			SimpleLockFreeQueue<ReceiveCapture> _receiveQueue;

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
			/// <param name="owner">the owner</param>
			public void Start(IEventTemplateDB db
				, IPEndPoint listenEP
				, Action<Socket, IPEndPoint> finishSocket
				, EventListenerBase owner)
			{
				_db = db;
				_listener = owner;
				_anyEP = (listenEP.AddressFamily == AddressFamily.InterNetworkV6)
					? new IPEndPoint(IPAddress.IPv6Any, 0)
					: new IPEndPoint(IPAddress.Any, 0);

				_receiveQueue = new SimpleLockFreeQueue<ReceiveCapture>();

				_listenEP = new UdpEndpoint(listenEP).Initialize(finishSocket);
				ParallelReceiver();
			}

			internal void Stop()
			{
				_listenerState.TryTransition(ListenerState.StopSignaled, ListenerState.Active, () =>
				{
					while (Thread.VolatileRead(ref _deserializers) > 0)
					{
						Thread.Sleep(CDisposeBackgroundThreadWaitTimeMS);
					}
					while (Thread.VolatileRead(ref _notifiers) > 0)
					{
						Thread.Sleep(CDisposeBackgroundThreadWaitTimeMS);
					}

					Util.Dispose(ref _listenEP);

					_listenerState.SpinWaitForState(ListenerState.Stopped, () => Thread.Sleep(CDisposeBackgroundThreadWaitTimeMS));
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
				try
				{
					ReceiveCapture input;
					while (_listenerState.IsLessThan(ListenerState.StopSignaled) && _receiveQueue.TryDequeue(out input))
					{
						GarbageHandlingVote handling = _listener.GetTrafficStrategyForEndpoint(input.RemoteEndPoint);
						if (handling == GarbageHandlingVote.None)
						{
							PerformEventDeserializationAndQueueForNotification(input.RemoteEndPoint, input.Buffer, 0, input.BytesTransferred);
						}
						else if (handling == GarbageHandlingVote.TreatTrafficFromEndpointAsGarbage)
						{
							_listener.HandleGarbageData(input.RemoteEndPoint, input.Buffer, 0, input.BytesTransferred);
						}
						// Otherwise the handling was GarbageHandlingStrategy.FailfastForTrafficOnEndpoint
						// and we're going to ignore it altogether.
					}
				}
				finally
				{
					int z = Interlocked.Decrement(ref _deserializers);
					if (z == 0 && !_receiveQueue.IsEmpty)
						EnsureDeserializerIsActive();
				}
			}

			private void Background_Notifier(object unused_state)
			{
				//
				// Drains the event queue and performs notification
				//
				try
				{
					Event ev;
					while (_listenerState.IsLessThan(ListenerState.StopSignaled) && _eventQueue.TryDequeue(out ev))
					{
						_listener.PerformEventArrival(ev);
					}
				}
				finally
				{
					int z = Interlocked.Decrement(ref _notifiers);
					if (z == 0 && !_receiveQueue.IsEmpty)
						EnsureNotifierIsActive();
				}
			}

			private void Background_ParallelReceiver(object unused_state)
			{
				// Continue until signalled to stop...
				if (_listenerState.IsLessThan(ListenerState.StopSignaled))
				{
					// Acquiring a buffer may block until a buffer
					// becomes available.
					byte[] buffer = BufferManager.AcquireBuffer(() => _listenerState.IsGreaterThan(ListenerState.Active));

					// If the buffer is null then the stop-signal was received while acquiring a buffer
					if (buffer != null)
					{
						_listenEP.ReceiveFromAsync(_anyEP, buffer, 0, buffer.Length, (op) =>
						{
							if (op.SocketError == SocketError.Success)
							{
								// Reschedule the receiver before pulling the buffer out, we want to catch receives
								// in the tightest loop possible, although we don't want to keep a threadpool thread
								// *forever* and possibly cause thread-starvation in for other jobs so we continually
								// put the job back in the queue - this way our parallelism plays nicely with other
								// jobs - now, if only the other jobs were programmed to give up their threads periodically
								// too... hmmm!
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
				_listenerState.TryTransition(ListenerState.Stopping, ListenerState.StopSignaled, () =>
				{
					while (Thread.VolatileRead(ref _deserializers) > 0)
					{
						Thread.Sleep(CDisposeBackgroundThreadWaitTimeMS);
					}
					while (Thread.VolatileRead(ref _notifiers) > 0)
					{
						Thread.Sleep(CDisposeBackgroundThreadWaitTimeMS);
					}
					_listenerState.SetState(ListenerState.Stopped);
				});
			}

			private void Dispose(bool disposing)
			{
				if (_listenerState.CurrentState == ListenerState.Active)
					Stop();
			}

			private void EnsureDeserializerIsActive()
			{
				int current = -1, value = Thread.VolatileRead(ref _deserializers);
				if (value < 1)
				{
					WaitCallback cb = new WaitCallback(Background_Deserializer);
					while (true)
					{
						current = value;
						value = Interlocked.CompareExchange(ref _deserializers, value + 1, current);
						if (value == current)
						{
							ThreadPool.QueueUserWorkItem(cb);
							break;
						}
					}
				}
			}

			private void EnsureNotifierIsActive()
			{
				int current = -1, value = Thread.VolatileRead(ref _notifiers);
				if (value < 1)
				{
					WaitCallback cb = new WaitCallback(Background_Notifier);
					while (true)
					{
						current = value;
						value = Interlocked.CompareExchange(ref _notifiers, value + 1, current);
						if (value == current)
						{
							ThreadPool.QueueUserWorkItem(cb);
							break;
						}
					}
				}
			}

			private void ParallelReceiver()
			{
				// Only startup once.
				if (_listenerState.TryTransition(ListenerState.Active, ListenerState.Unknown))
				{
					Background_ParallelReceiver(null);
				}
			}

			private void PerformEventDeserializationAndQueueForNotification(EndPoint rcep
				, byte[] buffer
				, int offset, int bytesTransferred)
			{
				IPEndPoint ep = (IPEndPoint)rcep;
				try
				{
					// For received events, set MetaEventInfo.ReciptTime, MetaEventInfo.SenderIP, and MetaEventInfo.SenderPort...
					Event ev = Event.BinaryDecode(_db, buffer, offset, bytesTransferred);
						ev.SetValue(Constants.MetaEventInfoAttributes.ReceiptTime.Name, Constants.DateTimeToLwesTimeTicks(DateTime.UtcNow));
						ev.SetValue(Constants.MetaEventInfoAttributes.SenderIP.Name, ep.Address);
						ev.SetValue(Constants.MetaEventInfoAttributes.SenderPort.Name, ep.Port);
					_eventQueue.Enqueue(ev);
				}
				catch (BadLwesDataException)
				{
					_listener.HandleGarbageData(rcep, buffer, offset, bytesTransferred);
				}

				BufferManager.ReleaseBuffer(buffer);
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

		class RegistrationKey : IEventSinkRegistrationKey
		{
			#region Fields

			bool _disableGarbageNotification;
			Status<EventSinkStatus> _status = new Status<EventSinkStatus>(EventSinkStatus.Suspended);
			bool _threadSafe;

			#endregion Fields

			#region Constructors

			public RegistrationKey(EventListenerBase listener, IEventSink sink)
			{
				Listener = listener;
				Sink = sink;
				_threadSafe = sink.IsThreadSafe;
			}

			#endregion Constructors

			#region Properties

			public object Handback
			{
				get;
				set;
			}

			public IEventListener Listener
			{
				get;
				private set;
			}

			public IEventSink Sink
			{
				get;
				private set;
			}

			public EventSinkStatus Status
			{
				get { return _status.CurrentState; }
			}

			#endregion Properties

			#region Methods

			public bool Activate()
			{
				return _status.SetStateIfLessThan(EventSinkStatus.Active, EventSinkStatus.Canceled);
			}

			public void Cancel()
			{
				_status.SetState(EventSinkStatus.Canceled);
			}

			public void DisableGarbageNotification()
			{
				Thread.MemoryBarrier();
				_disableGarbageNotification = true;
				Thread.MemoryBarrier();
			}

			public bool Suspend()
			{
				return _status.SetStateIfLessThan(EventSinkStatus.Suspended, EventSinkStatus.Canceled);
			}

			internal bool PerformEventArrival(Event ev, Action<RegistrationKey, Exception> errorHandler)
			{
				if (!_threadSafe)
				{
					if (_status.SpinToggleState(EventSinkStatus.Notifying, EventSinkStatus.Active))
					{
						try
						{
							Sink.HandleEventArrival(this, ev);
							_status.TryTransition(EventSinkStatus.Active, EventSinkStatus.Notifying);
						}
						catch (Exception e)
						{
							errorHandler(this, e);
						}
					}
				}
				else
				{
					try
					{
						EventSinkStatus s = _status.CompareExchange(EventSinkStatus.Notifying, EventSinkStatus.Active);
						if (s == EventSinkStatus.Active || s == EventSinkStatus.Notifying)
						{
							Sink.HandleEventArrival(this, ev);
							_status.TryTransition(EventSinkStatus.Active, EventSinkStatus.Notifying);
						}
					}
					catch (Exception e)
					{
						errorHandler(this, e);
					}
				}
				return _status.CurrentState == EventSinkStatus.Canceled;
			}

			internal GarbageHandlingVote PerformGarbageArrival(EndPoint remoteEndPoint, int priorGarbageCountForEndpoint, byte[] garbage,
				Action<RegistrationKey, Exception> errorHandler)
			{
				Thread.MemoryBarrier();
				bool ignoring = _disableGarbageNotification;

				GarbageHandlingVote strategy = GarbageHandlingVote.None;
				if (!ignoring)
				{
					if (!_threadSafe)
					{
						if (_status.SpinToggleState(EventSinkStatus.Active, EventSinkStatus.Notifying))
						{
							try
							{
								strategy = Sink.HandleGarbageData(this, remoteEndPoint, priorGarbageCountForEndpoint, garbage);
							}
							catch (Exception e)
							{
								errorHandler(this, e);
							}
							_status.TryTransition(EventSinkStatus.Active, EventSinkStatus.Notifying);
						}
					}
					else
					{
						try
						{
							if (_status.TryTransition(EventSinkStatus.Notifying, EventSinkStatus.Active))
							{
								strategy = Sink.HandleGarbageData(this, remoteEndPoint, priorGarbageCountForEndpoint, garbage);
								_status.TryTransition(EventSinkStatus.Active, EventSinkStatus.Notifying);
							}
						}
						catch (Exception e)
						{
							errorHandler(this, e);
						}
					}
				}
				return strategy;
			}

			#endregion Methods
		}

		class TrafficTrackingRec
		{
			#region Fields

			int _garbageCount = 0;

			#endregion Fields

			#region Constructors

			public TrafficTrackingRec(EndPoint ep)
			{
				RemoteEndPoint = ep;
			}

			#endregion Constructors

			#region Properties

			public bool IsEmpty
			{
				get { return RemoteEndPoint == null; }
			}

			public int PreviousGargageDataCount
			{
				get { return _garbageCount; }
			}

			public EndPoint RemoteEndPoint
			{
				get;
				private set;
			}

			public GarbageHandlingVote Strategy
			{
				get;
				set;
			}

			#endregion Properties

			#region Methods

			public int IncrementGarbageCount()
			{
				return Interlocked.Increment(ref _garbageCount);
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}