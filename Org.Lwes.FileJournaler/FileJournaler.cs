namespace Org.Lwes.FileJournaler
{
	using System;
	using System.IO;
	using System.Net;
	using System.Threading;
	using FlitBit.MemoryMap;
	using Org.Lwes.Journaler;

	public class FileJournaler : JournalerBase, ITraceable
	{
		#region Fields

		Object _lock = new Object();
		SimpleLockFreeQueue<Filer> _filers = new SimpleLockFreeQueue<Filer>();
		const int CHeaderFixedSize = sizeof(long) + sizeof(int) + sizeof(int) + sizeof(int);
		const string CLwesJournalerControlFileName = "JournalerDbControl";
				
		#endregion Fields

		#region Methods

		class Filer
		{
			MemoryMappedFile _workFile;
			Stream _outputStream;
			Mutex _mapReadLock;
			Mutex _mapWriteLock;
		
			internal void PerformHandleData(IPEndPoint ep, byte[] data, int offset, int count)
			{				
				// Do all header conversions outside of the lock...
				byte[] receiptTimeBytes = BitConverter.GetBytes(Org.Lwes.Constants.DateTimeToLwesTimeTicks(DateTime.UtcNow));
				byte[] senderIPBytes = ep.Address.GetAddressBytes();
				byte[] senderIPLenBytes = BitConverter.GetBytes(senderIPBytes.Length);
				byte[] senderPortBytes = BitConverter.GetBytes(ep.Port);

				Stream output;
				object lck = WriteLockStreamWithAdequateRoom(out output, CHeaderFixedSize + senderIPBytes.Length + count);
				try
				{
					// Write header info...
					output.Write(receiptTimeBytes, 0, receiptTimeBytes.Length);
					output.Write(senderIPLenBytes, 0, senderIPLenBytes.Length);
					output.Write(senderIPBytes, 0, senderIPBytes.Length);
					output.Write(senderPortBytes, 0, senderPortBytes.Length);
					// Write the data...
					output.Write(data, offset, count);
				}
				finally
				{
					try
					{
						output.Dispose();
					}
					catch (Exception e)
					{
						// TODO: Log the error but don't rethrow...
					}
					ReleaseWriteLock(lck);
				}				
			}

			private void ReleaseWriteLock(object lck)
			{
				throw new NotImplementedException();
			}

			private object WriteLockStreamWithAdequateRoom(out Stream output, int requiredBytesRemaining)
			{
				throw new NotImplementedException();
			}
		}

		protected override bool PerformHandleData(EndPoint remoteEP, byte[] data, int offset, int count)
		{
			Filer filer;
			while (!_filers.TryDequeue(out filer))
			{
				Thread.SpinWait(1000);
			}

			return true;														
		}

		private Stream GetOutputStreamWithEnoughRoomForData(int count)
		{
			throw new NotImplementedException();
		}

		public string JournalingFolder { get; private set; }
		public string FileNamePattern { get; private set; }

		protected override bool PerformInitialize()
		{
			if (!Directory.Exists(JournalingFolder)) throw new IOException("JournalingFolder does not exist");

			throw new NotImplementedException();
			//string fileName;
			//MemoryMappedFile mapFile = MemoryMappedFile.Create(fileName, MemoryProtection.ReadWrite, Int32.MaxValue, "LWES Journaler File");
		}

		#endregion Methods
	}
}