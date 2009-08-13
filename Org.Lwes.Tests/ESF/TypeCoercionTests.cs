namespace Org.Lwes.Tests.ESF
{
	using System;
	using System.Diagnostics;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>
	/// Summary description for TypeCoercionTests
	/// </summary>
	[TestClass]
	public class TypeCoercionTests
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
		public void TestPerformanceOfConvertAndCoerce()
		{
			var test = new { iterations = 1000000 };

			TimeCoercion(test.iterations);
			TimeConverstions(test.iterations);
			TimeCoercion(test.iterations);
			TimeConverstions(test.iterations);
			TimeCoercion(test.iterations);
			TimeConverstions(test.iterations);
		}

		[TestMethod]
		public void TryCoerce_INT16()
		{
			var coercibles = new
			{
				uint16 = (ushort)1,
				int16 = (short)2,
				uint32 = (uint)3,
				int32 = (int)4,
				string_ = "5",
				int64 = (long)6,
				uint64 = (ulong)7
			};

			short v;
			Assert.IsTrue(Coercion.TryCoerce(coercibles.uint16, out v));
			Assert.AreEqual(1, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.int16, out v));
			Assert.AreEqual(2, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.uint32, out v));
			Assert.AreEqual(3, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.int32, out v));
			Assert.AreEqual(4, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.string_, out v));
			Assert.AreEqual(5, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.int64, out v));
			Assert.AreEqual(6, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.uint64, out v));
			Assert.AreEqual(7, v);

			var boundary = new
			{
				uint16 = short.MaxValue,
				int16 = (short)short.MaxValue,
				uint32 = (uint)short.MaxValue,
				int32 = (int)short.MaxValue,
				string_ = short.MaxValue.ToString(),
				int64 = (long)short.MaxValue,
				uint64 = (ulong)short.MaxValue
			};

			Assert.IsTrue(Coercion.TryCoerce(boundary.uint16, out v));
			Assert.AreEqual(short.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.int16, out v));
			Assert.AreEqual(short.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.uint32, out v));
			Assert.AreEqual(short.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.int32, out v));
			Assert.AreEqual(short.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.string_, out v));
			Assert.AreEqual(short.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.int64, out v));
			Assert.AreEqual(short.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.uint64, out v));
			Assert.AreEqual(short.MaxValue, v);

			var outOfRange = new
			{
				uint16 = ushort.MaxValue,
				uint32 = uint.MaxValue,
				int32 = int.MinValue,
				string_ = (short.MaxValue + 1).ToString(),
				int64 = long.MinValue,
				uint64 = ulong.MaxValue
			};

			Assert.IsFalse(Coercion.TryCoerce(outOfRange.uint16, out v));
			Assert.IsFalse(Coercion.TryCoerce(outOfRange.uint32, out v));
			Assert.IsFalse(Coercion.TryCoerce(outOfRange.int32, out v));
			Assert.IsFalse(Coercion.TryCoerce(outOfRange.string_, out v));
			Assert.IsFalse(Coercion.TryCoerce(outOfRange.int64, out v));
			Assert.IsFalse(Coercion.TryCoerce(outOfRange.uint64, out v));
		}

		[TestMethod]
		public void TryCoerce_UINT16()
		{
			var coercibles = new
			{
				uint16 = (ushort)1,
				int16 = (short)2,
				uint32 = (uint)3,
				int32 = (int)4,
				string_ = "5",
				int64 = (long)6,
				uint64 = (ulong)7
			};

			ushort v;
			Assert.IsTrue(Coercion.TryCoerce(coercibles.uint16, out v));
			Assert.AreEqual(1, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.int16, out v));
			Assert.AreEqual(2, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.uint32, out v));
			Assert.AreEqual(3, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.int32, out v));
			Assert.AreEqual(4, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.string_, out v));
			Assert.AreEqual(5, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.int64, out v));
			Assert.AreEqual(6, v);
			Assert.IsTrue(Coercion.TryCoerce(coercibles.uint64, out v));
			Assert.AreEqual(7, v);

			var boundary = new
			{
				uint16 = ushort.MaxValue,
				int16 = (short)short.MaxValue,
				uint32 = (uint)ushort.MaxValue,
				int32 = (int)ushort.MaxValue,
				string_ = ushort.MaxValue.ToString(),
				int64 = (long)ushort.MaxValue,
				uint64 = (ulong)ushort.MaxValue
			};

			Assert.IsTrue(Coercion.TryCoerce(boundary.uint16, out v));
			Assert.AreEqual(ushort.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.int16, out v));
			Assert.AreEqual((ushort)short.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.uint32, out v));
			Assert.AreEqual(ushort.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.int32, out v));
			Assert.AreEqual(ushort.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.string_, out v));
			Assert.AreEqual(ushort.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.int64, out v));
			Assert.AreEqual(ushort.MaxValue, v);
			Assert.IsTrue(Coercion.TryCoerce(boundary.uint64, out v));
			Assert.AreEqual(ushort.MaxValue, v);

			var outOfRange = new
			{
				int16 = short.MinValue,
				uint32 = uint.MaxValue,
				int32 = int.MinValue,
				string_ = (ushort.MaxValue + 1).ToString(),
				int64 = long.MinValue,
				uint64 = ulong.MaxValue
			};

			Assert.IsFalse(Coercion.TryCoerce(outOfRange.int16, out v));
			Assert.IsFalse(Coercion.TryCoerce(outOfRange.uint32, out v));
			Assert.IsFalse(Coercion.TryCoerce(outOfRange.int32, out v));
			Assert.IsFalse(Coercion.TryCoerce(outOfRange.string_, out v));
			Assert.IsFalse(Coercion.TryCoerce(outOfRange.int64, out v));
			Assert.IsFalse(Coercion.TryCoerce(outOfRange.uint64, out v));
		}

		private void TimeCoercion(int iterations)
		{
			var coercibles = new
			{
				uint16 = (ushort)1,
				int16 = (short)2,
				uint32 = (uint)3,
				int32 = (int)4,
				string_ = "5",
				int64 = (long)6,
				uint64 = (ulong)7
			};

			ushort v;
			var time = Stopwatch.StartNew();
			for (int i = 0; i < iterations; i++)
			{
				Coercion.TryCoerce(coercibles.uint16, out v);
			}
			for (int i = 0; i < iterations; i++)
			{
				Coercion.TryCoerce(coercibles.int16, out v);
			}
			for (int i = 0; i < iterations; i++)
			{
				Coercion.TryCoerce(coercibles.uint32, out v);
			}
			for (int i = 0; i < iterations; i++)
			{
				Coercion.TryCoerce(coercibles.int32, out v);
			}
			for (int i = 0; i < iterations; i++)
			{
				Coercion.TryCoerce(coercibles.string_, out v);
			}
			for (int i = 0; i < iterations; i++)
			{
				Coercion.TryCoerce(coercibles.int64, out v);
			}
			for (int i = 0; i < iterations; i++)
			{
				Coercion.TryCoerce(coercibles.uint64, out v);
			}
			Console.WriteLine(String.Concat("Coerce time: ", time.ElapsedMilliseconds));
		}

		private void TimeConverstions(int iterations)
		{
			var coercibles = new
			{
				uint16 = (ushort)1,
				int16 = (short)2,
				uint32 = (uint)3,
				int32 = (int)4,
				string_ = "5",
				int64 = (long)6,
				uint64 = (ulong)7
			};

			ushort v;
			var time = Stopwatch.StartNew();
			for (int i = 0; i < iterations; i++)
			{
				v = Convert.ToUInt16(coercibles.uint16);
			}
			for (int i = 0; i < iterations; i++)
			{
				v = Convert.ToUInt16(coercibles.int16);
			}
			for (int i = 0; i < iterations; i++)
			{
				v = Convert.ToUInt16(coercibles.uint32);
			}
			for (int i = 0; i < iterations; i++)
			{
				v = Convert.ToUInt16(coercibles.int32);
			}
			for (int i = 0; i < iterations; i++)
			{
				v = Convert.ToUInt16(coercibles.string_);
			}
			for (int i = 0; i < iterations; i++)
			{
				v = Convert.ToUInt16(coercibles.int64);
			}
			for (int i = 0; i < iterations; i++)
			{
				v = Convert.ToUInt16(coercibles.uint64);
			}
			Console.WriteLine(String.Concat("Convert time: ", time.ElapsedMilliseconds));
		}

		#endregion Methods
	}
}