using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Softperson.Algorithms.Graphs
{
	// SOURCE: https://www.hackerrank.com/rest/contests/w27/challenges/coprime-paths/hackers/uwi/download_solution


	// SOURCE: http://codeforces.com/blog/entry/18369
	// Euler trees are good for path operations, subtree sums, and lca operations

	[PublicAPI]
	public class EulerTour
    {
        public readonly int[] Trace ;
        public readonly int[] Begin ;
        public readonly int[] End ;
	    public readonly int[] Depth;

	    public EulerTour(IList<int>[] g, int root, bool twice = true)
	    {
		    int n = g.Length;
		    Trace = new int[n * (twice ? 2 : 1)];
		    Begin = new int[n];
		    End = new int[n];
		    Depth = new int[n];
		    int t = -1;

		    for (int i = 0; i < n; i++)
			    Begin[i] = -1;

		    var stack = new int[n];
		    var indices = new int[n];
		    int sp = 0;
		    stack[sp++] = root;

		    while (sp > 0)
		    {
			    outer:
			    int current = stack[sp - 1], index = indices[sp - 1];
			    if (index == 0)
			    {
				    ++t;
				    Trace[t] = current;
				    Begin[current] = t;
				    Depth[current] = sp - 1;
			    }

			    var children = g[current];
			    while (index < children.Count)
			    {
				    int child = children[index++];
				    if (Begin[child] == -1)
				    {
					    indices[sp - 1] = index;
					    stack[sp] = child;
					    indices[sp] = 0;
					    sp++;
					    goto outer;
				    }
			    }

			    indices[sp - 1] = index;
			    if (index == children.Count)
			    {
				    sp--;
				    if (twice) Trace[++t] = current;
				    End[current] = t;
			    }
		    }
	    }


		// http://codeforces.com/blog/entry/18369
		// Edges sums on subtrees
		// -----------------------
		// Requres edge tours

		// Subtree sums (vertices)
		// ------------
		// Either normal or once euler tour
		// add(v, x) becomes add(Begin[v], x)
		// get_sum(v) becomes get_sum(Begin[v], End[v]) 

		// Path sums
		// -------------
		// Requires normal euler tour
		// add(v, x) becomes add(Begin[v], x) add(End[v], -x)
		// get_sum_on_path_from_root(v) is get_sum(0, Begin[v])
		// get_sum_on_path requires gsopfr on lca, u and v

		public bool IsBegin(int trace) => Begin[Trace[trace]] == trace;

		public bool IsEnd(int trace) => End[Trace[trace]] == trace;

        public int this[int index] => Trace[index];

    }
}
