#region Header

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

#endregion Header

namespace Org.Lwes.Listener
{
	using System.Net;

	/// <summary>
	/// Interface for classes that handle the arrival of LWES data.
	/// </summary>
	public interface IDataReceiverSink
	{
		/// <summary>
		/// Callback method invoked by event listeners when data arrives on an endpoint.
		/// </summary>
		/// <param name="key">Registration key for controlling the sink's registration</param>
		/// <param name="data">a byte array containing data received from the endpoint</param>
		/// <param name="count">number of bytes received</param>
		/// <param name="offset">offset to the first data byte in the buffer</param>
		/// <param name="remoteEP">remote endpoint that sent the data</param>
		/// <returns><em>true</em> if the listener should continue notifying the sink-chain; otherwise <em>false</em></returns>
		bool HandleData(ISinkRegistrationKey key, EndPoint remoteEP, byte[] data, int offset, int count);
	}
}