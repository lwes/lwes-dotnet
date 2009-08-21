/// 
/// This file is part of the LWES .NET Binding (LWES.net)
///
/// COPYRIGHT (C) 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
///   original .NET implementation
/// 
/// LWES.net is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// LWES.net is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
///
/// You should have received a copy of the GNU General Public License
/// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
///namespace Lwes_test_emitter_console
namespace Lwes_test_emitter_console
{
	using System;
	using System.Diagnostics;
	using System.Threading;

	using Org.Lwes;
	using Org.Lwes.Emitter;

	class Program
	{
		#region Methods

		static void Main(string[] args)
		{
			// Use emit some trace messages - these will route through
			// the configured LwesTraceListener to the LWES.
			// This is how most DotNet applications will interact with LWES.
			TraceSource ts = new TraceSource("Console");
			ts.TraceEvent(TraceEventType.Verbose, 1, "Starting Lwes-test-emitter-console");

			var control = new { NumberOfEventsToEmit = 10000, MaxNumberOfAttributes = 44 };

			Console.WriteLine(String.Format("LWES EventEmitter - \r\nThis console will generate and emit {0} random events to the LWES"
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
				}
			}

			ts.TraceEvent(TraceEventType.Verbose, 2, "Exiting Lwes-test-emitter-console");
		}

		#endregion Methods
	}
}