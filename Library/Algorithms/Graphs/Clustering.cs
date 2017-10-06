using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Softperson.Collections;
using Softperson.Scripting;

namespace Softperson.Algorithms.Graphs
{
	public class Clustering
	{
		public const string algodir = @"D:\Documents\Assts\Algo\";

		private int clusters;
		private readonly Dictionary<int, int> nodes = new Dictionary<int, int>();

		// clustering[12].txt

		public void Run()
		{
			var s = algodir + "clustering1.txt";

			var file = File.OpenText(s);
			var line0 = file.ReadLine();

			int nItems;
			line0.Out(out nItems);

			var list = new List<Edge>(nItems*(nItems + 1)/2);
			foreach (var line in file.ReadLines())
			{
				var edge = new Edge();
				line.Out(out edge.Node1)
					.Out(out edge.Node2)
					.Out(out edge.Cost);
				list.Add(edge);
			}

			list.SortBy(a => a.Cost);

			clusters = nItems;

			foreach (var e in list)
			{
				var n1 = e.Node1;
				var n2 = e.Node2;
				Merge(n1, n2);
				if (clusters == 3)
				{
					Console.WriteLine(e.Cost);
					break;
				}
			}


			Console.WriteLine();
		}

		public void Run2()
		{
			var s = algodir + "clustering2.txt";

			var file = File.OpenText(s);
			var line0 = file.ReadLine();

			int nItems;
			line0.Out(out nItems);

			var bits = new List<int>(nItems);
			foreach (var line in file.ReadLines())
			{
				bits.Add(ToBits(line));
			}

			Debug.Assert(bits.Count == nItems);

			var list = new List<Edge>(nItems*(nItems + 1)/2);
			var dict = new Dictionary<int, List<Edge>>();
			for (var i = 0; i < bits.Count - 1; i++)
				for (var j = i + 1; j < bits.Count; j++)
				{
					var e = new Edge
					{
						Node1 = i,
						Node2 = j,
						Cost = GetCost(bits[i], bits[j])
					};
					if (e.Cost > 4) continue;
					dict.AddMany(e.Cost, e);
				}

			list.AddRange(dict.Sorted(x => x.Key).SelectMany(x => x.Value));
			clusters = nItems;

			foreach (var e in list)
			{
				var n1 = e.Node1;
				var n2 = e.Node2;
				var prevClusters = clusters;
				Merge(n1, n2);
				if (e.Cost == 3)
				{
					Console.WriteLine(clusters);
					Console.WriteLine(e.Cost);
					break;
				}
			}


			Console.WriteLine();
		}

		int GetCost(int b1, int b2)
		{
			int b = b1 ^ b2;
			int cost = 0;
			while (b != 0)
			{
				b = b & (b - 1);
				cost++;
			}
			return cost;
		}

		int ToBits(string line)
		{
			int result = 0;
			int hit = 0;
			foreach(var x in line)
			{
				if (x == '0')
				{
					result <<= 1;
					hit++;
				}
				else if (x == '1')
				{
					result = (result << 1) + 1;
					hit++;
				}
			}

			Debug.Assert(hit == 24);
			return result;
		}

		public bool Merge(int n1, int n2)
		{
			var r1 = GetRoot(n1);
			var r2 = GetRoot(n2);

			if (r1 == r2)
				return false;

			clusters--;
			if (r1 <= r2)
				nodes[r2] = r1;
			else
				nodes[r1] = r2;
			return true;
		}

		public int GetRoot(int i)
		{
			if (!nodes.ContainsKey(i))
				return i;

			int root = nodes[i];
			if (root != i)
			{
				var oldroot = root;
				root = GetRoot(oldroot);
				nodes[i] = root;
			}
			return root;
		}

		public class Edge
		{
			public int Cost;
			public int Node1;
			public int Node2;
		}
	}
}
