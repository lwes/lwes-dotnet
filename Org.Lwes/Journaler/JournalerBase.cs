namespace Org.Lwes.Journaler
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;

	using Org.Lwes.Properties;

	public abstract class JournalerBase : IJournaler
	{
		#region Fields

		Status<JournalerState> _status;

		#endregion Fields

		#region Constructors

		~JournalerBase()
		{
			Dispose(false);
		}

		#endregion Constructors

		#region Properties

		public JournalerState Status
		{
			get { return _status.State; }
		}

		#endregion Properties

		#region Methods

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Initialize()
		{
			if (!_status.StateTransitionIfLessThan(JournalerState.Initializing, JournalerState.Initializing))
				throw new InvalidOperationException(Resources.Error_AlreadyInitialized);
		}

		public void Start()
		{
			throw new NotImplementedException();
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}

		private void Dispose(bool disposing)
		{
			_status.StateTransitionIfLessThan(JournalerState.Disposed, JournalerState.Disposed);
		}

		#endregion Methods
	}
}