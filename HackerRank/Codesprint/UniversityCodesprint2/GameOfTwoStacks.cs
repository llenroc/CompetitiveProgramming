
namespace HackerRank.UniversityCodesprint2.GameOfTwoStacks
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Linq;
	using static FastIO;

	class Solution
	{

		static long[] a;
		static long[] b;

		static void Main(String[] args)
		{

			InitIO();
			int g = Ni();

			for (int a0 = 0; a0 < g; a0++)
			{
				var na = Ni();
				var nb = Ni();
				var x = Ni();

				a = new long[na];
				long sum = 0;
				for (int i = 0; i < na; i++) a[i] = sum = sum + Ni();

				b = new long[nb];
				sum = 0;
				for (int i = 0; i < nb; i++) b[i] = sum = sum + Ni();

				long max = 0;
				for (int i = -1; i < a.Length; i++)
				{
					var asum = i < 0 ? 0 : a[i];
					if (asum > x) break;

					var t0 = Try(x - asum);
					var t = i + 1 + t0;
					if (t > max)
						max = t;
				}

				Console.WriteLine(max);
			}
		}

		static int Try(long v)
		{
			int i = Array.BinarySearch(b, 0, b.Length, v + 1);
			if (i < 0) i = ~i;
			while (i < b.Length && i > 0 && b[i] == b[i - 1]) i--;
			return i;
		}
	}


	public static class FastIO
	{
		static System.IO.Stream stream;
		static int idx, bytesRead;
		static byte[] buffer;
		static System.Text.StringBuilder builder;
		const int bufferSize = 4096 * 2;

		public static void InitIO(
			int stringCapacity = 16,
			System.IO.Stream input = null)
		{
			builder = new System.Text.StringBuilder(stringCapacity);
			stream = input ?? Console.OpenStandardInput(bufferSize);
			idx = bytesRead = 0;
			buffer = new byte[bufferSize];
		}


		static void ReadMore()
		{
			idx = 0;
			bytesRead = stream.Read(buffer, 0, buffer.Length);
			if (bytesRead <= 0) buffer[0] = 32;
		}

		public static int Read()
		{
			if (idx >= bytesRead) ReadMore();
			return buffer[idx++];
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
			int c;
			do c = Read(); while ((uint)c - 33 >= (127 - 33));
			bool neg = false;
			if (c == '-') { neg = true; c = Read(); }

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
			int c;
			do c = Read(); while ((uint)c - 33 >= (127 - 33));
			for (int i = 0; i < n; i++, c = Read()) list[i] = (byte)c;
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
	}
}