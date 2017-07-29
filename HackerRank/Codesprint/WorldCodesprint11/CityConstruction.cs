namespace HackerRank.WorldCodeSprint11.CityConstruction
{

	// https://www.hackerrank.com/contests/world-codesprint-11/challenges/hackerland/submissions/code/1301797197
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using static FastIO;

	public class Solution
	{
		public static void Main(string[] args)
		{
			InitInput(Console.OpenStandardInput());
			InitOutput(Console.OpenStandardOutput());
			Solution solution = new Solution();
			solution.solve();
		}

		public void solve()
		{
			int n = Ni();
			m = Ni();
			sz = n;

			int[,] edges = new int[m, 2];
			for (int i = 0; i < m; i++)
			{
				edges[i, 0] = Ni() - 1;
				edges[i, 1] = Ni() - 1;
			}

			int q = Ni();
			int[,] qq = new int[q, 3];
			for (int i = 0; i < q; i++)
			{
				qq[i, 0] = Ni();
				qq[i, 1] = Ni();
				qq[i, 2] = Ni();
				if (qq[i, 0] == 1) sz++;
			}

			visited = new int[sz];
			heights = new int[sz];
			group = new int[sz];
			g = new List<int>[sz];
			uconn = new UnionFind(sz);
			for (int i = 0; i < sz; i++)
				g[i] = new List<int>();

			for (int i = 0; i < m; i++)
			{
				int u = edges[i, 0];
				int v = edges[i, 1];
				g[u].Add(v);
				uconn.Union(u, v);
			}

			edges = null;

			int iu = n;
			for (int i = 0; i < q; i++)
			{
				if (qq[i, 0] == 2) continue;
				int u = iu++;
				int x = qq[i, 1] - 1;
				if (qq[i, 2] == 0)
					g[x].Add(u);
				else
					g[u].Add(x);
				uconn.Union(u, x);
			}

			var tarjan = new StronglyConnectedComponents(g, sz);
			tarjan.RunScc();
			dconn = tarjan.uf;

			g2 = new HashSet<int>[sz];
			for (int i = 0; i < sz; i++)
				g2[i] = new HashSet<int>();

			for (int i = 0; i < sz; i++)
			{
				if (g[i].Count == 0) continue;
				var r = dconn.Find(i);
				foreach (var v in g[i])
				{
					var r2 = dconn.Find(v);
					if (r2 != r) g2[r].Add(r2);
				}
			}

			for (int i = 0; i < sz; i++)
				DfsHeight(i);

			iu = n;
			for (int i = 0; i < q; i++)
			{
				if (qq[i, 0] != 2)
				{
					iu++;
					continue;
				}

				bool connected = false;
				int x = qq[i, 1] - 1;
				int y = qq[i, 2] - 1;
				if (x < iu && y < iu && uconn.Find(x) == uconn.Find(y))
				{
					x = dconn.Find(x);
					y = dconn.Find(y);
					if (x == y)
						connected = true;
					else if (group[x] == group[y])
						connected = heights[x] >= heights[y];
					else if (heights[y] < heights[x] && group[x] != group[y])
					{
						connected = Connected(x, y);
					}
				}

				WriteLine(connected ? "Yes" : "No");
			}
			Flush();
		}

		List<int>[] g;
		HashSet<int>[] g2;
		UnionFind uconn;
		UnionFind dconn;
		int[] visited;
		int[] heights;
		int[] group;

		int n, m, sz;

		public int DfsHeight(int u)
		{
			List<int> stack = null;
			if (heights[u] != 0)
				return heights[u];

			while (g2[u].Count == 1)
			{
				if (stack == null) stack = new List<int>();
				stack.Add(u);
				u = g2[u].First();
			}

			int height = 0;
			foreach (var c in g2[u])
			{
				height = Math.Max(DfsHeight(c), height);
			}
			height += 1;

			heights[u] = height;
			group[u] = u;
			if (stack != null)
			{
				for (int i = stack.Count - 1; i >= 0; i--)
				{
					var p = stack[i];
					heights[p] = ++height;
					group[p] = u;
				}
			}

			return height;
		}

		int time = 0;

		public bool Connected(int x, int y)
		{
			time++;
			return Dfs(x, y);
		}

		public bool Dfs(int x, int y)
		{
			if (x == y)
				return true;

			if (visited[x] == time)
				return false;

			visited[x] = time;

			foreach (var c in g2[x])
				if (Dfs(c, y))
					return true;

			return false;
		}

	}

	public class StronglyConnectedComponents
	{

		int _index;
		public VData[] Vertices;
		public UnionFind uf;
		readonly Stack<int> _set = new Stack<int>();

		public struct VData
		{
			public int Index;
			public int LowLink;
			public bool Pushed;
			public bool Visited;
		}

		List<int>[] g;
		int length;
		bool dirty;

		public StronglyConnectedComponents(List<int>[] g, int length)
		{
			this.g = g;
			this.uf = new UnionFind(length);
			this.length = length;
			Vertices = new VData[length];
		}

		struct State
		{
			public int Node;
			public int Parent;
			public int Vertex;
			public int Index;
		}

		public void RunScc()
		{
			if (dirty)
			{
				Array.Clear(Vertices, 0, Vertices.Length);
				_set.Clear();
				_index = 0;
			}

			for (int vv = 0; vv < length; vv++)
				if (Vertices[vv].Visited == false)
				{
					StrongConnect(g, vv);

				}

			dirty = true;
		}

		void StrongConnect(List<int>[] g, int v)
		{
			Vertices[v].Index = _index;
			Vertices[v].LowLink = _index;
			Vertices[v].Pushed = true;
			Vertices[v].Visited = true;
			_index++;
			_set.Push(v);

			foreach (var w in g[v])
			{
				if (Vertices[w].Visited == false)
				{
					StrongConnect(g, w);
					Vertices[v].LowLink = Math.Min(Vertices[v].LowLink, Vertices[w].LowLink);
				}
				else if (Vertices[w].Pushed)
				{
					Vertices[v].LowLink = Math.Min(Vertices[v].LowLink, Vertices[w].Index);
				}
			}

			if (Vertices[v].LowLink == Vertices[v].Index)
			{
				int w;
				do
				{
					w = _set.Pop();
					Vertices[w].Pushed = false;
					uf.Union(v, w);
				} while (w != v);
			}
		}

	}

	public class UnionFind
	{
		public readonly int[] Parents;

		public UnionFind(int size)
		{
			Parents = new int[size];
			for (int i = 0; i < Parents.Length; i++)
				Parents[i] = -1;
		}

		public void Union(int x, int y)
		{
			var rx = Find(x);
			var ry = Find(y);
			if (rx == ry) return;

			if (Parents[rx] <= Parents[ry])
			{
				Parents[rx] += Parents[ry];
				Parents[ry] = rx;
			}
			else
			{
				Parents[ry] += Parents[rx];
				Parents[rx] = ry;
			}
			return;
		}

		public int Find(int x)
		{
			var root = Parents[x];
			return root < 0
				? x
				: (Parents[x] = Find(root));
		}


	}



	public static class FastIO
	{
		#region  Input
		static System.IO.Stream inputStream;
		static int inputIndex, bytesRead;
		static byte[] inputBuffer;
		const int MonoBufferSize = 4096;

		public static void InitInput(Stream input = null)
		{
			inputStream = input ?? Console.OpenStandardInput();
			inputIndex = bytesRead = 0;
			inputBuffer = new byte[MonoBufferSize];
		}

		static void ReadMore()
		{
			inputIndex = 0;
			bytesRead = inputStream.Read(inputBuffer, 0, inputBuffer.Length);
			if (bytesRead <= 0) inputBuffer[0] = 32;
		}

		public static int Read()
		{
			if (inputIndex >= bytesRead) ReadMore();
			return inputBuffer[inputIndex++];
		}

		public static int[] Ni(int n)
		{
			var list = new int[n];
			for (int i = 0; i < n; i++) list[i] = Ni();
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

		public static int SkipSpaces()
		{
			int c;
			do c = Read(); while ((uint)c - 33 >= (127 - 33));
			return c;
		}
		#endregion

		#region Output

		static System.IO.Stream outputStream;
		static byte[] outputBuffer;
		static int outputIndex;

		public static void InitOutput(System.IO.Stream output = null)
		{
			outputStream = output ?? Console.OpenStandardOutput();
			outputIndex = 0;
			outputBuffer = new byte[65535];
			AppDomain.CurrentDomain.ProcessExit += delegate { Flush(); };
		}

		public static void WriteLine(object obj = null)
		{
			Write(obj);
			Write('\n');
		}

		public static void WriteLine(long number)
		{
			Write(number);
			Write('\n');
		}

		public static void Write(long signedNumber)
		{
			ulong number = (ulong)signedNumber;
			if (signedNumber < 0)
			{
				Write('-');
				number = (ulong)(-signedNumber);
			}

			Reserve(20 + 1); // 20 digits + 1 extra
			int left = outputIndex;
			do
			{
				outputBuffer[outputIndex++] = (byte)('0' + number % 10);
				number /= 10;
			}
			while (number > 0);

			int right = outputIndex - 1;
			while (left < right)
			{
				byte tmp = outputBuffer[left];
				outputBuffer[left++] = outputBuffer[right];
				outputBuffer[right--] = tmp;
			}
		}

		public static void Write(object obj)
		{
			if (obj == null) return;

			var s = obj.ToString();
			Reserve(s.Length);
			for (int i = 0; i < s.Length; i++)
				outputBuffer[outputIndex++] = (byte)s[i];
		}

		public static void Write(char c)
		{
			Reserve(1);
			outputBuffer[outputIndex++] = (byte)c;
		}

		static void Reserve(int n)
		{
			if (outputIndex + n <= outputBuffer.Length)
				return;

			Dump();
			if (n > outputBuffer.Length)
				Array.Resize(ref outputBuffer, Math.Max(outputBuffer.Length * 2, n));
		}

		static void Dump()
		{
			outputStream.Write(outputBuffer, 0, outputIndex);
			outputIndex = 0;
		}

		public static void Flush()
		{
			Dump();
			outputStream.Flush();
		}

		#endregion
	}


}