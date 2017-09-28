using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using static System.Array;
using static System.Math;
using static Library;

class Solution
{
	// public const int MOD = 1000000007;

	static int[] Primes = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103 };


	public void solve()
	{
		int t = Ni();
		while (t-- > 0)
		{
			int L = Ni();
			int[] a = Ni(6);

			var checks = new List<PrimeCheck>();

			bool empty = false;
			foreach (var p in Primes)
			{
				var pc = new PrimeCheck(p);

				int count = 0;

				for (int i = 0; i < p; i++)
				{
					var i2 = i * i;
					foreach (var v in a)
					{
						if ((i2 + v) % p == 0)
						{
							count++;
							pc.Mask[i] = true;
							break;
						}
					}
				}

				//if (count >= p)
				//	empty = true;
				//else if (count > 0)
					checks.Add(pc);
			}

			if (empty)
			{
				WriteLine(0);
				continue;
			}

			var start = a.Any(x => x % 2 == 1) ? 2 : 1;

			long sum = 0;
			for (long n = 1; n < L; n += n >= 100+start ? 2 : 1)
			{
				bool bad = false;

                if (n>103)
				foreach (var pc in checks)
				{
					if (pc.Mask[n % pc.Prime])
					{
						bad = true;
						break;
					}
				}

				if (bad) continue;

				var n2 = n * n;
				foreach (var v in a)
				{
					if (!MillerRabin.IsPrime(v + n2))
					{
						bad = true;
						break;
					}
				}

				if (bad) continue;

				for (int x = 1; x < 6; x++)
				{
					for (int y = a[x - 1] + 2; y < a[x]; y += 2)
					{
						if (MillerRabin.IsPrime(y + n2))
						{
							bad = true;
							break;
						}
					}
				}

				if (bad) continue;
				sum += n;
				//Console.Error.WriteLine($"Found {n}");
			}

			WriteLine(sum);
		}
	}

	public class PrimeCheck
	{
		public int Prime;
		public bool[] Mask;

		public PrimeCheck(int p)
		{
			Prime = p;
			Mask = new bool[p];

		}
	}

	public bool TestPrime(long n)
	{
		if (n % 2 == 0) return n == 2;
		for (long x = 3; x * x <= n; x += 2)
		{
			if (n % x == 0) return false;
		}
		return n > 1;
	}

}

public partial class Library
{
	#region Mod Math
	public const int MOD = 1000 * 1000 * 1000 + 7;

	static int[] _inverse;
	public static long Inverse(long n)
	{
		long result;

		if (_inverse == null)
			_inverse = new int[3000];

		if (n < _inverse.Length && (result = _inverse[n]) != 0)
			return result - 1;

		result = ModPow(n, MOD - 2);
		if (n < _inverse.Length)
			_inverse[n] = (int)(result + 1);
		return result;
	}

	public static long Mult(long left, long right)
	{
		return (left * right) % MOD;
	}

	public static long Div(long left, long divisor)
	{
		return left % divisor == 0
			? left / divisor
			: Mult(left, Inverse(divisor));
	}

	public static long Subtract(long left, long right)
	{
		return (left + (MOD - right)) % MOD;
	}

	public static long Fix(long m)
	{
		var result = m % MOD;
		if (result < 0) result += MOD;
		return result;
	}

	public static long ModPow(long n, long p, long mod = MOD)
	{
		long b = n;
		long result = 1;
		while (p != 0)
		{
			if ((p & 1) != 0)
				result = (result * b) % mod;
			p >>= 1;
			b = (b * b) % mod;
		}
		return result;
	}

	public static long Pow(long n, long p)
	{
		long b = n;
		long result = 1;
		while (p != 0)
		{
			if ((p & 1) != 0)
				result *= b;
			p >>= 1;
			b *= b;
		}
		return result;
	}

	#endregion

}

public static class MillerRabin
{

