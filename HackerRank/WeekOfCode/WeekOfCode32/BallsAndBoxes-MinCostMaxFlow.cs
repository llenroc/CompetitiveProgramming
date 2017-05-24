namespace HackerRank.WeekOfCode32.BallsAndBoxes
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using static Solution.FastIO;



	using MaxFlow = Solution.MinCostFlowDense;

	public class Solution
	{
		int n, m;
		int[] A, C;
		int[][] B;
		const int MAX = 100;

		int maxCandy;
		int minCandy = 100;

		public void solve(Stream input, Stream output)
		{
			InitInput(input);

			n = Ni();
			m = Ni();
			A = Ni(n);
			C = Ni(m);
			B = new int[n][];

			for (int i = 0; i < n; i++)
				B[i] = Ni(m);

			//TestSample();

			long ans = solve();

			var writer = new StreamWriter(output);
			writer.WriteLine(ans);
			writer.Flush();

#if DEBUG
			Console.Error.WriteLine(Process.GetCurrentProcess().TotalProcessorTime);
#endif
		}

		public void TestSample()
		{
			const int TEST = 20;
			n = TEST;
			m = TEST;
			A = new int[n];
			C = new int[m];
			B = new int[n][];

			var random = new Random();
			for (int i = 0; i < n; i++)
				A[i] = random.Next(1, 101);

			for (int i = 0; i < m; i++)
				C[i] = random.Next(0, 101);

			for (int i = 0; i < n; i++)
			{
				B[i] = new int[m];
				for (int j = 0; j < m; j++)
					B[i][j] = random.Next(0, 1001);
			}
		}

		public long solve()
		{
			// Add edge from start to each n color with color capacity
			// Add edge from each n color to each m box with candies
			// Add negative cost from each extra edge from box

			// Simplify A
			//for (int i = 0; i < n; i++)
			//	A[i] = Math.Min(A[i], m);

			maxCandy = 0;
			minCandy = MAX;
			for (int i = 0; i < n; i++)
			{
				maxCandy = Math.Max(maxCandy, B[i].Max());
				minCandy = Math.Min(minCandy, B[i].Min());
			}

			const int log100 = 7;

			var mx = Math.Max(n, A.Max());
			// we need to reduce the number of vertices
			//var mf = new MinCostMaxFlow(2 + n + m + mx);
			var mf = new MaxFlow(2 + n + m + mx);

			var start = 0;
			var end = 1;

			// From start to color nodes
			for (int i = 0; i < n; i++)
				mf.AddEdge(start, ColorNode(i), A[i]);

			// From color to boxes
			for (int i = 0; i < n; i++)
				for (int j = 0; j < m; j++)
					mf.AddEdge(ColorNode(i), BoxNode(j), 1, maxCandy - B[i][j]);

			// From color to no box
			for (int i = 0; i < n; i++)
				mf.AddEdge(ColorNode(i), end, A[i], maxCandy);

			// From boxes to end node
			for (int j = 0; j < m; j++)
				mf.AddEdge(BoxNode(j), end, C[j]);

			// Extra cost
			for (int i = 0; i < mx; i++)
				mf.AddEdge(TargetNode0(i), end, 100000, 0);

			for (int j = 0; j < m; j++)
			{
				for (int i = C[j]; i < n; i++)
				{
					var d = i - C[j];

					// Break if cost will exceed any benefit
					int cost = 2 * d + 1;
					if (cost >= maxCandy) break;
					mf.AddEdge(BoxNode(j), TargetNode0(d), 1, cost);
				}
			}

			long totcost;
			long flow = mf.GetMaxFlow(start, end, out totcost, A.Sum());

			long candies = flow * maxCandy - totcost;
			return candies;
		}

		public int ColorNode(int i) => 2 + i;

		public int BoxNode(int i) => 2 + n + i;

		public int TargetNode0(int i) => 2 + n + m + i;

		public class MinCostFlowDense
		{
			List<Edge>[] graph;

			class Edge
			{
				public int to, f, cap, cost, rev;

				public Edge(int to, int cap, int cost, int rev)
				{
					this.to = to;
					this.cap = cap;
					this.cost = cost;
					this.rev = rev;
				}
			}

			public MinCostFlowDense(int n)
			{
				graph = new List<Edge>[n];
				for (int i = 0; i < n; i++)
					graph[i] = new List<Edge>();
			}

			public void AddEdge(int s, int t, int cap, int cost = 0)
			{
				graph[s].Add(new Edge(t, cap, cost, graph[t].Count));
				graph[t].Add(new Edge(s, 0, -cost, graph[s].Count - 1));
			}

			public long GetMaxFlow(int s, int t, out long totalCost, int maxf)
			{
				int n = graph.Length;
				int[] prio = new int[n];
				int[] curflow = new int[n];
				int[] prevedge = new int[n];
				int[] prevnode = new int[n];
				int[] pot = new int[n];

				int flow = 0;
				int flowCost = 0;
				while (flow < maxf)
				{
					for (int i = 0; i < prio.Length; i++)
						prio[i] = int.MaxValue;
					prio[s] = 0;
					var finished = new bool[n];
					curflow[s] = int.MaxValue;
					for (int i = 0; i < n && !finished[t]; i++)
					{
						int u = -1;
						for (int j = 0; j < n; j++)
							if (!finished[j] && (u == -1 || prio[u] > prio[j]))
								u = j;
						if (prio[u] == int.MaxValue)
							break;
						finished[u] = true;
						for (int k = 0; k < graph[u].Count; k++)
						{
							Edge e = graph[u][k];
							if (e.f >= e.cap)
								continue;
							int v = e.to;
							int nprio = prio[u] + e.cost + pot[u] - pot[v];
							if (prio[v] > nprio)
							{
								prio[v] = nprio;
								prevnode[v] = u;
								prevedge[v] = k;
								curflow[v] = Math.Min(curflow[u], e.cap - e.f);
							}
						}
					}
					if (prio[t] == int.MaxValue)
						break;
					for (int i = 0; i < n; i++)
						if (finished[i])
							pot[i] += prio[i] - prio[t];
					int df = Math.Min(curflow[t], maxf - flow);
					flow += df;
					for (int v = t; v != s; v = prevnode[v])
					{
						Edge e = graph[prevnode[v]][prevedge[v]];
						e.f += df;
						graph[v][e.rev].f -= df;
						flowCost += df * e.cost;
					}
				}

				totalCost = flowCost;
				return flow;
			}

		}


		public static class FastIO
		{
			#region  Input

			static System.IO.Stream inputStream;
			static int inputIndex, bytesRead;
			static byte[] inputBuffer;
			static System.Text.StringBuilder builder;
			const int MonoBufferSize = 4096;

			public static void InitInput(System.IO.Stream input = null, int stringCapacity = 16)
			{
				builder = new System.Text.StringBuilder(stringCapacity);
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
				if (neg)
				{
					c = Read();
				}

				int number = c - '0';
				while (true)
				{
					var d = Read() - '0';
					if ((uint)d > 9) break;
					number = number * 10 + d;
				}
				return neg ? -number : number;
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

			#endregion

		}

		public static class Parameters
		{
#if DEBUG
			public const bool Verbose = true;
#else
	public const bool Verbose = false;
#endif
		}
	}

	class CaideConstants
	{
		public const string InputFile = null;
		public const string OutputFile = null;
	}
	public class Program
	{
		public static void Main(string[] args)
		{
			Solution solution = new Solution();
			solution.solve(Console.OpenStandardInput(), Console.OpenStandardOutput());

#if DEBUG
			Console.Error.WriteLine(System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime);
#endif
		}
	}


}