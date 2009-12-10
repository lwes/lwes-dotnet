using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Org.Lwes.Tests
{
	/// <summary>
	/// Summary description for TimerTests
	/// </summary>
	[TestClass]
	public class TimerTests
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void Basics()
		{
			double elapsedTime, cpuTime, userTime, kernelTime, cpuUtilization;
			Timer time = new Timer(),
						test = new Timer();
			time.Reset();
			for (int k = 1000000; k > 0; k--)
			{
				test.Reset(); // get times in one call
				elapsedTime = test.Read(out cpuTime, out cpuUtilization);
			}
			// get times one at a time. 
			elapsedTime = time.ElapsedTime;
			cpuTime = time.CpuTime;
			userTime = time.UserTime;
			kernelTime = time.KernelTime;
			Console.WriteLine("Timer read/write cost: {0:F4} microseconds.", elapsedTime);
			Console.WriteLine("CPU time: {0:F4} microseconds.", cpuTime);
			Console.WriteLine("User time: {0:F4} microseconds.", userTime);
			Console.WriteLine("Kernal time: {0:F4} microseconds.", kernelTime);
		}
	}
}
