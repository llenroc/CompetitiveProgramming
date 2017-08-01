using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        var array = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        int n = array[0];
        int m = array[1];
        var uf = new UnionFind(n+1);
        
        var list = new List<Tuple<int,int,int>>();
        for (int i=0; i<m; i++)
        {
            array = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
            list.Add(new Tuple<int,int,int>(array[2], array[0], array[1]));
        }
        
        list.Sort();
        
        long weight = 0;
        foreach(var e in list)
        {
            if (uf.Union(e.Item2, e.Item3))
                weight += e.Item1;
        }
        
        Console.WriteLine(weight);
    }
    
    public class UnionFind
	{
		readonly int[] _ds;
		int _count;

		public UnionFind(int size)
		{
			_ds = new int[size];
			Clear();
		}

		public int Count => _count;

		public int[] Array => _ds;

		public void Clear()
		{
			_count = _ds.Length;
			for (int i = 0; i < _ds.Length; i++)
				_ds[i] = -1;
		}

		public bool Union(int x, int y)
		{
			var rx = Find(x);
			var ry = Find(y);
			if (rx == ry) return false;

			if (_ds[rx] <= _ds[ry])
			{
				_ds[rx] += _ds[ry];
				_ds[ry] = rx;
			}
			else
			{
				_ds[ry] += _ds[rx];
				_ds[rx] = ry;
			}
			_count--;
			return true;
		}

		public int Find(int x)
		{
			var root = _ds[x];
			return root < 0
				? x 
				: (_ds[x] = Find(root));
		}

		public int GetCount(int x)
		{
			var c = _ds[Find(x)];
			return c >= 0 ? 1 : -c;
		}

		public IEnumerable<int> Roots()
		{
			for (int i = 0; i < _ds.Length; i++)
				if (_ds[i] < 0)
					yield return i;
		}

		public IEnumerable<List<int>> Components()
		{
			var comp = new Dictionary<int, List<int>>();
			foreach (var c in Roots())
				comp[c] = new List<int>(GetCount(c));
			for (int i = 0; i < _ds.Length; i++)
				comp[Find(i)].Add(i);
			return comp.Values;
		}
	}
}