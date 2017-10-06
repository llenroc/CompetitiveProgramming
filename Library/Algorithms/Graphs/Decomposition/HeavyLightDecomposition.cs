using System.Collections.Generic;
using System.Diagnostics;
using Softperson.Algorithms.RangeQueries;
using static System.Math;

namespace Softperson.Algorithms.Graphs
{

    // https://www.hackerrank.com/rest/contests/w27/challenges/how-many-substrings/hackers/uwi/download_solution
    public class HeavyLightDecomposition
    {
        public int[] Chains;
	    public int[] Positions;

		// Head is always at zero
		public int[][] Paths;

	    public TreeGraph Tree;
	    public IList<int>[] Graph;

        public HeavyLightDecomposition(IList<int>[] graph, int root)
        {
			Graph = graph;
			Tree = new TreeGraph(graph, root);
	        Chains = DecomposeToHeavyLight();
	        Paths = BuildPaths();
	        Positions = BuildPositions();
        }

	    int[] DecomposeToHeavyLight()
        {
            int n = Graph.Length;
			var queue = Tree.Queue;
	        var parents = Tree.Parents;

            int[] size = new int[n];
	        for (int i = 0; i < n; i++)
		        size[i] = 1;
            for (int i = n - 1; i > 0; i--) size[parents[queue[i]]] += size[queue[i]];

            int[] chain = new int[n];
	        for (int i = 0; i < n; i++)
		        chain[i] = -1;

			int p = 0;
            foreach (int u in queue)
            {
	            if (chain[u] == -1) chain[u] = p++;
	            int argmax = -1;
	            foreach (int v in Graph[u])
	            {
		            if (parents[u] != v && (argmax == -1 || size[v] > size[argmax]))
			            argmax = v;
	            }
	            if (argmax != -1) chain[argmax] = chain[u];
            }
            return chain;
        }

		int[][] BuildPaths()
        {
            int n = Chains.Length;
            int[] rp = new int[n];
            int sup = 0;
            for (int i = 0; i < n; i++)
            {
                rp[Chains[i]]++;
                sup = Max(sup, Chains[i]);
            }
            sup++;

            int[][] row = new int[sup][];
            for (int i = 0; i < sup; i++) row[i] = new int[rp[i]];

			var queue = Tree.Queue;
            for (int i = n - 1; i >= 0; i--)
            {
				var v = queue[i];
                row[Chains[v]][--rp[Chains[v]]] = v;
            }
            return row;
        }

        int[] BuildPositions()
        {
			int n = Graph.Length;
            int[] iind = new int[n];
            foreach (int[] path in Paths)
            {
                for (int i = 0; i < path.Length; i++)
                    iind[path[i]] = i;
            }
            return iind;
        }

        public int Lca(int x, int y)
        {
            int rx = Paths[Chains[x]][0];
            int ry = Paths[Chains[y]][0];

	        var depth = Tree.Depths;
			var parent = Tree.Parents;
			while (Chains[x] != Chains[y])
            {
                if (depth[rx] > depth[ry])
                {
                    x = parent[rx];
                    rx = Paths[Chains[x]][0];
                }
                else
                {
                    y = parent[ry];
                    ry = Paths[Chains[y]][0];
                }
            }
            return Positions[x] > Positions[y] ? y : x;
        }

        public int Ancestor(int x, int v)
        {
	        var parent = Tree.Parents;
            while (x != -1)
            {
                if (v <= Positions[x]) return Paths[Chains[x]][Positions[x] - v];
                v -= Positions[x] + 1;
                x = parent[Paths[Chains[x]][0]];
            }
            return x;
        }

	    public List<Segment> Query(int u, int v, List<Segment> path=null)
	    {
		    if (path == null) path = new List<Segment>();

		    path.Clear();

		    int lca = Lca(u, v);
		    var lcaChain = Chains[lca];
		    int head;
		    int lcaCount = 0;

		    // u to lca
		    for (var node = u; node != lca; node = Tree.Parents[head])
		    {
			    var chain = Chains[node];

			    if (chain == lcaChain)
			    {
				    path.Add(new Segment(this, lca, node, true ));
				    lcaCount++;
				    break;
			    }

			    head = Paths[chain][0];
			    path.Add(new Segment(this, head, node, true ));
		    }

		    int sav = path.Count;

		    // lca to v
		    for (var node = v; node != lca; node = Tree.Parents[head])
		    {
			    var chain = Chains[node];
			    if (chain == lcaChain)
			    {
				    path.Add(new Segment(this, lca, node, false ));
				    lcaCount++;
				    break;
			    }

			    head = Paths[chain][0];
			    path.Add(new Segment(this, head, node, false));
		    }

		    Debug.Assert(lcaCount < 2);
		    if (lcaCount == 0)
			    path.Add(new Segment(this, lca, lca, false));

		    path.Reverse(sav, path.Count - sav);
		    return path;
	    }

	    public struct Segment
		{
			public HeavyLightDecomposition Hld;

			/// <summary> Top always has lowest index and is an ancestor of node </summary>
			public int Top;
			/// <summary> Node always has higher index and is an descendant of top </summary>
		    public int Node;
		    public bool Up;
			public int Chain;

			public int TopIndex => Hld.Positions[Top];
			public int NodeIndex => Hld.Positions[Node];
			public int[] Paths => Hld.Paths[Chain];

			public Segment(HeavyLightDecomposition hld, int top, int node, bool up) : this()
			{
				Hld = hld;
				Top = top;
				Node = node;
				Up = up;
				Chain = hld.Chains[top];
			}

		    public override string ToString()
		    {
			    var up = Up ? "->" : "<-";
			    return $"{Node + 1} {up} {Top + 1}  ({Node} {up} {Top})";
		    }
	    }
	}
}
