#region Header

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

#endregion Header

namespace Org.Lwes
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Security;

	using Org.Lwes.Config;
	using Org.Lwes.Trace;
	using Org.Lwes.Trace.Filters;

	/// <summary>
	/// Diagnostics utility.
	/// </summary>
	public static class Diagnostics
	{
		#region Fields

		/// <summary>
		/// Trace ID used when none was given.
		/// </summary>
		public static int DefaultTraceEventID = 0;

		static Guid __traceProcessGuid = Guid.NewGuid();
		static String __traceEnvironment;
		static String __traceComponent;
		static String __traceApplication;
		static String __traceDefaultSource;
		static int __traceOsProcess = -1;
		private static Object __lock = new Object();
		private static Dictionary<string, ITraceSourceFilter> __filters = new Dictionary<string, ITraceSourceFilter>();

		#endregion Fields

		#region Constructors

		static Diagnostics()
		{
			LwesConfigurationSection config = LwesConfigurationSection.Current;
			DiagnosticsConfigurationElement diag = config.Diagnostics ?? new DiagnosticsConfigurationElement();

			__traceEnvironment = diag.Environment;
			__traceComponent = diag.Component;
			__traceDefaultSource = diag.DefaultTraceSource;
			__traceApplication = AppDomain.CurrentDomain.SetupInformation.ApplicationName;
			try
			{
				__traceOsProcess = Process.GetCurrentProcess().Id;
			}
			catch (SecurityException)
			{ // Not run with fulltrust, can't get the process Id.
			}
		}

		#endregion Constructors

		#region Methods

		public static void TraceData<T>(this T source, TraceEventType eventType, object data)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceData(eventType, DefaultTraceEventID, data);
		}

		public static void TraceData<T>(this T source, TraceEventType eventType, Func<object[]> dataGenerator)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceData(eventType, DefaultTraceEventID, dataGenerator());
		}

		public static void TraceData<T>(this T source, TraceEventType eventType, int id, object data)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceData(eventType, id, data);
		}

		public static void TraceData<T>(this T source, TraceEventType eventType, params object[] data)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceData(eventType, DefaultTraceEventID, data);
		}

		public static void TraceData<T>(this T source, TraceEventType eventType, int id, params object[] data)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceData(eventType, id, data);
		}

		public static void TraceData(Type sourceType, TraceEventType eventType, object data)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceData(eventType, DefaultTraceEventID, data);
			}
		}

		public static void TraceData(Type sourceType, TraceEventType eventType, Func<object[]> dataGenerator)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceData(eventType, DefaultTraceEventID, dataGenerator());
			}
		}

		public static void TraceData(Type sourceType, TraceEventType eventType, int id, object data)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceData(eventType, id, data);
			}
		}

		public static void TraceData(Type sourceType, TraceEventType eventType, params object[] data)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceData(eventType, DefaultTraceEventID, data);
			}
		}

		public static void TraceData(Type sourceType, TraceEventType eventType, int id, params object[] data)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceData(eventType, id, data);
			}
		}

		public static void TraceError<T>(this T source, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Error, DefaultTraceEventID, message);
		}

		public static void TraceError<T>(this T source, int id, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Error, id, message);
		}

		public static void TraceError<T>(this T source, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Error, DefaultTraceEventID, format, args);
		}

		public static void TraceError<T>(this T source, int id, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Error, id, format, args);
		}

		public static void TraceError(Type sourceType, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Error))
			{
				f.TraceEvent(TraceEventType.Error, DefaultTraceEventID, message);
			}
		}

		public static void TraceError(Type sourceType, int id, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Error))
			{
				f.TraceEvent(TraceEventType.Error, id, message);
			}
		}

		public static void TraceError(Type sourceType, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Error))
			{
				f.TraceEvent(TraceEventType.Error, DefaultTraceEventID, format, args);
			}
		}

		public static void TraceError(Type sourceType, int id, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Error))
			{
				f.TraceEvent(TraceEventType.Error, id, format, args);
			}
		}

		public static void TraceEvent<T>(this T source, TraceEventType eventType, int id)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(eventType, id);
		}

		public static void TraceEvent<T>(this T source, TraceEventType eventType, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(eventType, DefaultTraceEventID, message);
		}

		public static void TraceEvent<T>(this T source, TraceEventType eventType, int id, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(eventType, id, message);
		}

		public static void TraceEvent<T>(this T source, TraceEventType eventType, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(eventType, DefaultTraceEventID, format, args);
		}

		public static void TraceEvent<T>(this T source, TraceEventType eventType, int id, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(eventType, id, format, args);
		}

		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, id);
			}
		}

		public static void TraceEvent(Type sourceType, TraceEventType eventType, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, DefaultTraceEventID, message);
			}
		}

		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, id, message);
			}
		}

		public static void TraceEvent(Type sourceType, TraceEventType eventType, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, DefaultTraceEventID, format, args);
			}
		}

		public static void TraceEvent(Type sourceType, TraceEventType eventType, int id, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(eventType))
			{
				f.TraceEvent(eventType, id, format, args);
			}
		}

		public static void TraceInformation<T>(this T source, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Information, DefaultTraceEventID, message);
		}

		public static void TraceInformation<T>(this T source, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Information, DefaultTraceEventID, format, args);
		}

		public static void TraceInformation(Type sourceType, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Information))
			{
				f.TraceEvent(TraceEventType.Information, DefaultTraceEventID, message);
			}
		}

		public static void TraceInformation(Type sourceType, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Information))
			{
				f.TraceEvent(TraceEventType.Information, DefaultTraceEventID, format, args);
			}
		}

		public static void TraceTransfer<T>(this T source, string message, Guid relatedActivityId)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceTransfer(DefaultTraceEventID, message, relatedActivityId);
		}

		public static void TraceTransfer<T>(this T source, int id, string message, Guid relatedActivityId)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceTransfer(id, message, relatedActivityId);
		}

		public static void TraceTransfer(Type sourceType, string message, Guid relatedActivityId)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Transfer))
			{
				f.TraceTransfer(DefaultTraceEventID, message, relatedActivityId);
			}
		}

		public static void TraceTransfer(Type sourceType, int id, string message, Guid relatedActivityId)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Transfer))
			{
				f.TraceTransfer(id, message, relatedActivityId);
			}
		}

		public static void TraceVerbose<T>(this T source, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, message);
		}

		public static void TraceVerbose<T>(this T source, int id, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Verbose, id, message);
		}

		public static void TraceVerbose<T>(this T source, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, format, args);
		}

		public static void TraceVerbose<T>(this T source, int id, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Verbose, id, format, args);
		}

		public static void TraceVerbose(Type sourceType, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Verbose))
			{
				f.TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, message);
			}
		}

		public static void TraceVerbose(Type sourceType, int id, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Verbose))
			{
				f.TraceEvent(TraceEventType.Verbose, id, message);
			}
		}

		public static void TraceVerbose(Type sourceType, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Verbose))
			{
				f.TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, format, args);
			}
		}

		public static void TraceVerbose(Type sourceType, int id, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Verbose))
			{
				f.TraceEvent(TraceEventType.Verbose, id, format, args);
			}
		}

		public static void TraceWarning<T>(this T source, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Warning, DefaultTraceEventID, message);
		}

		public static void TraceWarning<T>(this T source, int id, string message)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Warning, id, message);
		}

		public static void TraceWarning<T>(this T source, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Warning, DefaultTraceEventID, format, args);
		}

		public static void TraceWarning<T>(this T source, int id, string format, params object[] args)
			where T : ITraceable
		{
			TraceAdapter<T>.Filter.TraceEvent(TraceEventType.Warning, id, format, args);
		}

		public static void TraceWarning(Type sourceType, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Warning))
			{
				f.TraceEvent(TraceEventType.Warning, DefaultTraceEventID, message);
			}
		}

		public static void TraceWarning(Type sourceType, int id, string message)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Warning))
			{
				f.TraceEvent(TraceEventType.Warning, id, message);
			}
		}

		public static void TraceWarning(Type sourceType, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Warning))
			{
				f.TraceEvent(TraceEventType.Warning, DefaultTraceEventID, format, args);
			}
		}

		public static void TraceWarning(Type sourceType, int id, string format, params object[] args)
		{
			if (sourceType == null) throw new ArgumentNullException("sourceType");

			ITraceSourceFilter f = AcquireSourceFilter(sourceType);
			if (f.ShouldTrace(TraceEventType.Warning))
			{
				f.TraceEvent(TraceEventType.Warning, id, format, args);
			}
		}

		internal static ITraceSourceFilter AcquireSourceFilter(Type t)
		{
			if (t == null) throw new ArgumentNullException("t");

			ITraceSourceFilter result;
			string key = String.Intern(t.Namespace);

			lock (__lock)
			{
				if (!__filters.TryGetValue(key, out result))
				{
					Stack<string> keys = new Stack<string>();
					TraceSource src = new TraceSource(key);
					keys.Push(key);
					while (src.Switch.DisplayName == key)
					{
						int i = key.LastIndexOf('.');
						if (i <= 0) break;
						key = String.Intern(key.Substring(0, i));
						if (__filters.TryGetValue(key, out result))
						{
							src = result.Source;
							break;
						}
						keys.Push(key);
						src = new TraceSource(key);
					}
					switch (src.Switch.Level)
					{
						case SourceLevels.ActivityTracing:
						case SourceLevels.All:
							result = new AllSourceFilter(src);
							break;
						case SourceLevels.Critical:
							result = new CriticalSourceFilter(src);
							break;
						case SourceLevels.Error:
							result = new ErrorSourceFilter(src);
							break;
						case SourceLevels.Information:
							result = new InformationSourceFilter(src);
							break;
						case SourceLevels.Verbose:
							result = new VerboseSourceFilter(src);
							break;
						case SourceLevels.Warning:
							result = new WarningSourceFilter(src);
							break;
						case SourceLevels.Off:
						default:
							result = new NullSourceFilter(src);
							break;
					}
					foreach (string kk in keys)
					{
						__filters.Add(kk, result);
					}
				}
			}
			return result;
		}

		#endregion Methods

		#region Nested Types

		internal static class TraceAdapter<T>
			where T : ITraceable
		{
			#region Fields

			private static ITraceSourceFilter __filter;

			#endregion Fields

			#region Constructors

			static TraceAdapter()
			{
				__filter = Diagnostics.AcquireSourceFilter(typeof(T));
			}

			#endregion Constructors

			#region Properties

			internal static ITraceSourceFilter Filter
			{
				get { return __filter; }
			}

			#endregion Properties
		}

		#endregion Nested Types
	}
}