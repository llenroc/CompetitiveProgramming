using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using static System.Array;
using static System.Math;

class Solution
{
    static long d, s = 0;
    static long[] cache = new long[3000];

    static long SumOfDivisors2(long z) {
      if (z < cache.Length && cache[z] != 0) 
          return cache[z];

      long s = SumOfDivisors(z);

      if (z < cache.Length) cache[z] = s;
      return(s);
    }

    static long SumOfDivisors(long z)
    {
        long s = z * z;
        long p = 2;
        long d;
        for (d=1; d != 0; d++)
        {
            long n = z / d - z / (d + 1);
            if (n <= 1)
            {
                p = d;
                break;
            }

            long a = z % d;
            s -= (2 * a + (n - 1) * d) * n / 2;
        }

        for (d = 2; d <= z / p; d++)
            s -= z % d;

        return s;
    }

    
    public static void Main() {
      int N = int.Parse(Console.ReadLine());
      int n = (int)Sqrt(N);
      int a = 0, b = 1, c = 1, d = n;

       Console.Error.WriteLine(SumOfDivisors2(5));
        
       while (d > 1) {
         int k = (n + b) / d;
         int aa = a, bb = b;
         a = c; b = d;
         c = k*c-aa;
         d = k*d-bb;

         long dd = N / (a*a + b*b);
         if (dd == 0) continue;
         s += (a+b) * SumOfDivisors2(dd);
       }

      s += SumOfDivisors2(N/2);
      s <<= 1;
      s += SumOfDivisors2(N);

      Console.WriteLine(s);
    }
    
}