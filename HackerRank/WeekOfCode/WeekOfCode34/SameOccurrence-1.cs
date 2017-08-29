namespace HackerRank.WeekOfCode34.SameOccurence1
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.IO;
	using System.Collections.Generic;
	using static System.Array;
	using static System.Math;
	using static Library;

	class Solution
	{
		private int n, q;
		private int[] arr;
		private Dictionary<int, Bucket> buckets = new Dictionary<int, Bucket>();
		private Bucket empty = new Bucket();

		public void solve()
		{
			n = Ni();
			q = Ni();
			arr = Ni(n);

			for (int i = 0; i < n; i++)
			{
				var v = arr[i];
				if (buckets.ContainsKey(v) == false)
					buckets[v] = new Bucket() {X = v};
				buckets[v].Indices.Add(i);
			}

			long allRanges = AllRanges(n);
			int time = 0;
			int freqStart = n;
			int[] freq = new int[2 * n + 1];
			int[] timestamp = new int[freq.Length];

			var queries = new Query[q];

			for (int a0 = 0; a0 < q; a0++)
			{
				var qq = queries[a0] = new Query
				{
					X = Ni(),
					Y = Ni(),
				};

				if (buckets.ContainsKey(qq.X) == false) qq.X = 0;
				if (buckets.ContainsKey(qq.Y) == false) qq.Y = 0;
				if (qq.X > qq.Y) Swap(ref qq.X, ref qq.Y);
			}

			var answers = queries.ToArray();

			Array.Sort(queries, (a, b) =>
			{
				int cmp = a.X.CompareTo(b.X);
				if (cmp != 0) return cmp;
				return a.Y.CompareTo(b.Y);
			});

			Query prevQuery = null;
			foreach (var qq in queries)
			{
				int x = qq.X;
				int y = qq.Y;

				if (prevQuery != null && prevQuery.X == x && prevQuery.Y == y)
				{
					qq.Answer = prevQuery.Answer;
					continue;
				}
				prevQuery = qq;

				Bucket bx, by;
				buckets.TryGetValue(x, out bx);
				buckets.TryGetValue(y, out by);

				if (bx == by)
				{
					qq.Answer = allRanges;
					continue;
				}

				if (bx == null)
					Swap(ref bx, ref by);

				if (by == null)
				{
					long emptyCount = AllRanges(bx.First - 0) + AllRanges(n - (bx.Last + 1));
					int length = bx.Last - bx.First + 1;
					if (bx.Indices.Count == 2)
					{
						qq.Answer = emptyCount + AllRanges(bx.Last - bx.First - 1);
						continue;
					}
					else if (length - bx.Indices.Count < 2)
					{
						qq.Answer = emptyCount + (length - bx.Indices.Count);
						continue;
					}

					by = empty;
				}

				int i = 0;
				int j = 0;
				int prev = -1;
				int bal = freqStart;
				time++;

				timestamp[bal] = time;
				freq[bal] = 0;

				while (i < bx.Indices.Count || j < by.Indices.Count)
				{
					int v;
					int newBal;
					if (i >= bx.Indices.Count || j < by.Indices.Count && by.Indices[j] < bx.Indices[i])
					{
						v = by.Indices[j++];
						newBal = bal - 1;
					}
					else
					{
						v = bx.Indices[i++];
						newBal = bal + 1;
					}

					if (timestamp[newBal] < time)
					{
						timestamp[newBal] = time;
						freq[newBal] = 0;
					}

					freq[bal] += v - prev;
					prev = v;
					bal = newBal;
				}

				freq[bal] += n - prev;

				int start = freqStart;
				while (start > 0 && timestamp[start - 1] == time) start--;

				long count = 0;
				while (start < freq.Length && timestamp[start] == time)
				{
					long f = freq[start];
					count += f * (f - 1) / 2;
					start++;
				}

				qq.Answer = count;
			}

			foreach (var qq in answers)
				WriteLine(qq.Answer);
		}


		long AllRanges(long length)
		{
			return length * (length + 1) / 2;
		}

		public class Query
		{
			public long Answer;
			public int X;
			public int Y;
		}

		public class Bucket
		{
			public int X;
			public List<int> Indices = new List<int>();
			public int First => Indices[0];
			public int Last => Indices[Indices.Count - 1];
		}
	}


	class CaideConstants
	{
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
			if (neg)
			{
				c = Read();
			}

			int number = c - '0';
			while (true)
			{
				var d = Read() - '0';
				if ((uint) d > 9) break;
				number = number * 10 + d;
			}
			return neg ? -number : number;
		}

		public static long Nl()
		{
			var c = SkipSpaces();
			bool neg = c == '-';
			if (neg)
			{
				c = Read();
			}

			long number = c - '0';
			while (true)
			{
				var d = Read() - '0';
				if ((uint) d > 9) break;
				number = number * 10 + d;
			}
			return neg ? -number : number;
		}

		public static char[] Nc(int n)
		{
			var list = new char[n];
			for (int i = 0, c = SkipSpaces(); i < n; i++, c = Read()) list[i] = (char) c;
			return list;
		}

		public static byte[] Nb(int n)
		{
			var list = new byte[n];
			for (int i = 0, c = SkipSpaces(); i < n; i++, c = Read()) list[i] = (byte) c;
			return list;
		}

		public static string Ns()
		{
			var c = SkipSpaces();
			builder.Clear();
			while (true)
			{
				if ((uint) c - 33 >= (127 - 33)) break;
				builder.Append((char) c);
				c = Read();
			}
			return builder.ToString();
		}

		public static int SkipSpaces()
		{
			int c;
			do c = Read(); while ((uint) c - 33 >= (127 - 33));
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
			ulong number = (ulong) signedNumber;
			if (signedNumber < 0)
			{
				Write('-');
				number = (ulong) (-signedNumber);
			}

			Reserve(20 + 1); // 20 digits + 1 extra
			int left = outputIndex;
			do
			{
				outputBuffer[outputIndex++] = (byte) ('0' + number % 10);
				number /= 10;
			} while (number > 0);

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
				outputBuffer[outputIndex++] = (byte) s[i];
		}

		public static void Write(char c)
		{
			Reserve(1);
			outputBuffer[outputIndex++] = (byte) c;
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
}