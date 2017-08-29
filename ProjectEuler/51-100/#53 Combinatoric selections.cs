using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

using Number = System.Double;
class Solution {
    static void Main(String[] args) {

        var pascal = new Number[2001];
        var pascal2 = new Number[2001];
        
        var array = Array.ConvertAll(Console.ReadLine().Split(), long.Parse);
        var n = array[0];
        var k = array[1];
        
        pascal[0] = 1;
        long count = 0;
        for (int i=1; i<=n; i++)
        {
            for (int j=0; j<=n; j++)
            {
                pascal2[j] = (j<n?pascal[j]:0) + (j>0?pascal[j-1]:0);
                if (pascal2[j] > k) count++;
            }
            
            var tmp = pascal;
            pascal = pascal2;
            pascal2 = tmp;
        }
        
        Console.WriteLine(count);
    }
}