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

namespace Org.Lwes.Trace.Filters
{
	using System;
	using System.Diagnostics;

	internal class NullSourceFilter : ITraceSourceFilter
	{
		#region Fields

		TraceSource _traceSource;

		#endregion Fields

		#region Constructors

		internal NullSourceFilter(TraceSource src)
		{
			_traceSource = src;
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
			return false;
		}

		void ITraceSourceFilter.TraceData(TraceEventType eventType, int id, object data)
		{
		}

		void ITraceSourceFilter.TraceData(TraceEventType eventType, int id, params object[] data)
		{
		}

		void ITraceSourceFilter.TraceError(int id, string message)
		{
		}

		void ITraceSourceFilter.TraceError(int id, string format, params object[] args)
		{
		}

		void ITraceSourceFilter.TraceEvent(TraceEventType eventType, int id)
		{
		}

		void ITraceSourceFilter.TraceEvent(TraceEventType eventType, int id, string message)
		{
		}

		void ITraceSourceFilter.TraceEvent(TraceEventType eventType, int id, string format, params object[] args)
		{
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