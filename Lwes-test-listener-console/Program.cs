namespace Lwes_test_listener_console
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;

	using Org.Lwes;
	using Org.Lwes.Listener;

	class Program
	{
		#region Fields

		static SimpleLockFreeQueue<Event> __incomingEvents = new SimpleLockFreeQueue<Event>();

		#endregion Fields

		#region Methods

		static void Main(string[] args)
		{
			using (IEventListener listener = EventListener.CreateDefault())
			{
				listener.OnEventArrival += new HandleEventArrival(listener_OnEventArrival);
				ManualResetEvent printerExited = new ManualResetEvent(false);
				bool userChoseToExit = false;
				int receivedEvents = 0;

				Console.WriteLine("LWES EventListener -\r\n  This console will continue to queue and print LWES events until\r\n  the user types 'exit' followed by a carriage return.");

				Object consoleWriteLock = new Object();

				ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
				{
					Event ev;
					while (!userChoseToExit)
					{
						if (__incomingEvents.Dequeue(out ev))
						{
							lock (consoleWriteLock)
							{
								Console.WriteLine(ev.ToString(true));
							}
							receivedEvents++;
						}
						else
						{
							Thread.Sleep(200);
						}
					}
					printerExited.Set();
				}));

				ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
				{
					while (!userChoseToExit)
					{
						int eventCount = receivedEvents;
						Thread.Sleep(2000);
						if (eventCount < receivedEvents)
						{
							lock (consoleWriteLock)
							{
								Console.WriteLine("LWES EventListener -\r\n  This console will continue to queue and print LWES events until\r\n  the user types 'exit' followed by a carriage return.");
							}
						}
					}
				}));

				string input;
				do
				{
					input = Console.ReadLine();
				} while (!String.Equals(input, "exit", StringComparison.CurrentCultureIgnoreCase));
				userChoseToExit = true;
				listener.Dispose();

				printerExited.WaitOne();
			}
		}

		static void listener_OnEventArrival(IEventListener sender, Org.Lwes.Event ev)
		{
			__incomingEvents.Enqueue(ev);
		}

		#endregion Methods
	}
}