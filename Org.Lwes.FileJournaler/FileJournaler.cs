namespace Org.Lwes.FileJournaler
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Threading;
	using Org.Lwes.Journaler;
	using Org.Lwes.Trace;
	
	public class FileJournaler : JournalerBase, ITraceable
	{
		#region Fields

		Object _lock = new Object();
		SimpleLockFreeQueue<Filer> _filers = new SimpleLockFreeQueue<Filer>();
		const int CHeaderFixedSize = sizeof(long) + sizeof(int) + sizeof(int) + sizeof(int);
		const string CLwesJournalerControlFileName = "JournalerFileControl";
		const string CDefaultFileNamingDatePattern = "yyyy-MM-dd-HH-mm-ss";
		const string CDefaultFileNamePattern = "{0}_{1}.log";
		const int CDefaultMaxBytesPerFile = 2 << 27; // 128MB
				
		#endregion Fields

		public FileJournaler(string baseDirectory, string fileNamingDatePattern, string fileNamePattern, int fileRolloverSize)
		{
			if (String.IsNullOrEmpty(baseDirectory)) throw new ArgumentException("base directory must have a value", "baseDirectory");
			if (!Directory.Exists(baseDirectory)) throw new ArgumentException(String.Concat("base directory does not exist: ", baseDirectory), "baseDirectory");
			if (fileRolloverSize < (2 << 16)) throw new ArgumentOutOfRangeException("fileRolloverSize", "file rollover size is unreasonably small");
			FileNamingDatePattern = fileNamingDatePattern ?? CDefaultFileNamingDatePattern;
			FileNamePattern = fileNamePattern ?? CDefaultFileNamePattern;
			FileRolloverSize = fileRolloverSize;

			if (File.Exists(Path.Combine(baseDirectory, CLwesJournalerControlFileName)))
			{
			}
		}

		#region Methods

		class Filer : ITraceable
		{
			string _datePattern = CDefaultFileNamingDatePattern;
			string _fileNamingPattern = CDefaultFileNamePattern;
			int _maxBytesPerFile = CDefaultMaxBytesPerFile;
			Object _currentFileLock = new Object();
			FileLockPair _currentFile;

			class FileLockPair
			{
				Object _lock = new Object();
				FileStream _file;
				long _precalculatedLength = 0;
				bool _logFileIsFull = false;

				public FileLockPair(FileStream file)
				{
					_file = file;
				}

				public bool TryLockFileForWrite(out Stream file, long count, long maxLength)
				{
					Monitor.Enter(_lock);
					if ((_precalculatedLength + count) >= maxLength)
					{
						_logFileIsFull = true;
						Monitor.Exit(_lock);
						file = null;
						return false;
					}
					_precalculatedLength += count;
					file = _file;
					return true;
				}
				public void ReleaseLock()
				{
					Monitor.Exit(_lock);
				}
			}

			internal void PerformHandleData(IPEndPoint ep, byte[] data, int offset, int count, Action<Filer> onComplete)
			{
				// Do all header conversions outside of the lock...
				byte[] receiptTimeBytes = BitConverter.GetBytes(Org.Lwes.Constants.DateTimeToLwesTimeTicks(DateTime.UtcNow));
				byte[] senderIPBytes = ep.Address.GetAddressBytes();
				byte[] senderIPLenBytes = BitConverter.GetBytes(senderIPBytes.Length);
				byte[] senderPortBytes = BitConverter.GetBytes(ep.Port);

				Stream output;
				Action releaseLock = WriteLockStreamWithAdequateRoom(out output, CHeaderFixedSize + senderIPBytes.Length + count);
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
					releaseLock();
				}
				onComplete(this);
			}

			private Action WriteLockStreamWithAdequateRoom(out Stream output, int requiredBytesRemaining)
			{
				lock (_currentFileLock)
				{
					if (_currentFile == null || !_currentFile.TryLockFileForWrite(out output, requiredBytesRemaining, _maxBytesPerFile))
					{
						Rollover();
						_currentFile.TryLockFileForWrite(out output, requiredBytesRemaining, _maxBytesPerFile);
					}

					return new Action(() => { _currentFile.ReleaseLock(); });
				}
			}

			private void Rollover()
			{
				
				var fileName = String.Format(CDefaultFileNamePattern, "LWES-Journaler-", DateTime.UtcNow.ToString(CDefaultFileNamingDatePattern));
				FileStream fs = new FileStream(fileName, FileMode.CreateNew, 
					FileAccess.Write,
					FileShare.None, 
					2 << 18,
					FileOptions.SequentialScan);
				_currentFile = new FileLockPair(fs);


			}
		}

		protected override bool PerformHandleData(EndPoint remoteEP, byte[] data, int offset, int count)
		{
			Filer filer;
			while (!_filers.TryDequeue(out filer))
			{
				Thread.SpinWait(1000);
			}
			filer.PerformHandleData((IPEndPoint)remoteEP, data, offset, count, f => _filers.Enqueue(f) );
			return true;
		}

		public string BaseDirectory { get; private set; }
		public string FileNamePattern { get; private set; }
		public string FileNamingDatePattern { get; private set; }
		public int FileRolloverSize { get; private set; }

		private string GenerateFileName()
		{
			throw new NotImplementedException();
		}

		protected override bool PerformInitialize()
		{
			throw new NotImplementedException();
		}

		#endregion Methods
	}
}