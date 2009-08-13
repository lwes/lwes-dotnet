namespace Org.Lwes.ESF
{
	using System;
	using System.Text;

	/// <summary>
	/// Exception indicating a parse error has occurred.
	/// </summary>
	[Serializable]
	public class ParseException : System.FormatException
	{
		#region Fields

		/// <summary>
		/// Value indicating an ErrorPosition is unknown or invalid.
		/// </summary>
		public static readonly int InvalidErrorPosition = -1;

		private int _position = InvalidErrorPosition;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ParseException()
		{
		}

		/// <summary>
		/// Creates a new instance initializing the error's message.
		/// </summary>
		/// <param name="msg">the error message</param>
		public ParseException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Creates a new instance initializing the error's message
		/// and indicating the cursor position of the error.
		/// </summary>
		/// <param name="msg">the error message</param>
		/// <param name="cursor">the cursor position of the error</param>
		public ParseException(string msg, Cursor cursor)
			: base(ParseException.MakeErrorMessage(msg, cursor))
		{
			_position = cursor;
		}

		/// <summary>
		/// Creates a new instance indicating the cursor position of the error
		/// and the values that were expected at that position.
		/// </summary>
		/// <param name="curs">the cursor position of the error</param>
		/// <param name="expected">expected values</param>
		public ParseException(Cursor curs, params string[] expected)
			: base(ParseException.MakeErrorMessage(curs, expected))
		{
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Position of the parse error within the parse input.
		/// </summary>
		/// <value>Offset from the beginning parse input where the error occurred.</value>
		public int ErrorPosition
		{
			get { return _position; }
		}

		#endregion Properties

		#region Methods

		private static string MakeErrorMessage(string message, Cursor curs)
		{
			return String.Concat(message, " - at ", curs);
		}

		private static string MakeErrorMessage(Cursor curs, params string[] expected)
		{
			StringBuilder buffer = new StringBuilder(400)
				.Append("Input cannot be parsed at ").Append(curs.ToString()).Append(": expected ");
			if (expected.Length == 1)
				buffer.Append(expected[0]);
			else
			{
				for (int i = 0; i < expected.Length; ++i)
				{
					if (i > 0)
					{
						if (i == expected.Length - 1) buffer.Append(" or ");
						else buffer.Append(", ");
					}
					buffer.Append(expected[i]);
				}
			}
			return buffer.ToString();
		}

		#endregion Methods
	}
}