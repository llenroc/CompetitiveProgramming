using Softperson.Mathematics;

namespace HackerRank.Contests.WalmartLabs.FiboWalmart
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Softperson.Algorithms;

	public partial class Solution
	{
		static void Driver(String[] args)
		{
			/* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */

			int q = int.Parse(Console.ReadLine());


			//for (int i=0; i<8; i++)
			//    Console.WriteLine(i + " -> " + Fib(i-1));


			for (int i = 0; i < q; i++)
			{
				Console.ReadLine();
				var array = Console.ReadLine().Split().Select(int.Parse).ToArray();
				Console.WriteLine(Fibo(array));
			}
		}

		public static int Fibo(int[] array)
		{
			Matrix m = new Matrix();
			int n = array.Length;

			var id = new Matrix(1, 0, 0, 1);
			var rightmat = new Matrix();
			for (int i = array.Length - 1; i >= 0; i--)
			{
				var mat = FibMatrix(array[i]);
				rightmat = mat * (id + rightmat);
				m += rightmat;
			}

			long f1 = 0;
			long f0 = 1;
			m.Apply(ref f1, ref f0);
			return (int) f1;
		}

		public const int MOD = 1000 * 1000 * 1000 + 7;

		public static int Inverse(long n)
		{
			return ModPow(n, MOD - 2);
		}

		public static int Mult(long left, long right)
		{
			return (int) ((left * right) % MOD);
		}

		public static int Add(int left, int right)
		{
			return ((left + right) % MOD);
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

		public static Matrix FibMatrix(int n)
		{
			if (n < 0)
				return new Matrix();

			return new Matrix(1, 1, 1, 0).Pow(n);
		}
	}

}