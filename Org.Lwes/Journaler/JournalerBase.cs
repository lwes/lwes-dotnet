// 
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT (C) 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
//   original .NET implementation
// 
// LWES.net is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// LWES.net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
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