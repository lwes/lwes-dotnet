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
namespace Org.Lwes.ESF
{
	using System.Text;

	/// <summary>
	/// Cursor used by parsing methods. Tracks offset, line, and line-position.
	/// </summary>
	public struct Cursor
	{
		#region Fields

		private int _line;
		private int _linepos;
		private int _offs;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new instance initialized to the offset given.
		/// </summary>
		/// <param name="ofs"></param>
		public Cursor(int ofs)
		{
			_offs = _linepos = ofs;
			_line = 0;
		}

		/// <summary>
		/// Creates a new instance with initialized values for offset, 
		/// line, and line-position.
		/// </summary>
		/// <param name="offs">Offset from the beginning of the parse input.</param>
		/// <param name="line">Zero based line number where the offset occurs.</param>
		/// <param name="linepos">Zero based character position within the line.</param>
		public Cursor(int offs, int line, int linepos)
		{
			_offs = offs;
			_line = line;
			_linepos = linepos;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="c">Copy cursor</param>
		public Cursor(Cursor c)
		{
			_offs = c._offs;
			_line = c._line;
			_linepos = c._linepos;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Zero based line number where offset occurs.
		/// </summary>
		public int Line
		{
			get { return _line; }
		}

		/// <summary>
		/// Zero based character position within the line.
		/// </summary>
		public int LinePos
		{
			get { return _linepos; }
		}

		/// <summary>
		/// Zero based offset from beginning of input.
		/// </summary>
		public int Offset
		{
			get { return _offs; }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Implicit conversion operator to Int32
		/// </summary>
		/// <param name="c">cursor to convert</param>
		/// <returns>an integer representing the cursor position</returns>
		public static implicit operator int(Cursor c)
		{
			return c._offs;
		}

		/// <summary>
		/// Implicit + operator for incrementing a cursor by an integer value.
		/// </summary>
		/// <param name="c">cursor to be incremented</param>
		/// <param name="inc">number by which the cursor is incremented</param>
		/// <returns>an incremented cursor</returns>
		public static Cursor operator +(Cursor c, int inc)
		{
			return new Cursor(c._offs + inc, c._line, c._linepos + inc);
		}

		/// <summary>
		/// Implicit ++ operator for incrementing a cursor's position.
		/// </summary>
		/// <param name="c">cursor being incremented</param>
		/// <returns>the incremented cursor</returns>
		public static Cursor operator ++(Cursor c)
		{
			return new Cursor(c._offs + 1, c._line, c._linepos + 1);
		}

		/// <summary>
		/// Increments the cursor.
		/// </summary>
		/// <returns>Cursor representing the incremented position</returns>
		public Cursor Increment()
		{
			return new Cursor(_offs+1, _line, _linepos+1);
		}

		/// <summary>
		/// Increments the cursor.
		/// </summary>
		/// <param name="count">indicates the number to increment the cursor by</param>
		/// <returns>Cursor representing the incremented position</returns>
		public Cursor Increment(int count)
		{
			return new Cursor(_offs + count, _line, _linepos + count);
		}

		/// <summary>
		/// Increments the cursor to reflect a new line. (single character newline)
		/// </summary>
		/// <returns>Cursor reflecting the new position</returns>
		public Cursor Newline()
		{
			return new Cursor(_offs + 1, _line + 1, 0);
		}

		/// <summary>
		/// Increments the cursor to reflect a new line. (multiple character newline)
		/// </summary>
		/// <param name="charCount">number of characters reflecting the new line</param>
		/// <returns>Cursor reflecting the new position</returns>
		public Cursor Newline(int charCount)
		{
			return new Cursor(_offs + charCount, _line + 1, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return new StringBuilder(65).Append("Cursor: { Offset: ").Append(_offs)
				.Append(", Line: ").Append(_line)
				.Append(", Character: ").Append(_linepos)
				.Append("}").ToString();
		}

		#endregion Methods
	}
}