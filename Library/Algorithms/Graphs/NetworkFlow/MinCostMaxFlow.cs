using System;
using System.Collections.Generic;

namespace Softperson.Algorithms.Graphs
{
    // Implementation of Min cost Max flow algorithm using adjacency
    // matrix (Edmonds and Karp 1972).  This implementation keeps track of
    // forward and reverse edges separately (so you can set cap[i][j] !=
    // cap[j][i]).  For a regular Math.Max flow, set all edge costs to 0.
    //
    // Running time, O(|V|^2) cost per augmentation
    //     Max flow:           O(|V|^3) augmentations
    //     Min cost Max flow:  O(|V|^4 * MAX_EDGE_COST) augmentations
    //     
    // INPUT: 
    //     - graph, constructed using AddEdge()
    //     - source
    //     - sink
    //
    // OUTPUT:
    //     - (maximum flow value, minimum cost value)
    //     - To obtain the actual flow, look at positive values only.

    public class MinCostMaxFlow
    {
        const long Inf = long.MaxValue / 4;
        readonly long[,] _cap;
        readonly long[,] _cost;
        readonly Tuple<int, int>[] _dad;
        readonly long[] _dist;
        readonly long[,] _flow;
        readonly bool[] _found;

        readonly int _n;
        readonly long[] _pi;
        readonly long[] _width;

        public MinCostMaxFlow(int N)
        {
            _n = N;
            _cap = new long[N, N];
            _flow = new long[N, N];
            _cost = new long[N, N];
            _found = new bool[N];
            _dist = new long[N];
            _pi = new long[N];
            _width = new long[N];
            _dad = new Tuple<int, int>[N];
        }

        public void AddEdge(int from, int to, long cap, long cost = 0)
        {
            _cap[from, to] = cap;
            _cost[from, to] = cost;
        }

        private void Relax(int s, int k, long cap, long cost, int dir)
        {
            var val = _dist[s] + _pi[s] - _pi[k] + cost;
            if (cap != 0 && val < _dist[k])
            {
                _dist[k] = val;
                _dad[k] = new Tuple<int, int>(s, dir);
                _width[k] = Math.Min(cap, _width[s]);
            }
        }

        private long Dijkstra(int s, int t)
        {
            _found.Fill(false);
            _dist.Fill(Inf);
            _width.Fill(0);
            _dist[s] = 0;
            _width[s] = Inf;

            while (s != -1)
            {
                var best = -1;
                _found[s] = true;
                for (var k = 0; k < _n; k++)
                {
                    if (_found[k]) continue;
                    Relax(s, k, _cap[s, k] - _flow[s, k], _cost[s, k], 1);
                    Relax(s, k, _flow[k, s], -_cost[k, s], -1);
                    if (best == -1 || _dist[k] < _dist[best]) best = k;
                }
                s = best;
            }

            for (var k = 0; k < _n; k++)
                _pi[k] = Math.Min(_pi[k] + _dist[k], Inf);
            return _width[t];
        }

        public long GetMaxFlow(int s, int t, out long totcost)
        {
            long totflow = 0;
            totcost = 0;
            long amt;
            while ((amt = Dijkstra(s, t)) != 0)
            {
                totflow += amt;
                for (var x = t; x != s; x = _dad[x].Item1)
                {
                    if (_dad[x].Item2 == 1)
                    {
                        _flow[_dad[x].Item1, x] += amt;
                        totcost += amt * _cost[_dad[x].Item1, x];
                    }
                    else
                    {
                        _flow[x, _dad[x].Item1] -= amt;
                        totcost -= amt * _cost[x, _dad[x].Item1];
                    }
                }
            }
            return totflow;
        }
    }

}