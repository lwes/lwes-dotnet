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
// Lesser GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Text;

	using Org.Lwes.DB;
	using Org.Lwes.ESF;
	using Org.Lwes.Properties;

	/// <summary>
	/// Event base class.
	/// </summary>
	[Serializable]
	public class Event
	{
		#region Fields

		List<IEventAttribute> _attributes;
		SupportedEncoding _encoding;
		Object _sync = new Object();
		EventTemplate _template;
		bool _validating;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new event with the given name.
		/// </summary>
		public Event(string evName)
			: this(new EventTemplate(false, evName), false, Constants.CDefaultSupportedEncoding)
		{
		}

		/// <summary>
		/// Creates a new instance with an encoding.
		/// </summary>
		/// <param name="enc">An encoding for the event.</param>
		public Event(SupportedEncoding enc)
			: this(EventTemplate.Empty, false, enc)
		{
		}

		/// <summary>
		/// Creates a new event.
		/// </summary>
		/// <param name="evTemplate">the event's template</param>
		/// <param name="validating"><em>true</em> if validating should be performed on the event;
		/// otherwise <em>false</em></param>
		/// <param name="enc">indicates the encoding to use for the event's character data</param>
		public Event(EventTemplate evTemplate, bool validating, SupportedEncoding enc)
		{
			_template = evTemplate;
			_validating = validating;
			_encoding = enc;
			_attributes = new List<IEventAttribute>(evTemplate.Count);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the attribute count.
		/// </summary>
		public int AttributeCount
		{
			get { return _template.Count; }
		}

		/// <summary>
		/// Gets the encoding used when the event is encoded for binary transmission.
		/// </summary>
		public SupportedEncoding Encoding
		{
			get { return _encoding; }
		}

		/// <summary>
		/// The event's name.
		/// </summary>
		public string Name
		{
			get { return _template.Name; }
		}

		#endregion Properties

		#region Indexers

		/// <summary>
		/// Gets an attribute by name.
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <returns>the attribute if it exists; otherwise an exception is thrown</returns>
		/// <exception cref="ArgumentOutOfRangeException">thrown if an attribute with the given <paramref name="name"/> doesn't exist</exception>
		public IEventAttribute this[string name]
		{
			get
			{
				int ord;
				if (!_template.TryGetOrdinal(name, out ord))
					throw new ArgumentOutOfRangeException("name", String.Format(Resources.Error_AttributeNotDefined, name));
				return _attributes[ord];
			}
		}

		/// <summary>
		/// Gets an attribute by ordinal position.
		/// </summary>
		/// <param name="ord">ordinal position of the attribute</param>
		/// <returns>an attribute at the <paramref name="ordinal"/> position given</returns>
		/// <exception cref="ArgumentOutOfRangeException">thrown if the <paramref name="ordinal"/> is out of range</exception>
		public IEventAttribute this[int ord]
		{
			get
			{
				if (0 > ord || ord >= _attributes.Count)
					throw new ArgumentOutOfRangeException("ordinal");
				return _attributes[ord];
			}
		}

		#endregion Indexers

		#region Methods

		/// <summary>
		/// Gets an attribute's value.
		/// </summary>
		/// <typeparam name="V">value type V</typeparam>
		/// <param name="name">attribute's name.</param>
		/// <returns>The value of the attribute, as type V</returns>
		/// <exception cref="InvalidCastException">if the attribute's value cannot be
		/// coerced into value type V</exception>
		/// <exception cref="NoSuchAttributeException">thrown if an attribute with the given name
		/// does not exist.</exception>
		public V GetValue<V>(string name)
		{
			int ord;
			if (_template.TryGetOrdinal(name, out ord))
			{
				return _attributes[ord].GetValue<V>();
			}
			throw new ArgumentOutOfRangeException("name", String.Format(Resources.Error_AttributeNotDefined, name));
		}

		/// <summary>
		/// Sets an attribute's value.
		/// </summary>
		/// <param name="name">the attribute's name</param>
		/// <param name="value">the attribute's UInt16 value</param>
		public void SetValue(string name, UInt16 value)
		{
			int ord;
			AttributeTemplate attr;
			if (_template.TryGetOrdinal(name, out ord))
			{
				attr = _template[ord];

				// Validate if necessary
				if (_validating && !attr.IsAssignable(value))
					throw new AttributeNotSetException("name");

				_attributes[ord] = _attributes[ord].Mutate(_template, value);
			}

			if (_validating)
			{
				// Allow for SiteID or SenderPort inherited from MetaInfoEvent
				if (!String.Equals(Constants.MetaEventInfoAttributes.SiteID.Name, name)
					&& !String.Equals(Constants.MetaEventInfoAttributes.SenderPort.Name, name))
					throw new ArgumentOutOfRangeException("name", String.Format(Resources.Error_AttributeNotDefined, name));
			}

			// Adding a new attribute
			AppendAttribute(AttributeTemplate.CreateTemplateForVariable(name, 0, value), value);
		}

		/// <summary>
		/// Sets an attribute's value.
		/// </summary>
		/// <param name="name">the attribute's name</param>
		/// <param name="value">the attribute's Int16 value</param>
		public void SetValue(string name, Int16 value)
		{
			int ord;
			AttributeTemplate attr;
			if (_template.TryGetOrdinal(name, out ord))
			{
				attr = _template[ord];

				// Validate if necessary
				if (_validating && !attr.IsAssignable(value))
					throw new AttributeNotSetException("name");

				_attributes[ord] = _attributes[ord].Mutate(_template, value);
			}

			// Allow for Encoding inherited from MetaInfoEvent
			if (String.Equals(Constants.MetaEventInfoAttributes.Encoding.Name, name))
			{
				// encoding must be the first attribute (when encoded), make it so
				PrependAttribute(Constants.MetaEventInfoAttributes.Encoding, value);
			}

			if (_validating)
				throw new ArgumentOutOfRangeException("name", String.Format(Resources.Error_AttributeNotDefined, name));

			// Adding a new attribute
			AppendAttribute(AttributeTemplate.CreateTemplateForVariable(name, 0, value), value);
		}

		/// <summary>
		/// Sets an attribute's value.
		/// </summary>
		/// <param name="name">the attribute's name</param>
		/// <param name="value">the attribute's UInt32 value</param>
		public void SetValue(string name, UInt32 value)
		{
			int ord;
			AttributeTemplate attr;
			if (_template.TryGetOrdinal(name, out ord))
			{
				attr = _template[ord];

				// Validate if necessary
				if (_validating && !attr.IsAssignable(value))
					throw new AttributeNotSetException("name");

				_attributes[ord] = _attributes[ord].Mutate(_template, value);
			}

			if (_validating)
				throw new ArgumentOutOfRangeException("name", String.Format(Resources.Error_AttributeNotDefined, name));

			// Adding a new attribute
			AppendAttribute(AttributeTemplate.CreateTemplateForVariable(name, 0, value), value);
		}

		/// <summary>
		/// Sets an attribute's value.
		/// </summary>
		/// <param name="name">the attribute's name</param>
		/// <param name="value">the attribute's Int32 value</param>
		public void SetValue(string name, Int32 value)
		{
			int ord;
			AttributeTemplate attr;
			if (_template.TryGetOrdinal(name, out ord))
			{
				attr = _template[ord];

				// Validate if necessary
				if (_validating && !attr.IsAssignable(value))
					throw new AttributeNotSetException("name");

				_attributes[ord] = _attributes[ord].Mutate(_template, value);
			}

			if (_validating)
				throw new ArgumentOutOfRangeException("name", String.Format(Resources.Error_AttributeNotDefined, name));

			// Adding a new attribute
			AppendAttribute(AttributeTemplate.CreateTemplateForVariable(name, 0, value), value);
		}

		/// <summary>
		/// Sets an attribute's value.
		/// </summary>
		/// <param name="name">the attribute's name</param>
		/// <param name="value">the attribute's String value</param>
		public void SetValue(string name, String value)
		{
			int ord;
			AttributeTemplate attr;
			if (_template.TryGetOrdinal(name, out ord))
			{
				attr = _template[ord];

				// Validate if necessary
				if (_validating && !attr.IsAssignable(value))
					throw new AttributeNotSetException("name");

				_attributes[ord] = _attributes[ord].Mutate(_template, value);
			}

			if (_validating)
				throw new ArgumentOutOfRangeException("name", String.Format(Resources.Error_AttributeNotDefined, name));

			// Adding a new attribute
			AppendAttribute( AttributeTemplate.CreateTemplateForVariable(name, 0, value), value);
		}

		/// <summary>
		/// Sets an attribute's value.
		/// </summary>
		/// <param name="name">the attribute's name</param>
		/// <param name="value">the attribute's IPAddress value</param>
		public void SetValue(string name, IPAddress value)
		{
			int ord;
			AttributeTemplate attr;
			if (_template.TryGetOrdinal(name, out ord))
			{
				attr = _template[ord];

				// Validate if necessary
				if (_validating && !attr.IsAssignable(value))
					throw new AttributeNotSetException("name");

				_attributes[ord] = _attributes[ord].Mutate(_template, value);
			}

			if (_validating)
			{
				// Allow for SenderIP inherited from MetaInfoEvent
				if (!String.Equals(Constants.MetaEventInfoAttributes.SenderIP.Name, name))
					throw new ArgumentOutOfRangeException("name", String.Format(Resources.Error_AttributeNotDefined, name));
			}

			// Adding a new attribute
			AppendAttribute(AttributeTemplate.CreateTemplateForVariable(name, 0, value), value);
		}

		/// <summary>
		/// Sets an attribute's value.
		/// </summary>
		/// <param name="name">the attribute's name</param>
		/// <param name="value">the attribute's Int64 value</param>
		public void SetValue(string name, Int64 value)
		{
			int ord;
			AttributeTemplate attr;
			if (_template.TryGetOrdinal(name, out ord))
			{
				attr = _template[ord];

				// Validate if necessary
				if (_validating && !attr.IsAssignable(value))
					throw new AttributeNotSetException("name");

				_attributes[ord] = _attributes[ord].Mutate(_template, value);
			}

			if (_validating)
			{
				// Allow for ReceiptTime inherited from MetaInfoEvent
				if (!String.Equals(Constants.MetaEventInfoAttributes.ReceiptTime.Name, name))
					throw new ArgumentOutOfRangeException("name", String.Format(Resources.Error_AttributeNotDefined, name));
			}

			// Adding a new attribute
			AppendAttribute(AttributeTemplate.CreateTemplateForVariable(name, 0, value), value);
		}

		/// <summary>
		/// Sets an attribute's value.
		/// </summary>
		/// <param name="name">the attribute's name</param>
		/// <param name="value">the attribute's UInt64 value</param>
		public void SetValue(string name, UInt64 value)
		{
			int ord;
			AttributeTemplate attr;
			if (_template.TryGetOrdinal(name, out ord))
			{
				attr = _template[ord];

				// Validate if necessary
				if (_validating && !attr.IsAssignable(value))
					throw new AttributeNotSetException("name");

				_attributes[ord] = _attributes[ord].Mutate(_template, value);
			}

			if (_validating)
				throw new ArgumentOutOfRangeException("name", String.Format(Resources.Error_AttributeNotDefined, name));

			// Adding a new attribute
			AppendAttribute(AttributeTemplate.CreateTemplateForVariable(name, 0, value), value);
		}

		/// <summary>
		/// Sets an attribute's value.
		/// </summary>
		/// <param name="name">the attribute's name</param>
		/// <param name="value">the attribute's Boolean value</param>
		public void SetValue(string name, Boolean value)
		{
			int ord;
			AttributeTemplate attr;
			if (_template.TryGetOrdinal(name, out ord))
			{
				attr = _template[ord];

				// Validate if necessary
				if (_validating && !attr.IsAssignable(value))
					throw new AttributeNotSetException("name");

				_attributes[ord] = _attributes[ord].Mutate(_template, value);
			}

			if (_validating)
				throw new ArgumentOutOfRangeException("name", String.Format(Resources.Error_AttributeNotDefined, name));

			// Adding a new attribute
			AppendAttribute( AttributeTemplate.CreateTemplateForVariable(name, 0, value), value);
		}

		/// <summary>
		/// Converts the event to a string.
		/// </summary>
		/// <returns>String containing the event.</returns>
		public override string ToString()
		{
			StringBuilder s = new StringBuilder(400);
			s.Append(_template.Name).Append(": { Name: '").Append(_template.Name)
				.Append("', FromESF: ").Append(_template.FromEsf)
				.Append(", Attributes: {");
			if (_attributes != null)
			{
				int i = 0;
				foreach (var a in _attributes)
				{
					if (i++ > 0) s.Append(", ").Append(a);
					else s.Append(a);
				}
			}
			return s.Append("}}").ToString();
		}

		/// <summary>
		/// Converts the event to a string.
		/// </summary>
		/// <param name="humanReadable">Whether the output should be formatted for human readability</param>
		/// <returns>string contianing the event</returns>
		public string ToString(bool humanReadable)
		{
			if (!humanReadable) return ToString();

			string nl = Environment.NewLine;

			StringBuilder s = new StringBuilder(400);
			s.Append(_template.Name).Append(":").Append(nl).Append(" { Name: '").Append(_template.Name)
				.Append("'").Append(nl).Append("  , FromESF: ").Append(_template.FromEsf)
				.Append(nl).Append("  , Attributes: {");
			if (_attributes != null)
			{
				int i = 0;
				foreach (var a in _attributes)
				{
					if (i++ > 0) s.Append(nl).Append("    , ").Append(a);
					else s.Append(a);
				}
			}
			return s.Append(nl).Append("  }").Append(nl).Append(" }").ToString();
		}

		/// <summary>
		/// Tries to get an attribute.
		/// </summary>
		/// <param name="name">attribute's name</param>
		/// <param name="value">reference to a variable where the attribute will be stored upon success</param>
		/// <returns><em>true</em> if the attribute exists; otherwise <em>false</em></returns>
		public bool TryGetAttribute(string name, out IEventAttribute value)
		{
			int ord;
			if (_template.TryGetOrdinal(name, out ord))
			{
				value = _attributes[ord];
				return true;
			}
			value = default(IEventAttribute);
			return false;
		}

		/// <summary>
		/// Tries to get an attribute's value as a UInt16.
		/// </summary>
		/// <param name="attr">the attribute's name</param>
		/// <param name="value">reference to a variable to hold the value</param>
		/// <returns><em>true</em> if the value can be retrieved as a UInt16, otherwise <em>false</em></returns>
		public bool TryGetValue(string attr, out ushort value)
		{
			int ord;
			if (_template.TryGetOrdinal(attr, out ord))
			{
				return _attributes[ord].TryGetValue(out value);
			}
			value = default(ushort);
			return false;
		}

		/// <summary>
		/// Tries to get an attribute's value as a Int16.
		/// </summary>
		/// <param name="attr">the attribute's name</param>
		/// <param name="value">reference to a variable to hold the value</param>
		/// <returns><em>true</em> if the value can be retrieved as a Int16, otherwise <em>false</em></returns>
		public bool TryGetValue(string attr, out short value)
		{
			int ord;
			if (_template.TryGetOrdinal(attr, out ord))
			{
				return _attributes[ord].TryGetValue(out value);
			}
			value = default(short);
			return false;
		}

		/// <summary>
		/// Tries to get an attribute's value as a UInt32.
		/// </summary>
		/// <param name="attr">the attribute's name</param>
		/// <param name="value">reference to a variable to hold the value</param>
		/// <returns><em>true</em> if the value can be retrieved as a UInt32, otherwise <em>false</em></returns>
		public bool TryGetValue(string attr, out uint value)
		{
			int ord;
			if (_template.TryGetOrdinal(attr, out ord))
			{
				return _attributes[ord].TryGetValue(out value);
			}
			value = default(uint);
			return false;
		}

		/// <summary>
		/// Tries to get an attribute's value as a Int32.
		/// </summary>
		/// <param name="attr">the attribute's name</param>
		/// <param name="value">reference to a variable to hold the value</param>
		/// <returns><em>true</em> if the value can be retrieved as a Int32, otherwise <em>false</em></returns>
		public bool TryGetValue(string attr, out int value)
		{
			int ord;
			if (_template.TryGetOrdinal(attr, out ord))
			{
				return _attributes[ord].TryGetValue(out value);
			}
			value = default(int);
			return false;
		}

		/// <summary>
		/// Tries to get an attribute's value as a string.
		/// </summary>
		/// <param name="attr">the attribute's name</param>
		/// <param name="value">reference to a variable to hold the value</param>
		/// <returns><em>true</em> if the value can be retrieved as a string, otherwise <em>false</em></returns>
		public bool TryGetValue(string attr, out string value)
		{
			int ord;
			if (_template.TryGetOrdinal(attr, out ord))
			{
				return _attributes[ord].TryGetValue(out value);
			}
			value = default(string);
			return false;
		}

		/// <summary>
		/// Tries to get an attribute's value as a IPAddress.
		/// </summary>
		/// <param name="attr">the attribute's name</param>
		/// <param name="value">reference to a variable to hold the value</param>
		/// <returns><em>true</em> if the value can be retrieved as a IPAddress, otherwise <em>false</em></returns>
		public bool TryGetValue(string attr, out IPAddress value)
		{
			int ord;
			if (_template.TryGetOrdinal(attr, out ord))
			{
				return _attributes[ord].TryGetValue(out value);
			}
			value = default(IPAddress);
			return false;
		}

		/// <summary>
		/// Tries to get an attribute's value as a UInt16.
		/// </summary>
		/// <param name="attr">the attribute's name</param>
		/// <param name="value">reference to a variable to hold the value</param>
		/// <returns><em>true</em> if the value can be retrieved as a UInt16, otherwise <em>false</em></returns>
		public bool TryGetValue(string attr, out long value)
		{
			int ord;
			if (_template.TryGetOrdinal(attr, out ord))
			{
				return _attributes[ord].TryGetValue(out value);
			}
			value = default(long);
			return false;
		}

		/// <summary>
		/// Tries to get an attribute's value as a UInt64.
		/// </summary>
		/// <param name="attr">the attribute's name</param>
		/// <param name="value">reference to a variable to hold the value</param>
		/// <returns><em>true</em> if the value can be retrieved as a UInt64, otherwise <em>false</em></returns>
		public bool TryGetValue(string attr, out ulong value)
		{
			int ord;
			if (_template.TryGetOrdinal(attr, out ord))
			{
				return _attributes[ord].TryGetValue(out value);
			}
			value = default(ulong);
			return false;
		}

		/// <summary>
		/// Tries to get an attribute's value as a Boolean.
		/// </summary>
		/// <param name="attr">the attribute's name</param>
		/// <param name="value">reference to a variable to hold the value</param>
		/// <returns><em>true</em> if the value can be retrieved as a Boolean, otherwise <em>false</em></returns>
		public bool TryGetValue(string attr, out bool value)
		{
			int ord;
			if (_template.TryGetOrdinal(attr, out ord))
			{
				return _attributes[ord].TryGetValue(out value);
			}
			value = default(bool);
			return false;
		}

		internal static Event BinaryDecode(IEventTemplateDB db, byte[] buffer, int offset, int count)
		{
			Decoder dec = Constants.DefaultEncoding.GetDecoder();
			Decoder bodyDecoder = dec;
			int ofs = offset, end_of_input = offset + count;

			string eventName = LwesSerializer.ReadEVENTWORD(buffer, ref ofs, dec);
			UInt16 attributeCount = LwesSerializer.ReadUInt16(buffer, ref ofs);

			Event ev;
			if (!db.TryCreateEvent(eventName, out ev, false, SupportedEncoding.Default))
			{
				ev = new Event(new EventTemplate(false, eventName), false, SupportedEncoding.Default);
			}

			for (int i = 0; i < attributeCount && ofs < end_of_input; i++)
			{
				string attributeName = LwesSerializer.ReadATTRIBUTEWORD(buffer, ref ofs, dec);
				TypeToken attributeTokenType = (TypeToken)LwesSerializer.ReadByte(buffer, ref ofs);

				// Encoding is in the first attribute position if it is present
				if (i == 0
					&& String.Equals(Constants.MetaEventInfoAttributes.Encoding, attributeName)
					&& attributeTokenType == TypeToken.UINT16)
				{
					Int16 changeEncoding = LwesSerializer.ReadInt16(buffer, ref ofs);
					bodyDecoder = Constants.GetEncoding(changeEncoding).GetDecoder();
					ev.SetValue(Constants.MetaEventInfoAttributes.Encoding.Name, changeEncoding);
				}
				else
				{
					switch (attributeTokenType)
					{
						case TypeToken.UINT16:
							ev.SetValue(attributeName, LwesSerializer.ReadUInt16(buffer, ref ofs));
							break;
						case TypeToken.INT16:
							ev.SetValue(attributeName, LwesSerializer.ReadInt16(buffer, ref ofs));
							break;
						case TypeToken.UINT32:
							ev.SetValue(attributeName, LwesSerializer.ReadUInt32(buffer, ref ofs));
							break;
						case TypeToken.INT32:
							ev.SetValue(attributeName, LwesSerializer.ReadInt32(buffer, ref ofs));
							break;
						case TypeToken.STRING:
							ev.SetValue(attributeName, LwesSerializer.ReadString(buffer, ref ofs, bodyDecoder));
							break;
						case TypeToken.IP_ADDR:
							ev.SetValue(attributeName, LwesSerializer.ReadIPAddress(buffer, ref ofs));
							break;
						case TypeToken.INT64:
							ev.SetValue(attributeName, LwesSerializer.ReadInt64(buffer, ref ofs));
							break;
						case TypeToken.UINT64:
							ev.SetValue(attributeName, LwesSerializer.ReadUInt64(buffer, ref ofs));
							break;
						case TypeToken.BOOLEAN:
							ev.SetValue(attributeName, LwesSerializer.ReadBoolean(buffer, ref ofs));
							break;
					}
				}
			}

			return ev;
		}

		internal int BinaryEncode(byte[] buffer, int offset)
		{
			/* Encoded using the following protocol:
			 *
			 * EVENTWORD,<number of elements>,ATTRIBUTEWORD,TYPETOKEN,
			 * (UINT16|INT16|UINT32|INT32|UINT64|INT64|BOOLEAN|STRING)
			 * ...ATTRIBUTEWORD,TYPETOKEN(UINT16|INT16|UINT32|INT32|
			 * UINT64|INT64|BOOLEAN|STRING)
			 *
			 * The EVENTWORD and ATTRIBUTEWORD(s) are encoded with
			 * the default encoding, the first attribute will be the
			 * encoding for strings if present (may differ from the
			 * encoding used for EVENTWORD and ATTRIBUTEWORD(s)
			 */
			int count = 0, ofs = offset;
			Encoder utf8 = Constants.DefaultEncoding.GetEncoder();

			lock (_sync)
			{
				Encoder bodyEncoder = Constants.GetEncoding((short)_encoding).GetEncoder();

				// Encode the EVENTWORD + attribute-count
				_template.BinaryEncode(buffer, ref ofs);

				// All of the template and dynamic attributes are encoded...
				for (int i = 0; i < _attributes.Count; i++)
				{
					count += _attributes[i].BinaryEncode(buffer, ref ofs, bodyEncoder);
				}
			}

			offset = ofs;

			return count;
		}

		internal int CalculateEncodedByteCount()
		{
			lock (_sync)
			{
				Encoding enc = Constants.GetEncoding((short)_encoding);
				int count = _template.GetByteCount();

				// All of the template and dynamic attributes are encoded...
				foreach (var a in _attributes)
				{
					count += a.GetByteCount(enc);
				}
				return count;
			}
		}

		private void AppendAttribute<T>(AttributeTemplate attr, T value)
		{
			lock (_sync)
			{
				_template = _template.AppendAttributes(attr);
				_attributes.Add(new EventAttribute<T>(attr, value));
			}
		}

		private void PrependAttribute<T>(AttributeTemplate attr, T value)
		{
			lock (_sync)
			{
				EventTemplate et = _template.PrependAttributes(attr);
				_attributes.Insert(0, new EventAttribute<T>(attr, value));
			}
		}

		#endregion Methods
	}
}