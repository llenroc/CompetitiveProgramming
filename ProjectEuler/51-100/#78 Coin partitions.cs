using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Linq;

class Solution {
    const long mod = 1000 * 1000 * 1000 + 7;
    static void Main(String[] args) {
        var t = int.Parse(Console.ReadLine());
        var ns = Enumerable.Range(0,t).Select(x=>int.Parse(Console.ReadLine())).ToArray();
        var max = ns.Max();
        var table = IntegerPartitionTable(max);
        
        foreach(var n in ns)
        {
            Console.WriteLine((table[n] % mod + mod) % mod);
        }
        
    }
    
    static long[] IntegerPartitionTable(int n)
    {
        var table = new long[n + 1];
        var g = new long[1000];
        for(int i=0; i<g.Length;i++)
            g[i] = GeneralizedPentagonalNumber(i);
        
        table[0] = 1;
        for (int i = 1; i < table.Length; i++)
        {
            for (int j = 1; ; j++)
            {
                var p = g[j];
                if (p > i) break;
                var sign = ((j - 1) >> 1 & 1) == 0 ? 1 : -1;
                //if (i<100)
                //    Console.Error.WriteLine($"table[{i}] += table[{i}-{p}] * {sign}");
                table[i] = (table[i] + table[i - p] * sign) % mod;
            }
        }

        return table;
    }
    
    static long GeneralizedPentagonalNumber(long k)
    {
        if (k == 0) return k;
        var kk = (k + 1) >> 1;
        if ((k & 1) == 0) kk = -kk;
        return kk * (3 * kk - 1) / 2;
    }

}