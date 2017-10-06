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

	public class LazySegmentTree
	{
		public STType Min;
		public STType Sum;
		public STType LazyAdd;
		public int Start;
		public int End;
		public LazySegmentTree Left;
		public LazySegmentTree Right;
		public bool Covering;
		public int Length => End - Start + 1;

		public LazySegmentTree(int n)
			: this(null, 0, n - 1)
		{
		}


		public LazySegmentTree(STType[] array)
			: this(array, 0, array.Length-1)
		{
		}

		LazySegmentTree(STType[]array, int start, int end)
		{
			Start = start;
			End = end;

			if (end > start)
			{
				int mid = (start + end) / 2;
				Left = new LazySegmentTree(array, start, mid);
				Right = new LazySegmentTree(array, mid + 1, end);
				UpdateNode();
			}
			else
			{
				var v = array != null ? array[start] : 0;
				Min = v;
				Sum = v;
			}
		}

		public long GetSumInclusive(int start, int end)
		{
			if (Start >= start && End <= end)
				return Sum;
			if (start > End || end < Start)
				return 0;

			LazyPropagate();
			return Left.GetSumInclusive(start, end) + Right.GetSumInclusive(start, end);
		}

		public long GetMinInclusive(int start, int end)
		{
			if (Start >= start && End <= end)
				return Min;
			if (start > End || end < Start)
				return long.MaxValue;

			LazyPropagate();
			return Min(Left.GetMinInclusive(start, end), Right.GetMinInclusive(start, end));
		}


		public void AddInclusive(int start, int end, STType value)
		{
			if (start > End || end < Start)
				return;

			if (Start >= start && End <= end)
			{
				Add(value);
				return;
			}

			LazyPropagate();
			Left.AddInclusive(start, end, value);
			Right.AddInclusive(start, end, value);
			UpdateNode();
		}

		void Add(STType value)
		{
			Sum += value * Length;
			Min += value;
			LazyAdd += value;
		}

		void LazyPropagate()
		{
			if (Start == End)
				return;

			if (Covering)
			{
				Left.Cover(Min);
				Right.Cover(Min);
				LazyAdd = 0;
				Covering = false;
				return;
			}

			if (LazyAdd != 0)
			{
				var value = LazyAdd;
				LazyAdd = 0;
				Left.Add(value);
				Right.Add(value);
			}
		}

		void UpdateNode()
		{
			var left = Left;
			var right = Right;
			Sum = left.Sum + right.Sum;
			Min = Min(left.Min, right.Min);
		}

		public void CoverInclusive(int start, int end, STType value)
		{
			if (start > End || end < Start)
				return;

			if (Start >= start && End <= end)
			{
				Cover(value);
				return;
			}

			LazyPropagate();
			Left.CoverInclusive(start, end, value);
			Right.CoverInclusive(start, end, value);
			UpdateNode();
		}

		void Cover(STType value)
		{
			Min = value;
			LazyAdd = 0;
			Sum = value * Length;
			Covering = true;
		}

		// TODO: 
		// SOURCE: http://e-maxx.ru/algo/segment_tree
		/*
			int FindKth(int v, int tl, int tr, int k)
			{
				if (k > t[v])
					return -1;
				if (tl == tr)
					return tl;
				int tm = (tl + tr) / 2;
				if (t[v * 2] >= k)
					return FindKth(v * 2, tl, tm, k);
				else
					return FindKth(v * 2 + 1, tm + 1, tr, k - t[v * 2]);
			}
		*/
	}
}
