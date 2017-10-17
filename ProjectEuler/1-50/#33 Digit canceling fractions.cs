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

	public void solve()
	{
		int n = Ni();
		int k = Ni();

		/*
		for (int i = 1; i <= 9; i++)
			for (int j = i+1; j < 9; j++)
			{
				int i2, j2;

				for (int m = 1; m < 10; m++)
				{
					i2 = i * 10 + m;
					j2 = j * 10 + m;
					if (i*j2 == i2*j)
						Console.Error.WriteLine($"{i}/{j} == {i2}/{j2}");

					i2 = m * 10 + i;
					j2 = j * 10 + m;
					if (i * j2 == i2 * j)
						Console.Error.WriteLine($"{i}/{j} == {i2}/{j2}");

					i2 = i * 10 + m;
					j2 = m * 10 + j;
					if (i * j2 == i2 * j)
						Console.Error.WriteLine($"{i}/{j} == {i2}/{j2}");

					i2 = m * 10 + i;
					j2 = m * 10 + j;
					if (i * j2 == i2 * j)
						Console.Error.WriteLine($"{i}/{j} == {i2}/{j2}");
				}
			}*/


		long nsum = 0;
		long dsum = 0;
		var start = Pow10(n - 1);
		var end = start * 10 - 1;
		var start2 = Pow10(n - k - 1);
		var end2 = start2 * 10 - 1;

		var hash = new HashSet<long>();

		var hits = new List<string>();

		for (int den = 1; den <= end2; den++)
		{
			var dMask = Mask(den, n - k);
			for (int num = 1; num < den; num++)
			{
				var nmask = Mask(num, n - k);
				var gcd = Gcd(num, den);
				var ndg = num / gcd;
				var ddg = den / gcd;

				int z = (start + ndg - 1) / ndg;
				int zend = end / ddg;
				for (; z <= zend; z++)
				{
					//if (z % 10 == 0) continue;

					var n2 = z * ndg;
					var nm2 = Mask(n2, n);
					var d2 = z * ddg;


					//if (n2 == 106 && d2 == 265) Debugger.Break();
					if ((nm2 & nmask) != nmask) continue;

					var dm2 = Mask(d2, n);
					if ((dm2 & dMask) != dMask) continue;

					dm2 &= ~dMask;
					nm2 &= ~nmask;
					if (((dm2 | nm2) & (1 << 6) - 1) != 0) continue;

					if (dm2 == nm2 || FixUp(dm2) == FixUp(nm2))
					{
						long code = ((long)n2 << 20) + d2;
						if (!hash.Contains(code)
							
							&& ContainsSubsequence(n2.ToString(), num.ToString())
							&& ContainsSubsequence(d2.ToString(), den.ToString()))
						{
							hash.Add(code);
							nsum += n2;
							dsum += d2;

							hits.Add($"{d2}/{n2} = {den}/{num}");
						}
						//break;
					}
				}
			}
		}

		hits.Sort();
		foreach (var hit in hits)
			Console.Error.WriteLine(hit);

		WriteLine($"{nsum} {dsum}");
	}

	public bool ContainsSubsequence(string s, string s2)
	{
		int i = 0;
		int j = 0;
		while (i < s.Length && j < s2.Length)
		{
			if (s[i] == s2[j])
			{
				i++;
				j++;
				continue;
			}

			i++;
		}

		return j == s2.Length;
	}

	public static long FixUp(long mask)
	{

		const long lobits = 1 << 0 | 1 << 6 | 1L << 12 | 1L << 18 | 1L << 24
						| 1L << 30 | 1L << 36 | 1L << 42 | 1L << 48 | 1L << 54;

		var result = mask & lobits;
		result |= mask & (result << 1);

		var other = mask & ~result;
		while (other > 0)
		{
			var bit = other & -other;
			other &= other - 1;

			while ((bit & lobits) == 0 && (result & (bit>>1)) == 0)
				bit >>= 1;

			result += bit;
		}

		return result;
	}

	public static int Gcd(int a, int b)
	{
		while (true)
		{
			if (a == 0) return b;
			b %= a;
			if (b == 0) return a;
			a %= b;
		}
	}

	static long Mask(int n, int digits)
	{
		long mask = 0;
		for (int dig = 0; dig < digits; dig++)
		{
			var d = n % 10;
			n /= 10;
			long bit = 1L << (6 * (int)d);
			while ((mask & bit) != 0) bit <<= 1;
			mask |= bit;
		}
		return mask;
	}

	static int BitCount(int x)
	{
		int count;
		var y = ((uint)x);
		for (count = 0; y != 0; count++)
			y &= y - 1;
		return count;
	}

	static int Pow10(int n)
	{
		var pow = 1;
		while (n-- > 0)
			pow *= 10;
		return pow;
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