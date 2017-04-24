namespace HackerRank.WalmartLabs
{

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	public class BracketSolution
	{
		public static void Driver(String[] args)
		{
			/* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */

			var str =
				@"1
8 6
";

			Console.SetIn(new StringReader(str));

			int tests = int.Parse(Console.ReadLine());
			for (int i = 0; i < tests; i++)
			{
				var input = Console.ReadLine().Split().Select(int.Parse).ToArray();
				var n = input[0];
				var k = input[1];

				Console.WriteLine(SVCountN(n, k));
			}
		}

		static int SVCountN(int n, int k)
		{
			if (n % 2 == 1) return 0;
			int p = n / 2;
			if (p <= 0) return k == 0 ? 1 : 0;
			if (p == 1) return k <= 1 ? 1 : 0;
			return SVCount(p, k);
		}

		static int SVCount(int p, int k)
		{
			if (k == 0) return Catalan(p) + 1;
			if (k == 1) return Catalan(p);
			if (p <= 0) return 0;
			if (k >= 2 * p) return 0;

			int[,] tb = new int[p + 1, k];
			for (int p2 = 1; p2 <= p; p2++)
			{
				tb[p2, 1] = 1;
				int limit = Math.Min(k, 2 * p2);
				for (int k2 = 2; k2 < limit; k2++)
				{
					// Parens
					tb[p2, k2] = tb[p2 - 1, k2];

					// Concatenation
					for (int p3 = 1; p3 < p2; p3++)
					for (int k3 = k2 - 2; k3 > 0; k3--)
						//for (int k3=Math.Min(k2-2,2*p3-1); k3>0; k3--)
					{
						//Console.WriteLine("p2={0} k2={1} p3={2} k3={3}", p2,k2,p3,k3);
						var addend = Mult(tb[p3, k3], tb[p2 - p3, k2 - 1 - k3]);
						//Console.WriteLine("tb[{0},{1}] += {2}", p2,k2,addend);
						tb[p2, k2] = (tb[p2, k2] + addend) % MOD;
					}
				}
			}

			int valid = Catalan(p);
			for (int kk = 1; kk < k; kk++)
			{
				//Console.WriteLine("valid={0} tb[{2},{3}]={1}",valid, tb[p,kk], p, kk);
				valid = Minus(valid, tb[p, kk]);
			}
			return valid;
		}

		/*    static int SVCount(int p, int k)
			{
				if (k == 0) return Catalan(p) + 1;
				if (k == 1) return Catalan(p);
				if (p <= 0) return 0;
				if (k >= 2*p) return 0;
	
				int[,] tb = new int[p+1,k];
				for (int p2=1; p2<=p; p2++)
				{
					tb[p2,1] = 1;
					int limit = k;//Math.Min(k,2*p2-1);
					for (int k2=2; k2<limit; k2++)
					{
						// Parens
						tb[p2,k2] = tb[p2-1,k2];
	
						// Concatenation
						for (int p3=1; p3<p2; p3++)
							for (int k3=k2-2; k3>0; k3--)
							//for (int k3=Math.Min(k2-2,2*p3-1); k3>0; k3--)
						{
							//Console.WriteLine("p2={0} k2={1} p3={2} k3={3}", p2,k2,p3,k3);
							var addend = Mult(tb[p3,k3], tb[p2-p3,k2-1-k3]);
							//Console.WriteLine("tb[{0},{1}] += {2}", p2,k2,addend);
							tb[p2,k2] = (tb[p2, k2] + addend) % MOD;
						}
					}
				}
	
				int valid = Catalan(p);
				for (int kk=1; kk<k; kk++)
				{
					//Console.WriteLine("valid={0} tb[{2},{3}]={1}",valid, tb[p,kk], p, kk);
					valid = Minus(valid, tb[p,kk]);
				}
				return valid;
			}
		*/

		public static int Catalan(int n)
		{
			return Div(Comb(2 * n, n), 1 + n);
		}

		public static int Comb(int n, int k)
		{
			int result = 1;
			int den = 1;
			for (int i = 1; i <= k; i++)
			{
				result = Mult(result, n - i + 1);
				den = Mult(den, i);
			}

			result = Div(result, den);
			return result;
		}


		public const int MOD = 1000 * 1000 * 1000 + 7;

		public static int Inverse(long n)
		{
			return ModPow(n, MOD - 2);
		}

		public static int Add(long left, long right)
		{
			return (int) ((left + right) % MOD);
		}

		public static int Minus(long left, long right)
		{
			return (int) ((left + MOD - right) % MOD);
		}

		public static int Mult(long left, long right)
		{
			return (int) ((left * right) % MOD);
		}

		public static int Div(long left, long divisor)
		{
			return Mult(left, Inverse(divisor));
		}

		public static int ModPow(long n, long p)
		{
			long b = n;
			long result = 1;
			while (p != 0)
			{
				if ((p & 1) != 0)
					result = (result * b) % MOD;
				p >>= 1;
				b = (b * b) % MOD;
			}
			return (int) result;
		}
	}
}