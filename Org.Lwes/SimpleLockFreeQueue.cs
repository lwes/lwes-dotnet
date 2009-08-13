namespace Org.Lwes
{
	using System;
	using System.Threading;

	/// <summary>
	/// Simple implementation of a lock-free queue.
	/// </summary>
	/// <typeparam name="T">queued item type T</typeparam>
	public class SimpleLockFreeQueue<T>
	{
		#region Fields

		private NodeRec _head;
		private NodeRec _tail;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public SimpleLockFreeQueue()
		{
			Node node = new Node();
			_head.Node = _tail.Node = node;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Determines if the queue is empty. Because of the non-blocking nature of the queue there is no guarantee
		/// made about the availablility of items in the queue after this call. At best it can serve as an indicator.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				NodeRec head = _head;
				NodeRec tail = _tail;
				NodeRec next = head.Node.Next;

				// Are head, tail, and next consistent?
				if (head.Count == _head.Count
					&& head.Node == _head.Node
					&& head.Node == tail.Node
					&& null == next.Node)
				{
					return true;
				}

				return false;
			}
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Dequeues an item if it is available in the queue.
		/// </summary>
		/// <param name="item">the next available item in the queue, otherwise default(T).</param>
		/// <returns><em>true</em> if an queued item was retreived by the call, otherwise <em>false</em></returns>
		public bool Dequeue(out T item)
		{
			NodeRec head;

			// Keep trying until we get an item or the queue is empty
			while (true)
			{
				// read head
				head = _head;

				// read tail
				NodeRec tail = _tail;

				// read next
				NodeRec next = head.Node.Next;

				// Are head, tail, and next consistent?
				if (head.Count == _head.Count && head.Node == _head.Node)
				{
					// is tail falling behind
					if (head.Node == tail.Node)
					{
						if (null == next.Node)
						{
							// queue is empty, we're done.
							break;
						}

						// Tail is falling behind. try to advance it
						CAS(ref _tail, tail, new NodeRec(next.Node, tail.Count + 1));
					}
					else
					{
						// No need to deal with tail,
						// read value before CAS otherwise concurrent op might try to free the next node
						item = next.Node.Value;

						// try to swing the head to the next node
						if (CAS(ref _head, head, new NodeRec(next.Node, head.Count + 1)))
						{
							return true;
						}
					}
				}
			}

			item = default(T);
			return false;
		}

		/// <summary>
		/// Enqueues an item.
		/// </summary>
		/// <param name="item">The item to place in the queue</param>
		public void Enqueue(T item)
		{
			Node node = new Node();
			node.Value = item;

			while (true)
			{
				NodeRec tail = _tail;
				NodeRec next = tail.Node.Next;

				if (tail.Count == _tail.Count && tail.Node == _tail.Node)
				{
					if (null == next.Node)
					{
						// Tail was pointing at the last node
						if (CAS(ref tail.Node.Next, next, new NodeRec(node, next.Count + 1)))
						{
							break;
						}
					}
					else
					{
						// tail was not pointing at the last node,
						// try to swing Tail to the next node
						CAS(ref _tail, tail, new NodeRec(next.Node, tail.Count + 1));
					}
				}
			}
		}

		private bool CAS(ref NodeRec destination, NodeRec compared, NodeRec exchange)
		{
			if (compared.Node == Interlocked.CompareExchange(ref destination.Node, exchange.Node, compared.Node))
			{
				Interlocked.Exchange(ref destination.Count, exchange.Count);
				return true;
			}

			return false;
		}

		#endregion Methods

		#region Nested Types

		struct NodeRec
		{
			#region Fields

			public long Count;
			public Node Node;

			#endregion Fields

			#region Constructors

			public NodeRec(Node node, long c)
			{
				Node = node;
				Count = c;
			}

			#endregion Constructors

			#if DEBUG

			public override string ToString()
			{
				return String.Concat("NodeRec { Count = ", Count.ToString(), ", Node = ", Node.ToString(), "}");
			}

			#endif
		}

		class Node
		{
			#region Fields

			public NodeRec Next;
			public T Value;

			#endregion Fields

			#if DEBUG

			// Helpful in the debugger when inspecting the node list
			public override string ToString()
			{
				return String.Concat("Node { Next = ", Next.Count.ToString(), ", Value = ",
					(Value != null) ? Value.ToString() : "null", "}");
			}

			#endif
		}

		#endregion Nested Types
	}
}