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
	using System;
	using System.Net;
	using System.Text;

	using Org.Lwes.DB;

	/// <summary>
	/// Serialization utility implementing the LWES binary format.
	/// </summary>
	public static class LwesSerializer
	{
		#region Methods

		/// <summary>
		/// Deserializes an LWES event from a byte array.
		/// </summary>
		/// <param name="buffer">the byte array containing the event</param>
		/// <param name="offset">offset to the first byte of the event</param>
		/// <param name="count">number of bytes to deserialize</param>
		/// <param name="db">an event template DB for creating events</param>
		/// <returns>the deserialized event</returns>
		public static Event Deserialize(byte[] buffer, int offset, int count, IEventTemplateDB db)
		{
			return Event.BinaryDecode(db, buffer, offset, count);
		}

		/// <summary>
		/// Reads an ATTRIBUTEWORD from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset 
		/// reflects the position within the buffer where reading should begin. 
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="coder">Decoder used to translate byte data to character data</param>
		/// <returns>an ATTRIBUTEWORD value taken from the buffer</returns>
		public static string ReadATTRIBUTEWORD(byte[] buffer, ref int offset, Decoder coder)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 1) throw new BadLwesDataException(String.Concat("Expected ATTRIBUTEWORD at offset ", offset));
			#endif
			return ReadStringWithByteLengthPrefix(buffer, ref offset, coder);
		}

		/// <summary>
		/// Reads a boolean from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; reflects the position
		/// within the buffer where reading should begin. Upon success the offset is incremented
		/// by the number of bytes that are used.</param>
		/// <returns>a boolean value taken from the buffer</returns>
		public static bool ReadBoolean(byte[] buffer, ref int offset)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 1) throw new BadLwesDataException(String.Concat("Expected Boolean at offset ", offset));

			return buffer[offset++] == 1;
		}

		/// <summary>
		/// Reads a byte from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset 
		/// reflects the position within the buffer where reading should begin. 
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <returns>a byte value taken from the buffer</returns>
		public static byte ReadByte(byte[] buffer, ref int offset)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 1) throw new BadLwesDataException(String.Concat("Expected byte at offset ", offset));

			return buffer[offset++];
		}

		/// <summary>
		/// Reads an EVENTWORD from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset 
		/// reflects the position within the buffer where reading should begin. 
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="coder">decoder used for transforming byte data to character data</param>
		/// <returns>an EVENTWORD value taken from the buffer</returns>
		public static string ReadEVENTWORD(byte[] buffer, ref int offset, Decoder coder)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 1) throw new BadLwesDataException(String.Concat("Expected EVENTWORD at offset ", offset));
			#endif
			return ReadStringWithByteLengthPrefix(buffer, ref offset, coder);
		}

		/// <summary>
		/// Reads an IPAddress from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset 
		/// reflects the position within the buffer where reading should begin. 
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <returns>an IPAddress value taken from the buffer</returns>
		public static IPAddress ReadIPAddress(byte[] buffer, ref int offset)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 4) throw new BadLwesDataException(String.Concat("Expected IPAddress at offset ", offset));

			uint result;
			unchecked
			{
				result = (((uint)buffer[offset] << 24)
					| ((uint)buffer[offset + 1] << 16)
					| ((uint)buffer[offset + 2] << 8)
					| (uint)buffer[offset + 3]);
			}
			offset += 4;

			return new IPAddress(result);
		}

		/// <summary>
		/// Reads a Int16 from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset 
		/// reflects the position within the buffer where reading should begin. 
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <returns>a Int16 value taken from the buffer</returns>
		public static Int16 ReadInt16(byte[] buffer, ref int offset)
		{
			unchecked
			{
				return (short)ReadUInt16(buffer, ref offset);
			}
		}

		/// <summary>
		/// Reads a Int16 from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset 
		/// reflects the position within the buffer where reading should begin. 
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <returns>a Int16 value taken from the buffer</returns>
		public static Int32 ReadInt32(byte[] buffer, ref int offset)
		{
			unchecked
			{
				return (int)ReadUInt32(buffer, ref offset);
			}
		}

		/// <summary>
		/// Reads a Int64 from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset 
		/// reflects the position within the buffer where reading should begin. 
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <returns>a Int64 value taken from the buffer</returns>
		public static Int64 ReadInt64(byte[] buffer, ref int offset)
		{
			unchecked
			{
				return (long)ReadUInt64(buffer, ref offset);
			}
		}

		/// <summary>
		/// Reads a String from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset 
		/// reflects the position within the buffer where reading should begin. 
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="coder">Decoder used to translate byte data to character data</param>
		/// <returns>a String value taken from the buffer</returns>
		public static string ReadString(byte[] buffer, ref int offset, Decoder coder)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 1) throw new BadLwesDataException(String.Concat("Expected String at offset ", offset));
			#endif
			return ReadStringWithUInt16LengthPrefix(buffer, ref offset, coder);
		}

		/// <summary>
		/// Reads a UInt16 from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset 
		/// reflects the position within the buffer where reading should begin. 
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <returns>a UInt16 value taken from the buffer</returns>
		public static ushort ReadUInt16(byte[] buffer, ref int offset)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 2) throw new BadLwesDataException(String.Concat("Expected UInt16 at offset ", offset));

			ushort result;
			unchecked
			{
				result = (ushort)(((uint)buffer[offset] << 8) | (uint)buffer[offset + 1]);
			}
			offset += 2;
			return result;
		}

		/// <summary>
		/// Reads a UInt32 from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset 
		/// reflects the position within the buffer where reading should begin. 
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <returns>a UInt32 value taken from the buffer</returns>
		public static uint ReadUInt32(byte[] buffer, ref int offset)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 4) throw new BadLwesDataException(String.Concat("Expected UInt32 at offset ", offset));

			uint result;
			unchecked
			{
				result = (((uint)buffer[offset] << 24)
					| ((uint)buffer[offset + 1] << 16)
					| ((uint)buffer[offset + 2] << 8)
					| (uint)buffer[offset + 3]);
			}
			offset += 4;
			return result;
		}

		/// <summary>
		/// Reads a UInt64 from the byte buffer.
		/// </summary>
		/// <param name="buffer">buffer containing serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset 
		/// reflects the position within the buffer where reading should begin. 
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <returns>a UInt64 value taken from the buffer</returns>
		public static ulong ReadUInt64(byte[] buffer, ref int offset)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 8) throw new BadLwesDataException(String.Concat("Expected UInt64 at offset ", offset));

			ulong result;
			unchecked
			{
				result = (((ulong)buffer[offset] << 56)
					| ((ulong)buffer[offset + 1] << 48)
					| ((ulong)buffer[offset + 2] << 40)
					| ((ulong)buffer[offset + 3] << 32)
					| ((ulong)buffer[offset + 4] << 24)
					| ((ulong)buffer[offset + 5] << 16)
					| ((ulong)buffer[offset + 6] << 8)
					| (ulong)buffer[offset + 7]);
			}
			offset += 8;
			return result;
		}

		/// <summary>
		/// Serializes an LWES event to a byte array.
		/// </summary>
		/// <param name="ev">an event to serialize</param>
		/// <returns>a byte array containing the event's serialized bytes</returns>
		public static byte[] Serialize(Event ev)
		{
			byte[] buffer = new byte[ev.CalculateEncodedByteCount()];
			ev.BinaryEncode(buffer, 0);
			return buffer;
		}

		/// <summary>
		/// Serializes an LWES event to a byte array taken from the MemoryBuffer
		/// class so that memory limits are enforced.
		/// </summary>
		/// <param name="ev">an event to serialize</param>
		/// <returns>a byte array containing the event's serialized bytes</returns>
		public static byte[] SerializeToMemoryBuffer(Event ev)
		{
			byte[] buffer = BufferManager.AcquireBuffer(ev.CalculateEncodedByteCount(), null);
			ev.BinaryEncode(buffer, 0);
			return buffer;
		}

		/// <summary>
		/// Writes a boolean value to the byte buffer.
		/// </summary>
		/// <param name="buffer">target buffer for the serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset
		/// reflects the position within the buffer where writing should begin.
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="value">boolean value to be written</param>
		/// <returns>number of bytes used</returns>
		public static int Write(byte[] buffer, ref int offset, bool value)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 1) throw new ArgumentOutOfRangeException("offset");
			#endif
			buffer[offset++] = (byte)((value) ? 1 : 0);
			return 1;
		}

		/// <summary>
		/// Writes a byte value to the byte buffer.
		/// </summary>
		/// <param name="buffer">target buffer for the serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset
		/// reflects the position within the buffer where writing should begin.
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="value">byte value to be written</param>
		/// <returns>number of bytes used</returns>
		public static int Write(byte[] buffer, ref int offset, byte value)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 1) throw new ArgumentOutOfRangeException("offset");
			#endif
			buffer[offset++] = value;
			return 1;
		}

		/// <summary>
		/// Writes a Int16 value to the byte buffer.
		/// </summary>
		/// <param name="buffer">target buffer for the serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset
		/// reflects the position within the buffer where writing should begin.
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="value">Int16 value to be written</param>
		/// <returns>number of bytes used</returns>
		public static int Write(byte[] buffer, ref int offset, Int16 value)
		{
			return Write(buffer, ref offset, (UInt16)value);
		}

		/// <summary>
		/// Writes a Int32 value to the byte buffer.
		/// </summary>
		/// <param name="buffer">target buffer for the serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset
		/// reflects the position within the buffer where writing should begin.
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="value">Int32 value to be written</param>
		/// <returns>number of bytes used</returns>
		public static int Write(byte[] buffer, ref int offset, Int32 value)
		{
			return Write(buffer, ref offset, (UInt32)value);
		}

		/// <summary>
		/// Writes a Int64 value to the byte buffer.
		/// </summary>
		/// <param name="buffer">target buffer for the serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset
		/// reflects the position within the buffer where writing should begin.
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="value">Int64 value to be written</param>
		/// <returns>number of bytes used</returns>
		public static int Write(byte[] buffer, ref int offset, Int64 value)
		{
			return Write(buffer, ref offset, (UInt64)value);
		}

		/// <summary>
		/// Writes a UInt16 value to the byte buffer.
		/// </summary>
		/// <param name="buffer">target buffer for the serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset
		/// reflects the position within the buffer where writing should begin.
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="value">UInt16 value to be written</param>
		/// <returns>number of bytes used</returns>
		public static int Write(byte[] buffer, ref int offset, UInt16 value)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - sizeof(UInt16)) throw new ArgumentOutOfRangeException("offset");
			#endif

			int v = (int)value;
			unchecked
			{
				buffer[offset] = (byte)((v >> 8) & 0xFF);
				buffer[offset+1] = (byte)(v & 0xFF);
			}
			offset += sizeof(UInt16);
			return sizeof(UInt16);
		}

		/// <summary>
		/// Writes a UInt32 value to the byte buffer.
		/// </summary>
		/// <param name="buffer">target buffer for the serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset
		/// reflects the position within the buffer where writing should begin.
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="value">UInt32 value to be written</param>
		/// <returns>number of bytes used</returns>
		public static int Write(byte[] buffer, ref int offset, UInt32 value)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - sizeof(UInt32)) throw new ArgumentOutOfRangeException("offset");
			#endif

			unchecked
			{
				buffer[offset] = (byte)((value >> 24) & 0xFF);
				buffer[offset + 1] = (byte)((value >> 16) & 0xFF);
				buffer[offset + 2] = (byte)((value >> 8) & 0xFF);
				buffer[offset + 3] = (byte)(byte)(value & 0xFF);
			}
			offset += sizeof(UInt32);
			return sizeof(UInt32);
		}

		/// <summary>
		/// Writes a UInt64 value to the byte buffer.
		/// </summary>
		/// <param name="buffer">target buffer for the serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset
		/// reflects the position within the buffer where writing should begin.
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="value">UInt64 value to be written</param>
		/// <returns>number of bytes used</returns>
		public static int Write(byte[] buffer, ref int offset, UInt64 value)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - sizeof(UInt64)) throw new ArgumentOutOfRangeException("offset");
			#endif

			unchecked
			{
				buffer[offset] = (byte)((value >> 56) & 0xFF);
				buffer[offset + 1] = (byte)((value >> 48) & 0xFF);
				buffer[offset + 2] = (byte)((value >> 40) & 0xFF);
				buffer[offset + 3] = (byte)((value >> 32) & 0xFF);
				buffer[offset + 4] = (byte)((value >> 24) & 0xFF);
				buffer[offset + 5] = (byte)((value >> 16) & 0xFF);
				buffer[offset + 6] = (byte)((value >> 8) & 0xFF);
				buffer[offset + 7] = (byte)(value & 0xFF);
			}
			offset += sizeof(UInt64);
			return sizeof(UInt64);
		}

		/// <summary>
		/// Writes a String value to the byte buffer.
		/// </summary>
		/// <param name="buffer">target buffer for the serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset
		/// reflects the position within the buffer where writing should begin.
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="value">String value to be written</param>
		/// <param name="coder">encoder used to transform the characters to bytes.</param>
		/// <returns>number of bytes used</returns>
		public static int Write(byte[] buffer, ref int offset, string value, Encoder coder)
		{
			#if DEBUG
			if (value == null) throw new ArgumentNullException("value");
			#endif
			return WriteWithUInt16LengthPrefix(buffer, ref offset, value.ToCharArray(), coder);
		}

		/// <summary>
		/// Writes a ATTRIBUTEWORD value to the byte buffer.
		/// </summary>
		/// <param name="buffer">target buffer for the serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset
		/// reflects the position within the buffer where writing should begin.
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="value">ATTRIBUTEWORD value to be written</param>
		/// <param name="coder">encoder used to transform the characters to bytes.</param>
		/// <returns>number of bytes used</returns>
		public static int WriteATTRIBUTEWORD(byte[] buffer, ref int offset, string value, Encoder coder)
		{
			#if DEBUG
			if (value == null) throw new ArgumentNullException("value");
			#endif
			return WriteWithByteLengthPrefix(buffer, ref offset, value.ToCharArray(), coder);
		}

		/// <summary>
		/// Writes a EVENTWORD value to the byte buffer.
		/// </summary>
		/// <param name="buffer">target buffer for the serialized data</param>
		/// <param name="offset">reference to an offset variable; upon entry offset
		/// reflects the position within the buffer where writing should begin.
		/// Upon success the offset will be incremented by the number of bytes that are used.</param>
		/// <param name="value">EVENTWORD value to be written</param>
		/// <param name="coder">encoder used to transform the characters to bytes.</param>
		/// <returns>number of bytes used</returns>
		public static int WriteEVENTWORD(byte[] buffer, ref int offset, string value, Encoder coder)
		{
			#if DEBUG
			if (value == null) throw new ArgumentNullException("value");
			#endif
			return WriteWithByteLengthPrefix(buffer, ref offset, value.ToCharArray(), coder);
		}

		private static string ReadStringWithByteLengthPrefix(byte[] buffer, ref int offset, Decoder coder)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 1) throw new ArgumentOutOfRangeException("offset");
			if (coder == null) throw new ArgumentNullException("coder");
			#endif
			int ofs = offset;
			int count = ReadByte(buffer, ref ofs);
			if (ofs + count > buffer.Length) throw new BadLwesDataException(
				String.Concat("Cannot deserialize incoming string at offset ", ofs));
			char[] result = new char[count];
			int bytesUsed;
			int charsUsed;
			bool completed;
			coder.Convert(buffer, ofs, count, result, 0, count, true
				, out bytesUsed
				, out charsUsed
				, out completed);
			if (!completed)
				throw new ArgumentException(String.Format(
					Properties.Resources.Error_BufferOutOfSpace), "buffer");
			offset = ofs + bytesUsed;
			return new String(result, 0, charsUsed);
		}

		private static string ReadStringWithUInt16LengthPrefix(byte[] buffer, ref int offset, Decoder coder)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (offset < 0 || offset > buffer.Length - 1) throw new ArgumentOutOfRangeException("offset");
			if (coder == null) throw new ArgumentNullException("coder");
			#endif
			int ofs = offset;
			int count = ReadUInt16(buffer, ref ofs);
			if (ofs + count > buffer.Length)
				throw new BadLwesDataException(String.Concat("Cannot deserialize incoming string at offset ", ofs));
			char[] result = new char[count];
			int bytesUsed;
			int charsUsed;
			bool completed;
			coder.Convert(buffer, ofs, count, result, 0, count, true
				, out bytesUsed
				, out charsUsed
				, out completed);
			if (!completed)
				throw new ArgumentException(String.Format(
					Properties.Resources.Error_BufferOutOfSpace), "buffer");
			offset = ofs + bytesUsed;
			return new String(result, 0, charsUsed);
		}

		private static int WriteWithByteLengthPrefix(byte[] buffer, ref int offset, Char[] value, Encoder coder)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (value == null) throw new ArgumentNullException("value");
			if (coder == null) throw new ArgumentNullException("coder");
			#endif
			int ofs = offset;
			int count = coder.GetByteCount(value, 0, value.Length, true);
			int bytesUsed;
			int charsUsed;
			bool completed;
			Write(buffer, ref ofs, (byte)count);
			coder.Convert(value, 0, value.Length, buffer, ofs, buffer.Length - ofs, true
				, out charsUsed
				, out bytesUsed
				, out completed);
			if (!completed)
				throw new ArgumentException(String.Format(
					Properties.Resources.Error_BufferOutOfSpace), "buffer");
			offset = ofs + bytesUsed;
			return bytesUsed + 1;
		}

		private static int WriteWithUInt16LengthPrefix(byte[] buffer, ref int offset, Char[] value, Encoder coder)
		{
			#if DEBUG
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (value == null) throw new ArgumentNullException("value");
			if (coder == null) throw new ArgumentNullException("coder");
			#endif
			int ofs = offset;
			int count = coder.GetByteCount(value, 0, value.Length, true);
			int bytesUsed;
			int charsUsed;
			bool completed;
			Write(buffer, ref ofs, (UInt16)count);
			coder.Convert(value, 0, value.Length, buffer, ofs, buffer.Length - ofs, true
				, out charsUsed
				, out bytesUsed
				, out completed);
			if (!completed)
				throw new ArgumentException(String.Format(
					Properties.Resources.Error_BufferOutOfSpace), "buffer");
			offset = ofs + bytesUsed;
			return bytesUsed + 2;
		}

		#endregion Methods
	}
}