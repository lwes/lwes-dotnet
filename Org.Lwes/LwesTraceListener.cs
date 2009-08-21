// 
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT (C) 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
//   original .NET implementation
// 
// LWES.net is free software: you can redistribute it and/or modify
// it under the terms of the Lesser GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// LWES.net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Net;

	using Org.Lwes.DB;
	using Org.Lwes.Emitter;

	/// <summary>
	/// TraceListener implementation that echoes trace events to the 
	/// light weight event system.
	/// </summary>
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
		private static readonly string SupportedAttributes_Multicast = "Multicast";
		private static readonly string SupportedAttributes_MutlicastTimeToLive = "MutlicastTimeToLive";
		private static readonly string SupportedAttributes_Parallel = "Parallel";
		private static readonly string SupportedAttributes_Port = "Port";

		IEventEmitter _emitter;
		string _emitterByName;
		IPAddress _ipAddress;
		bool? _multicast;
		int? _multicastTtl;
		bool? _parallel;
		int? _port;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public LwesTraceListener()
			: base()
		{
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="listenerName"></param>
		public LwesTraceListener(string listenerName)
			: base(listenerName)
		{
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// The IPAddress used by the trace listener.
		/// </summary>
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

		/// <summary>
		/// The name of a configured emitter to use.
		/// </summary>
		public string BindEmitterByName
		{
			get
			{
				return Util.LazyInitialize<string>(ref _emitterByName,
					() => { return GetRawAttributeValue(SupportedAttributes_BindEmitterByName); });
			}
		}

		/// <summary>
		/// Indicates whether the listener is threadsafe (it is).
		/// </summary>
		public override bool IsThreadSafe
		{
			get { return true; }
		}

		/// <summary>
		/// Indicates whether the underlying emitter will use a multicast
		/// strategy when emitting.
		/// </summary>
		public bool Multicast
		{
			get
			{
				if (!_multicast.HasValue)
				{
					string input = GetRawAttributeValue(SupportedAttributes_Multicast);
					_multicast = (String.IsNullOrEmpty(input))
						? default(bool)
						: bool.Parse(input);
				}
				return _multicast.Value;
			}
		}

		/// <summary>
		/// Indicates the multicast time-to-live if underlying emitter
		/// is using multicast.
		/// </summary>
		public int MulticastTimeToLive
		{
			get
			{
				if (!_multicastTtl.HasValue)
				{
					string input = GetRawAttributeValue(SupportedAttributes_MutlicastTimeToLive);
					_multicastTtl = (String.IsNullOrEmpty(input))
						? default(int)
						: int.Parse(input);
				}
				return _multicastTtl.Value;
			}
		}

		/// <summary>
		/// Indicates whether the underlying emitter will use a parallel 
		/// strategy when emitting.
		/// </summary>
		public bool Parallel
		{
			get
			{
				if (!_parallel.HasValue)
				{
					string input = GetRawAttributeValue(SupportedAttributes_Parallel);
					_parallel = (String.IsNullOrEmpty(input))
						? default(bool)
						: bool.Parse(input);
				}
				return _parallel.Value;
			}
		}

		/// <summary>
		/// Indicates the port the underlying emitter will use.
		/// </summary>
		public int Port
		{
			get
			{
				if (!_port.HasValue)
				{
					string input = GetRawAttributeValue(SupportedAttributes_Port);
					_port = (String.IsNullOrEmpty(input))
						? default(int)
						: int.Parse(input);
				}
				return _port.Value;
			}
		}

		IEventEmitter Emitter
		{
			get
			{
				return Util.LazyInitialize<IEventEmitter>(ref _emitter,
					() =>
					{
						if (!String.IsNullOrEmpty(BindEmitterByName))
							return EventEmitter.CreateNamedEmitter(BindEmitterByName);
						else
						{
							if (Multicast)
							{
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
									Parallel);
								return emitter;
							}
							else
							{
								throw new NotImplementedException("UnicastEventEmitter not supported");
							}
						}
					});
			}
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Closes the trace listener.
		/// </summary>
		public override void Close()
		{
			base.Close();
			Util.Dispose(ref _emitter);
		}

		/// <summary>
		/// Writes trace information, a data object and event information to the listener specific output.
		/// </summary>
		/// <param name="eventCache">A System.Diagnostics.TraceEventCache object that contains the current process
		/// ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application
		/// that generated the trace event.</param>
		/// <param name="eventType">One of the System.Diagnostics.TraceEventType values specifying the type of
		/// event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">The trace data to emit.</param>
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

		/// <summary>
		/// Writes trace information, a data object and event information to the listener specific output.
		/// </summary>
		/// <param name="eventCache">A System.Diagnostics.TraceEventCache object that contains the current process
		/// ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application
		/// that generated the trace event.</param>
		/// <param name="eventType">One of the System.Diagnostics.TraceEventType values specifying the type of
		/// event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">An array of objects to emit as data.</param>
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

		/// <summary>
		/// Writes trace information, a data object and event information to the listener specific output.
		/// </summary>
		/// <param name="eventCache">A System.Diagnostics.TraceEventCache object that contains the current process
		/// ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application
		/// that generated the trace event.</param>
		/// <param name="eventType">One of the System.Diagnostics.TraceEventType values specifying the type of
		/// event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
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

		/// <summary>
		/// Writes trace information, a data object and event information to the listener specific output.
		/// </summary>
		/// <param name="eventCache">A System.Diagnostics.TraceEventCache object that contains the current process
		/// ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application
		/// that generated the trace event.</param>
		/// <param name="eventType">One of the System.Diagnostics.TraceEventType values specifying the type of
		/// event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A format string that contains zero or more format items, which correspond
		/// to objects in the args array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
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

		/// <summary>
		/// Writes trace information, a data object and event information to the listener specific output.
		/// </summary>
		/// <param name="eventCache">A System.Diagnostics.TraceEventCache object that contains the current process
		/// ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application
		/// that generated the trace event.</param>
		/// <param name="eventType">One of the System.Diagnostics.TraceEventType values specifying the type of
		/// event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">A message to write.</param>
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

		/// <summary>
		/// Writes trace information, a message, a related activity identity and event
		/// information to the listener specific output.
		/// </summary>
		/// <param name="eventCache">A System.Diagnostics.TraceEventCache object that contains the current process
		/// ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application
		/// that generated the trace event.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">A message to write.</param>
		/// <param name="relatedActivityId">A System.Guid object identifying a related activity.</param>
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

		/// <summary>
		/// Writes the specified message to the listener.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public override void Write(string message)
		{
			Event ev = Emitter.CreateEvent(EventName_TraceMessage)
				.SetValue(AttributeName_Message, message);
			_emitter.Emit(ev);
		}

		/// <summary>
		/// Writes the specified message to the listener.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public override void WriteLine(string message)
		{
			Write(String.Concat(message, Environment.NewLine));
		}

		/// <summary>
		/// Disposes of the listener and the underlying emitter.
		/// </summary>
		/// <param name="disposing">Indicates whether the listener is being disposed.</param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			Util.Dispose(ref _emitter);
		}

		/// <summary>
		/// Gets the custom attributes supported by the trace listener.
		/// </summary>
		/// <returns>A string array naming the custom attributes supported by the trace listener.</returns>
		protected override string[] GetSupportedAttributes()
		{
			return new string[]
			{
				SupportedAttributes_BindEmitterByName,
				SupportedAttributes_Multicast,
				SupportedAttributes_Address,
				SupportedAttributes_Port,
				SupportedAttributes_MutlicastTimeToLive,
				SupportedAttributes_Parallel
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