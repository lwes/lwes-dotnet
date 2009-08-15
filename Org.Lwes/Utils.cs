namespace Org.Lwes
{
	using System;
	using System.Threading;

	/// <summary>
	/// Contains utility methods.
	/// </summary>
	public static class Util
	{
		#region Methods

		/// <summary>
		/// Disposes of the referenced item and sets its value to null.
		/// </summary>
		/// <typeparam name="T">item type T; must implement IDisposable</typeparam>
		/// <param name="item">item to dispose</param>
		public static void Dispose<T>(ref T item)
			where T : class, IDisposable
		{
			if (item != null)
			{
				item.Dispose();
				item = null;
			}
		}

		/// <summary>
		/// Initializes a referenced variable if it is not already initialized. 
		/// Uses the default constroctur for variable type T.
		/// </summary>
		/// <typeparam name="T">variable type T; must be a class with default constructor</typeparam>
		/// <param name="variable">reference to the variable being initialized</param>
		/// <returns>the value of the variable, after the lazy initialize</returns>
		public static T LazyInitialize<T>(ref T variable)
			where T : class, new()
		{
			if (variable == null)
			{
				T ourNewInstance = new T();
				T instanceCreatedByOtherThread = Interlocked.CompareExchange(ref variable, ourNewInstance, null);
				// prefer the race winner's instance... the GC will collect ours.
				variable = (instanceCreatedByOtherThread != null) ? instanceCreatedByOtherThread : ourNewInstance;
			}
			return variable;
		}

		/// <summary>
		/// Initializes a referenced variable if it is not already initialized. Uses
		/// the <paramref name="factoryDelegate"/> to create the instance if necessary.
		/// </summary>
		/// <typeparam name="T">variable type T</typeparam>
		/// <param name="variable">reference to the variable being initialized</param>
		/// <param name="factoryDelegate">factory delegate</param>
		/// <returns>the value of the variable, after the lazy initailize</returns>
		public static T LazyInitialize<T>(ref T variable, Func<T> factoryDelegate)
			where T : class
		{
			if (factoryDelegate == null) throw new ArgumentNullException("factoryDelegate");

			if (variable == null)
			{
				T ourNewInstance = factoryDelegate();
				T instanceCreatedByOtherThread = Interlocked.CompareExchange(ref variable, ourNewInstance, null);
				// prefer the race winner's instance... the GC will collect ours.
				variable = (instanceCreatedByOtherThread != null) ? instanceCreatedByOtherThread : ourNewInstance;
			}
			return variable;
		}

		/// <summary>
		/// Initializes a referenced variable if it is not already initialized. 
		/// </summary>
		/// <typeparam name="T">variable type T; must be a class with default constructor</typeparam>
		/// <param name="variable">reference to the variable being initialized</param>
		/// <returns>the value of the variable, after the lazy initialize</returns>
		/// <remarks>
		/// This method is thread-safe and non-blocking, however, callers must be aware
		/// that a race condition may occur during object creation. It is possible for two 
		/// instances of type T to be created by two different threads calling this method
		/// at the same time; in such a case, the race loser will discard 
		/// its instance in favor of the race winner's instance. This is safe for most well
		/// defined types whose constructors are side-effect free.
		/// </remarks>
		public static T NonBlockingLazyInitializeVolatile<T>(ref object variable)
			where T : class, new()
		{
			T currentValue = (T)Thread.VolatileRead(ref variable);
			if (currentValue != null)
			{
				return currentValue;
			}

			T ourNewInstance = new T();
			T instanceCreatedByOtherThread = (T)Interlocked.CompareExchange(ref variable, ourNewInstance, null);
			// prefer the race winner's instance... the GC will collect ours.
			return (instanceCreatedByOtherThread != null) ? instanceCreatedByOtherThread : ourNewInstance;
		}

		/// <summary>
		/// Initializes a referenced variable if it is not already initialized. 
		/// </summary>
		/// <typeparam name="T">variable type T</typeparam>
		/// <param name="variable">reference to the variable being initialized</param>
		/// <param name="factoryDelegate">factory method that will be called upon to 
		/// create a new instance if necessary</param>
		/// <returns>the value of the variable, after the lazy initialize</returns>
		/// <remarks>
		/// This method is thread-safe and non-blocking, however, callers must be aware
		/// that a race condition may occur during object creation. It is possible for two 
		/// instances of type T to be created by two different threads calling this method
		/// at the same time; in such a case, the race loser will discard 
		/// its instance in favor of the race winner's instance. This is safe for most well
		/// defined types whose constructors are side-effect free.
		/// </remarks>
		public static T NonBlockingLazyInitializeVolatile<T>(ref object variable, Func<T> factoryDelegate)
			where T : class
		{
			if (factoryDelegate == null) throw new ArgumentNullException("factoryDelegate");

			T currentValue = (T)Thread.VolatileRead(ref variable);
			if (currentValue != null)
			{
				return currentValue;
			}

			T ourNewInstance = factoryDelegate();
			T instanceCreatedByOtherThread = (T)Interlocked.CompareExchange(ref variable, ourNewInstance, null);
			// prefer the race winner's instance... the GC will collect ours.
			return (instanceCreatedByOtherThread != null) ? instanceCreatedByOtherThread : ourNewInstance;
		}

		#endregion Methods
	}
}