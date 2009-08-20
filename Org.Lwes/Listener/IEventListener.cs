namespace Org.Lwes.Listener
{
	using System;
	using System.Net;

	/// <summary>
	/// Interface for event listener implementations.
	/// </summary>
	public interface IEventListener : IDisposable
	{
		/// <summary>
		/// Registers an event sink and activates it.
		/// </summary>
		/// <param name="sink">the event sink to register</param>
		/// <param name="handback">a handback object - this object is opaque to the listener
		/// and will be attached to the registration key prior to activation</param>
		/// <returns>A registration key for the event sink.</returns>
		IEventSinkRegistrationKey RegisterAndActivateEventSink(IEventSink sink, object handback);

		/// <summary>
		/// Registers an event sink with the listener without activating the
		/// event sink.
		/// </summary>
		/// <param name="sink">the event sink to register</param>
		/// <returns>A registration key for the event sink</returns>
		/// <remarks>Event sinks will not begin to recieve event or garbage
		/// notification until *after* the registration key's Active method
		/// is called.</remarks>
		IEventSinkRegistrationKey RegisterEventSink(IEventSink sink);
	}
}