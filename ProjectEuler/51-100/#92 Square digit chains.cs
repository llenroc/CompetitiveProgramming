using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    
    const int MOD = 1000 * 1000 * 1000 + 7;
    static long[] dp = new long[10000000];
    
    static void Main(String[] args) {
        int k = int.Parse(Console.ReadLine());
        
        dp[1] = 1 + 1;
        dp[89] = 89 + 1;

        /*
        long count89 = 0;
        long limit = 100;
        for (long i=1; i<=limit; i++)
        {
            var v = Dfs(i);
            //if (1000000%i==0) Console.Error.WriteLine($"{i} -> {count89}");
            if (v==89) count89++;
            
        }*/
            
        var dict = new Dictionary<int, long>();
        for (int i=1; i<=9; i++)
            dict[i*i] = 1;
        
        long result = 7;
        for (int i=2; i<=k; i++)
        {
            var dict2 = new Dictionary<int, long>();
            
            foreach(var pair in dict)
            {
                for (int j= 0; j<=9; j++)
                {
                    var v = pair.Key + j*j;
                    if (dict2.ContainsKey(v)==false) dict2[v] = 0;
                    dict2[v] = (dict2[v] + pair.Value) % MOD;
                }
            }
            
            long c = 0;
            foreach(var pair in dict2.OrderBy(x=>x.Key))
            {
                c += pair.Value;
                var v = Dfs(pair.Key);
                //Console.Error.WriteLine($"{pair.Key} => v={v} count={pair.Value}");
                if (v==89)
                    result = (result + pair.Value) % MOD;
            }
            dict = dict2;
        }
        
        Console.WriteLine(result);
    }
    
    static long Dfs(long n)
    {
        if (dp[n] != 0) 
        {
            return dp[n] - 1;
        }
        
        long result = 0;
        var i = n;
        while (i>0)
        {
            var d = i%10;
            result += d*d;
            i /= 10;
        }

        if (result != n)
            result = Dfs(result);    
        
        dp[n] = result + 1;
        return result;
    }
    
}