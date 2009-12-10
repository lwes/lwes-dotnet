/// 
/// This file is part of the LWES .NET Binding (LWES.net)
///
/// COPYRIGHT© 2009, Phillip Clark (phillip[at*flitbit[dot*org)
///   original .NET implementation
/// 
/// LWES.net is free software: you can redistribute it and/or modify
/// it under the terms of the Lesser GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// LWES.net is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// Lesser GNU General Public License for more details.
///
/// You should have received a copy of the Lesser GNU General Public License
/// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
///namespace Lwes_test_emitter_console
namespace Org.Lwes.Tests
{
	using System;
	using System.Diagnostics;
	using System.Net;
	using System.Threading;

	using Org.Lwes;
	using Org.Lwes.DB;
	using Org.Lwes.Emitter;
	using Org.Lwes.Trace;

	class Program
	{
		#region Methods

		static void Main(string[] args)
		{
			Traceable.TraceEvent(typeof(Program), TraceEventType.Verbose,
				1, "Starting Lwes-test-emitter-console");

			var rand = new Random(Environment.TickCount);
			var control = new { NumberOfEventsToEmit = 1000000, MaxNumberOfAttributes = 25 };

			Console.WriteLine(String.Format("LWES EventEmitter - \r\nThis console will generate and emit {0} random events to the Light Weight Event System"
				, control.NumberOfEventsToEmit.ToString("N0")));

			Thread.Sleep(1000);

			// Create an emitter - this is the emitter named in the lwes/emitters
			// configuration node.
			using (IEventEmitter emitter = EventEmitter.CreateDefault())
			{
				for (int i = 0; i < control.NumberOfEventsToEmit; i++)
				{
					Event ev = EventUtils.GenerateRandomEvent(String.Concat("TestEvent_", i), control.MaxNumberOfAttributes, SupportedEncoding.UTF_8);
					emitter.Emit(ev);

					// Simulated processing between events.. up to 1000 spins
					Thread.SpinWait(rand.Next(1000));
				}
			}

			Traceable.TraceEvent(typeof(Program), TraceEventType.Verbose,
				2, "Stopping Lwes-test-emitter-console");
		}

		#endregion Methods
	}
}