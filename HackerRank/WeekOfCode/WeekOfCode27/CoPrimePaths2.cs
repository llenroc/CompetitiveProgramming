using System;
using System.Collections.Generic;
using System.Net.Configuration;
using Softperson.Graphs;

namespace HackerRank.CodeSprint
{
    public partial class CoPrimePaths
    {
        public partial class Solver
        {
            const int MaxNodes = 25010;


            public EulerTour tour;
            int counter;


            int[] counts = new int[10000001];

            long globalCounts = 0;


            void Add(int snode, int at)
            {
                int node = Math.Abs(snode);
                int delta = snode < 0 ? -1 : 1;

                var factors = this.factors[node];
                var products = new List<int>(8);

                foreach (var f in factors)
                {
                    for (int size = products.Count, k = 0; k < size; k++)
                        products.Add(-f * products[k]);
                    products.Add(f);
                }

                foreach (var p in products)
                {
                    var pp = p < 0 ? -p : p;
                    int sgn = p >= 0 ? 1 : -1;

                    globalCounts -= Calculate(counts[pp]);
                    counts[pp] += delta * sgn;
                    globalCounts += Calculate(counts[pp]);
                }
            }

            long Calculate(int n)
            {
                return n*(n - 1)/2;
            }

            public void ProcessQueries(List<Query> queries)
            {
                tour = new EulerTour(tree);

                var mos = new MosAlgorithm
                {
                    Add = (i, b) => Add(tour[i], b ? 1 : 0),
                    Remove = (i, b) => Add(-tour[i], b ? 1 : 0)
                };
                
                foreach (var q in queries)
                {
                    var tree1 = nodes[q.U];
                    var tree2 = nodes[q.V];
                    var lcaTree = TreeGraph.Lca(tree1, tree2);
                    var lca = lcaTree.Id;

                    mos.AddTask(tour[q.U], tour[lca],
                        q, t => q.Answer2 += globalCounts);

                    mos.AddTask(tour[q.V], tour[lca],
                        q, t => q.Answer2 += globalCounts);

                    var pathlength = tree1.Depth + tree2.Depth - 2 * lcaTree.Depth + 1;
                    q.AllPairs = pathlength * (pathlength - 1) / 2;
                    q.Answer1 -= (1 + nodes[lca].Depth) * 2;
                }

                mos.Execute();
            }

        }
    }
}
