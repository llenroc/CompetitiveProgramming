using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
class Solution {
    
    
    static void Main(String[] args) {
        int t = int.Parse(Console.ReadLine());
        
        var primes = new long[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67 };    
        
        while (t-->0)
        {
            var n = BigInteger.Parse(Console.ReadLine());
            
            BigInteger factor = 1;
            
            foreach(var v in primes)
            {
                if (factor * v >= n)
                    break;
                factor *= v;
            }
            
            Console.WriteLine(factor);
        }
        
    }
}