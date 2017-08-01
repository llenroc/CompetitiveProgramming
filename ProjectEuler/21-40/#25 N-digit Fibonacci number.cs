using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
class Solution {
    static void Main(String[] args) {
        var t = int.Parse(Console.ReadLine());
        
        List<int> list = new List<int>() {1};
        
        BigInteger a = 0;
        BigInteger b = 1;
        for (int term=1; ; term++)
        {
            int len = (int)BigInteger.Log10(b);
            if (len+1 == list.Count) list.Add(term);
            if (len == 5000) break;
            var c = a + b;
            a = b;
            b = c;
        }
        
        while (t-->0)
        {
            int n = int.Parse(Console.ReadLine());

            int term = list[n];
            Console.WriteLine(term);            
                
            //double term2 = (n - Math.Log(5,10)/2) / (Math.Log(1+Math.Sqrt(5),10) - Math.Log(2, 10));
            //Console.WriteLine((int)term2);
        }
        
        
        
    }
    
    
}