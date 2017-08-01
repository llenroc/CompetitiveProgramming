using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Solution {
    static void Main(String[] args) {
        int n = int.Parse(Console.ReadLine());
        var dict = new Dictionary<long, int>();
        
        for (int i=1; i<=n; i++)
        {
            long cur = i;
            for (int k=0; k<60; k++)
            {
                var rev = Reverse(cur);
                if (rev == cur)
                {
                    if (dict.ContainsKey(cur)==false) dict[cur] = 0;
                    dict[cur]++;
                    break;
                }
                cur = cur + rev;
            }
        }
        
        var max = dict.Values.Max();
        var v = dict.First(x=>x.Value==max);
        Console.WriteLine($"{v.Key} {max}");
    }
    
    static long Reverse(long num)
    {
        long result = 0;
        while (num>0)
        {
            result = result * 10 + num % 10;
            num /= 10;
        }
        return result;
    }
}