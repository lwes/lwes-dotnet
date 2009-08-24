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
// Lesser GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes.Emitter
{
	using System.Net;
	using System.Net.Sockets;

	using Org.Lwes.DB;

	/// <summary>
	/// Event emitter implementation for multicasting the event.
	/// </summary>
	public class MulticastEventEmitter : EventEmitterBase
	{
		#region Methods

		/// <summary>
		/// Initializes the emitter.
		/// </summary>
		/// <param name="enc">indicates the encoding to user for character data</param>
		/// <param name="performValidation">indicates whether the emitter performs validation</param>
		/// <param name="db">tempate db used to create events</param>
		/// <param name="multicastAddress">a multicast address where events will be emitted</param>
		/// <param name="multicastPort">a multicast port where events will be emitted</param>
		/// <param name="multicastTtl">the time-to-live used during multicast</param>
		/// <param name="useParallelEmit">indicates whether the emitter should parallelize the
		/// event emitting</param>
		public void Initialize(SupportedEncoding enc, bool performValidation, IEventTemplateDB db
			, IPAddress multicastAddress
			, int multicastPort
			, int multicastTtl
			, bool useParallelEmit)
		{
			base.Initialize(enc, performValidation, db, new IPEndPoint(multicastAddress, multicastPort),
				useParallelEmit, (s, e) =>
				{
					s.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);
					s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
					s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, multicastTtl);
					s.Connect(e); // TODO: Verify whether this is necessary when using Socket.SendTo().
				});
		}

		#endregion Methods
	}
}