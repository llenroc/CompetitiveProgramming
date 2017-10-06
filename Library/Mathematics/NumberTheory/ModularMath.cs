using System;
using static System.Math;

namespace Softperson.Mathematics
{
	public static class ModularMath
	{

		// TODO: Do this work for signed

		// WARNING: Unsigned math doesn't work well with modular arithmetic
		// Specifically, negative numbers and overflowed numbers

		/*
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
			if (b % 2 == 1)
				result = (result + y) % mod;
			y = (y * 2) % mod;
			b /= 2;
		}
		return result % mod;
	}*/

		public static long MultSlow(long a, long b, long mod)
		{
			// If both integers are positive 32-bit integers, use shortcut
			if ((a | b) <= (1L << 31))
				return a * b % mod;

			// If mod is positive 32-bit integer, use shortcut
			a %= mod;
			b %= mod;
			if (mod <= (1L << 31))
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
				if (b % 2 == 1)
				{
					result = (result + y);
					if (result >= mod) result -= mod;
				}
				y <<= 1;
				if (y >= mod) y -= mod;
				b >>= 1;
			}
			return result % mod;
		}

		public static long Mult(long a, long b, long mod)
		{
			// Ten times faster than MultSlow
			if ((ulong)(a) >= (ulong)mod) a %= mod;
			if (a < 0) a += mod;
			if ((ulong)(b) >= (ulong)mod) b %= mod;
			if (b < 0) b += mod;

			long ret = 0;
			int step = 62 - BitTools.Log2(mod);
			for (int x = BitTools.Log2(b); x >= 0; x -= step)
			{
				int shift = Min(x + 1, step);
				ret <<= shift;
				ret %= mod;
				ret += a * ((long)((ulong)b >> (x - shift + 1)) & (1L << shift) - 1);
				ret %= mod;
			}
			return ret;
		}


		// https://www.hackerrank.com/contests/projecteuler/challenges/euler048
		public static ulong Mult34Bit(ulong x, ulong y, ulong mod)
		{
			// x,y,mod must fit within 34 bits
			// x and y can be made to fit within 34 bits by modding first
			// 2^34 = 1.7 * 10^10

			// Thirty times faster than MultSlow
			if (x >= (1 << 32) && y >= (1 << 32))
			// if (x > (1 << 30) && y > (1 << 30))
			{
				// First term = Xhi (4-bits) * (Y (34-bits) * 2^30 % mod)
				// Second term = XLo (30-bits) * Y (34-bits) 
				return ((x >> 30) * ((y << 30) % mod) + y * (x & ((1 << 30) - 1))) % mod;
			}

			// 30 bit times 34 bits < 64 bits
			var z = x * y;
			if (z >= mod) z %= mod;
			return z;
		}

		public static ulong Mult42Bit(ulong x, ulong y, ulong mod)
		{
			// x,y,mod must fit within 42 bits
			// x and y can be made to fit within 42 bits by modding first
			// 2^42 = 4.39 * 10^12

			// Thirty times faster than MultSlow
			if (x <= 1ul << 22 || y <= 1ul << 22 || x < 1 << 32 && y < 1 << 32)
			{
				var z = x * y;
				if (z >= mod) z %= mod;
				return z;
			}

			// First term = Xhi * (Y  % mod)
			// (maxbit-bits) + maxbit <= 64
			// Second term = XLo (30-bits) * Y (34-bits)
			// bits + maxbit = 64
			return ((x >> 22) * ((y << 22) % mod) + y * (x & ((1ul << 22) - 1))) % mod;
		}

		public static long Mult34Bit(long x, long y, long mod)
		{
			// x,y,mod must fit within 34 bits
			// x and y can be made to fit within 34 bits by modding first
			// 2^34 = 1.7 * 10^10

			// Thirty times faster than MultSlow
			if (x >= (1 << 32) && y >= (1 << 32))
				// if (x > (1 << 30) && y > (1 << 30))
			{
				// First term = Xhi (4-bits) * (Y (34-bits) * 2^30 % mod)
				// Second term = XLo (30-bits) * Y (34-bits) 
				return (((x >> 30) * ((y << 30) % mod) + y * (x & ((1 << 30) - 1))) % mod);
			}

			// 30 bit times 34 bits < 64 bits
			var z = x * y;
			if (z >= mod) z %= mod;
			return z;
		}

