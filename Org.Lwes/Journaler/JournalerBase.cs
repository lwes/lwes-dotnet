namespace Org.Lwes.Journaler
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;

	using Org.Lwes.Properties;

	/// <summary>
	/// Base class implementation for journalers.
	/// </summary>
	public abstract class JournalerBase : IJournaler
	{
		#region Fields

		Status<JournalerState> _status;

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

		/// <summary>
		/// Disposes of the journaler.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Initializes the journaler.
		/// </summary>
		public void Initialize()
		{
			if (!_status.SetStateIfLessThan(JournalerState.Initializing, JournalerState.Initializing))
				throw new InvalidOperationException(Resources.Error_AlreadyInitialized);
		}

		/// <summary>
		/// Starts the journaler.
		/// </summary>
		public void Start()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Stops the journaler.
		/// </summary>
		public void Stop()
		{
			throw new NotImplementedException();
		}

		private void Dispose(bool disposing)
		{
			_status.SetStateIfLessThan(JournalerState.Disposed, JournalerState.Disposed);
		}

		#endregion Methods
	}
}