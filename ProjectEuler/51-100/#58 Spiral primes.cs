using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        int n = int.Parse(Console.ReadLine());
        
        long count = 1;
        long prime = 0;
        long side = 3;
        long k = 1;
        
        while (true)
        {
            var kk = (k*k*4) + 1;
            
            if (MillerRabin.IsPrime(kk-2*k)) prime++;
            if (MillerRabin.IsPrime(kk)) prime++;
            if (MillerRabin.IsPrime(kk+2*k)) prime++;
            count += 4;

            if (100*prime < n*count)
            {
                Console.WriteLine(side);
                break;
            }
            
            k++;
            side+=2;
        }
    }
    
    static IEnumerable<long> Ulam()
    {
        long k = 1;
        yield return 1;
        while (true)
        {
            var kk = (k*k*4) + 1;
            yield return kk - 2*k;
            yield return kk;
            yield return kk + 2*k;
            yield return kk + 4*k;
            k++;
        }
    }
    
    
    public static BitArray GetPrimeSet(int max)
    {
        var isPrime = new BitArray(max + 1, true)
        {
            [0] = false,
            [1] = false
        };

        // Should be 4
        for (int i = 4; i <= max; i += 2)
            isPrime[i] = false;

        for (int i = 3; i <= max; i += 2)
        {
            if (!isPrime[i]) continue;
            // NOTE: Squaring causes overflow
            for (long j = (long)i * i; j <= max; j += i + i)
                isPrime[(int)j] = false;
        }

        return isPrime;
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
		static readonly int[] Witness32 = { 2, 7, 61 }; //  4759123141
		static readonly long[] Witness40 = { 2, 13, 23, 1662803 }; //  1122004669633
		//static readonly long[] Witness41 = { 2, 3, 5, 7, 11, 13 }; // 3,474,749,660,383
		static readonly long[] Witness51 = { 2, 75088, 642735, 203659041, 3613982119 }; // 3071837692357849
		static readonly long[] Witness64 = { 2, 325, 9375, 28178, 450775, 9780504, 1795265022 }; // Can't be witness if w | n

		// TEST CASE: 46817 is a prime

		// Sieve is 10X faster for checking multiple primes.


		public static bool IsPrime32(uint n)
		{
			// 2 is the first prime
			if (n < 2) return false;

			// Return primes under 64 in constant time
			// Important Step! witnesses < 64 <= n
			if (n < 64) return (PrimesUnder64 & 1L << (int)n) != 0;

			// Filter out easy composites (3/4 of positive integers)
			if ((PrimeFilter235 & 1 << (int)(n % 30)) == 0)
				return false;

			// Hard test
			uint s = n - 1;
			while ((s & 1) == 0) { s >>= 1; }

			foreach (var w in Witness32)
			{
				// NOTE: V needs to be long because we are squaring
				long v = Pow(w, s, n);

				if (v != 1)
				{
					for (var t = s; v != n - 1; t <<= 1)
					{
						if (t >= n - 1)
							return false;
						v = v * v % n;
					}
				}
			}

			return true;
		}

		public static bool IsPrime(long n)
		{
			if (n < 2) return false;
			if (n <= int.MaxValue) return IsPrime32((uint)n);

			// Easy Test
			if (!MayBePrime(n))
				return false;

			// Hard test
			var witnesses = n < 1122004669633
				? Witness40
				: n < 3071837692357849
				? Witness51
				: Witness64;

			/* 
			 25% slower
			var witnesses = n < 3474749660383 // 41.6 bits 
				? Witness41
				: Witness64;
			*/

			var s = n - 1;
			while ((s & 1) == 0) { s >>= 1; }

			for (int i=0; i<witnesses.Length; i++)
			{
                var w = witnesses[i];
				// Witnesses can't be a multiple of n
				// The inequality w < 2^31 < n is guaranteed by the 32-bit int rerouting
				// if (w % n == 0) continue;

				long v = Pow(w, s, n);
				if (v != 1)
				{
					for (var t = s; v != n - 1; t <<= 1)
					{
						if (t >= n - 1)
							return false;
						v = Mult(v, v, n);
					}
				}
			}
			return true;
		}

		static int[] PrimesBetween7And61 = new int[] { 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61 };


		public static bool MayBePrime(long n)
		{
			const int PrimeFilter235 = 0
									   | 1 << 1 | 1 << 7
									   | 1 << 11 | 1 << 13 | 1 << 17 | 1 << 19
									   | 1 << 23 | 1 << 29;

			if ((PrimeFilter235 & 1 << (int)(n % 30)) == 0)
				return false;

			// Quick test
			foreach (var v in PrimesBetween7And61)
				if (n % v == 0)
					return false;

			return true;
		}
		
		public static bool FermatProbablyPrime(long n)
		{
			// 2 is the first prime
			if (n < 2) return false;

			// Return primes under 64 in constant time
			// Important Step! witnesses < 64 <= n
			if (n < 64) return (PrimesUnder64 & 1L << (int)n) != 0;

			return MayBePrime(n) 
				? Pow(2, n - 1, n) == 1 
				: n == 2;
		}


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
    
    public static long Mult(long a, long b, long mod)
    {
        if ((ulong)(a) >= (ulong)mod) a %= mod;
        if (a < 0) a += mod;
        if ((ulong)(b) >= (ulong)mod) b %= mod;
        if (b < 0) b += mod;

        long ret = 0;
        int step = 62 - Log2(mod);
        for (int x = Log2(b); x >= 0; x -= step)
        {
            int shift = Math.Min(x + 1, step);
            ret <<= shift;
            ret %= mod;
            ret += a * ((long)((ulong)b >> (x - shift + 1)) & (1L << shift) - 1);
            ret %= mod;
        }
        return ret;
    }
    
    
    public static int Log2(long value)
    {
        var log = 0;
        if ((ulong)value >= (1UL << 24))
        {
            if ((ulong)value >= (1UL << 48))
            {
                log = 48;
                value = (long)((ulong)value >> 48);
            }
            else
            {
                log = 24;
                value >>= 24;
            }
        }
        if (value >= (1 << 12))
        {
            log += 12;
            value >>= 12;
        }
        if (value >= (1 << 6))
        {
            log += 6;
            value >>= 6;
        }
        if (value >= (1 << 3))
        {
            log += 3;
            value >>= 3;
        }
        return log + (int)(value >> 1 & ~value >> 2);
    }

}