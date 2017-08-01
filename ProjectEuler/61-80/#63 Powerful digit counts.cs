using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
class Solution {
    static void Main(String[] args) {

        int n = int.Parse(Console.ReadLine());
        
        var limit = BigInteger.Pow(10,n);
        var lower = limit / 10;
        for (long i = 1; BigInteger.Pow(i,n)<limit; i++)
        {
            var cube = BigInteger.Pow(i,n);
            if (cube >= lower)
                Console.WriteLine(cube);
        }
    
    }
}