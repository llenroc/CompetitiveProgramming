using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {

        var primes = GetPrimes(1000*1000*4);
        
        long [] table = new long[primes.Length];
        for (int i=0; i<table.Length; i++)
        {
            long n = i+1;
            long p = primes[i];
            long p2 = p*1L*p;
            long b = Pow(p-1,n,p2) + Pow(p+1,n,p2);
            if (b>=p2) b -= p2;
            table[i]= i>0 ? Math.Max(b, table[i-1]) : b;
        }
        
        int t = int.Parse(Console.ReadLine());
        while (t-->0)
        {
            long B = long.Parse(Console.ReadLine());
            
            int left = 0;
            int right = table.Length-1;
            while (left<=right)
            {
                int mid = (left + right) >> 1;
                var val = table[mid];
                if (val <= B)
                    left = mid +1;
                else
                    right = mid - 1;
            }
            Console.WriteLine(left+1);
        }
    }

    public static long Pow(long n, long p, long mod)
	{
		long b = n;
		long result = 1;
		while (p != 0)
		{
			if ((p & 1) != 0)
				result = Mult(result, b, mod);
			p >>= 1;
			b = Mult(b, b, mod);
		}
		return result;
	}

    public static long Mult(long a, long b, long mod)
    {
        // Ten times faster than MultSlow
        if ((ulong)(a) >= (ulong)mod) a %= mod;
        if (a < 0) a += mod;
        if ((ulong)(b) >= (ulong)mod) b %= mod;
        if (b < 0) b += mod;

        long ret = 0;
        int step = 62 - Log2(mod);
        for (int x = Log2(b); x >= 0; x -= step)
        {
            int shift = Math.Min(x + 1, step);
            ret <<= shift;
            ret %= mod;
            ret += a * ((long)((ulong)b >> (x - shift + 1)) & (1L << shift) - 1);
            ret %= mod;
        }
        return ret;
    }

    public static int Log2(long value)
    {
        if (value <= 0)
            return value == 0 ? -1 : 63;

        var log = 0;
        if (value >= 0x100000000L)
        {
            log += 32;
            value >>= 32;
        }
        if (value >= 0x10000)
        {
            log += 16;
            value >>= 16;
        }
        if (value >= 0x100)
        {
            log += 8;
            value >>= 8;
        }
        if (value >= 0x10)
        {
            log += 4;
            value >>= 4;
        }
        if (value >= 0x4)
        {
            log += 2;
            value >>= 2;
        }
        if (value >= 0x2)
        {
            log += 1;
        }
        return log;
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

