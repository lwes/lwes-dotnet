namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Net;
	using System.Text;

	/// <summary>
	/// Configuration section for a listener.
	/// </summary>
	public class ListenerConfigurationSection : ConfigurationSection
	{
		#region Fields

		/// <summary>
		/// Property name for the address.
		/// </summary>
		public const string PropertyName_address = "address";

		/// <summary>
		/// Property name for the name.
		/// </summary>
		public const string PropertyName_name = "name";

		/// <summary>
		/// Property name for whether the listener uses a parallel strategy.
		/// </summary>
		public const string PropertyName_parallel = "parallel";

		/// <summary>
		/// Property name for the port.
		/// </summary>
		public const string PropertyName_port = "port";

		/// <summary>
		/// Property name for whether the listener uses multicast.
		/// </summary>
		public const string PropertyName_useMulticast = "multicast";

		#endregion Fields

		#region Properties

		/// <summary>
		/// The address of the configured listener as a string.
		/// </summary>
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

		/// <summary>
		/// The name of the configured listner.
		/// </summary>
		[ConfigurationProperty(PropertyName_name, IsRequired = true)]
		public string Name
		{
			get { return (string)this[PropertyName_name]; }
			set {	this[PropertyName_name] = value; }
		}

		/// <summary>
		/// The port that the listener listens on.
		/// </summary>
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

		/// <summary>
		/// Indicates whether the listener listens on a multicast port.
		/// </summary>
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

		/// <summary>
		/// Indicates whether the listener uses a parallel strategy.
		/// </summary>
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