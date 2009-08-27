//
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT© 2009, Phillip Clark (phillip[at*flitbit[dot*org)
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
				bool userChoseToExit = false;
				int eventCount = 0, incomingCount = 0;
				Event mostRecent = default(Event);

				Console.WriteLine("LWES EventListener -\r\n  This console will continue to queue and print LWES events until\r\n  the user types 'exit' followed by a carriage return.");
				
				listener.OnEventArrived += (sender, ev)	=>
					{
						Thread.MemoryBarrier();
						mostRecent = ev;
						Thread.MemoryBarrier();
						Interlocked.Increment(ref incomingCount);
					};

				ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
				{
					while (!userChoseToExit)
					{
						Thread.Sleep(2000);
						if (eventCount < Thread.VolatileRead(ref incomingCount))
						{
							eventCount = Thread.VolatileRead(ref incomingCount);
							Thread.MemoryBarrier();
							Event mr = mostRecent;
							Thread.MemoryBarrier();
							Console.WriteLine(mr.ToString(true));
							Console.WriteLine("Events received: {0}. Type 'exit' and hit the <return> key to exit.", eventCount.ToString("N0"));							
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

				Thread.Sleep(200);
				Console.WriteLine("Final event count: {0}", incomingCount.ToString("N0"));
				Thread.Sleep(2000);

			}
		}

		#endregion Methods
	}
}