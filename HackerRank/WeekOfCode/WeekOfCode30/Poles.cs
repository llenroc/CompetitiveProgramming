namespace HackerRank.WeekOfCode30.Poles
{
    using System;
    using static System.Math;
    using static System.Console;

    public static class Solution
    {
        #region Variables
        static int[] x;
        static int[] w;
        static long[,] cache;
        #endregion

        public static void Main()
        {
            var input = ReadLine().Split();
            int n = int.Parse(input[0]);
            int k = int.Parse(input[1]);

            x = new int[n];
            w = new int[n];
            cache = new long[n + 1, k + 1];

            for (int i = 0; i < n; i++)
            {
                input = ReadLine().Split();
                x[i] = int.Parse(input[0]);
                w[i] = int.Parse(input[1]);
            }

            const long MaxCost = long.MaxValue >> 15;

            for (int kk = 1; kk <= k; kk++)
            {
                for (int nn = kk + 1; nn <= n; nn++)
                {
                    long rollCost = 0;
                    long rollsCost = 0;
                    long minCost = MaxCost;
                    long xprev = x[nn - 1];
                    for (int i = n - 1; i >= k - 1; i--)
                    {
                        rollsCost += rollCost * (xprev - x[i]);
                        long cost = cache[i, k-1] + rollsCost;
                        if (cost < minCost) minCost = cost;
                        rollCost += w[i];
                        xprev = x[i];
                    }
                    cache[nn, kk] = minCost;
                }
            }

            WriteLine(cache[n, k]);
        }

    }
}