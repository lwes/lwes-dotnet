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
// GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes
{
	using System;
	using System.Net;

	/// <summary>
	/// Utility class for coercing attribute values.
	/// </summary>
	public static class Coercion
	{
		#region Methods

		/// <summary>
		/// Tries to coerce an Int16 into a UInt16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(short input, out ushort output)
		{
			if (input >= 0)
			{
				output = Convert.ToUInt16(input);
				return true;
			}
			output = default(ushort);
			return false;
		}

		/// <summary>
		/// Tries to coerce an Int32 into a UInt16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(int input, out ushort output)
		{
			if (input >= 0 && input <= ushort.MaxValue)
			{
				output = Convert.ToUInt16(input);
				return true;
			}
			output = default(ushort);
			return false;
		}

		/// <summary>
		/// Tries to coerce an UInt32 into a UInt16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(uint input, out ushort output)
		{
			if (input <= ushort.MaxValue)
			{
				output = Convert.ToUInt16(input);
				return true;
			}
			output = default(ushort);
			return false;
		}

		/// <summary>
		/// Tries to coerce an Int64 into a UInt16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(long input, out ushort output)
		{
			if (input >= 0 && input <= ushort.MaxValue)
			{
				output = Convert.ToUInt16(input);
				return true;
			}
			output = default(ushort);
			return false;
		}

		/// <summary>
		/// Tries to coerce an UInt64 into a UInt16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ulong input, out ushort output)
		{
			if (input <= ushort.MaxValue)
			{
				output = Convert.ToUInt16(input);
				return true;
			}
			output = default(ushort);
			return false;
		}

		/// <summary>
		/// Tries to coerce an string into a UInt16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(string input, out ushort output)
		{
			return ushort.TryParse(input, out output);
		}

		/// <summary>
		/// Tries to coerce an string into a UInt16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(IPAddress input, out ushort output)
		{
			output = default(ushort);
			return false;
		}

		/// <summary>
		/// Tries to coerce an bool into a UInt16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(bool input, out ushort output)
		{
			output = Convert.ToUInt16(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt16 into a Int16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ushort input, out short output)
		{
			if (input < short.MaxValue)
			{
				output = Convert.ToInt16(input);
				return true;
			}
			output = default(short);
			return false;
		}

		/// <summary>
		/// Tries to coerce an Int32 into a Int16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(int input, out short output)
		{
			if (input >= short.MinValue && input <= short.MaxValue)
			{
				output = Convert.ToInt16(input);
				return true;
			}
			output = default(short);
			return false;
		}

		/// <summary>
		/// Tries to coerce an UInt32 into a Int16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(uint input, out short output)
		{
			if (input <= short.MaxValue)
			{
				output = Convert.ToInt16(input);
				return true;
			}
			output = default(short);
			return false;
		}

		/// <summary>
		/// Tries to coerce an Int64 into a Int16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(long input, out short output)
		{
			if (input >= 0 && input <= ushort.MaxValue)
			{
				output = Convert.ToInt16(input);
				return true;
			}
			output = default(short);
			return false;
		}

		/// <summary>
		/// Tries to coerce an UInt64 into a Int16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ulong input, out short output)
		{
			if (input <= (ulong)short.MaxValue)
			{
				output = Convert.ToInt16(input);
				return true;
			}
			output = default(short);
			return false;
		}

		/// <summary>
		/// Tries to coerce an string into a Int16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(string input, out short output)
		{
			return short.TryParse(input, out output);
		}

		/// <summary>
		/// Tries to coerce an string into a Int16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(IPAddress input, out short output)
		{
			output = default(short);
			return false;
		}

		/// <summary>
		/// Tries to coerce an Boolean into a Int16.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(bool input, out short output)
		{
			output = Convert.ToInt16(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int16 into a UInt32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(short input, out uint output)
		{
			if (input >= 0)
			{
				output = Convert.ToUInt32(input);
				return true;
			}
			output = default(uint);
			return false;
		}

		/// <summary>
		/// Tries to coerce an UInt16 into a UInt32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ushort input, out uint output)
		{
			output = Convert.ToUInt32(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int32 into a UInt32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(int input, out uint output)
		{
			if (input >= 0)
			{
				output = Convert.ToUInt32(input);
				return true;
			}
			output = default(uint);
			return false;
		}

		/// <summary>
		/// Tries to coerce an Int64 into a UInt32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(long input, out uint output)
		{
			if (input >= 0 && input <= uint.MaxValue)
			{
				output = Convert.ToUInt32(input);
				return true;
			}
			output = default(uint);
			return false;
		}

		/// <summary>
		/// Tries to coerce an UInt64 into a UInt32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ulong input, out uint output)
		{
			if (input <= uint.MaxValue)
			{
				output = Convert.ToUInt32(input);
				return true;
			}
			output = default(uint);
			return false;
		}

		/// <summary>
		/// Tries to coerce an string into a UInt32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(string input, out uint output)
		{
			return uint.TryParse(input, out output);
		}

		/// <summary>
		/// Tries to coerce an IPAddress into a UInt32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(IPAddress input, out uint output)
		{
			byte[] addr = input.GetAddressBytes();
			output = BitConverter.ToUInt32(addr, 0);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Boolean into a UInt32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(bool input, out uint output)
		{
			output = Convert.ToUInt32(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int16 into a Int32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(short input, out int output)
		{
			output = Convert.ToInt32(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt16 into a Int32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ushort input, out int output)
		{
			output = Convert.ToInt32(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt32 into a Int32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(uint input, out int output)
		{
			if (input <= int.MaxValue)
			{
				output = Convert.ToInt32(input);
				return true;
			}
			output = default(int);
			return false;
		}

		/// <summary>
		/// Tries to coerce an Int64 into a Int32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(long input, out int output)
		{
			if (input >= int.MinValue && input <= int.MaxValue)
			{
				output = Convert.ToInt32(input);
				return true;
			}
			output = default(int);
			return false;
		}

		/// <summary>
		/// Tries to coerce an UInt64 into a Int32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ulong input, out int output)
		{
			if (input <= int.MaxValue)
			{
				output = Convert.ToInt32(input);
				return true;
			}
			output = default(int);
			return false;
		}

		/// <summary>
		/// Tries to coerce a string into a Int32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(string input, out int output)
		{
			return int.TryParse(input, out output);
		}

		/// <summary>
		/// Tries to coerce an IPAddress into a Int32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(IPAddress input, out int output)
		{
			if (input == null)
			{
				output = default(int);
				return false;
			}
			byte[] addr = input.GetAddressBytes();
			output = BitConverter.ToInt32(addr, 0);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int16 into a Int32.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(bool input, out int output)
		{
			output = Convert.ToInt32(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int16 into a string.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(short input, out string output)
		{
			output = Convert.ToString(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt16 into a string.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ushort input, out string output)
		{
			output = Convert.ToString(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt32 into a string.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(uint input, out string output)
		{
			output = Convert.ToString(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int32 into a string.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(int input, out string output)
		{
			output = Convert.ToString(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int64 into a string.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(long input, out string output)
		{
			output = Convert.ToString(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt64 into a string.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ulong input, out string output)
		{
			output = Convert.ToString(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an IPAddress into a string.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(IPAddress input, out string output)
		{
			output = Convert.ToString(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Boolean into a string.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(bool input, out string output)
		{
			output = Convert.ToString(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int16 into an IPAddress.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(short input, out IPAddress output)
		{
			output = default(IPAddress);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt16 into an IPAddress.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ushort input, out IPAddress output)
		{
			output = default(IPAddress);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt32 into an IPAddress.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(uint input, out IPAddress output)
		{
			output = new IPAddress(BitConverter.GetBytes(input));
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int32 into an IPAddress.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(int input, out IPAddress output)
		{
			output = new IPAddress(BitConverter.GetBytes(input));
			return true;
		}

		/// <summary>
		/// Tries to coerce an string into an IPAddress.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(string input, out IPAddress output)
		{
			return IPAddress.TryParse(input, out output);
		}

		/// <summary>
		/// Tries to coerce an Int64 into an IPAddress.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(long input, out IPAddress output)
		{
			output = default(IPAddress);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt64 into an IPAddress.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ulong input, out IPAddress output)
		{
			output = default(IPAddress);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Boolean into an IPAddress.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(bool input, out IPAddress output)
		{
			output = default(IPAddress);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int16 into an UInt64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(short input, out long output)
		{
			output = Convert.ToInt64(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt16 into an Int64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ushort input, out long output)
		{
			output = Convert.ToInt64(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt32 into an Int64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(uint input, out long output)
		{
			output = Convert.ToInt64(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int32 into an Int64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(int input, out long output)
		{
			output = Convert.ToInt64(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt64 into an Int64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ulong input, out long output)
		{
			if (input <= long.MaxValue)
			{
				output = Convert.ToInt64(input);
				return true;
			}
			output = default(long);
			return false;
		}

		/// <summary>
		/// Tries to coerce an string into an Int64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(string input, out long output)
		{
			return long.TryParse(input, out output);
		}

		/// <summary>
		/// Tries to coerce an IPAddress into an Int64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(IPAddress input, out long output)
		{
			output = default(long);
			return false;
		}

		/// <summary>
		/// Tries to coerce an Boolean into an Int64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(bool input, out long output)
		{
			output = Convert.ToInt64(input);
			return false;
		}

		/// <summary>
		/// Tries to coerce an Int16 into an UInt64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(short input, out ulong output)
		{
			output = Convert.ToUInt64(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt16 into an UInt64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ushort input, out ulong output)
		{
			output = Convert.ToUInt64(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt32 into an UInt64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(uint input, out ulong output)
		{
			output = Convert.ToUInt64(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int16 into an UInt64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(long input, out ulong output)
		{
			if (input > 0)
			{
				output = Convert.ToUInt64(input);
				return true;
			}
			output = default(ulong);
			return false;
		}

		/// <summary>
		/// Tries to coerce an string into an UInt64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(string input, out ulong output)
		{
			return ulong.TryParse(input, out output);
		}

		/// <summary>
		/// Tries to coerce an IPAddress into an UInt64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(IPAddress input, out ulong output)
		{
			output = default(ulong);
			return false;
		}

		/// <summary>
		/// Tries to coerce an Boolean into an UInt64.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(bool input, out ulong output)
		{
			output = Convert.ToUInt64(input);
			return false;
		}

		/// <summary>
		/// Tries to coerce an Int16 into an Boolean.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(short input, out bool output)
		{
			output = Convert.ToBoolean(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt16 into an Boolean.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ushort input, out bool output)
		{
			output = Convert.ToBoolean(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an UInt16 into an Boolean.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(uint input, out bool output)
		{
			output = Convert.ToBoolean(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an Int64 into an Boolean.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(long input, out bool output)
		{
			output = Convert.ToBoolean(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an string into an Boolean.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(string input, out bool output)
		{
			return bool.TryParse(input, out output);
		}

		/// <summary>
		/// Tries to coerce an IPAddress into an Boolean.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(IPAddress input, out bool output)
		{
			output = default(bool);
			return false;
		}

		/// <summary>
		/// Tries to coerce an UInt64 into an Boolean.
		/// </summary>
		/// <param name="input">the input value</param>
		/// <param name="output">the output value</param>
		/// <returns><em>true</em> if the input value is coerced into a value of the output type; otherwise <em>false</em></returns>
		public static bool TryCoerce(ulong input, out bool output)
		{
			output = Convert.ToBoolean(input);
			return true;
		}

		/// <summary>
		/// Tries to coerce an input value into a UInt16.
		/// </summary>
		/// <typeparam name="V">input value type V</typeparam>
		/// <param name="tt">TypeToken of the input value</param>
		/// <param name="input">input value</param>
		/// <param name="output">reference to a variable to hold the output</param>
		/// <returns><em>true</em> if the input is successfully coerced; otherwise <em>false</em></returns>
		internal static bool TryCoerce<V>(TypeToken tt, V input, out ushort output)
		{
			switch (tt)
			{
				case TypeToken.UINT16:
					output = Convert.ToUInt16(input);
					return true;
				case TypeToken.INT16:
					return TryCoerce(Convert.ToInt16(input), out output);
				case TypeToken.UINT32:
					return TryCoerce(Convert.ToUInt32(input), out output);
				case TypeToken.INT32:
					return TryCoerce(Convert.ToInt32(input), out output);
				case TypeToken.STRING:
					return TryCoerce(Convert.ToString(input), out output);
				case TypeToken.IP_ADDR:
					return TryCoerce(input as IPAddress, out output);
				case TypeToken.INT64:
					return TryCoerce(Convert.ToInt64(input), out output);
				case TypeToken.UINT64:
					return TryCoerce(Convert.ToUInt64(input), out output);
				case TypeToken.BOOLEAN:
					return TryCoerce(Convert.ToBoolean(input), out output);
			}
			output = default(ushort);
			return false;
		}

		/// <summary>
		/// Tries to coerce an input value into a Int16.
		/// </summary>
		/// <typeparam name="V">input value type V</typeparam>
		/// <param name="tt">TypeToken of the input value</param>
		/// <param name="input">input value</param>
		/// <param name="output">reference to a variable to hold the output</param>
		/// <returns><em>true</em> if the input is successfully coerced; otherwise <em>false</em></returns>
		internal static bool TryCoerce<V>(TypeToken tt, V input, out short output)
		{
			switch (tt)
			{
				case TypeToken.UINT16:
					return TryCoerce(Convert.ToUInt16(input), out output);
				case TypeToken.INT16:
					output = Convert.ToInt16(input);
					return true;
				case TypeToken.UINT32:
					return TryCoerce(Convert.ToUInt32(input), out output);
				case TypeToken.INT32:
					return TryCoerce(Convert.ToInt32(input), out output);
				case TypeToken.STRING:
					return TryCoerce(Convert.ToString(input), out output);
				case TypeToken.IP_ADDR:
					return TryCoerce(input as IPAddress, out output);
				case TypeToken.INT64:
					return TryCoerce(Convert.ToInt64(input), out output);
				case TypeToken.UINT64:
					return TryCoerce(Convert.ToUInt64(input), out output);
				case TypeToken.BOOLEAN:
					return TryCoerce(Convert.ToBoolean(input), out output);
			}
			output = default(short);
			return false;
		}

		/// <summary>
		/// Tries to coerce an input value into a Int32.
		/// </summary>
		/// <typeparam name="V">input value type V</typeparam>
		/// <param name="tt">TypeToken of the input value</param>
		/// <param name="input">input value</param>
		/// <param name="output">reference to a variable to hold the output</param>
		/// <returns><em>true</em> if the input is successfully coerced; otherwise <em>false</em></returns>
		internal static bool TryCoerce<V>(TypeToken tt, V input, out uint output)
		{
			switch (tt)
			{
				case TypeToken.UINT16:
					output = Convert.ToUInt16(input);
					return true;
				case TypeToken.INT16:
					return TryCoerce(Convert.ToInt16(input), out output);
				case TypeToken.UINT32:
					output = Convert.ToUInt32(input);
					return true;
				case TypeToken.INT32:
					return TryCoerce(Convert.ToInt32(input), out output);
				case TypeToken.STRING:
					return TryCoerce(Convert.ToString(input), out output);
				case TypeToken.IP_ADDR:
					return TryCoerce(input as IPAddress, out output);
				case TypeToken.INT64:
					return TryCoerce(Convert.ToInt64(input), out output);
				case TypeToken.UINT64:
					return TryCoerce(Convert.ToUInt64(input), out output);
				case TypeToken.BOOLEAN:
					return TryCoerce(Convert.ToBoolean(input), out output);
			}
			output = default(ushort);
			return false;
		}

		/// <summary>
		/// Tries to coerce an input value into a Int32.
		/// </summary>
		/// <typeparam name="V">input value type V</typeparam>
		/// <param name="tt">TypeToken of the input value</param>
		/// <param name="input">input value</param>
		/// <param name="output">reference to a variable to hold the output</param>
		/// <returns><em>true</em> if the input is successfully coerced; otherwise <em>false</em></returns>
		internal static bool TryCoerce<V>(TypeToken tt, V input, out int output)
		{
			switch (tt)
			{
				case TypeToken.UINT16:
					return TryCoerce(Convert.ToUInt16(input), out output);
				case TypeToken.INT16:
					return TryCoerce(Convert.ToInt16(input), out output);
				case TypeToken.UINT32:
					return TryCoerce(Convert.ToUInt32(input), out output);
				case TypeToken.INT32:
					output = Convert.ToInt32(input);
					return true;
				case TypeToken.STRING:
					return TryCoerce(Convert.ToString(input), out output);
				case TypeToken.IP_ADDR:
					return TryCoerce(input as IPAddress, out output);
				case TypeToken.INT64:
					return TryCoerce(Convert.ToInt64(input), out output);
				case TypeToken.UINT64:
					return TryCoerce(Convert.ToUInt64(input), out output);
				case TypeToken.BOOLEAN:
					return TryCoerce(Convert.ToBoolean(input), out output);
			}
			output = default(ushort);
			return false;
		}

		/// <summary>
		/// Tries to coerce an input value into a String.
		/// </summary>
		/// <typeparam name="V">input value type V</typeparam>
		/// <param name="tt">TypeToken of the input value</param>
		/// <param name="input">input value</param>
		/// <param name="output">reference to a variable to hold the output</param>
		/// <returns><em>true</em> if the input is successfully coerced; otherwise <em>false</em></returns>
		internal static bool TryCoerce<V>(TypeToken tt, V input, out string output)
		{
			switch (tt)
			{
				case TypeToken.UINT16:
					return TryCoerce(Convert.ToUInt16(input), out output);
				case TypeToken.INT16:
					return TryCoerce(Convert.ToInt16(input), out output);
				case TypeToken.UINT32:
					return TryCoerce(Convert.ToUInt32(input), out output);
				case TypeToken.INT32:
					return TryCoerce(Convert.ToInt32(input), out output);
				case TypeToken.STRING:
					output = Convert.ToString(input);
					return true;
				case TypeToken.IP_ADDR:
					return TryCoerce(input as IPAddress, out output);
				case TypeToken.INT64:
					return TryCoerce(Convert.ToInt64(input), out output);
				case TypeToken.UINT64:
					return TryCoerce(Convert.ToUInt64(input), out output);
				case TypeToken.BOOLEAN:
					return TryCoerce(Convert.ToBoolean(input), out output);
			}
			output = default(string);
			return false;
		}

		/// <summary>
		/// Tries to coerce an input value into a IPAddress.
		/// </summary>
		/// <typeparam name="V">input value type V</typeparam>
		/// <param name="tt">TypeToken of the input value</param>
		/// <param name="input">input value</param>
		/// <param name="output">reference to a variable to hold the output</param>
		/// <returns><em>true</em> if the input is successfully coerced; otherwise <em>false</em></returns>
		internal static bool TryCoerce<V>(TypeToken tt, V input, out IPAddress output)
		{
			switch (tt)
			{
				case TypeToken.UINT16:
					return TryCoerce(Convert.ToUInt16(input), out output);
				case TypeToken.INT16:
					return TryCoerce(Convert.ToInt16(input), out output);
				case TypeToken.UINT32:
					return TryCoerce(Convert.ToUInt32(input), out output);
				case TypeToken.INT32:
					return TryCoerce(Convert.ToInt32(input), out output);
				case TypeToken.STRING:
					return TryCoerce(Convert.ToString(input), out output);
				case TypeToken.IP_ADDR:
					output = input as IPAddress;
					return true;
				case TypeToken.INT64:
					return TryCoerce(Convert.ToInt64(input), out output);
				case TypeToken.UINT64:
					return TryCoerce(Convert.ToUInt64(input), out output);
				case TypeToken.BOOLEAN:
					return TryCoerce(Convert.ToBoolean(input), out output);
			}
			output = default(IPAddress);
			return false;
		}

		/// <summary>
		/// Tries to coerce an input value into a Int64.
		/// </summary>
		/// <typeparam name="V">input value type V</typeparam>
		/// <param name="tt">TypeToken of the input value</param>
		/// <param name="input">input value</param>
		/// <param name="output">reference to a variable to hold the output</param>
		/// <returns><em>true</em> if the input is successfully coerced; otherwise <em>false</em></returns>
		internal static bool TryCoerce<V>(TypeToken tt, V input, out long output)
		{
			switch (tt)
			{
				case TypeToken.UINT16:
					return TryCoerce(Convert.ToUInt16(input), out output);
				case TypeToken.INT16:
					return TryCoerce(Convert.ToInt16(input), out output);
				case TypeToken.UINT32:
					return TryCoerce(Convert.ToUInt32(input), out output);
				case TypeToken.INT32:
					return TryCoerce(Convert.ToInt32(input), out output);
				case TypeToken.STRING:
					return TryCoerce(Convert.ToString(input), out output);
				case TypeToken.IP_ADDR:
					return TryCoerce(input as IPAddress, out output);
				case TypeToken.INT64:
					output = Convert.ToUInt16(input);
					return true;
				case TypeToken.UINT64:
					return TryCoerce(Convert.ToUInt64(input), out output);
				case TypeToken.BOOLEAN:
					return TryCoerce(Convert.ToBoolean(input), out output);
			}
			output = default(ushort);
			return false;
		}

		/// <summary>
		/// Tries to coerce an input value into a UInt64.
		/// </summary>
		/// <typeparam name="V">input value type V</typeparam>
		/// <param name="tt">TypeToken of the input value</param>
		/// <param name="input">input value</param>
		/// <param name="output">reference to a variable to hold the output</param>
		/// <returns><em>true</em> if the input is successfully coerced; otherwise <em>false</em></returns>
		internal static bool TryCoerce<V>(TypeToken tt, V input, out ulong output)
		{
			switch (tt)
			{
				case TypeToken.UINT16:
					return TryCoerce(Convert.ToUInt16(input), out output);
				case TypeToken.INT16:
					return TryCoerce(Convert.ToInt16(input), out output);
				case TypeToken.UINT32:
					return TryCoerce(Convert.ToUInt32(input), out output);
				case TypeToken.INT32:
					return TryCoerce(Convert.ToInt32(input), out output);
				case TypeToken.STRING:
					return TryCoerce(Convert.ToString(input), out output);
				case TypeToken.IP_ADDR:
					return TryCoerce(input as IPAddress, out output);
				case TypeToken.INT64:
					return TryCoerce(Convert.ToInt64(input), out output);
				case TypeToken.UINT64:
					output = Convert.ToUInt64(input);
					return true;
				case TypeToken.BOOLEAN:
					return TryCoerce(Convert.ToBoolean(input), out output);
			}
			output = default(ulong);
			return false;
		}

		/// <summary>
		/// Tries to coerce an input value into a Boolean.
		/// </summary>
		/// <typeparam name="V">input value type V</typeparam>
		/// <param name="tt">TypeToken of the input value</param>
		/// <param name="input">input value</param>
		/// <param name="output">reference to a variable to hold the output</param>
		/// <returns><em>true</em> if the input is successfully coerced; otherwise <em>false</em></returns>
		internal static bool TryCoerce<V>(TypeToken tt, V input, out bool output)
		{
			switch (tt)
			{
				case TypeToken.UINT16:
					return TryCoerce(Convert.ToUInt16(input), out output);
				case TypeToken.INT16:
					return TryCoerce(Convert.ToInt16(input), out output);
				case TypeToken.UINT32:
					return TryCoerce(Convert.ToUInt32(input), out output);
				case TypeToken.INT32:
					return TryCoerce(Convert.ToInt32(input), out output);
				case TypeToken.STRING:
					return TryCoerce(Convert.ToString(input), out output);
				case TypeToken.IP_ADDR:
					return TryCoerce(input as IPAddress, out output);
				case TypeToken.INT64:
					return TryCoerce(Convert.ToInt64(input), out output);
				case TypeToken.UINT64:
					return TryCoerce(Convert.ToUInt64(input), out output);
				case TypeToken.BOOLEAN:
					output = Convert.ToBoolean(input);
					return true;
			}
			output = default(bool);
			return false;
		}

		#endregion Methods
	}
}