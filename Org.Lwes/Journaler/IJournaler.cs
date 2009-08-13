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
		/// Indicates the journaler has disposed of it's system resources and should no longer be used.
		/// </summary>
		Disposed = 5
	}

	#endregion Enumerations

	/// <summary>
	/// Interface for journalers.
	/// </summary>
	public interface IJournaler : IDisposable
	{
		#region Properties

		/// <summary>
		/// Gets the journaler's state.
		/// </summary>
		JournalerState Status
		{
			get;
		}

		#endregion Properties

		#region Methods

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

		#endregion Methods
	}
}