		public static long Mult42Bit(long x, long y, long mod)
		{
			// x,y,mod must fit within 42 bits
			// x and y can be made to fit within 42 bits by modding first
			// 2^42 = 4.39 * 10^12

			// Thirty times faster than MultSlow
			if ((ulong)x <= 1L << 22 || (ulong)y <= 1L << 22 
				|| (ulong)x < 1L << 32 && (ulong)y < 1L << 32)
			{
				var z = x * y;
				if (z >= mod) z %= mod;
				return z;
			}

			// First term = Xhi * (Y  % mod)
			// (maxbit-bits) + maxbit <= 64
			// Second term = XLo (30-bits) * Y (34-bits)
			// bits + maxbit = 64
			return ((x >> 22) * ((y << 22) % mod) + y * (x & ((1L << 22) - 1))) % mod;
		}


		public static ulong MultSlow(ulong a, ulong b, ulong mod)
		{
			// If both integers are positive 32-bit integers, use shortcut
			if ((a | b) <= (1UL << 31))
				return a * b % mod;

			// If mod is positive 32-bit integer, use shortcut
			a %= mod;
			b %= mod;
			if (mod <= (1UL << 31))
				return a * b % mod;

			if (a < b)
			{
				ulong tmp = a;
				a = b;
				b = tmp;
			}

			ulong result = 0;
			ulong y = a;
			while (b > 0)
			{
				if (b % 2 == 1)
				{
					result = result + y;
					if (result >= mod) result -= mod;
				}
				y <<= 1;
				if (y >= mod) y -= mod;
				b >>= 1;
			}
			return result % mod;
		}



		public static int Mult(int a, int b, int mod)
		{
			return (int)((long)a * b % mod);
		}

		public static ulong ModPow(ulong n, ulong p, ulong mod)
		{
			ulong result = 1;
			ulong b = n;
			while (p > 0)
			{
				if ((p & 1) == 1) result = MultSlow(result, b, mod);
				p >>= 1;
				b = MultSlow(b, b, mod);
			}
			return result;
		}

		public static int ModPow(int n, int p, int mod)
		{
			long result = 1;
			long b = n;
			while (p > 0)
			{
				if ((p & 1) == 1) result = result * b % mod;
				p >>= 1;
				b = b * b % mod;
			}
			return (int)result;
		}

		// return a % b (positive value)

		public static int Mod(int a, int b)
		{
			return (a % b + b) % b;
		}
		public static long Mod(long a, long b)
		{
			return (a % b + b) % b;
		}

		// computes b such that ab = 1 (mod n), returns -1 on failure

		public static int ModInverse(int a, int n)
		{
			var d = EuclidAlgo.ExtendedEuclid(a, n, out int x, out int y);
			if (d > 1) return -1;
			return Mod(x, n);
		}

		public static long ModInverse(long a, long n)
		{
			var d = EuclidAlgo.ExtendedEuclid(a, n, out long x, out long y);
			if (d > 1) return -1;
			return Mod(x, n);
		}


		public static int ModInverseFastUntested(int a, int p)
		{
			long res = 1;
			while (a > 1)
			{
				res = res * (p - p / a) % p;
				a = p % a;
			}
			return (int)res;
		}

		public static long ModInverseFastestUntested(long a, long mod)
		{
			long t = 0;
			long r = mod;
			long newt = 1;
			long newr = a;
			while (newr != 0)
			{
				var quotient = r / newr;
				t -= quotient * newt;
				r -= quotient * newr;

				if (r != 0)
				{
					quotient = newr / r;
					newt -= quotient * t;
					newr -= quotient * r;
				}
				else
				{
					r = newr;
					t = newt;
					break;
				}
			}
			if (r > 1) return -1;
			return t >= 0 ? t : t + mod;
		}

		// Old 1.48 1300839167
		// ModPow(n, p-2) 1.39 1300839781
		// CModInverseFastUntested - 1.41 1300839399
		// ModInverseFastestUntested - Giaym 1.37

		public static long ModPow(long n, long p, long mod)
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

		public static long Pow2Prime(long a, long b, long c, long mod)
		{
			return ModPow(a, ModPow(b, c, mod - 1), mod);
		}

		public static long Pow2Coprime(long a, long b, long c, long mod)
		{
			var tot = FactorizationSingle.TotientFunctionPR(mod);
			var p = ModPow(b, c, tot);
			return ModPow(a, p, mod);
		}

