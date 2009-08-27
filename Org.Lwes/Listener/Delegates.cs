using System;
using System.Net;

namespace Org.Lwes.Listener
{
	public delegate void OnLwesEventArrived(IEventListener listener, Event ev);


	public class GarbageDataEventArgs : EventArgs
	{
		private GarbageDataEventArgs() { }
		public GarbageDataEventArgs(EndPoint remoteEP, byte[] data, int priors, GarbageHandlingVote vote)
		{
			RemoteEndPoint = remoteEP;
			Garbage = data;
			PriorGarbageCountForEndpoint = priors;
			_vote = vote;
		}
		public byte[] Garbage { get; private set; }
		public EndPoint RemoteEndPoint { get; private set; }
		public int PriorGarbageCountForEndpoint { get; private set; }
		GarbageHandlingVote _vote;
		public GarbageHandlingVote HandlingVote
		{
			get { return _vote; }
			set
			{
				if (value > _vote)
					_vote = value;
			}
		}
	}
	public delegate void OnLwesGarbageArrived(IEventListener listener, GarbageDataEventArgs ev);
}
