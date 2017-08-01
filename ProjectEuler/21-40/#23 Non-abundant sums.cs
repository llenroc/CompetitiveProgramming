using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        
        int[] sod = new int[28200];
        
        for (int i=1; i<sod.Length; i++)
            for (int j=2*i; j<sod.Length; j+=i)
                sod[j] += i;
        
        var abundant = new HashSet<int>();
        for (int i=1; i<sod.Length; i++)
            if (sod[i] > i)
                abundant.Add(i);
        
        int t = int.Parse(Console.ReadLine());
        while (t-->0)
        {
            int n = int.Parse(Console.ReadLine());
            
            if (n > 28123)
            {
                Console.WriteLine("YES");
                continue;
            }
            
            bool bad = true;
            foreach(var v in abundant)
            {
                if (v >= n) break;
                if (abundant.Contains(v) && abundant.Contains(n-v))
                {
                    Console.WriteLine("YES");
                    bad = false;
                    break;
                }
            }
            
            if (bad) Console.WriteLine("NO");
        }
    }
}