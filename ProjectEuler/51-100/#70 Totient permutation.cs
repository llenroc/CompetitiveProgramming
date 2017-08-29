using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        int n = int.Parse(Console.ReadLine());
        var factors = PrimeFactorsUpTo(n);
        
        double ratio = double.MaxValue;
        int best = 1;
            
        int start = Math.Max(2, n-2000000);
        for (int i=start; i<n; i++)
        {
            int totient = TotientFunction(factors, i);
            if (i>10*totient) continue;
            if (Mask(totient) != Mask(i)) continue;

            var r = (double)i/(double)totient;
            if (ratio > r)
            {
                Console.Error.WriteLine($"{i} {r} {totient}");
                ratio = r;
                best = i;
            }
        }
        
        Console.WriteLine(best);
    }
    
    static long Mask(long n)
    {
        long mask = 0;
        while (n>0)
        {
            var d = n%10;
            n/=10;
            long bit = 1L << (6*(int)d);
            while ((mask&bit)!=0) bit<<=1;
            mask|=bit;
        }
        return mask;        
    }
    
    public static int TotientFunction(int[] table, int n)
    {
        int prev = 0;
        int result = n;
        for (int k = n; k > 1; k /= prev)
        {
            int next = table[k];
            if (next != prev) result -= result / next;
            prev = next;
        }
        return result;
    }
    
    public static int[] PrimeFactorsUpTo(int n)
    {
        var factors = new int[n + 1];

        for (int i = 2; i <= n; i += 2)
            factors[i] = 2;

        var sqrt = (int)Math.Sqrt(n);
        for (int i = 3; i <= sqrt; i += 2)
        {
            if (factors[i] != 0) continue;
            for (int j = i * i; j <= n; j += i + i)
            {
                if (factors[j] == 0)
                    factors[j] = i;
            }
        }

        for (int i = 3; i <= n; i += 2)
        {
            if (factors[i] == 0)
                factors[i] = i;
        }

        return factors;
    }
}