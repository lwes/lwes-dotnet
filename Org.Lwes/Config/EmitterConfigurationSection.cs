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
	/// Configuration section for an emitter.
	/// </summary>
	public class EmitterConfigurationSection : ConfigurationSection
	{
		#region Fields

		/// <summary>
		/// Property name for the address.
		/// </summary>
		public const string PropertyName_address = "address";

		/// <summary>
		/// Property name for the encoding used by an emitter.
		/// </summary>
		public const string PropertyName_encoding = "encoding";

		/// <summary>
		/// Property name for the multicast time to live used by an emitter.
		/// </summary>
		public const string PropertyName_multicastTimeToLive = "multicastTimeToLive";

		/// <summary>
		/// Property name for the name of an emitter.
		/// </summary>
		public const string PropertyName_name = "name";

		/// <summary>
		/// Property name indicating whether an emitter uses a parallel strategy.
		/// </summary>
		public const string PropertyName_parallel = "parallel";

		/// <summary>
		/// Property name for the port used by an emitter.
		/// </summary>
		public const string PropertyName_port = "port";

		/// <summary>
		/// Property name indicating whether an emitter uses multicast.
		/// </summary>
		public const string PropertyName_useMulticast = "multicast";

		#endregion Fields

		#region Properties

		/// <summary>
		/// The emitter's IP address (as a string)
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
		/// The encoding used by the emitter.
		/// </summary>
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

		/// <summary>
		/// The multicast time-to-live if the emitter is using multicast.
		/// </summary>
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

		/// <summary>
		/// The emitter's name.
		/// </summary>
		[ConfigurationProperty(PropertyName_name, IsRequired = true)]
		public string Name
		{
			get { return (string)this[PropertyName_name]; }
			set {	this[PropertyName_name] = value; }
		}

		/// <summary>
		/// The emitter's port.
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
		/// Indicates whether the emitter will multicast events.
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
		/// Indicates whether the emitter will use a parallel strategy when
		/// emitting.
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