using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        
        int[] sod = new int[100001];
        
        for (int i=1; i<sod.Length; i++)
            for (int j=2*i; j<sod.Length; j+=i)
                sod[j] += i;
        
        var amicable = new HashSet<int>();
        for (int i=1; i<sod.Length; i++)
            if (sod[i] != i && sod[i]<sod.Length && sod[sod[i]] == i)
                amicable.Add(i);
        
        int t = int.Parse(Console.ReadLine());
        while (t-->0)
        {
            int n = int.Parse(Console.ReadLine());
            
            bool bad = true;
            long sum = 0;
            foreach(var v in amicable)
            {
                if (v >= n) break;
                sum += v;
            }
            
            Console.WriteLine(sum);
        }
    }
}