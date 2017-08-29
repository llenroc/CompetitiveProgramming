using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
class Solution {
    static void Main(String[] args) {

        int n = int.Parse(Console.ReadLine());
        for (int i=0; i<n; i++)
        {
            BigInteger sum = 0;
            var big = BigInteger.Pow(2, int.Parse(Console.ReadLine()));
            while (big > 0)
            {
                sum += big % 10;
                big /= 10;
            }
            
            Console.WriteLine(sum);
        }
    
    }
}