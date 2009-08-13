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

			Console.WriteLine(String.Format(@"LWES EventEmitter -
			This console will generate and emit {0} random events to the LWES", control.NumberOfEventsToEmit.ToString("N0")));

			Thread.Sleep(1000);

			// Create an emitter - this is the emitter named in the lwes/emitters
			// configuration node.
			using (IEventEmitter emitter = EventEmitter.CreateNamedEmitter("Default"))
			{
				for (int i = 0; i < control.NumberOfEventsToEmit; i++)
				{
					Event ev = EventUtils.GenerateRandomEvent(String.Concat("TestEvent_", i), control.MaxNumberOfAttributes);
					emitter.Emit(ev);
				}
			}

			ts.TraceEvent(TraceEventType.Verbose, 2, "Exiting Lwes-test-emitter-console");
		}

		#endregion Methods
	}
}