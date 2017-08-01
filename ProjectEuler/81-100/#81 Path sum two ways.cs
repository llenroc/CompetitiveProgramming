using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {

        int n = int.Parse(Console.ReadLine());
        var m = Enumerable.Range(0,n).Select(x=>Array.ConvertAll(Console.ReadLine().Split(), long.Parse)).ToArray();
        
        for (int i=n-2; i>=0; i--)
            m[i][n-1] += m[i+1][n-1];

         for (int i=n-2; i>=0; i--)
            m[n-1][i] += m[n-1][i+1];

        for (int i=n-2; i>=0; i--)        
        for (int j=n-2; j>=0; j--)        
            m[i][j] += Math.Min(m[i+1][j], m[i][j+1]);
        
       // for (int i=0; i<n; i++)
       //     Console.Error.WriteLine(string.Join(" ", m[i]));
        
        
        Console.WriteLine(m[0][0]);
    }
}