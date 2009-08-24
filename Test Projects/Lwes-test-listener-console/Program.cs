//
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT© 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
//	 original .NET implementation
//
// LWES.net is free software: you can redistribute it and/or modify
// it under the terms of the Lesser GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// LWES.net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// Lesser GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes.Tests
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
						if (sink.Events.TryDequeue(out ev))
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