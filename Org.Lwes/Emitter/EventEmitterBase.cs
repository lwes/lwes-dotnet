namespace Org.Lwes.Emitter
{
	using System;
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

		SupportedEncoding _enc;
		Encoding _encoding;
		bool _initialized;
		bool _performValidation;
		IEventTemplateDB _templateDB;

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
			get { return _initialized; }
		}

		/// <summary>
		/// The event template database used by the factory.
		/// </summary>
		public IEventTemplateDB TemplateDB
		{
			get { return _templateDB; }
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
			if (!_initialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
			if (eventName == null) throw new ArgumentNullException("eventName");
			if (eventName.Length == 0) throw new ArgumentException(Resources.Error_EmptyStringNotAllowed, "eventName");

			Event result;
			if (!_templateDB.TryCreateEvent(eventName, out result, _performValidation, _enc))
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
			if (!_initialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
			if (eventName == null) throw new ArgumentNullException("eventName");
			if (eventName.Length == 0) throw new ArgumentException(Resources.Error_EmptyStringNotAllowed, "eventName");

			Event result;
			if (!_templateDB.TryCreateEvent(eventName, out result, _performValidation, enc))
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
			if (!_initialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
			if (eventName == null) throw new ArgumentNullException("eventName");
			if (eventName.Length == 0) throw new ArgumentException(Resources.Error_EmptyStringNotAllowed, "eventName");

			Event result;
			if (!_templateDB.TryCreateEvent(eventName, out result, validate, _enc))
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
			if (!_initialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
			if (eventName == null) throw new ArgumentNullException("eventName");
			if (eventName.Length == 0) throw new ArgumentException(Resources.Error_EmptyStringNotAllowed, "eventName");

			Event result;
			if (!_templateDB.TryCreateEvent(eventName, out result, validate, enc))
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
		public abstract void Emit(Event evt);

		/// <summary>
		/// Initializes the emitter.
		/// </summary>
		/// <param name="enc">encoding the emitter should use for character data.</param>
		/// <param name="performValidation">whether the emitter validates emitted messages</param>
		/// <param name="db">a template DB to use when creating events</param>
		public void Initialize(SupportedEncoding enc, bool performValidation, IEventTemplateDB db)
		{
			if (db == null) throw new ArgumentNullException("db");

			_enc = enc;
			_encoding = Constants.GetEncoding((short)enc);
			_performValidation = performValidation;
			_templateDB = db;

			_initialized = true;
		}

		/// <summary>
		/// Ensures the emitter has been initialized.
		/// </summary>
		/// <exception cref="InvalidOperationException">thrown if the emitter has not yet been initialized.</exception>
		protected void CheckInitialized()
		{
			if (!_initialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
		}

		/// <summary>
		/// Disposes of the emitter.
		/// </summary>
		/// <param name="disposing">Indicates whether the object is being disposed</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		#endregion Methods
	}
}