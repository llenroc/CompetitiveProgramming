using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    const long MOD = 1000*1000*1000 + 7;

    static void Main(String[] args) {

        int N = int.Parse(Console.ReadLine());
        long count = 0;
        
        for (int n=3; n<=N; n++)
        for (int i=0;i+2<n; i++)
        {
            for (int j=i+1; j+1<n; j++)
                for (int k=j+1; k<n; k++)
                {
                    // A10 A01 1A0 10A
                    long tmp = Pow(13, i) * Pow(14,j-i-1) % MOD * Pow(15,k-j-1) % MOD * Pow(16,n-k-1) % MOD;                                 
                    tmp = tmp * (i>0 ? 6 : 4) % MOD;
                    count += tmp;
                    if (count >= MOD)
                        count -= MOD;
                }
        }
        Console.WriteLine(count % MOD);
    }
    
    public static long Pow(long n, long p, long mod = MOD)
	{
		long b = n;
		long result = 1;
		while (p != 0)
		{
			if ((p & 1) != 0)
				result = (result * b) % mod;
			p >>= 1;
			b = (b * b) % mod;
		}
		return result;
	}
   
}