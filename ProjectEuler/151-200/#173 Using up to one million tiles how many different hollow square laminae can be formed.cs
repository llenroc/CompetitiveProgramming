using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        long n = long.Parse(Console.ReadLine());
        
        Console.WriteLine(Solve(n));
    }
    
    public static long Solve(long n)
    {
        if (n<8) return 0;

        long maxside = n/4+1;
        long count = 0;

        long fullsides = (long)Math.Sqrt(n);
        
        for (long thickness=1; thickness <= fullsides; thickness++)
        {
            long left = 1;
            long right = maxside;
            while (left <= right)
            {
                long mid = (left + right)/2;
                var v = Tiles(mid, thickness);
                if (v <= 0)
                    left = mid + 1;
                else 
                    right = mid - 1;
            }
            long start = left;
            
            left = 1;
            right = maxside;
            while (left <= right)
            {
                long mid = (left + right)/2;
                var v = Tiles(mid, thickness);
                if (v <= n)
                    left = mid + 1;
                else 
                    right = mid - 1;
            }
            long end = left;
            
            //Console.Error.WriteLine($"{thickness} - {start} to {end}");
            count += end - start;
        }

        return count;
    }
    
    public static long Tiles(long holesize, long thickness)
    {
        long farside = holesize + 2 * thickness;
        var result =  farside * farside - holesize * holesize;
        //Console.Error.WriteLine($"Tiles({holesize}, {thickness}) -> {result}");
        return result;
    }
}