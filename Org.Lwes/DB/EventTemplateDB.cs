namespace Org.Lwes.DB
{
	using System;
	using System.Configuration;

	using Microsoft.Practices.ServiceLocation;

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
			IEventTemplateDB result = IoCAdapter.CreateFromIoC<IEventTemplateDB>(Constants.DefaultEventTemplateDBContainerKey);

			if (result == null)
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

		public static IEventTemplateDB CreateFromConfig(string name)
		{
			TemplateDBConfigurationSection namedTemplateDBConfig = null;
			LwesConfigurationSection config = ConfigurationManager.GetSection(LwesConfigurationSection.SectionName) as LwesConfigurationSection;
			if (config != null)
			{
				namedTemplateDBConfig = config.TemplateDBs[name];
			}
			if (namedTemplateDBConfig == null) return null;

			FilePathEventTemplateDB db = new FilePathEventTemplateDB();
			db.InitializeFromFilePath(namedTemplateDBConfig.Path);
			return db;
		}

		public static IEventTemplateDB CreateNamedTemplateDB(string name)
		{
			IEventTemplateDB result = IoCAdapter.CreateFromIoC<IEventTemplateDB>(name);
			if (result == null)
			{
				result = CreateFromConfig(name);
			}
			return result;
		}

		private static IEventTemplateDB CreateFallbackTemplateDB()
		{
			FilePathEventTemplateDB db = new FilePathEventTemplateDB();
			db.InitializeFromFilePath(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
			return db;
		}

		#endregion Methods
	}
}