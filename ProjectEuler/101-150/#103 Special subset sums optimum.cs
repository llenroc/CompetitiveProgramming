using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using static System.Array;
using static System.Math;
using static Library;

class Solution {
	public void solve()
	{
		int n = Ni();

		var bit = new FenwickTree(n+1);

		int last = n - 1;
		bit.RangeUpdateInclusive(last, last, 1);

		for (int j = last-1; j >= 0; j--)
		{
			int mid = ((j + 1) + last + 1) / 2;
			long v = bit.SumInclusive(mid);
			bit.RangeUpdateInclusive(j, last, v);
		}

		for (int i=0; i<n; i++)
		{
			Write(bit.SumInclusive(i));
			Write(' ');
		}
		WriteLine();
	}
}

public class FenwickTree
{
	private long MOD = 715827881;
	#region Variables
	public readonly long[] A;
	#endregion

	#region Constructor
	/*public Fenwick(int[] a) : this(a.Length)
	{
		for (int i = 0; i < a.Length; i++)
			Add(i, a[i]);
	}*/

	public FenwickTree(long[] a) : this(a.Length)
	{
		int n = a.Length;
		System.Array.Copy(a, 0, A, 1, n);
		for (int k = 2, h = 1; k <= n; k *= 2, h *= 2)
		{
			for (int i = k; i <= n; i += k)
				A[i] += A[i - h];
		}

		//for (int i = 0; i < a.Length; i++)
		//	Add(i, a[i]);
	}

	public FenwickTree(long size)
	{
		A = new long[size + 1];
	}
	#endregion

	#region Properties
	public long this[int index] => SumInclusive(index, index);

	public int Length => A.Length - 1;

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public long[] Table
	{
		get
		{
			int n = A.Length - 1;
			long[] ret = new long[n];
			for (int i = 0; i < n; i++)
				ret[i] = SumInclusive(i);
			//for (int i = n - 1; i >= 1; i--)
			//	ret[i] -= ret[i - 1];
			return ret;
		}
	}
	#endregion


	#region Methods
	// Increments value		
	/// <summary>
	/// Adds val to the value at i
	/// </summary>
	/// <param name="i">The i.</param>
	/// <param name="val">The value.</param>
	public void Add(int i, long val)
	{
		if (val == 0) return;
		for (i++; i < A.Length; i += (i & -i))
			A[i] = (A[i] + val) % MOD;
	}

	// Sum from [0 ... i]
	public long SumInclusive(int i)
	{
		long sum = 0;
		for (i++; i > 0; i -= (i & -i))
			sum = (sum + A[i]) % MOD;
		return sum;
	}

	public long SumInclusive(int i, int j)
	{
		var result = (SumInclusive(j) - SumInclusive(i - 1)) %MOD;
		if (result < 0) result += MOD;
		return result;
	}

	/// <summary>
	/// Inclusive update of the range [left, right] by value
	/// The default operation of the fenwick tree is point update - range query.
	/// We use this if we want alternative range update - point query.
	/// SumInclusive becomes te point query function.
	/// </summary>
	/// <returns></returns>
	public void RangeUpdateInclusive(int left, int right, long delta)
	{
		Add(left, delta);
		Add(right + 1, MOD-delta);
	}

	// get largest value with cumulative sum less than x;
	// for smallest, pass x-1 and add 1 to result

	#endregion
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