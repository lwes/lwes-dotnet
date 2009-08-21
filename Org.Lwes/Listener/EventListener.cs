﻿// 
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT (C) 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
//   original .NET implementation
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

		/// <summary>
		/// Creates an listener from the configuration.
		/// </summary>
		/// <param name="name">name of the instance to create</param>
		/// <returns>the named instance if it exists within the configuration;
		/// otherwise null</returns>
		/// <remarks>Note that two subsequent calls to this method will return
		/// two separate instances of the configured instance.</remarks>
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
					namedListenerConfig.UseParallelEmit,
					namedListenerConfig.GarbageHandling);
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

		/// <summary>
		/// Creates the named instance. If an IoC container is in use the IoC
		/// container is consulted for the named instance first.
		/// </summary>
		/// <param name="name">name of the instance to create</param>
		/// <returns>the named instance if it exists within the IoC container
		/// or the configuration; otherwise null</returns>
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
				, true
				, ListenerGarbageHandling.Default);
			return emitter;
		}

		#endregion Methods
	}
}