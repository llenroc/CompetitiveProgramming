﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Collections
{
	public class MaxQueue<T> where T : IComparable<T>
	{
		readonly DoubleEndedQueue<Tuple<T,int>> _queue;

		public MaxQueue()
		{
			_queue = new DoubleEndedQueue<Tuple<T,int>>();
		}

		public int Count => _queue.Count;

		public void Enqueue(T item)
		{
			int count = 0;
			while (_queue.Count > 0 && _queue.Last().Item1.CompareTo(item) < 0)
				count += _queue.PopLast().Item2+1;
			_queue.PushLast(Tuple.Create(item, count));
		}
		
		public T Dequeue()
		{
			var peek = _queue.PopFirst();
			if (peek.Item2 > 0)
				_queue.PushFirst(Tuple.Create(peek.Item1, peek.Item2-1));
			return peek.Item1;
		}

	}
}
