namespace Org.Lwes.Listener
{
	using System;

	#region Delegates

	/// <summary>
	/// Delegate for event-arrival handlers.
	/// </summary>
	/// <param name="sender">The event listener that received the event.</param>
	/// <param name="ev">The event</param>
	public delegate void HandleEventArrival(IEventListener sender, Event ev);

	#endregion Delegates

	/// <summary>
	/// Interface for event listener implementations.
	/// </summary>
	public interface IEventListener : IDisposable
	{
		#region Events

		/// <summary>
		/// .NET language event signaled when a new LWES event arrives at the listener.
		/// </summary>
		event HandleEventArrival OnEventArrival;

		#endregion Events
	}
}