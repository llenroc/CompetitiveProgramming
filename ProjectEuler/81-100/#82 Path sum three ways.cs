using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {

        int n = int.Parse(Console.ReadLine());
        var m = Enumerable.Range(0,n).Select(x=>Array.ConvertAll(Console.ReadLine().Split(), long.Parse)).ToArray();
        var m2 = m.Select(x=>(long[]) x.Clone()).ToArray();

        var upmin = new long[n];
        var downmin = new long[n];
        
        for (int j=n-2; j>=0; j--) 
        {
            upmin[0] =  m[0][j] + m2[0][j+1];
            for (int i=1; i<n; i++)
                upmin[i] = m[i][j] + Math.Min(upmin[i-1], m2[i][j+1]);
            
            downmin[n-1] = m[n-1][j] + m2[n-1][j+1];
            for (int i=n-2; i>=0; i--)        
                downmin[i] = m[i][j] + Math.Min(downmin[i+1], m2[i][j+1]);
            
            for (int i=n-1; i>=0; i--)        
            {
                long min = m2[i][j] + m2[i][j+1];
                min = Math.Min(min, upmin[i]);
                min = Math.Min(min, downmin[i]);
                m2[i][j] = min;
            }
        }

        long ans = m2[0][0];
        for (int i=0; i<n; i++)
            ans = Math.Min(ans, m2[i][0]);
        
        Console.WriteLine(ans);
    }
}