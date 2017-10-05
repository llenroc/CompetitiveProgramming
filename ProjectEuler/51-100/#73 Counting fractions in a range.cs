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
using System.Runtime.CompilerServices;
using System.Xml.Schema;

class Solution
{
	// public const int MOD = 1000000007;

	static int limit;
	static Dictionary<long, long> cache = new Dictionary<long, long>();
	private static int[] factors;

	public void solve()
	{
		var a = Ni();
		limit = Ni();

		//var factors = PrimeFactorsUpTo((int)n);

		factors = PrimeFactorsUpTo(Math.Max(100,limit));

		var d1 = a;
		var d2 = a + 1;

		long total = 0;
		for (int i = a + a + 1; i <= limit; i++)
		{
			var lo = i / d2 + 1;
			var hi = (i + d1-1) / d1 - 1;
			var cnt = hi - lo + 1;

			long coplo = NumberUpToKCoprimeWithP(factors, lo-1, i);
			long cophi = NumberUpToKCoprimeWithP(factors, hi, i);
			long coprime = cophi - coplo;
			total += coprime;

			//Console.Error.WriteLine($"Added {cnt} from {lo}/{i} to {hi}/{i}");

		}


		//for (int i = 0; i < 12; i++)
		//{
		//	Console.Error.WriteLine($"{i}->{NumberUpToKCoprimeWithP(factors, i, 12)}");
		//}

		WriteLine(total);
	}

	public static long NumberUpToKCoprimeWithP(int[] table, int k, int p)
	{
		var totient = TotientFunction(table, p);
		var wholes = k / p;
		var frac = k % p;
		long parts = table[p] > frac ? frac : k-ScanCoprime(table, frac, p);
		return wholes * totient + parts;
	}

	static int ScanCoprime(int[] table, int frac, int p, int f2 = 1)
	{
		if (p == 1) return frac;

		var f = table[p];
		if (f > frac) return frac;
		do p /= f; while (p != 1 && table[p] == f);

		int result = 0;
		var f2New = f * f2;
		if (f2New <= frac)
		{

			result += frac / f2New;
			result += ScanCoprime(table, frac, p, f2);
			result -= ScanCoprime(table, frac, p, f2New);
		}
		return result;
	}

	static long SternBrocotCount(long d1, long d2)
	{
		long result = 0;
		while (true)
		{
			var d = d1 + d2;
			if (d > limit) break;
			result++;
			result += SternBrocotCount(d1, d);
			d1 = d;
		}
		return result;
	}


	public static int TotientFunction(int[] table, int n)
	{
		int prev = 0;
		int result = n;
		for (int k = n; k > 1; k /= prev)
		{
			int next = table[k];
			if (next != prev) result -= result / next;
			prev = next;
		}
		return result;
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



	static long Gcd(long a, long b)
	{
		if (a == 0) return b;
		return Gcd(b % a, a);
	}


	public static BigInteger Inverse(BigInteger x, BigInteger mod)
	{
		BigInteger xFactor, modFactor;
		var gcd = ExtendedGcd(x, mod, out xFactor, out modFactor);

		var inverse = ((xFactor % mod) + mod) % mod;
		return inverse;
	}

	public static BigInteger ExtendedGcd(BigInteger x, BigInteger y, out BigInteger xFactor, out BigInteger yFactor)
	{
		BigInteger gcd;

		if (x > y)
			return ExtendedGcd(y, x, out yFactor, out xFactor);

		if (x < 0)
		{
			gcd = ExtendedGcd(-x, y, out xFactor, out yFactor);
			xFactor = -xFactor;
			yFactor = -yFactor;
			return gcd;
		}

		if (x == y || x <= 1)
		{
			xFactor = 1;
			yFactor = 0;
			gcd = x;
			return gcd;
		}

		var quot = y / x;
		var rem = y % x;

		BigInteger xFactor2;
		gcd = ExtendedGcd(rem, x, out yFactor, out xFactor2);

		// yFactor2 * x + xfactor2 * rem = gcd
		// quot * x + rem = y 
		// ---------------------
		// xFactor * x + yFactor2 * (y - quot *x) = gcd
		// (xFactor2 - yFactor * quot)*x + yFactor * y = gcd

		xFactor = xFactor2 - yFactor * quot;
		return gcd;
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