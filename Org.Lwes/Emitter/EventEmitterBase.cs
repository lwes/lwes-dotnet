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
			StopSignaled = 3,
			Stopped = 4,
			Stopping = 5,
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
			int _senderState;

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
				if (Thread.VolatileRead(ref _senderState) > (int)EmitterState.Active)
					throw new InvalidOperationException(Resources.Error_EmitterHasEnteredShutdownState);

				_emitEP.SendTo(_sendToEP, LwesSerializer.Serialize(ev));
			}

			public void Start(IEventTemplateDB db, IPEndPoint sendToEP, Action<Socket, IPEndPoint> finishSocket)
			{
				_db = db;
				_sendToEP = sendToEP;
				_buffer = Buffers.AcquireBuffer(null);
				_emitEP = new UdpEndpoint(sendToEP).Initialize(finishSocket);
			}

			private void Dispose(bool p)
			{
				// Signal background threads...
				if (Interlocked.CompareExchange(ref _senderState, (int)EmitterState.StopSignaled, (int)EmitterState.Active) == (int)EmitterState.Active)
				{
					Thread.Sleep(0);
					Util.Dispose(ref _emitEP);
					Buffers.ReleaseBuffer(_buffer);
					_buffer = null;
				}
			}

			#endregion Methods
		}

		class ParallelEmitter : IEmitter
		{
			#region Fields

			byte[] _buffer;
			IEventTemplateDB _db;
			UdpEndpoint _emitEP;
			SimpleLockFreeQueue<Event> _eventQueue = new SimpleLockFreeQueue<Event>();
			EndPoint _sendToEP;
			int _senderState;

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
				EnsureSenderIsActive();
			}

			public void Start(IEventTemplateDB db, IPEndPoint sendToEP, Action<Socket, IPEndPoint> finishSocket)
			{
				_db = db;
				_sendToEP = sendToEP;
				_buffer = Buffers.AcquireBuffer(null);
				_emitEP = new UdpEndpoint(sendToEP).Initialize(finishSocket);
			}

			void Background_Emitter(object unused_state)
			{
				Thread.VolatileWrite(ref _senderState, (int)EmitterState.Active);
				Event ev;
				while (_eventQueue.Dequeue(out ev))
				{
					_emitEP.SendTo(_sendToEP, LwesSerializer.Serialize(ev));
				}
				Thread.VolatileWrite(ref _senderState, (int)EmitterState.Stopping);

				while (_eventQueue.Dequeue(out ev))
				{
					_emitEP.SendTo(_sendToEP, LwesSerializer.Serialize(ev));
				}
			}

			private void Dispose(bool p)
			{
				// Signal background threads...
				if (Interlocked.CompareExchange(ref _senderState, (int)EmitterState.StopSignaled, (int)EmitterState.Active) == (int)EmitterState.Active)
				{
					Thread.Sleep(0);
					Util.Dispose(ref _emitEP);
					Buffers.ReleaseBuffer(_buffer);
					_buffer = null;
				}
			}

			private void EnsureSenderIsActive()
			{
				if (Thread.VolatileRead(ref _senderState) != (int)EmitterState.Active)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(Background_Emitter));
				}
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}