using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Softperson.Mathematics;

namespace HackerRank.CodeSprint
{
    public partial class CoPrimePaths
    {

        public static void Main2()
        {
            // 1 <= n <= 25,000
            // 1 <= nodeValues <= 10,000,000

            var q = 25000;
            List<int> values;
            int[][] edges;
            int n;
            CreateSampleEdges(out edges, out values, out n);
            values.Insert(0, 0);

            var time = DateTime.Now;
            var random = new Random();
            var queries = new List<Query>();
            for (int i = 0; i < q; i++)
            {
                queries.Add(new Query
                {
                    Index = i,
                    U = random.Next(1, n),
                    V = random.Next(1, n),
                });
            }

            var solver = new Solver(values.ToArray(), edges);
            CoPrimePaths.ProcessQueries(solver, queries);
            Console.WriteLine(DateTime.Now - time);

        }

        public static void CreateSampleEdges(out int[][] edges, out List<int> values, out int n)
        {

            var factors = Factorization.PrimeFactorsUpTo(10 * 1000 * 1000);
            n = 25000;
            var elist = new List<int[]>(n - 1);
            values = new List<int>(n - 1);


            int vertex = 2;
            for (int i = 0; i < 4; i++)
            {
                int prev = i + 1;
                for (int j = 0; j < 1000; j++)
                {
                    elist.Add(new[] { prev, vertex });
                    prev = vertex++;
                }
            }

            var random = new Random();
            while (vertex <= n)
            {
                elist.Add(new[] { random.Next(1, vertex - 1), vertex++ });
            }

            while (values.Count < n)
            {

                var v = random.Next(1, 10 * 1000 * 1000);
                if (Factorization.PrimeFactorsOf(factors, v).Count() > 3)
                    continue;

                values.Add(v);
            }
            edges = elist.ToArray();
        }


        public partial class Solver
        {


#if false

            public long QueryOld(int u, int v)
            {
                var tree1 = nodes[u];
                var tree2 = nodes[v];
                var parent = Tree.Lca(tree1, tree2);

                var treePaths = new List<TreePath>();
                Action<Tree, Tree> action = (t1, t2) =>
                {
                    treePaths.Add(new TreePath { Tree1 = t1, Tree2 = t2 });
                };

                hld.QueryUp(tree1, parent, action);
                hld.QueryUp(tree2, parent, action);

                treePaths[treePaths.Count - 1].ExcludeParent = true;

                // Note: The very last path will include the parent a second time

                // Now find a way to make use of these chains

                return ProcessWork(_work);
            }

            public long Query(int u, int v)
            {
                var tree1 = nodes[u];
                var tree2 = nodes[v];
                var parent = Tree.Lca(tree1, tree2);


                return ProcessWork(_work);
            }

            public long ProcessWork(List<int> work)
            {
                long count = 0;
                var products = new List<int>(8);

                //Console.WriteLine(string.Join(",", work.Select(x=>x+"["+nodeValues[x]+"]")));
                for (int i = 0; i < work.Count; i++)
                {
                    var w = work[i];
                    var factors = this.factors[w];

                    products.Clear();
                    foreach (var f in factors)
                    {
                        for (int size = products.Count, k = 0; k < size; k++)
                            products.Add(-f * products[k]);
                        products.Add(f);
                    }

                    long tmpCount = i;
                    foreach (var p in products)
                    {
                        tmpCount += p >= 0
                            ? -_factorCounts[p]++
                            : _factorCounts[-p]++;
                    }

                    //Console.WriteLine($"{i}) w={w} nv={nv} count+{tmpCount} factors={string.Join(",",factors)} products={string.Join(",",products)}");
                    count += tmpCount;
                }

                return count;
            }

            public long BruteForce(Tree tree1, Tree tree2)
            {
                _work.Clear();

                if (tree1.Depth < tree2.Depth)
                    Swap(ref tree1, ref tree2);

                int depth = tree2.Depth;
                while (tree1.Depth > depth)
                {
                    _work.Add(tree1.Id);
                    tree1 = tree1.Parent;
                }

                while (tree1 != tree2)
                {
                    _work.Add(tree1.Id);
                    tree1 = tree1.Parent;
                    _work.Add(tree2.Id);
                    tree2 = tree2.Parent;
                }

                _work.Add(tree1.Id);

                return ProcessWork(_work);
            }

        }

#endif

      

 



            public class SegmentTree
            {
                private readonly int[] _tree;

                public SegmentTree(int size, bool powerOf2 = false)
                {
                    if (powerOf2)
                        size = LeastPowerOfTwo(size);
                    _tree = new int[size*2];
                }

                public SegmentTree(int[] array, bool powerOf2 = false)
                    : this(array.Length, powerOf2)
                {
                    int size = array.Length;
                    Array.Copy(array, 0, _tree, size, array.Length);
                    for (int i = size - 1; i > 0; i--)
                        _tree[i] = _tree[i << 1] + _tree[i << 1 | 1];
                }

                private int LeastPowerOfTwo(int size)
                {
                    int bits = size;
                    while ((bits & bits - 1) != 0)
                        bits &= bits - 1;
                    if (size > bits) bits <<= 1;
                    return bits;
                }

                public void Modify(int index, int value)
                {
                    int i = index + _tree.Length/2;
                    _tree[i] = value;
                    for (; i > 1; i >>= 1)
                        _tree[i >> 1] = _tree[i] + _tree[i ^ 1];

                }


                public int QueryInclusive(int left, int right)
                {
                    int size = _tree.Length/2;
                    left += size;
                    right += size;

                    int result = 0;
                    for (; left <= right; left >>= 1, right >>= 1)
                    {
                        if (left%2 == 1)
                            result += _tree[left++]; // if parent is the left child, then parents have the sum
                        if (right%2 == 0)
                            result += _tree[right--]; // if parent is the right child, then parents have the sum
                    }
                    return result;
                }

                public void RangeModify(int start, int count, int value)
                {
                    int size = _tree.Length/2;
                    int left = start + size;
                    int right = start + count + size; // open border
                    for (; left < right; left >>= 1, right >>= 1)
                    {
                        if (left%2 == 1) _tree[left++] += value;
                        if (right%2 == 1) _tree[--right] += value;
                    }
                }

                public int RangeQuery(int index)
                {
                    int res = 0;
                    int i = index + _tree.Length/2;
                    for (; i > 0; i >>= 1)
                        res += _tree[i];
                    return res;
                }

                public override string ToString()
                {
                    var sb = new StringBuilder();
                    foreach (int t in _tree)
                    {
                        sb.Append(t);
                        sb.Append(',');
                    }
                    return sb.ToString();
                }
            }


        }
    }
}
