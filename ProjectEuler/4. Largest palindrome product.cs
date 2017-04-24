namespace ProjectEuler.Problem4
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

			var palindromes = new HashSet<int>();
			for (int i = 100; i < 1000; i++)
			{
				int min = Math.Max(i, (100000 + i - 1) / i);
				int max = Math.Min(999, 999999 / i);

				for (int j = min; j <= max; j++)
				{
					var p = i * j;
					if (p / 100000 != p % 10) continue;
					p = p / 10 - (p % 10) * 10000;
					if (p / 1000 != p % 10) continue;
					p = p / 10 - (p % 10) * 100;
					if (p / 10 != p % 10) continue;
					palindromes.Add(i * j);
				}
			}

			var list = palindromes.ToList();
			list.Sort();

			for (int a0 = 0; a0 < t; a0++)
			{
				int n = Convert.ToInt32(Console.ReadLine());
				int i = list.BinarySearch(n);
				if (i < 0) i = ~i;
				Console.WriteLine(list[i - 1]);
			}
		}
	}

}
