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
namespace Org.Lwes
{
	using System;
	using System.Diagnostics;
	using Org.Lwes.Properties;

	/// <summary>
	/// Utility methods for interacting with IoC containers.
	/// </summary>
	public abstract class IoCAdapter
	{
		public abstract bool TryCreate<T>(out T instance);
		public abstract bool TryCreate<T>(string name, out T instance);

		#region Fields

		static IoCAdapter __current;
		static Object __creationLock = new Object();
		static bool __hardFailOnAccessingIoC;

		#endregion Fields

		#region Methods

		internal static IoCAdapter Current
		{
			get
			{
				if (__hardFailOnAccessingIoC) return null;
				return Util.LazyInitializeWithLock(ref __current, __creationLock, () =>
					{
						return CreateIoCAdapterFromConfiguration();
					});
			}
		}

		private static IoCAdapter CreateIoCAdapterFromConfiguration()
		{
			throw new NotImplementedException();
		}

		public static bool TryCreateFromIoC<T>(out T instance)
		{
			if (!__hardFailOnAccessingIoC)
			{
				try
				{
					// Try to use the service locator - this is IoC implementation independent
					IoCAdapter current = IoCAdapter.Current;
					if (current != null)
					{
						return current.TryCreate(out instance);
					}
				}
				catch (Exception e)
				{
					Diagnostics.TraceData(typeof(IoCAdapter), TraceEventType.Error, Resources.Error_IocAdapterFailure, e);
					/* IoC failure - remember this and stop trying */
					__hardFailOnAccessingIoC = true;
				}
			}
			instance = default(T);
			return false;
		}

		/// <summary>
		/// Attempts to create a named instance of type T using the currently
		/// configured IoC container.
		/// </summary>
		/// <typeparam name="T">new instance type T</typeparam>
		/// <param name="name">the name of the instance to create</param>
		/// <returns>if an IoC container is in use and there is a named
		/// instance configured in the container then the instance is 
		/// returned; otherwise null</returns>
		public static bool TryCreateFromIoC<T>(string name, out T instance)
		{
			if (!__hardFailOnAccessingIoC)
			{
				try
				{
					// Try to use the service locator - this is IoC implementation independent
					IoCAdapter current = IoCAdapter.Current;
					if (current != null)
					{
						return current.TryCreate(out instance);
					}
				}
				catch (Exception e)
				{
					Diagnostics.TraceData(typeof(IoCAdapter), TraceEventType.Error, Resources.Error_IocAdapterFailure, e);
					/* IoC failure - remember this and stop trying */
					__hardFailOnAccessingIoC = true;
				}
			}
			instance = default(T);
			return false;
		}

		#endregion Methods
	}
}