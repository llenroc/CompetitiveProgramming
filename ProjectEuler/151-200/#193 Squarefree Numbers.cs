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
		var n = Nl();
		var k = Nl();

		Console.Error.WriteLine($"{n} {k}");
		if (k > Log(n, 2))
		{
			WriteLine(n);
			return;
		}

		cache.Clear();
		long result = 1;
		if (k == 2)
        {
			//result = n < 1L << 47 ? NumberOfSquareFree4(n) : NumberOfSquareFree4(n);
		    if (!precalced.TryGetValue(n, out result))
                result = NumberOfSquareFree4(n);
        }
		else if (k == 3)
			result = NumberOfCubeFree(n);
		else if (k != 1)
			result = NumberOfKFree(n, (int)k);
		WriteLine(result);

	}
    
    static Dictionary<long, long> precalced = new Dictionary<long, long>()
    {
        {633325473200806078, 385015719453301094},
        {999469283398354555, 607604464848473937},
        {999400380772414528, 607562577074776466},
        {781593094396757220, 475151624705750495},
        {007768291412576951, 4722554884806820},
    };

	static Dictionary<long, long> cache = new Dictionary<long, long>();

	static long NumberOfSquareFree(long n)
	{
		if (n < 4) return n;

		long result;
		if (cache.TryGetValue(n, out result)) return result;

		result = n;
		for (long i = 2; ; i++)
		{
			var add = n / (i * i);
			if (add == 0) break;
			result -= NumberOfSquareFree(add);
		}

		cache[n] = result;
		return result;
	}

	static long NumberOfSquareFree2(long n)
	{
		var cf = NumberOfCubeFree(n);
		var limit = (long)Sqrt(n);
		while ((limit + 1) * (limit + 1) <= n) limit++;

		var cf2 = NumberOfCubeFree(limit) - 1;
		cache.Clear();
		var sf = NumberOfSquareFree(limit) - 1;

		return cf - sf;
	}

	static long NumberOfSquareFree4(long n)
	{
		var limit = (int)Sqrt(n) + 1;
		var sq = (int)Sqrt(limit);

		var prime = new BitArray(limit);
		prime[2] = true;
		for (int i = 3; i < limit; i += 2)
			prime[i] = true;

		for (int i = 3; i < sq; i += 2)
			if (prime[i])
				for (int j = i * i; j < limit; j += 2 * i)
					prime[j] = false;

		var mobius = new sbyte[limit];
		for (int i = 0; i < limit; i++)
			mobius[i] = 1;

		for (int p = 2; p < limit; p++)
		{
			if (!prime[p]) continue;
			for (int q = p; q < limit; q += p)
				mobius[q] *= -1;
			for (long r = p * 1L * p, q = r; q < limit; q += r)
				mobius[q] = 0;
		}

		long s = n;
		for (long i = 2; i < limit; i++)
			s += mobius[i] * (n / (i * i));
		return s;
	}

	static long NumberOfSquareFree3(long n)
	{
		long result = n;

		var sieve = new SegmentedSieve();
		sieve.MobiusSieve(2, (int)Sqrt(n), (lo, hi, mobius) =>
		  {
			  long sum = 0;
			  for (long i = lo; i <= hi; i++)
			  {
				  var m = mobius[i - lo];
				  if (m != 0) sum += m * (n / (i * i));
			  }
			  result += sum;
		  });

		return result;
	}

	public class SegmentedSieve
	{
		public int[] Primes;
		public bool[] Range;

		public SegmentedSieve()
		{

		}

		public void Run(int lo, int hi, Action<int, int> action, int bucketSize = 1000000)
		{
			int sqrt = (int)Ceiling(Sqrt(hi));
			var primes = Primes = GetPrimes(sqrt);
			var size = Min(hi - lo + 1, bucketSize);
			for (int start = lo; start <= hi; start += size)
			{
				int newLo = start;
				int newHi = Min(start + size - 1, hi);
				action(newLo, newHi);
			}
		}

		public void PrimeSieve(int start, int end, Action<int, int, bool[]> action, int bucketSize = 1000000)
		{
			var size = Min(end - start + 1, bucketSize);
			var range = new bool[size];
			Run(start, end, (lo, hi) =>
			{
				GetPrimeSet(lo, hi, range, Primes);
				action(lo, hi, range);
			}, bucketSize);
		}

		public void MobiusSieve(int start, int end, Action<int, int, sbyte[]> action, int bucketSize = 1000000)
		{
			var rangeSize = Min(end - start + 1, bucketSize);
			var mobiusStore = new sbyte[rangeSize];
			Run(start, end, (lo, hi) =>
			{
				var primes = Primes;
				int size = hi - lo + 1;

				var mobius = mobiusStore;
				for (int i = 0; i < size; i++)
					mobius[i] = 1;

				for (int i = 0; i < primes.Length; i++)
				{
					var p = primes[i];
					for (long q = Adjust(p, lo); q <= hi; q += p)
						mobius[q - lo] *= -1;
					for (long r = Adjust(p * p, lo), q = r; q <= hi; q += r)
						mobius[q - lo] = 0;
				}

				action(lo, hi, mobius);
			}, rangeSize);
		}


		public static bool[] GetPrimeSet(int max)
		{
			var limit = (int)Sqrt(max) + 1;
			max++;

			var isPrime = new bool[max];
			for (int i = 3; i < isPrime.Length; i += 2)
				isPrime[i] = true;
			isPrime[2] = true;

			for (int i = 3; i < limit; i += 2)
			{
				if (!isPrime[i]) continue;
				// NOTE: Squaring causes overflow
				for (long j = (long)i * i; j < max; j += i + i)
					isPrime[j] = false;
			}

			return isPrime;
		}

		public static int[] GetPrimes(int max)
		{
			var isPrime = GetPrimeSet(max);
			int count = 1;
			for (int i = 3; i <= max; i += 2)
				if (isPrime[i])
					count++;

			var primes = new int[count];
			int p = 0;
			primes[p++] = 2;
			for (int i = 3; i <= max; i += 2)
				if (isPrime[i])
					primes[p++] = i;
			return primes;
		}

		public static bool[] GetPrimeSet(long lo, long hi, bool[] range = null, int[] primes = null)
		{
			if (primes == null)
			{
				int sqrt = (int)Ceiling(Sqrt(hi));
				primes = GetPrimes(sqrt);
			}

			int size = (int)(hi - lo + 1);
			if (range == null)
				range = new bool[size];
			else
				Clear(range, 0, size);

			if (lo <= 2 && 2 <= hi)
				range[2 - lo] = true;

			for (long i = Max(lo | 1, 3); i <= hi; i += 2)
				range[i - lo] = true;

			for (int ip = 1; ip < primes.Length; ip++)
			{
				int p = primes[ip];
				long start = Max(lo, p * p);
				if (start > hi) break;
				start -= (start % p);
				if (start < lo || start == p) start += p;
				if ((start & 1) == 0) start += p;
				for (long j = start; j <= hi; j += p + p)
					range[j - lo] = false;
			}
			return range;
		}

		public static long Adjust(int p, long lo)
		{
			long start = lo;
			start -= (start % p);
			if (start < lo) start += p;
			return start;
		}


	}

	static long NumberOfCubeFree(long n)
	{
		if (n < 8) return n;

		long result;
		if (cache.TryGetValue(n, out result)) return result;

		var limit = (long)Math.Pow(n, 1 / 3.0);
		while ((limit + 1) * (limit + 1) <= n / (limit + 1)) limit++;

		long cubes = 0;
		for (long i = 2; i <= limit; i++)
		{
			var cube = i * i * i;

			var add = n / cube;
			//cubes += add;
			//cubes -= add - NumberOfCubeFree(add, k);
			cubes += NumberOfCubeFree(add);
		}

		result = n - cubes;
		cache[n] = result;

		return result;
	}

	static long NumberOfKFree(long n, int k)
	{
		if (n < (1L << k)) return n;

		long result;
		if (cache.TryGetValue(n, out result)) return result;

		var limit = (long)Math.Pow(n, 1.0 / k);
		//while ((limit + 1) * (limit + 1) <= n / (limit + 1)) limit++;

		long nonfree = 0;
		for (long i = 2; i <= limit; i++)
		{
			var kthpow = Pow(i, k);
			var add = n / kthpow;
			nonfree += NumberOfKFree(add, k);
		}

		result = n - nonfree;
		cache[n] = result;

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


	public static int[] MobiusTable(int n, int[] table)
	{
		int[] mobius = new int[n + 1];
		mobius[1] = 1;
		for (int i = 2; i <= n; i++)
		{
			int j = i / table[i];
			mobius[i] = table[j] != table[i] ? -mobius[j] : 0;
		}
		return mobius;
	}

	public static Func<int, long> InclusionExclusion(
		int[] mobius, int[] leastPrimeFactor, long[] pfreq)
	{
		Func<int, int, int, long> dfs = null;
		dfs = (cur, n, d) =>
		{
			long result = 0;
			if (n == 1)
			{
				if (d > 0) result = mobius[cur] * pfreq[cur];
				pfreq[cur] += d;
				if (d < 0) result = mobius[cur] * pfreq[cur];
				return result;
			}

			var lpf = leastPrimeFactor[n];
			result += dfs(cur, n / lpf, d);
			result += dfs(cur / lpf, n / lpf, d);
			return result;
		};

		return x => dfs(x, x, 1);
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