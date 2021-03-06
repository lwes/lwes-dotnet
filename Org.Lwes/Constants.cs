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
namespace Org.Lwes
{
	using System;
	using System.Net;
	using System.Text;

	using Org.Lwes.ESF;
	using Org.Lwes.Properties;

	/// <summary>
	/// Utility class containing constants used by LWES
	/// </summary>
	public static class Constants
	{
		#region Fields

		/// <summary>
		/// Default encoding for character data.
		/// </summary>
		public static readonly Encoding DefaultEncoding = Encoding.UTF8;

		/// <summary>
		/// Name identifying the default event emitter in the configuration file.
		/// </summary>
		public const string CDefaultEventEmitterConfigName = "default";

		/// <summary>
		/// IoC container key for the default IEventEmitter instance.
		/// </summary>
		public static readonly string DefaultEventEmitterContainerKey = "eventEmitter";

		/// <summary>
		/// Name identifying the default event listener in the configuration file.
		/// </summary>
		public static readonly string DefaultEventListenerConfigName = "default";

		/// <summary>
		/// IoC container key for the default IEventListener instance.
		/// </summary>
		public static readonly string DefaultEventListenerContainerKey = "eventListener";

		/// <summary>
		/// Name identifying the default event template DB in the configuration file.
		/// </summary>
		public static readonly string DefaultEventTemplateDBConfigName = "default";

		/// <summary>
		/// IoC container key for the default IEventTemplateDB instance.
		/// </summary>
		public static readonly string DefaultEventTemplateDBContainerKey = "templateDB";

		/// <summary>
		/// Default address used for multicast listening.
		/// </summary>
		public static readonly IPAddress DefaultMulticastAddress = IPAddress.Parse(CDefaultMulticastAddressString);

		/// <summary>
		/// Encoding for ISO-8859-1
		/// </summary>
		public static readonly Encoding ISO8859_1Encoding = Encoding.GetEncoding("ISO-8859-1");

		/// <summary>
		/// Represents the difference between the epoc used by LWES and the .NET DateTime ticks.
		/// </summary>
		public static readonly long LwesEpocOffsetTicks = new DateTime(1970, 1, 1).Ticks;

		/// <summary>
		/// Name of the event template inherited by other events.
		/// </summary>	
		public static readonly EventTemplate MetaEventInfo;

		/// <summary>
		/// Default length of buffers.
		/// </summary>
		public const int CAllocationBufferLength = 65535;

		/// <summary>
		/// Default multicast address (as a string)
		/// </summary>
		public const string CDefaultMulticastAddressString = "224.0.0.69";

		/// <summary>
		/// Default port used for multicast listening.
		/// </summary>
		public const int CDefaultMulticastPort = 9191;

		/// <summary>
		/// Default time-to-live for multicast emitting.
		/// </summary>
		public const int CDefaultMulticastTtl = 31;

		/// <summary>
		/// Identifies the default encoding used if encoding is not specified.
		/// </summary>
		public const SupportedEncoding CDefaultSupportedEncoding = SupportedEncoding.UTF_8;

		/// <summary>
		/// Maximum memory used for buffering incoming events.
		/// </summary>
		public const int CMaximumBufferMemory = 0x20000000; // .5GB

		#endregion Fields

#if DEBUG

		/// <summary>
		/// Default value indicating whether validation is performed
		/// </summary>
		public static readonly bool DefaultPerformValidation = true;

#else

		/// <summary>
		/// Default value indicating whether validation is performed
		/// </summary>
		public static readonly bool DefaultPerformValidation = false;

#endif

		#region Constructors

		static Constants()
		{
			MetaEventInfo = new EventTemplate(true, "MetaInfoEvent")
				.AppendAttributes(MetaEventInfoAttributes.Encoding)
				.AppendAttributes(MetaEventInfoAttributes.SenderIP)
				.AppendAttributes(MetaEventInfoAttributes.SenderPort)
				.AppendAttributes(MetaEventInfoAttributes.ReceiptTime)
				.AppendAttributes(MetaEventInfoAttributes.SiteID);
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// Converts a DateTime to LWES ticks.
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public static Int64 DateTimeToLwesTimeTicks(DateTime time)
		{
			return (time.Ticks - LwesEpocOffsetTicks);
		}

		/// <summary>
		/// Identifies the encoding's value on the wire.
		/// </summary>
		/// <param name="enc">An Encoding instance to identify</param>
		/// <returns>Either ISO_8859_1 (0) or UTF_8 (1)</returns>
		/// <exception cref="ArgumentOutOfRangeException">thrown if the encoding is not recognized</exception>
		public static SupportedEncoding IdentifyEncoding(Encoding enc)
		{
			if (enc == ISO8859_1Encoding) return SupportedEncoding.ISO_8859_1;
			if (enc == DefaultEncoding) return SupportedEncoding.UTF_8;
			throw new ArgumentOutOfRangeException(String.Format(Resources.Error_UnrecognizedEncoding, enc.EncodingName));
		}

		/// <summary>
		/// Converts LWES ticks to DateTime.
		/// </summary>
		/// <param name="lwesTimeTicks"></param>
		/// <returns></returns>
		public static DateTime LwesTimeTicksToDateTime(long lwesTimeTicks)
		{
			return new DateTime(lwesTimeTicks + LwesEpocOffsetTicks);
		}

		internal static void CheckEncoding(short enc)
		{
			if (!Enum.IsDefined(typeof(SupportedEncoding), enc))
				throw new InvalidOperationException("Encoding not supported");
		}

		internal static Encoding GetEncoding(short enc)
		{
			CheckEncoding(enc);
			if (enc == (short)SupportedEncoding.ISO_8859_1) return ISO8859_1Encoding;
			else return DefaultEncoding;
		}

		#endregion Methods

		#region Nested Types

		/// <summary>
		/// Contains names of event attributes that may be inherited by any event.
		/// </summary>
		public static class MetaEventInfoAttributes
		{
			#region Fields

			/// <summary>
			/// Name of the attribute that identifies the encoding used for an event. (set by emitters)
			/// </summary>
			public static readonly AttributeTemplate Encoding = new AttributeTemplate(TypeToken.INT16, "enc", 0);

			/// <summary>
			/// Name of the attribute that reflects the time an event was received, 
			/// in milliseconds since epoch. (set by journallers and listeners)
			/// </summary>
			public static readonly AttributeTemplate ReceiptTime = new AttributeTemplate(TypeToken.UINT64, "ReceiptTime", 1);

			/// <summary>
			/// Name of the attribute that reflects the sender's IP address.
			/// (set by journallers and listeners)
			/// </summary>
			public static readonly AttributeTemplate SenderIP = new AttributeTemplate(TypeToken.IP_ADDR, "SenderIP", 2);

			/// <summary>
			/// Name of the attribute that reflects the sender's port.
			/// (set by journallers and listeners)
			/// </summary>
			public static readonly AttributeTemplate SenderPort = new AttributeTemplate(TypeToken.INT32, "SenderPort", 3);

			/// <summary>
			/// Name of the attribute that reflects event's source site ID.
			/// (set by emitters)
			/// </summary>
			public static readonly AttributeTemplate SiteID = new AttributeTemplate(TypeToken.UINT32,"SiteID", 4);

			#endregion Fields
		}

		#endregion Nested Types
	}
}