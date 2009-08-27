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
using System.Net;

namespace Org.Lwes.Listener
{
	/// <summary>
	/// Interface for handling garbage data received by the event listeners.
	/// </summary>
	public interface IGarbageSink
	{
		/// <summary>
		/// Callback method invoked by event listeners when garbage data arrives on an endpoint.
		/// </summary>
		/// <param name="key">Registration key for controlling the registration and status of
		/// an event sink</param>
		/// <param name="remoteEndPoint">endpoint that sent the garbage data</param>
		/// <param name="priorGarbageCountForEndpoint">number of times garbage data has arrived from the
		/// endpoint</param>
		/// <param name="garbage">byte array containing the garbage</param>
		/// <returns>a vote as to how future traffic from the endpoint should be handled -- possible values are: 
		/// { None | TreatTrafficFromEndpointAsGarbage | IgnoreAllTrafficFromEndpoint }</returns>
		/// <remarks>
		/// <para>Whether a listener invokes garbage handling on the sink is dictated by the listner's
		/// garbage handling setting --  { <em>FailSilently</em> | <em>IgnoreEndpointsThatSendGarbage</em> | <em>AskEventSinksToVoteOnStrategy</em> }.</para>
		/// <para>If the listener's garbage handling setting is <em>AskEventSinksToVoteOnStrategy</em> then the
		/// the sinks are asked to vote.</para>
		/// <para>Votes are recorded on a per-remote endpoint basis. Upon Each active sink
		/// is given a copy of the garbage data and the listener records the sink's vote. The most 
		/// restrictive vote dictates the future data handling for the remote endpoint.</para></remarks>
		GarbageHandlingVote HandleGarbageData(ISinkRegistrationKey key, EndPoint remoteEndPoint, int priorGarbageCountForEndpoint, byte[] garbage);
	}
}
