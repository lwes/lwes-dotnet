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

	#region Enumerations

	/// <summary>
	/// Indicates a journaler's state.
	/// </summary>
	public enum JournalerState
	{
		/// <summary>
		/// Indicates the state is unknown. DEFAULT
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// Indicates the journaler is being initialized.
		/// </summary>
		Initializing = 1,
		/// <summary>
		/// Indicates the journaler has been initialilzed.
		/// </summary>
		Initialized = 2,
		/// <summary>
		/// Indicates the journaler is starting.
		/// </summary>
		Starting = 2,
		/// <summary>
		/// Indicates the journaler is active.
		/// </summary>
		Active = 3,
		/// <summary>
		/// Indicates the journaler is stopping.
		/// </summary>
		Stopping = 4,
		/// <summary>
		/// Indicates the journaler has stopped.
		/// </summary>
		Stopped = 5,
		/// <summary>
		/// Indicates the journaler is disposing.
		/// </summary>
		Disposing = 6,
		/// <summary>
		/// Indicates the journaler has disposed of it's system resources and should no longer be used.
		/// </summary>
		Disposed = 7
	}

	#endregion Enumerations

	/// <summary>
	/// Interface for journalers.
	/// </summary>
	public interface IJournaler : IDisposable
	{
		/// <summary>
		/// Gets the journaler's state.
		/// </summary>
		JournalerState Status
		{
			get;
		}

		/// <summary>
		/// Initializes the journaler.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Starts the journaler.
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the journaler.
		/// </summary>
		void Stop();
	}
}