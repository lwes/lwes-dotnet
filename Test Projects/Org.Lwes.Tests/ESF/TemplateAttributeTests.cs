namespace Org.Lwes.Tests.ESF
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Org.Lwes.ESF;

	/// <summary>
	/// Summary description for EsfAttributeTests
	/// </summary>
	[TestClass]
	public class TemplateAttributeTests
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
		public void ExpectAttribute()
		{
			var test = @"
			uint16 MyUnsignedInteger16;
			int16 MyInteger16;
			uint32 MyUnsignedInteger32;
			int32 MyInteger32;
			string MyString;
			ip_addr MyIPAddr;
			int64 MyInteger64;
			uint64 MyUnsignedInteger64;
			boolean MyBoolean;
			";

			Cursor cursor = new Cursor();

			AttributeTemplate attr = AttributeTemplate.ExpectAttribute(test.ToCharArray(), ref cursor);
			Assert.IsNotNull(attr);
			Assert.AreEqual(TypeToken.UINT16, attr.TypeToken);
			Assert.AreEqual("MyUnsignedInteger16", attr.Name);

			attr = AttributeTemplate.ExpectAttribute(test.ToCharArray(), ref cursor);
			Assert.IsNotNull(attr);
			Assert.AreEqual(TypeToken.INT16, attr.TypeToken);
			Assert.AreEqual("MyInteger16", attr.Name);

			attr = AttributeTemplate.ExpectAttribute(test.ToCharArray(), ref cursor);
			Assert.IsNotNull(attr);
			Assert.AreEqual(TypeToken.UINT32, attr.TypeToken);
			Assert.AreEqual("MyUnsignedInteger32", attr.Name);

			attr = AttributeTemplate.ExpectAttribute(test.ToCharArray(), ref cursor);
			Assert.IsNotNull(attr);
			Assert.AreEqual(TypeToken.INT32, attr.TypeToken);
			Assert.AreEqual("MyInteger32", attr.Name);

			attr = AttributeTemplate.ExpectAttribute(test.ToCharArray(), ref cursor);
			Assert.IsNotNull(attr);
			Assert.AreEqual(TypeToken.STRING, attr.TypeToken);
			Assert.AreEqual("MyString", attr.Name);

			attr = AttributeTemplate.ExpectAttribute(test.ToCharArray(), ref cursor);
			Assert.IsNotNull(attr);
			Assert.AreEqual(TypeToken.IP_ADDR, attr.TypeToken);
			Assert.AreEqual("MyIPAddr", attr.Name);

			attr = AttributeTemplate.ExpectAttribute(test.ToCharArray(), ref cursor);
			Assert.IsNotNull(attr);
			Assert.AreEqual(TypeToken.INT64, attr.TypeToken);
			Assert.AreEqual("MyInteger64", attr.Name);

			attr = AttributeTemplate.ExpectAttribute(test.ToCharArray(), ref cursor);
			Assert.IsNotNull(attr);
			Assert.AreEqual(TypeToken.UINT64, attr.TypeToken);
			Assert.AreEqual("MyUnsignedInteger64", attr.Name);

			attr = AttributeTemplate.ExpectAttribute(test.ToCharArray(), ref cursor);
			Assert.IsNotNull(attr);
			Assert.AreEqual(TypeToken.BOOLEAN, attr.TypeToken);
			Assert.AreEqual("MyBoolean", attr.Name);
		}

		[TestMethod]
		public void uint16_IsAssignable()
		{
			var test = @"uint16 MyUnsignedInteger16;";

			Cursor cursor = new Cursor();

			AttributeTemplate attr = AttributeTemplate.ExpectAttribute(test.ToCharArray(), ref cursor);
			Assert.IsNotNull(attr);
			Assert.AreEqual(TypeToken.UINT16, attr.TypeToken);
			Assert.AreEqual("MyUnsignedInteger16", attr.Name);
			Assert.IsTrue(attr.IsAssignable<ushort>(0));
			Assert.IsFalse(attr.IsAssignable<int>(0));
		}

		#endregion Methods
	}
}