using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using static System.Array;
using static System.Math;
using static Library;
using System.Numerics;
using T = System.Numerics.BigInteger;

class Solution
{

	public void solve()
	{
		var t = Ni();

		while (t-- > 0)
		{
			var m = Nl();
			var x = Nl();
			BigInteger result = 0;

			if (x == 1)
			{
				result = m;
			}
			else if (false && m < 20)
			{
				var mat = Build(m);
				var vec = BuildVec(m);

				result = 1;
				var mat2 = Clone(mat);
				//Console.Error.WriteLine(Text(mat2));
				while (true)
				{
					var mat3 = Mult(mat2, mat2);
					var y = MultVector(mat3, vec)[0];
					if (y <= 0 || y > x)
						break;
					mat2 = mat3;
					//Console.Error.WriteLine(Text(mat2));
					result = 2 * result;
				}

				BigInteger shift = result / 2;
				for (; shift > 0; shift >>= 1)
				{
					var mat3 = Pow(mat, (long)(result + shift));
					var y = MultVector(mat3, vec)[0];
					if (y <= 0 || y > x) continue;
					result += shift;
				}

				result++;
			}
			else
			{
				var f = new F1(m);

				bool quick = true;

				if (quick && f.F(3 * m) > x)
				{
					BigInteger left = m;
					BigInteger right = 3 * m;
					while (left <= right)
					{
						var mid = (left + right) / 2;
						var v = f.F(mid);
						if (v <= x)
							left = mid + 1;
						else
							right = mid - 1;
					}
					result = left;
				}
				else
				{
					for (BigInteger i = quick ? 3 * m : m; ; i++)
					{

						var y = f.F(i);
						//Console.Error.WriteLine($"F({i},{m})={y}  (+{y - prev}) {y2} {y3a}+{y3c}={y3}");
						//prev = y;
						if (y > x)
						{
							result = i;
							break;
						}
					}
				}
			}

			WriteLine(result);
		}

	}

	public T F2(long m, long n)
	{
		var mat = Build(m);
		var vec = BuildVec(m);
		var result = MultVector(Pow(mat, n), vec);
		return ((result[0]));
	}

	T[] BuildVec(long m)
	{
		var vec = new T[m + 1];
		vec[0] = 1;
		vec[1] = 1;
		return vec;
	}

	T[,] Build(long m)
	{
		var lim = m + 1;
		var mat = new T[lim, lim];

		mat[0, 0] = 2;
		mat[0, 1] = -1;
		mat[0, m] = 1;
		for (int i = 1; i < lim; i++)
			mat[i, i - 1] = 1;
		return mat;
	}

	public class F1
	{
		long m;
		static BigInteger[] cache = new BigInteger[20000000];
		static int inited = 0;
		BigInteger k2m;
		BigInteger f2m;
		BigInteger d2m;

		public F1(long m)
		{
			this.m = m;
			Array.Clear(cache, 0, inited + 1);
			inited = 0;

			k2m = m + 1; // 2 * m - m + 1;
			f2m = k2m * (k2m + 1) / 2 + 1;
			d2m = f2m - (k2m * (k2m - 1) / 2 + 1);
		}

		public T Get(BigInteger n)
		{
			if (n < m) return 1;

			if (n <= m * 2)
			{
				var k = (T)n - m + 1;
				return k * (k + 1) / 2 + 1;
			}

			if (n <= m * 3)
			{

				T k3 = n - 2 * m;
				// k3 * k3 /2 + k3 / 2 + 1 ->
				//long y3b = (k3 * (k3 + 1) * (2 * k3 + 1) + k3 * (k3 + 1) * 3) / 12 + k3;
				//long y3b = ((2*k3 + 6) * k3 + 16) * k3 / 12;
				//long y3c = (2 * k3*k3*(k3+1)*(k3+1)/4 + 6 * k3*(k3+1)*(2*k3+1)/6 + 16*k3*(k3+1)/2) / 12;
				var y3c = (((k3 + 6) * k3 + 23) * k3 + 18) * k3 / 24;
				return y3c + f2m + d2m * k3;
			}

			// long sumk = k * k / 2 + k / 2 + 1;
			// sk = k * (k + 1) * (2 * k + 1) / 12 + k * (k + 1) / 4 + k;
			return cache[(long)(n - (m * 3 + 1))];
		}

		public T F(BigInteger n)
		{
			long m3 = m * 3;
			if (n <= m3)
				return Get(n);

			var index = n - (m3 + 1);

			for (; inited <= index; inited++)
			{
				var k = inited + m3 + 1;
				cache[inited] = 2 * Get(k - 1) - Get(k - 2) + Get(k - m - 1);
			}

			return cache[(int)index];
		}

	}


	public static string Text(T[,] mat)
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

	public static T[,] Mult(T[,] a, T[,] b, T[,] c = null)
	{
		int arows = a.GetLength(0);
		int bcols = b.GetLength(1);
		int mid = a.GetLength(1);
		if (c == null) c = new T[arows, bcols];

		for (int i = 0; i < arows; i++)
		for (int j = 0; j < bcols; j++)
		{
			T t = 0;
			for (int k = 0; k < mid; k++)
				t += (T)(1L * a[i, k] * b[k, j]);
			c[i, j] = t;
		}

		return c;
	}

	public static T[] MultVector(T[,] a, T[] b, T[] c = null)
	{
		int n = a.GetLength(0);
		int m = a.GetLength(1);

		if (c == null) c = new T[n];
		for (int i = 0; i < n; i++)
		{
			T t = 0;
			for (int k = 0; k < m; k++)
				t += (T)(1L * a[i, k] * b[k]);
			c[i] = t;
		}

		return c;
	}


	public static T[,] MultSparse(T[,] a, T[,] b, T[,] result = null)
	{
		int m = a.GetLength(0);
		int n = a.GetLength(1);
		int p = b.GetLength(1);

		if (result == null) result = new T[m, p];
		else
		{
			for (int i = 0; i < m; i++)
			for (int j = 0; j < p; j++)
				result[i, j] = 0;
		}

		for (int i = 0; i < m; i++)
		for (int k = 0; k < n; k++)
			if (a[i, k] != 0)
				for (int j = 0; j < p; j++)
					result[i, j] += a[i, k] * b[k, j];

		return result;
	}

	public static T[,] Pow(T[,] a, long p)
	{
		int n = a.GetLength(0);
		var tmp = new T[n, n];
		var result = Diagonal(n);
		var b = Clone(a);

		while (p > 0)
		{
			if ((p & 1) != 0)
			{
				Mult(result, b, tmp);
				Assign(result, tmp);
			}
			p >>= 1;
			Mult(b, b, tmp);
			Assign(b, tmp);
		}
		return result;
	}

	public static T[,] Diagonal(int n, long d = 1)
	{
		var id = new T[n, n];
		for (int i = 0; i < n; i++)
			id[i, i] = d;
		return id;
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