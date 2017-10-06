using System;
using System.Collections.Generic;
using System.IO;
using Softperson.Scripting;

namespace Softperson.Algorithms.Graphs
{
	public class FloydWarshall
	{

		public static int[,] FindAllPairsShortestPath(IList<Edge> edges, int n)
		{
			var table = new int[n, n];
			for (var i = 0; i < n; i++)
				for (var j = 0; j < n; j++)
					table[i, j] = i == j ? 0 : int.MaxValue;

			foreach (var e in edges)
				table[e.V1, e.V2] = e.Cost;

			return FindAllPairsShortestPath(table);
		}

		public static int[,] FindAllPairsShortestPath(int[,] table)
		{
			int n = table.GetLength(0);
			for (var k = 0; k < n; k++)
			for (var i = 0; i < n; i++)
			for (var j = 0; j < n; j++)
			{
				var d = 0L + table[i, k] + table[k, j];
				if (d <= int.MaxValue)
					table[i, j] = Math.Min(table[i, j], (int)d);
			}
			return table;
		}

		public static int[,] FindAllPairsShortestPath(IList<int>[] edges)
		{
			int n = edges.Length;
			var table = new int[n, n];
			for (var i = 0; i < n; i++)
			for (var j = 0; j < n; j++)
				table[i, j] = i == j ? 0 : int.MaxValue;

			for (var i = 0; i < edges.Length; i++)
				foreach (var v in edges[i])
					table[i, v] = 1;

			return FindAllPairsShortestPath(table);
		}

		public static bool HasNegativeCycles(int[,] table)
		{
			int n = table.GetLength(0);
			for (var i = 0; i < n; i++)
			{
				if (table[i, i] < 0)
					return true;
			}
			return false;
		}

		public static int ShortestPath(int[,] table)
		{
			int n = table.GetLength(0);
			int shortestpath = int.MaxValue;
			for (var i = 0; i < n; i++)
				for (var j = 0; j < n; j++)
					if (i != j) shortestpath = Math.Min(shortestpath, table[i, j]);
			return shortestpath;
		}

		public class Edge
		{
			public int Cost;
			public int V1;
			public int V2;
		}
	}
}
