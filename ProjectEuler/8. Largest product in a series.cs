namespace ProjectEuler.Problem8
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
				string[] tokens_n = Console.ReadLine().Split(' ');
				int n = Convert.ToInt32(tokens_n[0]);
				int k = Convert.ToInt32(tokens_n[1]);
				string num = Console.ReadLine();

				long product = 1;
				long max = 0;
				int zeroes = 0;
				for (int i = 0; i < num.Length; i++)
				{
					var d = num[i] - '0';
					if (d == 0)
						zeroes++;
					else
						product *= d;

					var d2 = i < k ? 1 : num[i - k] - '0';
					if (d2 == 0)
						--zeroes;
					else
						product /= d2;


					if (product > max && zeroes == 0 && i >= k - 1)
						max = product;
				}

				Console.WriteLine(max);
			}
		}
	}

}
