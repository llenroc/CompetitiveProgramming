using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using static System.Array;
using static System.Math;
using static Library;

class Solution
{
	// public const int MOD = 1000000007;
	private int[] rad;
	int[] factors;
	private const int bucketLength = 60;
	long[] bucket = new long[bucketLength];
	long[] limit = new long[bucketLength];
	long[] bucketAdd = new long[bucketLength];
	long ignore;
	Dictionary<long, int> dict = new Dictionary<long, int>();
	private long maxL;
	private int maxk;
	List<Pair> inputs = new List<Pair>();
	private long[] answers;

	public void solve()
	{
		int t = Ni();

		for (int i = 0; i < t; i++)
		{
			long L = Nl();
			int k = Ni();
			inputs.Add(new Pair { L = L, k = k, I = i });
			if (L > maxL) maxL = L;

			var log = Log2(L);
			if (limit[log] < k) limit[log] = k;
		}

		maxk = 200001; //inputs.Max(x => x.k)+1;
		rad = new int[maxk];
		factors = new int[maxk];

		factors[1] = 1;
		rad[1] = 1;
		for (int i = 2; i < rad.Length; i++)
		{
			if (rad[i] != 0) continue;
			for (int j = i; j < rad.Length; j += i)
			{
				if (factors[j] != 0)
					rad[j] *= i;
				else
					rad[j] = factors[j] = i;
			}
		}

		answers = new long[t];
		if (maxL > 200000)
		{
			Small();
		}
		else
		{
			Small();
		}

		foreach (var a in answers)
			WriteLine(a);
	}

	public void Big()
	{
        var prevL = 0;
		foreach (var p in inputs)
		{
			var k = p.k;
            var L = p.L;
            
			for (int i = 1; i < rad.Length; i++)
			{
				if (rad[i] != i) continue;
				dict.Clear();
				Generate2(i, i, i, p.L);
				if (k > dict.Count)
				{
					k -= dict.Count;
					continue;
				}

				k--;
				var list = dict.Keys.ToList();
				list.Sort((a, b) =>
				{
					int cmp = dict[a].CompareTo(dict[b]);
					if (cmp != 0) return cmp;
					return a.CompareTo(b);
				});

				answers[p.I] = list[k];
				break;
			}
		}
	}

	public void Small()
	{

		int lastChange = 0;
		for (int i = 1; i < rad.Length; i++)
		{
			if (rad[i] != i) continue;

#if DEBUG
			int oldCount = dict.Count;
#endif
			Generate(i, i, i);
#if DEBUG
			if (dict.Count > oldCount + 1)
				lastChange = i;
#endif
			long sum = 0;
			for (int j = 0; j < bucket.Length; j++)
			{
				sum += bucketAdd[j];
				bucketAdd[j] = 0;
				bucket[j] += sum;
			}

			for (int j = bucket.Length - 1; j > 1; j--)
			{
				if (bucket[j - 1] < limit[j]) break;
				ignore |= 1L << j;
			}
		}

		var sortedNumbers = dict.Keys.ToList();
		var sortedRad = sortedNumbers.ToList();

		sortedNumbers.Sort();
		sortedRad.Sort((a, b) =>
		{
			int cmp = dict[a].CompareTo(dict[b]);
			if (cmp != 0) return cmp;
			cmp = a.CompareTo(b);
			return cmp;
		});

		var order = new Dictionary<long, int>();
		for (int i = 0; i < sortedRad.Count; i++)
			order[sortedRad[i]] = i;

		inputs.Sort((a, b) => a.L.CompareTo(b.L));

		var ft = new FenwickTree(sortedRad.Count);
		int ni = 0;

#if DEBUG
		Console.Error.WriteLine($"Dict={dict.Count} LastChange={lastChange} Ignore={ignore:X}");
		Console.Error.WriteLine("[" + string.Join(",", bucket) + "]");
#endif

		foreach (var p in inputs)
		{
			while (ni < sortedNumbers.Count)
			{
				var n = sortedNumbers[ni];
				if (n > p.L) break;
				ft.Add(order[n], 1);
				ni++;
			}

			int index = ft.GetIndexGreater(p.k - 1);
			var ans = answers[p.I] = sortedRad[index];
		}
	}

	public void Generate(int rad, decimal number, int seed)
	{
		dict[(long)number] = rad;
		while (seed > 1)
		{
			var f = factors[seed];
			var num = number * f;
			if (num > maxL) break;

			var log = Log2((long)num);
			if (log > 0 && (ignore & 1L << log - 1) != 0)
				break;

			bucketAdd[log]++;
			Generate(rad, num, seed);
			seed /= f;
		}
	}

	public void Generate2(int rad, decimal number, int seed, long max)
	{
		dict[(long)number] = rad;
		while (seed > 1)
		{
			var f = factors[seed];
			var num = number * f;
			if (num <= max)
			 Generate2(rad, num, seed, max);
			seed /= f;
		}
	}

	public class Pair
	{
		public long L;
		public int k;
		public int I;
	}

	public class FenwickTree
	{
		#region Variables
		public readonly int[] A;
		#endregion

		#region Constructor
		/*public Fenwick(int[] a) : this(a.Length)
        {
            for (int i = 0; i < a.Length; i++)
                Add(i, a[i]);
        }*/

		public FenwickTree(int[] a) : this(a.Length)
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

		public FenwickTree(int size)
		{
			A = new int[size + 1];
		}
		#endregion

		#region Properties
		public int this[int index] => SumInclusive(index, index);

		public int Length => A.Length - 1;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public int[] Table
		{
			get
			{
				int n = A.Length - 1;
				int[] ret = new int[n];
				for (int i = 0; i < n; i++)
					ret[i] = SumInclusive(i);
				for (int i = n - 1; i >= 1; i--)
					ret[i] -= ret[i - 1];
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
		public void Add(int i, int val)
		{
			if (val == 0) return;
			for (i++; i < A.Length; i += (i & -i))
				A[i] += val;
		}

		// Sum from [0 ... i]
		public int SumInclusive(int i)
		{
			int sum = 0;
			for (i++; i > 0; i -= (i & -i))
				sum += A[i];
			return sum;
		}

		public int SumInclusive(int i, int j)
		{
			return SumInclusive(j) - SumInclusive(i - 1);
		}

		/// <summary>
		/// Inclusive update of the range [left, right] by value
		/// The default operation of the fenwick tree is point update - range query.
		/// We use this if we want alternative range update - point query.
		/// SumInclusive becomes te point query function.
		/// </summary>
		/// <returns></returns>
		public void RangeUpdateInclusive(int left, int right, int delta)
		{
			Add(left, delta);
			Add(right + 1, -delta);
		}

		// get largest value with cumulative sum less than x;
		// for smallest, pass x-1 and add 1 to result
		public int GetIndexGreater(int x)
		{
			int i = 0, n = A.Length - 1;
			for (int bit = HighestOneBit(n); bit != 0; bit >>= 1)
			{
				int t = i | bit;

				// if (t <= n && Array[t] < x) for greater or equal 
				if (t <= n && A[t] <= x)
				{
					i = t;
					x -= A[t];
				}
			}
			return i;
		}
		#endregion
	}

	public static int HighestOneBit(int n)
	{
		return n != 0 ? 1 << Log2(n) : 0;
	}

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


	public static int Log2(int value)
	{
		// TESTED
		var log = 0;
		if ((uint)value >= (1U << 12))
		{
			log = 12;
			value = (int)((uint)value >> 12);
			if (value >= (1 << 12))
			{
				log += 12;
				value >>= 12;
			}
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
		return log + (value >> 1 & ~value >> 2);
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