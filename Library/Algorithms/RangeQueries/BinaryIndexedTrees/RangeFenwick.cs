using System.Diagnostics;

namespace Softperson.Algorithms.RangeQueries
{

	//http://www.geeksforgeeks.org/binary-indexed-tree-range-update-range-queries/

	public class RangeFenwick
	{
		#region Variables
		FenwickTree bit1; // Range Update; Point Query -- x coefficient
		FenwickTree bit2; // Point Update; Range Query -- const
		#endregion

		#region Constructor
		public RangeFenwick(int n)
		{
			bit1 = new FenwickTree(n);
			bit2 = new FenwickTree(n);
		}
		#endregion

		#region Properties
		public int Length => bit1.Length;

		public long this[int index] => QueryInclusive(index, index);

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public long[] Table
		{
			get
			{
				int n = Length;
				long[] result = new long[n];
				for (int i = 0; i < n; i++) result[i] = this[i];
				return result;
			}
		}
		#endregion

		#region Methods

		public void AddInclusive(int left, int right, long v)
		{
			if (left > right) return;
			bit1.AltAddInclusive(left, right, v);
			bit2.Add(left, -v * (left - 1));
			bit2.Add(right + 1, v * right);
		}

		// Range query: Returns the sum of all elements in [1...b]
		public long SumInclusive(int i)
		{
			if (i < 0) return 0;
			return bit1.SumInclusive(i) * i + bit2.SumInclusive(i);
		}

		// Range query: Returns the sum of all elements in [i...j]
		public long QueryInclusive(int i, int j)
		{
			return SumInclusive(j) - SumInclusive(i - 1);
		}

		#endregion
	}
}