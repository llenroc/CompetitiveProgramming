using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        var input = Array.ConvertAll(Console.ReadLine().Split(), long.Parse);
        var n = input[0];
        var k = input[1];
        
        for (long i=1; ; i++)
        {
            var Pn = i * (3*i - 1) >> 1;
            if (Pn >= n) break;
            
            var Tnd = Math.Sqrt(1+8*Pn);
            
            if (Tnd != Math.Floor(Tnd))
                continue;
            
            var Tn = (long) (Tnd - 1) / 2;
            if (k==3 || (Tn & 1) == 1)
                Console.WriteLine(Pn);
        }
    
    }
}