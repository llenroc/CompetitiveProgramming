using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.Graphs
{

    // https://www.hackerrank.com/rest/contests/world-codesprint-8/challenges/tree-coordinates/hackers/ACRush_TC/download_solution

    public class LcaConstantTime
    {
        readonly List<int> _value;
        readonly int _size;
        readonly int[] _depth;
        readonly int[][] _g;
        readonly int[] _pos;
        readonly int[,] f;
        readonly int[,] f2;
        readonly int[] _logw;

        public LcaConstantTime(int[][] graph, int root)
        {
            _g = graph;

            int n = _g.Length;
            int lg = BitTools.Log2(n)+1;
            _logw = new int[n];
            _depth = new int[n];

            int p = 0;
            for (int i = 0; i < graph.Length; i++)
            {
                while ((1 << (p + 1)) <= i) p++;
                _logw[i] = p;
            }

            _value = new List<int>(2 * n + 1);
            Dfs(root);
            _size = _value.Count;


            _pos = new int[_size];
            f = new int[lg, _size];
            f2 = new int[lg, _size];

            for (int i = 0; i < _size; i++)
                _pos[_value[i]] = i;

            for (int i = 0; i < _size; i++)
            {
                f[0,i] = _depth[_value[i]];
                f2[0,i] = _value[i];
            }

            for (int k = 1; (1 << k) <= _size; k++)
            {
                int j = (1 << (k - 1));
                for (int i = 0; j < _size; i++, j++)
                    if (f[k - 1,i] < f[k - 1,j])
                    {
                        f[k,i] = f[k - 1,i];
                        f2[k,i] = f2[k - 1,i];
                    }
                    else
                    {
                        f[k,i] = f[k - 1,j];
                        f2[k,i] = f2[k - 1,j];
                    }
            }
        }


        void Dfs(int p)
        {
            _value.Add(p);
            foreach (int x in _g[p])
            {
                _depth[x] = _depth[p] + 1;
                Dfs(x);
                _value.Add(p);
            }
        }

        public int LcaDepth(int a, int b)
        {
            if (a == b) return _depth[a];
            a = _pos[a];
            b = _pos[b];
            if (a > b)
                Utility.Swap(ref a, ref b);

            int w = _logw[b - a + 1];
            return Math.Min(f[w,a], f[w,b - (1 << w) + 1]);
        }

        public int Lca(int a, int b)
        {
            if (a == b) return a;
            a = _pos[a];
            b = _pos[b];
            if (a > b) Utility.Swap(ref a, ref b);
            int w = _logw[b - a + 1];
            b = (b - (1 << w) + 1);
            return f[w,a] < f[w,b] ? f2[w,a] : f2[w,b];
        }

        public int Distance(int a, int b)
        {
            return _depth[a] + _depth[b] - 2 * _depth[Lca(a, b)];
        }

    }
}