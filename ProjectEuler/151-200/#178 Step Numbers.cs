using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using static System.Array;
using static System.Math;
using static Library;
using T = System.Numerics.BigInteger;

class Solution
{
	string s;
	private int n;

	private T[,,,] dp;
	T m1 = -1;
	T one = 1;
	T zero = 0;

	public void solve()
	{
		s = T.Parse(Ns()).ToString();
		n = s.Length;

		// length, start, lo, hi

		dp = new T[2 /*n+1*/, 10, 10, 10];
		var lows = new int[n];
		var highs = new int[n];
        var step = new bool[n];
            
		lows[0] = highs[0] = s[0] - '0';
		step[0] = true;
        for (int i = 1; i < n; i++)
		{
            var prev = s[i-1] - '0';
			var sd = s[i] - '0';
			lows[i] = Min(sd, lows[i-1]);
			highs[i] = Max(sd, highs[i-1]);
            step[i] = step[i-1] && (sd==prev+1 || sd==prev-1);
		}
		
		T result = 0;
		for (int i = n - 1; i >= 0; i--)
		{
			int next = (i & 1);
            int curr = 1 - next;
			var sd = s[i] - '0';
            var sdPrev = i>0 ? s[i-1] - '0' : -1;

			for (int d = 0; d <= 9; d++)
			{
				for (int lo = 0; lo <= d; lo++)
				for (int hi = d; hi <= 9; hi++)
				{
					T count = zero;
                    
					if (i + 1 == n)
					{
						if (lo == 0 && hi == 9)
							count = one;
					}
					else
					{
                        T count1 = zero;
                        T count2 = zero;
						if (d > 0) count1 = Retrieve(next, d - 1, Min(lo, d - 1), hi);
						if (d < 9) count2 = Retrieve(next, d + 1, lo, Max(hi, d + 1));

                        count = count1;
                        if (!count2.IsZero)
                        {
                            if (count.IsZero) count = count2;
                            else count += count2;
                        }
					}
					Set(curr, d, lo, hi, count);
				}
                
                if (d < sd)
                {
                    if (i==0 && d>0)
                        result += Retrieve(curr, d, d, d);
                }
                else if (d==sd && step[i])
                {
                    if (i+1 < n)
                    {
                        if ( d+1 < s[i+1]-'0')
                            result += Retrieve(curr, d, lows[i], highs[i]);
                        else if (d > 0 && d-1 < s[i+1]-'0')
                            result += Retrieve(next, d - 1, Min(lows[i], d - 1), highs[i]);
                    }
                }
                
				if (d>0 && i>0) result += Retrieve(curr, d, d, d);
			}
		}

		WriteLine(result);
	}

    public T Dfs(int index, int d, int lo, int hi, bool top)
	{
		T count;

		if (top)
		{
			var sd = s[index] - '0';
			if (d > sd) return zero;
			if (d < sd) top = false;
		}
		else
		{
			count = Retrieve(index, d, lo, hi);
			if (count >= 0) return count;
		}

        count = zero;
		if (index + 1 == n)
		{
			if (lo == 0 && hi == 9 && !top)
				count = one;
		}
		else
		{
			if (d > 0) count = Dfs(index + 1, d - 1, Min(lo, d - 1), hi, top);
			if (d < 9)
            {
                var tmp = Dfs(index + 1, d + 1, lo, Max(hi, d + 1), top);
                if (count == 0) count = tmp;
                else if (tmp != 0) count += tmp;
            }
		}

		if (!top)
			Set(index, d, lo, hi, count);

		return count;
	}
    
    
	T Retrieve(int index, int d, int lo, int hi)
	{
		if (false && d >= 5)
		{
			d = 9 - d;
			int lo2 = lo, hi2 = hi;
			lo = 9 - hi2;
			hi = 9 - lo2;
		}

		return dp[index, d, lo, hi];
	}

	void Set(int len, int d, int lo, int hi, T count)
	{
		if (false && d >= 5)
		{
			d = 9 - d;
			int lo2 = lo, hi2 = hi;

			lo = 9 - hi2;
			hi = 9 - lo2;
		}

		dp[len, d, lo, hi] = count;
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