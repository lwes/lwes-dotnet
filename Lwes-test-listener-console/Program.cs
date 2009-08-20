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
		#region Methods

		static void Main(string[] args)
		{
			using (IEventListener listener = EventListener.CreateDefault())
			{
				EventSink sink = new EventSink();
				listener.RegisterEventSink(sink).Activate();
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
						if (sink.Events.Dequeue(out ev))
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

		#endregion Methods

		#region Nested Types

		class EventSink : IEventSink
		{
			#region Fields

			SimpleLockFreeQueue<Event> _incomingEvents = new SimpleLockFreeQueue<Event>();

			#endregion Fields

			#region Properties

			public SimpleLockFreeQueue<Event> Events
			{
				get { return _incomingEvents; }
			}

			public bool IsThreadSafe
			{
				get { return false; }
			}

			#endregion Properties

			#region Methods

			public void HandleEventArrival(IEventSinkRegistrationKey key, Event ev)
			{
				_incomingEvents.Enqueue(ev);
			}

			public GarbageHandlingVote HandleGarbageData(IEventSinkRegistrationKey key, System.Net.EndPoint remoteEndPoint, int priorGarbageCountForEndpoint, byte[] garbage)
			{
				return GarbageHandlingVote.IgnoreAllTrafficFromEndpoint;
			}

			#endregion Methods
		}

		#endregion Nested Types
	}
}