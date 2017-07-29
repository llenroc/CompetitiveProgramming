namespace HackRank.WorldSprint11.BestMask
{
	// https://www.hackerrank.com/contests/world-codesprint-11/challenges

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Text;
	using static FastIO;
	using static System.Math;
	using static System.Array;

	public class Solution
	{
		public void solve(Stream input, Stream output)
		{
			InitInput(input);
			InitOutput(output);
			solve();
			Flush();
		}

		int[][] lists;
		int n;
		int[] list;


		public void solve()
		{
			n = Ni();
			list = Ni(n);

			//while (true)
			{
				//int ans = Test();

				Sort(list);
				int max = list[n - 1];
				n = list.Length;

				lists = new int[26][];
				for (int i = 0; i < lists.Length && (1 << i) <= max; i++)
					lists[i] = new int[n];

				int result = DivideAndConquer(list, n);

				//var list2 = list.ConvertAll(x => x & ans);
				//Debug.Assert(ans == result);
				WriteLine(result);
			}
		}

		public int Test()
		{
			var random = new Random();

			n = 10;
			for (int i = 0; i < 10; i++)
				list[i] = (random.Next(1, 16384));

			int ans = int.MaxValue;
			for (int i = 1; i < 16384; i++)
			{
				bool good = true;
				for (int j = 0; j < list.Length; j++)
				{
					if ((list[j] & i) == 0)
					{
						good = false;
						break;
					}
				}

				if (good)
				{
					int cmp = BitCount(ans).CompareTo(BitCount(i));
					if (cmp > 0)
						ans = i;
				}
			}

			return ans;
		}



		int DivideAndConquer(int[] listParam, int length, int mask = 0, int exclude = 0, int max = int.MaxValue, int bitcount = 31, int depth = 0)
		{
			var list = lists[depth];
			//if (list == null) list = lists[depth] = new int[n];

			int listCount = 0;
			var prev = -1;
			var and = -1;
			int minBitCount = int.MaxValue;
			int minBitValue = int.MaxValue;
			for (int i = 0; i < length; i++)
			{
				var v = listParam[i] & ~exclude;
				if ((listParam[i] & mask) != 0 || v == prev) continue;
				list[listCount++] = v;
				and &= v;
				prev = v;

				var bc = BitCount(v & ~exclude);
				if (bc <= minBitCount)
				{
					if (bc < minBitCount)
					{
						if (bc == 0)
							return max;

						minBitCount = bc;
						minBitValue = v;
					}
					else
						minBitValue = Min(minBitValue, v);
				}
			}

			if (listCount == 0)
				return 0;

			if (and != 0)
				return and & -and;

			int result = max;
			int mask2 = minBitValue;
			while (mask2 != 0)
			{
				int bit = mask2 & -mask2;
				mask2 -= bit;

				var check = bit | mask;
				int cmp = BitCount(check).CompareTo(bitcount);
				if (cmp > 0 || cmp == 0 && check >= result) continue;

				var tmp = bit | mask | DivideAndConquer(list, listCount, bit | mask, exclude, result, bitcount, depth + 1);
				cmp = BitCount(tmp).CompareTo(bitcount);
				if (cmp < 0 || cmp == 0 && tmp < result)
				{
					result = tmp;
					bitcount = BitCount(tmp);
				}

				exclude |= bit;
			}

			return result;
		}

		public static int BitCount(int x)
		{
			int count;
			var y = unchecked((uint)x);
			for (count = 0; y != 0; count++)
				y &= y - 1;
			return count;
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