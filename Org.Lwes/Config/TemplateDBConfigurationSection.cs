namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Net;
	using System.Text;

	public class TemplateDBConfigurationSection : ConfigurationSection
	{
		#region Fields

		public const string PropertyName_name = "name";
		public const string PropertyName_path = "path";

		#endregion Fields

		#region Properties

		[ConfigurationProperty(PropertyName_name, IsRequired = true)]
		public string Name
		{
			get { return (string)this[PropertyName_name]; }
			set {	this[PropertyName_name] = value; }
		}

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