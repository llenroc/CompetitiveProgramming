using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    
    public void Run()
    {
        int n = int.Parse(Console.ReadLine());
        var factors = PrimeFactorsUpTo(10001);
        var hashset = new HashSet<int>();
        
        int allowed = ((1<<n)-1)<<1;
        int count = 0;
        int i;
        for (i=1; i<10000; i++)
        {
            int mask = Mask(i);
            if (mask == 0 || (mask & ~allowed) != 0) continue;

            bool good = false;
            EnumerateFactors(factors, i, 100, x=>
            {
                if (x*x>=i) return;
                
                var x2 = i/x;
                int mask1 = Mask(x);
                int mask2 = Mask(x2);
                if ( mask1==0 || mask2 == 0 || (mask1 & mask2) != 0 ) return;
                mask1 |= mask2;
                
                if ( (mask1 & mask) != 0 || (mask1 | mask) != allowed ) return;
                good = true;
            });
            
            if (good) count += i;
        }
        
        Console.WriteLine(count);
    }
    
    static int Mask(int n)
    {
        int mask = 0;
        while (n>0)
        {
            int d = n%10;
            n /= 10;
            if ((mask & 1<<d) != 0) return 0;
            mask |= 1<<d;
        }
        return mask;
    }
    
    
    static void Main(String[] args) {
        new Solution().Run();
    }
    
    public static int EnumerateFactors(int[] factors, int n, int max, Action<int> action = null, int f = 1)
    {
        if (f > max)
            return 0;

        if (n == 1)
        {
            action?.Invoke(f);
            return 1;
        }

        int p = factors[n];
        int c = 1;
        int next = n / p;
        while (next > 1 && factors[next] == p)
        {
            c++;
            next = next / p;
        }

        int result = EnumerateFactors(factors, next, max, action, f);
        while (c-- > 0)
        {
            f *= p;
            result += EnumerateFactors(factors, next, max, action, f);
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