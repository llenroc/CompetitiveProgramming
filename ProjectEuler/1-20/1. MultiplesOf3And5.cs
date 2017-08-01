namespace ProjectEuler.Problem1
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	class MultiplesOf3And5
	{

		static void Main(String[] args)
		{
			int t = Convert.ToInt32(Console.ReadLine());
			for (int a0 = 0; a0 < t; a0++)
			{
				int n = Convert.ToInt32(Console.ReadLine());

				long sum = 0;
				while (n % 15 > 0)
				{
					n--;
					if (n % 5 == 0 || n % 3 == 0) sum += n;
				}

				long nd = n / 15;
				sum += (0 + 3 + 5 + 6 + 9 + 10 + 12) * nd + Math.Max(0, 7 * 15 * nd * (nd - 1) / 2);
				Console.WriteLine(sum);
			}
		}
	}
}
