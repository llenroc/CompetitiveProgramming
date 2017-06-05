namespace HackerRank.WeekOfCode32.GeometricTrick
{
	// https://www.hackerrank.com/contests/w32/challenges/geometric-trick

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

		int n;
		char[] s;
		int[] factors;

		public void solve()
		{
			n = Ni();
			s = Nc(n);

			factors = PrimeFactorsUpTo(n);

			long count = 0;
			int j1;
			for (int j = 0; j < n; j++)
			{
				if (s[j] != 'b') continue;
				j1 = j + 1;
				EnumerateFactorsX2(factors, j1, j1, f =>
				{
					if (s[f - 1] == 'b') return;
					var f2 = 1L * j1 * j1 / f;
					Console.Error.WriteLine($"{j1},{f},{f2}");
					if (f2 > n || s[f - 1] != (s[f2 - 1] ^ 2)) return;
					count++;
				});
			}

			WriteLine(count);
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

		public static int EnumerateFactorsX2(int[] factors, long n, int max, Action<long> action = null, long f = 1)
		{
			if (f > max)
				return 0;

			if (n == 1)
			{
				action?.Invoke(f);
				return 1;
			}

			long p = factors[n];
			long c = 1;
			long next = n / p;
			while (next > 1 && factors[next] == p)
			{
				c++;
				next = next / p;
			}

			c *= 2;
			int result = EnumerateFactorsX2(factors, next, max, action, f);
			while (c-- > 0)
			{
				f *= p;
				result += EnumerateFactorsX2(factors, next, max, action, f);
			}

			return result;
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

	class CaideConstants
	{
		public const string InputFile = null;
		public const string OutputFile = null;
	}
	public class Program
	{
		public static void Main(string[] args)
		{
			Solution solution = new Solution();
			solution.solve(Console.OpenStandardInput(), Console.OpenStandardOutput());

#if DEBUG
			Console.Error.WriteLine(System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime);
#endif
		}
	}


}