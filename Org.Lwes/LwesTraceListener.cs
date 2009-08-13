namespace Org.Lwes
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Net;

	using Org.Lwes.DB;
	using Org.Lwes.Emitter;

	public class LwesTraceListener : TraceListener
	{
		#region Fields

		private static readonly string AttributeName_Callstack = "Callstack";
		private static readonly string AttributeName_Data = "Data";
		private static readonly string AttributeName_DateTime = "DateTime";
		private static readonly string AttributeName_ID = "ID";
		private static readonly string AttributeName_LogicalOperationStack = "LogicalOperationStack";
		private static readonly string AttributeName_Message = "Message";
		private static readonly string AttributeName_ProcessId = "ProcessId";
		private static readonly string AttributeName_RelatedActivityID = "RelatedActivityID";
		private static readonly string AttributeName_Source = "Source";
		private static readonly string AttributeName_ThreadId = "ThreadId";
		private static readonly string AttributeName_Timestamp = "Timestamp";
		private static readonly string AttributeName_TraceEventType = "TraceEventType";
		private static readonly string EventName_TraceMessage = "TraceMessage";
		private static readonly string SupportedAttributes_Address = "Address";
		private static readonly string SupportedAttributes_BindEmitterByName = "BindEmitterByName";
		private static readonly string SupportedAttributes_EmitterType = "EmitterType";
		private static readonly string SupportedAttributes_MutlicastTimeToLive = "MutlicastTimeToLive";
		private static readonly string SupportedAttributes_ParallelStrategy = "ParallelStrategy";
		private static readonly string SupportedAttributes_Port = "Port";

		IEventEmitter _emitter;
		string _emitterByName;
		string _emitterType;
		IPAddress _ipAddress;
		int _multicastTtl;
		int _port;

		#endregion Fields

		#region Constructors

		public LwesTraceListener()
			: base()
		{
		}

		public LwesTraceListener(string listenerName)
			: base(listenerName)
		{
		}

		#endregion Constructors

		#region Properties

		public IPAddress Address
		{
			get
			{
				return Util.LazyInitialize<IPAddress>(ref _ipAddress,
					() =>
					{
						string input = GetRawAttributeValue(SupportedAttributes_Address);
						return (String.IsNullOrEmpty(input))
							? default(IPAddress)
							: IPAddress.Parse(input);
					});
			}
		}

		public string BindEmitterByName
		{
			get
			{
				return Util.LazyInitialize<string>(ref _emitterByName,
					() => { return GetRawAttributeValue(SupportedAttributes_BindEmitterByName); });
			}
		}

		public string EmitterType
		{
			get
			{
				return Util.LazyInitialize<string>(ref _emitterType,
					() => { return GetRawAttributeValue(SupportedAttributes_EmitterType); });
			}
		}

		public override bool IsThreadSafe
		{
			get { return true; }
		}

		public int MulticastTimeToLive
		{
			get
			{
				if (_multicastTtl == default(int))
				{
					string input = GetRawAttributeValue(SupportedAttributes_MutlicastTimeToLive);
					_multicastTtl = (String.IsNullOrEmpty(input))
						? default(int)
						: int.Parse(input);
				}
				return _multicastTtl;
			}
		}

		public int Port
		{
			get
			{
				if (_port == default(int))
				{
					string input = GetRawAttributeValue(SupportedAttributes_Port);
					_port = (String.IsNullOrEmpty(input))
						? default(int)
						: int.Parse(input);
				}
				return _port;
			}
		}

		IEventEmitter Emitter
		{
			get
			{
				return Util.LazyInitialize<IEventEmitter>(ref _emitter,
					() =>
					{
						if (BindEmitterByName != null)
							return EventEmitter.CreateNamedEmitter(BindEmitterByName);
						else
						{
							if (String.Equals(EmitterType, "unicast", StringComparison.InvariantCultureIgnoreCase))
							{
								return null;
							}
							else
							{
								// Default to the mutlicast emitter
								MulticastEventEmitter emitter = new MulticastEventEmitter();
								IPAddress addy = Address ?? Constants.DefaultMulticastAddress;
								int port = (Port == 0) ? Constants.CDefaultMulticastPort : Port;
								int ttl = (MulticastTimeToLive == 0) ? Constants.CDefaultMulticastTtl : MulticastTimeToLive;
								emitter.Initialize(SupportedEncoding.Default,
			#if DEBUG
			 true,
			#else
									false,
			#endif
			 EventTemplateDB.CreateDefault(),
									addy,
									port,
									ttl,
									true);
								return emitter;
							}
						}
					});
			}
		}

		#endregion Properties

		#region Methods

		public override void Close()
		{
			base.Close();
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			Event ev = Emitter.CreateEvent(EventName_TraceMessage)
				.SetValue(AttributeName_ProcessId, eventCache.ProcessId)
				.SetValue(AttributeName_ThreadId, eventCache.ThreadId)
				.SetValue(AttributeName_DateTime, eventCache.DateTime.ToString("u"))
				.SetValue(AttributeName_Timestamp, eventCache.Timestamp);
			if (eventType <= TraceEventType.Warning)
			{
				ev = ev.SetValue(AttributeName_Callstack, Convert.ToString(eventCache.Callstack))
				.SetValue(AttributeName_LogicalOperationStack, Convert.ToString(eventCache.LogicalOperationStack));
			}
			ev = ev.SetValue(AttributeName_Source, source)
				.SetValue(AttributeName_TraceEventType, eventType.ToString())
				.SetValue(AttributeName_ID, id)
				.SetValue(AttributeName_Data, Convert.ToString(data));
			_emitter.Emit(ev);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			Event ev = Emitter.CreateEvent(EventName_TraceMessage)
				.SetValue(AttributeName_ProcessId, eventCache.ProcessId)
				.SetValue(AttributeName_ThreadId, eventCache.ThreadId)
				.SetValue(AttributeName_DateTime, eventCache.DateTime.ToString("u"))
				.SetValue(AttributeName_Timestamp, eventCache.Timestamp);
			if (eventType <= TraceEventType.Warning)
			{
				ev = ev.SetValue(AttributeName_Callstack, Convert.ToString(eventCache.Callstack))
				.SetValue(AttributeName_LogicalOperationStack, Convert.ToString(eventCache.LogicalOperationStack));
			}
			ev = ev.SetValue(AttributeName_Source, source)
				.SetValue(AttributeName_TraceEventType, eventType.ToString())
				.SetValue(AttributeName_ID, id);
			for (int i = 0; i < data.Length; i++)
			{
				ev = ev.SetValue(String.Concat(AttributeName_Data, "_", i), Convert.ToString(data[i]));
			}
			_emitter.Emit(ev);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
		{
			Event ev = Emitter.CreateEvent(EventName_TraceMessage)
				.SetValue(AttributeName_ProcessId, eventCache.ProcessId)
				.SetValue(AttributeName_ThreadId, eventCache.ThreadId)
				.SetValue(AttributeName_DateTime, eventCache.DateTime.ToString("u"))
				.SetValue(AttributeName_Timestamp, eventCache.Timestamp);
			if (eventType <= TraceEventType.Warning)
			{
				ev = ev.SetValue(AttributeName_Callstack, Convert.ToString(eventCache.Callstack))
				.SetValue(AttributeName_LogicalOperationStack, Convert.ToString(eventCache.LogicalOperationStack));
			}
			ev = ev.SetValue(AttributeName_Source, source)
				.SetValue(AttributeName_TraceEventType, eventType.ToString())
				.SetValue(AttributeName_ID, id);
			_emitter.Emit(ev);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			Event ev = Emitter.CreateEvent(EventName_TraceMessage)
				.SetValue(AttributeName_ProcessId, eventCache.ProcessId)
				.SetValue(AttributeName_ThreadId, eventCache.ThreadId)
				.SetValue(AttributeName_DateTime, eventCache.DateTime.ToString("u"))
				.SetValue(AttributeName_Timestamp, eventCache.Timestamp);
			if (eventType <= TraceEventType.Warning)
			{
				ev = ev.SetValue(AttributeName_Callstack, Convert.ToString(eventCache.Callstack))
				.SetValue(AttributeName_LogicalOperationStack, Convert.ToString(eventCache.LogicalOperationStack));
			}
			ev = ev.SetValue(AttributeName_Source, source)
				.SetValue(AttributeName_TraceEventType, eventType.ToString())
				.SetValue(AttributeName_ID, id)
				.SetValue(AttributeName_Message, String.Format(format, args));
			_emitter.Emit(ev);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			Event ev = Emitter.CreateEvent(EventName_TraceMessage)
				.SetValue(AttributeName_ProcessId, eventCache.ProcessId)
				.SetValue(AttributeName_ThreadId, eventCache.ThreadId)
				.SetValue(AttributeName_DateTime, eventCache.DateTime.ToString("u"))
				.SetValue(AttributeName_Timestamp, eventCache.Timestamp);
			if (eventType <= TraceEventType.Warning)
			{
				ev = ev.SetValue(AttributeName_Callstack, Convert.ToString(eventCache.Callstack))
				.SetValue(AttributeName_LogicalOperationStack, Convert.ToString(eventCache.LogicalOperationStack));
			}
			ev = ev.SetValue(AttributeName_Source, source)
				.SetValue(AttributeName_TraceEventType, eventType.ToString())
				.SetValue(AttributeName_ID, id)
				.SetValue(AttributeName_Message, message);
			_emitter.Emit(ev);
		}

		public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
		{
			Event ev = Emitter.CreateEvent(EventName_TraceMessage)
				.SetValue(AttributeName_ProcessId, eventCache.ProcessId)
				.SetValue(AttributeName_ThreadId, eventCache.ThreadId)
				.SetValue(AttributeName_DateTime, eventCache.DateTime.ToString("u"))
				.SetValue(AttributeName_Timestamp, eventCache.Timestamp)
				.SetValue(AttributeName_Source, source)
				.SetValue(AttributeName_ID, id)
				.SetValue(AttributeName_Message, message)
				.SetValue(AttributeName_RelatedActivityID, relatedActivityId.ToString("B"));
			_emitter.Emit(ev);
		}

		public override void Write(string message)
		{
			Event ev = Emitter.CreateEvent(EventName_TraceMessage)
				.SetValue(AttributeName_Message, message);
			_emitter.Emit(ev);
		}

		public override void WriteLine(string message)
		{
			Write(String.Concat(message, Environment.NewLine));
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			Util.Dispose(ref _emitter);
		}

		protected override string[] GetSupportedAttributes()
		{
			return new string[]
			{
				SupportedAttributes_BindEmitterByName,
				SupportedAttributes_EmitterType,
				SupportedAttributes_Address,
				SupportedAttributes_Port,
				SupportedAttributes_MutlicastTimeToLive,
				SupportedAttributes_ParallelStrategy
			};
		}

		private string GetRawAttributeValue(string attr)
		{
			foreach (DictionaryEntry de in this.Attributes)
				if (String.Equals(de.Key.ToString(), attr, StringComparison.InvariantCultureIgnoreCase))
					return de.Value.ToString();
			return null;
		}

		#endregion Methods
	}
}