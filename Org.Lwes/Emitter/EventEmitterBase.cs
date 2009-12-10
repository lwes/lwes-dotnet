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
namespace Org.Lwes.Emitter
{
	using System;
	using System.Diagnostics;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	using Org.Lwes.DB;
	using Org.Lwes.ESF;
	using Org.Lwes.Properties;
	using Org.Lwes.Trace;

	/// <summary>
	/// Base class for event emitters.
	/// </summary>
	public abstract class EventEmitterBase : IEventEmitter, ITraceable
	{
		#region Fields

		const int CDisposeBackgroundThreadWaitTimeMS = 200;

		IPAddress _address;
		IEventTemplateDB _db;
		IEmitter _emitter;
		SupportedEncoding _enc;
		Encoding _encoding;
		int _port;
		Status<EmitterState> _status;
		bool _validate;

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
			void Emit(Event ev);

			void Start(IEventTemplateDB db
				, IPEndPoint sendToEP
				, Action<Socket, IPEndPoint> finishSocket);
		}

		#endregion Nested Interfaces

		#region Properties

		/// <summary>
		/// The ip address to which events are emitted.
		/// </summary>
		public IPAddress Address
		{
			get
			{
				return _address;
			}
			set
			{
				if (IsInitialized) throw new InvalidOperationException(Resources.Error_AlreadyInitialized);
				_address = value;
			}
		}

		/// <summary>
		/// The character encoding used when performing event IO.
		/// </summary>
		public SupportedEncoding Encoding
		{
			get { return _enc; }
			set
			{
				if (IsInitialized) throw new InvalidOperationException(Resources.Error_AlreadyInitialized);
				_enc = value;
				_encoding = Constants.GetEncoding((short)value);
			}
		}

		/// <summary>
		/// Indicates whether the factory has been initialized.
		/// </summary>
		public virtual bool IsInitialized
		{
			get { return _status.CurrentState == EmitterState.Active; }
		}

		/// <summary>
		/// The ip port to which events are emitted.
		/// </summary>
		public int Port
		{
			get
			{
				return _port;
			}
			set
			{
				if (IsInitialized) throw new InvalidOperationException(Resources.Error_AlreadyInitialized);
				_port = value;
			}
		}

		/// <summary>
		/// The event template database used when creating events.
		/// </summary>
		public IEventTemplateDB TemplateDB
		{
			get { return _db; }
			set
			{
				if (IsInitialized) throw new InvalidOperationException(Resources.Error_AlreadyInitialized);
				_db = value;
			}
		}

		/// <summary>
		/// Indicates whether events issued from the factory will validate
		/// when they are written to.
		/// </summary>
		public bool Validate
		{
			get { return _validate; }
			set
			{
				if (IsInitialized) throw new InvalidOperationException(Resources.Error_AlreadyInitialized);
				_validate = value;
			}
		}

