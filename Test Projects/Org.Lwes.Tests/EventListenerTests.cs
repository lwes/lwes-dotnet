namespace Org.Lwes.Tests
{
	using System;
	using System.Net;
	using System.Threading;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using Org.Lwes.Emitter;
	using Org.Lwes.Listener;

	[TestClass]
	public class EventListenerTests
	{
		#region Methods

		[TestMethod]
		public void AccessDefaultListener()
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
			bool done = false;
			Event receivedEvent = default(Event);

			// Mock an IEventSink that records the incomming event...
			Mock<IEventSink> mock = new Mock<IEventSink>();
			mock.Setup(framework => framework.HandleEventArrival(It.IsAny<ISinkRegistrationKey>(), It.IsAny<Event>()))
				.Callback<ISinkRegistrationKey, Event>((k, ev) =>
				{
					receivedEvent = ev;
					done = true;
				});

			Event sourceEvent;
			using (IEventListener listener = EventListener.CreateDefault())
			{
				Assert.IsNotNull(listener);
				listener.RegisterEventSink(mock.Object).Activate();

				IEventEmitter emitter = EventEmitter.CreateDefault();
				Assert.IsNotNull(emitter);

				// Create the event...
				sourceEvent = new Event(e.EventName);
				sourceEvent.SetValue(e.Attributes.UserName.Name, e.Attributes.UserName.Value);
				sourceEvent.SetValue(e.Attributes.Password.Name, e.Attributes.Password.Value);
				sourceEvent.SetValue(e.Attributes.ClientIP.Name, e.Attributes.ClientIP.Value);
				sourceEvent.SetValue(e.Attributes.Successful.Name, e.Attributes.Successful.Value);

				long time_out_ticks = DateTime.Now.Ticks + TimeSpan.FromSeconds(20).Ticks;

				while (!done && DateTime.Now.Ticks < time_out_ticks)
				{
					emitter.Emit(sourceEvent);
					Thread.Sleep(1000);
				}
			}
			Assert.IsTrue(done, "Should have received an event");
			Assert.AreEqual(sourceEvent[e.Attributes.UserName.Name].GetValue<string>(), receivedEvent[e.Attributes.UserName.Name].GetValue<string>());
			Assert.AreEqual(sourceEvent[e.Attributes.Password.Name].GetValue<ulong>(), receivedEvent[e.Attributes.Password.Name].GetValue<ulong>());
			Assert.AreEqual(sourceEvent[e.Attributes.ClientIP.Name].GetValue<IPAddress>(), receivedEvent[e.Attributes.ClientIP.Name].GetValue<IPAddress>());
			Assert.AreEqual(sourceEvent[e.Attributes.Successful.Name].GetValue<bool>(), receivedEvent[e.Attributes.Successful.Name].GetValue<bool>());
			Console.Write(receivedEvent.ToString());
		}

		#endregion Methods
	}
}