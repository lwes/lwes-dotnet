namespace Org.Lwes.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Org.Lwes.Emitter;
	using Org.Lwes.Listener;

	[TestClass]
	public class EventEmitterTests
	{
		#region Fields

		Random _rand = new Random(Environment.TickCount);

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

			// Create the event...
			Event ev = new Event(e.EventName)
				.SetValue(e.Attributes.UserName.Name, e.Attributes.UserName.Value)
				.SetValue(e.Attributes.Password.Name, e.Attributes.Password.Value)
				.SetValue(e.Attributes.ClientIP.Name, e.Attributes.ClientIP.Value)
				.SetValue(e.Attributes.Successful.Name, e.Attributes.Successful.Value);

			IEventEmitter emitter = EventEmitter.CreateDefault();
			Assert.IsNotNull(emitter);

			emitter.Emit(ev);
		}

		#endregion Methods
	}
}