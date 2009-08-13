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
		public void Initialize(IEventTemplateDB db
			, IPAddress multicastAddress
			, int multicastPort
			, bool parallel)
		{
			if (IsInitialized) throw new InvalidOperationException(Resources.Error_AlreadyInitialized);

			base.Initialize(db, new IPEndPoint(multicastAddress, multicastPort), parallel,
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