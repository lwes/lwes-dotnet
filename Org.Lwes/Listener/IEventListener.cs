// 
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT (C) 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
//   original .NET implementation
// 
// LWES.net is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// LWES.net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
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