using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Softperson.Mathematics.ModularMath;

namespace Softperson.Mathematics
{
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
				long v = ModPow(w, s, n);

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

			foreach (var w in witnesses)
			{
				// Witnesses can't be a multiple of n
				// The inequality w < 2^31 < n is guaranteed by the 32-bit int rerouting
				// if (w % n == 0) continue;

				long v = ModPow(w, s, n);
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

		public static bool IsPrime(BigInteger n)
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

			for (int i = 0; i < witnesses.Length; i++)
			{
				var w = witnesses[i];
				// Witnesses can't be a multiple of n
				// The inequality w < 2^31 < n is guaranteed by the 32-bit int rerouting
				// if (w % n == 0) continue;

				var v = BigInteger.ModPow(w, s, n);
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

		// Warning: Needs to be under 64 -- otherwsise prime will be cut out
		static int[] PrimesBetween7And61 = new int[] { 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61 };



		public static bool MayBePrime(BigInteger n)
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
					return n == v; // Safety check

			// all filtered numbers less than 61 * 67 must be prime

			return true;
		}

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
					return n == v; // Safety check

			return true;
		}
		
		public static bool FermatProbablyPrime(long n)
		{
			const long PrimesUnder64 = 0
			| 1L << 02 | 1L << 03 | 1L << 05 | 1L << 07
			| 1L << 11 | 1L << 13 | 1L << 17 | 1L << 19
			| 1L << 23 | 1L << 29
			| 1L << 31 | 1L << 37
			| 1L << 41 | 1L << 43 | 1L << 47
			| 1L << 53 | 1L << 59
			| 1L << 61;
			
			// 2 is the first prime
			if (n < 2) return false;

			// Return primes under 64 in constant time
			// Important Step! witnesses < 64 <= n
			if (n < 64) return (PrimesUnder64 & 1L << (int)n) != 0;

			return MayBePrime(n) 
				? FermatProbablyPrime(n,2) && FermatProbablyPrime(n,3) && FermatProbablyPrime(n,5)
				: n == 2;
		}

		public static bool FermatProbablyPrime(long n, long b)
		{
			return ModPow(b, n - 1, n) == 1;
		}

		public static bool EulerProbablyPrime(long n)
		{
			// isprime(n) - Test whether n is prime using a variety of pseudoprime tests.*/

			return MayBePrime(n)
				? EulerProbablyPrime(n, 2) && EulerProbablyPrime(n, 3) && EulerProbablyPrime(n, 5)
				: n == 2;
		}

		public static bool EulerProbablyPrime(long n, long b)
		{
			if (!FermatProbablyPrime(n, b)) return false;

			long r = n - 1;
			while ((r & 1) == 0) r >>= 1;

			long c = ModPow(b, r, n);
			if (c != 1)
			{
				while (c != n - 1)
				{
					c = ModPow(c, 2, n);
					if (c == 1) return false;
				}
			}

			return true;
		}
	}
}
