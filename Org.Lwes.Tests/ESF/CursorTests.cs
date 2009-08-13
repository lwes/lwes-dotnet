namespace Org.Lwes.Tests.ESF
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Org.Lwes.ESF;

	/// <summary>
	/// Summary description for CursorTests
	/// </summary>
	[TestClass]
	public class CursorTests
	{
		#region Properties

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get; set;
		}

		#endregion Properties

		#region Methods

		[TestMethod]
		public void IncrementTests()
		{
			Cursor c = new Cursor();

			Assert.AreEqual(0, c.Offset);
			Assert.AreEqual(0, c.Line);
			Assert.AreEqual(0, c.LinePos);

			c++;

			Assert.AreEqual(1, c.Offset);
			Assert.AreEqual(0, c.Line);
			Assert.AreEqual(1, c.LinePos);

			c++;

			Assert.AreEqual(2, c.Offset);
			Assert.AreEqual(0, c.Line);
			Assert.AreEqual(2, c.LinePos);

			c = c.Newline();

			Assert.AreEqual(3, c.Offset);
			Assert.AreEqual(1, c.Line);
			Assert.AreEqual(0, c.LinePos);

			c++;

			Assert.AreEqual(4, c.Offset);
			Assert.AreEqual(1, c.Line);
			Assert.AreEqual(1, c.LinePos);

			c = c + 2;

			Assert.AreEqual(6, c.Offset);
			Assert.AreEqual(1, c.Line);
			Assert.AreEqual(3, c.LinePos);
		}

		#endregion Methods
	}
}