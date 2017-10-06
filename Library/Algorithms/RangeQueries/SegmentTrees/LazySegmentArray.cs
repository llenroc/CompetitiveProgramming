using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Softperson.Algorithms.RangeQueries
{
	public class LazySegmentArray
	{
		long[] _sum;
		long[] _min;
		long[] _max;
		long[] _lazy;
		int _n;
		int _size;


		public LazySegmentArray(long[] array) : this(array.Length)
		{

			for (int i = 0; i < array.Length; i++)
				_sum[_n + i] = _min[_n + i] = _max[_n + i] = array[i];

			for (int i = _n - 1; i > 0; i--)
				UpdateNode(i);
		}

		public LazySegmentArray(int n)
		{
			_n = LeastPowerOfTwoGreaterOrEqualTo(n);
			_size = 2 * _n;
			_sum = new long[_size];
			_min = new long[_size];
			_max = new long[_size];
			_lazy = new long[_size];

			for (int i = 0; i < _n; i++)
			{
				_min[i] = long.MaxValue;
				_max[i] = long.MinValue;
			}
		}

		public static int LeastPowerOfTwoGreaterOrEqualTo(int n)
		{
			int bits = n;
			while ((bits & bits - 1) != 0)
				bits &= bits - 1;
			if (n > bits) bits <<= 1;
			return bits;
		}

		public void AddExclusive(int start, int end, int value)
		{
			if (value == 0) return;
			Add(1, start, end, value, 0, _n);
		}

		void Add(int node, int start, int end, long value, int nodeStart, int nodeLimit)
		{
			if (start >= nodeLimit || end <= nodeStart)
				return;

			if (nodeStart >= start && nodeLimit <= end)
			{
				LazyAdd(node, value, nodeStart, nodeLimit);
				return;
			}

			LazyPropagate(node, nodeStart, nodeLimit);
			int mid = (nodeStart + nodeLimit) >> 1;
			Add(node * 2, start, end, value, nodeStart, mid);
			Add(node * 2 + 1, start, end, value, mid, nodeLimit);
			UpdateNode(node);
		}

		// TODO: ADD Update 1
		/*
		         void update(int node, int start, int end, int idx, int val)
        {
            if (start == end)
            {
                // Leaf node
                A[idx] += val;
                tree[node] += val;
            }
            else
            {
                int mid = (start + end) / 2;
                if (start <= idx && idx <= mid)
                {
                    // If idx is in the left child, recurse on the left child
                    update(2 * node, start, mid, idx, val);
                }
                else
                {
                    // if idx is in the right child, recurse on the right child
                    update(2 * node + 1, mid + 1, end, idx, val);
                }
                // Internal node will have the sum of both of its children
                tree[node] = tree[2 * node] + tree[2 * node + 1];
            }
        }
		 */


		void LazyAdd(int node, long value, int nodeStart, int nodeLimit)
		{
			_sum[node] += value * (nodeLimit - nodeStart);
			_min[node] += value;
			_max[node] += value;
			_lazy[node] += value;
		}

		public void DivExclusive(int start, int end, int value)
		{
			if (value == 1) return;
			Div(1, start, end, value, 0, _n);
		}

		void Div(int node, int start, int end, int value, int nodeStart, int nodeLimit)
		{
			if (start >= nodeLimit || end <= nodeStart)
				return;

			if (nodeStart >= start && nodeLimit <= end)
			{
				var d1 = Div(_min[node], value) - _min[node];
				var d2 = Div(_max[node], value) - _max[node];
				if (d1 == d2)
				{
					LazyAdd(node, d1, nodeStart, nodeLimit);
					return;
				}
			}

			int mid = (nodeStart + nodeLimit) >> 1;
			LazyPropagate(node, nodeStart, nodeLimit);
			Div(node * 2, start, end, value, nodeStart, mid);
			Div(node * 2 + 1, start, end, value, mid, nodeLimit);
			UpdateNode(node);
		}


		public static long Div(long v, long d)
		{
			return (long)Floor(v / (double)d);
		}

		public long GetMinExclusive(int start, int end)
		{
			return GetMin(1, start, end, 0, _n);
		}

		long GetMin(int node, int start, int end, int nodeStart, int nodeLimit)
		{
			if (nodeStart >= start && nodeLimit <= end)
				return _min[node];
			if (start >= nodeLimit || end <= nodeStart)
				return long.MaxValue;

			int mid = (nodeStart + nodeLimit) >> 1;
			LazyPropagate(node, nodeStart, nodeLimit);
			return Min(GetMin(node * 2, start, end, nodeStart, mid),
				GetMin(node * 2 + 1, start, end, mid, nodeLimit));
		}

		public long GetSumExclusive(int start, int end)
		{
			return GetSum(1, start, end, 0, _n);
		}

		long GetSum(int node, int start, int end, int nodeStart, int nodeLimit)
		{
			if (nodeStart >= start && nodeLimit <= end)
				return _sum[node];
			if (start >= nodeLimit || end <= nodeStart)
				return 0;

			int mid = (nodeStart + nodeLimit) >> 1;
			LazyPropagate(node, nodeStart, nodeLimit);
			return GetSum(node * 2, start, end, nodeStart, mid)
				   + GetSum(node * 2 + 1, start, end, mid, nodeLimit);
		}

		void LazyPropagate(int node, int nodeStart, int nodeLimit)
		{
			if (_lazy[node] != 0)
			{
				int mid = (nodeStart + nodeLimit) >> 1;
				LazyAdd(node * 2, _lazy[node], nodeStart, mid);
				LazyAdd(node * 2 + 1, _lazy[node], mid, nodeLimit);
				_lazy[node] = 0;
			}
		}

		void UpdateNode(int node)
		{
			_min[node] = Min(_min[node * 2], _min[node * 2 + 1]);
			_max[node] = Max(_max[node * 2], _max[node * 2 + 1]);
			_sum[node] = _sum[node * 2] + _sum[node * 2 + 1];
		}
	}

}
