using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Softperson;
using static Softperson.BitTools;

namespace Softperson.Algorithms.Graphs
{

	// https://www.hackerrank.com/rest/contests/w27/challenges/coprime-paths/hackers/uwi/download_solution

	public class LowestCommonAncestor
	{
		int[] depth;
		int[,] ancestors;

		public LowestCommonAncestor(TreeGraph builder) : this(builder.Parents, builder.Depths)
		{

		}

		public LowestCommonAncestor(int[] parents, int [] depth)
		{
			this.depth = depth;

			int n = parents.Length;
			int m = NumberOfTrailingZeros(HighestOneBit(n - 1)) + 1;
			var ancestors = new int[n,m]; 

			for (int i=0; i<n; i++)
				ancestors[i,0] = parents[i]; 

			for (int j = 1; j < m; j++)
			{
				for (int i = 0; i < n; i++)
					ancestors[i, j] = ancestors[i, j - 1] == -1 
						? -1 
						: ancestors[ancestors[i, j - 1], j - 1];
			}

			this.ancestors = ancestors;
		}

		public int Lca(int a, int b)
		{
			if (depth[a] < depth[b])
				b = Ancestor(b, depth[b] - depth[a]);
			else if (depth[a] > depth[b])
				a = Ancestor(a, depth[a] - depth[b]);

			/*
			if (depth[a] < depth[b])
				Utility.Swap(ref a, ref b);

			for (var i = ancestors.GetLength(1)-1; i >= 0; i--)
				if (depth[a] - (1 << i) >= depth[b])
					a = ancestors[a, i];
			*/		

			if (a == b)
				return a;
			int sa = a, sb = b;
			for (int low = 0, high = depth[a], 
				t = HighestOneBit(high), k = NumberOfTrailingZeros(t); 
				t > 0; 
				t >>= 1, k--)
			{
				if ((low ^ high) >= t)
				{
					if (ancestors[sa, k] != ancestors[sb, k])
					{
						low |= t;
						sa = ancestors[sa, k];
						sb = ancestors[sb, k];
					}
					else
					{
						high = low | t - 1;
					}
				}
			}
			return ancestors[sa, 0];
		}


		public int Ancestor(int a, int m)
		{
			for (int i = 0; m > 0 && a != -1; m >>= 1, i++)
			{
				if ((m & 1) == 1)
				{
					a = ancestors[a, i];
				}
			}
			return a;
		}

		public int Distance(int a, int b)
		{
			return depth[a] + depth[b] - 2 * depth[Lca(a, b)];
		}
	}
}
