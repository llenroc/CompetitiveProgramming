using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        var coins = new int[] { 1, 2, 5, 10, 20, 50, 100, 200 };
        var dp = new long[coins.Length+1, 100001];
        int MOD = 1000*1000*1000+7;
        
        dp[0,0] = 1;
        for (int i=1; i<=coins.Length; i++)
        {
            dp[i,0] = 1;
            int c = coins[i-1];
            for (int j=1; j<100001; j++)
                dp[i,j] = (dp[i-1,j] + (j>=c ? dp[i,j-c] : 0))%MOD;
        }
        
        for (int t = int.Parse(Console.ReadLine()); t>0; t--)
            Console.WriteLine(dp[coins.Length, int.Parse(Console.ReadLine())] % MOD);
        
    }
}