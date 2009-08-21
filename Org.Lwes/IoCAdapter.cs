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

	using Microsoft.Practices.ServiceLocation;

	/// <summary>
	/// Utility methods for interacting with IoC containers.
	/// </summary>
	public static class IoCAdapter
	{
		#region Fields

		static bool __hardFailOnAccessingIoC;

		#endregion Fields

		#region Methods

		/// <summary>
		/// Attempts to create a named instance of type T using the currently
		/// configured IoC container.
		/// </summary>
		/// <typeparam name="T">new instance type T</typeparam>
		/// <param name="name">the name of the instance to create</param>
		/// <returns>if an IoC container is in use and there is a named
		/// instance configured in the container then the instance is 
		/// returned; otherwise null</returns>
		public static T CreateFromIoC<T>(string name)
		{
			T result = default(T);

			if (!__hardFailOnAccessingIoC)
			{
				try
				{
					// Try to use the service locator - this is IoC implementation independent
					IServiceLocator loc = ServiceLocator.Current;
					if (loc != null)
					{
						result = loc.GetInstance<T>(name);
					}
				}
				catch (NullReferenceException)
				{
					/* no IoC - fall through */
					__hardFailOnAccessingIoC = true;
				}
			}
			return result;
		}

		#endregion Methods
	}
}