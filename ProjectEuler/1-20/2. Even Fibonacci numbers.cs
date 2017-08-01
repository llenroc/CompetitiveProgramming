namespace ProjectEuler.Problem2
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
				long n = Convert.ToInt64(Console.ReadLine());

				long f_2 = 1;
				long f_1 = 2;
				long sum = 0;
				while (f_1 < n)
				{
					if (f_1 % 2 == 0) sum += f_1;
					long tmp = f_1 + f_2;
					f_2 = f_1;
					f_1 = tmp;
				}

				Console.WriteLine(sum);
			}
		}
	}

}
