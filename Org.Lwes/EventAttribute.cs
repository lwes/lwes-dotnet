namespace Org.Lwes
{
	using System;
	using System.Net;
	using System.Text;

	using Org.Lwes.ESF;

	/// <summary>
	/// Interface for working with LWES event attributes values.
	/// </summary>
	public interface IEventAttribute
	{
		#region Properties

		/// <summary>
		/// Indicates the attribute's value equals the default value for the attribute.
		/// </summary>
		bool HasDefaultValue
		{
			get;
		}

		/// <summary>
		/// The attribute's type name.
		/// </summary>
		string TypeName
		{
			get;
		}

		/// <summary>
		/// The attribute's type token.
		/// </summary>
		TypeToken TypeToken
		{
			get;
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Used by the LWES event system during serialization. Encodes
		/// the event attribute into the byte buffer.
		/// </summary>
		/// <param name="buffer">destination buffer</param>
		/// <param name="ofs">offset into the buffer where the attribute
		/// will be encoded</param>
		/// <param name="encoder">character encoder used for transforming
		/// character data to bytes</param>
		/// <returns>number of bytes used during the encoding</returns>
		int BinaryEncode(byte[] buffer, ref int ofs, Encoder encoder);

		/// <summary>
		/// Used by the LWES event system during serialization. Determines
		/// the number of bytes required to encode the attribute.
		/// </summary>
		/// <param name="enc">character encoder used for calculating the
		/// number of bytes required for character data</param>
		/// <returns>size in bytes of the value when encoded</returns>
		int GetByteCount(Encoding enc);

		/// <summary>
		/// Gets the value by conversion to type V.
		/// </summary>
		/// <typeparam name="V">value type V.</typeparam>
		/// <returns>the value, converted to type V.</returns>
		/// <exception cref="InvalidCastException">thrown if the value cannot be converted</exception>
		V GetValue<V>();

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		IEventAttribute Mutate(EventTemplate ev, ushort value);

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		IEventAttribute Mutate(EventTemplate ev, short value);

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		IEventAttribute Mutate(EventTemplate ev, uint value);

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		IEventAttribute Mutate(EventTemplate ev, int value);

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		IEventAttribute Mutate(EventTemplate ev, string value);

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		IEventAttribute Mutate(EventTemplate ev, IPAddress value);

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		IEventAttribute Mutate(EventTemplate ev, long value);

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		IEventAttribute Mutate(EventTemplate ev, ulong value);

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		IEventAttribute Mutate(EventTemplate ev, bool value);

		/// <summary>
		/// Tries to get the attribute's value as a UInt16
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		bool TryGetValue(out ushort value);

		/// <summary>
		/// Tries to get the attribute's value as a Int16
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		bool TryGetValue(out short value);

		/// <summary>
		/// Tries to get the attribute's value as a UInt32
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		bool TryGetValue(out uint value);

		/// <summary>
		/// Tries to get the attribute's value as a Int32
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		bool TryGetValue(out int value);

		/// <summary>
		/// Tries to get the attribute's value as a String
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		bool TryGetValue(out string value);

		/// <summary>
		/// Tries to get the attribute's value as a IPAddress
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		bool TryGetValue(out IPAddress value);

		/// <summary>
		/// Tries to get the attribute's value as a Int64
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		bool TryGetValue(out long value);

		/// <summary>
		/// Tries to get the attribute's value as a UInt64
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		bool TryGetValue(out ulong value);

		/// <summary>
		/// Tries to get the attribute's value as a Boolean
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		bool TryGetValue(out bool value);

		#endregion Methods
	}

	internal interface IEventAttribute<T> : IEventAttribute
	{
		#region Properties

		T Value
		{
			get;
		}

		#endregion Properties
	}

	/// <summary>
	/// Encapsulates an event attribute's value along with metadata
	/// required by serialization.
	/// </summary>
	[Serializable]
	internal struct EventAttribute<T> : IEventAttribute<T>
	{
		#region Fields

		private static readonly int[] TypeSizeMap = 
			{
				0,
				2, // sizeof(ushort)
				2, // sizeof(short)
				4, // sizeof(uint)
				4, // sizeof(int)
				CalculateEncodedSize, // calculated by encoding
				4, // sizeof IP address
				8, // sizeof(long)
				8, // sizeof(ulong)
				1, // sizeof(byte)
			};

		private const int CalculateEncodedSize = -1;
		private const int InvalidTypeSize = 0;

		AttributeTemplate _template;
		T _value;

		#endregion Fields

		#region Constructors

		public EventAttribute(AttributeTemplate template, T value)
		{
			_template = template;
			_value = value;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Indicates the attribute's value equals the default value for the attribute.
		/// </summary>
		public bool HasDefaultValue
		{
			get { return Object.Equals(_value, default(T)); }
		}

		public string TypeName
		{
			get { return EsfParser.TypeTokenNameMap[(int)_template.TypeToken]; }
		}

		public TypeToken TypeToken
		{
			get { return _template.TypeToken; }
		}

		public T Value
		{
			get { return _value; }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Used by the LWES event system during serialization. Encodes
		/// the event attribute into the byte buffer.
		/// </summary>
		/// <param name="buffer">destination buffer</param>
		/// <param name="offset">offset into the buffer where the attribute
		/// will be encoded</param>
		/// <param name="encoder">character encoder used for transforming
		/// character data to bytes</param>
		/// <returns>number of bytes used during the encoding</returns>
		public int BinaryEncode(byte[] buffer, ref int offset, Encoder encoder)
		{
			/* Encoded using the following protocol:
			 *
			 * ATTRIBUTEWORD,TYPETOKEN,(UINT16|INT16|UINT32|INT32|UINT64|INT64|BOOLEAN|STRING)
			 */
			int ofs = offset;
			int count = _template.BinaryEncode(buffer, ref ofs, encoder);

			switch (_template.TypeToken)
			{
				case TypeToken.UINT16:
					 count += LwesSerializer.Write(buffer, ref ofs, GetValue<UInt16>());
					break;
				case TypeToken.INT16:
					count += LwesSerializer.Write(buffer, ref ofs, GetValue<Int16>());
					break;
				case TypeToken.UINT32:
					count += LwesSerializer.Write(buffer, ref ofs, GetValue<UInt32>());
					break;
				case TypeToken.INT32:
					count += LwesSerializer.Write(buffer, ref ofs, GetValue<Int32>());
					break;
				case TypeToken.STRING:
					count += LwesSerializer.Write(buffer, ref ofs, GetValue<string>(), encoder);
					break;
				case TypeToken.IP_ADDR:
					uint addr;
					Coercion.TryCoerce(GetValue<IPAddress>(), out addr);
					count += LwesSerializer.Write(buffer, ref ofs, addr);
					break;
				case TypeToken.INT64:
					count += LwesSerializer.Write(buffer, ref ofs, GetValue<Int64>());
					break;
				case TypeToken.UINT64:
					count += LwesSerializer.Write(buffer, ref ofs, GetValue<UInt64>());
					break;
				case TypeToken.BOOLEAN:
					count += LwesSerializer.Write(buffer, ref ofs, GetValue<bool>());
					break;
				default:
					throw new InvalidOperationException();
			}

			offset = ofs;
			return count;
		}

		/// <summary>
		/// Used by the LWES event system during serialization. Determines
		/// the number of bytes required to encode the attribute.
		/// </summary>
		/// <param name="enc">character encoder used for calculating the
		/// number of bytes required for character data</param>
		/// <returns>size in bytes of the value when encoded</returns>
		public int GetByteCount(Encoding enc)
		{
			int size = TypeSizeMap[(int)_template.TypeToken];
			if (size == CalculateEncodedSize)
			{
				size = enc.GetByteCount(GetValue<string>()) + sizeof(UInt16);
			}
			if (size > 0)
			{
				return _template.GetByteCount() + size;
			}
			throw new InvalidOperationException("Cannot calculate byte size for an undefined type");
		}

		/// <summary>
		/// Gets the value by conversion to type V.
		/// </summary>
		/// <typeparam name="V">value type V.</typeparam>
		/// <returns>the value, converted to type V.</returns>
		/// <exception cref="InvalidCastException">thrown if the value cannot be converted</exception>
		public V GetValue<V>()
		{
			if (typeof(T).IsInstanceOfType(_value))
			{
				return (V)((object)_value); // Implicit conversion by boxing/unboxing
			}
			else
			{
				return (V)Convert.ChangeType(_value, typeof(T));
			}
		}

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		public IEventAttribute Mutate(EventTemplate ev, ushort value)
		{
			return EventAttribute.MutateByCoercion(ev, _template, value);
		}

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		public IEventAttribute Mutate(EventTemplate ev, short value)
		{
			return EventAttribute.MutateByCoercion(ev, _template, value);
		}

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		public IEventAttribute Mutate(EventTemplate ev, uint value)
		{
			return EventAttribute.MutateByCoercion(ev, _template, value);
		}

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		public IEventAttribute Mutate(EventTemplate ev, int value)
		{
			return EventAttribute.MutateByCoercion(ev, _template, value);
		}

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		public IEventAttribute Mutate(EventTemplate ev, string value)
		{
			return EventAttribute.MutateByCoercion(ev, _template, value);
		}

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		public IEventAttribute Mutate(EventTemplate ev, IPAddress value)
		{
			return EventAttribute.MutateByCoercion(ev, _template, value);
		}

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		public IEventAttribute Mutate(EventTemplate ev, ulong value)
		{
			return EventAttribute.MutateByCoercion(ev, _template, value);
		}

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		public IEventAttribute Mutate(EventTemplate ev, long value)
		{
			return EventAttribute.MutateByCoercion(ev, _template, value);
		}

		/// <summary>
		/// Mutates the attribute's value.
		/// </summary>
		/// <param name="ev">the attribute's event template</param>
		/// <param name="value">a new value</param>
		/// <returns>An event attribute containing the value given.</returns>
		public IEventAttribute Mutate(EventTemplate ev, bool value)
		{
			return EventAttribute.MutateByCoercion(ev, _template, value);
		}

		public override string ToString()
		{
			return String.Concat(_template.Name
				, ": { Name: '", _template.Name
				, "', Type: ", _template.TypeToken
				, ", Value: ", EventAttribute.ConvertValueToString(_template, _value)
				, "}");
		}

		/// <summary>
		/// Tries to get the attribute's value as a Boolean
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		public bool TryGetValue(out ushort value)
		{
			return Coercion.TryCoerce(_template.TypeToken, _value, out value);
		}

		/// <summary>
		/// Tries to get the attribute's value as a Boolean
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		public bool TryGetValue(out short value)
		{
			return Coercion.TryCoerce(_template.TypeToken, _value, out value);
		}

		/// <summary>
		/// Tries to get the attribute's value as a Boolean
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		public bool TryGetValue(out uint value)
		{
			return Coercion.TryCoerce(_template.TypeToken, _value, out value);
		}

		/// <summary>
		/// Tries to get the attribute's value as a Boolean
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		public bool TryGetValue(out int value)
		{
			return Coercion.TryCoerce(_template.TypeToken, _value, out value);
		}

		/// <summary>
		/// Tries to get the attribute's value as a Boolean
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		public bool TryGetValue(out string value)
		{
			return Coercion.TryCoerce(_template.TypeToken, _value, out value);
		}

		/// <summary>
		/// Tries to get the attribute's value as a Boolean
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		public bool TryGetValue(out IPAddress value)
		{
			return Coercion.TryCoerce(_template.TypeToken, _value, out value);
		}

		/// <summary>
		/// Tries to get the attribute's value as a Boolean
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		public bool TryGetValue(out long value)
		{
			return Coercion.TryCoerce(_template.TypeToken, _value, out value);
		}

		/// <summary>
		/// Tries to get the attribute's value as a Boolean
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		public bool TryGetValue(out ulong value)
		{
			return Coercion.TryCoerce(_template.TypeToken, _value, out value);
		}

		/// <summary>
		/// Tries to get the attribute's value as a Boolean
		/// </summary>
		/// <param name="value">reference to a variable that will hold the value upon success</param>
		/// <returns><em>true</em> if the value is retrieved; otherwise <em>false</em></returns>
		public bool TryGetValue(out bool value)
		{
			return Coercion.TryCoerce(_template.TypeToken, _value, out value);
		}

		#endregion Methods
	}

	internal static class EventAttribute
	{
		#region Methods

		/// <summary>
		/// Creates an event attribute from a template and a value.
		/// </summary>
		/// <typeparam name="V">value type V</typeparam>
		/// <param name="template">an attribute template</param>
		/// <param name="value">a value for the attribute</param>
		/// <returns>a new event attribute</returns>
		public static IEventAttribute Create<V>(AttributeTemplate template, V value)
		{
			return new EventAttribute<V>(template, value);
		}

		/// <summary>
		/// Creates an event attribute from a template. The new attribute will
		/// have a default value.
		/// </summary>
		/// <param name="template">an attribute template</param>
		/// <returns>a new event attribute</returns>
		public static IEventAttribute Create(AttributeTemplate template)
		{
			switch (template.TypeToken)
			{
				case TypeToken.UINT16: return new EventAttribute<UInt16>(template, default(UInt16));
				case TypeToken.INT16: return new EventAttribute<Int16>(template, default(Int16));
				case TypeToken.UINT32: return new EventAttribute<UInt32>(template, default(UInt32));
				case TypeToken.INT32: return new EventAttribute<Int32>(template, default(Int32));
				case TypeToken.STRING: return new EventAttribute<string>(template, String.Empty);
				case TypeToken.IP_ADDR: return new EventAttribute<IPAddress>(template, IPAddress.Any);
				case TypeToken.INT64: return new EventAttribute<Int64>(template, default(Int64));
				case TypeToken.UINT64: return new EventAttribute<UInt64>(template, default(UInt64));
				case TypeToken.BOOLEAN: return new EventAttribute<bool>(template, default(bool));
			}
			throw new InvalidOperationException("cannot create an event value for an unknown attribute type");
		}

		internal static string ConvertValueToString<V>(AttributeTemplate attr, V value)
		{
			if (attr.TypeToken == TypeToken.STRING)
				return String.Concat("'", value.ToString().Replace("'", "\\'"), "'");
			else if (attr.TypeToken == TypeToken.UNDEFINED)
				return "undefined";
			else
				return value.ToString();
		}

		internal static IEventAttribute MutateByCoercion(EventTemplate ev, AttributeTemplate attr, ushort value)
		{
			TypeToken tt = attr.TypeToken;
			switch (tt)
			{
				case TypeToken.UINT16: return new EventAttribute<UInt16>(attr, value);
				case TypeToken.INT16:
					short sh;
					if (Coercion.TryCoerce(value, out sh))
					{
						return new EventAttribute<Int16>(attr, sh);
					}
					break;
				case TypeToken.UINT32:
					uint ui;
					if (Coercion.TryCoerce(value, out ui))
					{
						return new EventAttribute<UInt32>(attr, ui);
					}
					break;
				case TypeToken.INT32:
					uint i;
					if (Coercion.TryCoerce(value, out i))
					{
						return new EventAttribute<UInt32>(attr, i);
					}
					break;
				case TypeToken.STRING:
					string s;
					if (Coercion.TryCoerce(value, out s))
					{
						return new EventAttribute<string>(attr, s);
					}
					break;
				case TypeToken.IP_ADDR:
					string ip;
					if (Coercion.TryCoerce(value, out ip))
					{
						return new EventAttribute<string>(attr, ip);
					}
					break;
				case TypeToken.INT64:
					string l;
					if (Coercion.TryCoerce(value, out l))
					{
						return new EventAttribute<string>(attr, l);
					}
					break;
				case TypeToken.UINT64:
					string ul;
					if (Coercion.TryCoerce(value, out ul))
					{
						return new EventAttribute<string>(attr, ul);
					}
					break;
				case TypeToken.BOOLEAN:
					string b;
					if (Coercion.TryCoerce(value, out b))
					{
						return new EventAttribute<string>(attr, b);
					}
					break;
			}
			throw new NoSuchAttributeTypeException(string.Concat("type mismatch attempting to set "
				, ev.Name, ".", attr.Name, ": expecting " + EsfParser.TypeTokenNameMap[(int)tt]
				, ", received non-coercible type UInt16")
				);
		}

		internal static IEventAttribute MutateByCoercion(EventTemplate ev, AttributeTemplate attr, short value)
		{
			TypeToken tt = attr.TypeToken;
			switch (tt)
			{
				case TypeToken.UINT16:
					ushort sh;
					if (Coercion.TryCoerce(value, out sh))
					{
						return new EventAttribute<UInt16>(attr, sh);
					}
					break;
				case TypeToken.INT16: return new EventAttribute<Int16>(attr, value);
				case TypeToken.UINT32:
					uint ui;
					if (Coercion.TryCoerce(value, out ui))
					{
						return new EventAttribute<UInt32>(attr, ui);
					}
					break;
				case TypeToken.INT32:
					uint i;
					if (Coercion.TryCoerce(value, out i))
					{
						return new EventAttribute<UInt32>(attr, i);
					}
					break;
				case TypeToken.STRING:
					string s;
					if (Coercion.TryCoerce(value, out s))
					{
						return new EventAttribute<string>(attr, s);
					}
					break;
				case TypeToken.IP_ADDR:
					string ip;
					if (Coercion.TryCoerce(value, out ip))
					{
						return new EventAttribute<string>(attr, ip);
					}
					break;
				case TypeToken.INT64:
					string l;
					if (Coercion.TryCoerce(value, out l))
					{
						return new EventAttribute<string>(attr, l);
					}
					break;
				case TypeToken.UINT64:
					string ul;
					if (Coercion.TryCoerce(value, out ul))
					{
						return new EventAttribute<string>(attr, ul);
					}
					break;
				case TypeToken.BOOLEAN:
					string b;
					if (Coercion.TryCoerce(value, out b))
					{
						return new EventAttribute<string>(attr, b);
					}
					break;
			}
			throw new NoSuchAttributeTypeException(string.Concat("type mismatch attempting to set "
				, ev.Name, ".", attr.Name, ": expecting " + EsfParser.TypeTokenNameMap[(int)tt]
				, ", received non-coercible type Int16")
				);
		}

		internal static IEventAttribute MutateByCoercion(EventTemplate ev, AttributeTemplate attr, int value)
		{
			TypeToken tt = attr.TypeToken;
			switch (tt)
			{
				case TypeToken.UINT16:
					ushort ush;
					if (Coercion.TryCoerce(value, out ush))
					{
						return new EventAttribute<UInt16>(attr, ush);
					}
					break;
				case TypeToken.INT16:
					short sh;
					if (Coercion.TryCoerce(value, out sh))
					{
						return new EventAttribute<Int16>(attr, sh);
					}
					break;
				case TypeToken.UINT32: return new EventAttribute<Int32>(attr, value);
				case TypeToken.INT32:
					uint i;
					if (Coercion.TryCoerce(value, out i))
					{
						return new EventAttribute<UInt32>(attr, i);
					}
					break;
				case TypeToken.STRING:
					string s;
					if (Coercion.TryCoerce(value, out s))
					{
						return new EventAttribute<string>(attr, s);
					}
					break;
				case TypeToken.IP_ADDR:
					string ip;
					if (Coercion.TryCoerce(value, out ip))
					{
						return new EventAttribute<string>(attr, ip);
					}
					break;
				case TypeToken.INT64:
					string l;
					if (Coercion.TryCoerce(value, out l))
					{
						return new EventAttribute<string>(attr, l);
					}
					break;
				case TypeToken.UINT64:
					string ul;
					if (Coercion.TryCoerce(value, out ul))
					{
						return new EventAttribute<string>(attr, ul);
					}
					break;
				case TypeToken.BOOLEAN:
					string b;
					if (Coercion.TryCoerce(value, out b))
					{
						return new EventAttribute<string>(attr, b);
					}
					break;
			}
			throw new NoSuchAttributeTypeException(string.Concat("type mismatch attempting to set "
				, ev.Name, ".", attr.Name, ": expecting " + EsfParser.TypeTokenNameMap[(int)tt]
				, ", received non-coercible type Int16")
				);
		}

		internal static IEventAttribute MutateByCoercion(EventTemplate ev, AttributeTemplate attr, uint value)
		{
			TypeToken tt = attr.TypeToken;
			switch (tt)
			{
				case TypeToken.UINT16:
					ushort ush;
					if (Coercion.TryCoerce(value, out ush))
					{
						return new EventAttribute<UInt16>(attr, ush);
					}
					break;
				case TypeToken.INT16:
					short sh;
					if (Coercion.TryCoerce(value, out sh))
					{
						return new EventAttribute<Int16>(attr, sh);
					}
					break;
				case TypeToken.UINT32: return new EventAttribute<UInt32>(attr, value);
				case TypeToken.INT32:
					uint i;
					if (Coercion.TryCoerce(value, out i))
					{
						return new EventAttribute<UInt32>(attr, i);
					}
					break;
				case TypeToken.STRING:
					string s;
					if (Coercion.TryCoerce(value, out s))
					{
						return new EventAttribute<string>(attr, s);
					}
					break;
				case TypeToken.IP_ADDR:
					string ip;
					if (Coercion.TryCoerce(value, out ip))
					{
						return new EventAttribute<string>(attr, ip);
					}
					break;
				case TypeToken.INT64:
					string l;
					if (Coercion.TryCoerce(value, out l))
					{
						return new EventAttribute<string>(attr, l);
					}
					break;
				case TypeToken.UINT64:
					string ul;
					if (Coercion.TryCoerce(value, out ul))
					{
						return new EventAttribute<string>(attr, ul);
					}
					break;
				case TypeToken.BOOLEAN:
					string b;
					if (Coercion.TryCoerce(value, out b))
					{
						return new EventAttribute<string>(attr, b);
					}
					break;
			}
			throw new NoSuchAttributeTypeException(string.Concat("type mismatch attempting to set "
				, ev.Name, ".", attr.Name, ": expecting " + EsfParser.TypeTokenNameMap[(int)tt]
				, ", received non-coercible type UInt16")
				);
		}

		internal static IEventAttribute MutateByCoercion(EventTemplate ev, AttributeTemplate attr, string value)
		{
			TypeToken tt = attr.TypeToken;
			switch (tt)
			{
				case TypeToken.UINT16:
					ushort ush;
					if (Coercion.TryCoerce(value, out ush))
					{
						return new EventAttribute<UInt16>(attr, ush);
					}
					break;
				case TypeToken.INT16:
					short sh;
					if (Coercion.TryCoerce(value, out sh))
					{
						return new EventAttribute<Int16>(attr, sh);
					}
					break;
				case TypeToken.UINT32:
					uint ui;
					if (Coercion.TryCoerce(value, out ui))
					{
						return new EventAttribute<UInt32>(attr, ui);
					}
					break;
				case TypeToken.INT32:
					uint i;
					if (Coercion.TryCoerce(value, out i))
					{
						return new EventAttribute<UInt32>(attr, i);
					}
					break;
				case TypeToken.STRING: return new EventAttribute<string>(attr, value);
				case TypeToken.IP_ADDR:
					IPAddress ip;
					if (Coercion.TryCoerce(value, out ip))
					{
						return new EventAttribute<IPAddress>(attr, ip);
					}
					break;
				case TypeToken.INT64:
					long l;
					if (Coercion.TryCoerce(value, out l))
					{
						return new EventAttribute<long>(attr, l);
					}
					break;
				case TypeToken.UINT64:
					ulong ul;
					if (Coercion.TryCoerce(value, out ul))
					{
						return new EventAttribute<ulong>(attr, ul);
					}
					break;
				case TypeToken.BOOLEAN:
					bool b;
					if (Coercion.TryCoerce(value, out b))
					{
						return new EventAttribute<bool>(attr, b);
					}
					break;
			}
			throw new NoSuchAttributeTypeException(string.Concat("type mismatch attempting to set "
				, ev.Name, ".", attr.Name, ": expecting " + EsfParser.TypeTokenNameMap[(int)tt]
				, ", received non-coercible type string")
				);
		}

		internal static IEventAttribute MutateByCoercion(EventTemplate ev, AttributeTemplate attr, IPAddress value)
		{
			TypeToken tt = attr.TypeToken;
			switch (tt)
			{
				case TypeToken.UINT16:
					ushort ush;
					if (Coercion.TryCoerce(value, out ush))
					{
						return new EventAttribute<UInt16>(attr, ush);
					}
					break;
				case TypeToken.INT16:
					short sh;
					if (Coercion.TryCoerce(value, out sh))
					{
						return new EventAttribute<Int16>(attr, sh);
					}
					break;
				case TypeToken.UINT32:
					uint ui;
					if (Coercion.TryCoerce(value, out ui))
					{
						return new EventAttribute<UInt32>(attr, ui);
					}
					break;
				case TypeToken.INT32:
					uint i;
					if (Coercion.TryCoerce(value, out i))
					{
						return new EventAttribute<UInt32>(attr, i);
					}
					break;
				case TypeToken.STRING:
					string s;
					if (Coercion.TryCoerce(value, out s))
					{
						return new EventAttribute<string>(attr, s);
					}
					break;
				case TypeToken.IP_ADDR: return new EventAttribute<IPAddress>(attr, value);
				case TypeToken.INT64:
					string l;
					if (Coercion.TryCoerce(value, out l))
					{
						return new EventAttribute<string>(attr, l);
					}
					break;
				case TypeToken.UINT64:
					string ul;
					if (Coercion.TryCoerce(value, out ul))
					{
						return new EventAttribute<string>(attr, ul);
					}
					break;
				case TypeToken.BOOLEAN:
					string b;
					if (Coercion.TryCoerce(value, out b))
					{
						return new EventAttribute<string>(attr, b);
					}
					break;
			}
			throw new NoSuchAttributeTypeException(string.Concat("type mismatch attempting to set "
				, ev.Name, ".", attr.Name, ": expecting " + EsfParser.TypeTokenNameMap[(int)tt]
				, ", received non-coercible type IPAddress")
				);
		}

		internal static IEventAttribute MutateByCoercion(EventTemplate ev, AttributeTemplate attr, long value)
		{
			TypeToken tt = attr.TypeToken;
			switch (tt)
			{
				case TypeToken.UINT16:
					ushort ush;
					if (Coercion.TryCoerce(value, out ush))
					{
						return new EventAttribute<UInt16>(attr, ush);
					}
					break;
				case TypeToken.INT16:
					short sh;
					if (Coercion.TryCoerce(value, out sh))
					{
						return new EventAttribute<Int16>(attr, sh);
					}
					break;
				case TypeToken.UINT32:
					uint ui;
					if (Coercion.TryCoerce(value, out ui))
					{
						return new EventAttribute<UInt32>(attr, ui);
					}
					break;
				case TypeToken.INT32:
					uint i;
					if (Coercion.TryCoerce(value, out i))
					{
						return new EventAttribute<UInt32>(attr, i);
					}
					break;
				case TypeToken.STRING:
					string s;
					if (Coercion.TryCoerce(value, out s))
					{
						return new EventAttribute<string>(attr, s);
					}
					break;
				case TypeToken.IP_ADDR:
					string ip;
					if (Coercion.TryCoerce(value, out ip))
					{
						return new EventAttribute<string>(attr, ip);
					}
					break;
				case TypeToken.INT64: return new EventAttribute<Int64>(attr, value);
				case TypeToken.UINT64:
					string ul;
					if (Coercion.TryCoerce(value, out ul))
					{
						return new EventAttribute<string>(attr, ul);
					}
					break;
				case TypeToken.BOOLEAN:
					string b;
					if (Coercion.TryCoerce(value, out b))
					{
						return new EventAttribute<string>(attr, b);
					}
					break;
			}
			throw new NoSuchAttributeTypeException(string.Concat("type mismatch attempting to set "
				, ev.Name, ".", attr.Name, ": expecting " + EsfParser.TypeTokenNameMap[(int)tt]
				, ", received non-coercible type Int64")
				);
		}

		internal static IEventAttribute MutateByCoercion(EventTemplate ev, AttributeTemplate attr, ulong value)
		{
			TypeToken tt = attr.TypeToken;
			switch (tt)
			{
				case TypeToken.UINT16:
					ushort ush;
					if (Coercion.TryCoerce(value, out ush))
					{
						return new EventAttribute<UInt16>(attr, ush);
					}
					break;
				case TypeToken.INT16:
					short sh;
					if (Coercion.TryCoerce(value, out sh))
					{
						return new EventAttribute<Int16>(attr, sh);
					}
					break;
				case TypeToken.UINT32:
					uint ui;
					if (Coercion.TryCoerce(value, out ui))
					{
						return new EventAttribute<UInt32>(attr, ui);
					}
					break;
				case TypeToken.INT32:
					uint i;
					if (Coercion.TryCoerce(value, out i))
					{
						return new EventAttribute<UInt32>(attr, i);
					}
					break;
				case TypeToken.STRING:
					string s;
					if (Coercion.TryCoerce(value, out s))
					{
						return new EventAttribute<string>(attr, s);
					}
					break;
				case TypeToken.IP_ADDR:
					string ip;
					if (Coercion.TryCoerce(value, out ip))
					{
						return new EventAttribute<string>(attr, ip);
					}
					break;
				case TypeToken.INT64:
					string l;
					if (Coercion.TryCoerce(value, out l))
					{
						return new EventAttribute<string>(attr, l);
					}
					break;
				case TypeToken.UINT64: return new EventAttribute<UInt64>(attr, value);
				case TypeToken.BOOLEAN:
					string b;
					if (Coercion.TryCoerce(value, out b))
					{
						return new EventAttribute<string>(attr, b);
					}
					break;
			}
			throw new NoSuchAttributeTypeException(string.Concat("type mismatch attempting to set "
				, ev.Name, ".", attr.Name, ": expecting " + EsfParser.TypeTokenNameMap[(int)tt]
				, ", received non-coercible type UInt64")
				);
		}

		internal static IEventAttribute MutateByCoercion(EventTemplate ev, AttributeTemplate attr, bool value)
		{
			TypeToken tt = attr.TypeToken;
			switch (tt)
			{
				case TypeToken.UINT16:
					ushort ush;
					if (Coercion.TryCoerce(value, out ush))
					{
						return new EventAttribute<UInt16>(attr, ush);
					}
					break;
				case TypeToken.INT16:
					short sh;
					if (Coercion.TryCoerce(value, out sh))
					{
						return new EventAttribute<Int16>(attr, sh);
					}
					break;
				case TypeToken.UINT32:
					uint ui;
					if (Coercion.TryCoerce(value, out ui))
					{
						return new EventAttribute<UInt32>(attr, ui);
					}
					break;
				case TypeToken.INT32:
					uint i;
					if (Coercion.TryCoerce(value, out i))
					{
						return new EventAttribute<UInt32>(attr, i);
					}
					break;
				case TypeToken.STRING:
					string s;
					if (Coercion.TryCoerce(value, out s))
					{
						return new EventAttribute<string>(attr, s);
					}
					break;
				case TypeToken.IP_ADDR:
					string ip;
					if (Coercion.TryCoerce(value, out ip))
					{
						return new EventAttribute<string>(attr, ip);
					}
					break;
				case TypeToken.INT64:
					string l;
					if (Coercion.TryCoerce(value, out l))
					{
						return new EventAttribute<string>(attr, l);
					}
					break;
				case TypeToken.UINT64:
					string ul;
					if (Coercion.TryCoerce(value, out ul))
					{
						return new EventAttribute<string>(attr, ul);
					}
					break;
				case TypeToken.BOOLEAN: return new EventAttribute<bool>(attr, value);
			}
			throw new NoSuchAttributeTypeException(string.Concat("type mismatch attempting to set "
				, ev.Name, ".", attr.Name, ": expecting " + EsfParser.TypeTokenNameMap[(int)tt]
				, ", received non-coercible type Boolean")
				);
		}

		#endregion Methods
	}
}