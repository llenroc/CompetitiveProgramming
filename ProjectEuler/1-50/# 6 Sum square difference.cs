namespace ProjectEuler.Problem6
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
				long n = Convert.ToInt32(Console.ReadLine());

				long sumq = n * (n + 1) * (2 * n + 1) / 6;
				long sum = n * (n + 1) / 2;
				long qsum = sum * sum;


				long diff = Math.Abs(sumq - qsum);
				Console.WriteLine(diff);

			}
		}
	}

}
