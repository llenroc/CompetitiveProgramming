using System;


namespace Softperson.Collections
{
	public class DisjointSetPersistent
	{
		PArray<int> _ds;
		int Count;

		public DisjointSetPersistent(UnionFind set)
		{
			_ds = new PArray<int>((int[])set.Array.Clone());
			Count = set.Count;
		}

		public DisjointSetPersistent(int size)
		{
			var array = new int[size];
			for (var i = 0; i < size; i++)
				array[i] = i;

			_ds = new  PArray<int>(array);
			Count = size;
		}

		DisjointSetPersistent(PArray<int> ds, int count)
		{
			Count = count;
			_ds = ds;
		}

		public DisjointSetPersistent Union(int x, int y)
		{
			var rx = Find(x);
			var ry = Find(y);
			return rx == ry ? this : new DisjointSetPersistent(ry > rx ? _ds.Set(rx, ry) : _ds.Set(ry, rx), Count -1);
		}

		public int Find(int x)
		{
			var root = _ds[x];
			if (root == x) return x;

			var f = Find(root);
			_ds = _ds.Set(x, f);
			return f;
		}
	}
}