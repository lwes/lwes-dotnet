using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Org.Lwes
{
	public class Timer
	{
		#region imports
		/// <summary>
		/// Uses GetSystemTimes() for cpu clocks, and QueryPerformanceCounter() for elapsed time clock.
		/// </summary>
		/// <value>Uses GetSystemTimes() for cpu clocks, and QueryPerformanceCounter() for elapsed time clock.</value>
		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern bool GetSystemTimes(out ulong idleTicksBegin, out ulong kernelTicksBegin, out ulong userTicksBegin);
		/// <value>Ticks since the start of time. </value>
		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern bool QueryPerformanceCounter(out ulong ticks);
		/// <value>Query Performance Counter ticks per second. </value>
		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern bool QueryPerformanceFrequency(out ulong ticksPerSecond);
		#endregion

		#region Fields

		static ulong __ticksPerSecond = 1;
		/// <summary>
		/// Number of 100 nanosecond ticks in a second.
		/// </summary>
		const double CTicksPerSecond = 1e7;
		const int CAssumedOverhead = 3;
		/// <summary>
		/// Number of processors present on the machine.
		/// </summary>
		static readonly int __cpuCount = Environment.ProcessorCount;
		/// <summary>
		/// QueryPerformanceCounter ticks when timer was started.
		/// </summary>
		ulong _clockTicksBegin;
		/// <summary>
		/// GetSystemTime idle tick count when clock was started.
		/// </summary>
		ulong _idleTicksBegin;
		/// <summary>
		/// GetSystemTime kernal tick counter.
		/// </summary>
		ulong _kernelTicksBegin;
		/// <summary>
		/// GetSystemTime user tick counter.
		/// </summary>
		ulong _userTicksBegin;

		#endregion

		/// <summary>
		/// Starts a timers that track elapsed and cpu user, kernel, and idle time.
		/// </summary>
		public void Reset()
		{
			QueryPerformanceCounter(out _clockTicksBegin);
			_clockTicksBegin += CAssumedOverhead;
			GetSystemTimes(out _idleTicksBegin, out _kernelTicksBegin, out _userTicksBegin);
		}
		/// <summary>
		/// Returns elapsed time as value and cpu consumption as output param.
		/// </summary>
		/// <param name="cpuTime">CPU busy time in seconds since <see cref="Reset()"/> was invoked.
		/// Can be multiple of the number of processors on the machine.</param>
		/// <param name="cpuUtilization">CPU utilization since <see cref="Reset()"/> was invoked.
		/// On an 8-way system this is a number between 1 and 8.</param>
		/// <returns>Elapsed time since the timer was reset.</returns>
		public double Read(out double cpuTime, out double cpuUtilization)
		{   // compute elapsed time
			ulong clockTicksNow;                            // current QueryPerformanceCounter ticks
			QueryPerformanceCounter(out clockTicksNow);
			if (__ticksPerSecond == 1) QueryPerformanceFrequency(out __ticksPerSecond);
			double elapsedTime = ((double)(clockTicksNow - _clockTicksBegin)) / __ticksPerSecond; // wraps in 10,000 years. 
			// compute cpu time.
			ulong idleTicksNow, kernelTicksNow, userTicksNow;
			GetSystemTimes(out idleTicksNow, out kernelTicksNow, out userTicksNow);
			ulong idleTicks = (idleTicksNow - _idleTicksBegin);
			ulong kernelTicks = (kernelTicksNow - _kernelTicksBegin) + 1;
			ulong userTicks = (userTicksNow - _userTicksBegin) + 1;
			ulong busyTicks = (kernelTicks + userTicks - idleTicks) + 1;
			cpuTime = ((double)busyTicks / CTicksPerSecond);
			cpuTime = Math.Min(cpuTime, __cpuCount * elapsedTime); // limit utilization to 100%
			cpuUtilization = cpuTime / (elapsedTime * __cpuCount);
			return elapsedTime;
		}
		/// <value>ElapsedTime since the timer was Reset();</value>
		public double ElapsedTime
		{
			get
			{
				// compute elapsed time
				ulong clockTicksNow;                            // current QueryPerformanceCounter ticks
				QueryPerformanceCounter(out clockTicksNow);
				if (__ticksPerSecond == 1) QueryPerformanceFrequency(out __ticksPerSecond);
				double elapsedTime = ((double)(clockTicksNow - _clockTicksBegin)) / __ticksPerSecond; // wraps in 10,000 years. return name;
				return elapsedTime;
			}
		}
				
		/// <summary>
		/// Gets the amount of CPU time consumed since the timer was reset.
		/// </summary>
		public double CpuTime
		{
			get
			{  // compute cpu time.
				ulong idleTicksNow, kernelTicksNow, userTicksNow;
				GetSystemTimes(out idleTicksNow, out kernelTicksNow, out userTicksNow);
				ulong idleTicks = (idleTicksNow - _idleTicksBegin);
				ulong kernelTicks = (kernelTicksNow - _kernelTicksBegin) + 1;
				ulong userTicks = (userTicksNow - _userTicksBegin) + 1;
				ulong busyTicks = (kernelTicks + userTicks - idleTicks) + 1;
				double cpuTime = ((double)busyTicks / CTicksPerSecond);
				return Math.Min(cpuTime, __cpuCount * this.ElapsedTime); // limit utilization to 100%
			}
		}
		
		/// <summary>
		/// Gets the amount of user time consumed since the timer was reset.
		/// </summary>
		public double UserTime
		{
			get
			{  // compute User cpu time consumed .
				ulong idleTicksNow, kernelTicksNow, userTicksNow;
				GetSystemTimes(out idleTicksNow, out kernelTicksNow, out userTicksNow);
				ulong userTicks = (userTicksNow - _userTicksBegin) + 1;
				return (double)userTicks / CTicksPerSecond;
			}
		}

		/// <summary>
		/// Gets the amount of kernal time consumed since the timer was reset.
		/// </summary>
		public double KernelTime
		{
			get
			{  // compute kernel cpu time consumed.
				ulong idleTicksNow, kernelTicksNow, userTicksNow;
				GetSystemTimes(out idleTicksNow, out kernelTicksNow, out userTicksNow);
				ulong kernelTicks = (kernelTicksNow - _kernelTicksBegin) - (idleTicksNow - _idleTicksBegin);
				return ((double)kernelTicks / CTicksPerSecond);
			}
		}
	}

}
