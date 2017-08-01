using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        
        var array = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        var n = array[0];
        var k = array[1];
        
        for (int i=125874; i<=n; i++)
        {
            long mask = Mask(i);
            bool good = true;
            for (long j=2; j<=k; j++)
                if (Mask(j*i) != mask)
                {
                    good = false;
                    break;
                }
            if (good)
            {
                Console.Error.WriteLine(mask.ToString("X"));
                Console.WriteLine(string.Join(" ", Enumerable.Range(1,k).Select(x=>(x*i))));
            }
        }
        
    }
    
    static long Mask(long n)
    {
        long mask = 0;
        while (n>0)
        {
            var d = n%10;
            n/=10;
            long bit = 1L << (6*(int)d);
            while ((mask&bit)!=0) bit<<=1;
            mask|=bit;
        }
        return mask;        
    }
}



