﻿//
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
	using System.Net;

	/// <summary>
	/// Interface for classes that handle the arrival of LWES events.
	/// </summary>
	public interface IEventSink
	{
		/// <summary>
		/// Callback method invoked by event listeners when LWES events arrive.
		/// </summary>
		/// <param name="key">Registration key for controlling the registration and status of
		/// an event sink</param>
		/// <param name="ev">a newly arrived LWES event</param>
		/// <returns><em>true</em> if the listener should continue notifying the sink-chain; otherwise <em>false</em></returns>
		bool HandleEventArrival(ISinkRegistrationKey key, Event ev);
	}
}