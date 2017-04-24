using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Library;

namespace HackerRank.CodeSprint
{
    public unsafe class CountingOnTrees
    {
        static void Main(String[] args)
        {
            new CountingOnTrees().Run();
        }

        int nodeCount;

        public void Run()
        {
            var a = Console.ReadLine().Split().Select(int.Parse).ToArray();
            var queries = a[1];
            nodeCount = a[0];

            Read();

            BuildTree();

            while (queries-- > 0)
            {
                a = Console.ReadLine().Split().Select(int.Parse).ToArray();
                int w = a[0], x = a[1], y = a[2], z = a[3];
                long ans = Query(w, x, y, z);
                Console.WriteLine(ans);
            }
        }

        #region Variables
        const int MaxN = 50000 + 10;
        const int MaxM = 333 + 10;
        const int bsz = 300;

        readonly List<int> _a = new List<int>(MaxN) { -1 };
        int[,] dp = new int[MaxM, MaxN];
        readonly List<int>[] _neighbors = new List<int>[MaxN];
        Dictionary<int, int> _m = new Dictionary<int, int>();

        int _chainNo;
        readonly int[] _chainHead = new int[MaxN];
        readonly int[] _chainInd = new int[MaxN];
        readonly int[] _chainSz = new int[MaxN];
        readonly int[] _subsize = new int[MaxN];
        readonly int[] _is = new int[MaxN];
        readonly int[] _posInBase = new int[MaxN];
        readonly int[,] _lca = new int[MaxN, 22];
        readonly int[] _depth = new int[MaxN];
        readonly int[] _base = new int[MaxN];
        int _lg;
        int _ptr;
        #endregion

        #region Fenwick Tree
        // B1 and B2 are two fenwick trees
        // Original array entries are assumed to be 0
        // and only updates are stored.
        private FenwickTree _b1 = new FenwickTree(MaxN);
        private FenwickTree _b2 = new FenwickTree(MaxN);

        public class FenwickTree
        {
            private long[] _ft;
            private int _n;
            public FenwickTree(int n)
            {
                _ft = new long[n];
                _n = n;
            }


            public long Query(int b)
            {
                long sum = 0;
                for (; b != 0; b -= (b&-b)) sum += _ft[b];
                return sum;
            }

            public void Update(int k, long v)
            {
                for (; k < _n; k += (k & -k))
                    _ft[k] += v;
            }

        }


        // Point query
        // Returns value at position b in the array for ft = B1
        // Returns value to be subtracted from query(B1, b) * b for ft = B2

        // Range query: Returns the sum of all elements in [1...b]
        long Query(int b)
        {
            return _b1.Query(b) * b - _b2.Query(b);
        }

        // Range query: Returns the sum of all elements in [i...j]
        long RangeQuery(int i, int j)
        {
            return Query(j) - Query(i - 1);
        }
        
        // Range update: Adds v to each element in [i...j]
        void RangeUpdate(int i, int j, long v)
        {
            _b1.Update(i, v);
            _b1.Update(j + 1, -v);
            _b2.Update(i, v * (i - 1));
            _b2.Update(j + 1, -v * j);
        }

        #endregion

        int GetBlock(int x)
        {
            return (x - 1) / bsz + 1;
        }

        void Dfs(int u, int parent = -1)
        {
            _lca[u, 0] = parent;
            _depth[u] = parent != -1 ? _depth[parent] + 1 : 1;
            _subsize[u] = 1;
            foreach (var n in _neighbors[u])
            {
                if (parent != n) continue;
                Dfs(n, u);
                _subsize[u] += _subsize[n];
            }
        }

        void HeavyLightDecomposition(int curNode = 1, int parent = -1)
        {
            if (_chainHead[_chainNo] == 0)
                _chainHead[_chainNo] = curNode;

            _chainInd[curNode] = _chainNo;
            _chainSz[_chainNo]++;
            _posInBase[curNode] = ++_ptr;
            _base[_ptr] = curNode;

            int sc = -1;
            foreach (var neighbor in _neighbors[curNode])
            {
                if (neighbor != parent 
                    && (sc == -1 || _subsize[sc] < _subsize[neighbor]))
                    sc = neighbor;
            }

            if (sc != -1)
                HeavyLightDecomposition(sc, curNode);

            foreach (var neighbor in _neighbors[curNode])
            {
                if (neighbor != parent && sc != neighbor)
                {
                    _chainNo++;
                    HeavyLightDecomposition(neighbor, curNode);
                }
            }
        }


        int GetLca(int x, int y)
        {
            if (_depth[x] < _depth[y])
                Swap(ref x, ref y);

            for (int i = _lg; i >= 0; i--)
            {
                if (_lca[x, i] != -1 && _depth[_lca[x, i]] >= _depth[y])
                    x = _lca[x, i];
            }

            if (x == y)
                return x;

            for (int i = _lg; i >= 0; i--)
            {
                if (_lca[x, i] != -1 && _lca[x, i] != _lca[y, i])
                {
                    x = _lca[x, i];
                    y = _lca[y, i];
                }

            }

            return _lca[x, 0];
        }

        void ConstructLca(int n)
        {
            _lg = (int)Math.Ceiling(Math.Log(n, 2));

            int length = _lca.GetLength(1);
            for (int i = 0; i < _lca.GetLength(0); i++)
            for (int j = 0; j < length; j++)
                _lca[i, j] = -1;

            Dfs(1, -1);
            for (int i = 1; i <= _lg; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (_lca[j, i - 1] != -1)
                        _lca[j, i] = _lca[_lca[j, i - 1], i - 1];
                }
            }
        }

