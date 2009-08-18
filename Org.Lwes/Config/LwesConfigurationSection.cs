namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Text;

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
		/// Collection of configured emitters.
		/// </summary>
		[ConfigurationProperty(PropertyName_emitters, IsDefaultCollection = false
			, IsRequired = false)]
		public EmitterConfigurationElementCollection Emitters
		{
			get { return (EmitterConfigurationElementCollection)this[PropertyName_emitters]; }
		}

		/// <summary>
		/// Collection of configured listeners.
		/// </summary>
		[ConfigurationProperty(PropertyName_listeners, IsDefaultCollection = false
			, IsRequired = false)]
		public ListenerConfigurationElementCollection Listeners
		{
			get { return (ListenerConfigurationElementCollection)this[PropertyName_listeners]; }
		}

		/// <summary>
		/// Indicates the maximum amount of memory used for buffering events.
		/// Note that this setting will throttle event IO if the limit is 
		/// reached.
		/// </summary>
		[ConfigurationProperty(PropertyName_maximumBufferMemory
			, IsRequired = false
			, DefaultValue = Constants.CMaximumBufferingMemory)]
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
		}

		#endregion Properties
	}
}