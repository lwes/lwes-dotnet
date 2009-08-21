#region Header

//
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT© 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
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
// GNU General Public License for more details.
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

	public static class Diagnostics
	{
		#region Fields

		public static readonly string CDiagnosticsConfigSectionName = "diagnostics";

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
		{
			Diagnostics<T>.TraceData(eventType, DefaultTraceEventID, data);
		}

		public static void TraceData<T>(this T source, TraceEventType eventType, Func<object[]> dataGenerator)
		{
			if (Diagnostics<T>.ShouldTrace(eventType))
			{
				Diagnostics<T>.TraceData(eventType, DefaultTraceEventID, dataGenerator());
			}
		}

		public static void TraceData<T>(this T source, TraceEventType eventType, int id, object data)
		{
			Diagnostics<T>.TraceData(eventType, id, data);
		}

		public static void TraceData<T>(this T source, TraceEventType eventType, params object[] data)
		{
			Diagnostics<T>.TraceData(eventType, DefaultTraceEventID, data);
		}

		public static void TraceData<T>(this T source, TraceEventType eventType, int id, params object[] data)
		{
			Diagnostics<T>.TraceData(eventType, id, data);
		}

		public static void TraceError<T>(this T source, string message)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Error, DefaultTraceEventID, message);
		}

		public static void TraceError<T>(this T source, int id, string message)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Error, id, message);
		}

		public static void TraceError<T>(this T source, string format, params object[] args)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Error, DefaultTraceEventID, format, args);
		}

		public static void TraceError<T>(this T source, int id, string format, params object[] args)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Error, id, format, args);
		}

		public static void TraceEvent<T>(this T source, TraceEventType eventType, int id)
		{
			Diagnostics<T>.TraceEvent(eventType, id);
		}

		public static void TraceEvent<T>(this T source, TraceEventType eventType, string message)
		{
			Diagnostics<T>.TraceEvent(eventType, DefaultTraceEventID, message);
		}

		public static void TraceEvent<T>(this T source, TraceEventType eventType, int id, string message)
		{
			Diagnostics<T>.TraceEvent(eventType, id, message);
		}

		public static void TraceEvent<T>(this T source, TraceEventType eventType, string format, params object[] args)
		{
			Diagnostics<T>.TraceEvent(eventType, DefaultTraceEventID, format, args);
		}

		public static void TraceEvent<T>(this T source, TraceEventType eventType, int id, string format, params object[] args)
		{
			Diagnostics<T>.TraceEvent(eventType, id, format, args);
		}

		public static void TraceInformation<T>(this T source, string message)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Information, DefaultTraceEventID, message);
		}

		public static void TraceInformation<T>(this T source, string format, params object[] args)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Information, DefaultTraceEventID, format, args);
		}

		public static void TraceTransfer<T>(this T source, string message, Guid relatedActivityId)
		{
			Diagnostics<T>.TraceTransfer(DefaultTraceEventID, message, relatedActivityId);
		}

		public static void TraceTransfer<T>(this T source, int id, string message, Guid relatedActivityId)
		{
			Diagnostics<T>.TraceTransfer(id, message, relatedActivityId);
		}

		public static void TraceVerbose<T>(this T source, string message)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, message);
		}

		public static void TraceVerbose<T>(this T source, int id, string message)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Verbose, id, message);
		}

		public static void TraceVerbose<T>(this T source, string format, params object[] args)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Verbose, DefaultTraceEventID, format, args);
		}

		public static void TraceVerbose<T>(this T source, int id, string format, params object[] args)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Verbose, id, format, args);
		}

		public static void TraceWarning<T>(this T source, string message)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Warning, DefaultTraceEventID, message);
		}

		public static void TraceWarning<T>(this T source, int id, string message)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Warning, id, message);
		}

		public static void TraceWarning<T>(this T source, string format, params object[] args)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Warning, DefaultTraceEventID, format, args);
		}

		public static void TraceWarning<T>(this T source, int id, string format, params object[] args)
		{
			Diagnostics<T>.TraceEvent(TraceEventType.Warning, id, format, args);
		}

		internal static ITraceSourceFilter DiscoverSourceFilter<T>()
		{
			ITraceSourceFilter result;
			string key = String.Intern(typeof(T).Namespace);

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
	}

	public static class Diagnostics<T>
	{
		#region Fields

		const int CDefaultTraceEventID = 0;

		private static ITraceSourceFilter __filter;

		#endregion Fields

		#region Constructors

		static Diagnostics()
		{
			__filter = Diagnostics.DiscoverSourceFilter<T>();
		}

		#endregion Constructors

		#region Properties

		public static TraceSource Source
		{
			get { return __filter.Source; }
		}

		internal static ITraceSourceFilter Filter
		{
			get { return __filter; }
		}

		#endregion Properties

		#region Methods

		public static bool ShouldTrace(TraceEventType eventType)
		{
			return __filter.ShouldTrace(eventType);
		}

		public static void TraceData(TraceEventType eventType, object data)
		{
			if (ShouldTrace(eventType))
				__filter.TraceData(eventType, CDefaultTraceEventID, data);
		}

		public static void TraceData(TraceEventType eventType, int id, object data)
		{
			if (ShouldTrace(eventType))
				__filter.TraceData(eventType, id, data);
		}

		public static void TraceData(TraceEventType eventType, params object[] data)
		{
			if (ShouldTrace(eventType))
				__filter.TraceData(eventType, CDefaultTraceEventID, data);
		}

		public static void TraceData(TraceEventType eventType, int id, params object[] data)
		{
			if (ShouldTrace(eventType))
				__filter.TraceData(eventType, id, data);
		}

		public static void TraceError(string message)
		{
			if (ShouldTrace(TraceEventType.Error))
				__filter.TraceEvent(TraceEventType.Error, CDefaultTraceEventID, message);
		}

		public static void TraceError(int id, string message)
		{
			if (ShouldTrace(TraceEventType.Error))
				__filter.TraceEvent(TraceEventType.Error, id, message);
		}

		public static void TraceError(string format, params object[] args)
		{
			if (ShouldTrace(TraceEventType.Error))
				__filter.TraceEvent(TraceEventType.Error, CDefaultTraceEventID, format, args);
		}

		public static void TraceError(int id, string format, params object[] args)
		{
			if (ShouldTrace(TraceEventType.Error))
				__filter.TraceEvent(TraceEventType.Error, id, format, args);
		}

		public static void TraceEvent(TraceEventType eventType, int id)
		{
			if (ShouldTrace(eventType))
				__filter.TraceEvent(eventType, id);
		}

		public static void TraceEvent(TraceEventType eventType, string message)
		{
			if (ShouldTrace(eventType))
				__filter.TraceEvent(eventType, CDefaultTraceEventID, message);
		}

		public static void TraceEvent(TraceEventType eventType, int id, string message)
		{
			if (ShouldTrace(eventType))
				__filter.TraceEvent(eventType, id, message);
		}

		public static void TraceEvent(TraceEventType eventType, string format, params object[] args)
		{
			if (ShouldTrace(eventType))
				__filter.TraceEvent(eventType, CDefaultTraceEventID, format, args);
		}

		public static void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
			if (ShouldTrace(eventType))
				__filter.TraceEvent(eventType, id, format, args);
		}

		public static void TraceInformation(string message)
		{
			if (ShouldTrace(TraceEventType.Information))
				__filter.TraceEvent(TraceEventType.Information, CDefaultTraceEventID, message);
		}

		public static void TraceInformation(string format, params object[] args)
		{
			if (ShouldTrace(TraceEventType.Information))
				__filter.TraceEvent(TraceEventType.Information, CDefaultTraceEventID, format, args);
		}

		public static void TraceTransfer(string message, Guid relatedActivityId)
		{
			if (ShouldTrace(TraceEventType.Transfer))
				__filter.TraceTransfer(CDefaultTraceEventID, message, relatedActivityId);
		}

		public static void TraceTransfer(int id, string message, Guid relatedActivityId)
		{
			if (ShouldTrace(TraceEventType.Transfer))
				__filter.TraceTransfer(id, message, relatedActivityId);
		}

		public static void TraceVerbose(string message)
		{
			if (ShouldTrace(TraceEventType.Verbose))
				__filter.TraceEvent(TraceEventType.Verbose, CDefaultTraceEventID, message);
		}

		public static void TraceVerbose(int id, string message)
		{
			if (ShouldTrace(TraceEventType.Verbose))
				__filter.TraceEvent(TraceEventType.Verbose, id, message);
		}

		public static void TraceVerbose(string format, params object[] args)
		{
			if (ShouldTrace(TraceEventType.Verbose))
				__filter.TraceEvent(TraceEventType.Verbose, CDefaultTraceEventID, format, args);
		}

		public static void TraceVerbose(int id, string format, params object[] args)
		{
			if (ShouldTrace(TraceEventType.Verbose))
				__filter.TraceEvent(TraceEventType.Verbose, id, format, args);
		}

		public static void TraceWarning(string message)
		{
			if (ShouldTrace(TraceEventType.Warning))
				__filter.TraceEvent(TraceEventType.Warning, CDefaultTraceEventID, message);
		}

		public static void TraceWarning(int id, string message)
		{
			if (ShouldTrace(TraceEventType.Warning))
				__filter.TraceEvent(TraceEventType.Warning, id, message);
		}

		public static void TraceWarning(string format, params object[] args)
		{
			if (ShouldTrace(TraceEventType.Warning))
				__filter.TraceEvent(TraceEventType.Warning, CDefaultTraceEventID, format, args);
		}

		public static void TraceWarning(int id, string format, params object[] args)
		{
			if (ShouldTrace(TraceEventType.Warning))
				__filter.TraceEvent(TraceEventType.Warning, id, format, args);
		}

		#endregion Methods
	}
}