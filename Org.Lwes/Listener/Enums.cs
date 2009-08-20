namespace Org.Lwes.Listener
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	#region Enumerations

	/// <summary>
	/// Represents the state of an event sink.
	/// </summary>
	public enum EventSinkStatus
	{
		/// <summary>
		/// Indicates the event sink has been suspended and should not receive
		/// events until activated. This is the initial state for new IEventSinkRegistrationKeys.
		/// </summary>
		Suspended = 0,
		/// <summary>
		/// Indicates the event sink is active and may be notified of events
		/// at any time.
		/// </summary>
		Active = 1,
		/// <summary>
		/// For event sinks that are not thread-safe, indicates that the sink
		/// is currently being invoked.
		/// </summary>
		Notifying = 2,
		/// <summary>
		/// Indicates the event sink has been canceled.
		/// </summary>
		Canceled = 3
	}

	/// <summary>
	/// Strategies to be taken when garbage data arrives from an endpoint.
	/// </summary>
	public enum GarbageHandlingVote
	{
		/// <summary>
		/// No opinion on garbage handling. Continue as normal.
		/// </summary>
		None,
		/// <summary>
		/// Causes all traffic from an endpoint to be handled by the event
		/// sink's HandleGarbageData.
		/// </summary>
		TreatTrafficFromEndpointAsGarbage,
		/// <summary>
		/// Causes any data received from the endpoint to be discarded as soon
		/// as possible.
		/// </summary>
		IgnoreAllTrafficFromEndpoint,
		/// <summary>
		/// The default strategy. Same as None
		/// </summary>
		Default = None
	}

	/// <summary>
	/// Enumeration for garbage handling strategy employed by Listeners.
	/// </summary>
	public enum ListenerGarbageHandling
	{
		/// <summary>
		/// Indicates the listener should silently handle garbage data
		/// but continue to accept data from all endpoints.
		/// </summary>
		FailSilently = 0,
		/// <summary>
		/// Indicates the listener should ignore data from endpoints
		/// from which garbage has been received.
		/// </summary>
		IgnoreEndpointsThatSendGarbage = 1,
		/// <summary>
		/// Indicates the listener should ask the event sinks to vote
		/// on the strategy on a per-endpoint basis.
		/// </summary>
		/// <see cref="Org.Lwes.Listener.IEventSink"/>
		AskEventSinksToVoteOnStrategy = 2,
		/// <summary>
		/// The default value: FailSilently
		/// </summary>
		Default = FailSilently
	}

	#endregion Enumerations
}