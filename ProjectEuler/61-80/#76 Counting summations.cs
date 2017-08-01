using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
class Solution {
    static void Main(String[] args) {

        var table = IntegerPartitionTable(1000);

        int t=int.Parse(Console.ReadLine());
        while (t-->0)
        {
            long n = long.Parse(Console.ReadLine());
            Console.WriteLine((table[n]-1) % 1000000007);
        }

    }
    
    // https://www.hackerrank.com/contests/projecteuler/challenges/euler076/copy-from/1302323130
    static BigInteger[] IntegerPartitionTable(int n)
    {
        var table = new BigInteger[n+1];

        table[0] = 1;
        for (int i=1; i<table.Length; i++)
        {
            for (int j=1;;j++)
            {
                var p = GeneralizedPentagonalNumber(j);
                if (p>i) break;
                var sign = ((j-1)>>1 & 1)==0 ? 1 : -1;
                //if (i<100)
                //    Console.Error.WriteLine($"table[{i}] += table[{i}-{p}] * {sign}");
                table[i] += table[i-p] * sign; 
            }
            
            //if (i<100||i%10==0)
            //Console.Error.WriteLine($"{i}->{table[i]}");
        }
        
        return table;
    }
    
    static long PentagonalNumber(long k)
    {
        return k*(3*k-1)/2;
    }
    
    static long GeneralizedPentagonalNumber(long k)
    {
        if (k==0) return k;
        var kk = (k+1)>>1;
        if ( (k&1) == 0 ) kk = -kk;
        return kk*(3*kk-1)/2;
    }
}