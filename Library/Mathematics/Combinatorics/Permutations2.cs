using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Softperson.Mathematics
{
	public static partial class Permutations
	{

		#region Combinations

		/// <summary>
		/// Combinationse with replacement.
		/// </summary>
		/// <param name="n">The n.</param>
		/// <param name="k">The k.</param>
		/// <returns></returns>
		public static long CombinationsWithReplacement(int n, int k)
		{
			// http://www.mathsisfun.com/combinatorics/combinations-permutations.html
			// Repetition is Allowed: such as coins in your pocket(5,5,5,10,10)
			// No Repetition: such as lottery numbers(2, 14, 15, 27, 30, 33)

			return Combinations(n + k - 1, k);
		}

		/// <summary>
		/// Combinationse with replacement.
		/// </summary>
		/// <param name="n">The n.</param>
		/// <param name="k">The k.</param>
		/// <returns></returns>
		public static long BeggarsCombinations(int n, int k)
		{
			// Ways to assign n coins to k beggars
			return Combinations(n + k - 1, k - 1);
		}

		/// <summary>
		/// Combinations without replacement.
		/// </summary>
		/// <param name="n">The n.</param>
		/// <param name="k">The k.</param>
		/// <returns></returns>
		public static long Combinations(long n, long k)
		{
			if (k <= 0)
			{
				if (k == 0) return 1;
				var combo = Combinations(-k - 1, n - k);
				if ((n - k) % 2 != 0)
					combo *= -1;
				return combo;
			}

			if (k + k > n)
			{
				if (k > n) return 0;
				return Combinations(n, n - k);
			}

			var result = 1L;
			var top = n - (k - 1);
			var bottom = 1L;

			while (bottom <= k)
			{
				result = (result * top) / bottom;
				bottom++;
				top++;
			}

			return result;
		}
		
		public static long MultinomialCoefficient(params long[] coefficients)
		{
			var n = coefficients.Sum();
			throw new NotImplementedException();
		}

		/// <summary>
		/// Calculates the factorial of n mod p
		/// O(p logp(n))
		/// see: http://e-maxx.ru/algo/modular_factorial
		/// </summary>
		/// <param name="n">The n.</param>
		/// <param name="p">The p.</param>
		/// <returns></returns>
		public static int FactMod(int n, int p)
		{
			int res = 1;
			while (n > 1)
			{
				res = (res * ((n / p) % 2 != 0 ? p - 1 : 1)) % p;
				for (int i = 2; i <= n % p; ++i)
					res = (res * i) % p;
				n /= p;
			}
			return res % p;
		}

		public static int FindDerangement(int n)
		{
			long sum = 0;
			long np = Fact(n);
			for (int i = n; i >= 0; i--)
			{
				long factor = np * InverseFact(i) % MOD;
				sum = factor - sum;
			}
			return (int)Fix(sum);
		}

		public static int Derangements(int n, int mod)
		{
			if (n < 2) return n == 0 ? 1 : 0;
			long a = 0, b = 1;
			for (int i = 3; i <= n; i++)
			{
				long c = (b + a) * (i - 1) % mod;
				a = b; b = c;
			}
			return (int)b;
		}

		public static int[] DerangementsTable(int n, int mod)
		{
			return BuildFactorialTable(n, mod, 1, 0);
		}

		public static int[] FactorialTable(int n, int mod)
		{
			return BuildFactorialTable(n, mod, 1, 1);
		}

		static int[] BuildFactorialTable(int n, int mod, int a, int b)
		{
			var dp = new int[Math.Max(n + 1, 2)];
			dp[0] = a;
			dp[1] = b;
			for (int i = 2; i <= n; i++)
				dp[i] = (dp[i - 1] + dp[i - 2]) * (i - 1) % mod;
			return dp;
		}

		static double Stirling(int n, int k)
		{
			if (k == 0) return n == 0 ? 1 : 0;
			if (k < 0) return 0;

			double sum = 0;
			for (int j = 0; j <= k; ++j)
			{
				var a = (k - j) % 2 == 1 ? -1 : 1; ;
				sum += a * Comb(k, j) * ModPow(j, n);
			}
			return sum / Fact(k);
		}

		public static long Comb(int n, int k)
		{
			if (k <= 1) return k == 1 ? n : k == 0 ? 1 : 0;
			if (k + k > n) return Comb(n, n - k);
			return Mult(Mult(Fact(n), InverseFact(k)), InverseFact(n - k));
		}

		static List<long> _fact;
		static List<long> _ifact;

		public static long Fact(int n)
		{
			if (_fact == null) _fact = new List<long>(100) { 1 };
			for (int i = _fact.Count; i <= n; i++)
				_fact.Add(Mult(_fact[i - 1], i));
			return _fact[n];
		}

		public static long InverseFact(int n)
		{
			if (_ifact == null) _ifact = new List<long>(100) { 1 };
			for (int i = _ifact.Count; i <= n; i++)
				_ifact.Add(Div(_ifact[i - 1], i));
			return _ifact[n];
		}

		#endregion



		#region Helpers
		const int MOD = 1000 * 1000 * 1000 + 7;


		public static int Mult(long left, long right)
		{
			return (int)(left * right % MOD);
		}

		public static long Div(long left, long divisor)
		{
			return left % divisor == 0
				? left / divisor
				: Mult(left, Inverse(divisor));
		}

		public static int Inverse(long n)
		{
			return ModPow(n, MOD - 2);
		}

		public static long Fix(long n)
		{
			return ((n % MOD) + MOD) % MOD;
		}

		public static int ModPow(long n, int p)
		{
			long b = n;
			int result = 1;
			while (p != 0)
			{
				if ((p & 1) != 0)
					result = Mult(result, b);
				p >>= 1;
				b = Mult(b, b);
			}
			return result;
		}

		#endregion
	}
}
