using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        
        bool[] sod = new bool[100001];
        
        for (long i=2; i<sod.Length; i++)
            for (long j=i*i; j<sod.Length; j+=i)
                sod[j] = true;
        
        var primes = new HashSet<int>();
        for (int i=1; i<sod.Length; i++)
            if (!sod[i])
               primes.Add(i);
        
        int n = int.Parse(Console.ReadLine());

        int maxLen = 0;
        int abest = 0;
        int bbest = 0;
        
        for (int b=1; b<=n; b++)
        {
            if (!primes.Contains(b)) continue;
            for (int a=2-b; a<=n; a++)
            {
                if (!primes.Contains(maxLen * (a + maxLen)+b)) continue;
                //Console.Error.WriteLine($"a={a} b={b}");
                
                int len = 0;
                while (primes.Contains(len*(len + a) + b))
                    len++;
                
                if (len >= maxLen)
                {
                    maxLen = len;
                    abest = a;
                    bbest = b;
                }
            }
        }
        
        Console.WriteLine($"{abest} {bbest}");
        Console.Error.WriteLine(maxLen);
    }
}