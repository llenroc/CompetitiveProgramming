using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerRank.Algorithms;
using HackerRank.Graphs;
using HackerRank.Expressions;

namespace HackerRank.CodeSprint
{
    public partial class CoPrimePaths
    {
        #region Main
        public static void Main()
        {
            Console.SetIn(File.OpenText(@"d:\test\hr\coprimepaths.txt"));
            var tokens_n = Console.ReadLine().Split();
            int n = int.Parse(tokens_n[0]);
            int q = int.Parse(tokens_n[1]);

            var values = new List<int> { 0 };
            values.AddRange(Array.ConvertAll(Console.ReadLine().Split(), int.Parse));
            var edges = Enumerable.Range(0, n - 1).Select(x => Array.ConvertAll(Console.ReadLine().Split(), int.Parse)).ToArray();

            // 1 <= n <= 25,000
            // 1 <= nodeValues <= 10,000,000

            var queries = new List<Query>();
            for (int i = 0; i < q; i++)
            {
                // 1 <= u,v <= n
                var arr = Console.ReadLine().Split().Select(int.Parse).ToArray();
                queries.Add(new Query
                {
                    Index = i,
                    U = arr[0],
                    V = arr[1],
                });
            }

            var solver = new Solver(values.ToArray(), edges);
            ProcessQueries(solver, queries);
        }

        public static void ProcessQueries(Solver solver, List<Query> queries)
        {
            solver.ProcessQueries(queries);
            foreach (var q in queries)
                Console.WriteLine($" {q.AllPairs} - {q.Answer}= {-q.Answer + q.AllPairs}");
        }


        #endregion

        public partial class Solver
        {
            #region Variables

            int nodeCount;
            private int m;
            int[][] factors;
            public TreeGraph[] nodes;
            public TreeGraph tree;
            public List<List<int>> edges;
            private readonly List<int> _work = new List<int>();


            private long[,] dp;

            #endregion

            public Solver(int[] values, int[][] edges)
            {
                int max = values.Max();

                this.nodeCount = values.Length - 1;
                this.m = (int)Math.Ceiling(Math.Sqrt(nodeCount));
                this.factors = new int[nodeCount + 1][];
                _work.Capacity = nodeCount + 1;

                var list = new List<int>();
                var primeFactors = PrimeFactorsUpTo(max);
                for (int i = 1; i <= nodeCount; i++)
                {
                    list.Clear();
                    list.AddRange(PrimeFactorsOf(primeFactors, values[i]));
                    this.factors[i] = list.ToArray();
                }

                this.edges = BuildEdges(edges, nodeCount);
                var root = CentroidDecomposition.FindCenterOfTree(this.edges, nodeCount);
                this.nodes = TreeGraph.Build(this.edges, root);
                this.tree = nodes[root];
            }


            public List<List<int>> BuildEdges(int[][] edges, int n)
            {
                var list = new List<List<int>>(n + 1) { null };
                list.AddRange(Enumerable.Range(1, n).Select(x => new List<int>()));
                foreach (var e in edges)
                {
                    list[e[0]].Add(e[1]);
                    list[e[1]].Add(e[0]);
                }
                return list;
            }
            
        }


        #region Helpers

        #region Prime Factors

        /// <summary>
        /// Returns a list of prime factors for each number up to n.
        /// Space and time complexity is O(n lg(n)).
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="ignorePrimes">if set to <c>true</c> ignore primes.</param>
        /// <returns></returns>
        public static int[] PrimeFactorsUpTo(int n)
        {
            var factors = new int[n + 1];

            for (int i = 2; i <= n; i += 2)
                factors[i] = 2;

            var sqrt = (int)Math.Sqrt(n);
            for (int i = 3; i <= sqrt; i += 2)
            {
                if (factors[i] != 0) continue;
                for (int j = i * i; j <= n; j += i + i)
                {
                    if (factors[j] == 0)
                        factors[j] = i;
                }
            }

            for (int i = 3; i <= n; i += 2)
            {
                if (factors[i] == 0)
                    factors[i] = i;
            }

            return factors;
        }

        public static IEnumerable<int> PrimeFactorsOf(int[] table, int n)
        {
            int prev = 0;
            int k = n;
            while (k > 1)
            {
                int next = table[k];
                if (next != prev) yield return next;
                k /= next;
                prev = next;
            }
        }

        #endregion

        public static void Swap<T>(ref T x, ref T y)
        {
            var tmp = x;
            x = y;
            y = tmp;
        }

        public class Query
        {
            public int U;
            public int V;
            public int Index;
            public long Answer1;
            public long Answer2;
            public long Answer => Answer1 + Answer2;

            public long AllPairs;

            public override string ToString()
            {
                return $"#{Index} U={U} V={V} Answer={Answer}/{AllPairs}";
            }
        }


        #endregion
    }
}
