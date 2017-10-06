using System;
using System.Collections.Generic;
using System.Linq;
using Softperson.Collections;

namespace Softperson.Algorithms.Graphs
{
	public class Kosaraju
	{
		int _t;
		int _sourceVertex;
		readonly VertexData[] _data;

		public Kosaraju(Dictionary<int, HashSet<int>> g)
		{
			_data = new VertexData[g.Count + 1];
			var grev = TarjanSCC.Invert(g);
			DFSLoop(grev);
			DFSLoop2(g);

			var list = _data.Select(x => x?.Count ?? 0).OrderByDescending(x => x).Take(5);
			foreach (var elem in list)
				Console.WriteLine(elem);
		}



		public class VertexData
		{
			public int Count;
			public bool Explored;
			public bool Explored2;
			public int Leader;
			public int Time;
		}

		private int _count;

		public void DFSLoop2(Dictionary<int, HashSet<int>> g)
		{
			var list = new List<int>(ListTools.Range(1, g.Count).OrderByDescending(x => _data[x].Time));
			foreach (var i in list)
			{
				if (_data[i].Explored2)
					continue;
				_count = 0;
				DFS2(g, i);
				_data[i].Count = _count;
			}
		}

		public void DFS2(Dictionary<int, HashSet<int>> g, int i)
		{
			_count++;
			_data[i].Explored2 = true;
			if (g.ContainsKey(i))
				foreach (var j in g[i])
				{
					if (!_data[j].Explored2)
						DFS2(g, j);
				}
		}

		public void DFSLoop(Dictionary<int, HashSet<int>> g)
		{
			for (var i = g.Count; i > 0; i--)
				if (!ContainsKey(i))
				{
					_sourceVertex = i;
					DFS(g, i);
				}
		}

		public void DFS(Dictionary<int, HashSet<int>> g, int i)
		{
			var data = new VertexData { Leader = _sourceVertex };
			_data[i] = data;

			if (g.ContainsKey(i))
				foreach (var j in g[i])
				{
					if (!ContainsKey(j))
						DFS(g, j);
				}
			data.Time = ++_t;
		}

		public bool ContainsKey(int i)
		{
			return _data[i] != null;
		}

#if false
		struct DFSFrame
		{
			public int V;
			public IEnumerator<int> Children;
		}

		IEnumerable<int> GetChildren(Graph g, int v)
		{
			if (g.ContainsKey(v))
			{
				foreach (var i in g[v])
					yield return i;
			}
		}

		public void DFSMain(Graph g, int v, bool Func)
		{
			var stack = new Stack<DFSFrame>();
			var visited = new HashSet<int>();

			var frame = new DFSFrame()
							{
								V=v,
								Children = GetChildren(g,v).GetEnumerator(),
							};

			stack.Push(frame);

			while (stack.Count > 0)
			{
				var f= stack.Pop();
				int i = f.V;
				visited.Add(i);

				if (frame.Children.MoveNext())
				{
					var j = frame.Children.Current;
					stack.Push(f);
					stack.Push(new DFSFrame
					{
						V=j,
						Children = GetChildren(g,j).GetEnumerator(),
					});
				}
			}
		}

#endif
	}
}