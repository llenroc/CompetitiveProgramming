using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        
        int t = int.Parse(Console.ReadLine());
        var factors = PrimeFactorsUpTo(1000000);
        
        
        var x = new long[1000001];
        x[1] = 0;
        for (int i=2; i<x.Length; i++)
            x[i] = x[i-1] + TotientFunction(factors,i);
        
        for (int i = 0; i<t; i++)
        {
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine(x[n]);
        }   
        
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