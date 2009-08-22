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
// GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes
{
	using System;
	using System.Configuration;
	using System.Threading;

	using Org.Lwes.Config;

	/// <summary>
	/// A simple utility for managing the the amount of memory used
	/// for buffering events. This class imposes the limit
	/// set by the configuration lwes\maximumBufferingMemory
	/// </summary>
	internal static class BufferManager
	{
		#region Fields

		static readonly int __bufferAllocationLength = Constants.CAllocationBufferLength;
		static readonly int __maxMemory = Constants.CMaximumBufferMemory;

		static int __memoryInUse = 0;

		#endregion Fields

		#if DEBUG

		static int __waitCount = 0;

		#endif

		#region Constructors

		static BufferManager()
		{
			// Check for override via configuration...
			LwesConfigurationSection config = LwesConfigurationSection.Current;
			__maxMemory = config.MaximumBufferMemory;
			__bufferAllocationLength = config.BufferAllocationLength;
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
			return AcquireBuffer(__bufferAllocationLength, cancelSignal);
		}

		internal static byte[] AcquireBuffer(int bufferLength, Func<bool> cancelSignal)
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
				if (cancelSignal != null && cancelSignal()) return null;

				init = fin;
				if (init < (__maxMemory - bufferLength))
				{
					fin = Interlocked.CompareExchange(ref __memoryInUse, init + bufferLength, init);
					if (fin == init)
					{
						return new byte[bufferLength];
					}
				}
			#if DEBUG
				Interlocked.Increment(ref __waitCount);
			#endif
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