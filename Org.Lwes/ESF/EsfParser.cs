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
	using System;
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Utility class for parsing ESF files.
	/// </summary>
	public class EsfParser
	{
		#region Fields

		/// <summary>
		/// Array of characters used as line terminators.
		/// </summary>
		public static readonly char[] LineTerminatorChars = new char[] { '\r', '\n', '\u2028', '\u2029' };

		/// <summary>
		/// Map of type token names.
		/// </summary>
		public static readonly string[] TypeTokenNameMap = 
			{ "undefined", // -
				"uint16",    // 0x01 - UINT16
				"int16",     // 0x02 - INT16
				"uint32",    // 0x03 - UINT32
				"int32",     // 0x04 - INT32
				"string",    // 0x05 - STRING
				"ip_addr",   // 0x06 - IPADDR
				"int64",     // 0x07 - INT64
				"uint64",    // 0x08 - UINT64
				"boolean"    // 0x09 - BOOLEAN
			};

		/// <summary>
		/// Constant for the colon.
		/// </summary>
		public const char Colon = ':';

		/// <summary>
		/// Constant for the latin number nine.
		/// </summary>
		public const char DigitNine = '9';

		/// <summary>
		/// Constant for the latin number zero.
		/// </summary>
		public const char DigitZero = '0';

		/// <summary>
		/// Constant for the latin uppercase letter A.
		/// </summary>
		public const char LatinCapitalLetterA = 'A';

		/// <summary>
		/// Constant for the latin uppercase letter Z.
		/// </summary>
		public const char LatinCapitalLetterZ = 'Z';

		/// <summary>
		/// Constant for the latin lowercase letter a.
		/// </summary>
		public const char LatinSmallLetterA = 'a';

		/// <summary>
		/// Constant for the latin lowercase letter z.
		/// </summary>
		public const char LatinSmallLetterZ = 'z';

		/// <summary>
		/// Constant for the left-curly-bracket - delimits the beginning of an attribute-list.
		/// </summary>
		public const char LeftCurlyBracket = '{';

		/// <summary>
		/// Constant for the lowline (underscore).
		/// </summary>
		public const char LowLine = '_';

		/// <summary>
		/// Constant for the numbersign - delimits the beginning of a single line comment.
		/// </summary>
		public const char NumberSign = '#';

		/// <summary>
		/// Constant for the right-curly-bracket - delimits the end of an attribute-list.
		/// </summary>
		public const char RightCurlyBracket = '}';

		/// <summary>
		/// Constant for the semicolon - indicates the end of an attribute definition.
		/// </summary>
		public const char SemiColon = ';';

		private const char NoCharacter = '\u0000';

		#endregion Fields

		#region Methods

		/// <summary>
		/// Validates that the character at the cursor is the expected
		/// character and advances the cursor.
		/// </summary>
		/// <param name="input">array of input characters</param>
		/// <param name="cursor">reference to the parse cursor</param>
		/// <param name="ch">the expected character</param>
		/// <returns>the expected character (taken from the input) if the 
		/// expected character is present; otherwise a ParseException is thrown</returns>
		/// <exception cref="ArgumentNullException">thrown if the <paramref name="input"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">thrown if the cursor is beyond the end of the input.</exception>
		/// <exception cref="ParseException">thrown if the expected character is not present.</exception>
		public static char ExpectChar(char[] input, ref Cursor cursor, char ch)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if (cursor < 0
				|| input.Length <= cursor)
				throw new ArgumentOutOfRangeException("cursor", "Cursor out of input range");

			if (input[cursor] != ch)
				throw new ParseException(cursor, ch.ToString());

			return input[cursor++];
		}

		/// <summary>
		/// Validates that a sequence of characters are present at the cursor.
		/// </summary>
		/// <param name="input">input characters</param>
		/// <param name="cursor">input cursor</param>
		/// <param name="sequence">string containing the expected sequence of characters</param>
		/// <param name="numAlreadyValidated">Number of characters already validated</param>
		/// <returns>If the expected characters (<paramref name="sequence"/>) are present at the cursor
		/// then the sequence is returned; otherwise an exception is raised.</returns>
		public static string ExpectCharSequence(char[] input, ref Cursor cursor, string sequence, int numAlreadyValidated)
		{
			if (sequence == null)
				throw new ArgumentNullException("sequence");

			return new String(ExpectCharSequence(input, ref cursor, sequence.ToCharArray(), numAlreadyValidated));
		}

		/// <summary>
		/// Validates that a sequence of characters are present at the cursor.
		/// </summary>
		/// <param name="input">input characters</param>
		/// <param name="cursor">input cursor</param>
		/// <param name="sequence">the expected sequence of characters</param>
		/// <param name="numAlreadyValidated">Number of characters already validated</param>
		/// <returns>If the expected characters (<paramref name="sequence"/>) are present at the cursor
		/// then the sequence is returned; otherwise an exception is raised.</returns>
		public static char[] ExpectCharSequence(char[] input, ref Cursor cursor, char[] sequence, int numAlreadyValidated)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if (cursor < 0 || input.Length <= cursor)
				throw new ArgumentOutOfRangeException("cursor", "Cursor out of input range");

			if (numAlreadyValidated < 0)
				throw new ArgumentOutOfRangeException("ofs", "Cursor out of input range");

			if (sequence == null)
				throw new ArgumentNullException("sequence");

			// If the number of characters already validated is greater than the number
			// of characters in the sequence then we're already done,
			// advance the cursor and return.
			if (numAlreadyValidated > sequence.Length)
			{
				cursor += sequence.Length;
				return sequence;
			}

			int len = sequence.Length;
			if (input.Length < cursor + len)
				throw new ParseException(String.Concat("Expected '", new String(sequence), "'"), cursor);

			Cursor c = new Cursor(cursor + numAlreadyValidated);
			for (int i = numAlreadyValidated; i < len; i++, c++)
			{
				if (input[c] != sequence[i])
				{
					throw new ParseException(String.Concat("Expected '", new String(sequence), "'"), cursor);
				}
			}
			cursor = c;
			return sequence;
		}

		/// <summary>
		/// Validates that a newline occurs at cursor and advances cursor.
		/// </summary>
		/// <param name="input">input characters.</param>
		/// <param name="cursor">Reference to the cursor.</param>
		public static void ExpectSkipLineTerminator(char[] input, ref Cursor cursor)
		{
			char ch = PeekExpectChar(input, cursor, LineTerminatorChars);

			// If we got carriage return, see if there is a linefeed...
			if ((ch == '\r')
				&& input.Length > (cursor + 1)
				&& (input[cursor + 1] == '\n'))
			{ // ... there is, skip it too...
				cursor = cursor.Newline(2);
			}
			else
				cursor = cursor.Newline();
		}

		/// <summary>
		/// Validates that an ESF type token appears in the input at the cursor,
		/// returns the token and advances the cursor.
		/// </summary>
		/// <param name="input">input characters</param>
		/// <param name="cursor">input cursor</param>
		/// <returns>the ESF type token located at the cursor</returns>
		public static TypeToken ExpectTypeToken(char[] input, ref Cursor cursor)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if (cursor < 0
				|| input.Length <= cursor)
				throw new ArgumentOutOfRangeException("cursor", "Cursor out of input range");

			Cursor c = cursor;
			int rmn = input.Length - c;

			// all token names are atleast 5 characters in length
			if (rmn < 5)
				throw new ParseException("Expected an ESF type name", cursor);

			switch (input[c])
			{
				case 'u':
					c++;
					if (input[c] == 'i')
					{
						c++;
						EsfParser.ExpectCharSequence(input, ref c, "nt", 0);
						if (input[c] == '1')
						{ // can only be uint16
							c++;
							EsfParser.ExpectChar(input, ref c, '6');
							cursor = c;
							return TypeToken.UINT16;
						}
						else if (input[c] == '3')
						{ // can only be uint32
							c++;
							EsfParser.ExpectChar(input, ref c, '2');
							cursor = c;
							return TypeToken.UINT32;
						}
						else if (input[c] == '6')
						{ // can only be uint64
							c++;
							EsfParser.ExpectChar(input, ref c, '4');
							cursor = c;
							return TypeToken.UINT64;
						}
					}
					break;
				case 'i':
					c++;
					if (input[c] == 'n')
					{
						c++;
						EsfParser.ExpectChar(input, ref c, 't');
						if (input[c] == '1')
						{ // can only be int16
							c++;
							EsfParser.ExpectChar(input, ref c, '6');
							cursor = c;
							return TypeToken.INT16;
						}
						else if (input[c] == '3')
						{ // can only be int32
							c++;
							EsfParser.ExpectChar(input, ref c, '2');
							cursor = c;
							return TypeToken.INT32;
						}
						else if (input[c] == '6')
						{ // can only be int64
							c++;
							EsfParser.ExpectChar(input, ref c, '4');
							cursor = c;
							return TypeToken.INT64;
						}
					}
					else if (input[c] == 'p')
					{ // can only be ip_addr
						EsfParser.ExpectCharSequence(input, ref cursor, TypeTokenNameMap[(int)TypeToken.IP_ADDR], 2);
						return TypeToken.IP_ADDR;
					}
					break;
				case 's':
					EsfParser.ExpectCharSequence(input, ref cursor, TypeTokenNameMap[(int)TypeToken.STRING], 1);
					return TypeToken.STRING;
				case 'b':
					EsfParser.ExpectCharSequence(input, ref cursor, TypeTokenNameMap[(int)TypeToken.BOOLEAN], 1);
					return TypeToken.BOOLEAN;
			}

			throw new ParseException("Expected an ESF type name", cursor);
		}

		/// <summary>
		/// Validates that an ESF word is present at the cursor
		/// and advances the cursor.
		/// </summary>
		/// <param name="input">input characters</param>
		/// <param name="cursor">input cursor</param>
		/// <returns>the ESF word located </returns>
		public static string ExpectWord(char[] input, ref Cursor cursor)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if (cursor < 0
				|| input.Length <= cursor)
				throw new ArgumentOutOfRangeException("cursor", "Cursor out of input range");

			Cursor c = new Cursor(cursor);
			while (c < input.Length && IsWordCharacter(input[c])) c++;

			if (c == cursor)
				throw new ParseException("Expected either an EVENTWORD or an ATTRIBUTEWORD at the cursor", cursor);

			int ofs = cursor;
			cursor = c;
			return new String(input, ofs, cursor.Offset - ofs);
		}

		/// <summary>
		/// Determines if a character is a newline character.
		/// </summary>
		/// <param name="ch">character to check</param>
		/// <returns><em>true</em> if the character is a newline character; otherwise <em>false</em></returns>
		public static bool IsLineTerminator(char ch)
		{
			return (ch == '\r'    // Unicode4 codepoint CarriageReturn
				|| ch == '\n'       // Unicode4 codepoint LineFeed
				|| ch == '\u2028'   // Unicode4 codepoint LineSeparator
				|| ch == '\u2029'); // Unicode4 codepoint ParagraphSeparator
		}

		/// <summary>
		/// Determins if a word is an ESF word character.
		/// </summary>
		/// <param name="c">a character</param>
		/// <returns><em>true</em> if the character is a legal
		/// word char; otherwise <em>false</em></returns>
		public static bool IsWordCharacter(char c)
		{
			// assumes lower case characters are more common,
			// then upper case characters
			// then digits (and the colon),
			// then the lowline.

			// 0..9 and : are contiguous \u0030..\u0x003A
			return (c >= LatinSmallLetterA && c <= LatinSmallLetterZ)
				|| (c >= LatinCapitalLetterA && c <= LatinCapitalLetterZ)
				|| (c >= DigitZero && c <= Colon)
				|| c == LowLine;
		}

		/// <summary>
		/// Validates that the character at the cursor is the expected character
		/// but does not advance the cursor.
		/// </summary>
		/// <param name="input">input characters</param>
		/// <param name="cursor">reference to the parse cursor</param>
		/// <param name="ch">the expected character</param>
		/// <returns>the expected character (taken from the input) if the 
		/// expected character is present; otherwise a ParseException is thrown</returns>
		/// <exception cref="ArgumentNullException">thrown if the <paramref name="input"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">thrown if the cursor is beyond the end of the input.</exception>
		/// <exception cref="ParseException">thrown if the expected character is not present.</exception>
		public static char PeekExpectChar(char[] input, Cursor cursor, char ch)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if (cursor < 0
				|| input.Length <= cursor)
				throw new ArgumentOutOfRangeException("cursor", "Cursor out of input range");

			if (input[cursor] != ch)
				throw new ParseException(cursor, ch.ToString());

			return input[cursor];
		}

		/// <summary>
		/// Validates that the character at the cursor is one of the expected characters
		/// but does not advance the cursor.
		/// </summary>
		/// <param name="input">input characters</param>
		/// <param name="cursor">reference to the parse cursor</param>
		/// <param name="chars">an array containing the expected characters</param>
		/// <returns>the expected character at the cursor's position (taken from the input)
		/// if an expected character is present; otherwise a ParseException is thrown</returns>
		/// <exception cref="ArgumentNullException">thrown if the <paramref name="input"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">thrown if the cursor is beyond the end of the input.</exception>
		/// <exception cref="ParseException">thrown if an expected character is not present at the cursor position.</exception>
		public static char PeekExpectChar(char[] input, Cursor cursor, char[] chars)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if (cursor < 0
				|| input.Length <= cursor)
				throw new ArgumentOutOfRangeException("cursor", "Cursor out of input range");

			for (int i = 0; i < chars.Length; i++)
			{
				if (input[cursor] == chars[i])
				{
					return input[cursor];
				}
			}
			throw new ParseException("Expected newline characters", cursor);
		}

		/// <summary>
		/// Advances the cursor to the next line terminator or
		/// the end of input, whichever is first.
		/// </summary>
		/// <param name="input">input characters</param>
		/// <param name="cursor">input cursor</param>
		public static void SkipToLineTerminator(char[] input, ref Cursor cursor)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if (cursor < 0 || input.Length <= cursor)
				throw new ArgumentOutOfRangeException("cursor", "Cursor out of input range");

			while (cursor < input.Length
				&& !IsLineTerminator(input[cursor]))
				cursor++;
		}

		/// <summary>
		/// Advances the cursor past any whitespace or comments.
		/// </summary>
		/// <param name="input">input characters</param>
		/// <param name="cursor"></param>
		public static void SkipWhitespaceAndComments(char[] input, ref Cursor cursor)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if (cursor < 0)
				throw new ArgumentOutOfRangeException("cursor", "Cursor out of input range");

			while (cursor < input.Length)
			{
				while (Char.IsWhiteSpace(input[cursor]))
				{
					if (IsLineTerminator(input[cursor]))
						ExpectSkipLineTerminator(input, ref cursor);
					else
						cursor++;
				}
				if (input[cursor] == NumberSign)
				{
					SkipToLineTerminator(input, ref cursor);
				}
				else
				{ // Done skipping
					return;
				}
			}
		}

		/// <summary>
		/// Gets the byte value of an ESF token given the token name.
		/// </summary>
		/// <param name="tokenName">ESF token name</param>
		/// <returns>byte value of the ESF token</returns>
		public static byte TokenNameToByte(string tokenName)
		{
			if (tokenName == null) throw new ArgumentNullException("tokenName");
			if (tokenName.Length < 5) throw new ArgumentException("not a valid token name");

			Cursor c = new Cursor();
			try
			{
				return (byte)EsfParser.ExpectTypeToken(tokenName.ToCharArray(), ref c);
			}
			catch (ParseException)
			{
				return (byte)TypeToken.UNDEFINED;
			}
		}

		/// <summary>
		/// Parses event templates from the input stream.
		/// </summary>
		/// <param name="inputStream">input stream</param>
		/// <returns>Enumerable of TempateEvents</returns>
		public IEnumerable<EventTemplate> ParseEventTemplates(Stream inputStream)
		{
			char[] input;
			using (StreamReader r = new StreamReader(inputStream))
			{
				input = r.ReadToEnd().ToCharArray();
			}
			return ParseEventTemplates(input);
		}

		private IEnumerable<EventTemplate> ParseEventTemplates(char[] input)
		{
			Cursor c = new Cursor();

			// read the attribute list
			List<EventTemplate> events = new List<EventTemplate>();
			while (c < input.Length)
			{
				EsfParser.SkipWhitespaceAndComments(input, ref c);
				if (c < input.Length)
				{
					events.Add(EventTemplate.ExpectEvent(input, ref c));
				}
			}

			return events;
		}

		#endregion Methods
	}
}