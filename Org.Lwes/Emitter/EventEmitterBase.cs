namespace Org.Lwes.Emitter
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;

	using Org.Lwes.DB;
	using Org.Lwes.ESF;
	using Org.Lwes.Properties;

	/// <summary>
	/// Base class for event emitters.
	/// </summary>
	public abstract class EventEmitterBase : IEventEmitter
	{
		#region Fields
		const int CDisposeBackgroundThreadWaitTimeMS = 200;

		IEventTemplateDB _db;
		IEmitter _emitter;
		SupportedEncoding _enc;
		Encoding _encoding;
		int _initialized;
		bool _performValidation;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		protected EventEmitterBase()
		{
		}

		/// <summary>
		/// Destroys the instance; completes the IDisposable pattern.
		/// </summary>
		~EventEmitterBase()
		{
			Dispose(false);
		}

		#endregion Constructors

		#region Enumerations

		enum EmitterState
		{
			Unknown = 0,
			Initializing = 1,
			Active = 2,
			Suspending = 3,
			Suspended = 4,
			StopSignaled = 5,
			Stopping = 6,
			Stopped = 7,
		}

		#endregion Enumerations

		#region Nested Interfaces

		interface IEmitter : IDisposable
		{
			#region Methods

			void Emit(Event ev);

			void Start(IEventTemplateDB db
				, IPEndPoint sendToEP
				, Action<Socket, IPEndPoint> finishSocket);

			#endregion Methods
		}

		#endregion Nested Interfaces

		#region Properties

		/// <summary>
		/// The character encoding used when performing event IO.
		/// </summary>
		public Encoding Encoding
		{
			get { return _encoding; }
		}

		/// <summary>
		/// Indicates whether the factory has been initialized.
		/// </summary>
		public virtual bool IsInitialized
		{
			get { return Thread.VolatileRead(ref _initialized) == (int)EmitterState.Active; }
		}

		/// <summary>
		/// The event template database used by the factory.
		/// </summary>
		public IEventTemplateDB TemplateDB
		{
			get { return _db; }
		}

		/// <summary>
		/// Indicates whether events issued from the factory will validate
		/// when they are written to.
		/// </summary>
		public bool Validate
		{
			get { return _performValidation; }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <returns>a new LWES event instance</returns>
		public Event CreateEvent(string eventName)
		{
			if (!IsInitialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
			if (eventName == null) throw new ArgumentNullException("eventName");
			if (eventName.Length == 0) throw new ArgumentException(Resources.Error_EmptyStringNotAllowed, "eventName");

			Event result;
			if (!_db.TryCreateEvent(eventName, out result, _performValidation, _enc))
			{
				result = new Event(new EventTemplate(false, eventName), false, _enc);
			}
			return result;
		}

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <param name="enc">encoding used when performing IO on the event</param>
		/// <returns>a new LWES event instance</returns>
		public Event CreateEvent(string eventName, SupportedEncoding enc)
		{
			if (!IsInitialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
			if (eventName == null) throw new ArgumentNullException("eventName");
			if (eventName.Length == 0) throw new ArgumentException(Resources.Error_EmptyStringNotAllowed, "eventName");

			Event result;
			if (!_db.TryCreateEvent(eventName, out result, _performValidation, enc))
			{
				result = new Event(new EventTemplate(false, eventName), false, enc);
			}
			return result;
		}

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <param name="validate">whether the event is validated</param>
		/// <returns>a new LWES event instance</returns>
		public Event CreateEvent(string eventName, bool validate)
		{
			if (!IsInitialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
			if (eventName == null) throw new ArgumentNullException("eventName");
			if (eventName.Length == 0) throw new ArgumentException(Resources.Error_EmptyStringNotAllowed, "eventName");

			Event result;
			if (!_db.TryCreateEvent(eventName, out result, validate, _enc))
			{
				result = new Event(new EventTemplate(false, eventName), validate, _enc);
			}
			return result;
		}

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <param name="validate">whether the event is validated</param>
		/// <param name="enc">encoding used when performing IO on the event</param>
		/// <returns>a new LWES event instance</returns>
		public Event CreateEvent(string eventName, bool validate, SupportedEncoding enc)
		{
			if (!IsInitialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
			if (eventName == null) throw new ArgumentNullException("eventName");
			if (eventName.Length == 0) throw new ArgumentException(Resources.Error_EmptyStringNotAllowed, "eventName");

			Event result;
			if (!_db.TryCreateEvent(eventName, out result, validate, enc))
			{
				result = new Event(new EventTemplate(false, eventName), validate, enc);
			}
			return result;
		}

		/// <summary>
		/// Disposes of the emitter and frees any resources held.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Emits an event to the event system.
		/// </summary>
		/// <param name="evt">the event being emitted</param>
		public void Emit(Event evt)
		{
			if (!IsInitialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);

			_emitter.Emit(evt);
		}

		/// <summary>
		/// Disposes of the emitter.
		/// </summary>
		/// <param name="disposing">Indicates whether the object is being disposed</param>
		protected virtual void Dispose(bool disposing)
		{
			Util.Dispose(ref _emitter);
		}

		/// <summary>
		/// Initializes the emitter.
		/// </summary>
		/// <param name="enc">encoding the emitter should use for character data.</param>
		/// <param name="performValidation">whether the emitter validates emitted messages</param>
		/// <param name="db">a template DB to use when creating events</param>
		/// <param name="endpoint">An IP endpoint where events will be emitted</param>
		/// <param name="parallel">indicates whether the emitter will use the parallel strategy</param>
		/// <param name="finishSocket">callback method used to finish setup of the socket</param>
		protected void Initialize(SupportedEncoding enc, bool performValidation, IEventTemplateDB db
			, IPEndPoint endpoint, bool parallel, Action<Socket, IPEndPoint> finishSocket)
		{
			if (db == null) throw new ArgumentNullException("db");
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (finishSocket == null) throw new ArgumentNullException("finishSocket");

			if (Interlocked.CompareExchange(ref _initialized, (int)EmitterState.Initializing, (int)EmitterState.Unknown) == (int)EmitterState.Unknown)
			{
				_enc = enc;
				_encoding = Constants.GetEncoding((short)enc);
				_performValidation = performValidation;
				_db = db;

				IEmitter emitter = (parallel)
					? (IEmitter)new ParallelEmitter()
					: (IEmitter)new DirectEmitter();

				emitter.Start(db, endpoint, finishSocket);

				_emitter = emitter;

				Thread.VolatileWrite(ref _initialized, (int)EmitterState.Active);
			}
			else throw new InvalidOperationException(Resources.Error_AlreadyInitialized);
		}

		#endregion Methods

		#region Nested Types

		class DirectEmitter : IEmitter
		{
			#region Fields

			byte[] _buffer;
			IEventTemplateDB _db;
			UdpEndpoint _emitEP;
			EndPoint _sendToEP;
			Status<EmitterState> _senderState;

			#endregion Fields

			#region Constructors

			~DirectEmitter()
			{
				Dispose(false);
			}

			#endregion Constructors

			#region Methods

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			public void Emit(Event ev)
			{
				if (_senderState.IsGreaterThan(EmitterState.Active))
					throw new InvalidOperationException(Resources.Error_EmitterHasEnteredShutdownState);

				_emitEP.SendTo(_sendToEP, LwesSerializer.Serialize(ev));
			}

			public void Start(IEventTemplateDB db, IPEndPoint sendToEP, Action<Socket, IPEndPoint> finishSocket)
			{
				_db = db;
				_sendToEP = sendToEP;
				_buffer = BufferManager.AcquireBuffer(null);
				_emitEP = new UdpEndpoint(sendToEP).Initialize(finishSocket);
				_senderState.SetState(EmitterState.Active);
			}

			private void Dispose(bool p)
			{
				// Signal background threads...
				_senderState.TrySetStateWithAction(EmitterState.StopSignaled, EmitterState.Active, () =>
					{
						Util.Dispose(ref _emitEP);
						BufferManager.ReleaseBuffer(_buffer);
						_buffer = null;
					});
			}

			#endregion Methods
		}

		class ParallelEmitter : IEmitter
		{
			#region Fields

			IEventTemplateDB _db;
			UdpEndpoint _emitEP;
			SimpleLockFreeQueue<Event> _eventQueue = new SimpleLockFreeQueue<Event>();
			SimpleLockFreeQueue<byte[]> _dataQueue = new SimpleLockFreeQueue<byte[]>();
			EndPoint _sendToEP;
			Status<EmitterState> _serializerState;
			Status<EmitterState> _senderState = new Status<EmitterState>(EmitterState.Suspended);

			#endregion Fields

			#region Constructors

			~ParallelEmitter()
			{
				Dispose(false);
			}

			#endregion Constructors

			#region Methods

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			public void Emit(Event ev)
			{
				_eventQueue.Enqueue(ev);
				EnsureSerializerIsActive();
			}

			public void Start(IEventTemplateDB db, IPEndPoint sendToEP, Action<Socket, IPEndPoint> finishSocket)
			{
				_db = db;
				_sendToEP = sendToEP;
				_emitEP = new UdpEndpoint(sendToEP).Initialize(finishSocket);
				_serializerState.SetState(EmitterState.Suspended);
			}

			void Background_Serializer(object unused_state)
			{
				//
				// Drains the event queue and performs notification
				//
				if (_serializerState.SetStateIfLessThan(EmitterState.Active, EmitterState.StopSignaled))
				{
					Event ev;
					while (_eventQueue.Dequeue(out ev))
					{
						_dataQueue.Enqueue(LwesSerializer.SerializeToMemoryBuffer(ev));
						EnsureSenderIsActive();
					}
					_serializerState.TrySetStateWithAction(EmitterState.Suspending, EmitterState.Active, () =>
					{
						// We may temporarily have a thread active and a thread suspending
						// this will be temporary and together they will drain the queue
						while (_eventQueue.Dequeue(out ev))
						{
							_dataQueue.Enqueue(LwesSerializer.SerializeToMemoryBuffer(ev));
							EnsureSenderIsActive();
						}
						_serializerState.TrySetState(EmitterState.Suspended, EmitterState.Suspending);
					});
				}
			}

			void Background_Sender(object unused_state)
			{
				//
				// Drains the event queue and performs notification
				//
				byte[] data;

				if (_senderState.SetStateIfLessThan(EmitterState.Active, EmitterState.StopSignaled))
				{
					while (_senderState.CurrentState == EmitterState.Active && _dataQueue.Dequeue(out data))
					{
						_emitEP.SendToAsync(_sendToEP, data, data.Length, (op) =>
							{
								BufferManager.ReleaseBuffer(op.Buffer);
								if (op.SocketError == SocketError.OperationAborted)
								{
									_senderState.SetState(EmitterState.StopSignaled);
								}
								return false;
							});
					}
					_senderState.TrySetStateWithAction(EmitterState.Suspending, EmitterState.Active, () =>
					{
						// We may temporarily have a thread active and a thread suspending
						// this will be temporary and together they will drain the queue
						while (_senderState.IsLessThan(EmitterState.StopSignaled) && _dataQueue.Dequeue(out data))
						{
							_emitEP.SendToAsync(_sendToEP, data, data.Length, (op) =>
							{
								BufferManager.ReleaseBuffer(op.Buffer);
								if (op.SocketError == SocketError.OperationAborted)
								{
									_senderState.SetState(EmitterState.StopSignaled);
								}
								return false;
							});
						}
						_senderState.TrySetState(EmitterState.Suspended, EmitterState.Suspending);
					});
				}
			}

			private void EnsureSerializerIsActive()
			{
				if (_serializerState.IsGreaterThan(EmitterState.Active))
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(Background_Serializer));
				}
			}
			private void EnsureSenderIsActive()
			{
				if (_senderState.IsGreaterThan(EmitterState.Active))
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(Background_Sender));
				}
			}

			private void Dispose(bool p)
			{
				// Signal background threads...
				_senderState.TrySetStateWithAction(EmitterState.StopSignaled, EmitterState.Active, () =>
				{
					_serializerState.SetState(EmitterState.StopSignaled);
					_senderState.SetState(EmitterState.StopSignaled);
					Util.Dispose(ref _emitEP);
					Thread.Sleep(CDisposeBackgroundThreadWaitTimeMS);
					byte[] b;
					while (_dataQueue.Dequeue(out b))
					{
						BufferManager.ReleaseBuffer(b);
					}
				});
			}
			
			#endregion Methods
		}

		#endregion Nested Types
	}
}