#region Header

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
// GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//

#endregion Header

namespace Org.Lwes.Config
{
	using System.Configuration;

	/// <summary>
	/// Configuration section for LWES specific diagnostics settings.
	/// </summary>
	public sealed class DiagnosticsConfigurationElement : ConfigurationElement
	{
		#region Fields

		/// <summary>
		/// Default value indicating unknown.
		/// </summary>
		public const string DefaultValue_unknown = "unknown";
		/// <summary>
		/// Property name for component.
		/// </summary>
		public const string PropertyName_component = "component";
		/// <summary>
		/// Property name for defaultTraceSource.
		/// </summary>
		public const string PropertyName_defaultTraceSource = "defaultTraceSource";
		/// <summary>
		/// Property name for environment.
		/// </summary>
		public const string PropertyName_environment = "environment";

		#endregion Fields

		#region Properties

		/// <summary>
		/// Indicates the name of the component that the current application represents.
		/// The meaning of "component" is up to the user but in general indicates a
		/// role that an application performs within a system.
		/// </summary>
		[ConfigurationProperty(DiagnosticsConfigurationElement.PropertyName_component
			, DefaultValue = DiagnosticsConfigurationElement.DefaultValue_unknown)]
		public string Component
		{
			get { return (string)this[PropertyName_component]; }
			set { this[PropertyName_component] = value; }
		}

		/// <summary>
		/// For systems using the LWES Diagnostics utility class, determines the default
		/// trace source.
		/// </summary>
		/// <seealso cref="System.Diagnostics.TraceSource"/>
		[ConfigurationProperty(DiagnosticsConfigurationElement.PropertyName_defaultTraceSource
			, DefaultValue = DiagnosticsConfigurationElement.DefaultValue_unknown)]
		public string DefaultTraceSource
		{
			get { return (string)this[PropertyName_defaultTraceSource]; }
			set { this[PropertyName_defaultTraceSource] = value; }
		}

		/// <summary>
		/// Indicates the name of the environment in which the application is executing.
		/// The meaning of "environment" is up to the user but in general indicates an
		/// environment such as: { dev | test | staging | production }. In cases where
		/// events in one environment can be heard by journalers in another environment
		/// the presence of this value in an event helps with filtering.
		/// </summary>
		[ConfigurationProperty(DiagnosticsConfigurationElement.PropertyName_environment
			, DefaultValue = DiagnosticsConfigurationElement.DefaultValue_unknown)]
		public string Environment
		{
			get { return (string)this[PropertyName_environment]; }
			set { this[PropertyName_environment] = value; }
		}

		#endregion Properties
	}
}