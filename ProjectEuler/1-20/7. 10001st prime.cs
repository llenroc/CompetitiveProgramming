namespace ProjectEuler.Problem7
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
			var nums = Enumerable.Range(0, t).Select(x => int.Parse(Console.ReadLine())).ToArray();
			int max = nums.Max();

			var primes = new List<int> { 2, 3, 5, 7, 11, 13, 17, 19 };

			for (int i = 23; primes.Count < max; i++)
			{
				bool prime = true;
				foreach (var p in primes)
				{
					if (i % p == 0)
					{
						prime = false;
						break;
					}
				}
				if (prime)
					primes.Add(i);
			}

			foreach (var n in nums)
				Console.WriteLine(primes[n - 1]);
		}


	}

}
