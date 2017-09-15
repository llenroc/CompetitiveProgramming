using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using static System.Array;
using static System.Math;
using static Library;

class Solution
{
	private int[] factors;

	public void solve()
	{

		factors = PrimeFactorsUpTo(2000002);

#if DEBUG
		for (int i = 1; i < 12; i++)
		{
			var b = SearchForFirstSquareWithDivisors(i);
			Console.Error.WriteLine($"{i}->{b}");
		}
#endif
		// Test();


		int q = Ni();
		while (q-- > 0)
		{
			int n = Ni();
			if (n == 1)
			{
				WriteLine(3);
				continue;
			}

			var x = 2 * n + 1;
			var best = SearchForFirstSquareWithDivisors(x);
			var answer = best * 2;
			if (answer <= 0 || answer >= Large)
				answer = -1;

			WriteLine(answer);
#if DEBUG
			//WriteLine($"{n} -> {answer}");
#endif
		}
	}

	public void Test()
	{
		var divisors = new long[1000001];
		var multiplicities = new long[divisors.Length];

		for (int i = 0; i < divisors.Length; i++)
			divisors[i] = 1;

		for (long i = 2; i < divisors.Length; i++)
		{
			if (multiplicities[i] != 0) continue;

			for (long k = i, p = 1; k < divisors.Length; k = k * i, p++)
			for (long j = k; j < divisors.Length; j += k)
				multiplicities[j] = (int)p;

			for (long j = i; j < divisors.Length; j += i)
				divisors[j] *= 2 * multiplicities[j] + 1;
		}

		var table = new long[4000001];
		for (long i = 1; i < table.Length; i++)
		{
			long j = i;
			var p2 = 0;
			while ((j & 1) == 0)
			{
				j >>= 1;
				p2++;
			}

			if (j >= divisors.Length) continue;

			long div = divisors[j];

			//v = divisors[i * i / 4];
			//table[i] = divisors[j * j] * (2 * (p2 - 1) + 1);
			long v = p2 != 0 ? div * (2 * p2 - 1) : div;

			table[i] = (v - 1) / 2;
		}

		var table2 = new int[1000001];
		for (int i = 0; i < table.Length; i++)
		{
			var v = table[i];
			if (v >= 0 && v < table2.Length && table2[v] == 0)
				table2[v] = i;
		}

		for (int i = 1; i < 4000; i++)
		{
			var answer1 = table2[i];
			var x = 2 * i + 1;

			var best = SearchForFirstSquareWithDivisors(x);
			var answer2 = best * 2;

			if (answer2 != answer1)
			{
				Console.Error.WriteLine($"Mismatch at {i}: {answer1} {answer2}");
			}
		}
	}

	static long Gcd(long a, long b)
	{
		return (a == 0) ? b : Gcd(b % a, a);
	}

	public static int[][] FactorsUpTo(int n)
	{
		n++;
		var counts = new short[n];
		var divisors = new int[n][];
		for (int i = 1; i < n; i++)
		{
			for (int j = i; j < n; j += i)
				counts[j]++;
		}

		for (int i = 1; i < n; i++)
			divisors[i] = new int[counts[i]];

		for (int i = 1; i < n; i++)
		{
			for (int j = i; j < n; j += i)
				divisors[j][--counts[j]] = i;
		}
		return divisors;
	}

	private static long[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101 };

	// From 110 Diophantine reciprocals

	private List<int> xfactors = new List<int>();

	public long SearchForFirstSquareWithDivisors(int desired)
	{
		GenerateFactors(factors, desired, xfactors);
		//if (f.Length == 2) return BigInteger.Pow(2, desired - 1);

		var best = Large;
		SearchForFirstSquareWithDivisors(xfactors,
			1, 0, xfactors.Count-1, 1, desired, ref best);
		return best;
	}

	private long Large = 10000000000000000;

	public void SearchForFirstSquareWithDivisors(List<int> xfactors, long num, int index, long bound,
		double divisors, double desired, ref long best)
	{
		// var pairings = (divisors + 1) / 2;
		if (divisors >= desired)
		{
			if (divisors == desired && best > num) best = num;
			return;
		}

		if (index >= primes.Length) return;
		var p = primes[index];

		for (int j = 1; j <= bound; j++)
		{
			var d = xfactors[j];
			var pow = d - 1;
			if ((pow & 1) == 1)
				continue;

			var div2 = divisors * d;
			if (div2 > desired) continue;

			double pf = Pow(p, pow >> 1);
			if (num * pf > Large) break;
			var v = num * (long)pf;
			SearchForFirstSquareWithDivisors(xfactors, v, index + 1, j, div2, desired, ref best);
		}
	}

	public static void GenerateFactors(int[] factors, int n, List<int> result)
	{
		result.Clear();
		result.Add(1);
		var prev = -1;
		int start = 0;
		while (n > 1)
		{
			var f = factors[n];
			if (f != prev) { start = 0; prev = f; }
			n /= f;
			int end = result.Count;
			for (; start < end; start++)
				result.Add(result[start] * f);
		}
		result.Sort();
	}

	public static int[] PrimeFactorsUpTo(int n)
	{
		var factors = new int[n + 1];

		for (int i = 2; i <= n; i += 2)
			factors[i] = 2;

		var sqrt = (int)Math.Sqrt(n);
		for (int i = 3; i <= sqrt; i += 2)
		{
			if (factors[i] != 0) continue;
			for (int j = i * i; j <= n; j += i + i)
			{
				if (factors[j] == 0)
					factors[j] = i;
			}
		}

		for (int i = 3; i <= n; i += 2)
		{
			if (factors[i] == 0)
				factors[i] = i;
		}

		return factors;
	}
}class CaideConstants {
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