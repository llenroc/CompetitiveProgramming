namespace ProjectEuler.Problem10
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using static System.Math;

	class Solution
	{

		static void Main(String[] args)
		{
			int t = Convert.ToInt32(Console.ReadLine());
			var nums = Enumerable.Range(0, t).Select(x => int.Parse(Console.ReadLine())).ToArray();
			int max = nums.Max();

			var primes = GetPrimes(max);
			var primesum = new List<long>();
			long sum = 0;
			foreach (var p in primes)
			{
				sum += p;
				primesum.Add(sum);
			}

			foreach (var n in nums)
			{
				int left = 0;
				int right = primes.Count - 1;
				while (left <= right)
				{
					int mid = (left + right) / 2;
					if (n >= primes[mid])
						left = mid + 1;
					else
						right = mid - 1;
				}
				Console.WriteLine(primesum[left - 1]);
			}
		}

		public static BitArray GetPrimeSet(int max)
		{
			var isPrime = new BitArray(max + 1, true);
			for (int i = 2; i <= max; i += 2)
				isPrime[i] = false;

			for (int i = 3; i <= max; i += 2)
			{
				if (!isPrime[i]) continue;
				for (long j = (long)i * i; j <= max; j += i + i)
					isPrime[(int)j] = false;
			}

			return isPrime;
		}

		public static List<int> GetPrimes(int max)
		{
			var isPrime = GetPrimeSet(max);
			var primes = new List<int> { 2 };
			for (int i = 3; i <= max; i += 2)
				if (isPrime[i])
					primes.Add(i);
			return primes;
		}

	}

}
