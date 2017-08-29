using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {

        var composites = new bool[1000001];
        composites[1] = true;
        composites[0] = true;
        
        for (int i=2; i*i<composites.Length; i+=i!=2?2:1)
            for (long j=i*i; j<composites.Length;j+=i)
                composites[j] = true;
        
        int t = int.Parse(Console.ReadLine());
        
        while (t-->0)
        {
            int n = int.Parse(Console.ReadLine());
            
            int count = 0;
            for (int i=1; 2*i*i<n; i++)
            {
                if (!composites[n-2*i*i]) count++;
            }
            Console.WriteLine(count);
        }
    
    }
}