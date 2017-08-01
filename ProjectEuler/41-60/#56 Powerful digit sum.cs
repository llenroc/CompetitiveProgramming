using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
class Solution {
    static void Main(String[] args) {
        int n = int.Parse(Console.ReadLine());
        
        long max = 0;
        for (int a = 1; a<n; a++)
            for (int b=1; b<n; b++)
            {
                var big = BigInteger.Pow(a,b);
                long sum = 0;
                foreach (var ch in big.ToString())
                    sum += ch - '0';
                if (sum > max) max = sum;
            }
        
        Console.WriteLine(max);
    }
}