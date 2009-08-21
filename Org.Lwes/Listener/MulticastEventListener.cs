﻿// 
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
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;

	using Org.Lwes.DB;
	using Org.Lwes.Properties;

	/// <summary>
	/// Implementation of the IEventListener for listening to multicast events.
	/// </summary>
	public sealed class MulticastEventListener : EventListenerBase
	{
		#region Methods

		/// <summary>
		/// Initializes the instance.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="multicastAddress"></param>
		/// <param name="multicastPort"></param>
		/// <param name="parallel"></param>
		/// <param name="garbageHandling"></param>
		public void Initialize(IEventTemplateDB db
			, IPAddress multicastAddress
			, int multicastPort
			, bool parallel
			, ListenerGarbageHandling garbageHandling)
		{
			if (IsInitialized) throw new InvalidOperationException(Resources.Error_AlreadyInitialized);

			base.Initialize(db, new IPEndPoint(multicastAddress, multicastPort), parallel, garbageHandling,
				(s, e) =>
				{
					s.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);
					s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
					if (e.AddressFamily == AddressFamily.InterNetworkV6)
					{
						s.Bind(new IPEndPoint(IPAddress.IPv6Any, e.Port));
						s.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership,
								new IPv6MulticastOption(e.Address));
					}
					else
					{
						s.Bind(new IPEndPoint(IPAddress.Any, e.Port));
						s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
								new MulticastOption(e.Address));
					}
				});
		}

		#endregion Methods
	}
}