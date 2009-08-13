namespace Org.Lwes.Emitter
{
	using System;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;
	using System.Threading;

	using Org.Lwes.DB;
	using Org.Lwes.Properties;

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