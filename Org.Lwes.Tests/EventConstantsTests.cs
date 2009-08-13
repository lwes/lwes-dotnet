namespace Org.Lwes.Tests
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Org.Lwes.ESF;

	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class EventConstantsTests
	{
		#region Properties

		public TestContext TestContext
		{
			get; set;
		}

		#endregion Properties

		#region Methods

		[TestMethod]
		public void EsfTokenNameToByte_ReturnsUndefinedTokenOnInvalidName()
		{
			Assert.AreEqual((byte)TypeToken.UNDEFINED, EsfParser.TokenNameToByte("invalid"));
		}

		[TestMethod]
		public void EsfTokenNameToByte_SuccessfullyConvertsTokenNames()
		{
			var tokens = new
				{
					UINT16 = new { Value = 0x01, Name = "uint16" },
					INT16 = new { Value = 0x02, Name = "int16" },
					UINT32 = new { Value = 0x03, Name = "uint32" },
					INT32 = new { Value = 0x04, Name = "int32" },
					STRING = new { Value = 0x05, Name = "string" },
					IPADDR = new { Value = 0x06, Name = "ip_addr" },
					INT64 = new { Value = 0x07, Name = "int64" },
					UINT64 = new { Value = 0x08, Name = "uint64" },
					BOOLEAN = new { Value = 0x09, Name = "boolean" }
				};

			Assert.AreEqual(tokens.UINT16.Value, EsfParser.TokenNameToByte(tokens.UINT16.Name));
			Assert.AreEqual(tokens.INT16.Value, EsfParser.TokenNameToByte(tokens.INT16.Name));
			Assert.AreEqual(tokens.UINT32.Value, EsfParser.TokenNameToByte(tokens.UINT32.Name));
			Assert.AreEqual(tokens.INT32.Value, EsfParser.TokenNameToByte(tokens.INT32.Name));
			Assert.AreEqual(tokens.STRING.Value, EsfParser.TokenNameToByte(tokens.STRING.Name));
			Assert.AreEqual(tokens.IPADDR.Value, EsfParser.TokenNameToByte(tokens.IPADDR.Name));
			Assert.AreEqual(tokens.INT64.Value, EsfParser.TokenNameToByte(tokens.INT64.Name));
			Assert.AreEqual(tokens.UINT64.Value, EsfParser.TokenNameToByte(tokens.UINT64.Name));
			Assert.AreEqual(tokens.BOOLEAN.Value, EsfParser.TokenNameToByte(tokens.BOOLEAN.Name));
		}

		#endregion Methods
	}
}