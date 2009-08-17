namespace Org.Lwes
{
	using System;
	using System.Threading;

	/// <summary>
	/// Utility structure for performing and tracking threadsafe state transitions.
	/// </summary>
	/// <typeparam name="E">State type E (should be an enum)</typeparam>
	public struct Status<E>
	{
		#region Fields

		int _status;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="initialialState">Initial state</param>
		public Status(E initialialState)
		{
			_status = Convert.ToInt32(initialialState);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Accesses the current state.
		/// </summary>
		public E CurrentState
		{
			get { return (E)Enum.ToObject(typeof(E), Thread.VolatileRead(ref _status)); }
		}

		/// <summary>
		/// Perfroms a spinwait until the current state equals the target state.
		/// </summary>
		/// <param name="targetState">the target state</param>
		/// <param name="loopAction">An action to perform inside the spin cycle</param>
		public void SpinWaitForState(E targetState, Action loopAction)
		{
			int state = Convert.ToInt32(targetState);
			while (Thread.VolatileRead(ref _status) != state)
			{
				loopAction();
			}
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Transitions to the given state.
		/// </summary>
		/// <param name="value">the target state</param>
		public void ForceTransition(E value)
		{
			Thread.VolatileWrite(ref _status, Convert.ToInt32(value));
		}

		/// <summary>
		/// Performs a state transition if the current state compares greater than the <paramref name="comparand"/>
		/// </summary>
		/// <param name="value">the target state</param>
		/// <param name="comparand">comparand state</param>
		/// <returns><em>true</em> if the current state compares greater than <paramref name="comparand"/>; otherwise <em>false</em></returns>
		public bool StateTransitionIfGreaterThan(E value, E comparand)
		{
			int c = Convert.ToInt32(comparand);
			int v = Convert.ToInt32(value);

			int init, fin = Thread.VolatileRead(ref _status);
			while (true)
			{
				if (fin < c) return false;

				init = fin;
				fin = Interlocked.CompareExchange(ref _status, c, init);
				if (fin == init) return true;
			}
		}

		/// <summary>
		/// Performs a state transition if the current state compares less than the <paramref name="comparand"/>
		/// </summary>
		/// <param name="value">the target state</param>
		/// <param name="comparand">comparand state</param>
		/// <returns><em>true</em> if the current state compares less than <paramref name="comparand"/>; otherwise <em>false</em></returns>
		public bool StateTransitionIfLessThan(E value, E comparand)
		{
			int c = Convert.ToInt32(comparand);
			int v = Convert.ToInt32(value);

			int init, fin = Thread.VolatileRead(ref _status);
			while (true)
			{
				if (fin > c) return false;

				init = fin;
				fin = Interlocked.CompareExchange(ref _status, c, init);
				if (fin == init) return true;
			}
		}

		/// <summary>
		/// Tries to transition the journaler's state
		/// </summary>
		/// <param name="value">the target state</param>
		/// <param name="comparand">existing state</param>
		/// <returns><em>true</em> if the current state matches <paramref name="comparand"/> and the state is transitioned to <paramref name="value"/>; otherwise <em>false</em></returns>
		public bool TryStateTransition(E value, E comparand)
		{
			int c = Convert.ToInt32(comparand);
			return Interlocked.CompareExchange(ref _status, Convert.ToInt32(value), c) == c;
		}

		#endregion Methods
	}
}