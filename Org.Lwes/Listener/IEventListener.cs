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
	using System;
	using System.Net;

	/// <summary>
	/// Interface for event listener implementations.
	/// </summary>
	public interface IEventListener : IDisposable
	{
		event OnLwesEventArrived OnEventArrived;
		event OnLwesGarbageArrived OnGarbageArrived;

		/// <summary>
		/// Initializes the listener.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">thrown if the listener has already been initialized</exception>
		void Initialize();

		/// <summary>
		/// The ip address where the listener will listen.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">thrown if the listener has already been initialized</exception>
		IPAddress Address
		{
			get;
			set;
		}

		/// <summary>
		/// The ip port where the listener will listen.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">thrown if the listener has already been initialized</exception>
		int Port
		{
			get;
			set;
		}

		/// <summary>
		/// Registers an event sink and activates it.
		/// </summary>
		/// <param name="sink">the sink to register</param>
		/// <param name="handback">a handback object - this object is opaque to the listener
		/// and will be attached to the registration key prior to activation</param>
		/// <returns>A registration key for the sink.</returns>
		ISinkRegistrationKey RegisterAndActivateEventSink(IEventSink sink, object handback);

		/// <summary>
		/// Registers an event sink with the listener without activating the
		/// event sink.
		/// </summary>
		/// <param name="sink">the event sink to register</param>
		/// <returns>A registration key for the event sink</returns>
		/// <remarks>Sinks will not begin to recieve event notification 
		/// until <em>after</em> the registration key's <em>Activate</em> 
		/// method is called.</remarks>
		ISinkRegistrationKey RegisterEventSink(IEventSink sink);

		/// <summary>
		/// Registers a garbage sink and activates it.
		/// </summary>
		/// <param name="sink">the garbage sink to register</param>
		/// <param name="handback">a handback object - this object is opaque to the listener
		/// and will be attached to the registration key prior to activation</param>
		/// <returns>A registration key for the sink.</returns>
		ISinkRegistrationKey RegisterAndActivateGarbageSink(IGarbageSink sink, object handback);

		/// <summary>
		/// Registers a garbage sink with the listener without activating.
		/// </summary>
		/// <param name="sink">the sink to register</param>
		/// <returns>A registration key for the sink</returns>
		/// <remarks>Sinks will not begin to recieve garbage notification 
		/// until <em>after</em> the registration key's <em>Activate</em> 
		/// method is called.</remarks>
		ISinkRegistrationKey RegisterGarbageSink(IGarbageSink sink);
	}
}