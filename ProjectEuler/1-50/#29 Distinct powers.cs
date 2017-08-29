using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
class Solution {
    static void Main(String[] args) {

        int n = int.Parse(Console.ReadLine());
        
        bool [] derived = new bool[n+1];
        for (int i=2; i*i<=n; i++)
        {
            if (derived[i]) continue;
            for (int j = i*i; j<=n; j*=i)
                derived[j] = true;
        }
        
        var set = new HashSet<int>();
        var powerSize = new long[20];
        var limit = Math.Ceiling(Math.Log(n,2))+1;
        for (int i=1; i<limit; i++)
        {
            for (int j=2; j<=n; j++)
                set.Add(i*j);
            powerSize[i] = set.Count;
        }
            
        long result = 0;
        for (int i=2; i<=n; i++)
        {
            if (derived[i]) continue;
/*            int pow = 1;
            long x = i;
            while (x*i <= n)
            {
                pow++;
                x*=i;
            }
*/            
            int pow = (int)(Math.Log(n,i) + .0000001);
            result += powerSize[pow];
        }
        
        Console.WriteLine(result);
    }
    
    
    static long gcd(long x, long y)
    {
        if (x==0) return y;
        return gcd(y%x, x);
    }
}