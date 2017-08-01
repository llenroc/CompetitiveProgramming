using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        long n = long.Parse(Console.ReadLine());
        
        var factors = PrimeFactorsUpTo(5000000);
        var minimum = new long[n+1];
        
        long remaining = n;
        for (long i=1; i*i<=n; i++)
            remaining--;
        
        for (long i=2; true; i++)
        {
            var sqrt = (int)Math.Sqrt(i);
            if (sqrt*sqrt==i) continue;
            
            long val = i*i-1;
            if (i+1 >= factors.Length)
            {
                Console.Error.WriteLine($"Broke at {i} with {remaining} remaining");
                break;
            }
            
            EnumerateSquareFactors
                (factors, i-1, i+1, f=>
                 {
                     //if (f==1) return;
                     var d = val/f;
                     if (d >= minimum.Length || minimum[d]!=0) return;
                     //Console.Error.WriteLine($"Found {i}^2 - {d}*{Math.Sqrt(f)}^2 = 1 (rem={remaining}-1)");
                     minimum[d] = i;
                 });
        }
        
        long best = -1;
        for (int i=2; i<=n; i++)
            if (best==-1 || minimum[best]<minimum[i])
                best = i;
        
        Console.WriteLine(best);
    }
    
    public static long EnumerateSquareFactors(int[] factors, long n1, long n2,  
                                             Action<long> action = null, long f = 1)
    {
        if (n1 == 1 && n2 == 1)
        {
            action?.Invoke(f);
            return 1;
        }

        long p1 = factors[n1];
        long p2 = factors[n2];
        long p = n1 == 1 ? p2 : n2 == 1 ? p1 : Math.Min(p1, p2);

        long c = 0;
        long next1 = n1;
        long next2 = n2;

        while (next1 > 1 && factors[next1] == p)
        {
            c++;
            next1 /= p;
        }

        while (next2 > 1 && factors[next2] == p)
        {
            c++;
            next2 /= p;
        }

        var result = EnumerateSquareFactors(factors, next1, next2, action, f);
        while (c >= 2)
        {
            c-=2;
            f *= p * p;
            result += EnumerateSquareFactors(factors, next1, next2, action, f);
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

