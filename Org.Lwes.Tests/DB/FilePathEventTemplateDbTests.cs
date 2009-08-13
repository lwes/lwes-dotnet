namespace Org.Lwes.Tests.DB
{
	using System;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Org.Lwes.DB;
	using Org.Lwes.ESF;

	/// <summary>
	/// Summary description for FilePathEventTemplateDb
	/// </summary>
	[TestClass]
	public class FilePathEventTemplateDbTests
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
		public void InitializeFromFilePath()
		{
			var test = new
				{
					TemplateName = "EventContainingAttributesOfAllTypes",
					UInt16Attribute = new { Name = "UnsignedShortValue", Token = TypeToken.UINT16, Ordinal = 0 },
					Int16Attribute = new { Name = "SignedShortValue", Token = TypeToken.INT16, Ordinal = 1 },
					UInt32Attribute = new { Name = "UnsignedIntValue", Token = TypeToken.UINT32, Ordinal = 2 },
					Int32Attribute = new { Name = "SignedIntValue", Token = TypeToken.INT32, Ordinal = 3 },
					StringAttribute = new { Name = "StringValue", Token = TypeToken.STRING, Ordinal = 4 },
					IPAddressAttribute = new { Name = "IPAddressValue", Token = TypeToken.IP_ADDR, Ordinal = 5 },
					Int64Attribute = new { Name = "SignedLongValue", Token = TypeToken.INT64, Ordinal = 6 },
					UInt64Attribute = new { Name = "UnsignedLongValue", Token = TypeToken.UINT64, Ordinal = 7 },
					BooleanAttribute = new { Name = "BooleanValue", Token = TypeToken.BOOLEAN, Ordinal = 8 }
				};

			FilePathEventTemplateDB db = new FilePathEventTemplateDB();

			Assert.IsFalse(db.IsInitialized);

			string filePath = Environment.CurrentDirectory;
			db.InitializeFromFilePath(filePath);

			Assert.IsTrue(db.IsInitialized);
			Assert.IsTrue(db.TemplateExists(test.TemplateName));

			EventTemplate ev = db.GetEventTemplate(test.TemplateName);
			Assert.IsInstanceOfType(ev, typeof(EventTemplate));

			EventTemplate ev1;
			Assert.IsTrue(db.TryGetEventTemplate(test.TemplateName, out ev1));

			Assert.IsTrue(ev.HasAttribute(test.UInt16Attribute.Name));
			Assert.IsTrue(ev.HasAttribute(test.Int16Attribute.Name));
			Assert.IsTrue(ev.HasAttribute(test.UInt32Attribute.Name));
			Assert.IsTrue(ev.HasAttribute(test.Int32Attribute.Name));
			Assert.IsTrue(ev.HasAttribute(test.StringAttribute.Name));
			Assert.IsTrue(ev.HasAttribute(test.IPAddressAttribute.Name));
			Assert.IsTrue(ev.HasAttribute(test.Int64Attribute.Name));
			Assert.IsTrue(ev.HasAttribute(test.UInt64Attribute.Name));
			Assert.IsTrue(ev.HasAttribute(test.BooleanAttribute.Name));

			Assert.AreEqual(TypeToken.UINT16, ev[test.UInt16Attribute.Ordinal].TypeToken);
			Assert.AreEqual(TypeToken.INT16, ev[test.Int16Attribute.Ordinal].TypeToken);
			Assert.AreEqual(TypeToken.UINT32, ev[test.UInt32Attribute.Ordinal].TypeToken);
			Assert.AreEqual(TypeToken.INT32, ev[test.Int32Attribute.Ordinal].TypeToken);
			Assert.AreEqual(TypeToken.STRING, ev[test.StringAttribute.Ordinal].TypeToken);
			Assert.AreEqual(TypeToken.IP_ADDR, ev[test.IPAddressAttribute.Ordinal].TypeToken);
			Assert.AreEqual(TypeToken.INT64, ev[test.Int64Attribute.Ordinal].TypeToken);
			Assert.AreEqual(TypeToken.UINT64, ev[test.UInt64Attribute.Ordinal].TypeToken);
			Assert.AreEqual(TypeToken.BOOLEAN, ev[test.BooleanAttribute.Ordinal].TypeToken);
		}

		[TestMethod]
		public void StartsOutUnInitialized()
		{
			FilePathEventTemplateDB db = new FilePathEventTemplateDB();
			Assert.IsFalse(db.IsInitialized);
		}

		#endregion Methods
	}
}