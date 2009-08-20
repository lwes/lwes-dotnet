namespace Org.Lwes.Listener
{
	using System;
	using System.Net;

	/// <summary>
	/// Interface for event sinks that handle events and data received by the event listeners.
	/// </summary>
	public interface IEventSink
	{
		/// <summary>
		/// Indicates whether the event sink is thread-safe.
		/// </summary>
		/// <remarks>A sink is thread-safe if BOTH HandleEventArrival and HandleGarbageData
		/// can be called multiple times concurrently without failure.</remarks>
		bool IsThreadSafe
		{
			get;
		}

		/// <summary>
		/// Callback method invoked by event listeners when LWES events arrive.
		/// </summary>
		/// <param name="key">Registration key for controlling the registration and status of
		/// an event sink</param>
		/// <param name="ev">a newly arrived LWES event</param>
		void HandleEventArrival(IEventSinkRegistrationKey key, Event ev);

		/// <summary>
		/// Callback method invoked by event listeners when garbage data arrives on an endpoint.
		/// </summary>
		/// <param name="key">Registration key for controlling the registration and status of
		/// an event sink</param>
		/// <param name="remoteEndPoint">endpoint that sent the garbage data</param>
		/// <param name="priorGarbageCountForEndpoint">number of times garbage data has arrived from the
		/// endpoint</param>
		/// <param name="garbage">byte array containing the garbage</param>
		/// <returns>a vote as to how future traffic from the endpoint should be handled</returns>
		GarbageHandlingVote HandleGarbageData(IEventSinkRegistrationKey key, EndPoint remoteEndPoint, int priorGarbageCountForEndpoint, byte[] garbage);
	}

}