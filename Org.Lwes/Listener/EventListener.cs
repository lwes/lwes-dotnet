namespace Org.Lwes.Listener
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Text;

	using Microsoft.Practices.ServiceLocation;

	using Org.Lwes.DB;

	/// <summary>
	/// Utility class for accessing the default IEventListener implementation.
	/// </summary>
	/// <remarks>
	/// <para>If there is an ambient ServiceLocator present then this utility class will
	/// delegate to the ServiceLocator. The ServiceLocator should declare an instance
	/// of IEventListener with the name "eventListener".</para>
	/// <para>If an IoC container is not present, or if a service instance is not defined
	/// with the name "eventListener" then this utility will create a FilePathEventListener
	/// initialized to use the application's base directory to load event specification files (*.esf) files.</para>
	/// </remarks>
	public static class EventListener
	{
		#region Fields

		static Object __lock;
		static WeakReference __pseudoSingleton;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Accesses the default instance of the IEventListener. Delegates to
		/// an IoC container if present.
		/// </summary>
		public static IEventListener Default
		{
			get
			{
				IEventListener result = null;

				try
				{
					// Try to use the service locator - this is IoC implementation independent
					IServiceLocator loc = ServiceLocator.Current;
					if (loc != null)
					{
						result = loc.GetInstance<IEventListener>(Constants.DefaultEventListenerContainerKey);
					}
				}
				catch (NullReferenceException)
				{
					/* no IoC - fall through */
				}

				if (result == null)
				{ // Either there isn't a default event template defined in the IoC container
					// or there isn't an IoC container in use... fall back to programmatic default.
					result = InitializeDefaultFilePathListener();
				}
				return result;
			}
		}

		#endregion Properties

		#region Methods

		private static IEventListener InitializeDefaultFilePathListener()
		{
			// Lazy load both the lock and the pseudoSingleton because ideally we'd be
			// using an IoC...
			object lck = Util.NonBlockingLazyInitializeVolatile<Object>(ref __lock);

			lock (lck)
			{
				if (__pseudoSingleton == null || __pseudoSingleton.IsAlive == false)
				{
					MulticastEventListener listener = new MulticastEventListener();
					listener.Initialize(EventTemplateDB.CreateDefault()
						, Constants.DefaultMulticastAddress
						, Constants.CDefaultMulticastPort
						, true);
					__pseudoSingleton = new WeakReference(listener);
				}
			}
			return __pseudoSingleton.Target as IEventListener;
		}

		#endregion Methods
	}
}