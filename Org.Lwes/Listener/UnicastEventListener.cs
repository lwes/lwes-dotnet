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
	using System.Net.Sockets;

	using Org.Lwes.DB;
	using Org.Lwes.Properties;

	/// <summary>
	/// Implementation of the IEventListener for listening to multicast events.
	/// </summary>
	public sealed class UnicastEventListener : EventListenerBase
	{
		#region Methods

		/// <summary>
		/// Initializes the event listener.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="address"></param>
		/// <param name="port"></param>
		/// <param name="parallel"></param>
		/// <param name="garbageHandling"></param>
		public void Initialize(IEventTemplateDB db
			, IPAddress address
			, int port
			, bool parallel
			, ListenerGarbageHandling garbageHandling)
		{
			TemplateDB = db;
			Address = address;
			Port = port;
			IsParallel = parallel;
			Initialize();
		}

		/// <summary>
		/// Initializes the listener on a unicast address.
		/// </summary>
		protected override void PerformInitialization()
		{
			base.FinishInitialize(new IPEndPoint(Address, Port),
				(s, e) =>
				{
					s.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);
					s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
					s.Bind(e);
				});
		}

		#endregion Methods
	}
}