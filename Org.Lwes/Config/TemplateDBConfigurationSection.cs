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
namespace Org.Lwes.Config
{
	using System;
	using System.Configuration;

	/// <summary>
	/// Configuration section for an event template DB.
	/// </summary>
	public class TemplateDBConfigurationSection : ConfigurationSection
	{
		#region Fields

		/// <summary>
		/// Property name for includeSubdirectories
		/// </summary>
		public const string PropertyName_includeSubdirectories = "includeSubdirectories";

		/// <summary>
		/// Property name for the tempate DB's name.
		/// </summary>
		public const string PropertyName_name = "name";

		/// <summary>
		/// Property name for the path.
		/// </summary>
		public const string PropertyName_path = "path";

		private static readonly string AppPathToken = "$(AppPath)";
		private static readonly int AppPathTokenLength = AppPathToken.Length;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Path where the template DB should look for '.esf' files.
		/// </summary>
		[ConfigurationProperty(PropertyName_includeSubdirectories
			, IsRequired = false
			, DefaultValue = false)]
		public bool IncludeSubdirectories
		{
			get { return (bool)this[PropertyName_includeSubdirectories]; }
			set
			{
				this[PropertyName_includeSubdirectories] = value;
			}
		}

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
				this[PropertyName_path] = (!String.IsNullOrEmpty(value) && value.Contains("$"))
					? ReplacePathTokens(value)
					: value;
			}
		}

		#endregion Properties

		#region Methods

		private string ReplacePathTokens(string value)
		{
			// The only token supported right now is the app-path, which must appear
			// at the beginning of the string...
			if (value.StartsWith(AppPathToken, StringComparison.InvariantCultureIgnoreCase))
			{
				return String.Concat(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
					value.Substring(AppPathTokenLength - 1));
			}
			else return value;
		}

		#endregion Methods
	}
}