// 
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT (C) 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
//   original .NET implementation
// 
// LWES.net is free software: you can redistribute it and/or modify
// it under the terms of the Lesser GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// LWES.net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes
{
	using System;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;

	/// <summary>
	/// Encapsulates the state of an operation that occurred
	/// in parallel on an EndPoint.
	/// </summary>
	internal struct EndPointOpState
	{
		#region Fields

		internal Func<EndPointOpState, bool> Callback;
		internal SocketAsyncEventArgs SocketArgs;

		object _handback;

		#endregion Fields

		#region Constructors

		internal EndPointOpState(SocketAsyncEventArgs args, Func<EndPointOpState, bool> cb, object hb)
		{
			SocketArgs = args;
			Callback = cb;
			_handback = hb;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Accesses the raw byte buffer containing data. For SendXXX operations this
		/// is the data sent; for ReceiveXXX operations this is the data received.
		/// </summary>
		public byte[] Buffer
		{
			get { return SocketArgs.Buffer; }
		}

		/// <summary>
		/// Number of bytes used in the operation.
		/// </summary>
		public int BytesTransferred
		{
			get { return SocketArgs.BytesTransferred; }
		}

		/// <summary>
		/// Handback object given by the initiator of the operation.
		/// </summary>
		public object Handback
		{
			get { return _handback; }
		}

		/// <summary>
		/// Offset within the buffer where valid data begins. WARNING: May not be the beginning of the buffer.
		/// </summary>
		public int Offset
		{
			get { return SocketArgs.Offset; }
		}

		/// <summary>
		/// Identifies the operation that was performed.
		/// </summary>
		/// <see cref="System.Net.Sockets.SocketAsyncOperation"/>
		public SocketAsyncOperation Operation
		{
			get { return SocketArgs.LastOperation; }
		}

		/// <summary>
		/// Identifies the remote side of the communication.
		/// </summary>
		public EndPoint RemoteEndPoint
		{
			get { return SocketArgs.RemoteEndPoint; }
		}

		/// <summary>
		/// Identifies the socket error. NOTE: SocketError.Success indicates no error occurred!
		/// </summary>
		public SocketError SocketError
		{
			get { return SocketArgs.SocketError; }
		}

		#endregion Properties
	}

	/// <summary>
	/// Socket endpoint adapter for working with UDP.
	/// </summary>
	internal class UdpEndpoint : IDisposable
	{
		#region Fields

		IPEndPoint _endpoint;
		Socket _socket;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="endpoint">endpoint to be adapted</param>
		public UdpEndpoint(IPEndPoint endpoint)
		{
			if (endpoint == null) throw new ArgumentNullException("endpoint");

			_endpoint = endpoint;
		}

		~UdpEndpoint()
		{
			Dispose(false);
		}

		#endregion Constructors

		#region Properties

		public EndPoint EndPoint
		{
			get { return _endpoint; }
		}

		#endregion Properties

		#region Methods

		public void ContinueReceiveFromAsync(EndPointOpState state, int offset, int count)
		{
			state.SocketArgs.SetBuffer(state.SocketArgs.Buffer, offset, count);
			if (!_socket.ReceiveFromAsync(state.SocketArgs))
			{
				// If the callback reuses the args (indicated by return value of true)
				// then we return without queuing the args.
				if (state.Callback != null
						&& state.Callback(new EndPointOpState(state.SocketArgs
							, state.Callback
							, state.Handback
							)))
					return;
			}
		}

		public void ContinueReceiveFromAsync(EndPointOpState state, byte[] buffer)
		{
			state.SocketArgs.SetBuffer(buffer, 0, buffer.Length);
			if (!_socket.ReceiveFromAsync(state.SocketArgs))
			{
				// If the callback reuses the args (indicated by return value of true)
				// then we return without queuing the args.
				if (state.Callback != null
						&& state.Callback(new EndPointOpState(state.SocketArgs
							, state.Callback
							, state.Handback
							)))
					return;
			}
		}

		public void ContinueSendToAsync(EndPointOpState state, int offset, int count)
		{
			state.SocketArgs.SetBuffer(state.SocketArgs.Buffer, offset, count);
			if (!_socket.SendToAsync(state.SocketArgs))
			{
				// If the callback reuses the args (indicated by return value of true)
				// then we return without queuing the args.
				if (state.Callback != null
						&& state.Callback(new EndPointOpState(state.SocketArgs
							, state.Callback
							, state.Handback
							)))
					return;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public UdpEndpoint Initialize(Action<Socket, IPEndPoint> finishSocket)
		{
			if (finishSocket == null) throw new ArgumentNullException("finishSocket");

			_socket = new Socket(_endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
			finishSocket(_socket, _endpoint);
			return this;
		}

		public int ReceiveFrom(ref EndPoint endpoint, byte[] data, int offset, int count)
		{
			return _socket.ReceiveFrom(data, offset, count, SocketFlags.None, ref endpoint);
		}

		public void ReceiveFromAsync(EndPoint endpoint, byte[] data, int offset, int count, Func<EndPointOpState, bool> callback, object handback)
		{
			SocketAsyncEventArgs args = InitArgs();
			args.RemoteEndPoint = endpoint;
			args.SetBuffer(data, offset, count);
			args.UserToken = new Completion(callback, handback);
			if (!_socket.ReceiveFromAsync(args))
			{
				// If the callback reuses the args (indicated by return value of true)
				// then we return without queuing the args.
				if (callback != null
					&& callback(new EndPointOpState(args
						, callback
						, handback
						))) return;

			}
		}

		public void SendTo(EndPoint endpoint, byte[] data)
		{
			_socket.SendTo(data, endpoint);
		}

		public void SendTo(EndPoint endpoint, byte[] data, int count)
		{
			_socket.SendTo(data, count, SocketFlags.None, endpoint);
		}

		public void SendTo(EndPoint endpoint, byte[] data, int offset, int count, SocketFlags flags)
		{
			_socket.SendTo(data, offset, count, flags, endpoint);
		}

		public void SendToAsync(EndPoint endpoint, byte[] data, int count, Func<EndPointOpState, bool> callback)
		{
			SendToAsync(endpoint, data, 0, count, callback, null);
		}

		public void SendToAsync(EndPoint endpoint, byte[] data, int count, Func<EndPointOpState, bool> callback, object handback)
		{
			SendToAsync(endpoint, data, 0, count, callback, handback);
		}

		public void SendToAsync(EndPoint endpoint, byte[] data, int offset, int count, Func<EndPointOpState, bool> callback, object handback)
		{
			SocketAsyncEventArgs args = InitArgs();
			args.RemoteEndPoint = endpoint;
			args.SetBuffer(data, offset, count);
			args.UserToken = new Completion(callback, handback);
			if (!_socket.SendToAsync(args))
			{
				// If the callback reuses the args (indicated by return value of true)
				// then we return without queuing the args.
				if (callback != null
					&& callback(new EndPointOpState(args
						, callback
						, handback
						))) return;

			}
		}

		void BackgroundCompletedCallback(object sender, SocketAsyncEventArgs e)
		{
			Completion c = (Completion)e.UserToken;
			if (c.Callback != null)
			{
				// If the callback reuses the args (indicated by return value of true)
				// then we return without queuing the args.
				if (!c.Callback(new EndPointOpState(e, c.Callback, c.Handback)))
					return;
			}
		}

		private void Dispose(bool disposing)
		{
			if (_socket != null)
			{
				if (_socket.Connected)
				{
					try
					{
						_socket.Close();
					}
					catch (IOException) { /* error eaten on purpose; may be called from the GC */ }
				}

				Util.Dispose(ref _socket);
			}
		}

		private SocketAsyncEventArgs InitArgs()
		{
			SocketAsyncEventArgs args = new SocketAsyncEventArgs();
			args.Completed += new EventHandler<SocketAsyncEventArgs>(BackgroundCompletedCallback);
			return args;
		}

		#endregion Methods

		#region Nested Types

		class Completion
		{
			#region Fields

			public Func<EndPointOpState, bool> Callback;
			public Object Handback;

			#endregion Fields

			#region Constructors

			internal Completion(Func<EndPointOpState, bool> cb, Object hb)
			{
				Callback = cb;
				Handback = hb;
			}

			#endregion Constructors
		}

		#endregion Nested Types
	}
}