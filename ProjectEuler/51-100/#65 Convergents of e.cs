using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
class Solution {
    static void Main(String[] args) {
        int n = int.Parse(Console.ReadLine());
    
        int i = 1;
        foreach(var v in ConvertToFraction(x=> (x==0) ? 2 : (x+1)%3==0 ? 2*(x+1)/3 : 1 ))
        {
            if (i++==n)
            {
                int sum = 0;
                foreach(var ch in v.Item1.ToString())
                    sum += ch-'0';
                Console.WriteLine(sum);
                break;
            }
        }
    
    }

    public static IEnumerable<Tuple<BigInteger, BigInteger>> ConvertToFraction(Func<BigInteger, BigInteger> cf)
    {
        BigInteger ns0 = 0;
        BigInteger ds0 = 1;
        BigInteger ns1 = 1;
        BigInteger ds1 = 0;

        for (long i = 0; true; i++)
        {
            var c = cf(i);
            if (c == 0) break;
            var ns = c * ns1 + ns0;
            var ds = c * ds1 + ds0;
            //Console.Error.WriteLine($"{i+1} {c} {ns} {ds}");

            yield return new Tuple<BigInteger,BigInteger>(ns, ds);
            ns0 = ns1;
            ds0 = ds1;
            ns1 = ns;
            ds1 = ds;
        }
    }
}