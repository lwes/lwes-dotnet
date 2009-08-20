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
			if (IsInitialized) throw new InvalidOperationException(Resources.Error_AlreadyInitialized);

			base.Initialize(db, new IPEndPoint(address, port), parallel, garbageHandling,
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