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

namespace Org.Lwes.Trace.Filters
{
	using System;
	using System.Diagnostics;

	internal class ErrorSourceFilter : ITraceSourceFilter
	{
		#region Fields

		private TraceSource _traceSource;

		#endregion Fields

		#region Constructors

		internal ErrorSourceFilter(TraceSource source)
		{
			if (source == null) throw new ArgumentNullException("source");

			_traceSource = source;
		}

		internal ErrorSourceFilter(string sourceName)
		{
			if (sourceName == null) throw new ArgumentNullException("sourceName");

			_traceSource = new TraceSource(sourceName);
		}

		#endregion Constructors

		#region Properties

		TraceSource ITraceSourceFilter.Source
		{
			get { return _traceSource; }
		}

		#endregion Properties

		#region Methods

		bool ITraceSourceFilter.ShouldTrace(TraceEventType traceType)
		{
			return _traceSource.Switch.ShouldTrace(traceType);
		}

		void ITraceSourceFilter.TraceData(TraceEventType eventType, int id, object data)
		{
			if (eventType <= TraceEventType.Error)
				_traceSource.TraceData(eventType, id, data);
		}

		void ITraceSourceFilter.TraceData(TraceEventType eventType, int id, params object[] data)
		{
			if (eventType <= TraceEventType.Error)
				_traceSource.TraceData(eventType, id, data);
		}

		void ITraceSourceFilter.TraceError(int id, string message)
		{
			_traceSource.TraceEvent(TraceEventType.Error, id, message);
		}

		void ITraceSourceFilter.TraceError(int id, string format, params object[] args)
		{
			_traceSource.TraceEvent(TraceEventType.Error, id, String.Format(format, args));
		}

		void ITraceSourceFilter.TraceEvent(TraceEventType eventType, int id)
		{
			if (eventType <= TraceEventType.Error)
				_traceSource.TraceEvent(eventType, id);
		}

		void ITraceSourceFilter.TraceEvent(TraceEventType eventType, int id, string message)
		{
			if (eventType <= TraceEventType.Error)
				_traceSource.TraceEvent(eventType, id, message);
		}

		void ITraceSourceFilter.TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
			if (eventType <= TraceEventType.Error)
				_traceSource.TraceEvent(eventType, id, format, args);
		}

		void ITraceSourceFilter.TraceTransfer(int id, string message, Guid relatedActivityId)
		{
		}

		void ITraceSourceFilter.TraceVerbose(int id, string message)
		{
		}

		void ITraceSourceFilter.TraceVerbose(int id, string format, params object[] args)
		{
		}

		void ITraceSourceFilter.TraceWarning(int id, string message)
		{
		}

		void ITraceSourceFilter.TraceWarning(int id, string format, params object[] args)
		{
		}

		#endregion Methods
	}
}