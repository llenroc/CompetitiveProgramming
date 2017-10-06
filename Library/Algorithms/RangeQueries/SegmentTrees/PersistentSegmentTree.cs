using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Softperson.Algorithms.RangeQueries
{
	using STType = Int64;

	public class PersistentSegmentTree
	{
		STType _min;
		STType _max;
		STType _sum;
		STType _add;
		readonly PersistentSegmentTree _left;
		readonly PersistentSegmentTree _right;
		bool _covering;
		public int Count;

		// public int Start;
		// public int End => Start + Count;

		public PersistentSegmentTree(STType[] array)
			: this(array, 0, array.Length)
		{
		}

		public PersistentSegmentTree(STType[] array, int start, int count)
		{
			// Start = start;
			Count = count;

			if (count >= 2)
			{
				int mid = count / 2;
				_left = new PersistentSegmentTree(array, start, mid);
				_right = new PersistentSegmentTree(array, start + mid, count - mid);
				UpdateNode();
			}
			else
			{
				var v = array[start];
				_min = v;
				_max = v;
				_sum = v;
			}
		}

		PersistentSegmentTree(PersistentSegmentTree left, PersistentSegmentTree right)
		{
			Count = left.Count + right.Count;
			_left = left;
			_right = right;
			UpdateNode();
		}

		public long GetMin(int start, int count)
		{
			int end = start + count;
			if (start <= 0 && Count <= end)
				return _min;
			if (start >= Count || end <= 0)
				return long.MaxValue;

			LazyPropagate();
			return Min(_left.GetMin(start, count),
				_right.GetMin(start - _left.Count, count));
		}


		public long GetSum(int start, int count)
		{
			int end = start + count;
			if (start <= 0 && Count <= end)
				return _sum;
			if (start >= Count || end <= 0)
				return 0;

			LazyPropagate();
			return _left.GetSum(start, count)
				+ _right.GetSum(start - _left.Count, count);
		}

		public PersistentSegmentTree Add(int start, int count, STType value)
		{
			int end = start + count;
			if (start >= Count || end <= 0)
				return this;

			if (start <= 0 && Count <= end)
				return Add(value);

			LazyPropagate();
			return new PersistentSegmentTree(
				_left.Add(start, count, value),
				_right.Add(start - _left.Count, count, value)
			);
		}

		public PersistentSegmentTree Cover(int start, int count, STType value)
		{
			int end = start + count;
			if (start >= Count || end <= 0)
				return this;

			if (start <= 0 && Count <= end)
				return Cover(value);

			LazyPropagate();
			return new PersistentSegmentTree(
				_left.Cover(start, count, value),
				_right.Cover(start - _left.Count, count, value));
		}

		PersistentSegmentTree Clone()
		{
			return (PersistentSegmentTree)MemberwiseClone();
		}

		PersistentSegmentTree Add(STType value)
		{
			var clone = Clone();
			clone._sum += value * Count;
			clone._min += value;
			clone._max += value;
			clone._add += value;
			return clone;
		}

		PersistentSegmentTree Cover(STType value)
		{
			var clone = Clone();
			clone._min = value;
			clone._max = value;
			clone._add = 0;
			clone._sum = value * Count;
			clone._covering = true;
			return clone;
		}

		void LazyPropagate()
		{
			if (Count <= 1)
				return;

			if (_covering)
			{
				_left.Cover(_min);
				_right.Cover(_min);
				_add = 0;
				_covering = false;
				return;
			}

			if (_add != 0)
			{
				var value = _add;
				_add = 0;
				_left.Add(value);
				_right.Add(value);
			}
		}

		void UpdateNode()
		{
			var left = _left;
			var right = _right;
			_sum = left._sum + right._sum;
			_min = Min(left._min, right._min);
			_max = Max(left._max, right._max);
		}

		public PersistentSegmentTree Div(int start, int count, int value)
		{
			int end = start + count;
			if (value == 1 || start >= Count || end <= 0)
				return this;

			if (start <= 0 && Count <= end)
			{
				/*
				// Before
				Min = Div(Min, value);
				Max = Div(Max, value);
				if (Min == Max)
				{
				    Cover(Min);
				    return;
				}
				*/

				// After
				var d1 = Div(_min, value) - _min;
				var d2 = Div(_max, value) - _max;
				if (d1 == d2)
					return Add(d1);
			}

			LazyPropagate();
			return new PersistentSegmentTree(
				_left.Div(start, count, value),
				_right.Div(start - _left.Count, count, value)
			);
		}

		static long Div(long v, long d)
		{
			return (long)Floor(v / (double)d);
		}
	}
}
