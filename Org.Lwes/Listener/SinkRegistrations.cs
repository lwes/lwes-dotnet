#region Header

//
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT© 2009, Phillip Clark (phillip[at*flitbit[dot*org)
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
// Lesser GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//

#endregion Header

namespace Org.Lwes.Listener
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;

	internal class SinkRegistrations<TKey> : IEnumerable<TKey>
		where TKey : class, ISinkRegistrationKey
	{
		#region Fields

		const int LeadNotifier = 1;

		List<TKey> _additions = new List<TKey>();
		int _consolidationVotes = 0;
		int _notifiers = 0;
		TKey[] _registrations = new TKey[0];
		ReaderWriterLockSlim _rwlock = new ReaderWriterLockSlim();

		#endregion Fields

		#region Methods

		public IEnumerator<TKey> GetEnumerator()
		{
			int notifier = Interlocked.Increment(ref _notifiers);
			try
			{
				if (notifier == LeadNotifier) _rwlock.EnterUpgradeableReadLock();
				else _rwlock.EnterReadLock();
				try
				{
					foreach (var r in _registrations)
					{
						yield return r;
						if (r.Status == SinkStatus.Canceled)
							Interlocked.Increment(ref _consolidationVotes);
					}
					if (notifier == LeadNotifier && Thread.VolatileRead(ref _consolidationVotes) > 0)
					{
						SafeConsolidateRegistrations();
					}
				}
				finally
				{
					if (notifier == LeadNotifier) _rwlock.ExitUpgradeableReadLock();
					else _rwlock.ExitReadLock();
				}
			}
			finally
			{
				Interlocked.Decrement(ref _notifiers);
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal void AddRegistration(TKey key)
		{
			int notifier = Interlocked.Increment(ref _notifiers);
			try
			{
				if (notifier == LeadNotifier)
				{
					if (_rwlock.TryEnterWriteLock(20))
					{
						try
						{
							lock (_additions)
							{
								_additions.Add(key);
								UnsafeConsolidateRegistrations();
							}
							return;
						}
						finally
						{
							_rwlock.ExitWriteLock();
						}
					}
				}

				// We couldn't get the writelock so we have to schedule
				// the key to be added by the leader thread...
				lock (_additions)
				{
					_additions.Add(key);
					Interlocked.Increment(ref _consolidationVotes);
				}
			}
			finally
			{
				Interlocked.Decrement(ref _notifiers);
			}
		}

		private void SafeConsolidateRegistrations()
		{
			_rwlock.EnterWriteLock();
			try
			{
				lock (_additions)
				{
					UnsafeConsolidateRegistrations();
				}
			}
			finally
			{
				_rwlock.ExitWriteLock();
			}
		}

		private void UnsafeConsolidateRegistrations()
		{
			#if DEBUG
			Debug.Assert(_rwlock.IsWriteLockHeld);
			#endif
			_registrations = (from r in _registrations
												where r.Status != SinkStatus.Canceled
												select r).Concat(from r in _additions
																				 where r.Status != SinkStatus.Canceled
																				 select r).ToArray();
			_additions.Clear();

			Thread.VolatileWrite(ref _consolidationVotes, 0);
		}

		#endregion Methods
	}
}