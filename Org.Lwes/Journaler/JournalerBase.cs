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
namespace Org.Lwes.Journaler
{
	using System;

	using Org.Lwes.Properties;
	using System.Diagnostics;
using Org.Lwes.Listener;
	using System.Net;

	/// <summary>
	/// Base class implementation for journalers.
	/// </summary>
	/// <remarks>
	/// This base class assumes there is a one-to-one relationship between the journaler
	/// and a listener.
	/// </remarks>
	public abstract class JournalerBase : IJournaler, IEventSink
	{
		#region Fields

		Status<JournalerState> _status;
		IEventListener _listener;
		IEventSinkRegistrationKey _registrationKey;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Finalizes the journaler.
		/// </summary>
		~JournalerBase()
		{
			Dispose(false);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the status of the journaler.
		/// </summary>
		public JournalerState Status
		{
			get { return _status.CurrentState; }
		}

		#endregion Properties

		#region Methods

		protected JournalerBase(IEventListener listener)
		{
			if (listener == null) throw new ArgumentNullException("listener");
			_listener = listener;
		}

		/// <summary>
		/// Disposes of the journaler.
		/// </summary>
		public void Dispose()
		{
			if (_status.SetStateIfLessThan(JournalerState.Disposing, JournalerState.Disposing))
			{
				try
				{
					Dispose(true);
				}
				catch (Exception e)
				{
					this.TraceData(TraceEventType.Error, Resources.Error_UnexpectedErrorDisposingJournaler, e);
				}
				finally
				{
					GC.SuppressFinalize(this);
					_status.SetState(JournalerState.Disposed);
				}
			}
		}

		/// <summary>
		/// Initializes the journaler.
		/// </summary>
		public void Initialize()
		{
			if (_status.SetStateIfLessThan(JournalerState.Initializing, JournalerState.Initializing))
			{
				try
				{
					// Let the subclass decide if the initialize succeeded.
					if (PerformInitialize())
					{
						_status.TryTransition(JournalerState.Initialized, JournalerState.Initializing);
					}
				}
				catch (Exception e)
				{
					_status.TryTransition(JournalerState.Unknown, JournalerState.Initializing);
				}
			}
			else
			{
				throw new InvalidOperationException(Resources.Error_AlreadyInitialized);
			}
		}

		protected abstract bool PerformInitialize();

		/// <summary>
		/// Starts the journaler.
		/// </summary>
		public void Start()
		{
			if (_status.SetStateIfLessThan(JournalerState.Starting, JournalerState.Initialized))
			{
				try
				{
					_registrationKey = _listener.RegisterEventSink(this);
					// Let the subclass decide if the start succeeded.
					if (PerfromStart(_registrationKey) && _registrationKey.Activate())
					{
						_status.TryTransition(JournalerState.Active, JournalerState.Starting);						
					}
				}
				catch (Exception e)
				{
					this.TraceData(TraceEventType.Error, "Error trying to start journaler", e);
					_status.TryTransition(JournalerState.Initialized, JournalerState.Starting);
				}
			}
		}

		protected virtual bool PerfromStart(IEventSinkRegistrationKey registrationKey)
		{
			return true;
		}

		/// <summary>
		/// Stops the journaler.
		/// </summary>
		public void Stop()
		{
			if (_status.SetStateIfLessThan(JournalerState.Stopping, JournalerState.Active))
			{
				bool stopped = false;
				try
				{
					_registrationKey.Suspend();
					// Let the subclass decide if the stop succeeded.
					if (stopped = PerfromStop(_registrationKey))
					{
						_registrationKey.Cancel();
						_registrationKey = null;
					}
				}
				finally
				{
					// An exception in the subclass causes the state transition to fail.
					if (stopped)
						_status.TryTransition(JournalerState.Stopped, JournalerState.Stopping);
					else
						_status.TryTransition(JournalerState.Active, JournalerState.Stopping);
				}
			}
		}

		protected virtual bool PerfromStop(IEventSinkRegistrationKey registrationKey)
		{
			return true;
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		#endregion Methods

		#region IEventSink Members

		public bool IsThreadSafe
		{
			get;
			protected set;
		}

		void IEventSink.HandleEventArrival(IEventSinkRegistrationKey key, Event ev)
		{
			OnHandleEventArrival(key, ev);
		}

		protected abstract void OnHandleEventArrival(IEventSinkRegistrationKey key, Event ev);

		GarbageHandlingVote IEventSink.HandleGarbageData(IEventSinkRegistrationKey key, EndPoint remoteEndPoint, int priorGarbageCountForEndpoint, byte[] garbage)
		{
			return PerformHandleGarbageData(key, remoteEndPoint, priorGarbageCountForEndpoint, garbage);
		}

		protected virtual GarbageHandlingVote PerformHandleGarbageData(IEventSinkRegistrationKey key, EndPoint remoteEndPoint, int priorGarbageCountForEndpoint, byte[] garbage)
		{
			return GarbageHandlingVote.Default;
		}

		#endregion
	}
}