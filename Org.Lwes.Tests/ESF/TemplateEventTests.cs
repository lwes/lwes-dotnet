namespace Org.Lwes.Tests.ESF
{
	using System;
	using System.Linq;
	using System.Net;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Org.Lwes.ESF;

	/// <summary>
	/// Summary description for EsfEventTests
	/// </summary>
	[TestClass]
	public class TemplateEventTests
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
		public void ExpectEvent()
		{
			var test = @"
			# This is an event template for testing
			EventForTesting {
			uint16 MyUnsignedInteger16;
			int16 MyInteger16;
			uint32 MyUnsignedInteger32;
			int32 MyInteger32;
			string MyString;
			ip_addr MyIPAddr;
			int64 MyInteger64;
			uint64 MyUnsignedInteger64;
			boolean MyBoolean;
			}";

			Cursor cursor = new Cursor();
			EventTemplate evt = EventTemplate.ExpectEvent(test.ToCharArray(), ref cursor);
			Assert.IsNotNull(evt);
			Assert.AreEqual("EventForTesting", evt.Name);
			Assert.AreEqual(9, evt.Attributes.Count());

			evt["MyUnsignedInteger16"].IsAssignable<ushort>(0);
			evt["MyInteger16"].IsAssignable<short>(0);
			evt["MyUnsignedInteger32"].IsAssignable<uint>(0);
			evt["MyInteger32"].IsAssignable<int>(0);
			evt["MyString"].IsAssignable<string>(String.Empty);
			evt["MyIPAddr"].IsAssignable<IPAddress>(null);
			evt["MyInteger64"].IsAssignable<long>(0);
			evt["MyUnsignedInteger64"].IsAssignable<ulong>(0);
			evt["MyBoolean"].IsAssignable<bool>(false);
		}

		private void AssertHasAttribute(EventTemplate evt, TypeToken tt, string name)
		{
			AttributeTemplate attr = evt[name];
			Assert.IsNotNull(attr);
			Assert.AreEqual(tt, attr.TypeToken);
		}

		#endregion Methods
	}
}