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
	using System.Text.RegularExpressions;
	using System.Collections.Specialized;
	using System.IO;

	class Program
	{
		static void Main(string[] args)
		{
			var arguments = new Arguments(args);

			bool userChoseToExit = false;
			int eventCount = 0, incomingCount = 0;
			Event mostRecent = default(Event);
			SimpleLockFreeQueue<Event> writeQ = null;
			var fileName = arguments["f"];

			if (!String.IsNullOrEmpty(fileName))
			{
				writeQ = new SimpleLockFreeQueue<Event>();
				var dir = Path.GetDirectoryName(fileName);
				if (!String.IsNullOrEmpty(dir) && !Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}

				// Start the file writer...
				ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
				{					
					using (var f = File.CreateText(fileName))
					{
						while (!userChoseToExit)
						{
							Event ev;
							while (writeQ.TryDequeue(out ev))
							{
								f.Write(ev.ToString(true));
							}
							Thread.Sleep(200);
						}
					}
				}));
			}

			using (IEventListener listener = EventListener.CreateDefault())
			{
				Console.WriteLine("LWES EventListener -\r\n  This console will continue to queue and print LWES events until\r\n  the user types 'exit' followed by a carriage return.");

				listener.OnEventArrived += (sender, ev) =>
					{
						Thread.MemoryBarrier();
						mostRecent = ev;
						Thread.MemoryBarrier();
						if (writeQ != null) writeQ.Enqueue(ev);
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
			}
			Thread.Sleep(200);
			Console.WriteLine("Final event count: {0}", incomingCount.ToString("N0"));
			Thread.Sleep(2000);
		}		
	}

	public class Arguments
	{
		StringDictionary _params;

		public Arguments(string[] args)
		{
			_params = new StringDictionary();
			Regex spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			Regex remover = new Regex(@"^['""]?(.*?)['""]?$",	RegexOptions.IgnoreCase | RegexOptions.Compiled);

			string parm = null;
			string[] parts;

			foreach (var a in args)
			{
				parts = spliter.Split(a, 3);
				switch (parts.Length)
				{
					case 1:
						if (parm != null)
						{
							if (!_params.ContainsKey(parm))
							{
								parts[0] = remover.Replace(parts[0], "$1");
								_params.Add(parm, parts[0]);
							}
							parm = null;
						}
						break;
					case 2:
						if (parm != null)
						{
							if (!_params.ContainsKey(parm))
								_params.Add(parm, "true");
						}
						parm = parts[1];
						break;
					case 3:
						if (parm != null)
						{
							if (!_params.ContainsKey(parm))
								_params.Add(parm, "true");
						}
						parm = parts[1];
						if (!_params.ContainsKey(parm))
						{
							parts[2] = remover.Replace(parts[2], "$1");
							_params.Add(parm, parts[2]);
						}

						parm = null;
						break;
				}
			}

			if (parm != null)
			{
				if (!_params.ContainsKey(parm))
					_params.Add(parm, "true");
			}
		}

		public string this[string Param]
		{
			get
			{
				return (_params[Param]);
			}
		}
	}

}