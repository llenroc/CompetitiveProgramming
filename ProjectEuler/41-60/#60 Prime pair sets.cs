using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;

class Solution {
    
    static List<int>[] pairs;
    static int[] primes;
    
    static void Main(String[] args) {
        
        var array = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        int n = array[0];
        int k = array[1];
        
        int max = 100000;
        var isPrime = GetPrimeSet(max);
        int count = 1;
        for (int i = 3; i < n; i += 2)
            if (isPrime[i])
                count++;

/*        for (int i=0; i<6; i++)
            for (int j=0; j<6; j++)
            {
                var pi = Pow(10,i,int.MaxValue);
                var pj = Pow(10,j,int.MaxValue);
                Console.Error.WriteLine($"{pi} . {pj} = {Combine((int)pi,(int)pj)}");
            }
*/        
        
        primes = new int[count];
        int p = 0;
        primes[p++] = 2;
        for (int i = 3; i <= n; i += 2)
            if (isPrime[i])
                primes[p++] = i;

        pairs = new List<int>[count];
        for (int i=0; i<primes.Length; i++)
        {
            pairs[i] = new List<int>();
            for (int j=i+1; j<primes.Length; j++)
            {
                var pi = primes[i];
                var pj = primes[j];
                long num = Combine(pi,pj);
                if (num<isPrime.Length ? !isPrime[(int)num] : !FermatProbablyPrime(num)) continue;
                num = Combine(pj, pi);
                if (num<isPrime.Length ? !isPrime[(int)num] : !FermatProbablyPrime(num)) continue;
                pairs[i].Add(j);
            }
        }
        
        var results = new List<int>();
        for (int i=0; i<primes.Length; i++)
        {
            if (pairs[i].Count<k-1) continue;
            Dfs(i, pairs[i], k-1, 0, x=>results.Add(x));
        }
        
        foreach(var result in results.OrderBy(x=>x))
            Console.WriteLine(result);
    }
    
    static void Dfs(int target, ICollection<int> choices, int remaining, int sum, Action<int> action, int depth=0)
    {
        if (choices.Count<remaining) return;
        var p = primes[target];
        sum += p;

        var indent = new string(' ', depth);
        Console.Error.WriteLine($"{indent}-> Dfs({target} - {p}, {choices.Count}, {remaining}, {sum})");
            
        if (remaining==0)
        {
            Console.Error.WriteLine($"{indent}-> {sum}");
            action(sum);
            return;
        }
        
        foreach(var v in choices)
        {
            var hashset = new HashSet<int>();
            foreach(var v2 in pairs[v])
                if (choices.Contains(v2))
                    hashset.Add(v2);
            
            Dfs(v, hashset, remaining-1, sum, action, depth+1);
        }
        
    }
    
    static long Combine(long num, long num2)
    {
        if (num2<1000)
        {
            if (num2>=100) return num*1000 + num2;
            if (num2>=10) return num*100 + num2;
            return num * 10 + num2;
        }
        if (num2<10000) return num * 10000 + num2;
        if (num2<100000) return num * 100000 + num2;
        return num *1000000 + num2;
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

    static int[] PrimesBetween7And61 = new int[] { 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61,  };

    public static bool MayBePrime(long n)
    {
        const int PrimeFilter235 = 0
            | 1 << 1 | 1 << 7
            | 1 << 11 | 1 << 13 | 1 << 17 | 1 << 19
            | 1 << 23 | 1 << 29;

        if ((PrimeFilter235 & 1 << (int)(n % 30)) == 0)
            return false;

        // Quick test
        foreach (var v in PrimesBetween7And61)
            if (n % v == 0)
                return false;

        return true;
    }

    public static bool FermatProbablyPrime(long n)
    {
		const long PrimesUnder64 = 0
            | 1L << 02 | 1L << 03 | 1L << 05 | 1L << 07
            | 1L << 11 | 1L << 13 | 1L << 17 | 1L << 19
            | 1L << 23 | 1L << 29
            | 1L << 31 | 1L << 37
            | 1L << 41 | 1L << 43 | 1L << 47
            | 1L << 53 | 1L << 59
            | 1L << 61;

        // 2 is the first prime
        if (n < 2) return false;

        // Return primes under 64 in constant time
        // Important Step! witnesses < 64 <= n
        if (n < 64) return (PrimesUnder64 & 1L << (int)n) != 0;

        return MayBePrime(n) && Pow(2, n - 1, n) == 1 && Pow(3, n - 1, n) == 1 &&  Pow(5, n - 1, n) == 1;
    }

    public static long Pow(long n, long p, long mod)
    {
        long result = 1;
        long b = n;
        while (p > 0)
        {
            if ((p & 1) == 1) result = Mult(result, b, mod);
            p >>= 1;
            b = Mult(b, b, mod);
        }
        return result;
    }

    public static long Mult(long a, long b, long mod)
    {
        return (long)Mult42Bit((ulong)a, (ulong)b, (ulong)mod);
    }
    
   		public static ulong Mult42Bit(ulong x, ulong y, ulong mod)
		{
			// x,y,mod must fit within 42 bits
			// x and y can be made to fit within 42 bits by modding first
			// 2^42 = 4.39 * 10^12

			// Thirty times faster than MultSlow
			if (x <= 1ul << 22 || y <= 1ul << 22 || x < 1 << 32 && y < 1 << 32)
			{
				var z = x * y;
				if (z >= mod) z %= mod;
				return z;
			}

			// First term = Xhi * (Y  % mod)
			// (maxbit-bits) + maxbit <= 64
			// Second term = XLo (30-bits) * Y (34-bits)
			// bits + maxbit = 64
			return ((x >> 22) * ((y << 22) % mod) + y * (x & ((1ul << 22) - 1))) % mod;
		}

    
    public static int Log2(long value)
    {
        var log = 0;
        if ((ulong)value >= (1UL << 24))
        {
            if ((ulong)value >= (1UL << 48))
            {
                log = 48;
                value = (long)((ulong)value >> 48);
            }
            else
            {
                log = 24;
                value >>= 24;
            }
        }
        if (value >= (1 << 12))
        {
            log += 12;
            value >>= 12;
        }
        if (value >= (1 << 6))
        {
            log += 6;
            value >>= 6;
        }
        if (value >= (1 << 3))
        {
            log += 3;
            value >>= 3;
        }
        return log + (int)(value >> 1 & ~value >> 2);
    }
}