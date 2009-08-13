namespace Org.Lwes.Tests.ESF
{
	using System;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Org.Lwes.ESF;

	/// <summary>
	/// Tests the EsfParser class
	/// </summary>
	[TestClass]
	public class EsfParserTests
	{
		#region Properties

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get;
			set;
		}

		#endregion Properties

		#region Methods

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ExpectChar_ArgumentNullExceptionIsRaisedWhenInputIsNull()
		{
			char[] input = null;

			Cursor c = new Cursor();
			Assert.AreEqual(0, c);

			Assert.AreEqual('x', EsfParser.ExpectChar(input, ref c, 'x'));
			Assert.Fail("Should have blew up on the null input");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ExpectChar_ArgumentOutOfRangeExceptionIsRaisedWhenCursorLessThanZero()
		{
			char[] input = "input".ToCharArray();

			Cursor c = new Cursor(-1);
			Assert.AreEqual(-1, c);

			Assert.AreEqual('i', EsfParser.ExpectChar(input, ref c, 'i'));
			Assert.Fail("Should have blew up on the cursor out of range");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ExpectChar_ArgumentOutOfRangeExceptionIsRaisedWhenCursorOutOfRange()
		{
			char[] input = "input".ToCharArray();

			Cursor c = new Cursor(8);
			Assert.AreEqual(8, c);

			Assert.AreEqual('i', EsfParser.ExpectChar(input, ref c, 'i'));
			Assert.Fail("Should have blew up on the cursor out of range");
		}

		[TestMethod]
		[ExpectedException(typeof(ParseException))]
		public void ExpectChar_ParseExceptionIsRaisedWhenInputDoesNotMatchExpected()
		{
			char[] input = "xxyy  zz".ToCharArray();

			Cursor c = new Cursor();
			Assert.AreEqual(0, c);

			Assert.AreEqual('x', EsfParser.ExpectChar(input, ref c, 'x'));
			Assert.AreEqual('x', EsfParser.ExpectChar(input, ref c, 'x'));
			Assert.AreEqual('y', EsfParser.ExpectChar(input, ref c, 'y'));
			Assert.AreEqual('y', EsfParser.ExpectChar(input, ref c, 'y'));
			Assert.AreEqual('a', EsfParser.ExpectChar(input, ref c, 'a'));
			Assert.Fail("Should have blew up when expecting an 'a' in position 4");
		}

		[TestMethod]
		public void ExpectChar_SuccessCase()
		{
			char[] input = "this is the input".ToCharArray();

			Cursor c = new Cursor();
			Assert.AreEqual(0, c);

			for (int i = 0; i < input.Length; i++)
			{
				Assert.AreEqual(input[i],
					EsfParser.ExpectChar(input, ref c, input[i]));
			}
		}

		[TestMethod]
		public void ExpectTypeToken_SuccessCases()
		{
			char[] input = @"uint16
			int16
			uint32
			int32
			string
			ip_addr
			int64
			uint64
			boolean".ToCharArray();

			Cursor cursor = new Cursor();
			Assert.AreEqual(TypeToken.UINT16, EsfParser.ExpectTypeToken(input, ref cursor));
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);

			Assert.AreEqual(TypeToken.INT16, EsfParser.ExpectTypeToken(input, ref cursor));
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);

			Assert.AreEqual(TypeToken.UINT32, EsfParser.ExpectTypeToken(input, ref cursor));
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);

			Assert.AreEqual(TypeToken.INT32, EsfParser.ExpectTypeToken(input, ref cursor));
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);

			Assert.AreEqual(TypeToken.STRING, EsfParser.ExpectTypeToken(input, ref cursor));
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);

			Assert.AreEqual(TypeToken.IP_ADDR, EsfParser.ExpectTypeToken(input, ref cursor));
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);

			Assert.AreEqual(TypeToken.INT64, EsfParser.ExpectTypeToken(input, ref cursor));
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);

			Assert.AreEqual(TypeToken.UINT64, EsfParser.ExpectTypeToken(input, ref cursor));
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);

			Assert.AreEqual(TypeToken.BOOLEAN, EsfParser.ExpectTypeToken(input, ref cursor));
		}

		[TestMethod]
		public void ExpectWord_SuccessCase()
		{
			char[] input = "ns::en".ToCharArray();

			// eventname at the begining of the input
			Cursor cursor = new Cursor();
			Assert.AreEqual("ns::en", EsfParser.ExpectWord(input, ref cursor));

			// eventname at the begining of the input followed by a space
			input = "ns::en { UINT16 myAttribute; }".ToCharArray();
			cursor = new Cursor();
			Assert.AreEqual("ns::en", EsfParser.ExpectWord(input, ref cursor));

			// eventname at the begining of the input followed by a comment
			input = "ns::en# { UINT16 myAttribute; }".ToCharArray();
			cursor = new Cursor();
			Assert.AreEqual("ns::en", EsfParser.ExpectWord(input, ref cursor));

			// eventname at the begining of the input followed by a comment
			input = @"ns::en # My class, namespace::eventname
			{
			UINT16 myAttribute;
			}".ToCharArray();
			cursor = new Cursor();
			Assert.AreEqual("ns::en", EsfParser.ExpectWord(input, ref cursor));

			// eventname at the begining of the input followed by an attribute list
			input = @"ns::en{ # My class, namespace::eventname
			UINT16 myAttribute;
			}".ToCharArray();
			cursor = new Cursor();
			Assert.AreEqual("ns::en", EsfParser.ExpectWord(input, ref cursor));
		}

		[TestMethod]
		public void IsLineTerminator()
		{
			Assert.IsTrue(EsfParser.IsLineTerminator('\r'));
			Assert.IsTrue(EsfParser.IsLineTerminator('\n'));
			Assert.IsTrue(EsfParser.IsLineTerminator('\u2028'));
			Assert.IsTrue(EsfParser.IsLineTerminator('\u2029'));

			for (int i = 32; i < 1024; i++)
			{
				Assert.IsFalse(EsfParser.IsLineTerminator((char)i));
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void PeekExpectChar_ArgumentNullExceptionIsRaisedWhenInputIsNull()
		{
			char[] input = null;

			Cursor c = new Cursor();

			Assert.AreEqual('x', EsfParser.PeekExpectChar(input, c, 'x'));
			Assert.Fail("Should have blew up on the null input");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void PeekExpectChar_ArgumentOutOfRangeExceptionIsRaisedWhenCursorLessThanZero()
		{
			char[] input = "input".ToCharArray();

			Cursor c = new Cursor(-1);

			Assert.AreEqual('i', EsfParser.PeekExpectChar(input, c, 'i'));
			Assert.Fail("Should have blew up on the cursor out of range");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void PeekExpectChar_ArgumentOutOfRangeExceptionIsRaisedWhenCursorOutOfRange()
		{
			char[] input = "input".ToCharArray();

			Cursor c = new Cursor(8);

			Assert.AreEqual('i', EsfParser.PeekExpectChar(input, c, 'i'));
			Assert.Fail("Should have blew up on the cursor out of range");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void PeekExpectChar_Multiple_ArgumentNullExceptionIsRaisedWhenInputIsNull()
		{
			char[] input = null;

			Cursor c = new Cursor();

			Assert.AreEqual('x', EsfParser.PeekExpectChar(input, c, new char[] { 'i' }));
			Assert.Fail("Should have blew up on the null input");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void PeekExpectChar_Multiple_ArgumentOutOfRangeExceptionIsRaisedWhenCursorLessThanZero()
		{
			char[] input = "input".ToCharArray();

			Cursor c = new Cursor(-1);

			Assert.AreEqual('i', EsfParser.PeekExpectChar(input, c, new char[] { 'i' }));
			Assert.Fail("Should have blew up on the cursor out of range");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void PeekExpectChar_Multiple_ArgumentOutOfRangeExceptionIsRaisedWhenCursorOutOfRange()
		{
			char[] input = "input".ToCharArray();

			Cursor c = new Cursor(8);

			Assert.AreEqual('i', EsfParser.PeekExpectChar(input, c, new char[] { 'i' }));
			Assert.Fail("Should have blew up on the cursor out of range");
		}

		[TestMethod]
		[ExpectedException(typeof(ParseException))]
		public void PeekExpectChar_Multiple_ParseExceptionIsRaisedWhenInputDoesNotMatchExpected()
		{
			char[] input = "xxyy  zz".ToCharArray();

			Cursor c = new Cursor();

			Assert.AreEqual('x', EsfParser.PeekExpectChar(input, c, new char[] { 'x' }));
			c = c.Increment();
			Assert.AreEqual('x', EsfParser.PeekExpectChar(input, c, new char[] { 'x' }));
			c = c.Increment();
			Assert.AreEqual('y', EsfParser.PeekExpectChar(input, c, new char[] { 'y' }));
			c = c.Increment();
			Assert.AreEqual('y', EsfParser.PeekExpectChar(input, c, new char[] { 'y' }));
			c = c.Increment();
			Assert.AreEqual('a', EsfParser.PeekExpectChar(input, c, new char[] { 'a' }));
			Assert.Fail("Should have blew up when expecting an 'a' in position 4");
		}

		[TestMethod]
		public void PeekExpectChar_Multiple_SuccessCase()
		{
			char[] input = "this is the input".ToCharArray();

			Cursor c = new Cursor();
			for (int i = 0; i < input.Length; i++)
			{
				Assert.AreEqual(i, c);
				Assert.AreEqual(input[i],
					EsfParser.PeekExpectChar(input, c, new char[] { input[i], 'x' }));
				c = c.Increment();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ParseException))]
		public void PeekExpectChar_ParseExceptionIsRaisedWhenInputDoesNotMatchExpected()
		{
			char[] input = "xxyy  zz".ToCharArray();

			Cursor c = new Cursor();

			Assert.AreEqual('x', EsfParser.PeekExpectChar(input, c, 'x'));
			c = c.Increment();
			Assert.AreEqual('x', EsfParser.PeekExpectChar(input, c, 'x'));
			c = c.Increment();
			Assert.AreEqual('y', EsfParser.PeekExpectChar(input, c, 'y'));
			c = c.Increment();
			Assert.AreEqual('y', EsfParser.PeekExpectChar(input, c, 'y'));
			c = c.Increment();
			Assert.AreEqual('a', EsfParser.PeekExpectChar(input, c, 'a'));
			Assert.Fail("Should have blew up when expecting an 'a' in position 4");
		}

		[TestMethod]
		public void PeekExpectChar_SuccessCase()
		{
			char[] input = "this is the input".ToCharArray();

			Cursor c = new Cursor();
			for (int i = 0; i < input.Length; i++)
			{
				Assert.AreEqual(i, c);
				Assert.AreEqual(input[i],
					EsfParser.PeekExpectChar(input, c, input[i]));
				c = c.Increment();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SkipToLineTerminator_ArgumentNullExceptionIsRaisedWhenInputIsNull()
		{
			char[] input = null;

			Cursor c = new Cursor();

			EsfParser.SkipToLineTerminator(input, ref c);
			Assert.Fail("Should have blew up on the null input");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SkipToLineTerminator_ArgumentOutOfRangeExceptionIsRaisedWhenCursorLessThanZero()
		{
			char[] input = "input".ToCharArray();

			Cursor c = new Cursor(-1);

			EsfParser.SkipToLineTerminator(input, ref c);
			Assert.Fail("Should have blew up on the cursor out of range");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SkipToLineTerminator_ArgumentOutOfRangeExceptionIsRaisedWhenCursorOutOfRange()
		{
			char[] input = "input".ToCharArray();

			Cursor c = new Cursor(8);

			EsfParser.SkipToLineTerminator(input, ref c);
			Assert.Fail("Should have blew up on the cursor out of range");
		}

		[TestMethod]
		public void SkipToLineTerminator_SuccessCase()
		{
			var test = new
			{
				InputWithLineTerminators = "this is a line with a line terminator at positions 53\r\n, 59\rand 66\r\n".ToCharArray(),
				InputWithoutLineTerminators = "this is a line without line terminators, it is 65 characters long".ToCharArray()
			};

			char[] input = test.InputWithLineTerminators;

			Cursor cursor = new Cursor();
			EsfParser.SkipToLineTerminator(input, ref cursor);
			Assert.AreEqual(53, cursor);
			EsfParser.ExpectSkipLineTerminator(input, ref cursor);
			Assert.AreEqual(55, cursor, "Should have skipped a two character line terminator");
			EsfParser.SkipToLineTerminator(input, ref cursor);
			Assert.AreEqual(59, cursor);
			EsfParser.ExpectSkipLineTerminator(input, ref cursor);
			Assert.AreEqual(60, cursor, "Should have skipped a one character line terminator");
			EsfParser.SkipToLineTerminator(input, ref cursor);
			Assert.AreEqual(66, cursor);
			EsfParser.ExpectSkipLineTerminator(input, ref cursor);
			Assert.AreEqual(68, cursor);

			// Reset the cursor and input
			cursor = new Cursor();
			input = test.InputWithoutLineTerminators;

			EsfParser.SkipToLineTerminator(input, ref cursor);
			Assert.AreEqual(65, cursor);
		}

		[TestMethod]
		public void SkipWhitespaceAndComments_SuccessCases()
		{
			var test = new
			{
				InputNoLeadingSpaceOrComments = "ns::en",
				InputWithLeadingWhitespace = @"
			ns::en",
				InputWithLeadingComments = "# Leading comment\r\nns::en",
				InputWithMixOrCommentsAndWhitespace = @"#Leading comment
			# and a bunch of comments
			# and line terminators
			####

			ns::en"
			};

			char[] input = test.InputNoLeadingSpaceOrComments.ToCharArray();
			Cursor cursor = new Cursor();
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);
			Assert.AreEqual(0, cursor, "EVENTWORD starts at position 0 (zero)");

			input = test.InputWithLeadingWhitespace.ToCharArray();
			cursor = new Cursor();
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);
			Assert.AreEqual(5, cursor, "EVENTWORD starts at position 5");

			input = test.InputWithLeadingComments.ToCharArray();
			cursor = new Cursor();
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);
			Assert.AreEqual(19, cursor, "EVENTWORD starts at position 19");

			input = test.InputWithMixOrCommentsAndWhitespace.ToCharArray();
			cursor = new Cursor();
			EsfParser.SkipWhitespaceAndComments(input, ref cursor);
			Assert.AreEqual(89, cursor, "EVENTWORD starts at position 89");
		}

		#endregion Methods
	}
}