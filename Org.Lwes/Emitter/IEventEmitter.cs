namespace Org.Lwes.Emitter
{
	using System;
	using System.IO;

	using Org.Lwes.Factory;

	/// <summary>
	/// Interface for classes that emit Events into the light-weight event system.
	/// </summary>
	/// <remarks>
	/// This interface makes the use of the IDisposable pattern explicit; implementations 
	/// must guarantee that cleanup occured before returning from Dispose.
	/// </remarks>
	public interface IEventEmitter : IEventFactory, IDisposable
	{
		#region Methods

		/// <summary>
		/// Emits an event to the event system.
		/// </summary>
		/// <param name="evt">the event being emitted</param>
		void Emit(Event evt);

		#endregion Methods
	}
}