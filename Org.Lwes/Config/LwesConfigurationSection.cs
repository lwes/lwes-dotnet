namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Text;

	public class LwesConfigurationSection : ConfigurationSection
	{
		#region Fields

		public const string PropertyName_bufferAllocationLength = "bufferAllocationLength";
		public const string PropertyName_emitters = "emitters";
		public const string PropertyName_listeners = "listeners";
		public const string PropertyName_maximumBufferingMemory = "maximumBufferingMemory";
		public const string SectionName = "lwes";

		#endregion Fields

		#region Properties

		[ConfigurationProperty(PropertyName_bufferAllocationLength
	, IsRequired = false
	, DefaultValue = Constants.CAllocationBufferLength)]
		public int BufferAllocationLength
		{
			get { return (int)this[PropertyName_bufferAllocationLength]; }
			set { this[PropertyName_bufferAllocationLength] = value; }
		}

		[ConfigurationProperty(PropertyName_emitters, IsDefaultCollection = false
			, IsRequired = false)]
		public EmitterConfigurationElementCollection Emitters
		{
			get { return (EmitterConfigurationElementCollection)this[PropertyName_emitters]; }
		}

		[ConfigurationProperty(PropertyName_listeners, IsDefaultCollection = false
			, IsRequired = false)]
		public ListenerConfigurationElementCollection Listeners
		{
			get { return (ListenerConfigurationElementCollection)this[PropertyName_listeners]; }
		}

		[ConfigurationProperty(PropertyName_maximumBufferingMemory 
			, IsRequired = false
			, DefaultValue = Constants.CMaximumBufferingMemory)]
		public int MaximumBufferingMemory
		{
			get { return (int)this[PropertyName_maximumBufferingMemory]; }
			set { this[PropertyName_maximumBufferingMemory] = value; }
		}

		#endregion Properties
	}
}