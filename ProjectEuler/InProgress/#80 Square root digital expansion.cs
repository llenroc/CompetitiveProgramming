using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
class Solution {
    
    static BigInteger upper;
    static BigInteger lower;
        
    static void Main(String[] args) {
        
        var factors = new int[10001];
        int[] lastDigit = new int[10001];
        var sums = new long[10001];
        
        for (int i=2; i<factors.Length; i++)
        {
            if (i>2 && (i&1)==0) continue;
            if (factors[i]!=0) continue;
            for (int j=i*i; j<factors.Length; j+=i)
                factors[j] = i;
        }
        
        var table = new BigInteger[10001];
        var n = int.Parse(Console.ReadLine());
        var p = int.Parse(Console.ReadLine());
        
        var pp = p+5;
        var b2p = BigInteger.Pow(10,2*pp);
        var bp = BigInteger.Pow(10,pp);
        long sum = 0;
        for (int i=2; i<=n; i++)
        {
            var sqd = Math.Sqrt(i);
            var sq = (int)sqd;
            if (sq*sq==i)
            {
                table[i] = table[sq]*table[sq]/bp;
                continue;
            }

            BigInteger sqrt; 
            if ( factors[i] != 0 )
                sqrt = table[i/factors[i]]*table[factors[i]]/bp;
            else
            {
                long d = 1;
                long m = 0;
                long cf = sq;

                var ns0 = BigInteger.Zero;
                var ds0 = BigInteger.One;
                var ns1 = BigInteger.One;
                var ds1 = BigInteger.Zero;                
                
                var max = Math.Max(1000, 2*p);
                for (int z=0; z<max; z++)
                {
       				var ns = cf * ns1 + ns0;
                    var ds = cf * ds1 + ds0;
                    ns0 = ns1;
                    ds0 = ds1;
                    ns1 = ns;
                    ds1 = ds;
                    m = d*cf - m;
                    d = (i - m*m)/d;
                    cf = (sq + m)/d;
                }

                sqrt = ns1 * bp / ds1;
            }
            
            var s = sqrt.ToString();
            long sum2 = 0;
            int count = 0;
            foreach(char ch in s)
            {
                if (count++ == p) break;
                sum2 += ch-'0';
            }

            lastDigit[i] = s[p-1]-'0';
            sums[i] = sum2;
            table[i] = sqrt;
            sum += sum2;
        }

        Console.WriteLine(sum);
        Console.Error.WriteLine(System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime);
    }
    



}