using System.Collections.Generic;

namespace Softperson.Algorithms.Graphs
{
	public class TreeGraph
	{
		#region Variables
		public int[] Parents;
		public int[] Queue;
		public int[] Depths;
		public int[] Sizes;
		public IList<int>[] Graph;
		public int Root;
		public int TreeSize;
		public int Separator;

		bool sizesInited;
		#endregion

		#region Constructor
		public TreeGraph(IList<int>[] g, int root=0, int avoid=-1)
		{
			Graph = g;
			if (root >= 0)
				Init(root, avoid);
		}
		#endregion

		#region Methods
		public void Init(int root, int avoid=-1)
		{
			var g = Graph;
			int n = g.Length;
			Root = root;
			Separator = avoid;

			Queue = new int[n];
			Parents = new int[n];
			Depths = new int[n];

			for (int i = 0; i < Parents.Length; i++)
				Parents[i] = -1;

			Queue[0] = root;

			int treeSize = 1;
			for (int p = 0; p < treeSize; p++)
			{
				int cur = Queue[p];
				var par = Parents[cur];
				foreach (var child in g[cur])
				{
					if (child != par && child != avoid)
					{
						Queue[treeSize++] = child;
						Parents[child] = cur;
						Depths[child] = Depths[cur] + 1;
					}
				}
			}

			TreeSize = treeSize;
		}

		public void InitSizes()
		{
			if (sizesInited)
				return;

			if (Sizes == null)
				Sizes = new int[Graph.Length];
			sizesInited = true;

			Sizes[Separator] = 0;
			for (int i = TreeSize - 1; i >= 0; i--)
			{
				int current = Queue[i];
				Sizes[current] = 1;
				foreach (int e in Graph[current])
					if (Parents[current] != e)
						Sizes[current] += Sizes[e];
			}
		}
		#endregion


	}
}