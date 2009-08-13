namespace Org.Lwes
{
	using System;
	using System.Configuration;
	using System.Threading;

	using Org.Lwes.Config;

	public static class Buffers
	{
		#region Fields

		static readonly int __bufferAllocationLength = Constants.CAllocationBufferLength;
		static readonly int __maxMemory = Constants.CMaximumBufferingMemory;

		static int __memoryInUse = 0;

		#endregion Fields

		#region Constructors

		static Buffers()
		{
			// Check for override via configuration...
			LwesConfigurationSection config = ConfigurationManager.GetSection(LwesConfigurationSection.SectionName) as LwesConfigurationSection;
			if (config != null)
			{
				__maxMemory = config.MaximumBufferingMemory;
				__bufferAllocationLength = config.BufferAllocationLength;
			}
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Acquires a buffer and tracks the total bytes in use for buffering.
		/// </summary>
		/// <param name="cancelSignal">an optional function that indicates whether
		/// the acquire shold be canceled</param>
		/// <returns>a buffer if memory becomes available before the cancel signal
		/// becomes <em>true</em>; otherwise <em>null</em></returns>
		internal static byte[] AcquireBuffer(Func<bool> cancelSignal)
		{
			//
			// This strategy equates to a spinwait.
			// It may be worthwhile to introduce a sleep after some number of failures,
			// although, I'm wary that newer requests would convoy in front of older,
			// sleeping requests. More testing is necessary. -Pdc
			//
			int init, fin = Thread.VolatileRead(ref __memoryInUse);
			while (true)
			{
				if (cancelSignal != null && cancelSignal())	return null;

				init = fin;
				fin = Interlocked.CompareExchange(ref __memoryInUse, init + __bufferAllocationLength, init);
				if (fin == init)
				{
					return new byte[__bufferAllocationLength];
				}
			}
		}

		internal static void ReleaseBuffer(byte[] buffer)
		{
			int init;
			while (true)
			{
				init = Thread.VolatileRead(ref __memoryInUse);
				if (Interlocked.CompareExchange(ref __memoryInUse, init - buffer.Length, init) == init)
					return;
			}
		}

		#endregion Methods
	}
}