	private const long PrimesUnder64 = 0
									   | 1L << 02 | 1L << 03 | 1L << 05 | 1L << 07
									   | 1L << 11 | 1L << 13 | 1L << 17 | 1L << 19
									   | 1L << 23 | 1L << 29
									   | 1L << 31 | 1L << 37
									   | 1L << 41 | 1L << 43 | 1L << 47
									   | 1L << 53 | 1L << 59
									   | 1L << 61;

	private const int PrimeFilter235 = 0
									   | 1 << 1 | 1 << 7
									   | 1 << 11 | 1 << 13 | 1 << 17 | 1 << 19
									   | 1 << 23 | 1 << 29;

	// Witnesses must all be less that 64-2=62
	// We filter out numbers below 64
	// https://miller-rabin.appspot.com
	static readonly int[] WitnessFast = { 2, 7,  }; //  4759123141
	static readonly int[] Witness32 = { 2, 7, 61 }; //  4759123141
	static readonly long[] Witness40 = { 2, 13, 23, 1662803 }; //  1122004669633
															   //static readonly long[] Witness41 = { 2, 3, 5, 7, 11, 13 }; // 3,474,749,660,383
	static readonly long[] Witness51 = { 2, 75088, 642735, 203659041, 3613982119 }; // 3071837692357849
	static readonly long[] Witness64 = { 2, 325, 9375, 28178, 450775, 9780504, 1795265022 }; // Can't be witness if w | n

	// TEST CASE: 46817 is a prime

	// Sieve is 10X faster for checking multiple primes.


	public static bool IsPrime32(uint n)
	{
		// 2 is the first prime
		if (n < 2) return false;

		// Return primes under 64 in constant time
		// Important Step! witnesses < 64 <= n
		if (n < 64) return (PrimesUnder64 & 1L << (int)n) != 0;

		// Filter out easy composites (3/4 of positive integers)
		if ((PrimeFilter235 & 1 << (int)(n % 30)) == 0)
			return false;

		// Hard test
		uint s = n - 1;
		while ((s & 1) == 0) { s >>= 1; }

		foreach (var w in Witness40)
		{
			// NOTE: V needs to be long because we are squaring
			long v = ModPow(w, s, n);

			if (v != 1)
			{
				for (var t = s; v != n - 1; t <<= 1)
				{
					if (t >= n - 1)
						return false;
					v = v * v % n;
				}
			}
		}

		return true;
	}


	public static long ModPow(long n, long p, long mod)
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
		// Ten times faster than MultSlow
		if ((ulong)(a) >= (ulong)mod) a %= mod;
		if (a < 0) a += mod;
		if ((ulong)(b) >= (ulong)mod) b %= mod;
		if (b < 0) b += mod;

		long ret = 0;
		int step = 62 - Log2(mod);
		for (int x = Log2(b); x >= 0; x -= step)
		{
			int shift = Min(x + 1, step);
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


	public static bool IsPrime(long n)
	{
		if (n < 2) return false;
		if (n <= int.MaxValue) return IsPrime32((uint)n);

		// Easy Test
		if (!MayBePrime(n))
			return false;

		// Hard test
		var witnesses = n < 1122004669633
			? Witness40
			: n < 3071837692357849
				? Witness51
				: Witness64;

		/* 
		 25% slower
		var witnesses = n < 3474749660383 // 41.6 bits 
			? Witness41
			: Witness64;
		*/

		var s = n - 1;
		while ((s & 1) == 0) { s >>= 1; }

		foreach (var w in WitnessFast)
		{
			// Witnesses can't be a multiple of n
			// The inequality w < 2^31 < n is guaranteed by the 32-bit int rerouting
			// if (w % n == 0) continue;

			long v = ModPow(w, s, n);
			if (v != 1)
			{
				for (var t = s; v != n - 1; t <<= 1)
				{
					if (t >= n - 1)
						return false;
					v = Mult(v, v, n);
				}
			}
		}
		return true;
	}



	// Warning: Needs to be under 64 -- otherwsise prime will be cut out
	static int[] PrimesBetween7And61 = new int[] { 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61 };

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
				return n == v; // Safety check
        
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

		return MayBePrime(n)
			? FermatProbablyPrime(n, 2) && FermatProbablyPrime(n, 3) && FermatProbablyPrime(n, 5)
			: n == 2;
	}

