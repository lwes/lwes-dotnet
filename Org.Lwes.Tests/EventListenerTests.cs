namespace Org.Lwes.Tests
{
	using System;
	using System.Net;
	using System.Threading;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Org.Lwes.Emitter;
	using Org.Lwes.Listener;

	[TestClass]
	public class EventListenerTests
	{
		#region Fields

		bool _done;
		Event _receivedEvent;
		Event _sourceEvent;

		#endregion Fields

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

			IEventListener listener = EventListener.CreateDefault();
			Assert.IsNotNull(listener);
			listener.OnEventArrival += new HandleEventArrival(listener_OnEventArrival);

			IEventEmitter emitter = EventEmitter.CreateDefault();
			Assert.IsNotNull(emitter);

			// Create the event...
			Event _sourceEvent = new Event(e.EventName)
				.SetValue(e.Attributes.UserName.Name, e.Attributes.UserName.Value)
				.SetValue(e.Attributes.Password.Name, e.Attributes.Password.Value)
				.SetValue(e.Attributes.ClientIP.Name, e.Attributes.ClientIP.Value)
				.SetValue(e.Attributes.Successful.Name, e.Attributes.Successful.Value);

			long time_out_ticks = DateTime.Now.Ticks + TimeSpan.FromSeconds(20).Ticks;

			while (!_done && DateTime.Now.Ticks < time_out_ticks)
			{
				emitter.Emit(_sourceEvent);
				Thread.Sleep(1000);
			}

			Assert.IsTrue(_done, "Should have received an event");
			Assert.AreEqual(_sourceEvent[e.Attributes.UserName.Name].GetValue<string>(), _receivedEvent[e.Attributes.UserName.Name].GetValue<string>());
			Assert.AreEqual(_sourceEvent[e.Attributes.Password.Name].GetValue<ulong>(), _receivedEvent[e.Attributes.Password.Name].GetValue<ulong>());
			Assert.AreEqual(_sourceEvent[e.Attributes.ClientIP.Name].GetValue<IPAddress>(), _receivedEvent[e.Attributes.ClientIP.Name].GetValue<IPAddress>());
			Assert.AreEqual(_sourceEvent[e.Attributes.Successful.Name].GetValue<bool>(), _receivedEvent[e.Attributes.Successful.Name].GetValue<bool>());
			Console.Write(_receivedEvent.ToString());
		}

		void listener_OnEventArrival(IEventListener sender, Event ev)
		{
			_done = true;
			_receivedEvent = ev;
		}

		#endregion Methods
	}
}