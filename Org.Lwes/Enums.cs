// 
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
namespace Org.Lwes
{
	#region Enumerations

	/// <summary>
	/// Supported encodings
	/// </summary>
	public enum SupportedEncoding : short
	{
		/// <summary>
		/// Value indicating the ISO-8859-1 encoding.
		/// </summary>
		ISO_8859_1 = 0,
		/// <summary>
		/// Value indicating the UTF-8 encoding.
		/// </summary>
		UTF_8 = 1,
		/// <summary>
		/// Value indicating the default encoding.
		/// </summary>
		Default = UTF_8
	}

	/// <summary>
	/// Tokens used in the Event Serialization Protocol
	/// </summary>
	public enum TypeToken : byte
	{
		/// <summary>
		/// Indicates a UInt16
		/// </summary>
		UINT16 = (byte)0x01,
		/// <summary>
		/// Indicates an Int16
		/// </summary>
		INT16 = (byte)0x02,
		/// <summary>
		/// Indicates a UInt32
		/// </summary>
		UINT32 = (byte)0x03,
		/// <summary>
		/// Indicates an Int32
		/// </summary>
		INT32 = (byte)0x04,
		/// <summary>
		/// Indicates a string
		/// </summary>
		STRING = (byte)0x05,
		/// <summary>
		/// Indicates a IP address
		/// </summary>
		IP_ADDR = (byte)0x06,
		/// <summary>
		/// Indicates an Int64
		/// </summary>
		INT64 = (byte)0x07,
		/// <summary>
		/// Indicates a UInt64
		/// </summary>
		UINT64 = (byte)0x08,
		/// <summary>
		/// Indicates a boolean
		/// </summary>
		BOOLEAN = (byte)0x09,
		/// <summary>
		/// Indicates the token type is undefined
		/// </summary>
		UNDEFINED = (byte)0xff,
		/// <summary>
		/// Minimum value.
		/// </summary>
		MinValue = 0x01,
		/// <summary>
		/// Maximum defined value.
		/// </summary>
		MaxValue = 0x09
	}

	#endregion Enumerations
}