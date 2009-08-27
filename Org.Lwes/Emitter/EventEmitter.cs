//
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT© 2009, Phillip Clark (phillip[at*flitbit[dot*org)
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
// Lesser GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes.Emitter
{
	using System;
	using System.Net;

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
			IEventEmitter result;
			if (!IoCAdapter.TryCreateFromIoC<IEventEmitter>(Constants.DefaultEventEmitterContainerKey, out result))
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

		/// <summary>
		/// Creates an emitter from the configuration.
		/// </summary>
		/// <param name="name">name of the instance to create</param>
		/// <returns>the named instance if it exists within the configuration;
		/// otherwise null</returns>
		/// <remarks>Note that two subsequent calls to this method will return
		/// two separate instances of the configured instance.</remarks>
		public static IEventEmitter CreateFromConfig(string name)
		{
			LwesConfigurationSection config = LwesConfigurationSection.Current;
			if (config.Emitters == null) return null;

			EmitterConfigurationSection namedConfig = config.Emitters[name];
			if (namedConfig == null) return null;

			if (namedConfig.UseMulticast)
			{
				MulticastEventEmitter mee = new MulticastEventEmitter();
				mee.InitializeAll(namedConfig.Encoding, false, EventTemplateDB.CreateDefault(),
					IPAddress.Parse(namedConfig.AddressString), namedConfig.Port,
					namedConfig.MulticastTimeToLive, namedConfig.UseParallelEmit);
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

		/// <summary>
		/// Creates the named instance. If an IoC container is in use the IoC
		/// container is consulted for the named instance first.
		/// </summary>
		/// <param name="name">name of the instance to create</param>
		/// <returns>the named instance if it exists within the IoC container
		/// or the configuration; otherwise null</returns>
		public static IEventEmitter CreateNamedEmitter(string name)
		{
			IEventEmitter result;
			if (!IoCAdapter.TryCreateFromIoC<IEventEmitter>(name, out result))
			{
				result = CreateFromConfig(name);
			}
			return result;
		}

		private static IEventEmitter CreateFallbackEmitter()
		{
			MulticastEventEmitter emitter = new MulticastEventEmitter();
			emitter.InitializeAll(SupportedEncoding.Default
				, Constants.DefaultPerformValidation
				, EventTemplateDB.CreateDefault()
				, Constants.DefaultMulticastAddress
				, Constants.CDefaultMulticastPort
				, Constants.CDefaultMulticastTtl
				, true);
			return emitter;
		}

		#endregion Methods
	}
}