using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    
    
    static int n;
    
    static void Main(String[] args) {
        n = int.Parse(Console.ReadLine());
        
        long sum = 0;
        for (int i=0; i<=n; i++)
            sum += Dfs(i, 1L<<i, n);
        Console.WriteLine(sum);
    }
    
    static long[] primes = { 1, 2, 3, 5, 7, 11, 13, 17, 19 };
    
    static long Dfs(long num, long mask, int depth)
    {
        var ndigits = n-depth+1;
        if (ndigits>3 && num % 1000 % primes[ndigits-3] != 0) return 0;
        if (depth == 0) return num;
        
        long sum = 0;
        num *= 10;
        for (int i=0; i<=n; i++)
        {
            if ((mask & 1L<<i) != 0) continue;
            sum += Dfs(num+i, mask|1L<<i, depth-1);
        }
        return sum;
    }
}
