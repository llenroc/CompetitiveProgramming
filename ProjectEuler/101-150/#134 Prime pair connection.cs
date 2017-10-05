using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32;
using static System.Array;
using static System.Math;
using static Library;
using System.Numerics;



class Solution
{
    public BitArray basePrimes = GetPrimeSet(1<<16);
    
	public void solve()
	{

		long lo = 0;
		long hi = 1002000;
		BitArray primes = null; //GetPrimeSet(lo, hi);

		int t = Ni();
		while (t-- > 0)
		{
			long L = Nl(), R = Nl();

			//if (primes == null || !(lo <= L && R + 2000 <= hi))
			{
				lo = L;
                hi = R + 1000;
				//hi = Max(R, L + 1000000) + 2000;
				primes = GetPrimeSet(lo, hi);
			}

			decimal sum = 0;
			long p = -1;
			for (long p2 = L; p2<=R+1000; p2++)
			{
				if (!primes[(int)(p2-lo)]) continue;

				if (p < L)
				{
					p = p2;
					continue;
				}

                if (p > R) break;
                
				long pow = 10;
				while (pow <= p) pow *= 10;

				//long y = Mult(Pow(p2, (pow/10<<2)-1, pow), p, pow);
				//var ans1 =y * p2;

				long x = Mult(ModInverse(pow,p2), (p2 - p), p2);
				//long x = Mult(Pow(pow, p2 - 2, p2), (p2 - p), p2);
				var ans2 = x * pow + p;
				sum += ans2;

				//Console.Error.WriteLine($"{p}+{p2} -> {ans1}  {ans2}");

				p = p2;
				if (p > R) break;
			}

			WriteLine(sum);

		}

	}

    public static long ModInverse(long a, long n)
    {
        long x, y;
        var d = ExtendedEuclid(a, n, out x, out y);
        if (d > 1) return -1;
        x%=n;
        if (x<0) x+=n;
        return x;
    }

    public static long ExtendedEuclid(long a, long b, out long x, out long y)
    {
        var xx = y = 0;
        var yy = x = 1;
        while (b != 0)
        {
            var q = a / b;
            var t = b;
            b = a % b;
            a = t;
            t = xx;
            xx = x - q * xx;
            x = t;
            t = yy;
            yy = y - q * yy;
            y = t;
        }
        return a;
    }

    
    
	public  BitArray GetPrimeSet(long lo, long hi)
	{
		if (hi < lo)
			throw new ArgumentOutOfRangeException();

		if (lo == 0 && hi < Int32.MaxValue)
			return GetPrimeSet((int)hi);

		int range = (int)(hi - lo + 1);
		var check = new BitArray(range, true);

		// Mark all numbers less than 2 as composite
		for (long i = lo; i < 2 && i <= hi; i++)
			check[(int)(i - lo)] = false;

		// Mark all even numbers as composite
		for (long i = Max(lo + (lo & 1), 4); i <= hi; i += 2)
			check[(int)(i - lo)] = false;

		int sqrt = (int)Sqrt(hi) + 1;
		var primes = basePrimes;
		for (int i = 3; i <= sqrt; i += 2)
		{
			if (primes[i])
			{
				// Use longs here to avoid overflow
				long start = Max(lo, 2*i);
                start -= (start % i);
                if (start < lo) start += i;
				for (long j = start; j <= hi; j += i)
					check[(int)(j - lo)] = false;
			}
		}
		return check;
	}
	public static long Pow(long n, long p, long mod)
	{
		long result = 1;
		long b = n % mod;
		while (p > 0)
		{
			if ((p & 1) == 1) result = Mult(result,b,mod);
			p >>= 1;
			b = Mult(b,b, mod);
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
		// UNTESTED
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

	public int Log10(long x)
	{
		int count = 0;
		if (x >= 1000L * 1000L)
		{
			if (x >= 1000L * 1000L * 1000L * 1000L)
			{
				count = 16;
				x /= 1000L * 1000L * 1000L * 1000L;
			}
			else
			{
				count = 8;
				x /= 1000L * 1000L;
			}
		}
		if (x >= 10000)
		{
			count += 4;
			x /= 10000;
		}
		if (x >= 100)
		{
			count += 2;
			x /= 100;
		}
		if (x >= 10)
		{
			count += 1;
		}
		return count;
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

		var limit = (long)Sqrt(max) + 1;
		//for (int i = 3; i <= max; i += 2)
		for (int i = 3; i < limit; i += 2)
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
		}
		return neg ? -number : number;
	}

	public static long Nl()
	{
		var c = SkipSpaces();
		bool neg = c == '-';
		if (neg) { c = Read(); }

		long number = c - '0';
		while (true)
		{
			var d = Read() - '0';
			if ((uint)d > 9) break;
			number = number * 10 + d;
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