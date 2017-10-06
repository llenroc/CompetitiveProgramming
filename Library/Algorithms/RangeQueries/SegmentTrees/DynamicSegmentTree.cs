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

	public class DynamicSegmentTree
	{
		public STType Min;
		public STType Max;
		public STType Sum;
		public STType LazyAdd;
		public int Count;
		public DynamicSegmentTree Left;
		public DynamicSegmentTree Right;
		public bool Covering;

		// public int Start;
		// public int End => Start + Count;

		public DynamicSegmentTree(STType[] array)
			: this(array, 0, array.Length)
		{
		}

		DynamicSegmentTree(STType[]array, int start, int count)
		{
			// Start = start;
			Count = count;
			
			if (count >= 2)
			{
				int mid = count / 2;
				Left = new DynamicSegmentTree(array, start, mid);
				Right = new DynamicSegmentTree(array, start+mid, count-mid);
				UpdateNode();
			}
			else
			{
				var v = array[start];
				Min = v;
				Max = v;
				Sum = v;
			}
		}


		public long GetMin(int start, int count)
		{
			int end = start + count;
			if (start <= 0 && Count <= end)
				return Min;
			if (start >= Count || end <= 0)
				return long.MaxValue;

			LazyPropagate();
			return Min(Left.GetMin(start, count),
				Right.GetMin(start - Left.Count, count));
		}


		public long GetSum(int start, int count)
		{
			int end = start + count;
			if (start <= 0 && Count <= end)
				return Sum;
			if (start >= Count || end <= 0)
				return 0;

			LazyPropagate();
			return Left.GetSum(start, count)
				+ Right.GetSum(start - Left.Count, count);
		}

		public void Add(int start, int count, STType value)
		{
			int end = start + count;
			if (start>=Count || end <= 0)
				return;

			if (start <= 0 && Count <= end)
			{
				Add(value);
				return;
			}

			LazyPropagate();
			Left.Add(start, count, value);
			Right.Add(start - Left.Count, count, value);
			UpdateNode();
		}

		void Add(STType value)
		{
			Sum += value * Count;
			Min += value;
			Max += value;
			LazyAdd += value;
		}


		public void Cover(int start, int count, STType value)
		{
			int end = start + count;
			if (start >= Count || end <= 0)
				return;

			if (start <= 0 && Count <= end)
			{
				Cover(value);
				return;
			}

			LazyPropagate();
			Left.Cover(start, count, value);
			Right.Cover(start-Left.Count, count, value);
			UpdateNode();
		}

		void Cover(STType value)
		{
			Min = value;
			Max = value;
			LazyAdd = 0;
			Sum = value * Count;
			Covering = true;
		}

		void LazyPropagate()
		{
			if (Count <= 1)
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
			Max = Max(left.Max, right.Max);
		}

		public void Div(int start, int count, int value)
		{
			int end = start + count;
			if (value==1 || start >= Count || end <= 0)
				return;

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
				var d1 = Div(Min, value) - Min;
				var d2 = Div(Max, value) - Max;
				if (d1 == d2)
				{
					Add(d1);
					return;
				}
			}

			LazyPropagate();
			Left.Div(start, count, value);
			Right.Div(start - Left.Count, count, value);
			UpdateNode();
		}

		static long Div(long v, long d)
		{
			return (long)Floor(v / (double)d);
		}
	}
}
