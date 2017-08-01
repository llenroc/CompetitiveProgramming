using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using big = System.Numerics.BigInteger;
class Solution {
    static void Main(String[] args) {
        var d = big.Parse(Console.ReadLine());
     
        Console.WriteLine(DiatomicSeries(d+1));
    }
    
    static Dictionary<big,big> cache = new Dictionary<big, big>();
    
    static big Solve(big d)
    {
        if (d<2) return 1;
        if (cache.ContainsKey(d)) return cache[d];
        if (d%2==1) return Solve(d/2);

        big result = Solve((d-2)/2) + Solve(d/2);
        cache[d] = result;
        return result;
    }
    
    static big DiatomicSeries(big n)
    {
        while (n>0 && (n&1)==0) n/=2;
        if (n<2) return n;
        if (cache.ContainsKey(n)) return cache[n];
        var result = DiatomicSeries(n/2) + DiatomicSeries(n/2+1);
        cache[n] = result;
        return result;
    }
}