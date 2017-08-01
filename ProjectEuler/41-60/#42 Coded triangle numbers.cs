using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {

        int t = int.Parse(Console.ReadLine());
        
        while (t-->0)
        {
            long tn = long.Parse(Console.ReadLine());
            
            double r = Math.Sqrt(1 + 8*tn);
            double n = (-1+r)/2;
            if (n>0 && n==Math.Floor(n))
                Console.WriteLine(n);
            else
                Console.WriteLine(-1);
        }
    
    }
}