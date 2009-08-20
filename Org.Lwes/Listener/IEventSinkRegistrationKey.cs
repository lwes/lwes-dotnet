using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Lwes.Listener
{
	/// <summary>
	/// Registration key for event sinks.
	/// </summary>
	public interface IEventSinkRegistrationKey
	{
		/// <summary>
		/// An opaque handback object.
		/// </summary>
		Object Handback
		{
			get;
			set;
		}

		/// <summary>
		/// The listener with which the event sink is registered.
		/// </summary>
		IEventListener Listener
		{
			get;
		}

		/// <summary>
		/// The status of the event sink.
		/// </summary>
		EventSinkStatus Status
		{
			get;
		}

		/// <summary>
		/// Activates the event sink.
		/// </summary>
		/// <returns><em>true</em> if the event sink has not already been canceled; otherwise <em>false</em></returns>
		bool Activate();

		/// <summary>
		/// Cancels a registration. An event sink whose registration is canceled will no longer
		/// receive event or garbage notification.
		/// </summary>
		void Cancel();

		/// <summary>
		/// Suspends the event sink. An event sink whose registration is suspended will not
		/// receive event or garbage notification until it is re-activated.
		/// </summary>
		/// <returns><em>true</em> if the event sink has not already been canceled; otherwise <em>false</em></returns>
		bool Suspend();

		/// <summary>
		/// Disables garbage notification for a sink.
		/// </summary>		
		void DisableGarbageNotification();
	}

}
