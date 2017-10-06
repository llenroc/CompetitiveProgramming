using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.RangeQueries
{
    // Fenwick Tree or Binary Indexed Trees
    // https://prodeportiva.wordpress.com/2013/02/17/arbol-de-fenwick/
    // https://www.hackerearth.com/notes/binary-indexed-tree-or-fenwick-tree/

    public class FenwickTree
    {
		#region Variables
		public readonly long[] A;
		#endregion

		#region Constructor
		/*public Fenwick(int[] a) : this(a.Length)
        {
            for (int i = 0; i < a.Length; i++)
                Add(i, a[i]);
        }*/

		public FenwickTree(long[] a) : this(a.Length)
		{
			int n = a.Length;
			System.Array.Copy(a, 0, A, 1, n);
			for (int k = 2, h = 1; k <= n; k *= 2, h *= 2)
			{
				for (int i = k; i <= n; i += k)
					A[i] += A[i - h];
			}

			//for (int i = 0; i < a.Length; i++)
			//	Add(i, a[i]);
		}

		public FenwickTree(long size)
        {
            A = new long[size + 1];
        }
		#endregion
		
		#region Properties
		public long this[int index] => AltRangeUpdatePointQueryMode ? SumInclusive(index) : SumInclusive(index, index);

		public int Length => A.Length - 1;

	    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	    public long[] Table
	    {
		    get
		    {
			    int n = A.Length - 1;
			    long[] ret = new long[n];
			    for (int i = 0; i < n; i++)
				    ret[i] = SumInclusive(i);
				if (!AltRangeUpdatePointQueryMode)
					for (int i = n - 1; i >= 1; i--)
						ret[i] -= ret[i - 1];
			    return ret;
		    }
	    }
		#endregion


		#region Methods
		// Increments value		
		/// <summary>
		/// Adds val to the value at i
		/// </summary>
		/// <param name="i">The i.</param>
		/// <param name="val">The value.</param>
		public void Add(int i, long val)
        {
			if (val == 0) return;
            for (i++; i < A.Length; i += (i & -i))
                A[i] += val;
        }

		// Sum from [0 ... i]
        public long SumInclusive(int i)
        {
            long sum = 0;
            for (i++; i > 0; i -= (i & -i))
                sum += A[i];
            return sum;
        }

        public long SumInclusive(int i, int j)
        {
            return SumInclusive(j) - SumInclusive(i - 1);
        }

		// get largest value with cumulative sum less than x;
		// for smallest, pass x-1 and add 1 to result
		public int GetIndexGreater(long x)
		{
			int i = 0, n = A.Length - 1;
			for (int bit = BitTools.HighestOneBit(n); bit != 0; bit >>= 1)
			{
				int t = i | bit;

				// if (t <= n && Array[t] < x) for greater or equal 
				if (t <= n && A[t] <= x)
				{
					i = t;
					x -= A[t];
				}
			}
			return i;
		}

		#endregion

		#region Alternative Range Update Point Query Mode  ( cf Standard Point Update Range Query )

		public bool AltRangeUpdatePointQueryMode;

		/// <summary>
		/// Inclusive update of the range [left, right] by value
		/// The default operation of the fenwick tree is point update - range query.
		/// We use this if we want alternative range update - point query.
		/// SumInclusive becomes te point query function.
		/// </summary>
		/// <returns></returns>
		public void AltAddInclusive(int left, int right, long delta)
		{
			Add(left, delta);
			Add(right + 1, -delta);
		}

		public long AltQuery(int index)
		{
			return SumInclusive(index);
		}


		#endregion
	}
}

