using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Org.Lwes.Listener
{
	/// <summary>
	/// Strategies to be taken when garbage data arrives from an endpoint.
	/// </summary>
	public enum GarbageHandlingStrategy
	{
		/// <summary>
		/// No opinion on garbage handling.
		/// </summary>
		None,
		/// <summary>
		/// Continue handling traffic from the endpoint as if no garbage
		/// has arrived.
		/// </summary>
		ContinueNormalHandlingForEndpoint,
		/// <summary>
		/// Treats all traffic from the endpoint as garbage without trying to
		/// deserialize the data.
		/// </summary>
		TreatAllTrafficFromEndpointAsGarbage,
		/// <summary>
		/// Causes any data received from the endpoint to be discarded as soon
		/// as possible.
		/// </summary>
		FailfastForTrafficOnEndpoint,
		/// <summary>
		/// The default strategy. Same as FailfastForTrafficOnEndpoint
		/// </summary>
		Default = None
	}
	/// <summary>
	/// Represents the state of an event sink.
	/// </summary>
	public enum EventSinkStatus
	{
		/// <summary>
		/// Indicates the event sink has been suspended and should not receive
		/// events until activated. This is the initial state for new IEventSinkRegistrationKeys.
		/// </summary>
		Suspended = 0,		
		/// <summary>
		/// Indicates the event sink is active and may be notified of events
		/// at any time.
		/// </summary>
		Active = 1,
		/// <summary>
		/// For event sinks that are not thread-safe, indicates that the sink
		/// is currently being invoked.
		/// </summary>
		Notifying = 2,
		/// <summary>
		/// Indicates the event sink has been canceled.
		/// </summary>
		Canceled = 3
	}
	public interface IEventSinkRegistrationKey
	{
		IEventListener Listener { get; }
		EventSinkStatus Status { get; }
		bool Activate();
		bool Suspend();
		void Cancel();
		Object Handback { get; set; }
	}
	public interface IEventSink
	{
		bool IsThreadSafe { get; }
		void HandleEventArrival(IEventSinkRegistrationKey key, Event ev);
		GarbageHandlingStrategy HandleGarbageArrival(IEventSinkRegistrationKey key, EndPoint remoteEndPoint, int priorGarbageCountForEndpoint, byte[] garbage);
	}
}
