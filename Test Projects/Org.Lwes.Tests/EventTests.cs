﻿namespace Org.Lwes.Tests
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Threading;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using Org.Lwes.DB;
	using Org.Lwes.ESF;

	/// <summary>
	/// Summary description for EventTests
	/// </summary>
	[TestClass]
	public class EventTests
	{
		#region Fields

		Random _rand = new Random(Environment.TickCount);

		#endregion Fields

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
		public void MyDynamicEvent()
		{
			var e = new
			{
				EventName = "MyDynamicEvent",
				Attributes = new
				{
					ValueUInt16 = new
					{
						Name = "ValueUInt16",
						Token = TypeToken.UINT16,
						Value = Convert.ToUInt16(_rand.Next(UInt16.MinValue, UInt16.MaxValue))
					},
					ValueInt16 = new
					{
						Name = "ValueInt16",
						Token = TypeToken.INT16,
						Value = Convert.ToInt16(_rand.Next(Int16.MinValue, Int16.MaxValue))
					},
					ValueUInt32 = new
						{
							Name = "ValueUInt32",
							Token = TypeToken.UINT32,
							Value = Convert.ToUInt32(_rand.Next(0, Int32.MaxValue))
						},
					ValueInt32 = new
					{
						Name = "ValueInt32",
						Token = TypeToken.INT32,
						Value = _rand.Next()
					},
					ValueString = new
					{
						Name = "ValueString",
						Token = TypeToken.STRING,
						Value = "This is a string"
					},
					ValueIPAddress = new
					{
						Name = "ValueIPAddress",
						Token = TypeToken.IP_ADDR,
						Value = IPAddress.Parse("127.0.0.1")
					},
					ValueInt64 = new
					{
						Name = "ValueInt64",
						Token = TypeToken.INT64,
						Value = Convert.ToInt64(_rand.Next())
					},
					ValueUInt64 = new
					{
						Name = "ValueUInt64",
						Token = TypeToken.UINT64,
						Value = Convert.ToUInt64(_rand.Next(0, Int32.MaxValue))
					},
					ValueBoolean = new
					{
						Name = "ValueBoolean",
						Token = TypeToken.BOOLEAN,
						Value = (_rand.Next() % 2 == 0)
					},
				}
			};

			// Events are immutable; .SetValue methods return a new mutated event
			var ev = new Event(e.EventName);
				ev.SetValue(e.Attributes.ValueUInt16.Name, e.Attributes.ValueUInt16.Value);

			// Ensure the attributes match...
			Assert.AreEqual(e.Attributes.ValueUInt16.Token, ev[e.Attributes.ValueUInt16.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueUInt16.Value, ev[e.Attributes.ValueUInt16.Name].GetValue<UInt16>());

			// Be sure to assign it as it mutates...
			ev.SetValue(e.Attributes.ValueInt16.Name, e.Attributes.ValueInt16.Value);

			// Ensure the attributes match...
			Assert.AreEqual(e.Attributes.ValueUInt16.Token, ev[e.Attributes.ValueUInt16.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueUInt16.Value, ev[e.Attributes.ValueUInt16.Name].GetValue<UInt16>());
			Assert.AreEqual(e.Attributes.ValueInt16.Token, ev[e.Attributes.ValueInt16.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueInt16.Value, ev[e.Attributes.ValueInt16.Name].GetValue<Int16>());

			// Be sure to assign it as it mutates...
			ev.SetValue(e.Attributes.ValueUInt32.Name, e.Attributes.ValueUInt32.Value);

			// Ensure the attributes match...
			Assert.AreEqual(e.Attributes.ValueUInt16.Token, ev[e.Attributes.ValueUInt16.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueUInt16.Value, ev[e.Attributes.ValueUInt16.Name].GetValue<UInt16>());
			Assert.AreEqual(e.Attributes.ValueInt16.Token, ev[e.Attributes.ValueInt16.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueInt16.Value, ev[e.Attributes.ValueInt16.Name].GetValue<Int16>());
			Assert.AreEqual(e.Attributes.ValueUInt32.Token, ev[e.Attributes.ValueUInt32.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueUInt32.Value, ev[e.Attributes.ValueUInt32.Name].GetValue<UInt32>());

			// Be sure to assign it as it mutates...
			ev.SetValue(e.Attributes.ValueInt32.Name, e.Attributes.ValueInt32.Value);
			ev.SetValue(e.Attributes.ValueString.Name, e.Attributes.ValueString.Value);
			ev.SetValue(e.Attributes.ValueIPAddress.Name, e.Attributes.ValueIPAddress.Value);
			ev.SetValue(e.Attributes.ValueInt64.Name, e.Attributes.ValueInt64.Value);
			ev.SetValue(e.Attributes.ValueUInt64.Name, e.Attributes.ValueUInt64.Value);
			ev.SetValue(e.Attributes.ValueBoolean.Name, e.Attributes.ValueBoolean.Value);

			// Ensure the attributes match...
			Assert.AreEqual(e.Attributes.ValueUInt16.Token, ev[e.Attributes.ValueUInt16.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueUInt16.Value, ev[e.Attributes.ValueUInt16.Name].GetValue<UInt16>());

			Assert.AreEqual(e.Attributes.ValueInt16.Token, ev[e.Attributes.ValueInt16.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueInt16.Value, ev[e.Attributes.ValueInt16.Name].GetValue<Int16>());

			Assert.AreEqual(e.Attributes.ValueUInt32.Token, ev[e.Attributes.ValueUInt32.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueUInt32.Value, ev[e.Attributes.ValueUInt32.Name].GetValue<UInt32>());

			Assert.AreEqual(e.Attributes.ValueInt32.Token, ev[e.Attributes.ValueInt32.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueInt32.Value, ev[e.Attributes.ValueInt32.Name].GetValue<Int32>());

			Assert.AreEqual(e.Attributes.ValueString.Token, ev[e.Attributes.ValueString.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueString.Value, ev[e.Attributes.ValueString.Name].GetValue<String>());

			Assert.AreEqual(e.Attributes.ValueIPAddress.Token, ev[e.Attributes.ValueIPAddress.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueIPAddress.Value, ev[e.Attributes.ValueIPAddress.Name].GetValue<IPAddress>());

			Assert.AreEqual(e.Attributes.ValueInt64.Token, ev[e.Attributes.ValueInt64.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueInt64.Value, ev[e.Attributes.ValueInt64.Name].GetValue<Int64>());

			Assert.AreEqual(e.Attributes.ValueUInt64.Token, ev[e.Attributes.ValueUInt64.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueUInt64.Value, ev[e.Attributes.ValueUInt64.Name].GetValue<UInt64>());

			Assert.AreEqual(e.Attributes.ValueBoolean.Token, ev[e.Attributes.ValueBoolean.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueBoolean.Value, ev[e.Attributes.ValueBoolean.Name].GetValue<Boolean>());

			// Serialization requires an IEventTemplateDB, we're gonna mock it here...
			var mock = new Mock<IEventTemplateDB>();
			Event dummy;
			mock.Setup(db => db.TryCreateEvent(e.EventName, out dummy, It.IsAny<bool>(), It.IsAny<SupportedEncoding>()))
				.Returns(false);

			// Perform a roundtrip serialization...
			byte[] data = LwesSerializer.Serialize(ev);
			Event ev5 = LwesSerializer.Deserialize(data, 0, data.Length, mock.Object);

			// Ensure the attributes of the deserialized event match...
			Assert.AreEqual(e.EventName, ev5.Name);
			Assert.AreEqual(e.Attributes.ValueUInt16.Token, ev5[e.Attributes.ValueUInt16.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueUInt16.Value, ev5[e.Attributes.ValueUInt16.Name].GetValue<UInt16>());

			Assert.AreEqual(e.Attributes.ValueInt16.Token, ev5[e.Attributes.ValueInt16.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueInt16.Value, ev5[e.Attributes.ValueInt16.Name].GetValue<Int16>());

			Assert.AreEqual(e.Attributes.ValueUInt32.Token, ev5[e.Attributes.ValueUInt32.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueUInt32.Value, ev5[e.Attributes.ValueUInt32.Name].GetValue<UInt32>());

			Assert.AreEqual(e.Attributes.ValueInt32.Token, ev5[e.Attributes.ValueInt32.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueInt32.Value, ev5[e.Attributes.ValueInt32.Name].GetValue<Int32>());

			Assert.AreEqual(e.Attributes.ValueString.Token, ev5[e.Attributes.ValueString.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueString.Value, ev5[e.Attributes.ValueString.Name].GetValue<String>());

			Assert.AreEqual(e.Attributes.ValueIPAddress.Token, ev5[e.Attributes.ValueIPAddress.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueIPAddress.Value, ev5[e.Attributes.ValueIPAddress.Name].GetValue<IPAddress>());

			Assert.AreEqual(e.Attributes.ValueInt64.Token, ev5[e.Attributes.ValueInt64.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueInt64.Value, ev5[e.Attributes.ValueInt64.Name].GetValue<Int64>());

			Assert.AreEqual(e.Attributes.ValueUInt64.Token, ev5[e.Attributes.ValueUInt64.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueUInt64.Value, ev5[e.Attributes.ValueUInt64.Name].GetValue<UInt64>());

			Assert.AreEqual(e.Attributes.ValueBoolean.Token, ev5[e.Attributes.ValueBoolean.Name].TypeToken);
			Assert.AreEqual(e.Attributes.ValueBoolean.Value, ev5[e.Attributes.ValueBoolean.Name].GetValue<Boolean>());
		}

		[TestMethod]
		public void RoundTripDotNetSerializationOfRandomlyGeneratedEvents()
		{
			// DotNet serializer will cause OutOfMemoryException if we try to serialize a million
			var t = new { EventsToGenerate = 10000, MaxNumberOfAttributes = 36, Encoding = SupportedEncoding.ISO_8859_1 };

			int totalNumberOfAttributes = 0;

			// Serialization requires an IEventTemplateDB, we're gonna mock it here...
			var mock = new Mock<IEventTemplateDB>();
			Event dummy;
			mock.Setup(db => db.TryCreateEvent(String.Empty, out dummy, false, SupportedEncoding.Default))
				.Returns(false);

			SimpleLockFreeQueue<Event> serializationQueue = new SimpleLockFreeQueue<Event>();
			SimpleLockFreeQueue<Event> comparisonQueue = new SimpleLockFreeQueue<Event>();
			int serializationCount = 0;
			SimpleLockFreeQueue<byte[]> dataQueue = new SimpleLockFreeQueue<byte[]>();
			SimpleLockFreeQueue<Event> deserializationQueue = new SimpleLockFreeQueue<Event>();
			int deserializationCount = 0;
			long totalByteCount = 0;

			IFormatter serializer = new BinaryFormatter();
			IFormatter deserializer = new BinaryFormatter();
			Exception err = null;

			Stopwatch timer = Stopwatch.StartNew();
			ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
			{
				for (int i = 0; i < t.EventsToGenerate && err == null; i++)
				{
					Event original = EventUtils.GenerateRandomEvent(String.Concat("MyEvent_", i), t.MaxNumberOfAttributes, t.Encoding);
					totalNumberOfAttributes += original.AttributeCount;
					serializationQueue.Enqueue(original);
				}
			}
				));

			ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
			{
				Event original;
				try
				{
					while (serializationCount < t.EventsToGenerate && err == null)
					{
						if (serializationQueue.TryDequeue(out original))
						{
							using (MemoryStream data = new MemoryStream(400))
							{
								serializer.Serialize(data, original);
								dataQueue.Enqueue(data.GetBuffer());
								totalByteCount += (data.Position + 1);
							}
							serializationCount++;
						}
					}
				}
				catch (Exception e)
				{
					err = e;
				}
			}
				));

			byte[] buffer;
			try
			{
				while (deserializationCount < t.EventsToGenerate && err == null)
				{
					if (dataQueue.TryDequeue(out buffer))
					{
						using (MemoryStream data = new MemoryStream(buffer))
						{
							Event copy = (Event)deserializer.Deserialize(data);
						}
						deserializationCount++;
					}
					else Thread.Sleep(0);
				}
			}
			catch (Exception e)
			{
				err = e;
			}
			if (err != null) throw err;

			timer.Stop();

			Console.Write(String.Concat("Generated ", t.EventsToGenerate, " events in ", timer.Elapsed
				, " which is ", (t.EventsToGenerate / timer.ElapsedMilliseconds), " per millisecond and "
				, (totalNumberOfAttributes / timer.ElapsedMilliseconds), " attributes per millisecond.",
				Environment.NewLine, (totalByteCount / timer.Elapsed.TotalSeconds).ToString("N00"), " bytes per second"));
		}

		[TestMethod]
		public void RoundTripLwesSerializationOfRandomlyGeneratedEvents()
		{
			var t = new { EventsToGenerate = 10000, MaxNumberOfAttributes = 36, Encoding = SupportedEncoding.ISO_8859_1 };

			int totalNumberOfAttributes = 0;

			// Serialization requires an IEventTemplateDB, we're gonna mock it here...
			var mock = new Mock<IEventTemplateDB>();
			Event dummy;
			mock.Setup(db => db.TryCreateEvent(String.Empty, out dummy, false, t.Encoding))
				.Returns(false);

			SimpleLockFreeQueue<Event> serializationQueue = new SimpleLockFreeQueue<Event>();
			SimpleLockFreeQueue<Event> comparisonQueue = new SimpleLockFreeQueue<Event>();
			int serializationCount = 0;
			SimpleLockFreeQueue<byte[]> dataQueue = new SimpleLockFreeQueue<byte[]>();
			SimpleLockFreeQueue<Event> deserializationQueue = new SimpleLockFreeQueue<Event>();
			int deserializationCount = 0;
			long totalByteCount = 0;

			Stopwatch timer = Stopwatch.StartNew();
			ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
				{
					for (int i = 0; i < t.EventsToGenerate; i++)
					{
						Event original = EventUtils.GenerateRandomEvent(String.Concat("MyEvent_", i), t.MaxNumberOfAttributes, t.Encoding);
						totalNumberOfAttributes += original.AttributeCount;
						serializationQueue.Enqueue(original);
					}
				}
				));

			ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
				{
					Event original;
					while (serializationCount < t.EventsToGenerate)
					{
						if (serializationQueue.TryDequeue(out original))
						{
							byte[] data = LwesSerializer.Serialize(original);
							dataQueue.Enqueue(data);
							serializationCount++;
						}
					}
				}
				));

			byte[] buffer;
			while (deserializationCount < t.EventsToGenerate)
			{
				if (dataQueue.TryDequeue(out buffer))
				{
					totalByteCount += buffer.Length;
					Event copy = LwesSerializer.Deserialize(buffer, 0, buffer.Length, mock.Object);
					deserializationCount++;
				}
				else Thread.Sleep(0);
			}

			timer.Stop();

			Console.Write(String.Concat("Generated ", t.EventsToGenerate, " events in ", timer.Elapsed
				, " which is ", (t.EventsToGenerate / timer.ElapsedMilliseconds), " per millisecond and "
				, (totalNumberOfAttributes / timer.ElapsedMilliseconds), " attributes per millisecond.",
				Environment.NewLine, (totalByteCount / timer.Elapsed.TotalSeconds).ToString("N00"), " bytes per second"));
		}

		#endregion Methods
	}
}