		public static long ModPow2(long a, long b, long c, long mod)
		{
			// ANALYSIS: https://stackoverflow.com/questions/21367824/how-to-evalute-an-exponential-tower-modulo-a-prime
			// We really only need to check if b^c > log2(mod) or 64
			if (c * Log(b) < Log(long.MaxValue))
				return ModPow(a, ModPow(b, c, long.MaxValue), mod);

			var tot = FactorizationSingle.TotientFunctionPR(mod);
			var p = ModPow(b, c, tot);

			var result = p + tot >= 0 
				? ModPow(a, p + tot, mod) 
				: Mult(ModPow(a, p, mod), ModPow(a, tot, mod), mod);

			return result;
		}

		public class InverseHelper
		{
			int _modulus;
			long _totientm1;

			public InverseHelper(int modulus)
			{
				_modulus = modulus;
				_totientm1 = FactorizationSingle.TotientFunction(modulus) - 1;
			}

			public long Inverse(int m)
			{
				var result = ModPow(m, _modulus, _totientm1);
				return result;
			}
		}


		// SOURCE: http://www.dms.umontreal.ca/~andrew/PDF/BinCoeff.pdf
		public static long LargeComb(long n, long k, long p,
			Func<long, long, long> comb)
		{
			// p must be prime
			long n1 = (n + p - 1) / p;
			long k1 = (k + p - 1) / p;
			long n2 = n % p;
			long k2 = k % p;
			return Mult(comb(n1, k1), comb(n2, k2), p);
		}

		// SOURCE: https://www37.atwiki.jp/uwicoder/pages/2118.html
		public static long C(int n, int r, int p)
		{
			long ret = 1;
			while (true)
			{
				if (r == 0)
					break;
				int N = n % p;
				int R = r % p;
				if (N < R)
					return 0;

				for (int i = 0; i < R; i++)
				{
					ret = ret * (N - i) % p;
				}
				long imul = 1;
				for (int i = 0; i < R; i++)
				{
					imul = imul * (i + 1) % p;
				}
				ret = ret * ModInverseFastestUntested(imul, p) % p;
				n /= p;
				r /= p;
			}
			return ret;
		}

		// n<10^7  O(n/lg n)
		public static long C(int n, int r, int mod, int[] primes)
		{
			if (n < 0 || r < 0 || r > n)
				return 0;
			if (r > n / 2)
				r = n - r;
			int[] a = new int[n];
			for (int i = 0; i < r; i++)
				a[i] = n - i;

			foreach (int p in primes)
			{
				if (p > r)
					break;
				for (long q = p; q <= r; q *= p)
				{
					long m = n % q;
					for (long i = m, j = 0; j < r / q; i += q, j++)
					{
						a[i] /= p;
					}
				}
			}

			long mul = 1;
			for (int i = 0; i < r; i++)
			{
				mul = mul * a[i] % mod;
			}
			return mul;
		}



		#region Experiment

		static void Mult64(ulong u, ulong v, out ulong whi, out ulong wlo)
		{
			var u0 = u & 0xFFFFFFFF;
			var u1 = u >> 32;
			var v1 = v >> 32;
			var v0 = v & 0xFFFFFFFF;

			var t = u0 * v0;
			var w0 = t & 0xFFFFFFFF;
			var k = t >> 32;

			t = u1 * v0 + k;
			var w1 = t & 0xFFFFFFFF;
			var w2 = t >> 32;

			t = u0 * v1 + w1;
			k = t >> 32;

			wlo = (t << 32) + w0;
			whi = u1 * v1 + w2 + k;
		}


		/// <summary>
		/// Divides (x || y) by z, for 64-bit integers x, y,
		/// and z, giving the remainder(modulus) as the result.
		/// </summary>
		/// <param name="dividendHi">Hi word of dividend</param>
		/// <param name="dividendLo">Lo word of dividend</param>
		/// <param name="divisor">Divisor or modulus</param>
		/// <param name="quotientHi">Hi word of quotient</param>
		/// <param name="quotientLo">Lo word of quotient</param>
		/// <returns>The remainder of the division</returns>
		public static ulong Div64(ulong dividendHi, ulong dividendLo,
			ulong divisor,
			out ulong quotientHi, out ulong quotientLo)
		{
			var remainder = dividendHi % divisor;
			for (var i = 1; i <= 64; i++)
			{
				var t = (long)remainder >> 63; // All 1's if x(63) = 1.
				remainder = (remainder << 1) | (dividendLo >> 63); // Shift x || y left
				dividendLo = dividendLo << 1;
				if ((remainder | (ulong)t) >= divisor)
				{
					remainder = remainder - divisor;
					dividendLo = dividendLo + 1;
				}
			}

			quotientHi = dividendHi / divisor;
			quotientLo = dividendLo;
			return remainder;
		}

		#endregion
	}
}