	public static bool FermatProbablyPrime(long n, long b)
	{
		return ModPow(b, n - 1, n) == 1;
	}

	public static bool EulerProbablyPrime(long n)
	{
		// isprime(n) - Test whether n is prime using a variety of pseudoprime tests.*/

		return MayBePrime(n)
			? EulerProbablyPrime(n, 2) && EulerProbablyPrime(n, 3) && EulerProbablyPrime(n, 5)
			: n == 2;
	}

	public static bool EulerProbablyPrime(long n, long b)
	{
		if (!FermatProbablyPrime(n, b)) return false;

		long r = n - 1;
		while ((r & 1) == 0) r >>= 1;

		long c = ModPow(b, r, n);
		if (c != 1)
		{
			while (c != n - 1)
			{
				c = ModPow(c, 2, n);
				if (c == 1) return false;
			}
		}

		return true;
	}
}

class CaideConstants {
    public const string InputFile = null;
    public const string OutputFile = null;
}

static partial class Library
{

	#region Common

	public static void Swap<T>(ref T a, ref T b)
	{
		var tmp = a;
		a = b;
		b = tmp;
	}

	public static void Clear<T>(T[] t, T value = default(T))
	{
		for (int i = 0; i < t.Length; i++)
			t[i] = value;
	}

	public static int BinarySearch<T>(T[] array, T value, int left, int right, bool upper = false)
		where T : IComparable<T>
	{
		while (left <= right)
		{
			int mid = left + (right - left) / 2;
			int cmp = value.CompareTo(array[mid]);
			if (cmp > 0 || cmp == 0 && upper)
				left = mid + 1;
			else
				right = mid - 1;
		}
		return left;
	}

	#endregion

	#region  Input
	static System.IO.Stream inputStream;
	static int inputIndex, bytesRead;
	static byte[] inputBuffer;
	static System.Text.StringBuilder builder;
	const int MonoBufferSize = 4096;

	public static void InitInput(System.IO.Stream input = null, int stringCapacity = 16)
	{
		builder = new System.Text.StringBuilder(stringCapacity);
		inputStream = input ?? Console.OpenStandardInput();
		inputIndex = bytesRead = 0;
		inputBuffer = new byte[MonoBufferSize];
	}

	static void ReadMore()
	{
		inputIndex = 0;
		bytesRead = inputStream.Read(inputBuffer, 0, inputBuffer.Length);
		if (bytesRead <= 0) inputBuffer[0] = 32;
	}

	public static int Read()
	{
		if (inputIndex >= bytesRead) ReadMore();
		return inputBuffer[inputIndex++];
	}

	public static T[] N<T>(int n, Func<T> func)
	{
		var list = new T[n];
		for (int i = 0; i < n; i++) list[i] = func();
		return list;
	}

	public static int[] Ni(int n)
	{
		var list = new int[n];
		for (int i = 0; i < n; i++) list[i] = Ni();
		return list;
	}

	public static long[] Nl(int n)
	{
		var list = new long[n];
		for (int i = 0; i < n; i++) list[i] = Nl();
		return list;
	}

	public static string[] Ns(int n)
	{
		var list = new string[n];
		for (int i = 0; i < n; i++) list[i] = Ns();
		return list;
	}

    public static int Ni()
    {
        var c = SkipSpaces();
        bool neg = c == '-';
        if (neg) { c = Read(); }

        int number = c - '0';
        while (true)
        {
            var d = Read() - '0';
            if ((uint)d > 9) break;
            number = number * 10 + d;
	        if (number < 0) throw new FormatException();
        }
        return neg ? -number : number;
    }

