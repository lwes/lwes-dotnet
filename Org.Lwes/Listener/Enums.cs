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
namespace Org.Lwes.Listener
{
	#region Enumerations

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

	/// <summary>
	/// Represents the state of an event sink.
	/// </summary>
	public enum SinkStatus
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

	#endregion Enumerations
}