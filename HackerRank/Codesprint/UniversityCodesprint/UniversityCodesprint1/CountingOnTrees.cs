using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Library;

namespace HackerRank.UniversityCodeSprint
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	class CountingOnTrees
	{

		int n;
		int q;
		int[] parent;
		int[] self;
		int[] depth;
		List<int> values;
		int unique = 0;
		Dictionary<int, List<int>> dict;

		static void Main(String[] args)
		{
			new CountingOnTrees().Run();
		}

		public void Run()
		{
			Read();
			Build();

			for (int i = 0; i < q; i++)
			{
				var a = Console.ReadLine().Split().Select(int.Parse).ToArray();
				int w = a[0], x = a[1], y = a[2], z = a[3];
				Console.WriteLine(Query(w, x, y, z));
			}
		}

		public int Query(int w0, int x0, int y0, int z0)
		{
			int w = self[w0], x = self[x0], y = self[y0], z = self[z0];
			var path1 = Path(w, x);
			var path2 = Path(y, z);

			int count = 0;
			int dupes = 0;
			var dict = new Dictionary<int, int>();

			foreach (var e in path1)
			{
				if (path2.Contains(e)) dupes++;
				var v = values[e];
				dict[v] = dict.ContainsKey(v) ? dict[v] + 1 : 1;
			}

			foreach (var e in path2)
			{
				var v = values[e];
				if (dict.ContainsKey(v))
					count += dict[v];
			}

			/*
			Console.WriteLine($"Query ({w},{x},{y},{z})");
			Console.WriteLine("Path 1 - " + string.Join(",", path1));
			Console.WriteLine("Path 2 - " + string.Join(",", path2));
			Console.WriteLine($"Counts({count}) - dupes({dupes}) = {count-dupes}");
			*/

			return count - dupes;
		}

		public HashSet<int> Path(int w, int x)
		{
			var set = new HashSet<int>();
			if (depth[x] < depth[w])
			{
				int tmp = x;
				x = w;
				w = tmp;
			}

			while (depth[x] > depth[w])
			{
				if (values[x] != -1) set.Add(x);
				x = parent[x];
			}

			while (x != w)
			{
				if (values[x] != -1) set.Add(x);
				if (values[w] != -1) set.Add(w);
				x = parent[x];
				w = parent[w];
			}

			if (values[x] != -1) set.Add(x);
			return set;
		}

		public void Read()
		{
			var a = Console.ReadLine().Split().Select(int.Parse).ToArray();
			n = a[0];
			q = a[1];
			values = new List<int> { -1 }; ;
			values.AddRange(Console.ReadLine().Split().Select(int.Parse));
			dict = Enumerable.Range(1, n).ToDictionary(x => x, x => new List<int>());
			for (int i = 1; i < n; i++)
			{
				a = Console.ReadLine().Split().Select(int.Parse).ToArray();
				dict[a[0]].Add(a[1]);
				dict[a[1]].Add(a[0]);
			}

			var counts = new Dictionary<int, int>();
			var remap = new Dictionary<int, int>();
			for (int i = 1; i < values.Count; i++)
			{
				var v = values[i];
				counts[v] = counts.ContainsKey(v) ? counts[v] + 1 : 1;
			}

			for (int i = 1; i < values.Count; i++)
			{
				var v = values[i];
				if (counts[v] < 2)
					values[i] = -1;
				else
				{
					if (!remap.ContainsKey(v))
						remap[v] = remap.Count;
					values[i] = remap[v];
				}
			}

			unique = remap.Count;
		}


		public void Build()
		{
			parent = new int[n + 1];
			self = new int[n + 1];
			depth = Enumerable.Range(0, n + 1).Select(x => -1).ToArray();
			var queue = new Queue<int>();
			queue.Enqueue(1);
			parent[1] = -1;
			self[1] = 1;

			while (queue.Count > 0)
			{
				var pop = queue.Dequeue();
				var p = self[pop];
				foreach (var child in dict[pop])
				{
					if (parent[child] != 0) continue;
					self[child] = child; //(values[child] == -1) ? p : child;
					parent[child] = p;
					depth[child] = depth[p] + 1;
					queue.Enqueue(child);
				}
			}
		}
	}
}
