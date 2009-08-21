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
namespace Org.Lwes.ESF
{
	using System;
	using System.Net;
	using System.Text;

	/// <summary>
	/// A template for an event attribute.
	/// </summary>
	[Serializable]
	public struct AttributeTemplate
	{
		#region Fields

		string _name;
		int _ordinal;
		TypeToken _typeToken;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="tt">the attribute's type</param>
		/// <param name="name">the attribute's name</param>
		/// <param name="ordinal">ordinal position of the attribute among attributes in the event</param>
		internal AttributeTemplate(TypeToken tt, string name, int ordinal)
		{
			_typeToken = tt;
			_name = name;
			_ordinal = ordinal;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// The attribute's name.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// The attribute's ordinal position within an event template.
		/// </summary>
		public int Ordinal
		{
			get { return _ordinal; }
		}

		/// <summary>
		/// The attribute's token type.
		/// </summary>
		public TypeToken TypeToken
		{
			get { return _typeToken; }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Utility method used to parse an attribute within an event template.
		/// </summary>
		/// <param name="input">array of characters being parsed.</param>
		/// <param name="cursor">reference to a cursor, updated if an attribute template is present at the cursor</param>
		/// <returns>An attribute template</returns>
		public static AttributeTemplate ExpectAttribute(char[] input, ref Cursor cursor)
		{
			Cursor c = new Cursor(cursor);
			EsfParser.SkipWhitespaceAndComments(input, ref c);

			TypeToken tt = EsfParser.ExpectTypeToken(input, ref c);
			EsfParser.SkipWhitespaceAndComments(input, ref c);

			string attrName = EsfParser.ExpectWord(input, ref c);
			AttributeTemplate evt = new AttributeTemplate(tt, attrName, 0);

			EsfParser.SkipWhitespaceAndComments(input, ref c);

			EsfParser.ExpectChar(input, ref c, EsfParser.SemiColon);

			cursor = c; // successful, advance the cursor.
			return evt;
		}

		/// <summary>
		/// Determines if a value type V is assignable to the attribute.
		/// </summary>
		/// <typeparam name="V">value type V</typeparam>
		/// <param name="value">a value of type V</param>
		/// <returns><em>true</em> if the value is assignable to the attribute; 
		/// otherwise <em>false</em>.</returns>
		public bool IsAssignable<V>(V value)
		{
			switch (this.TypeToken)
			{
				case TypeToken.UINT16:
					return typeof(UInt16).IsAssignableFrom(typeof(V));
				case TypeToken.INT16:
					return typeof(Int16).IsAssignableFrom(typeof(V));
				case TypeToken.UINT32:
					return typeof(UInt32).IsAssignableFrom(typeof(V));
				case TypeToken.INT32:
					return typeof(Int32).IsAssignableFrom(typeof(V));
				case TypeToken.STRING:
					return typeof(String).IsAssignableFrom(typeof(V));
				case TypeToken.IP_ADDR:
					return typeof(IPAddress).IsAssignableFrom(typeof(V));
				case TypeToken.INT64:
					return typeof(Int64).IsAssignableFrom(typeof(V));
				case TypeToken.UINT64:
					return typeof(UInt64).IsAssignableFrom(typeof(V));
				case TypeToken.BOOLEAN:
					return typeof(Boolean).IsAssignableFrom(typeof(V));
			}
			return false;
		}

		internal static AttributeTemplate CreateTemplateForVariable(string name, int ord, ushort value)
		{
			return new AttributeTemplate(TypeToken.UINT16, name, ord);
		}

		internal static AttributeTemplate CreateTemplateForVariable(string name, int ord, short value)
		{
			return new AttributeTemplate(TypeToken.INT16, name, ord);
		}

		internal static AttributeTemplate CreateTemplateForVariable(string name, int ord, uint value)
		{
			return new AttributeTemplate(TypeToken.UINT32, name, ord);
		}

		internal static AttributeTemplate CreateTemplateForVariable(string name, int ord, int value)
		{
			return new AttributeTemplate(TypeToken.INT32, name, ord);
		}

		internal static AttributeTemplate CreateTemplateForVariable(string name, int ord, string value)
		{
			return new AttributeTemplate(TypeToken.STRING, name, ord);
		}

		internal static AttributeTemplate CreateTemplateForVariable(string name, int ord, IPAddress value)
		{
			return new AttributeTemplate(TypeToken.IP_ADDR, name, ord);
		}

		internal static AttributeTemplate CreateTemplateForVariable(string name, int ord, long value)
		{
			return new AttributeTemplate(TypeToken.INT64, name, ord);
		}

		internal static AttributeTemplate CreateTemplateForVariable(string name, int ord, ulong value)
		{
			return new AttributeTemplate(TypeToken.UINT64, name, ord);
		}

		internal static AttributeTemplate CreateTemplateForVariable(string name, int ord, bool value)
		{
			return new AttributeTemplate(TypeToken.BOOLEAN, name, ord);
		}

		internal int BinaryEncode(byte[] buffer, ref int offset, Encoder encoder)
		{
			/* Encoded using the following protocol:
			 *
			 * ATTRIBUTEWORD,TYPETOKEN
			 */
			int count = 0, ofs = offset;

			count += LwesSerializer.WriteATTRIBUTEWORD(buffer, ref ofs, _name, Constants.DefaultEncoding.GetEncoder());
			count += LwesSerializer.Write(buffer, ref ofs, (byte)_typeToken);

			offset = ofs;
			return count;
		}

		internal int GetByteCount()
		{
			// [1-byte-length-prefix][1-255-byte-attributeword][1-byte-typetoken]
			return Constants.DefaultEncoding.GetByteCount(_name) + 2;
		}

		#endregion Methods
	}
}