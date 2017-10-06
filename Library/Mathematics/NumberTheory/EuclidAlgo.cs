﻿using System;
using System.Collections.Generic;

namespace Softperson.Mathematics
{
	public static class EuclidAlgo
	{
		// O(sqrt(x)) Exhaustive Primality Test
		public const double EPS = 1e-7;
		// This is a collection of useful code for solving problems that
		// involve modular linear equations.  Note that all of the
		// algorithms described here work on nonnegative integers.

		// computes gcd(a,b)
		public static int Gcd(int a, int b)
		{
			while (true)
			{
				if (a == 0) return b;
				b %= a;
				if (b == 0) return a;
				a %= b;
			}
		}

		// computes lcm(a,b)
		public static int Lcm(int a, int b)
		{
			return a/Gcd(a, b)*b;
		}

		// returns d = gcd(a,b); finds x,y such that d = ax + by
		public static int ExtendedEuclid(int a, int b, out int x, out int y)
		{
			var xx = y = 0;
			var yy = x = 1;
			while (b != 0)
			{
				var q = a/b;
				var t = b;
				b = a%b;
				a = t;
				t = xx;
				xx = x - q*xx;
				x = t;
				t = yy;
				yy = y - q*yy;
				y = t;
			}
			return a;
		}

		public static long ExtendedEuclid(long a, long b, out long x, out long y)
		{
			var xx = y = 0;
			var yy = x = 1;
			while (b != 0)
			{
				var q = a / b;
				var t = b;
				b = a % b;
				a = t;
				t = xx;
				xx = x - q * xx;
				x = t;
				t = yy;
				yy = y - q * yy;
				y = t;
			}
			return a;
		}


		// finds all solutions to ax = b (mod n)
		public static List<int> ModularLinearEquationSolver(int a, int b, int n)
		{
			int x, y;
			var solutions = new List<int>();
			var d = ExtendedEuclid(a, n, out x, out y);
			if (b%d == 0)
			{
				x = ModularMath.Mod(x*(b/d), n);
				for (var i = 0; i < d; i++)
				{
					solutions.Add(ModularMath.Mod(x + i*(n/d), n));
				}
			}
			return solutions;
		}



		// Chinese remainder theorem (special case): find z such that
		// z % x = a, z % y = b.  Here, z is unique modulo M = lcm(x,y).
		// Return (z,M).  On failure, M = -1.
		public static Pair<int, int> ChineseRemainderTheorem(int x, int a, int y, int b)
		{
			int s, t;
			var d = ExtendedEuclid(x, y, out s, out t);
			if (a%d != b%d) return new Pair<int, int>(0, -1);
			return new Pair<int, int>(ModularMath.Mod(s*b*x + t*a*y, x*y)/d, x*y/d);
		}

		// Chinese remainder theorem: find z such that
		// z % x[i] = a[i] for all i.  Note that the solution is
		// unique modulo M = lcm_i (x[i]).  Return (z,M).  On 
		// failure, M = -1.  Note that we do not require the a[i]'s
		// to be relatively prime.
		public static Pair<int, int> ChineseRemainderTheorem(List<int> x, List<int> a)
		{
			var ret = new Pair<int, int>(a[0], x[0]);
			for (var i = 1; i < x.Count; i++)
			{
				ret = ChineseRemainderTheorem(ret.Second, ret.First, x[i], a[i]);
				if (ret.Second == -1) break;
			}
			return ret;
		}

        #region Chinese Remainder Theorem

        // SOURCE: https://www37.atwiki.jp/uwicoder/pages/2118.html

        public static long[] ExGcd(long a, long b)
        {
            if (a == 0 || b == 0)
                return null;
            int @as = Math.Sign(a);
            int bs = Math.Sign(b);
            a = Math.Abs(a);
            b = Math.Abs(b);
            long p = 1, q = 0, r = 0, s = 1;
            while (b > 0)
            {
                long c = a / b;
                long d;
                d = a;
                a = b;
                b = d % b;
                d = p;
                p = q;
                q = d - c * q;
                d = r;
                r = s;
                s = d - c * s;
            }
            return new long[] { a, p * @as, r * bs };
        }

        public static long Crt(long[] divs, long[] mods)
        {
            long div = divs[0];
            long mod = mods[0];
            for (int i = 1; i < divs.Length; i++)
            {
                long[] apr = ExGcd(div, divs[i]);
                if ((mods[i] - mod) % apr[0] != 0)
                    return -1;
                long ndiv = div * divs[i] / apr[0];
                long nmod = (apr[1] * (mods[i] - mod) / apr[0] * div + mod) % ndiv;
                if (nmod < 0)
                    nmod += ndiv;
                div = ndiv;
                mod = nmod;
            }
            return mod;
        }

        #endregion



        // computes x and y such that ax + by = c; on failure, x = y =-1
        public static void LinearDiophantine(int a, int b, int c, out int x, out int y)
		{
			var d = Gcd(a, b);
			if (c%d != 0)
			{
				x = y = -1;
			}
			else
			{
				x = c/d* ModularMath.ModInverse(a/d, b/d);
				y = (c - a*x)/b;
			}
		}

		private static bool IsPrimeSlow(long x)
		{
			if (x <= 1) return false;
			if (x <= 3) return true;
			if (x%2 == 0 || x%3 == 0) return false;
			var s = (long) (Math.Sqrt(x) + EPS);
			for (long i = 5; i <= s; i += 6)
			{
				if (x%i == 0 || x%(i + 2) == 0) return false;
			}
			return true;
		}

		//    661   673   677   683   691   701   709   719   727   733   739   743
		//    599   601   607   613   617   619   631   641   643   647   653   659
		//    509   521   523   541   547   557   563   569   571   577   587   593
		//    439   443   449   457   461   463   467   479   487   491   499   503
		//    367   373   379   383   389   397   401   409   419   421   431   433
		//    283   293   307   311   313   317   331   337   347   349   353   359
		//    227   229   233   239   241   251   257   263   269   271   277   281
		//    157   163   167   173   179   181   191   193   197   199   211   223
		//     97   101   103   107   109   113   127   131   137   139   149   151
		//     41    43    47    53    59    61    67    71    73    79    83    89
		//      2     3     5     7    11    13    17    19    23    29    31    37
		// Primes less than 1000:
		//    751   757   761   769   773   787   797   809   811   821   823   827
		//    829   839   853   857   859   863   877   881   883   887   907   911
		//    919   929   937   941   947   953   967   971   977   983   991   997

		// Other primes:
		//    The largest prime smaller than 10 is 7.
		//    The largest prime smaller than 100 is 97.
		//    The largest prime smaller than 1000 is 997.
		//    The largest prime smaller than 10000 is 9973.
		//    The largest prime smaller than 100000 is 99991.
		//    The largest prime smaller than 1000000 is 999983.
		//    The largest prime smaller than 10000000 is 9999991.
		//    The largest prime smaller than 100000000 is 99999989.
		//    The largest prime smaller than 1000000000 is 999999937.
		//    The largest prime smaller than 10000000000 is 9999999967.
		//    The largest prime smaller than 100000000000 is 99999999977.
		//    The largest prime smaller than 1000000000000 is 999999999989.
		//    The largest prime smaller than 10000000000000 is 9999999999971.
		//    The largest prime smaller than 100000000000000 is 99999999999973.
		//    The largest prime smaller than 1000000000000000 is 999999999999989.
		//    The largest prime smaller than 10000000000000000 is 9999999999999937.
		//    The largest prime smaller than 100000000000000000 is 99999999999999997.
		//    The largest prime smaller than 1000000000000000000 is 999999999999999989.
	}
}