    public static long Nl()
    {
        var c = SkipSpaces();
        bool neg = c=='-';
        if (neg) { c = Read(); }

        long number = c - '0';
        while (true)
        {
            var d = Read() - '0';
            if ((uint)d > 9) break;
            number = number * 10 + d;
	        if (number < 0) throw new FormatException();
        }
		return neg ? -number : number;
    }

	public static char[] Nc(int n)
	{
		var list = new char[n];
		for (int i = 0, c = SkipSpaces(); i < n; i++, c = Read()) list[i] = (char)c;
		return list;
	}

	public static byte[] Nb(int n)
	{
		var list = new byte[n];
		for (int i = 0, c = SkipSpaces(); i < n; i++, c = Read()) list[i] = (byte)c;
		return list;
	}

	public static string Ns()
	{
		var c = SkipSpaces();
		builder.Clear();
		while (true)
		{
			if ((uint)c - 33 >= (127 - 33)) break;
			builder.Append((char)c);
			c = Read();
		}
		return builder.ToString();
	}

	public static int SkipSpaces()
	{
		int c;
		do c = Read(); while ((uint)c - 33 >= (127 - 33));
		return c;
	}
	#endregion

	#region Output

	static System.IO.Stream outputStream;
	static byte[] outputBuffer;
	static int outputIndex;

	public static void InitOutput(System.IO.Stream output = null)
	{
		outputStream = output ?? Console.OpenStandardOutput();
		outputIndex = 0;
		outputBuffer = new byte[65535];
		AppDomain.CurrentDomain.ProcessExit += delegate { Flush(); };
	}

	public static void WriteLine(object obj = null)
	{
		Write(obj);
		Write('\n');
	}

	public static void WriteLine(long number)
	{
		Write(number);
		Write('\n');
	}

	public static void Write(long signedNumber)
	{
		ulong number = (ulong)signedNumber;
		if (signedNumber < 0)
		{
			Write('-');
			number = (ulong)(-signedNumber);
		}

		Reserve(20 + 1); // 20 digits + 1 extra
		int left = outputIndex;
		do
		{
			outputBuffer[outputIndex++] = (byte)('0' + number % 10);
			number /= 10;
		}
		while (number > 0);

		int right = outputIndex - 1;
		while (left < right)
		{
			byte tmp = outputBuffer[left];
			outputBuffer[left++] = outputBuffer[right];
			outputBuffer[right--] = tmp;
		}
	}

	public static void Write(object obj)
	{
		if (obj == null) return;

		var s = obj.ToString();
		Reserve(s.Length);
		for (int i = 0; i < s.Length; i++)
			outputBuffer[outputIndex++] = (byte)s[i];
	}

	public static void Write(char c)
	{
		Reserve(1);
		outputBuffer[outputIndex++] = (byte)c;
	}

	public static void Write(byte[] array, int count)
	{
		Reserve(count);
		Array.Copy(array, 0, outputBuffer, outputIndex, count);
		outputIndex += count;
	}

	static void Reserve(int n)
	{
		if (outputIndex + n <= outputBuffer.Length)
			return;

		Dump();
		if (n > outputBuffer.Length)
			Array.Resize(ref outputBuffer, Math.Max(outputBuffer.Length * 2, n));
	}

	static void Dump()
	{
		outputStream.Write(outputBuffer, 0, outputIndex);
		outputIndex = 0;
	}

	public static void Flush()
	{
		Dump();
		outputStream.Flush();
	}

	#endregion

}


public class Program
{
	public static void Main(string[] args)
	{
		InitInput(Console.OpenStandardInput());
		InitOutput(Console.OpenStandardOutput());
		Solution solution = new Solution();
		solution.solve();
		Flush();
#if DEBUG
		Console.Error.WriteLine(System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime);
#endif
	}
}