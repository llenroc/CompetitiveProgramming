namespace ProjectEuler.Problem9
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

				long max = -1;
				long aMax = n - 2;
				for (long a = (n + 2) / 3; a <= aMax; a++)
				{
					long n2 = n - a;
					long bMin = (n2 + 1) / 2;
					long bMax = Math.Min(a, n2 - 1);
					if (a * bMax * bMax <= max) continue;

					for (long b = bMin; b <= bMax; b++)
					{
						long c = n - a - b;
						if (a * a == b * b + c * c)
							max = Math.Max(a * b * c, max);
					}
				}

				Console.WriteLine(max);
			}
		}
	}

}
