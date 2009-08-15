namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Net;
	using System.Text;

	/// <summary>
	/// Configuration section for an event template DB.
	/// </summary>
	public class TemplateDBConfigurationSection : ConfigurationSection
	{
		#region Fields

		/// <summary>
		/// Property name for the tempate DB's name.
		/// </summary>
		public const string PropertyName_name = "name";
		/// <summary>
		/// Property name for the path.
		/// </summary>
		public const string PropertyName_path = "path";

		#endregion Fields

		#region Properties

		/// <summary>
		/// The name of the configured template DB.
		/// </summary>
		[ConfigurationProperty(PropertyName_name, IsRequired = true)]
		public string Name
		{
			get { return (string)this[PropertyName_name]; }
			set {	this[PropertyName_name] = value; }
		}

		/// <summary>
		/// Path where the template DB should look for '.esf' files.
		/// </summary>
		[ConfigurationProperty(PropertyName_path
			, IsRequired = true)]
		public string Path
		{
			get { return (string)this[PropertyName_path]; }
			set
			{
				this[PropertyName_path] = value;
			}
		}

		#endregion Properties
	}
}