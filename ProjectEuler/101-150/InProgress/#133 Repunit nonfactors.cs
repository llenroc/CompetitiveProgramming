using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using static System.Array;
using static System.Math;
using static Library;

class Solution {
	public void solve()
	{
		var t = Ni();
		var cases = Ni(t);
		var answers = new long[t];
		var indices = new int[t];
		for (int i = 0; i < indices.Length; i++)
			indices[i] = i;

		Sort(cases, indices);
        var max = cases[cases.Length-1];
        
		var composite = new bool[max/2+1];
        var limit = (long)Math.Sqrt(max)+1;
		for (long i = 3; i <= limit; i += 2)
		{
			if (composite[(int) i>>1]) continue;
			for (long j = i * i; j>>1 < composite.Length; j += i+i)
				composite[(int) j>>1] = true;
		}

		ulong biglong = 10000000000UL;
		ulong biggerlong = 100000000000000UL;
		//BigInteger big = BigInteger.Pow(10, 30);

		int current = 0;
		while (current < cases.Length && cases[current] <= 2)
			current++;

		long sum = 2;
		for (long i = 3; i <= max; i+=2)
		{
			while (current < cases.Length && cases[current] <= i)
			{
				answers[indices[current]] = sum;
				current++;
			}

			if (composite[(int)i>>1]) continue;

			if (ModPow(biglong, ModPow(biglong,biglong, (ulong)(6*(i-1))), (ulong)(9* i)) != 1) //3^2 - 3^1 = 6
			//if (BigInteger.ModPow(10, big, 9 * i) != 1)
					sum += i;
		}

		// Finished
		while (current < cases.Length)
		{
			answers[indices[current]] = sum;
			current++;
		}

		foreach (var ans in answers)
			WriteLine(ans);
	}

	public static ulong ModPow(ulong n, ulong p, ulong mod)
	{
		ulong b = n;
		ulong result = 1;
		while (p != 0)
		{
			if ((p & 1) != 0)
				result = Mult(result, b, mod);
			p >>= 1;
			b = Mult(b, b, mod);
		}
		return result;
	}

	public static ulong Mult(ulong x, ulong y, ulong mod)
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
    
/*    public static long Mult(long a, long b, long mod)
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
    }*/

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

		public static int Log2(ulong value)
		{
			var log = 0;
			if (value >= (1UL << 24))
			{
				if (value >= (1UL << 48))
				{
					log = 48;
					value = (value >> 48);
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