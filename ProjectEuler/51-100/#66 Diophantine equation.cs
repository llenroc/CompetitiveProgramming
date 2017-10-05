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
using System.Numerics;

class Solution 
{
	// public const int MOD = 1000000007;

	public void solve()
	{
		int N = Ni();

		BigInteger max = 0;
		long maxi = 0;

		for (int i = 1; i <= N; i++)
		{
			var sq = (int)Sqrt(i);
			if (sq * sq == i) continue;

			var pell = Pell(i);
			if (pell >= max)
			{
				max = pell;
				maxi = i;
			}
		}

		Console.Error.WriteLine(max);
		WriteLine(maxi);

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

		var limit = (int)Sqrt(max) + 1;
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

	public static BigInteger Pell(long d)
	{
		BigInteger p = 1;
		BigInteger k = 1;
		BigInteger x1 = 1;
		BigInteger y = 0;
		BigInteger sd = (BigInteger)Sqrt(d);
		BigInteger x = 0;

		while (k != 1 || y == 0)
		{
			p = k * (p / k + 1) - p;
			p = p - (int)((p - sd) / k) * k;
			x = (p * x1 + d * y) / BigInteger.Abs(k);
			y = (p * y + x1) / BigInteger.Abs(k);
			k = (p * p - d) / k;
			x1 = x;
		}

		return x;
	}

	public static void Pell(long n, out BigInteger p, out BigInteger q)
	{
		BigInteger p2 = BigInteger.One;
		BigInteger p1 = BigInteger.Zero;
		BigInteger q2 = BigInteger.Zero;
		BigInteger q1 = BigInteger.One;
		BigInteger a0, a1;
		a0 = a1 = (BigInteger)(Math.Sqrt(n));
		BigInteger g1 = BigInteger.Zero;
		BigInteger h1 = BigInteger.One;
		BigInteger n0 = n;
		while (true)
		{
			BigInteger g2 = a1*h1 - g1;
			BigInteger h2 = (n0 - (g2 * g2))/h1;
			BigInteger a2 = (g2 + a0)/h2;
			p = p2*a1 + p1;
			q = q2 *a1 + q1;
			if (p*p - (n0 * (q * q))==1)
				return;
			a1 = a2;
			g1 = g2;
			h1 = h2;
			p1 = p2;
			p2 = p;
			q1 = q2;
			q2 = q;
		}
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