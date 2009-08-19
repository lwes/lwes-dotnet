namespace Org.Lwes.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	/// <summary>
	/// Summary description for StatusTests
	/// </summary>
	[TestClass]
	public class StatusTests
	{
		#region Enumerations

		enum TestStates
		{
			Default = 0,
			On = 2,
			Off = 3,
			Undecided = 4,
			ShutdownSignaled = 5,
			OnStateDone = 6,
			OffStateDone = 7,
			Done = 8
		}

		#endregion Enumerations

		#region Properties

		public TestContext TestContext
		{
			get; set;
		}

		#endregion Properties

		#region Methods

		[TestMethod]
		public void TestParallelStateTransitions()
		{
			// The intention here is to cause maximum contention on the Status<> class's methods
			// and thereby provide sufficient confidence that the class provides non-blocking,
			// thread-safe state transitions.
			var control = new
				{
					NumberOfThreadsPerState = 10,
					WaitPulseMilliseconds = 200,
					MainThreadWaitTimeBeforeShutdown = TimeSpan.FromSeconds(10)
				};

			Status<TestStates> state = new Status<TestStates>(default(TestStates));
			int onThreadsStarted = 0;
			int offThreadsStarted = 0;
			int undecidedThreadsStarted = 0;

			int transitionsOn = 0;
			int transitionsOff = 0;
			int transitionsUndecided = 0;
			int transitionsOnStateDone = 0;
			int transitionsOffStateDone = 0;
			int transitionsDone = 0;

			for (int i = 0; i < control.NumberOfThreadsPerState; i++)
			{
				// These background jobs transition the state from TestStates.Undecided
				// to TestStates.On - when successful it increments transitionsOn.
				ThreadPool.QueueUserWorkItem(new WaitCallback((unused_state) =>
					{
						state.SpinWaitForState(TestStates.Undecided, () => Thread.Sleep(control.WaitPulseMilliseconds));
						Interlocked.Increment(ref onThreadsStarted);
						while (state.IsLessThan(TestStates.ShutdownSignaled))
						{
							state.TrySetState(TestStates.On, TestStates.Undecided, () =>
								{
									Interlocked.Increment(ref transitionsOn);
								});
						}
						state.TrySetState(TestStates.OnStateDone, TestStates.ShutdownSignaled, () =>
							{
								Interlocked.Increment(ref transitionsOnStateDone);
							});
					}));
				// These background jobs transition the state from TestStates.On
				// to TestStates.Off - when successful it increments transitionsOff.
				ThreadPool.QueueUserWorkItem(new WaitCallback((unused_state) =>
					{
						state.SpinWaitForState(TestStates.On, () => Thread.Sleep(control.WaitPulseMilliseconds));
						Interlocked.Increment(ref offThreadsStarted);

						while(state.IsLessThan(TestStates.OnStateDone))
						{
							state.TrySetState(TestStates.Off, TestStates.On, () =>
								{
									Interlocked.Increment(ref transitionsOff);
								});
						}
						state.TrySetState(TestStates.OffStateDone, TestStates.OnStateDone, () =>
						{
							Interlocked.Increment(ref transitionsOffStateDone);
						});
					}));
				// These background jobs transition the state from TestStates.Off
				// to TestStates.Undecided - when successful it increments transitionsUndecided.
				ThreadPool.QueueUserWorkItem(new WaitCallback((unused_state) =>
				{
					state.SpinWaitForState(TestStates.Off, () => Thread.Sleep(control.WaitPulseMilliseconds));
					Interlocked.Increment(ref undecidedThreadsStarted);

					while (state.IsLessThan(TestStates.OffStateDone))
					{
						state.TrySetState(TestStates.Undecided, TestStates.Off, () =>
						{
							Interlocked.Increment(ref transitionsUndecided);
						});
					}

					state.TrySetState(TestStates.Done, TestStates.OffStateDone, () =>
					{
						Interlocked.Increment(ref transitionsDone);
					});
				}));
			}

			// Signal the first group of threads that they should start.
			state.SetState(TestStates.Undecided);

			// Wait the prescribed amount of time...
			Thread.Sleep(control.MainThreadWaitTimeBeforeShutdown);
			// Signal the shutdown...
			state.SetState(TestStates.ShutdownSignaled);
			// Wait for background threads to shutdown...
			state.SpinWaitForState(TestStates.Done, () => Thread.Sleep(control.WaitPulseMilliseconds));

			Assert.IsTrue(Thread.VolatileRead(ref transitionsOn) >= Thread.VolatileRead(ref transitionsOff));
			Assert.IsTrue(Thread.VolatileRead(ref transitionsOff) >= Thread.VolatileRead(ref transitionsUndecided));
			Assert.AreEqual(1, Thread.VolatileRead(ref transitionsOnStateDone));
			Assert.AreEqual(1, Thread.VolatileRead(ref transitionsOffStateDone));
			Assert.AreEqual(1, Thread.VolatileRead(ref transitionsDone));

			Console.WriteLine(String.Concat("Threads transitioninig to On: ", onThreadsStarted, ", transitions = ", transitionsOn));
			Console.WriteLine(String.Concat("Threads transitioninig to Off: ", offThreadsStarted, ", transitions = ", transitionsOff));
			Console.WriteLine(String.Concat("Threads transitioninig to Undecided: ", undecidedThreadsStarted, ", transitions = ", transitionsUndecided));
		}

		#endregion Methods
	}
}