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
	using System.Net;
	using System.Net.Sockets;

	using Org.Lwes.DB;
	using System;
	using Org.Lwes.Trace;
	using System.Diagnostics;

	/// <summary>
	/// Event emitter implementation for multicasting the event.
	/// </summary>
	public class MulticastEventEmitter : EventEmitterBase, ITraceable
	{
		#region Properties

		/// <summary>
		/// The time-to-live setting for the multicast traffic.
		/// </summary>
		public int MulticastTimeToLive
		{
			get; private set;
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Initializes the emitter.
		/// </summary>
		/// <param name="enc">indicates the encoding to user for character data</param>
		/// <param name="validate">indicates whether the emitter performs validation</param>
		/// <param name="db">tempate db used to create events</param>
		/// <param name="multicastAddress">a multicast address where events will be emitted</param>
		/// <param name="multicastPort">a multicast port where events will be emitted</param>
		/// <param name="multicastTtl">the time-to-live used during multicast</param>
		/// <param name="parallel">indicates whether the emitter can use a parallel strategy
		/// when emitting</param>
		public void InitializeAll(SupportedEncoding enc, bool validate, IEventTemplateDB db
			, IPAddress multicastAddress
			, int multicastPort
			, int multicastTtl
			, bool parallel)
		{
			this.TraceData(TraceEventType.Verbose, () =>
			{
				return new object[] { String.Concat("MulticastEventEmitter Initializing"
					, Environment.NewLine, "\twith validate = ", validate
					, Environment.NewLine, "\tand encoding = ", enc
					, Environment.NewLine, "\tand multicastAddress = ", multicastAddress
					, Environment.NewLine, "\tand multicastPort = ", multicastPort
					, Environment.NewLine, "\tand multicastTtl = ", multicastTtl
					, Environment.NewLine, "\tand parallel = ", parallel
					) };
			});

			Encoding = enc;
			Validate = validate;
			TemplateDB = db;
			Address = multicastAddress;
			Port = multicastPort;
			MulticastTimeToLive = multicastTtl;
			IsParallel = parallel;
			base.Initialize();

			this.TraceData(TraceEventType.Verbose, "MulticastEventEmitter Initialized");			
		}

		/// <summary>
		/// Performs initialization for multicast.
		/// </summary>
		protected override void PerformInitialization()
		{
			base.FinishInitialize(new IPEndPoint(Address, Port),
				(s, e) =>
				{
					s.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);
					s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
					s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, MulticastTimeToLive);
					s.Connect(e); // TODO: Verify whether this is necessary when using Socket.SendTo().
				});
		}

		#endregion Methods

		public static MulticastEventEmitter CreateInitialized(SupportedEncoding enc
			, bool validate
			, IEventTemplateDB db
			, IPAddress multicastAddress
			, int multicastPort
			, int multicastTtl
			, bool parallel)
		{
			if (db == null) throw new ArgumentNullException("db", "db cannot be null");
			var result = new MulticastEventEmitter();
			result.InitializeAll(enc, validate, db, multicastAddress, multicastPort, multicastTtl, parallel);
			return result;
		}

		public static MulticastEventEmitter CreateInitialized(
			IPAddress multicastAddress
			, int multicastPort
			, int multicastTtl
			, bool parallel)
		{
			return CreateInitialized(SupportedEncoding.Default
				, false
				, EventTemplateDB.CreateDefault()
				, multicastAddress
				, multicastPort
				, multicastTtl
				, parallel);			
		}
	}
}