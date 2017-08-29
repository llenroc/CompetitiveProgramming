using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        long n = long.Parse(Console.ReadLine());
        
        long count = 0;
        for (int i=4; i<=n; i+=4)
        {
            long count2 = 0;
            var factors = Factor(i);
            foreach(var f in factors)
            {
                var f2 = i / f;
                if (f >= f2 || (f&1) != (f2&1) ) continue;

                var xpy = f2;
                var xmy = f;
                var x = (xpy + xmy)/2;
                var y = xpy - x;
                if ( x==y || (x&1) != (y&1) ) continue;
                //Console.Error.WriteLine($"{x}^2-{y}^2 from {f} and {f2}");
                count2 ++;
            }
            if (count2>0)
            //Console.Error.WriteLine($"{i} -> {count2}");
            count += count2;
        }
        
        Console.WriteLine(count);
    }
    
    public static Dictionary<long, int> PrimeFactors(long n)
    {
        var dict = new Dictionary<long, int>();

        int cnt = 0;
        while (n % 2 == 0)
        {
            n /= 2;
            cnt++;
        }
        
        if (cnt > 0)
            dict[2] = cnt;

        for (int i = 3; i * i <= n; i += 2)
        {
            cnt = 0;
            while (n % i == 0)
            {
                n /= i;
                cnt++;
            }
            if (cnt > 0)
                dict[i] = cnt;
        }

        if (n != 1)
            dict[n] = 1;

        return dict;
    }

    public static List<long> Factor(long n)
    {
        var primes = PrimeFactors(n);
        
        var list = new List<long>{1};
        foreach (var pair in primes)
        {
            int count = list.Count;

            long f = pair.Key;
            for (int j = 0; j < pair.Value; j++, f *= pair.Key)
            {
                for (int i = 0; i < count; i++)
                    list.Add(list[i] * f);
            }
        }

        return list;
    }

}