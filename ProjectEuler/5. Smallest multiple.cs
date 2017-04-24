namespace ProjectEuler.Problem5
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	class Solution
	{

		static void Main(String[] args)
		{
			int t = Convert.ToInt32(Console.ReadLine());
			for (int a0 = 0; a0 < t; a0++)
			{
				int n = Convert.ToInt32(Console.ReadLine());
				Console.WriteLine(Enumerable.Range(1, n).Aggregate(lcm));
			}
		}

		static int gcd(int a, int b)
		{
			if (a == 0) return b;
			if (b < a) return gcd(b, a);
			return gcd(b % a, a);
		}

		static int lcm(int a, int b)
		{
			return a * b / gcd(a, b);
		}

	}

}
