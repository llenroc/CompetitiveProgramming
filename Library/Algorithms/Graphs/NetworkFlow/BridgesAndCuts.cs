using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Softperson.Collections;
using static System.Math;

namespace Softperson.Algorithms.Graphs
{
	public class BridgesAndCuts
	{
		#region Using
		IList<int>[] _graph;
		public HashSet<int> CutPoints;
		public List<Bridge> Bridges;
		#endregion

		public BridgesAndCuts(IList<int>[] graph)
		{
			_graph = graph;
			var builder = new Builder(_graph);
			CutPoints = builder.CutPoints;
			Bridges = builder.Bridges;
		}

		public struct Builder
		{
			int[] low;
			int[] num;
			IList<int>[] _graph;
			int curnum;

			public HashSet<int> CutPoints;
			public List<Bridge> Bridges;

			public Builder(IList<int>[] graph)
			{
				int n = graph.Length;
				_graph = graph;
				low = new int[n + 1];
				num = new int[n + 1];

				CutPoints = new HashSet<int>();
				Bridges = new List<Bridge>();

				for (int i = 0; i < 4 * n; i++)
					num[i] = -1;

				curnum = 0;
				for (int i = 0; i < n; i++)
					if (num[i] == -1)
						Dfs(i);
			}

			void Dfs(int u, int p = -1)
			{
				low[u] = num[u] = curnum++;
				int cnt = 0;
				bool found = false;

				for (int i = 0; i < _graph[u].Count; i++)
				{
					int v = _graph[u][i];
					if (num[v] == -1)
					{
						Dfs(v, u);
						low[u] = Min(low[u], low[v]);
						cnt++;
						found = found || low[v] >= num[u];
						if (low[v] > num[u]) Bridges.Add(new Bridge(u, v));
					}
					else if (p != v) low[u] = Min(low[u], num[v]);
				}

				if (found && (p != -1 || cnt > 1))
					CutPoints.Add(u);
			}
		}

		long Combine(int x, int y)
		{
			if (x>y)
			{
				var tmp = x;
				x = y;
				y = tmp;
			}

			return ((long)x << 32) + y;
		}

		public UnionFind GetComponents(bool avoidBridges=true, bool avoidCuts=true)
		{
			int n = _graph.Length;
			var ds = new UnionFind(n);
			var hs = new HashSet<long>();

			if (avoidBridges)
			{
				foreach (var bridge in Bridges)
					hs.Add(Combine(bridge.X, bridge.Y));
			}

			for (int i=0; i<n; i++)
			{
				if (avoidCuts && CutPoints.Contains(i)) continue;
				foreach(var e in _graph[i])
				{
					if (e<i || avoidCuts && CutPoints.Contains(e) || avoidBridges && hs.Contains(Combine(i,e)))
						continue;
					ds.Union(i, e);
				}
			}

			return ds;
		}

		public class Bridge
		{
			public int X;
			public int Y;

			public Bridge(int x, int y)
			{
				X = x;
				Y = y;
			}
		}
	}

}
