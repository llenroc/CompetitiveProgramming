//namespace HackerRank.WeekOfCode31.Problem
// Powered by caide (code generator, tester, and library code inliner)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static FastIO;

public class Solution
{
	public void solve(Stream input, Stream output)
	{
		InitInput(input);
		InitOutput(output);
		solve();
		Flush();
	}

	int[] r;
	long[] dp; 
	long n, s, t;
	long r0, g, seed, p;

	public void solve()
	{
		n = Ni();
		s = Ni();
		t = Ni();

		r0 = Ni();
		g = Ni();
		seed = Ni();
		p = Ni();

		r = new int[n];
		r[0] = (int) r0;
		for (int i=1; i<n; i++)
		{
			long ri = r[i - 1] * g + seed; // BUGGY
			r[i] = (int) (ri % p);
		}

		long left = s;
		long right = s;
		long maxleft = left - r[s];
		long maxright = right + r[s];

		long ans = -1;
		for (int k=0; k<=n; k++)
		{
			if (Between(left, right, t))
			{
				ans = k;
				break;
			}

			var newLeft = left;
			var newRight = right;

			for (long i=maxleft; i<left; i++)
			{
				var v = r[Fix(i)];
				newLeft = Math.Min(i - v, newLeft);
				newRight = Math.Max(i + v, newRight);
			}

			for (long i = right+1; i<= maxright; i++)
			{
				var v = r[Fix(i)];
				newLeft = Math.Min(i - v, newLeft);
				newRight = Math.Max(i + v, newRight);
			}

            if (left == maxleft && right == maxright)
                break;
            
			left = maxleft;
			right = maxright;
			maxleft = newLeft;
			maxright = newRight;
		}

		WriteLine(ans);
	}

	bool Between(long left, long right, long pos)
	{
		var count = right - left;

		var fixleft = Fix(left);
		if (pos < fixleft)
			pos += n;

		return pos - fixleft <= count;
	}


	public long Fix(long x)
	{
		return ((x % n) + n) % n;
	}



}



public static class FastIO
{
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

public static class Parameters
{
#if DEBUG
	public const bool Verbose = true;
#else
	public const bool Verbose = false;
#endif
}

class CaideConstants {
    public const string InputFile = null;
    public const string OutputFile = null;
}
public class Program {
    public static void Main(string[] args)
    {
        Solution solution = new Solution();
        solution.solve(Console.OpenStandardInput(), Console.OpenStandardOutput());

		Console.Error.WriteLine(System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime);
	}
}

