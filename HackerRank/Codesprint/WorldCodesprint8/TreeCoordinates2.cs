
namespace HackerRank.WorldCodeSprint8
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using static FastIO;
	class TreeCoordinates2
	{

		static void Main(String[] args)
		{
			LaunchTimer();
			InitIO();
			int n = Ni();
			int m = Ni();
			var edges = N(n - 1, () => Ni(2));
			var points = NL(m, () => Ni(2));

			var result = new TreeCoordinates2(edges, points, n).Solve();
			Console.WriteLine(result);
		}

		TreeCoordinates2(int[][] edges, List<int[]> points, int n)
		{
			this.nodes = new Tree[n + 1];
			this.points = points;
			Build(edges, n);
		}


		private static int maxDist;
		private static System.Threading.Timer timer;

		public static void LaunchTimer()
		{
			// Use a timer to quit the search
			timer = new System.Threading.Timer(
				delegate
				{
					if (maxDist != 0)
					{
						Console.WriteLine(maxDist);
						Environment.Exit(0);
					}
				}, null, 2000, 0);
		}


		private Tree[] nodes;
		private List<int[]> points;

		private int[] ds;

		public int FindBalancedRoot(Dictionary<int, List<int>> graph, int n)
		{
			// Find Balanced Root O(V) -- it gives us good distance metrics
			// -- start from leaves
			// -- last queued item is the root
			var queue = new Queue<int>();
			var degree = new int[n + 1];

			foreach (var pair in graph)
			{
				var v = pair.Key;
				var count = degree[v] = pair.Value.Count;
				if (count != 1) continue;
				degree[v] = 0;
				queue.Enqueue(v);
			}

			int root = -1;
			while (queue.Count > 0)
			{
				root = queue.Dequeue();
				foreach (var v2 in graph[root])
					if (--degree[v2] == 1) // Might still be buggy
					{
						degree[v2] = 0;
						queue.Enqueue(v2);
					}
			}

			return root;
		}


		public static Dictionary<int, List<int>> Clone(Dictionary<int, List<int>> graph)
		{
			var dict = new Dictionary<int, List<int>>(graph.Count);
			foreach (var pair in graph)
				dict[pair.Key] = new List<int>(pair.Value);
			return dict;
		}


		public static int[] CreateDisjointSet(int size, out int[] counts)
		{
			int[] set = new int[size];
			counts = new int[size];
			for (int i = 0; i < size; i++)
			{
				set[i] = i;
				counts[i] = 1;
			}
			return set;
		}


		public static bool Union(int[] ds, int[] counts, int a, int b)
		{
			int ra = Find(ds, a);
			int rb = Find(ds, b);
			if (ra == rb)
				return false;
			ds[rb] = ra;
			counts[ra] += counts[rb];
			return true;
		}

		public static int Find(int[] ds, int a)
		{
			int r = ds[a];
			if (r != a)
				ds[a] = r = Find(ds, r);
			return r;
		}

		public void Build(int[][] edges, int n)
		{
			var graph = BuildEdges(edges, n);
			var root = FindBalancedRoot(graph, n);
			var tree = BuildTree(graph, root);
		}

		public Tree BuildTree(Dictionary<int, List<int>> edges, int parent)
		{
			var queue = new Queue<Tree>();

			var tree = nodes[parent] = new Tree(parent, null);
			queue.Enqueue(tree);

			var children = new List<Tree>();
			while (queue.Count > 0)
			{
				var p = queue.Dequeue();
				children.Clear();
				foreach (var c in edges[p.Id])
				{
					if (nodes[c] != null) continue;

					var child = nodes[c] = new Tree(c, p);
					queue.Enqueue(child);
				}
				p.children = children.ToArray();
			}
			return tree;
		}

		public Dictionary<int, List<int>> BuildEdges(int[][] edges, int n)
		{
			var dict = Enumerable.Range(1, n).ToDictionary(x => x, x => new List<int>());

			foreach (var e in edges)
			{
				dict[e[0]].Add(e[1]);
				dict[e[1]].Add(e[0]);
			}
			return dict;
		}


		public int Distance(int a, int b)
		{
			var p = Tree.Lca(nodes[a], nodes[b]);
			return nodes[a].Depth + nodes[b].Depth - 2 * p.Depth;
		}

		public class Tree
		{
			public static int AncestorCount = 17;
			public int Id;
			public int Depth;
			public Tree[] ancestors;
			public Tree[] children;

			public Tree(int id, Tree parent)
			{
				ancestors = new Tree[AncestorCount];
				if (parent == null)
				{
					for (int i = 0; i < ancestors.Length; i++)
						ancestors[i] = this;
				}
				else
				{
					ancestors[0] = parent;
					for (int i = 1; i < ancestors.Length; i++)
						ancestors[i] = ancestors[i - 1].ancestors[i - 1];
				}

				Id = id;
				Depth = parent != null ? parent.Depth + 1 : 0;
			}

			public static Tree Lca(Tree node1, Tree node2)
			{
				if (node1.Depth < node2.Depth)
					Swap(ref node1, ref node2);


				for (int i = node1.ancestors.Length - 1; i >= 0; i--)
				{
					if (node1.Depth - (1 << i) >= node2.Depth)
						node1 = node1.ancestors[i];
				}

				if (node1 == node2)
					return node1;

				for (int i = node1.ancestors.Length - 1; i >= 0; i--)
				{
					if (node1.ancestors[i] != null && node1.ancestors[i] != node2.ancestors[i])
					{
						node1 = node1.ancestors[i];
						node2 = node2.ancestors[i];
					}
				}

				return node1.ancestors[0];
			}
		}

		public int PointDistance(int[] point1, int[] point2)
		{
			int xdist = Distance(point1[0], point2[0]);
			int ydist = Distance(point1[1], point2[1]);
			var dist = xdist + ydist;
			return dist;
		}

		long Solve()
		{
			points.Sort((a, b) =>
			{
				int depth1 = nodes[a[0]].Depth + nodes[a[1]].Depth;
				int depth2 = nodes[b[0]].Depth + nodes[b[1]].Depth;
				return -depth1.CompareTo(depth2);
			});

			int size = points.Count;
			var random = new Random();

			for (int i = 0; i < size; i++)
			{
				var v = i;
				var newV = v;
				for (int iter = 0; iter < 2; iter++)
				{
					if (iter == 2)
					{
						if (i + 1 > size - 1) break;
						v = random.Next(i + 1, size - 1);
					}

					var p = points[v];
					if (v > i) points[v] = points[--size];

					for (int j = i + 1; j < size; j++)
					{
						var dist = Distance(p[0], points[j][0]) + Distance(p[1], points[j][1]);
						if (dist > maxDist)
						{
							maxDist = dist;
							newV = j;
						}
					}

					if (v == newV)
					{
						if (iter < 2) { iter = 1; continue; }
						break;
					}
					v = newV;
				}
			}

			return maxDist;
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T tmp = a;
			a = b;
			b = tmp;
		}
	}


	public static class FastIO
	{
		static System.IO.Stream stream;
		static int idx, bytesRead;
		static byte[] buffer;
		static System.Text.StringBuilder builder;

		public static void InitIO(
			int stringCapacity = 16,
			int bufferSize = 1 << 20,
			System.IO.Stream input = null)
		{
			builder = new System.Text.StringBuilder(stringCapacity);
			stream = input ?? Console.OpenStandardInput();
			idx = bytesRead = 0;
			buffer = new byte[bufferSize];
		}


		static void ReadMore()
		{
			idx = 0;
			bytesRead = stream.Read(buffer, 0, buffer.Length);
			if (bytesRead <= 0) buffer[0] = 32;
		}

		public static int Read()
		{
			if (idx >= bytesRead) ReadMore();
			return buffer[idx++];
		}


		public static T[] N<T>(int n, Func<T> func)
		{
			var list = new T[n];
			for (int i = 0; i < n; i++) list[i] = func();
			return list;
		}

		public static List<T> NL<T>(int n, Func<T> func)
		{
			var list = new List<T>(n);
			for (int i = 0; i < n; i++) list.Add(func());
			return list;
		}



		public static int[] Ni(int n)
		{
			var list = new int[n];
			for (int i = 0; i < n; i++) list[i] = Ni();
			return list;
		}

		public static long[] Nl(int n)
		{
			var list = new long[n];
			for (int i = 0; i < n; i++) list[i] = Nl();
			return list;
		}

		public static string[] Ns(int n)
		{
			var list = new string[n];
			for (int i = 0; i < n; i++) list[i] = Ns();
			return list;
		}

		public static int Ni()
		{
			var c = SkipSpaces();
			bool neg = c == '-';
			if (neg) { c = Read(); }

			int number = c - '0';
			while (true)
			{
				var d = Read() - '0';
				if ((uint)d > 9) break;
				number = number * 10 + d;
			}
			return neg ? -number : number;
		}

		public static long Nl()
		{
			var c = SkipSpaces();
			bool neg = c == '-';
			if (neg) { c = Read(); }

			long number = c - '0';
			while (true)
			{
				var d = Read() - '0';
				if ((uint)d > 9) break;
				number = number * 10 + d;
			}
			return neg ? -number : number;
		}

		public static char[] Nc(int n)
		{
			var list = new char[n];
			for (int i = 0, c = SkipSpaces(); i < n; i++, c = Read()) list[i] = (char)c;
			return list;
		}

		public static byte[] Nb(int n)
		{
			var list = new byte[n];
			for (int i = 0, c = SkipSpaces(); i < n; i++, c = Read()) list[i] = (byte)c;
			return list;
		}

		public static string Ns()
		{
			var c = SkipSpaces();
			builder.Clear();
			while (true)
			{
				if ((uint)c - 33 >= (127 - 33)) break;
				builder.Append((char)c);
				c = Read();
			}
			return builder.ToString();
		}

		public static int SkipSpaces()
		{
			int c;
			do c = Read(); while ((uint)c - 33 >= (127 - 33));
			return c;
		}
	}
}
