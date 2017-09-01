using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using static System.Array;
using static System.Math;
using static Library;

//using T = Solution.Number;
//using T = System.Double;
using T = System.Int64;

class Solution
{
	public void solve()
	{
		int n = Ni();
		var a = Nl(n + 1);


		var factors = new T[n + 1];
		for (int i = 0; i <= n; i++)
		{
			long sum = 0;
			for (int j = n; j >= 0; j--)
				sum = (sum * (i + 1)) % MOD + a[j];
			factors[i] = sum % MOD;
		}

		var array = factors.ToArray();
		var factors2 = factors.ToArray();
		var list = new List<long>();
		var list2 = new List<long>();
		//Console.WriteLine(string.Join(" ", a));
		//Console.WriteLine(string.Join(" ", factors));

		double num = 1;
		for (int x = 1; x <= n; x++)
		{

			//num *= x;
			{
				long result = 0;
				int i = x - 1;
				for (int j = 0; j < i; j++)
				{
					factors[j] = Div(Div(Mult(Mult(factors[j], x), x - 1 - j), j - i), x - j);
					factors[i] = Mult(Mult(factors[i], (x - j)), Inverse(i - j));
				}

				//Console.WriteLine($"{x} -> " + string.Join(" ", (IEnumerable<long>)factors));

				for (int j = 0; j < x; j++)
				{
					result += factors[j];
					if (result > MOD)
						result -= MOD;
				}
				list.Add(Fix(result));
			}



			//int i = x - 1;
			//for (int j = 0; j < i; j++)
			//{
			//	factors[j] = factors[j] * (x) * (x - 1 - j) / (j - i) / (x - j);
			//	factors[i] = factors[i] * (x - j) / (i - j);
			//}
			//for (int j = 0; j < x; j++)
			//	result += (double)factors[j];

			//{
			//	long result2 = 0;
			//	for (int i = 0; i < x; i++)
			//	{
			//		factors2[i] = array[i];
			//		for (int j = x - 1; j >= 0; j--)
			//		{
			//			if (j != i)
			//				factors2[i] = Div(Mult(factors2[i], x - j), i - j);
			//		}
			//	}
				
			//	//Console.WriteLine($"{x} -> " + string.Join(" ", (IEnumerable<long>)factors));

			//	for (int j = 0; j < x; j++)
			//	{
			//		result2 += factors2[j];
			//		if (result2 > MOD)
			//			result2 -= MOD;
			//	}
			//	list2.Add(Fix(result2));
			//}
		}

		WriteLine(string.Join(" ", list));
		Flush();
		//Console.Error.WriteLine("CMP\n" + string.Join(" ", list2));
	}


	#region Mod Math
	public const int MOD = 1000 * 1000 * 1000 + 7;

	static int[] _inverse;
	public static long Inverse(long n)
	{
		long result;

		if (_inverse == null)
			_inverse = new int[4000];

		if (n < 0) return -Inverse(-n);

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

	public class Number
	{
		public List<double> den = new List<double>();
		public List<double> num = new List<double>();


		public static Number Clone(Number n)
		{
			if (n == null) return new Number();
			return new Number { num = n.num.ToList(), den = n.den.ToList() };
		}


		public static implicit operator Number(double d)
		{
			return new Number { num = { d } };
		}

		public static explicit operator double(Number n)
		{
			double result = 1;
			foreach (var nn in n.num)
				result *= nn;
			foreach (var d in n.den)
				result /= d;
			return result;
		}

		public static Number operator *(Number n, double f)
		{
			n = Clone(n);
			n.num.Add(f);
			return n;
		}

		public static Number operator /(Number n, double f)
		{
			n = Clone(n);
			n.den.Add(f);
			return n;
		}


		public override string ToString()
		{
			var n = num.Count > 0 ? string.Join("*", num) : "1";
			var d = den.Count > 0 ? string.Join("*", den) : "1";
			return n + "/" + d;
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