using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using static System.Array;
using static System.Math;
using static Library;
using System.Text;
using T = System.Int64;

class Solution
{
	public void solve()
	{
		int t = Ni();
		const long mod = 1000000007;

		var list = new long[t];
		for (int i = 0; i < t; i++)
			list[i] = Nl();

		var answers = new long[t];
		var index = new int[t];
		for (int i = 0; i < index.Length; i++)
			index[i] = i;

		Array.Sort(list, index);


		var mat = new long[3, 3]
		{
			{13, 8, 0},
			{8, 5, 0},
			{(mod + 1) / 2, 0, 1},
		};

		var tmp = new long[3, 3];

		var vec = new long[] { 34, 21, 0 };
		var result = new long[3];

		var prev = 0L;
		var fast = new FastMatrixPow(mat, mod);

		for (var i = 0; i < list.Length; i++)
		{
			var n = list[i];
			//for (int i=1; i<=n; i++)
			//    sum += Fibonacci(6*i+3)/2;

			if (prev != n)
			{
				Mult(fast.Pow(n - prev, tmp), vec, mod, result);
				answers[index[i]] = result[2];
    			Swap(ref result, ref vec);
			}
			else
			{
				answers[index[i]] = answers[index[i - 1]];
			}

			prev = n;
		}

		for (int i = 0; i < answers.Length; i++)
			WriteLine(answers[i]);
	}

	static long Root(long x)
	{
		var sqrt = Math.Sqrt(x);
		if (sqrt == (long)sqrt)
			return (long)sqrt;
		return -1;
	}


	public static T[,] Mult(T[,] a, T[,] b, T mod, T[,] c = null)
	{
		if (c == null) c = new T[3, 3];
		c[0, 0] = (a[0, 0] * b[0, 0] % mod + a[0, 1] * b[1, 0] % mod)%mod + a[0, 2] * b[2, 0] % mod;
		c[0, 0] %= mod;
		c[0, 1] = (a[0, 0] * b[0, 1] % mod + a[0, 1] * b[1, 1] % mod)%mod + a[0, 2] * b[2, 1] % mod;
		c[0, 1] %= mod;
		c[0, 2] = (a[0, 0] * b[0, 2] % mod + a[0, 1] * b[1, 2] % mod)%mod + a[0, 2] * b[2, 2] % mod;
		c[0, 2] %= mod;
		c[1, 0] = (a[1, 0] * b[0, 0] % mod + a[1, 1] * b[1, 0] % mod)%mod + a[1, 2] * b[2, 0] % mod;
		c[1, 0] %= mod;
		c[1, 1] = (a[1, 0] * b[0, 1] % mod + a[1, 1] * b[1, 1] % mod)%mod + a[1, 2] * b[2, 1] % mod;
		c[1, 1] %= mod;
		c[1, 2] = (a[1, 0] * b[0, 2] % mod + a[1, 1] * b[1, 2] % mod)%mod + a[1, 2] * b[2, 2] % mod;
		c[1, 2] %= mod;
		c[2, 0] = (a[2, 0] * b[0, 0] % mod + a[2, 1] * b[1, 0] % mod)%mod + a[2, 2] * b[2, 0] % mod;
		c[2, 0] %= mod;
		c[2, 1] = (a[2, 0] * b[0, 1] % mod + a[2, 1] * b[1, 1] % mod)%mod + a[2, 2] * b[2, 1] % mod;
		c[2, 1] %= mod;
		c[2, 2] = (a[2, 0] * b[0, 2] % mod + a[2, 1] * b[1, 2] % mod)%mod + a[2, 2] * b[2, 2] % mod;
		c[2, 2] %= mod;
		return c;
	}

	public static T[] Mult(T[,] a, T[] b, T mod, T[] c = null)
	{
		if (c == null) c = new T[3];
		c[0] = (a[0, 0] * b[0] % mod + a[0, 1] * b[1] % mod)%mod + a[0, 2] * b[2] % mod;
		c[0] %= mod;
		c[1] = (a[1, 0] * b[0] % mod + a[1, 1] * b[1] % mod)%mod + a[1, 2] * b[2] % mod;
		c[1] %= mod;
		c[2] = (a[2, 0] * b[0] % mod + a[2, 1] * b[1] % mod)%mod + a[2, 2] * b[2] % mod;
		c[2] %= mod;
		return c;
	}

	public static T[,] Diagonal(int n, T d = 1)
	{
		var id = new T[n, n];
		for (int i = 0; i < n; i++)
			id[i, i] = d;
		return id;
	}

	public struct FastMatrixPow
	{
		private const int shift = 4;
		private const long mask = (1L << shift) - 1;

		readonly T[][][,] _cache;
		readonly long _mod;
		readonly T[,] _tmp;
		readonly int _n;

		public FastMatrixPow(T[,] a, long mod)
		{
			_n = a.GetLength(0);
			_cache = new T[64 / shift][][,];
			_mod = mod;
			_tmp = new T[_n, _n];

			for (int j = 0; j < _cache.Length; j++)
			{
				var t = _cache[j] = new T[mask + 1][,];
				t[1] = j == 0 ? Clone(a) : Mult(_cache[j - 1][1], _cache[j - 1][mask], mod);
				for (int i = 2; i < t.Length; i++)
					t[i] = Mult(t[1], t[i - 1], mod);
			}
		}

		public T[,] Pow(long p, T[,] result = null)
		{

			if (result == null) result = new T[_n, _n];
			else Clear(result, 0, result.Length);

			//if (p == 0)
			//{
			//	Clear(result, 0, result.Length);
			//	for (int i = result.GetLength(0) - 1; i >= 0; i--)
			//		result[i, i] = 1;
			//	return result;
			//}

			for (int i = result.GetLength(0) - 1; i >= 0; i--)
				result[i, i] = 1;


			var asst = 0;
			int bit = 0;
			while (p > 0)
			{
				if ((p & mask) != 0)
				{
					Assign(result, true // asst++ > 0
						? Mult(result, _cache[bit][p & mask], _mod, _tmp)
						: _cache[bit][p & mask]);
				}
				p >>= shift;
				bit++;
			}
			return result;
		}


		public static T[,] Clone(T[,] m)
		{
			return (T[,])m.Clone();
		}

		public static void Assign(T[,] dest, T[,] src)
		{
			Array.Copy(src, dest, src.Length);
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