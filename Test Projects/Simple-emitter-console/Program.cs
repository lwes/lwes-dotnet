
namespace Sample
{
	using System.Net;
	using Org.Lwes;
	using Org.Lwes.Emitter;
	using System.Threading;	

	class Program
	{
		static void Main(string[] args)
		{
			// Just in case there's a listener we need to wait for...
			Thread.Sleep(2000);

			// IEventEmitter is an IDisposable so always protect
			// with either a using clause or try/finally...
			using (var emitter = EventEmitter.CreateDefault())
			{
				// Create the event...
				Event evt = new Event("UserLogin");
				evt.SetValue("username", "bob");
				evt.SetValue("password", 0xfeedabbadeadbeefUL);
				evt.SetValue("clientIP", IPAddress.Parse("127.0.0.1"));
				evt.SetValue("successful", false);

				// Emit the event...
				emitter.Emit(evt);
			}
		}
	}
}
