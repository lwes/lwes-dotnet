namespace Org.Lwes.Factory
{
	using System.Text;

	using Org.Lwes.DB;

	/// <summary>
	/// Interface for event factories. 
	/// </summary>
	public interface IEventFactory
	{
		/// <summary>
		/// The character encoding used when performing event IO.
		/// </summary>
		Encoding Encoding
		{
			get;
		}

		/// <summary>
		/// Indicates whether the factory has been initialized.
		/// </summary>
		bool IsInitialized
		{
			get;
		}

		/// <summary>
		/// The event template database used by the factory.
		/// </summary>
		IEventTemplateDB TemplateDB
		{
			get;
		}

		/// <summary>
		/// Indicates whether events issued from the factory will validate
		/// when they are written to.
		/// </summary>
		bool Validate
		{
			get;
		}

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <returns>a new LWES event instance</returns>
		Event CreateEvent(string eventName);

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <param name="enc">encoding used when performing IO on the event</param>
		/// <returns>a new LWES event instance</returns>
		Event CreateEvent(string eventName, SupportedEncoding enc);

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <param name="validate">whether the event is validated</param>
		/// <returns>a new LWES event instance</returns>
		Event CreateEvent(string eventName, bool validate);

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <param name="validate">whether the event is validated</param>
		/// <param name="enc">encoding used when performing IO on the event</param>
		/// <returns>a new LWES event instance</returns>
		Event CreateEvent(string eventName, bool validate, SupportedEncoding enc);

		/// <summary>
		/// Initializes the factory.
		/// </summary>
		/// <param name="enc">encoding for events</param>
		/// <param name="performValidation">Indicates whether validation should occur by default</param>
		/// <param name="db">event template database</param>
		void Initialize(SupportedEncoding enc, bool performValidation, IEventTemplateDB db);
	}
}