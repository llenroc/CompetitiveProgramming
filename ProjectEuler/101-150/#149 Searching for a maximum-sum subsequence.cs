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
		int N = Ni();
		int L = Ni();
		int[] a = Ni(L);

		int[] f = Ni(5);
		int m = Ni();
		int[] b = Ni(m);
		int[] g = Ni(5);

		var s = new int[N, N];
		int n = 0;
		int z = 0;
		int ii = 0;
		for (int i = 0; i < s.Length; i++)
		{
			s[n, z] = a[f[ii]] + b[g[ii]];
			if (++z >= N)
			{
				z = 0;
				n++;
			}
			f[ii] = (f[0] + f[1] + f[2] + f[3] + f[4]) % L;
			g[ii] = (g[0] + g[1] + g[2] + g[3] + g[4]) % m;
			if (++ii >= 5) ii = 0;
		}

		// Console.Error.WriteLine(Text(s));

		var sum = new long[2 * N + 2, 6];

		long max = long.MinValue;
		for (n = 1; n <= N; n++)
		{
			for (z = 0; z < n - 1; z++)
			{
				sum[z, 0] = Max(0, sum[z, 0]) + s[n - 1, z];
				max = Max(max, sum[z, 0]);
				sum[z, 1] = Max(0, sum[z, 1]) + s[z, n - 1];
				max = Max(max, sum[z, 1]);
			}

			for (z = 0; z < n; z++)
			{
				sum[n - 1, 0] = Max(0, sum[n - 1, 0]) + s[z, n - 1];
				max = Max(max, sum[n - 1, 0]);
				sum[n - 1, 1] = Max(0, sum[n - 1, 1]) + s[n - 1, z];
				max = Max(max, sum[n - 1, 1]);
			}

			for (z = 1; z <= n; z++)
			{
				sum[n - z + N, 2] = Max(0, sum[n - z + N, 2]) + s[n - 1, z - 1];
				max = Max(max, sum[n - z + N, 2]);
				if (z == n) continue;
				sum[z - n + N, 2] = Max(0, sum[z - n + N, 2]) + s[z - 1, n - 1];
				max = Max(max, sum[z - n + N, 2]);
			}

			for (z = 0; z < n; z++)
			{
				sum[n + z, 5] = sum[z + n, 5] + s[n - 1, z] + (z!=n-1 ? s[z, n - 1]:0);
				max = Max(max, sum[n + z, 5]);
				sum[n + z, 3] = Max(Max(0, sum[z + n, 3]) + s[n - 1, z], sum[n + z, 5]);
				max = Max(max, sum[n + z, 3]);
				sum[z + n, 4] = Max(Max(0, sum[z + n, 4]) + s[z, n - 1], sum[n + z, 5]);
				max = Max(max, sum[n + z, 4]);
			}

			WriteLine(max);
		}
	}


	public static string Text(int[,] mat)
	{
		StringBuilder sb = new StringBuilder();
		if (mat == null)
			return null;

		int n = mat.GetLength(0);
		int m = mat.GetLength(1);

		for (int i = 0; i < n; i++)
		{
			for (int j = 0; j < m; j++)
			{
				if (j > 0) sb.Append(' ');
				sb.Append(mat[i, j]);
			}
			sb.AppendLine();
		}

		return sb.ToString();
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