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
		#region Fields

		int _emitterState;
		IPEndPoint _endpoint;

		/// <summary>
		/// Indicates whether emitting will be performed in parallel;
		/// otherwise Emit(Event) is a blocking call.
		/// </summary>
		bool _parallel;
		UdpEndpoint _udp;

		#endregion Fields

		#region Enumerations

		enum EmitterState
		{
			Unknown = 0,
			Active = 1,
			Disposing = 2,
			Disposed = 3
		}

		#endregion Enumerations

		#region Properties

		/// <summary>
		/// Indicates whether the factory has been initialized.
		/// </summary>
		public override bool IsInitialized
		{
			get {	return base.IsInitialized && _udp != null;	}
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Emits an event to the event system.
		/// </summary>
		/// <param name="evt">the event being emitted</param>
		public override void Emit(Event evt)
		{
			if (_udp == null) throw new InvalidOperationException(Resources.Error_NotYetInitialized);

			if (_parallel)
			{
			}
			else
			{
				if (Thread.VolatileRead(ref _emitterState) > (int)EmitterState.Active)
					throw new InvalidOperationException(Resources.Error_EmitterHasEnteredShutdownState);

				_udp.SendTo(_endpoint, LwesSerializer.Serialize(evt));
			}
		}

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
			if (_udp != null) throw new InvalidOperationException(Resources.Error_AlreadyInitialized);

			base.Initialize(enc, performValidation, db);

			_parallel = useParallelEmit;

			_endpoint = new IPEndPoint(multicastAddress, multicastPort);
			_udp = new UdpEndpoint(_endpoint);
			_udp.Initialize((s, e) =>
				{
					s.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);
					s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
					s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, multicastTtl);
					s.Connect(e); // TODO: Verify whether this is necessary when using Socket.SendTo().
				});
			Thread.VolatileWrite(ref _emitterState, (int)EmitterState.Active);
		}

		/// <summary>
		/// Overrides the inherited Dispose method. Closes and disposes the underlying socket.
		/// </summary>
		/// <param name="disposing">Indicates whether the call originated from the Dispose method.</param>
		protected override void Dispose(bool disposing)
		{
			if (Interlocked.CompareExchange(ref _emitterState, (int)EmitterState.Disposing, (int)EmitterState.Active) == (int)EmitterState.Active)
			{
				base.Dispose(disposing);

				Util.Dispose(ref _udp);

				Thread.VolatileWrite(ref _emitterState, (int)EmitterState.Disposed);
			}
		}

		#endregion Methods
	}
}