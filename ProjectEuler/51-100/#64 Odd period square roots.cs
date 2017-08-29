using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        long n = long.Parse(Console.ReadLine());
        
        long count = 0;
        for (int i=2; i<=n; i++)
        {
            long whole = (long) Math.Sqrt(i);
            if (whole * whole == i) continue;
            
            long period = 0;
            long d = 1;
            long m = 0;
            long cf = whole;
            
            Console.Error.WriteLine($"#{i} ~= {whole} x {whole}");
            Console.Error.WriteLine($"m={m} d={d} cf={cf} period={period}");
            while (cf != 2*whole)
            {
                // https://en.wikipedia.org/wiki/Methods_of_computing_square_roots#Continued_fraction_expansion
                m = d*cf - m;
                d = (i - m*m)/d;
                cf = (whole + m)/d;
                period++;
                //Console.Error.WriteLine($"m={m} d={d} cf={cf} = (sqrt({i}) + {m})/{d}  period={period}");
            }
            
            if ((period & 1) == 1)
                count++;
        }
        
        Console.WriteLine(count);
    }
}