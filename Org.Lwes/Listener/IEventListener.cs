namespace Org.Lwes.Listener
{
	using System;
	using System.Net;

	/// <summary>
	/// Interface for event listener implementations.
	/// </summary>
	public interface IEventListener : IDisposable
	{
		IEventSinkRegistrationKey RegisterEventSink(IEventSink sink);
	}
}