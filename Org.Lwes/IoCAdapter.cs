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
		#region Fields

		static IoCAdapter __current;
		static Object __creationLock = new Object();
		static bool __hardFailOnAccessingIoC;

		#endregion Fields

		#region Properties

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

		#endregion Properties

		#region Methods

		/// <summary>
		/// Tries to create a named instance of type T using the currently
		/// configured IoC container.
		/// </summary>
		/// <typeparam name="T">instance type T</typeparam>
		/// <param name="instance">reference to a variable where the resulting 
		/// instance will be stored</param>
		/// <returns><em>true</em> if a valid instance of type <typeparamref name="T"/> was stored
		/// in <paramref name="instance"/>; otherwise <em>false</em></returns>
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
		/// Tries to create a named instance of type T using the currently
		/// configured IoC container.
		/// </summary>
		/// <typeparam name="T">instance type T</typeparam>
		/// <param name="name">name of the instance</param>
		/// <param name="instance">reference to a variable where the resulting 
		/// instance will be stored</param>
		/// <returns><em>true</em> if a valid instance of type T was stored
		/// in <paramref name="instance"/>; otherwise <em>false</em></returns>
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
					// Set the failure first so we don't recurse inadvertently
					__hardFailOnAccessingIoC = true;

					Diagnostics.TraceData(typeof(IoCAdapter), TraceEventType.Error, Resources.Error_IocAdapterFailure, e);
					/* IoC failure - remember this and stop trying */
				}
			}
			instance = default(T);
			return false;
		}

		/// <summary>
		/// Tries to create/access an instance of type T
		/// </summary>
		/// <typeparam name="T">target type T</typeparam>
		/// <param name="instance">reference to a variable that will hold the instance</param>
		/// <returns><em>true</em> if the variable is set to a valid instance of type T
		/// during the call; otherwise <em>false</em></returns>
		public abstract bool TryCreate<T>(out T instance);

		/// <summary>
		/// Tries to create/access a named instance of type T
		/// </summary>
		/// <typeparam name="T">target type T</typeparam>
		/// <param name="name">name of an instance to create</param>
		/// <param name="instance">reference to a variable that will hold the instance</param>
		/// <returns><em>true</em> if the variable is set to a valid instance of type T
		/// during the call; otherwise <em>false</em></returns>
		public abstract bool TryCreate<T>(string name, out T instance);

		private static IoCAdapter CreateIoCAdapterFromConfiguration()
		{
			throw new NotImplementedException();
		}

		#endregion Methods
	}
}