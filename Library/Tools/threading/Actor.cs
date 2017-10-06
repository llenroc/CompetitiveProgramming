#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2005, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

#endregion

namespace Softperson.Threading
{
	public class Actor
	{
		#region Variables

		private readonly ActionList _actionList;

		#endregion

		#region Constructor

		protected Actor(string name)
		{
			_actionList = new ActionList(name);
		}

		#endregion

		#region Properties

		public bool Busy
		{
			get { return _actionList.Busy; }
		}

		#endregion
	}

	public class ActionList
	{
		#region Properties

		public bool Busy
		{
			get { return _alive != 0; }
		}

		#endregion

		#region Classes

		[DebuggerStepThrough]
		public class Message : IComparable<Message>
		{
			private static long time;
			public readonly Action<Message> Action;
			public readonly long Time;
			public bool Completed;
			public bool Ignore;
			public int Priority;
			public object Tag;

			public Message(int priority, Action<Message> a)
			{
				Time = time++;
				Action = a;
				Priority = priority;
			}

			#region IComparable<Message> Members

			public int CompareTo(Message other)
			{
				var cmp = Priority.CompareTo(other.Priority);
				if (cmp != 0)
					return cmp;

				cmp = Time.CompareTo(other.Time);
				return cmp;
			}

			#endregion
		}

		#endregion

		#region Variables

		//private readonly Heap<Message> heap = new Heap<Message>();
		private readonly Queue<Message> _queue = new Queue<Message>();
		public Message ActiveMessage;
		public string Name;
		public ThreadPriority Priority = ThreadPriority.Normal;
		public Thread Thread;
		private int _alive;
		public event EventHandler OnException;

		public event Action<Message> Action
		{
			add { Add(value); }
			remove { throw new InvalidOperationException(); }
		}

		#endregion

		#region Methods

		public ActionList(string name)
		{
			Name = name;
		}

		[DebuggerStepThrough]
		public Message Add(Action<Message> a)
		{
			return Add(0, a);
		}

		[DebuggerStepThrough]
		public Message Add(int priority, Action<Message> a)
		{
			var m = new Message(priority, a);
			Enqueue(m);
			return m;
		}

		// Pulse must be called inside lock
		private void Pulse()
		{
			var alive = Interlocked.Exchange(ref _alive, 1);
			if (alive != 0)
				return;

			Thread.CurrentThread.Priority = ThreadPriority.Normal;
			ThreadPool.QueueUserWorkItem(
				delegate
				{
					Thread = Thread.CurrentThread;
					Thread.Name = Name;
					Thread.Priority = Priority;

					while (true)
					{
						Message m;
						lock (_queue)
						{
							if (_queue.Count == 0)
							{
								Interlocked.Exchange(ref _alive, 0);
								return;
							}
							m = _queue.Dequeue();
						}

						try
						{
							ActiveMessage = m;
							if (!m.Ignore)
								m.Action(m);
						}
						catch (Exception e)
						{
							OnException(e, EventArgs.Empty);
						}
						finally
						{
							m.Completed = true;
							ActiveMessage = null;
						}
					}
				});
		}

		[DebuggerStepThrough]
		private void Enqueue(Message data)
		{
			if (data == null) throw new ArgumentNullException("data");
			lock (_queue)
			{
				_queue.Enqueue(data);
				Pulse();
			}
		}

		[DebuggerStepThrough]
		public void ClearAll()
		{
			lock (_queue)
			{
				_queue.Clear();

				var m = ActiveMessage;
				if (m != null)
					m.Ignore = true;
				Pulse();
			}
		}

		#endregion
	}
}