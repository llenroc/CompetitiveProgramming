using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
class Solution {
    
    static List<int>[] primes = new List<int>[512];
    static List<int> results = new List<int>();
    
    static void Main(String[] args) {
        var t = int.Parse(Console.ReadLine());
        
        for (int i=1; i<512; i++)
        {
            primes[i] = new List<int>();
            EnumeratePrimes(primes[i], i, 0);
            //Console.Error.WriteLine($"{i} -> " + string.Join(", ", primes[i]));
        }
        
        while (t-->0)
        {
            var s = Console.ReadLine();
            var mask = 0;
            foreach(var ch in s)
                mask |= 1 << ch - '1';

            results.Clear();
            Dfs(mask);
            
            results.Sort();
            foreach(var v in results)
                Console.WriteLine(v);
            Console.WriteLine();
       }
    }

    static void EnumeratePrimes(List<int> list, int mask, int num)
    {
        if (MillerRabin.IsPrime(num))
            list.Add(num);
        
        for (int i=1; i<=9; i++)
        {
            int bit = 1<<i-1;
            if ((mask & bit) != 0)
                EnumeratePrimes(list, mask-bit, num*10+i);
        }
    }
    
    static int Mask(int n)
    {
        int mask = 0;
        while (n > 0)
        {
            mask |= 1 << (n%10-1);
            n/=10;
        }
        return mask;
    }
    
    
    static void Dfs(int mask, int sum = 0, int depth=0, int min=0)
    {
        if (mask==0)
        {
            results.Add(sum);
            return;
        }
        
        //var indent = new string(' ', depth);
        for (int i=0; i<primes[mask].Count; i++)
        {
            var p = primes[mask][i];
            if (p<min) continue;
            var m = Mask(p);
            //Console.Error.WriteLine($"{indent}->{p}/{mask:x}/{m:x} - Dfs({mask&~m:x}, {sum+p})");
            Dfs(mask & ~m, sum+p, depth+1, p+1);
        }
    }

    public static class MillerRabin
    {

        private const long PrimesUnder64 = 0
            | 1L << 02 | 1L << 03 | 1L << 05 | 1L << 07
            | 1L << 11 | 1L << 13 | 1L << 17 | 1L << 19
            | 1L << 23 | 1L << 29
            | 1L << 31 | 1L << 37
            | 1L << 41 | 1L << 43 | 1L << 47
            | 1L << 53 | 1L << 59
            | 1L << 61;

        private const int PrimeFilter235 = 0
            | 1 << 1 | 1 << 7
            | 1 << 11 | 1 << 13 | 1 << 17 | 1 << 19
            | 1 << 23 | 1 << 29;

        // Witnesses must all be less that 64-2=62
        // We filter out numbers below 64
        // https://miller-rabin.appspot.com
        static readonly int[] Witness32 = { 2, /* 7 , 61*/ }; //  4759123141
        static readonly long[] Witness40 = { 2, 13, 23, 1662803 }; //  1122004669633
        static readonly long[] Witness41 = { 2, 3, 5, 7, 11, 13 }; // 3,474,749,660,383
        static readonly long[] Witness51 = { 2, 75088, 642735, 203659041, 3613982119 }; // 3071837692357849
        static readonly long[] Witness64 = { 2, 325, 9375, 28178, 450775, 9780504, 1795265022 }; // Can't be witness if w | n

        // TEST CASE: 46817 is a prime

        // Sieve is 10X faster for checking multiple primes.

        static int[] higherprimes = new int[] { 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 73, 101, 239 };
        
        public static bool IsPrime(long n)
        {
            if (n < 2) return false;
            if (n < 64) return (PrimesUnder64 & 1L << (int)n) != 0;
            if ((PrimeFilter235 & 1 << (int)(n % 30)) == 0)
                return false;

            foreach (var v in higherprimes)
            {
                if (v > n) break;
                if (n % v == 0)
                    return n == v;
            }
            
            return Pow(2, n-1, n)==1 && Pow(3, n-1, n)==1 && Pow(5, n-1, n)==1;
        }
        
        
        public static long Mult(long a, long b, long mod)
		{
			// If both integers are positive 32-bit integers, use shortcut
			if ((ulong)(a | b) <= (1UL << 31))
				return a * b % mod;

			// If mod is positive 32-bit integer, use shortcut
			a %= mod;
			b %= mod;
			if ((ulong)mod <= (1UL << 31))
				return a * b % mod;

			if (a < b)
			{
				long tmp = a;
				a = b;
				b = tmp;
			}

			long result = 0;
			long y = a;
			while (b > 0)
			{
				if ((b & 1) == 1)
                {
                    result += y;
                    if (result >= mod) result -= mod;
                }
                y += y;
                if (y>=mod) y -= mod;
				b >>= 1;
			}
			return result % mod;
		}
        
        public static long Pow(long n, long p, long mod)
		{
			long result = 1;
			long b = n;
			while (p > 0)
			{
				if ((p & 1) == 1) result = Mult(result, b, mod);
				p >>= 1;
				b = Mult(b, b, mod);
			}
			return result;
		}
    }
}
