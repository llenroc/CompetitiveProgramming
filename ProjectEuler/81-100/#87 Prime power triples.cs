using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Solution {
    static void Main(String[] args) {
        int max = 10000000;
        var primes = GetPrimes(max);
        
        var counts = new int[max];
        
        foreach(var p2 in primes)
        {
            long sum2 = p2*1L*p2;
            if (sum2>=max) break;
            foreach (var p3 in primes)
            {
                long sum3 = sum2 + 1L*p3*p3*p3;
                if (sum3>=max) break;

                foreach(var p4 in primes)
                {
                    long sum4 = sum3 + 1L*p4*p4*p4*p4;
                    if (sum4>=counts.Length) break;
                    counts[sum4]=1;
                }
            }
        }
        
        int sum = 0;
        for (int i=0; i<counts.Length; i++)
            counts[i] = sum = sum + counts[i];
        
        int t = int.Parse(Console.ReadLine());
        while (t-->0)
        {
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine(counts[n]);
        }
            
        }
    
    	public static BitArray GetPrimeSet(int max)
		{
			var isPrime = new BitArray(max + 1, true)
			{
				[0] = false,
				[1] = false
			};

			// Should be 4
			for (int i = 4; i <= max; i += 2)
				isPrime[i] = false;

			for (int i = 3; i <= max; i += 2)
			{
				if (!isPrime[i]) continue;
				// NOTE: Squaring causes overflow
				for (long j = (long)i * i; j <= max; j += i + i)
					isPrime[(int)j] = false;
			}

			return isPrime;
		}

		public static int[] GetPrimes(int max)
		{
			var isPrime = GetPrimeSet(max);
			int count = 1;
			for (int i = 3; i <= max; i += 2)
				if (isPrime[i])
					count++;

			var primes = new int[count];
			int p = 0;
			primes[p++] = 2;
			for (int i = 3; i <= max; i += 2)
				if (isPrime[i])
					primes[p++] = i;
			return primes;
		}
    
}