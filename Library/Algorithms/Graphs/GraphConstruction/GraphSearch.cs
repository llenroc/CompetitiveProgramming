using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerRank.DataStructures
{
	public class GraphSearch
	{

		public static int[] TopologicalSort(int[,] g)
		{
			int n = g.GetLength(0);
			int[] indegree = new int[n];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					if (g[i, j] > 0)
						indegree[j]++;
				}
			}

			int[] ret = new int[n];
			int p = 0;
			int q = 0;

			for (int i = 0; i < n; i++)
			{
				bool good = true;
				for (int j = 0; j < n; j++)
				{
					if (g[j, i] > 0)
					{
						good = false;
						break;
					}
				}

				if (good) ret[q++] = i;
			}

			for (; p < q; p++)
			{
				int cur = ret[p];
				for (int i = 0; i < n; i++)
				{
					if (g[cur, i] > 0)
					{
						indegree[i]--;
						if (indegree[i] == 0) ret[q++] = i;
					}
				}
			}
			for (int i = 0; i < n; i++)
			{
				if (indegree[i] > 0)
					return null;
			}
			return ret;
		}


		public static List<int> LexicographicTopologicalSort(List<int>[] g)
		{
			int n = g.Length;
			var counts = new int[n];
			var queue = new SortedSet<int>();
			for (int i = 0; i < n; i++)
			{
				foreach (var v in g[i])
					counts[v]++;
			}

			for (int i = 0; i < n; i++)
			{
				if (counts[i] != 0) continue;
				// Isolated vertex
				if (g[i].Count == 0) continue;
				queue.Add(i);
			}

			var result = new List<int>(n);

			while (queue.Count > 0)
			{
				var min = queue.Min();
				queue.Remove(min);

				foreach (var v in g[min])
				{
					if (--counts[v] == 0)
						queue.Add(v);
				}

				result.Add(min);
			}

			for (int i = 0; i < n; i++)
			{
				if (counts[i] > 0)
					return null;
			}

			return result;
		}

		public static List<int> TopologicalSort(List<int>[] g)
		{
			int n = g.Length;
			var counts = new int[n];
			var queue = new Queue<int>();
			for (int i = 0; i < n; i++)
			{
				foreach (var v in g[i])
					counts[v]++;
			}

			for (int i = 0; i < n; i++)
			{
				if (counts[i] != 0) continue;
				// Isolated vertex
				if (g[i].Count == 0) continue;
				queue.Enqueue(i);
			}

			var result = new List<int>(n);

			while (queue.Count > 0)
			{
				var min = queue.Dequeue();

				foreach (var v in g[min])
				{
					if (--counts[v] == 0)
						queue.Enqueue(v);
				}

				result.Add(min);
			}

			for (int i = 0; i < n; i++)
			{
				if (counts[i] > 0)
					return null;
			}

			return result;
		}

		public class InterativeDfs
		{
			private int[] timestamp;
			private int[] stack;
			private int time;
			private List<int>[] g;

			public InterativeDfs(List<int>[] g)
			{
				this.g = new List<int>[g.Length];
				timestamp = new int[g.Length];
				stack = new int[g.Length];
			}

			void DfsIterative(List<int>[] g, int u, int p, Action<int> action)
			{
				int stackSize = 0;
				time++;

				stack[stackSize++] = u;
				timestamp[u] = time;
				if (p >= 0) timestamp[p] = time;

				while (stackSize > 0)
				{
					var pop = stack[--stackSize];
					foreach (var child in g[pop])
					{
						var v = child;
						if (timestamp[v] >= time) continue;
						timestamp[v] = time;
						action(v);
						stack[stackSize++] = v;
					}
				}
			}

		}


		public static int[] ComputeDistance(IList<int>[] g, int start)
		{
			int n = g.Length;
			int[] d = new int[n];
			for (int i = 0; i < n; i++)
				d[i] = n + 3;

			int[] q = new int[n];
			int p = 0;
			q[p++] = start;
			d[start] = 0;
			for (int r = 0; r < p; r++)
			{
				int cur = q[r];
				foreach (int e in g[cur])
				{
					if (d[e] > d[cur] + 1)
					{
						d[e] = d[cur] + 1;
						q[p++] = e;
					}
				}
			}
			return d;
		}


		public struct FindPath
		{
			public List<int> Path;
			public IList<int>[] Graph;

			public FindPath(IList<int>[] g, int v, int to, bool bfs = true)
			{
				Path = new List<int>();
				Graph = g;
				if (bfs)
					Bfs(v, to);
				else
					Dfs(v, to, -1);
			}

			bool Dfs(int v, int to, int p)
			{
				if (v == to)
				{
					Path.Add(to);
					return true;
				}
				Path.Add(v);
				foreach (int u in Graph[v])
				{
					if (p != u && Dfs(u, to, v))
						return true;
				}
				Path.RemoveAt(Path.Count - 1);
				return false;
			}

			bool Bfs(int v, int to)
			{
				var queue = new Queue<int>();
				var parent = new int[Graph.Length];
				for (int i = 0; i < parent.Length; i++)
					parent[i] = -1;

				queue.Enqueue(v);

				while (queue.Count > 0)
				{
					var pop = queue.Dequeue();
					if (pop == to)
					{
						for (var p = to; p >= 0; p = parent[p])
							Path.Add(p);
						Path.Reverse();
						return true;
					}

					foreach (var c in Graph[pop])
					{
						if (parent[c] >= 0) continue;
						parent[c] = pop;
						queue.Enqueue(c);
					}
				}

				return false;
			}


		}
	}
}
