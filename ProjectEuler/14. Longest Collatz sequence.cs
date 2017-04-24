namespace ProjectEuler.Problem14
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	class Solution
	{
		static void Main(String[] args)
		{

			var save = new int[60000001];
			var longest = new int[6000001];

			int best = 0;
			int bestLength = 0;
			for (int i = 1; i < longest.Length; i++)
			{
				int len = Collatz(i, save);
				if (len >= bestLength)
				{
					best = i;
					bestLength = len;
				}
				longest[i] = best;
			}

			int tc = int.Parse(Console.ReadLine());
			while (tc-- > 0)
			{
				int n = int.Parse(Console.ReadLine());
				Console.WriteLine(longest[n]);
			}
		}

		static int Collatz(long n, int[] save)
		{
			if (n <= 1) return 1;

			int extra = 0;
			if ((n & 1) == 1)
			{
				n = 3 * n + 1;
				extra = 1;
			}

			var nn = n >> 1;
			if (nn < save.Length && save[nn] != 0)
				return save[nn] + extra;

			int result = 1 + Collatz(nn, save);

			var result2 = result;
			while (nn < save.Length && save[nn] == 0)
			{
				save[nn] = result2++;
				nn <<= 1;
			}

			return result + extra;
		}

		static int Collatz2(long n, int[] save)
		{
			if (n <= 1) return 1;

			if (n < save.Length && save[n] != 0)
				return save[n];

			int result = 1 + ((n & 1) == 1 ? Collatz(3 * n + 1, save) : Collatz(n / 2, save));

			var result2 = result;
			int iters = 4;
			while (n < save.Length && save[n] == 0 && iters-- > 0)
			{
				save[n] = result2++;
				n <<= 1;
			}

			return result;
		}
	}
}
