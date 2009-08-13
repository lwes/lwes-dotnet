namespace Org.Lwes.Listener
{
	using System;
	using System.Configuration;
	using System.Net;

	using Microsoft.Practices.ServiceLocation;

	using Org.Lwes.Config;
	using Org.Lwes.DB;

	/// <summary>
	/// Utility class for accessing the default IEventListener implementation.
	/// </summary>
	/// <remarks>
	/// <para>If there is an ambient ServiceLocator present then this utility class will
	/// delegate to the ServiceLocator. The ServiceLocator should declare an instance
	/// of IEventListener with the name "eventListener".</para>
	/// <para>If an IoC container is not present, or if a service instance is not defined
	/// with the name "eventListener" then this utility will create a MutlicastEventListener.</para>
	/// </remarks>
	public static class EventListener
	{
		#region Methods

		/// <summary>
		/// Accesses the default instance of the IEventListener. Delegates to
		/// an IoC container if present.
		/// </summary>
		public static IEventListener CreateDefault()
		{
			IEventListener result = IoCAdapter.CreateFromIoC<IEventListener>(Constants.DefaultEventListenerContainerKey);

			if (result == null)
			{ // Either there isn't a default event template defined in the IoC container
				// or there isn't an IoC container in use... fall back to configuration section.
				result = CreateFromConfig(Constants.DefaultEventListenerConfigName);
			}
			if (result == null)
			{ // Not in IoC and not configured; fallback to programmatic default.
				result = CreateFallbackListener();
			}
			return result;
		}

		public static IEventListener CreateFromConfig(string name)
		{
			ListenerConfigurationSection namedListenerConfig = null;
			LwesConfigurationSection config = ConfigurationManager.GetSection(LwesConfigurationSection.SectionName) as LwesConfigurationSection;
			if (config != null)
			{
				namedListenerConfig = config.Listeners[name];
			}
			if (namedListenerConfig == null) return null;

			if (namedListenerConfig.UseMulticast)
			{
				MulticastEventListener mee = new MulticastEventListener();
				mee.Initialize(EventTemplateDB.CreateDefault(),
					IPAddress.Parse(namedListenerConfig.AddressString),
					namedListenerConfig.Port,
					namedListenerConfig.UseParallelEmit);
				return mee;
			}
			else
			{
				throw new NotImplementedException("TODO: Support UnicastEventListener");
				//UnicastEventListener mee = new UnicastEventListener();
				//mee.Initialize(EventTemplateDB.CreateDefault(),
				//  IPAddress.Parse(namedListenerConfig.AddressString),
				//  namedListenerConfig.Port,
				//  namedListenerConfig.UseParallelEmit);
				//return mee;
			}
		}

		public static IEventListener CreateNamedListener(string name)
		{
			IEventListener result = IoCAdapter.CreateFromIoC<IEventListener>(name);
			if (result == null)
			{
				result = CreateFromConfig(name);
			}
			return result;
		}

		private static IEventListener CreateFallbackListener()
		{
			MulticastEventListener emitter = new MulticastEventListener();
			emitter.Initialize(EventTemplateDB.CreateDefault()
				, Constants.DefaultMulticastAddress
				, Constants.CDefaultMulticastPort
				, true);
			return emitter;
		}

		#endregion Methods
	}
}