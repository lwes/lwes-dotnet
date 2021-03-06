﻿//
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

	using Org.Lwes.Listener;

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
		/// Property name for garbage handling.
		/// </summary>
		public const string PropertyName_garbageHandling = "garbageHandling";

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
		/// Indicates whether the listener listens on a multicast port.
		/// </summary>
		[ConfigurationProperty(PropertyName_garbageHandling
			, IsRequired = false
			, DefaultValue = ListenerGarbageHandling.FailSilently)]
		public ListenerGarbageHandling GarbageHandling
		{
			get
			{
				return (ListenerGarbageHandling)this[PropertyName_garbageHandling];
			}
			set
			{
				this[PropertyName_garbageHandling] = value;
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