		/// <summary>
		/// Indicates whether the emitter is using a parallel emit strategy.
		/// </summary>
		protected bool IsParallel
		{
			get; set;
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
			return CreateEvent(eventName, _validate, _enc);
		}

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <param name="enc">encoding used when performing IO on the event</param>
		/// <returns>a new LWES event instance</returns>
		public Event CreateEvent(string eventName, SupportedEncoding enc)
		{
			return CreateEvent(eventName, _validate, enc);
		}

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <param name="validate">whether the event is validated</param>
		/// <returns>a new LWES event instance</returns>
		public Event CreateEvent(string eventName, bool validate)
		{
			return CreateEvent(eventName, validate, _enc);			
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

			this.TraceData(TraceEventType.Verbose, () => { return new object[] { String.Concat("CreateEvent: ", eventName
				,	Environment.NewLine, "\twith validate = ", validate
				, Environment.NewLine, "\tand encoding = ", enc) }; }
				);

			Event result;
			if (!_db.TryCreateEvent(eventName, out result, validate, enc))
			{
				this.TraceData(TraceEventType.Verbose, () => { return new object[] { String.Concat("CreateEvent, event not found in db: ", eventName) }; });

				result = new Event(new EventTemplate(false, eventName), false, _enc);
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
		/// Initializes the emitter.
		/// </summary>
		public void Initialize()
		{
			if (IsInitialized) throw new InvalidOperationException(Resources.Error_AlreadyInitialized);

			if (_status.SetStateIfLessThan(EmitterState.Initializing, EmitterState.Initializing))
			{
				try
				{
					PerformInitialization();
				}
				finally
				{
					_status.TryTransition(EmitterState.Active, EmitterState.Initializing);
				}
			}
		}

		/// <summary>
		/// Disposes of the emitter.
		/// </summary>
		/// <param name="disposing">Indicates whether the object is being disposed</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing) this.TraceData(TraceEventType.Verbose, "EventEmitterBase - disposing");
			Util.Dispose(ref _emitter);
			if (disposing) this.TraceData(TraceEventType.Verbose, "EventEmitterBase - disposed");
		}

		/// <summary>
		/// Finishes initialization of the emitter.
		/// </summary>
		/// <param name="endpoint">An IP endpoint where events will be emitted</param>
		/// <param name="finishSocket">callback method used to finish setup of the socket</param>
		protected void FinishInitialize(IPEndPoint endpoint, Action<Socket, IPEndPoint> finishSocket)
		{
			if (endpoint == null) throw new ArgumentNullException("endpoint");
			if (finishSocket == null) throw new ArgumentNullException("finishSocket");

			if (_status.CurrentState != EmitterState.Initializing)
				throw new InvalidOperationException("only valid while initialzing");

			if (_db == null) throw new InvalidOperationException("TemplateDB must be set before initialization");
			if (_encoding == null) throw new InvalidOperationException("Encoding must be set before initialization");

			IEmitter emitter = (IsParallel)
				? (IEmitter)new ParallelEmitter()
				: (IEmitter)new DirectEmitter();

			emitter.Start(_db, endpoint, finishSocket);

			_emitter = emitter;
		}

		/// <summary>
		/// Performs initialization of the emitter. Derived classes must implement this method
		/// and subsequently call the <em>FinishInitialize</em> method of the base class.
		/// </summary>
		protected abstract void PerformInitialization();

		#endregion Methods

		#region Nested Types

		class DirectEmitter : IEmitter, ITraceable
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

				byte[] bytes = LwesSerializer.Serialize(ev);
				this.TraceData(TraceEventType.Verbose, () =>
				{
					return new object[] { String.Concat("EventEmitterBase.DirectEmitter - Emitting to ", _sendToEP, ": ", ev.ToString(true)),
						String.Concat(" octets: ", Util.BytesToOctets(bytes, 0, bytes.Length)) };
				});
				_emitEP.SendTo(_sendToEP, bytes);
			}

			public void Start(IEventTemplateDB db, IPEndPoint sendToEP, Action<Socket, IPEndPoint> finishSocket)
			{
				this.TraceData(TraceEventType.Verbose, "EventEmitterBase.DirectEmitter - Starting");
				_db = db;
				_sendToEP = sendToEP;
				_buffer = BufferManager.AcquireBuffer(null);
				_emitEP = new UdpEndpoint(sendToEP).Initialize(finishSocket);
				_senderState.SetState(EmitterState.Active);
				this.TraceData(TraceEventType.Verbose, "EventEmitterBase.DirectEmitter - Started");
			}

			private void Dispose(bool disposing)
			{
				// Signal background threads...
				if (disposing) this.TraceData(TraceEventType.Verbose, "EventEmitterBase.DirectEmitter - disposing, sending stop signal");
				_senderState.TryTransition(EmitterState.StopSignaled, EmitterState.Active, () =>
					{
						Util.Dispose(ref _emitEP);
						BufferManager.ReleaseBuffer(_buffer);
						_buffer = null;
						if (disposing) this.TraceData(TraceEventType.Verbose, "EventEmitterBase.DirectEmitter - disposed");
					});
			}

