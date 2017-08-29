using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        int t = int.Parse(Console.ReadLine());

        var composites = new BitArray(100000001);
        var primes = new List<int>();
        for (int i=2; i<composites.Length; i++)
        {
            if (composites[i]) continue;
            primes.Add(i);
            for (long j=i*1L*i; j<composites.Length; j+=i)
                composites[(int)j] = true;
        }
        
        while (t-->0)
        {
            int n = int.Parse(Console.ReadLine());
            int count = 0;
            
            for(var i=0; i<primes.Count; i++)
            {
                var p = primes[i];
                if (p*p > n) break;
            
                int index = primes.BinarySearch((n+p-1)/p);
                if (index<0) index = ~index;
                
                int add = Math.Max(0, index-i);
                //Console.Error.WriteLine($"#{i}-{p}->{add}");
                count += add;
            }

            Console.WriteLine(count);
        }
    }
}