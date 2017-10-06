﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.Graphs
{
	public static class GraphConstruction
	{

		public static int[][,] CreateWeightedUndirectedGraph(int n, int[] from, int[] to, int[] w)
		{
			int[][,] g = new int[n][,];
			int[] p = new int[n];
			foreach (int f in from)
				p[f]++;
			foreach (int t in to)
				p[t]++;
			for (int i = 0; i < n; i++)
				g[i] = new int[p[i], 2];
			for (int i = 0; i < from.Length; i++)
			{
				--p[from[i]];
				g[from[i]][p[from[i]], 0] = to[i];
				g[from[i]][p[from[i]], 1] = w[i];
				--p[to[i]];
				g[to[i]][p[to[i]], 0] = from[i];
				g[to[i]][p[to[i]], 1] = w[i];
			}
			return g;
		}

		public static int[][] CreateUndirectedGraph(int n, int[] from, int[] to)
		{
			var g = new int[n][];
			int[] p = new int[n];
			foreach (int f in from)
				p[f]++;
			foreach (int t in to)
				p[t]++;
			for (int i = 0; i < n; i++)
				g[i] = new int[p[i]];
			for (int i = 0; i < from.Length; i++)
			{
				--p[from[i]];
				g[from[i]][p[from[i]]] = to[i];
				--p[to[i]];
				g[to[i]][p[to[i]]] = from[i];
			}
			return g;
		}

		public static int[][] CreateDirectedGraph(int n, int[] from, int[] to)
		{
			var g = new int[n][];
			int[] p = new int[n];
			foreach (int f in from)
				p[f]++;
			for (int i = 0; i < n; i++)
				g[i] = new int[p[i]];
			for (int i = 0; i < from.Length; i++)
			{
				--p[from[i]];
				g[from[i]][p[from[i]]] = to[i];
			}
			return g;
		}


		public static int[][] ParentToChildren(int[] parents)
		{
			int n = parents.Length;
			int[] count = new int[n];
			for (int i = 0; i < n; i++)
				if (parents[i] >= 0)
					count[parents[i]]++;

			int[][] graphs = new int[n][];
			for (int i = 0; i < n; i++)
				graphs[i] = new int[count[i]];

			for (int i = 0; i < n; i++)
				if (parents[i] >= 0)
					graphs[parents[i]][--count[parents[i]]] = i;

			return graphs;
		}

		// TODO: Graph to tree
		// TODO: Tree to graph


		public static List<int>[] ToAdjacencyList(HashSet<int>[] g)
		{
			var list = new List<int>[g.Length];
			for (int i = 0; i < g.Length; i++)
				list[i] = g[i]?.ToList();
			return list;
		}

		public static HashSet<int>[] ToAdjacencyHash(List<int>[] g)
		{
			var hash = new HashSet<int>[g.Length];
			for (int i = 0; i < g.Length; i++)
				hash[i] = new HashSet<int>(g[i]);
			return hash;
		}


		/// <summary>
		/// Finds the Jordan diametric center of tree.
		/// </summary>
		/// <param name="graph">The graph.</param>
		/// <param name="n">The n.</param>
		/// <returns></returns>
		public static int FindJordanCenterOfTree(IList<int>[] graph)
		{
			int n = graph.Length;
			var queue = new Queue<int>();
			var degree = new int[n + 1];

			for (int v = 0; v <= n; v++)
			{
				var value = graph[v];
				if (value == null) continue;
				var count = degree[v] = value.Count;
				if (count != 1) continue;
				degree[v] = 0;
				queue.Enqueue(v);
			}

			int root = -1;
			while (queue.Count > 0)
			{
				root = queue.Dequeue();
				foreach (var v2 in graph[root])
					if (--degree[v2] == 1)
					{
						degree[v2] = 0;
						queue.Enqueue(v2);
					}
			}

			return root;
		}

		public static List<int>[] TransposeGraph(List<int>[] g)
		{
			var g2 = new List<int>[g.Length];
			for (int i = 0; i < g.Length; i++)
			{
				var list = g[i];
				if (list == null) continue;
				foreach (var v in list)
					g2[v].Add(i);
			}
			return g2;
		}


		public static List<int> FindJordanCentersOfTree(IList<int>[] graph)
		{
			int n = graph.Length;
			var degree = new int[n + 1];
			var queue1 = new List<int>();

			for (int v = 0; v <= n; v++)
			{
				var value = graph[v];
				if (value == null) continue;
				var count = degree[v] = value.Count;
				if (count != 1) continue;
				degree[v] = 0;
				queue1.Add(v);
			}

			var queue2 = new List<int>(queue1.Count);
			while (queue1.Count > 0)
			{
				foreach (var v in queue1)
				{
					foreach (var v2 in graph[v])
						if (--degree[v2] == 1)
						{
							degree[v2] = 0;
							queue2.Add(v2);
						}
				}

				if (queue2.Count == 0)
					break;

				queue1.Clear();
				var tmp = queue1;
				queue1 = queue2;
				queue2 = tmp;
			}

			queue1.TrimExcess();
			return queue1;
		}


		const int B1 = 31;
		const int M1 = 1000000007;
		const int Z1 = 16983;
		const int B2 = 37;
		const int M2 = 1000000009;
		const int Z2 = 18657;

		public static long UnrootedHash(int[][] g)
		{
			var cs = FindJordanCentersOfTree(g);
			long[] lhs = new long[cs.Count];
			int p = 0;
			foreach (int c in cs)
			{
				lhs[p++] = RootedHash(c, -1, g);
			}
			Array.Sort(lhs);
			long hl = Z1, hr = Z2;
			foreach (long lh in lhs)
			{
				long lhl = lh >> 32, lhr = (int)lh;
				hl = (hl * B1 + lhl * lhl) % M1;
				hr = (hr * B2 + lhr * lhr) % M2;
			}
			return hl << 32 | hr;
		}

		public static long RootedHash(int cur, int pre, int[][] g)
		{
			long[] hs = new long[g[cur].Length];
			int p = 0;
			foreach (int e in g[cur])
			{
				if (e != pre)
					hs[p++] = RootedHash(e, cur, g);
			}
			Array.Sort(hs, 0, p);
			long hl = Z1, hr = Z2;
			for (int i = 0; i < p; i++)
			{
				hl = (hl * B1 + hs[i] * hs[i]) % M1;
				hr = (hr * B2 + hs[i] * hs[i]) % M2;
			}
			return hl << 32 | hr;
		}

		public static List<int> GraphCenter(IList<int>[] graph)
		{
			int n = graph.Length;
			int[,] D = FloydWarshall.FindAllPairsShortestPath(graph);

			int[] vmax = new int[n];
			for (int i = 0; i < n; i++)
			{
				vmax[i] = 0;
				for (int j = 0; j < n; j++)
				{
					if (vmax[i] < D[i, j])
						vmax[i] = D[i, j];
				}
			}

			var minvmax = int.MaxValue;
			for (int i = 0; i < n; i++)
			{
				if (minvmax < vmax[i])
					minvmax = vmax[i];
			}

			var centers = new List<int>();
			for (int i = 0; i < n; i++)
			{
				if (vmax[i] == minvmax)
					centers.Add(i);
			}

			return centers;
		}



	}
}