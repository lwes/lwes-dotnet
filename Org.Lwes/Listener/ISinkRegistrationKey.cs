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

	/// <summary>
	/// Registration key for event sinks.
	/// </summary>
	public interface ISinkRegistrationKey
	{
		/// <summary>
		/// An opaque handback object.
		/// </summary>
		Object Handback
		{
			get;
			set;
		}

		/// <summary>
		/// The listener with which the event sink is registered.
		/// </summary>
		IEventListener Listener
		{
			get;
		}

		/// <summary>
		/// The status of the event sink.
		/// </summary>
		SinkStatus Status
		{
			get;
		}

		/// <summary>
		/// Activates the event sink.
		/// </summary>
		/// <returns><em>true</em> if the event sink has not already been canceled; otherwise <em>false</em></returns>
		bool Activate();

		/// <summary>
		/// Cancels a registration. An event sink whose registration is canceled will no longer
		/// receive event or garbage notification.
		/// </summary>
		void Cancel();

		/// <summary>
		/// Suspends the event sink. An event sink whose registration is suspended will not
		/// receive event or garbage notification until it is re-activated.
		/// </summary>
		/// <returns><em>true</em> if the event sink has not already been canceled; otherwise <em>false</em></returns>
		bool Suspend();
	}
}