        int GetSum(int x1, int x2, int y1, int y2)
        {
            return (int)(dp[x2, y2] - dp[x1 - 1, y2] - dp[x2, y1 - 1] + dp[x1 - 1, y1 - 1]);
        }

        List<Tuple<int, int>>[] Q = new List<Tuple<int, int>>[2];

        void QueryUp(int u, int v, int ind)
        {
            int vchain = _chainInd[v];
            while (true)
            {
                var uchain = _chainInd[u];
                if (uchain == vchain)
                {
                    Q[ind].Add(new Tuple<int, int>(_posInBase[v], _posInBase[u]));
                    break;
                }

                // do query from u chain’s head to u
                Q[ind].Add(new Tuple<int, int>(_posInBase[_chainHead[uchain]], _posInBase[u]));
                u = _chainHead[uchain];
                // promote u to chain’s head and then to its parent
                // update answer
                u = _lca[u, 0];
            }
        }

        public void Read()
        {
            _a.Clear();
            _a.Add(-1);
            _a.AddRange(Console.ReadLine().Split().Select(int.Parse));

            for (int i = 1; i <= nodeCount; i++)
                _m[_a[i]] = 1;

            int cnt = 0;
            foreach (var key in _m.Keys.ToList())
                _m[key] = ++cnt;

            for (int i = 1; i <= nodeCount; i++)
                _a[i] = _m[_a[i]];

            for (int i = 1; i <= nodeCount - 1; i++)
                _neighbors[i] = new List<int>();

            for (int i = 1; i <= nodeCount - 1; i++)
            {
                var a = Console.ReadLine().Split().Select(int.Parse).ToArray();
                int u = a[0], v = a[1];
                _neighbors[u].Add(v);
                _neighbors[v].Add(u);
            }
        }

        public void BuildTree()
        {
            ConstructLca(nodeCount);
            HeavyLightDecomposition();

            for (int i = 1; i <= nodeCount; i++)
                _base[i] = _a[_base[i]];

            for (int i=1; i <= nodeCount; )
            {
                int blk = GetBlock(i);

                for (int j=i; j <= nodeCount && blk == GetBlock(j); j++)
                    _is[_base[j]] += 1;

                for (int j = 1; j <= nodeCount; j++)
                    dp[blk, j] = _is[_base[j]];

                for (; i <= nodeCount && blk == GetBlock(i); i++)
                    _is[_base[i]] = 0;
            }

            int sz = GetBlock(nodeCount);
            for (int i = 1; i <= sz; i++)
            {
                int sum = 0;
                for (int j = 1; j <= nodeCount; j++)
                {
                    dp[i, j] += sum;
                    sum = dp[i, j];
                    dp[i, j] += dp[i - 1, j];
                }
            }
        }

        public long Query(int w, int x, int y, int z)
        {
            var l1 = GetLca(w, x);
            var l2 = GetLca(y, z);
            QueryUp(w, l1, 0);
            QueryUp(x, l1, 0);
            QueryUp(y, l2, 1);
            QueryUp(z, l2, 1);

            foreach (var it in Q[0])
            {
                RangeUpdate(it.Item1, it.Item2, 1);
            }

            RangeUpdate(_posInBase[l1], _posInBase[l1], -1);

            long ans = 0;
            foreach (var it in Q[1])
                ans -= RangeQuery(it.Item1, it.Item2);

            ans += RangeQuery(_posInBase[l2], _posInBase[l2]);

            foreach (var it in Q[0])
            {
                RangeUpdate(it.Item1, it.Item2, -1);
            }

            RangeUpdate(_posInBase[l1], _posInBase[l1], 1);
            // checked O.K

            var tmp = new List<int>();

            foreach (var it1 in Q[0])
            {
                var l = GetBlock(it1.Item1); l++;
                var r = GetBlock(it1.Item2); r--;
                if (l <= r)
                {
                    foreach (var it2 in Q[1])
                    {
                        ans += GetSum(l, r, it2.Item1, it2.Item2);
                        var left = GetBlock(it2.Item1); left++; left = (left - 1) * bsz + 1;
                        var right = GetBlock(it2.Item2); right--; right = Math.Min(nodeCount, right * bsz);
                        if (left <= right)
                            ans -= GetSum(l, r, left, right);
                    }
                    ans -= GetSum(l, r, _posInBase[l2], _posInBase[l2]);
                }
                l = it1.Item1; r = it1.Item2;
                int blk = GetBlock(l);
                while (l <= r && blk == GetBlock(l))
                {
                    tmp.Add(_base[l]);
                    _is[_base[l++]]++;
                }
                blk = GetBlock(r);
                while (l <= r && blk == GetBlock(r))
                {
                    tmp.Add(_base[r]);
                    _is[_base[r--]]++;
                }
            }

            _is[_a[l1]]--;

            foreach (var it1 in Q[1])
            {
                var l = GetBlock(it1.Item1)+1;
                var r = GetBlock(it1.Item2)-1;
                if (l <= r)
                {
                    foreach (var it2 in Q[0])
                        ans += GetSum(l, r, it2.Item1, it2.Item2);
                    ans -= GetSum(l, r, _posInBase[l1], _posInBase[l1]);
                }
                l = it1.Item1; r = it1.Item2;

                int blk = GetBlock(l);
                while (l <= r && blk == GetBlock(l))
                    ans += _is[_base[l++]];

                blk = GetBlock(r);
                while (l <= r && blk == GetBlock(r))
                    ans += _is[_base[r--]];
            }
            ans -= _is[_a[l2]];

            foreach (var it in tmp) _is[it] = 0;
            Q[0].Clear();
            Q[1].Clear();
            tmp.Clear();
            return ans;
        }
    }
}
