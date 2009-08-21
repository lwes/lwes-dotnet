//
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT© 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
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
// GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes.Listener
{
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