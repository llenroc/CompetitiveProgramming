using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Softperson.Mathematics.ModularMath;

namespace Softperson.Mathematics.Combinatorics
{
	public class DigitProcessing
	{
		public static int FindNthDigit(long n)
		{
			if (n == 0)
				return 0;

			int digits = 1;
			long factor = 1;
			n--;
			while (true)
			{
				var addend = 9 * digits * factor;
				if (n < addend) break;
				n -= addend;
				factor *= 10;
				digits++;
			}

			var mod = n % digits;
			n /= digits;
			n += factor;
			while (mod + 1 < digits)
			{
				mod++;
				n /= 10;
			}

			return (int)(n % 10);
		}

		public static long MaskRepeated(long n)
		{
			long mask = 0;
			while (n > 0)
			{
				var d = n % 10;
				n /= 10;
				long bit = 1L << (6 * (int)d);
				while ((mask & bit) != 0) bit <<= 1;
				mask |= bit;
			}
			return mask;
		}

		public static int DigitMask(long n)
		{
			var mask = 0;
			while (n > 0)
			{
				var d = n % 10;
				n /= 10;
				int bit = 1 << (int)d;
				mask |= bit;
			}
			return mask;
		}

		public static long FingerPrint(long n)
		{
			// Permutations of the same number have the same fingerprint
			long result = 0;
			while (n > 0)
			{
				var digit = (int)(n % 10);
				n /= 10;
				result += 1L << 5 * digit;
			}
			return result;
		}


		public static int PandigitMask(long n)
		{
			var mask = 0;
			while (n > 0)
			{
				var d = n % 10;
				n /= 10;
				int bit = 1 << (int)d;
				if ((mask & bit) != 0) return 0;
				mask |= bit;
			}
			return mask;
		}

		public static long TotalSumOfSquareDigits(string number, long mod)
		{
			int n = number.Length;
			int max = 81 * n;

			var counts = new long[n, max + 1];
			var sums = new long[n, max + 1];
			counts[0, 0] = 1;

			var squares = new HashSet<long>();
			for (int i = 1; i * i <= max; i++)
				squares.Add(i * i);

			long fact = 1;
			for (int i = 1; i < n; i++)
			{
				for (int k = 81 * i; k >= 0; k--)
				{
					long c = counts[i - 1, k];
					long s = sums[i - 1, k];
					for (int j = 0; j < 10; j++)
					{
						var k2 = k + j * j;
						var c2 = counts[i, k2] + c;
						if (c2 >= mod) c2 -= mod;
						counts[i, k2] = c2;
						sums[i, k2] = (sums[i, k2] + fact * j % mod * c % mod + s) % mod;
					}
				}

				fact = (fact * 10) % mod;
			}

			long result = 0;
			long sum = 0;
			fact = ModPow(10, n - 1, mod);
			long inv10 = ModInverse(10, mod);
			long prefix = 0;
			for (int i = 0; i < n; i++)
			{
				int len = n - i - 1;
				int d = number[i] - '0';

				for (int j = 0; j < d; j++)
				{
					var prefix2 = (j * fact % mod + prefix) % mod;
					var sum2 = sum + j * j;
					foreach (var sq in squares)
					{
						if (sq >= sum2)
							result += prefix2 * counts[len, sq - sum2] % mod + sums[len, sq - sum2];
					}
				}

				sum += d * d;
				prefix = (d * fact % mod + prefix) % mod;
				fact = fact * inv10 % mod;
			}

			if (squares.Contains(sum))
				result += prefix;

			return (result % mod + mod) % mod;
		}

		public static long CountSumOfSquareDigits(string s, long mod)
		{
			int n = s.Length;
			int max = 81 * n;

			var array = new long[n, max + 1];
			array[0, 0] = 1;

			var squares = new HashSet<long>();
			for (int i = 1; i * i <= max; i++)
				squares.Add(i * i);

			for (int i = 1; i < n; i++)
			{
				for (int k = 81 * i; k >= 0; k--)
				{
					long c = array[i - 1, k];
					for (int j = 0; j < 10; j++)
					{
						var k2 = k + j * j;
						var c2 = array[i, k2] + c;
						if (c2 >= mod) c2 -= mod;
						array[i, k2] = c2;
					}
				}
			}

			long result = 0;
			long sum = 0;
			for (int i = 0; i < n; i++)
			{
				int len = n - i - 1;
				int d = s[i] - '0';

				for (int j = 0; j < d; j++)
				{
					var sum2 = sum + j * j;

					// Could iterate over squares instead
					//for (int k = 81 * len; k >= 0; k--)
					//{
					//	if (!squares.Contains(sum2 + k)) continue;
					//	result += array[len, sum2 + k];
					//}
					foreach (var sq in squares)
					{
						if (sq >= sum2)
							result += array[len, sq - sum2];
					}
				}

				sum += d * d;
			}

			if (squares.Contains(sum))
				result++;

			return (result%mod + mod) %mod;
		}
		
	}
}
