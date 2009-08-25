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
	using System.Configuration;

	/// <summary>
	/// Configuration for the light weight event system.
	/// </summary>
	public class LwesConfigurationSection : ConfigurationSection
	{
		#region Fields

		/// <summary>
		/// Property name for buffer allocation length.
		/// </summary>
		public const string PropertyName_bufferAllocationLength = "bufferAllocationLength";

		/// <summary>
		/// Property name for the diagnostics section.
		/// </summary>
		public const string PropertyName_diagnostics = "diagnostics";

		/// <summary>
		/// Property name for the emitters section.
		/// </summary>
		public const string PropertyName_emitters = "emitters";

		/// <summary>
		/// Property name ofr the listeners section.
		/// </summary>
		public const string PropertyName_listeners = "listeners";

		/// <summary>
		/// Property name for maximum buffer memory.
		/// </summary>
		public const string PropertyName_maximumBufferMemory = "maximumBufferMemory";

		/// <summary>
		/// Property name for the event template DBs section.
		/// </summary>
		public const string PropertyName_templateDBs = "templateDBs";

		/// <summary>
		/// Section name used for configuring the Light Weight Event System.
		/// </summary>
		public const string SectionName = "lwes";

		#endregion Fields

		#region Properties

		/// <summary>
		/// Indicates the length of buffers used for event buffering.
		/// </summary>
		[ConfigurationProperty(PropertyName_bufferAllocationLength
	, IsRequired = false
	, DefaultValue = Constants.CAllocationBufferLength)]
		public int BufferAllocationLength
		{
			get { return (int)this[PropertyName_bufferAllocationLength]; }
			set { this[PropertyName_bufferAllocationLength] = value; }
		}

		/// <summary>
		/// Configuration section containing diagnostics settings.
		/// </summary>
		[ConfigurationProperty(PropertyName_diagnostics, IsRequired = false)]
		public DiagnosticsConfigurationElement Diagnostics
		{
			get { return (DiagnosticsConfigurationElement)this[PropertyName_diagnostics]; }
			private set { this[PropertyName_diagnostics] = value; }
		}

		/// <summary>
		/// Collection of configured emitters.
		/// </summary>
		[ConfigurationProperty(PropertyName_emitters, IsDefaultCollection = false
			, IsRequired = false)]
		public EmitterConfigurationElementCollection Emitters
		{
			get { return (EmitterConfigurationElementCollection)this[PropertyName_emitters]; }
			private set { this[PropertyName_emitters] = value; }
		}

		/// <summary>
		/// Collection of configured listeners.
		/// </summary>
		[ConfigurationProperty(PropertyName_listeners, IsDefaultCollection = false
			, IsRequired = false)]
		public ListenerConfigurationElementCollection Listeners
		{
			get { return (ListenerConfigurationElementCollection)this[PropertyName_listeners]; }
			private set { this[PropertyName_listeners] = value; }
		}

		/// <summary>
		/// Indicates the maximum amount of memory used for buffering events.
		/// Note that this setting will throttle event IO if the limit is 
		/// reached.
		/// </summary>
		[ConfigurationProperty(PropertyName_maximumBufferMemory
			, IsRequired = false
			, DefaultValue = Constants.CMaximumBufferMemory)]
		public int MaximumBufferMemory
		{
			get { return (int)this[PropertyName_maximumBufferMemory]; }
			set { this[PropertyName_maximumBufferMemory] = value; }
		}

		/// <summary>
		/// Collection of configured template DBs.
		/// </summary>
		[ConfigurationProperty(PropertyName_templateDBs, IsDefaultCollection = false
	, IsRequired = false)]
		public TemplateDBConfigurationElementCollection TemplateDBs
		{
			get { return (TemplateDBConfigurationElementCollection)this[PropertyName_templateDBs]; }
			private set { this[PropertyName_templateDBs] = value; }
		}

		internal static LwesConfigurationSection Current
		{
			get
			{
				LwesConfigurationSection config = ConfigurationManager.GetSection(
					LwesConfigurationSection.SectionName) as LwesConfigurationSection;
				if (config == null)
				{
					config = new LwesConfigurationSection();
					//config.Diagnostics = new DiagnosticsConfigurationElement();
					//config.Emitters = new EmitterConfigurationElementCollection();
					//config.TemplateDBs = new TemplateDBConfigurationElementCollection();
					//config.Listeners = new ListenerConfigurationElementCollection();
				}
				return config;
			}
		}

		#endregion Properties
	}
}