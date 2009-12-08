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
namespace Org.Lwes.Journaler
{
	using System;
	using System.Diagnostics;
	using System.Net;

	using Org.Lwes.Listener;
	using Org.Lwes.Properties;
	using Org.Lwes.Trace;

	/// <summary>
	/// Base class implementation for journalers.
	/// </summary>
	/// <remarks>
	/// This base class assumes there is a one-to-one relationship between the journaler
	/// and a listener.
	/// </remarks>
	public abstract class JournalerBase : IJournaler, IDataReceiverSink, ITraceable
	{
		#region Fields

		IEventListener _listener;
		ISinkRegistrationKey _registrationKey;
		Status<JournalerState> _status;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		protected JournalerBase()
		{
		}

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
		/// Indicates whether the journaler has been initialized.
		/// </summary>
		public bool IsInitialized
		{
			get { return _status.IsGreaterThan(JournalerState.Initializing); }
		}

		/// <summary>
		/// Gets the status of the journaler.
		/// </summary>
		public JournalerState Status
		{
			get { return _status.CurrentState; }
		}

		/// <summary>
		/// Accesses the listener from which the journaler is receiving data.
		/// </summary>
		protected IEventListener Listener
		{
			get { return _listener; }
			set
			{
				if (IsInitialized) throw new InvalidOperationException(Resources.Error_AlreadyInitialized);
				_listener = value;
			}
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Disposes of the journaler.
		/// </summary>
		public void Dispose()
		{
			if (_status.CurrentState == JournalerState.Active)
			{
				Stop();
			}
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

		bool IDataReceiverSink.HandleData(ISinkRegistrationKey key, EndPoint remoteEP, byte[] data, int offset, int count)
		{
			return PerformHandleData(remoteEP, data, offset, count);
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
					this.TraceData(TraceEventType.Error, Resources.Error_UnexpectedErrorInitializingJournaler, e);
				}
			}
			else
			{
				throw new InvalidOperationException(Resources.Error_AlreadyInitialized);
			}
		}

		/// <summary>
		/// Starts the journaler.
		/// </summary>
		public void Start()
		{
			if (_status.SetStateIfLessThan(JournalerState.Starting, JournalerState.Initialized))
			{
				try
				{
					_registrationKey = _listener.RegisterDataReceiverSink(this);
					// Let the subclass decide if the start succeeded.
					if (PerformStart(_registrationKey) && _registrationKey.Activate())
					{
						_status.TryTransition(JournalerState.Active, JournalerState.Starting);
					}
				}
				catch (Exception e)
				{
					this.TraceData(TraceEventType.Error, Resources.Error_UnexpectedErrorStartngJournaler, e);
					_status.TryTransition(JournalerState.Initialized, JournalerState.Starting);
				}
			}
		}

		/// <summary>
		/// Stops the journaler.
		/// </summary>
		public void Stop()
		{
			if (_status.SetStateIfLessThan(JournalerState.Stopping, JournalerState.Active))
			{
				try
				{
					_registrationKey.Suspend();
					// Let the subclass decide if the stop succeeded.
					if (PerformStop(_registrationKey))
					{
						_registrationKey = null;
						_status.TryTransition(JournalerState.Stopped, JournalerState.Stopping);
					}
				}
				catch (Exception e)
				{
					this.TraceData(TraceEventType.Error, Resources.Error_UnexpectedErrorStoppingJournaler, e);
					_status.TryTransition(JournalerState.Active, JournalerState.Stopping);
				}
			}
		}

		/// <summary>
		/// Disposes the journaler. Derived classes may override this method to perform
		/// additional cleanup.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Handles received data. Derived classes must override this method 
		/// to handle data received from the Light Weight Event System.
		/// </summary>
		/// <param name="data">a byte array containing data received from the endpoint</param>
		/// <param name="count">number of bytes received</param>
		/// <param name="offset">offset to the first data byte in the buffer</param>
		/// <param name="remoteEP">remote endpoint that sent the data</param>
		/// <returns><em>true</em> if the listener should continue notifying the sink-chain; otherwise <em>false</em></returns>
		protected abstract bool PerformHandleData(EndPoint remoteEP, byte[] data, int offset, int count);

		/// <summary>
		/// Performs initialization of the derived class.
		/// </summary>
		/// <returns></returns>
		protected abstract bool PerformInitialize();

		/// <summary>
		/// Starts the journaler; derived classes may override this method to perform
		/// additional logic when starting. The registration key controls the journaler's
		/// registration with the underlying IEventListener. At the time of the call
		/// the registration is suspended (journaler is not yet receiving LWES events).
		/// It is the responsibility of the derived class to either call the base-class
		/// PerformStart method or activate the registration key. LWES events will not
		/// flow to the journaler until the key has been activated.
		/// </summary>
		/// <param name="registrationKey"></param>
		/// <returns></returns>
		protected virtual bool PerformStart(ISinkRegistrationKey registrationKey)
		{
			if (registrationKey == null) throw new ArgumentNullException("registrationKey");
			return registrationKey.Activate();
		}

		/// <summary>
		/// Stops the journaler; derived classes may override this method to perform
		/// additional logic when stopping. The registration key controls the journaler's
		/// registration with the underlying IEventListener. At the time of the call
		/// the registration is suspended (journaler is no longer receiving LWES events).
		/// It is the responsibility of the derived class to either call the base-class
		/// PerformStop method or cancel the registration key. The journaler will hold
		/// the registration key indefinitely if it is not canceled.
		/// </summary>
		/// <param name="registrationKey"></param>
		/// <returns></returns>
		protected virtual bool PerformStop(ISinkRegistrationKey registrationKey)
		{
			if (registrationKey == null) throw new ArgumentNullException("registrationKey");
			registrationKey.Cancel();
			return true;
		}

		#endregion Methods
	}
}