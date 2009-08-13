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
		public static readonly string DefaultEventEmitterConfigName = "default";

		/// <summary>
		/// IoC container key for the default IEventEmitter instance.
		/// </summary>
		public static readonly string DefaultEventEmitterContainerKey = "eventEmitter";

		/// <summary>
		/// IoC container key for the default IEventListener instance.
		/// </summary>
		public static readonly string DefaultEventListenerContainerKey = "eventListener";

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
		public const int CMaximumBufferingMemory = 0x40000000; // 1GB

		#endregion Fields

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