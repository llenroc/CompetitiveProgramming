using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        int mod = 1000*1000*1000+7;
        int tc = int.Parse(Console.ReadLine());
        while (tc-->0)
        {
            long v = long.Parse(Console.ReadLine());

            long n = (v-1)/2 % mod;
            long term1 = 8 * n % mod * (n+1) % mod * (2*n+1) % mod * 333333336 % mod;
            long term2 = 2 * n % mod * (n+1) % mod;
            long term3 = 4 * n % mod;
            long term = (term1 + term2 + term3 + 1) % mod;

            //Console.Error.WriteLine($"{n} - {term1} - {term2} - {term3} - {term}");
            Console.WriteLine(term);
        }
        
        
    }
}