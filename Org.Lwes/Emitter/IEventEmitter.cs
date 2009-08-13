namespace Org.Lwes.Emitter
{
	using System;
	using System.IO;
	using System.Text;

	using Org.Lwes.DB;
	using Org.Lwes.Factory;

	/// <summary>
	/// Interface for classes that emit Events into the light-weight event system.
	/// </summary>
	/// <remarks>
	/// This interface makes the use of the IDisposable pattern explicit; implementations 
	/// must guarantee that cleanup occured before returning from Dispose.
	/// </remarks>
	public interface IEventEmitter : IDisposable
	{
		#region Properties

		/// <summary>
		/// The character encoding used when performing event IO.
		/// </summary>
		Encoding Encoding
		{
			get;
		}

		/// <summary>
		/// Indicates whether the emitter has been initialized.
		/// </summary>
		bool IsInitialized
		{
			get;
		}

		/// <summary>
		/// The event template database used by the emitter.
		/// </summary>
		IEventTemplateDB TemplateDB
		{
			get;
		}

		/// <summary>
		/// Indicates whether events issued from the emitter will validate
		/// when they are written to.
		/// </summary>
		bool Validate
		{
			get;
		}

		#endregion Properties

		#region Methods

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
		/// Emits an event to the event system.
		/// </summary>
		/// <param name="evt">the event being emitted</param>
		void Emit(Event evt);

		#endregion Methods
	}
}