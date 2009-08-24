namespace Org.Lwes.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Text;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using Org.Lwes.DB;

	[TestClass]
	public class SerializerTests
	{
		#region Fields

		Random _rand = new Random(Environment.TickCount);

		#endregion Fields

		#region Methods

		[TestMethod]
		public void RoundTripSerialize_Boolean()
		{
			var values = new bool[]
					{ (_rand.Next() % 2 == 0)
					, (_rand.Next() % 2 == 0)
					, (_rand.Next() % 2 == 0)
					, (_rand.Next() % 2 == 0)
					, (_rand.Next() % 2 == 0)
					, (_rand.Next() % 2 == 0)
					, (_rand.Next() % 2 == 0)
					};

			byte[] data = new byte[values.Length * sizeof(UInt64)];
			int cursor = 0;

			// Serialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Write the value and assert the length written...
				Assert.AreEqual(sizeof(byte), LwesSerializer.Write(data, ref cursor, values[i]));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual(i + 1, cursor);
			}

			// Reset the cursor
			cursor = 0;
			// Deserialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				Assert.AreEqual(values[i], LwesSerializer.ReadBoolean(data, ref cursor));
				Assert.AreEqual(i + 1, cursor);
			}
		}

		[TestMethod]
		public void RoundTripSerialize_Byte()
		{
			var values = new byte[]
					{ Convert.ToByte(_rand.Next(byte.MinValue, byte.MaxValue))
					, Convert.ToByte(_rand.Next(byte.MinValue, byte.MaxValue))
					, Convert.ToByte(_rand.Next(byte.MinValue, byte.MaxValue))
					, Convert.ToByte(_rand.Next(byte.MinValue, byte.MaxValue))
					, Convert.ToByte(_rand.Next(byte.MinValue, byte.MaxValue))
					, Convert.ToByte(_rand.Next(byte.MinValue, byte.MaxValue))
					, Convert.ToByte(_rand.Next(byte.MinValue, byte.MaxValue))
					, Convert.ToByte(_rand.Next(byte.MinValue, byte.MaxValue))
					, Convert.ToByte(_rand.Next(byte.MinValue, byte.MaxValue))
					, Convert.ToByte(_rand.Next(byte.MinValue, byte.MaxValue))
					, Convert.ToByte(_rand.Next(byte.MinValue, byte.MaxValue))
					};

			byte[] data = new byte[values.Length * sizeof(UInt64)];
			int cursor = 0;

			// Serialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Write the value and assert the length written...
				Assert.AreEqual(sizeof(byte), LwesSerializer.Write(data, ref cursor, values[i]));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual(i + sizeof(byte), cursor);
			}

			// Reset the cursor
			cursor = 0;
			// Deserialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				Assert.AreEqual(values[i], LwesSerializer.ReadByte(data, ref cursor));
				Assert.AreEqual(i + sizeof(byte), cursor);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(BadLwesDataException))]
		public void RoundTripSerialize_DeserializeGarbageData()
		{
			byte[] bytes = new byte[400];
			Random _rand = new Random(Environment.TickCount);
			_rand.NextBytes(bytes);

			var mock = new Mock<IEventTemplateDB>();
			Event dummy;
			mock.Setup(db => db.TryCreateEvent(It.IsAny<string>(), out dummy, It.IsAny<bool>(), It.IsAny<SupportedEncoding>()))
				.Returns(false);

			LwesSerializer.Deserialize(bytes, 0, bytes.Length, mock.Object);
		}

		[TestMethod]
		public void RoundTripSerialize_Event()
		{
			var e = new
			{
				EventName = "UserLogin",
				Attributes = new
				{
					UserName = new { Name = "username", Token = TypeToken.STRING, Value = "bob" },
					Password = new { Name = "password", Token = TypeToken.UINT64, Value = 0xfeedabbadeadbeefUL },
					ClientIP = new { Name = "clientIP", Token = TypeToken.IP_ADDR, Value = IPAddress.Parse("127.0.0.1") },
					Successful = new { Name = "successful", Token = TypeToken.BOOLEAN, Value = false }
				}
			};

			// Create the event...
			Event ev = new Event(e.EventName);
				ev.SetValue(e.Attributes.UserName.Name, e.Attributes.UserName.Value);
				ev.SetValue(e.Attributes.Password.Name, e.Attributes.Password.Value);
				ev.SetValue(e.Attributes.ClientIP.Name, e.Attributes.ClientIP.Value);
				ev.SetValue(e.Attributes.Successful.Name, e.Attributes.Successful.Value);

			// Ensure the token types match...
			Assert.AreEqual(e.Attributes.UserName.Token, ev[e.Attributes.UserName.Name].TypeToken);
			Assert.AreEqual(e.Attributes.Password.Token, ev[e.Attributes.Password.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ClientIP.Token, ev[e.Attributes.ClientIP.Name].TypeToken);
			Assert.AreEqual(e.Attributes.Successful.Token, ev[e.Attributes.Successful.Name].TypeToken);

			// Ensure the values match...
			Assert.AreEqual(e.Attributes.UserName.Value, ev[e.Attributes.UserName.Name].GetValue<string>());
			Assert.AreEqual(e.Attributes.Password.Value, ev[e.Attributes.Password.Name].GetValue<UInt64>());
			Assert.AreEqual(e.Attributes.ClientIP.Value, ev[e.Attributes.ClientIP.Name].GetValue<IPAddress>());
			Assert.AreEqual(e.Attributes.Successful.Value, ev[e.Attributes.Successful.Name].GetValue<bool>());

			// Serialization requires an IEventTemplateDB, we're gonna mock it here...
			var mock = new Mock<IEventTemplateDB>();
			Event dummy;
			mock.Setup(db => db.TryCreateEvent(It.IsAny<string>(), out dummy, It.IsAny<bool>(), It.IsAny<SupportedEncoding>()))
				.Returns(false);

			// Perform a roundtrip serialization...
			byte[] data = LwesSerializer.Serialize(ev);
			Event ev2 = LwesSerializer.Deserialize(data, 0, data.Length, mock.Object);

			// Ensure the token types match on the deserialized object...
			Assert.AreEqual(e.Attributes.UserName.Token, ev2[e.Attributes.UserName.Name].TypeToken);
			Assert.AreEqual(e.Attributes.Password.Token, ev2[e.Attributes.Password.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ClientIP.Token, ev2[e.Attributes.ClientIP.Name].TypeToken);
			Assert.AreEqual(e.Attributes.Successful.Token, ev2[e.Attributes.Successful.Name].TypeToken);

			// Ensure the values match on the deserialized object...
			Assert.AreEqual(e.Attributes.UserName.Value, ev2[e.Attributes.UserName.Name].GetValue<string>());
			Assert.AreEqual(e.Attributes.Password.Value, ev2[e.Attributes.Password.Name].GetValue<UInt64>());
			Assert.AreEqual(e.Attributes.ClientIP.Value, ev2[e.Attributes.ClientIP.Name].GetValue<IPAddress>());
			Assert.AreEqual(e.Attributes.Successful.Value, ev2[e.Attributes.Successful.Name].GetValue<bool>());
		}

		[TestMethod]
		public void RoundTripSerialize_Int16()
		{
			var values = new Int16[]
					{ Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					, Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					, Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					, Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					, Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					, Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					, Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					, Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					, Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					, Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					, Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					};

			byte[] data = new byte[values.Length * sizeof(UInt64)];
			int cursor = 0;

			// Serialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Write the value and assert the length written...
				Assert.AreEqual(sizeof(Int16), LwesSerializer.Write(data, ref cursor, values[i]));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(Int16), cursor);
			}

			// Reset the cursor
			cursor = 0;
			// Deserialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Read the and assert the value...
				Assert.AreEqual(values[i], LwesSerializer.ReadInt16(data, ref cursor));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(Int16), cursor);
			}
		}

		[TestMethod]
		public void RoundTripSerialize_Int32()
		{
			var values = new Int32[]
					{ Convert.ToInt32(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt32(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt32(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt32(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt32(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt32(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt32(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt32(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt32(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt32(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt32(_rand.Next(Int32.MinValue, Int32.MaxValue))
					};

			byte[] data = new byte[values.Length * sizeof(UInt64)];
			int cursor = 0;

			// Serialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Write the value and assert the length written...
				Assert.AreEqual(sizeof(Int32), LwesSerializer.Write(data, ref cursor, values[i]));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(Int32), cursor);
			}

			// Reset the cursor
			cursor = 0;
			// Deserialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Read the and assert the value...
				Assert.AreEqual(values[i], LwesSerializer.ReadInt32(data, ref cursor));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(Int32), cursor);
			}
		}

		[TestMethod]
		public void RoundTripSerialize_Int64()
		{
			var values = new Int64[]
					{ Convert.ToInt64(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt64(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt64(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt64(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt64(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt64(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt64(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt64(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt64(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt64(_rand.Next(Int32.MinValue, Int32.MaxValue))
					, Convert.ToInt64(_rand.Next(Int32.MinValue, Int32.MaxValue))
					};

			byte[] data = new byte[values.Length * sizeof(UInt64)];
			int cursor = 0;

			// Serialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Write the value and assert the length written...
				Assert.AreEqual(sizeof(Int64), LwesSerializer.Write(data, ref cursor, values[i]));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(Int64), cursor);
			}

			// Reset the cursor
			cursor = 0;
			// Deserialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Read the and assert the value...
				Assert.AreEqual(values[i], LwesSerializer.ReadInt64(data, ref cursor));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(Int64), cursor);
			}
		}

		[TestMethod]
		public void RoundTripSerialize_String_ISO_8859_1()
		{
			var values = new string[]
					{ EventUtils.GenerateRandomString(_rand.Next(1, 400), SupportedEncoding.ISO_8859_1)
					, EventUtils.GenerateRandomString(_rand.Next(1, 400), SupportedEncoding.ISO_8859_1)
					, EventUtils.GenerateRandomString(_rand.Next(1, 400), SupportedEncoding.ISO_8859_1)
					, EventUtils.GenerateRandomString(_rand.Next(1, 400), SupportedEncoding.ISO_8859_1)
					};

			byte[] data = new byte[values.Length * 400 * sizeof(UInt64)];
			int cursor = 0, accumulatedSize = 0, encodedSize;

			Queue<int> encodedSizes = new Queue<int>();
			// Serialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				encodedSize = Constants.ISO8859_1Encoding.GetByteCount(values[i]) + sizeof(UInt16);
				accumulatedSize += encodedSize;
				// Write the value and assert the length written...
				Assert.AreEqual(encodedSize, LwesSerializer.Write(data, ref cursor, values[i], Constants.ISO8859_1Encoding.GetEncoder()));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual(accumulatedSize, cursor);
				encodedSizes.Enqueue(accumulatedSize);
			}

			// Reset the cursor
			cursor = 0;
			// Deserialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Read the and assert the value...
				Assert.AreEqual(values[i], LwesSerializer.ReadString(data, ref cursor, Constants.ISO8859_1Encoding.GetDecoder()));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual(encodedSizes.Dequeue(), cursor);
			}
		}

		[TestMethod]
		public void RoundTripSerialize_String_UTF_8()
		{
			var values = new string[]
					{ EventUtils.GenerateRandomString(_rand.Next(1, 400), SupportedEncoding.UTF_8)
					, EventUtils.GenerateRandomString(_rand.Next(1, 400), SupportedEncoding.UTF_8)
					, EventUtils.GenerateRandomString(_rand.Next(1, 400), SupportedEncoding.UTF_8)
					, EventUtils.GenerateRandomString(_rand.Next(1, 400), SupportedEncoding.UTF_8)
					};

			byte[] data = new byte[values.Length * 400 * sizeof(UInt64)];
			int cursor = 0, accumulatedSize = 0, encodedSize;

			Queue<int> encodedSizes = new Queue<int>();
			// Serialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				encodedSize = Constants.DefaultEncoding.GetByteCount(values[i]) + sizeof(UInt16);
				accumulatedSize += encodedSize;
				// Write the value and assert the length written...
				Assert.AreEqual(encodedSize, LwesSerializer.Write(data, ref cursor, values[i], Constants.DefaultEncoding.GetEncoder()));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual(accumulatedSize, cursor);
				encodedSizes.Enqueue(accumulatedSize);
			}

			// Reset the cursor
			cursor = 0;
			// Deserialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Read the and assert the value...
				Assert.AreEqual(values[i], LwesSerializer.ReadString(data, ref cursor, Constants.DefaultEncoding.GetDecoder()));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual(encodedSizes.Dequeue(), cursor);
			}
		}

		[TestMethod]
		public void RoundTripSerialize_UInt16()
		{
			var values = new UInt16[]
					{ Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					, Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					, Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					, Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					, Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					, Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					, Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					, Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					, Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					, Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					, Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					};

			byte[] data = new byte[values.Length * sizeof(UInt64)];
			int cursor = 0;

			// Serialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Write the value and assert the length written...
				Assert.AreEqual(sizeof(UInt16), LwesSerializer.Write(data, ref cursor, values[i]));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(UInt16), cursor);
			}

			// Reset the cursor
			cursor = 0;
			// Deserialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Read the and assert the value...
				Assert.AreEqual(values[i], LwesSerializer.ReadUInt16(data, ref cursor));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(UInt16), cursor);
			}
		}

		[TestMethod]
		public void RoundTripSerialize_UInt32()
		{
			var values = new UInt32[]
					{ Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
					};

			byte[] data = new byte[values.Length * sizeof(UInt64)];
			int cursor = 0;

			// Serialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Write the value and assert the length written...
				Assert.AreEqual(sizeof(UInt32), LwesSerializer.Write(data, ref cursor, values[i]));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(UInt32), cursor);
			}

			// Reset the cursor
			cursor = 0;
			// Deserialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Read the and assert the value...
				Assert.AreEqual(values[i], LwesSerializer.ReadUInt32(data, ref cursor));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(UInt32), cursor);
			}
		}

		[TestMethod]
		public void RoundTripSerialize_UInt64()
		{
			var values = new UInt64[]
					{ Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					, Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					};

			byte[] data = new byte[values.Length * sizeof(UInt64)];
			int cursor = 0;

			// Serialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Write the value and assert the length written...
				Assert.AreEqual(sizeof(UInt64), LwesSerializer.Write(data, ref cursor, values[i]));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(UInt64), cursor);
			}

			// Reset the cursor
			cursor = 0;
			// Deserialize all of the values...
			for (int i = 0; i < values.Length; i++)
			{
				// Read the and assert the value...
				Assert.AreEqual(values[i], LwesSerializer.ReadUInt64(data, ref cursor));
				// Verify that the cursor moved appropriately...
				Assert.AreEqual((i + 1) * sizeof(UInt64), cursor);
			}
		}

		#endregion Methods
	}
}