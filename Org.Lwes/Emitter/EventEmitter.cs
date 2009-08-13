namespace Org.Lwes.Emitter
{
	using System;
	using System.Configuration;
	using System.Net;

	using Microsoft.Practices.ServiceLocation;

	using Org.Lwes.Config;
	using Org.Lwes.DB;

	/// <summary>
	/// Utility class for accessing the default IEventEmitter implementation.
	/// </summary>
	/// <remarks>
	/// <para>If there is an ambient ServiceLocator present then this utility class will
	/// delegate to the ServiceLocator. The ServiceLocator should declare an instance
	/// of IEventEmitter with the name "eventEmitter".</para>
	/// <para>If an IoC container is not present, or if a service instance is not defined
	/// with the name "eventEmitter" then this utility will create a MulticastEventEmitter.</para>
	/// </remarks>
	public static class EventEmitter
	{
		#region Methods

		/// <summary>
		/// Accesses the default instance of the IEventEmitter. Delegates to
		/// an IoC container if present.
		/// </summary>
		public static IEventEmitter CreateDefault()
		{
			IEventEmitter result = IoCAdapter.CreateFromIoC<IEventEmitter>(Constants.DefaultEventEmitterContainerKey);

			if (result == null)
			{ // Either there isn't a default event template defined in the IoC container
				// or there isn't an IoC container in use... fall back to configuration section.
				result = CreateFromConfig(Constants.DefaultEventEmitterConfigName);
			}
			if (result == null)
			{ // Not in IoC and not configured; fallback to programmatic default.
				result = CreateFallbackEmitter();
			}
			return result;
		}

		public static IEventEmitter CreateFromConfig(string name)
		{
			EmitterConfigurationSection namedEmitterConfig = null;
			LwesConfigurationSection config = ConfigurationManager.GetSection(LwesConfigurationSection.SectionName) as LwesConfigurationSection;
			if (config != null)
			{
				namedEmitterConfig = config.Emitters[name];
			}
			if (namedEmitterConfig == null) return null;

			if (namedEmitterConfig.UseMulticast)
			{
				MulticastEventEmitter mee = new MulticastEventEmitter();
				mee.Initialize(namedEmitterConfig.Encoding, false, EventTemplateDB.CreateDefault(),
					IPAddress.Parse(namedEmitterConfig.AddressString), namedEmitterConfig.Port,
					namedEmitterConfig.MulticastTimeToLive, namedEmitterConfig.UseParallelEmit);
				return mee;
			}
			else
			{
				throw new NotImplementedException("TODO: Support UnicastEventEmitter");
				//UnicastEventEmitter mee = new UnicastEventEmitter();
				//mee.Initialize(namedEmitterConfig.Encoding, false, EventTemplateDB.Default,
				//  IPAddress.Parse(namedEmitterConfig.AddressString), namedEmitterConfig.Port,
				//  namedEmitterConfig.MulticastTimeToLive, namedEmitterConfig.UseParallelEmit);
				//return mee;
			}
		}

		public static IEventEmitter CreateNamedEmitter(string name)
		{
			IEventEmitter result = IoCAdapter.CreateFromIoC<IEventEmitter>(name);
			if (result == null)
			{
				result = CreateFromConfig(name);
			}
			return result;
		}

		private static IEventEmitter CreateFallbackEmitter()
		{
			MulticastEventEmitter emitter = new MulticastEventEmitter();
			emitter.Initialize(SupportedEncoding.Default
				, Constants.DefaultPerformValidation
				, EventTemplateDB.CreateDefault()
				, Constants.DefaultMulticastAddress
				, Constants.CDefaultMulticastPort
				, Constants.CDefaultMulticastTtl
				, false);
			return emitter;
		}

		#endregion Methods
	}
}