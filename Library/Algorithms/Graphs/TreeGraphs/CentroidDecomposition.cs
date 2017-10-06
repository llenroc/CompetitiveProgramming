using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Softperson.Algorithms.Graphs
{
	[PublicAPI]
	public class CentroidDecomposition
	{
		#region Variables
		bool _ignoreLeaf;
		#endregion

		#region Construction
		public CentroidDecomposition(IList<int>[] graph)
		{
			int n = graph.Length;
			Graph = graph;
			Parents = new int[n];
			Queue = new int[n];
			Size = new int[n];
		}
		#endregion

		#region Properties

		// To use Queue and Parents for both PreAction & PostAction
		// BuildQueue must be called with centroid & centroidParent

		public readonly IList<int>[] Graph;
		/// <summary> level order of descendants of currently decomposed subtree </summary>
		public readonly int[] Queue;
		/// <summary> parents of descendants of currently decomposed subtree </summary>
		public readonly int[] Parents;
		/// <summary> depths of descendants of currently decomposed subtree </summary>
		public readonly int[] Size;

		public Action<int, int, int> PreAction;
		public Action<int, int, int> PostAction;

		#endregion

		#region Public Methods
		public void Decompose(int root, bool ignoreLeaf = false)
		{
			_ignoreLeaf = ignoreLeaf;
			Decompose(root, -1);
		}

		public void BuildTree(int centroid, int centroidParent)
		{
			Parents[centroid] = -1;
			//Size[centroid] = 0;
			int treeSize = 1;
			for (int p = 0; p < treeSize; p++)
			{
				int cur = Queue[p];
				//int curDepth = Size[cur];
				foreach (int child in Graph[cur])
				{
					if (Parents[cur] != child && child != centroidParent)
					{
						Queue[treeSize++] = child;
						Parents[child] = cur;
						//Size[child] = curDepth + 1;
					}
				}
			}
		}

		public void BuildDepths(int treeSize)
		{
			Size[0] = 0;
			for (int p = 1; p < treeSize; p++)
				Size[Queue[p]] = Size[Parents[p]] + 1;
		}

		#endregion

		#region Private Methods
		int BuildQueuePrivate(int root, int centroidParent)
		{
			Parents[root] = -1;
			int treeSize = 1;
			for (int p = 0; p < treeSize; p++)
			{
				int cur = Queue[p];
				foreach (int child in Graph[cur])
				{
					if (Parents[cur] != child && child != centroidParent)
					{
						Queue[treeSize++] = child;
						Parents[child] = cur;
					}
				}
			}
			return treeSize;
		}

		void Decompose(int root, int centroidParent)
		{
			int treeSize = BuildQueuePrivate(root, centroidParent);
			if (treeSize == 1 && _ignoreLeaf)
				return;

			int centroid = FindCentroid(treeSize, centroidParent);

			PreAction?.Invoke(centroid, centroidParent, treeSize);

			foreach (int e in Graph[centroid])
			{
				if (e != centroidParent)
					Decompose(e, centroid);
			}

			PostAction?.Invoke(centroid, centroidParent, treeSize);
		}

		int FindCentroid(int treeSize, int centroidParent)
		{
			if (centroidParent>=0) Size[centroidParent] = 0;
			for (int i = treeSize - 1; i >= 0; i--)
			{
				int current = Queue[i];
				Size[current] = 1;
				foreach (int e in Graph[current])
					if (Parents[current] != e)
						Size[current] += Size[e];

				if (Size[current] << 1 >= treeSize)
					return current;
			}
			return Queue[0];
		}
		#endregion
	}
}