			#endregion Methods
		}

		class ParallelEmitter : IEmitter, ITraceable
		{
			#region Fields

			SimpleLockFreeQueue<byte[]> _dataQueue = new SimpleLockFreeQueue<byte[]>();
			IEventTemplateDB _db;
			UdpEndpoint _emitEP;
			Status<EmitterState> _emitterState;
			EndPoint _sendToEP;
			int _senders;

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
				this.TraceData(TraceEventType.Verbose, () =>
				{
					return new object[] { String.Concat("EventEmitterBase.ParallelEmitter - Queuing for ", _sendToEP, ": ", ev.ToString(true)) };
				});
				_dataQueue.Enqueue(LwesSerializer.SerializeToMemoryBuffer(ev));
				EnsureSenderIsActive();
			}

			public void Start(IEventTemplateDB db, IPEndPoint sendToEP, Action<Socket, IPEndPoint> finishSocket)
			{
				this.TraceData(TraceEventType.Verbose, "EventEmitterBase.ParallelEmitter - Starting");
				_db = db;
				_sendToEP = sendToEP;
				_emitEP = new UdpEndpoint(sendToEP).Initialize(finishSocket);
				_emitterState.SetState(EmitterState.Active);
				this.TraceData(TraceEventType.Verbose, "EventEmitterBase.ParallelEmitter - Started");
			}

			void Background_Sender(object unused_state)
			{
				//
				// Drains the event queue and performs notification
				//
				try
				{
					byte[] data;
					while (_emitterState.IsLessThan(EmitterState.StopSignaled) && _dataQueue.TryDequeue(out data))
					{
						this.TraceData(TraceEventType.Verbose, () =>
						{
							return new object[] { String.Concat("EventEmitterBase.ParallelEmitter - Background_Sender - Sending to ", _sendToEP, " octects: ", Util.BytesToOctets(data, 0, data.Length)) };
						});
						_emitEP.SendTo(_sendToEP, data);
						BufferManager.ReleaseBuffer(data);
					}
				}
				finally
				{
					int z = Interlocked.Decrement(ref _senders);
					if (_emitterState.IsLessThan(EmitterState.StopSignaled))
						this.TraceData(TraceEventType.Verbose, "EventEmitterBase.ParallelEmitter - Background_Sender stopped because queue is empty");
					else
						this.TraceData(TraceEventType.Verbose, "EventEmitterBase.ParallelEmitter - Background_Sender stopped sending because it was signaled to stop");

					if (z == 0 && _emitterState.IsLessThan(EmitterState.StopSignaled) && !_dataQueue.IsEmpty)
						EnsureSenderIsActive();
				}
			}

			private void Dispose(bool disposing)
			{
				if (disposing) this.TraceData(TraceEventType.Verbose, "EventEmitterBase.ParallelEmitter - disposing, sending stop signal");
				// Signal background threads...
				_emitterState.TryTransition(EmitterState.StopSignaled, EmitterState.Active, () =>
				{
					while (Thread.VolatileRead(ref _senders) > 0)
					{
						Thread.Sleep(CDisposeBackgroundThreadWaitTimeMS);
					}
					byte[] b;
					while (_dataQueue.TryDequeue(out b))
					{
						BufferManager.ReleaseBuffer(b);
					}
					Util.Dispose(ref _emitEP);
					if (disposing) this.TraceData(TraceEventType.Verbose, "EventEmitterBase.ParallelEmitter - disposed");
				});
			}

			private void EnsureSenderIsActive()
			{
				var value = Thread.VolatileRead(ref _senders);
				if (value <= 0)
				{
					WaitCallback cb = new WaitCallback(Background_Sender);
					if (Interlocked.CompareExchange(ref _senders, value + 1, value) == value)
					{
						this.TraceData(TraceEventType.Verbose, "EventEmitterBase.ParallelEmitter - Background_Sender started on demand");
						ThreadPool.QueueUserWorkItem(cb);
					}
				}
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}