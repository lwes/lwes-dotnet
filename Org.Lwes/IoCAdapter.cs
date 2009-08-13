namespace Org.Lwes
{
	using System;

	using Microsoft.Practices.ServiceLocation;

	public static class IoCAdapter
	{
		#region Fields

		static bool __hardFailOnAccessingIoC;

		#endregion Fields

		#region Methods

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