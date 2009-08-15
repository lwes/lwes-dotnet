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