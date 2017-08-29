using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        
        var triples = new long[1000];
        for (int i=0; i<triples.Length; i++)
        {
            if ( (i%10) + (i/10%10) + (i/100) < 10 )
                triples[i] = 1;
        }

        const int MOD = 1000*1000*1000 + 7;
        var dp = triples.ToArray();
        int n = int.Parse(Console.ReadLine());
        
        for (int i=4; i<=n; i++)
        {
            var dp2 = new long[1000];
            for (int j=0; j<1000; j++)
            {
                if (dp[j] == 0) continue;
                
                int jj = j/10; 
                for (int k=0; k<10; k++)
                {
                    var kjj = k*100 + jj;
                    if (triples[kjj] == 0) continue;
                    dp2[kjj] += dp[j];
                    if (dp2[kjj] >= MOD) dp2[kjj] -= MOD;
                }
            }

            //for (int j=0; j<1000; j++)
            //    Console.Error.WriteLine($"#{j} - {dp[j]}");
            
            dp = dp2;
        }

        long ans = 0;

        for (int j=0; j<1000; j++)
            Console.Error.WriteLine($"#{j} - {dp[j]}");

        
        for (int i=100; i<1000; i++)
            ans += dp[i];
        
        Console.WriteLine(ans % MOD);
    }
}