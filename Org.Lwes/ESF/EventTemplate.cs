namespace Org.Lwes.ESF
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Org.Lwes.DB;

	/// <summary>
	/// Template for an event type.
	/// </summary>
	[Serializable]
	public struct EventTemplate
	{
		#region Fields

		/// <summary>
		/// Empty event template.
		/// </summary>
		public static readonly EventTemplate Empty = new EventTemplate();

		AttributeTemplate[] _attributes;
		Dictionary<string, int> _attributesByName;
		bool _fromEsf;
		string _name;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new template.
		/// </summary>
		/// <param name="fromEsf">Indicates whether the tempate is from an ESF definition.</param>
		/// <param name="eventName">Name of the event.</param>
		public EventTemplate(bool fromEsf, string eventName)
		{
			_fromEsf = fromEsf;
			_name = eventName;
			_attributes = new AttributeTemplate[0];
			_attributesByName = null;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the tempate's attributes.
		/// </summary>
		public IEnumerable<AttributeTemplate> Attributes
		{
			get { return _attributes; }
			private set
			{
				if (value == null) throw new ArgumentNullException("Attributes");

				// When the attributes are set we calculate the ordinal positions
				int ordinal = 0;
				_attributes = (from a in value
											 select new AttributeTemplate(a.TypeToken, a.Name, ordinal++)).ToArray();
				_attributesByName = new Dictionary<string, int>();
				foreach (var a in _attributes)
				{
					_attributesByName.Add(a.Name, a.Ordinal);
				}
			}
		}

		/// <summary>
		/// Gets the attribute count.
		/// </summary>
		public int Count
		{
			get { return _attributes.Length; }
		}

		/// <summary>
		/// Indicates the template is from ESF
		/// </summary>
		public bool FromEsf
		{
			get { return _fromEsf; }
		}

		/// <summary>
		/// The event template's name.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		#endregion Properties

		#region Indexers

		/// <summary>
		/// Gets an attribute's template.
		/// </summary>
		/// <param name="name">attribute name</param>
		/// <returns>the attribute's template</returns>
		/// <exception cref="ArgumentOutOfRangeException">thrown if the attribute does not exist</exception>
		public AttributeTemplate this[string name]
		{
			get
			{
				int ord;
				if (TryGetOrdinal(name, out ord))
				{
					return _attributes[ord];
				}
				throw new ArgumentOutOfRangeException("name");
			}
		}

		/// <summary>
		/// Gets an attribute's template at the ordinal position given.
		/// </summary>
		/// <param name="ordinal">the attribute's position</param>
		/// <returns>the attribute's template</returns>
		/// <exception cref="ArgumentOutOfRangeException">thrown if the ordinal position is out of range</exception>
		public AttributeTemplate this[int ordinal]
		{
			get
			{
				return _attributes[ordinal];
			}
		}

		#endregion Indexers

		#region Methods

		/// <summary>
		/// Parses a character array for an event tempate.
		/// </summary>
		/// <param name="input">character input</param>
		/// <param name="cursor">reference to a position within the input; upon success
		/// this variable is advanced by the number of characters taken while parsing</param>
		/// <returns>the event template</returns>
		public static EventTemplate ExpectEvent(char[] input, ref Cursor cursor)
		{
			Cursor c = new Cursor(cursor);
			EsfParser.SkipWhitespaceAndComments(input, ref c);

			// parse the event name
			string eventName = EsfParser.ExpectWord(input, ref c);
			EventTemplate evt = new EventTemplate(false, eventName);

			EsfParser.SkipWhitespaceAndComments(input, ref c);
			EsfParser.ExpectChar(input, ref c, EsfParser.LeftCurlyBracket);

			// read the attribute list
			List<AttributeTemplate> attributes = new List<AttributeTemplate>();
			while (c < input.Length && input[c] != EsfParser.RightCurlyBracket)
			{
				EsfParser.SkipWhitespaceAndComments(input, ref c);
				if (c < input.Length && input[c] != EsfParser.RightCurlyBracket)
				{
					attributes.Add(AttributeTemplate.ExpectAttribute(input, ref c));
				}
			}
			evt.Attributes = attributes;

			EsfParser.SkipWhitespaceAndComments(input, ref c);
			EsfParser.ExpectChar(input, ref c, EsfParser.RightCurlyBracket);

			// Advance the cursor upon success.
			cursor = c;
			return evt;
		}

		/// <summary>
		/// Indicates whether there is an attribute defined with the given name.
		/// </summary>
		/// <param name="name">attribute's name</param>
		/// <returns><em>true</em> if the attribute exists; otherwise <em>false</em></returns>
		public bool HasAttribute(string name)
		{
			return _attributesByName != null && _attributesByName.ContainsKey(name);
		}

		/// <summary>
		/// Tries to get the ordinal position of the attribute given.
		/// </summary>
		/// <param name="name">attribute's name</param>
		/// <param name="ord">reference to a variable that will hold the ordinal position of the attribute upon success</param>
		/// <returns><em>true</em> if the attribute exists; otherwise <em>false</em></returns>
		public bool TryGetOrdinal(string name, out int ord)
		{
			if (_attributesByName == null)
			{
				ord = -1;
				return false;
			}
			return _attributesByName.TryGetValue(name, out ord);
		}

		internal EventTemplate AppendAttributes(params AttributeTemplate[] append)
		{
			EventTemplate ev = new EventTemplate(false, Name);
			int ord = 0;
			ev.Attributes = from a in Enumerable.Concat(_attributes, append)
											select new AttributeTemplate(a.TypeToken, a.Name, ord++);
			return ev;
		}

		internal int BinaryEncode(byte[] buffer, ref int offset)
		{
			int count = 0, ofs = offset;

			count += LwesSerializer.WriteEVENTWORD(buffer, ref ofs, _name, Constants.DefaultEncoding.GetEncoder());
			count += LwesSerializer.Write(buffer, ref ofs, (UInt16)_attributes.Length);

			offset = ofs;
			return count;
		}

		internal int GetByteCount()
		{
			// [1-byte-length-prefix][1-255-byte-eventword][2-byte-attribute-count]
			return Constants.DefaultEncoding.GetByteCount(_name) + 3;
		}

		internal EventTemplate PrependAttributes(params AttributeTemplate[] prepend)
		{
			EventTemplate ev = new EventTemplate(false, Name);
			int ord = 0;
			ev.Attributes = from a in Enumerable.Concat(prepend, _attributes)
											select new AttributeTemplate(a.TypeToken, a.Name, ord++);
			return ev;
		}

		#endregion Methods
	}
}