namespace ProjectEuler.Problem3
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using static System.Math;

	class Solution
	{

		static void Main(String[] args)
		{
			int t = int.Parse(Console.ReadLine());
			var nums = Enumerable.Range(1, t).Select(x => long.Parse(Console.ReadLine())).ToArray();
			var max = nums.Max();

			var sqrt = (int)Math.Ceiling(Math.Sqrt(max));
			if (sqrt < 0 || sqrt > 10e6)
				return;

			var isComposite = new System.Collections.BitArray(sqrt / 2 + 1);

			for (long i = 3; i <= sqrt; i += 2)
			{
				if (isComposite[(int)(i / 2)]) continue;
				for (long j = i * i; j <= sqrt; j += i + i)
					isComposite[(int)(j / 2)] = true;
			}

			var primes = new List<int> { 2 };
			for (int i = 3; i <= sqrt; i += 2)
				if (!isComposite[i / 2])
					primes.Add(i);

			foreach (var n in nums)
				Console.WriteLine(LargestPrime(primes, n));
		}

		public static long LargestPrime(List<int> primes, long n)
		{
			long largest = 0;
			var sqrt = (long)(Math.Ceiling(Math.Sqrt(n)));
			foreach (var p in primes)
			{
				if (p > sqrt) break;
				if (n % p == 0)
				{
					while (n % p == 0)
						n /= p;
					largest = p;
					sqrt = (long)(Math.Ceiling(Math.Sqrt(n)));
					if (n == 1) break;
				}
			}
			return Math.Max(largest, n);
		}
	}

}
