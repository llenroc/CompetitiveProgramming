using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using static System.Array;
using static System.Math;
using static Library;

class Solution
{
	// public const int MOD = 1000000007;

	public void solve()
	{
        checked {
            
            var squares = new List<long>();
            for (long i = 0; i <= 1000000; i++)
                squares.Add(i * i);

            int t = Ni();

            var Ks = new int[t];
            var Ls = new long[t];

            for (int i=0; i<t; i++)
            {
                Ks[i] = Ni();
                Ls[i] = Nl();
            }

            var kset = new HashSet<int>(Ks).ToArray();
            var lset = new HashSet<long>(Ls).ToArray();
            Sort(kset);
            Sort(lset);
            
            var visited = new HashSet<long>();

            var mat = new decimal[kset.Length+1, lset.Length + 1];
            var maxL = lset[lset.Length - 1];
            var maxK = kset[kset.Length - 1];
            var collisions = 0L;
            var collisionSum = 0L;
            
            for (long a = 2; ; a++)
            {
                var a3 = a * a * a;
                if (a3 > maxL) break;

                for (long b = 1; b < a; b++)
                {
                    if ( b*b*(a3 + 1) > maxL) break;
                    if (Gcd(a, b) > 1) continue;

                    for (long c = 1;; c++)
                    {
                        long n = b * c * (a3 * c + b);
                        if (n >= maxL) break;

                        int index = squares.BinarySearch(n);
                        if (index < 0) index = ~index;

                        long targetK = maxK+1;
                        if (index > 0) targetK = Min(targetK, n - squares[index - 1]);
                        if (index < squares.Count) targetK = Min(targetK, squares[index] - n);
                        if (targetK > maxK) continue;
                        
                        int indexK = BinarySearch(kset,(int)targetK);
                        int indexL = BinarySearch(lset, n+1);
                        if (indexK < 0) indexK = ~indexK;
                        if (indexL < 0) indexL = ~indexL;

                        if (indexK>=kset.Length || indexL>=lset.Length) continue;
                        if (visited.Contains(n)) { collisions++; collisionSum+=n; continue;}
                        visited.Add(n);

                        mat[indexK, indexL] += n;
                    }
                }
            }
            
            Console.Error.WriteLine(collisions);
            Console.Error.WriteLine(collisionSum);

            var matsum = new MatrixSum(mat, false);

            for (int i = 0; i < t; i++)
            {
                int indexK = Library.BinarySearch(kset, Ks[i], false);
                int indexL = Library.BinarySearch(lset, Ls[i], false);
                var sum = matsum.GetSumInclusive(0,0,indexK, indexL);
                WriteLine(sum);
            }
        }
	}

	public class MatrixSum
	{
		readonly decimal[,] _matrix;

		public MatrixSum(decimal[,] matrix, bool inplace = false)
		{
			int rows = matrix.GetLength(0);
			int cols = matrix.GetLength(1);

			if (!inplace)
				matrix = (decimal[,])matrix.Clone();

			_matrix = matrix;

			for (int i = 1; i < rows; i++)
			for (int j = 0; j < cols; j++)
				matrix[i, j] += matrix[i - 1, j];

			for (int i = 0; i < rows; i++)
			for (int j = 1; j < cols; j++)
				matrix[i, j] += matrix[i, j - 1];
		}

		public decimal GetSum(int x, int y, int dx, int dy)
		{
			return GetSumInclusive(x, y, x + dx - 1, y + dy - 1);
		}


		public decimal GetSumInclusive(int x1, int y1, int x2, int y2)
		{
			var result = _matrix[x2, y2];

			if (x1 > 0)
				result -= _matrix[x1 - 1, y2];

			if (y1 > 0)
				result -= _matrix[x2, y1 - 1];

			if (x1 > 0 && y1 > 0)
				result += _matrix[x1 - 1, y1 - 1];

			return result;
		}
	}

	public static long Gcd(long a, long b)
	{
		while (true)
		{
			if (a == 0) return b;
			b %= a;
			if (b == 0) return a;
			a %= b;
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

	public static int BinarySearch<T>(T[] array, T value, bool upper = false)
		where T : IComparable<T>
	{
		int left = 0;
		int right = array.Length - 1;

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
            if (unchecked((uint)d > 9)) break;
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
            if (unchecked((uint)d > 9)) break;
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
		do c = Read(); while (unchecked((uint)c - 33 >= (127 - 33)));
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