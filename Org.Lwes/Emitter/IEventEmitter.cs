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
namespace Org.Lwes.Emitter
{
	using System;
	using System.Net;
	using System.Text;

	using Org.Lwes.DB;

	/// <summary>
	/// Interface for classes that emit Events into the light-weight event system.
	/// </summary>
	/// <remarks>
	/// This interface makes the use of the IDisposable pattern explicit; implementations 
	/// must guarantee that cleanup occured before returning from Dispose.
	/// </remarks>
	public interface IEventEmitter : IDisposable
	{
		/// <summary>
		/// Initializes the emitter.
		/// </summary>
		void Initialize();

		/// <summary>
		/// The ip address to which events are emitted.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">thrown if the emitter has already been initialized</exception>
		IPAddress Address
		{
			get; set;
		}

		/// <summary>
		/// The character encoding used when performing event IO.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">thrown if the emitter has already been initialized</exception>
		SupportedEncoding Encoding
		{
			get;
			set;
		}

		/// <summary>
		/// Indicates whether the emitter has been initialized.
		/// </summary>
		bool IsInitialized
		{
			get;
		}

		/// <summary>
		/// The ip port to which events are emitted.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">thrown if the emitter has already been initialized</exception>
		int Port
		{
			get; set;
		}

		/// <summary>
		/// The event template database used by the emitter.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">thrown if the emitter has already been initialized</exception>
		IEventTemplateDB TemplateDB
		{
			get;
			set;
		}

		/// <summary>
		/// Indicates whether events issued from the emitter will validate
		/// event contents.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">thrown if the emitter has already been initialized</exception>
		bool Validate
		{
			get;
			set;
		}

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <returns>a new LWES event instance</returns>
		/// <exception cref="System.InvalidOperationException">thrown if the emitter has not been initialized</exception>
		Event CreateEvent(string eventName);

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <param name="enc">encoding used when performing IO on the event</param>
		/// <returns>a new LWES event instance</returns>
		/// <exception cref="System.InvalidOperationException">thrown if the emitter has not been initialized</exception>
		Event CreateEvent(string eventName, SupportedEncoding enc);

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <param name="validate">whether the event is validated</param>
		/// <returns>a new LWES event instance</returns>
		/// <exception cref="System.InvalidOperationException">thrown if the emitter has not been initialized</exception>
		Event CreateEvent(string eventName, bool validate);

		/// <summary>
		/// Creates an event type identified by the event name.
		/// </summary>
		/// <param name="eventName">the event type's name</param>
		/// <param name="validate">whether the event is validated</param>
		/// <param name="enc">encoding used when performing IO on the event</param>
		/// <returns>a new LWES event instance</returns>
		/// <exception cref="System.InvalidOperationException">thrown if the emitter has not been initialized</exception>
		Event CreateEvent(string eventName, bool validate, SupportedEncoding enc);

		/// <summary>
		/// Emits an event to the event system.
		/// </summary>
		/// <param name="evt">the event being emitted</param>
		/// <exception cref="System.InvalidOperationException">thrown if the emitter has not been initialized</exception>
		void Emit(Event evt);
	}
}