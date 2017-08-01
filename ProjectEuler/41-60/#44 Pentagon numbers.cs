using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        var input = Array.ConvertAll(Console.ReadLine().Split(), long.Parse);
        var n = input[0];
        var k = input[1];
        
        for (long i=k+1; i<n ; i++)
        {
            var Pn = Penta(i);
            var Pnk = Penta(i-k);
            
            //Console.Error.WriteLine($"P{i} = {Pn} {Pn-Pnk} {Pn+Pnk}");
            if (IsPentagonal(Pn - Pnk) || IsPentagonal(Pn + Pnk))
                Console.WriteLine(Pn);
        }
    
    }
    
    public static long Penta(long i)
    {
        return i * (3*i - 1) >> 1;
    }
    
    public static bool IsPentagonal(long Pn)
    {
        //var radical = 1+8*Pn; 
        //var r = Sqrt(radical);
        //if (r * r != radical) return false;
        //return (r + 1) % 6 == 0;

        var radical = 1+24*Pn; 
        var r = Sqrt(radical);
        if (r * r != radical) return false;
        return (r + 1) % 6 == 0;
    }
    
    public static long Sqrt(long x)
    {
        if (x == 0 || x == 1)
            return x;

        long start = 1, end = Math.Min(x, 3037000499L), ans = 1;
        while (start <= end)
        {
            long mid = (start + end)>>1;
            long sqr = mid * mid;
            if (sqr == x)
            {
                ans = mid;
                break;
            }

            if (sqr < x)
            {
                start = mid + 1;
                ans = mid;
            }
            else
                end = mid - 1;
        }
        //Console.Error.WriteLine($"sqrt({x}) => {ans}");
        return ans;
    }
}