//
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT© 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
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
namespace Org.Lwes.DB
{
	using System;

	using Org.Lwes.Config;

	/// <summary>
	/// Utility class for accessing the default IEventTemplateDB implementation.
	/// </summary>
	/// <remarks>
	/// <para>If there is an ambient ServiceLocator present then this utility class will
	/// delegate to the ServiceLocator. The ServiceLocator should declare an instance
	/// of IEventTemplateDB with the name "eventTemplateDB".</para>
	/// <para>If an IoC container is not present, or if a service instance is not defined
	/// with the name "eventTemplateDB" then this utility will create a FilePathEventTemplateDB
	/// initialized to use the application's base directory to load event specification files (*.esf) files.</para>
	/// </remarks>
	public static class EventTemplateDB
	{
		#region Methods

		/// <summary>
		/// Accesses the default instance of the IEventTemplateDB. Delegates to
		/// an IoC container if present.
		/// </summary>
		public static IEventTemplateDB CreateDefault()
		{
			IEventTemplateDB result;
			if (!IoCAdapter.TryCreateFromIoC<IEventTemplateDB>(Constants.DefaultEventTemplateDBContainerKey, out result))
			{ // Either there isn't a default event template defined in the IoC container
				// or there isn't an IoC container in use... fall back to configuration section.
				result = CreateFromConfig(Constants.DefaultEventTemplateDBConfigName);
			}
			if (result == null)
			{ // Not in IoC and not configured; fallback to programmatic default.
				result = CreateFallbackTemplateDB();
			}
			return result;
		}

		/// <summary>
		/// Creates an event template DB from the configuration.
		/// </summary>
		/// <param name="name">name of the instance to create</param>
		/// <returns>the named instance if it exists within the configuration;
		/// otherwise null</returns>
		/// <remarks>Note that two subsequent calls to this method will return
		/// two separate instances of the configured instance.</remarks>
		public static IEventTemplateDB CreateFromConfig(string name)
		{
			LwesConfigurationSection config = LwesConfigurationSection.Current;
			if (config.TemplateDBs == null) return null;

			TemplateDBConfigurationSection namedTemplateDBConfig = config.TemplateDBs[name];
			if (namedTemplateDBConfig == null) return null;

			FilePathEventTemplateDB db = new FilePathEventTemplateDB();
			db.InitializeFromFilePath(namedTemplateDBConfig.Path, namedTemplateDBConfig.IncludeSubdirectories);
			return db;
		}

		/// <summary>
		/// Creates the named instance. If an IoC container is in use the IoC
		/// container is consulted for the named instance first.
		/// </summary>
		/// <param name="name">name of the instance to create</param>
		/// <returns>the named instance if it exists either within the IoC container
		/// or the configuration; otherwise null</returns>
		public static IEventTemplateDB CreateNamedTemplateDB(string name)
		{
			IEventTemplateDB result;
			if (!IoCAdapter.TryCreateFromIoC<IEventTemplateDB>(name, out result))
			{
				result = CreateFromConfig(name);
			}
			return result;
		}

		private static IEventTemplateDB CreateFallbackTemplateDB()
		{
			FilePathEventTemplateDB db = new FilePathEventTemplateDB();
			db.InitializeFromFilePath(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, true);
			return db;
		}

		#endregion Methods
	}
}