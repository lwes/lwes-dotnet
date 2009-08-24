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
// Lesser GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//

#endregion Header

namespace Org.Lwes.Trace
{
	using System;
	using System.Diagnostics;

	internal interface ITraceSourceFilter
	{
		TraceSource Source
		{
			get;
		}

		bool ShouldTrace(TraceEventType eventType);

		void TraceData(TraceEventType eventType, int id, object data);

		void TraceData(TraceEventType eventType, int id, params object[] data);

		void TraceError(int id, string message);

		void TraceError(int id, string format, params object[] args);

		void TraceEvent(TraceEventType eventType, int id);

		void TraceEvent(TraceEventType eventType, int id, string message);

		void TraceEvent(TraceEventType eventType, int id, string format, params object[] args);

		void TraceTransfer(int id, string message, Guid relatedActivityId);

		void TraceVerbose(int id, string message);

		void TraceVerbose(int id, string format, params object[] args);

		void TraceWarning(int id, string message);

		void TraceWarning(int id, string format, params object[] args);
	}
}