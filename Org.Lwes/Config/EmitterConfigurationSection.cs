namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Net;
	using System.Text;

	public class EmitterConfigurationSection : ConfigurationSection
	{
		#region Fields

		public const string PropertyName_address = "address";
		public const string PropertyName_encoding = "encoding";
		public const string PropertyName_multicastTimeToLive = "multicastTimeToLive";
		public const string PropertyName_name = "name";
		public const string PropertyName_parallel = "parallel";
		public const string PropertyName_port = "port";
		public const string PropertyName_useMulticast = "multicast";

		#endregion Fields

		#region Properties

		[ConfigurationProperty(PropertyName_address
			, IsRequired = false
			, DefaultValue = Constants.CDefaultMulticastAddressString)]
		public string AddressString
		{
			get { return (string)this[PropertyName_address]; }
			set
			{
				this[PropertyName_address] = value;
			}
		}

		[ConfigurationProperty(PropertyName_encoding
			, IsRequired = false
			, DefaultValue = SupportedEncoding.Default)]
		public SupportedEncoding Encoding
		{
			get
			{
				return (SupportedEncoding)this[PropertyName_encoding];
			}
			set
			{
				this[PropertyName_encoding] = value;
			}
		}

		[ConfigurationProperty(PropertyName_multicastTimeToLive
			, IsRequired = false
			, DefaultValue = Constants.CDefaultMulticastTtl)]
		public int MulticastTimeToLive
		{
			get
			{
				return (int)this[PropertyName_multicastTimeToLive];
			}
			set
			{
				this[PropertyName_multicastTimeToLive] = value;
			}
		}

		[ConfigurationProperty(PropertyName_name, IsRequired = true)]
		public string Name
		{
			get { return (string)this[PropertyName_name]; }
			set {	this[PropertyName_name] = value; }
		}

		[ConfigurationProperty(PropertyName_port
			, IsRequired = false
			, DefaultValue = Constants.CDefaultMulticastPort)]
		public int Port
		{
			get
			{
				return (int)this[PropertyName_port];
			}
			set
			{
				this[PropertyName_port] = value;
			}
		}

		[ConfigurationProperty(PropertyName_useMulticast
			, IsRequired = false
			, DefaultValue = true)]
		public bool UseMulticast
		{
			get
			{
				return (bool)this[PropertyName_useMulticast];
			}
			set
			{
				this[PropertyName_useMulticast] = value;
			}
		}

		[ConfigurationProperty(PropertyName_parallel
			, IsRequired = false
			, DefaultValue = true)]
		public bool UseParallelEmit
		{
			get
			{
				return (bool)this[PropertyName_parallel];
			}
			set
			{
				this[PropertyName_parallel] = value;
			}
		}

		#endregion Properties
	}
}