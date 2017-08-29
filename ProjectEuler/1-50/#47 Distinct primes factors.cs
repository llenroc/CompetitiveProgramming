using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {

        var input = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        var n = input[0];
        var k = input[1];
        var nk = n+k;
        
        int[] table = new int[nk];
        for (int i=2; i<nk; i++)
        {
            if ( table[i] != 0) continue;
            for (int j=i; j<nk; j+=i)
                table[j]++;
        }
        
        for (int i=2; i<20; i++)
            Console.Error.WriteLine($"{i} -> {table[i]}");
        
        
        for (int i=2; i<=n; i++)
        {
            int j;
            for (j=0; j<k; j++)
                if (table[i+j] != k) break;
            if (j==k)
                Console.WriteLine(i);
        }
                
    }
}