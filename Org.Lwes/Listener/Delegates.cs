namespace Org.Lwes.Listener
{
	using System;
	using System.Net;

	#region Delegates

	/// <summary>
	/// Represents the method that handles the <see cref="IEventListener.OnEventArrived"/> event of an <see cref="IEventListener"/>
	/// </summary>
	/// <param name="listener">the source of the event</param>
	/// <param name="ev">the event that arrived</param>
	public delegate void OnLwesEventArrived(IEventListener listener, Event ev);

	/// <summary>
	/// Represents the method that handles the <see cref="IEventListener.OnGarbageArrived"/> event of an <see cref="IEventListener"/>
	/// </summary>
	/// <param name="listener">the source of the event</param>
	/// <param name="args">A <seealso cref="GarbageDataEventArgs"/>  that contains the event data</param>
	public delegate void OnLwesGarbageArrived(IEventListener listener, GarbageDataEventArgs args);

	#endregion Delegates

	/// <summary>
	/// Provides data for the <see cref="IEventListener.OnGarbageArrived"/> event.
	/// </summary>
	public class GarbageDataEventArgs : EventArgs
	{
		#region Fields

		GarbageHandlingVote _vote;

		#endregion Fields

		#region Constructors

		internal GarbageDataEventArgs(EndPoint remoteEP, byte[] data, int priors, GarbageHandlingVote vote)
		{
			RemoteEndPoint = remoteEP;
			Garbage = data;
			PriorGarbageCountForEndpoint = priors;
			_vote = vote;
		}

		private GarbageDataEventArgs()
		{
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// A byte array containing a copy of the garbage data.
		/// </summary>
		public byte[] Garbage
		{
			get; private set;
		}

		/// <summary>
		/// Gets or sets the vote. Garbage handlers may set this value to 
		/// influence how the listener resonds to future data from the remote endoint.
		/// </summary>
		public GarbageHandlingVote HandlingVote
		{
			get { return _vote; }
			set
			{
				if (value > _vote)
					_vote = value;
			}
		}

		/// <summary>
		/// Number of times the endpoint has sent garbage data.
		/// </summary>
		public int PriorGarbageCountForEndpoint
		{
			get; private set;
		}

		/// <summary>
		/// The endpoint that sent the garbage data.
		/// </summary>
		public EndPoint RemoteEndPoint
		{
			get; private set;
		}

		#endregion Properties
	}
}