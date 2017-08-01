using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        ValueTuple<int,int> valueTuple;
        
        int n = int.Parse(Console.ReadLine());
        int count = 0;
        int iter = 0;
        foreach(var v in ConvertToFraction(x=>x>0 ? 2 : 1))
        {
            if (iter!=0 && (int)BigInteger.Log10(v.Item1) > (int)BigInteger.Log10(v.Item2))
            {
                //Console.Error.WriteLine(v);
                Console.WriteLine(iter);
            }
            iter++;
            if (iter>n) break;
        }
    }
    
    public static IEnumerable<ValueTuple<BigInteger, BigInteger>> ConvertToFraction(Func<long, long> cf)
    {
        BigInteger ns0 = 0;
        BigInteger ds0 = 1;
        BigInteger ns1 = 1;
        BigInteger ds1 = 0;

        for (long i = 0; true; i++)
        {
            var c = cf(i);
            if (c == 0) break;
            BigInteger ns = c * ns1 + ns0;
            BigInteger ds = c * ds1 + ds0;
            yield return new ValueTuple<BigInteger, BigInteger>(ns, ds);
            ns0 = ns1;
            ds0 = ds1;
            ns1 = ns;
            ds1 = ds;
        }
    }
}