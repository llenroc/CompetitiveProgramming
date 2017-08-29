using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        int t = int.Parse(Console.ReadLine());
        var dict = new Dictionary<long, int>();
        string maxString = "";
        int best = 0;
        
        long start = 1;
        for (int j=1; j<t; j++);
            start *= 10;
        
        long i=1;
        while (i*i<start)
            i++;
        
        for (; ; i++)
        {
            var sqr = i*i;
            var s = sqr.ToString();
            if (s.Length < t) continue;
            if (s.Length > t) break;
            
            long code = 0;
            foreach(var ch in s)
                code += 1L << 4*(ch-'0'); 

            if (dict.ContainsKey(code)==false) dict[code]=0;
            var count = dict[code] + 1;
            dict[code] = count;
            
            if (count>best || count==best && string.CompareOrdinal(maxString,s)<0)
            {
                maxString = s;
                best = count;
                //if (count>10)
                //    Console.Error.WriteLine($"{s} {code:x} {count}");
            }
        }
        
        Console.WriteLine(maxString);
        
    }
}