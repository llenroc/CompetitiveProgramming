namespace HackerRank.WeekOfCode34.MaximumGcdAndSum
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using static FastInput;

	class Solution
	{

		static int maximumGcdAndSum(int[] A, int[] B)
		{

			var table = new byte[1000001];
			foreach (var a in A) table[a] |= 1;
			foreach (var b in B) table[b] |= 2;

			int maxsum = 0;
			for (int i = table.Length - 1; i >= 1; i--)
			{
				int maxa = 0;
				int maxb = 0;
				for (int j = i; j < table.Length; j += i)
				{
					var v = table[j];
					if ((v & 1) != 0) maxa = j;
					if ((v & 2) != 0) maxb = j;
				}

				if (maxa != 0 && maxb != 0)
				{
					maxsum = maxa + maxb;
					break;
				}
			}

			return maxsum;
		}

		static void Main(String[] args)
		{
			InitIO();
			int n = Ni();
			int[] A = Ni(n);
			int[] B = Ni(n);
			int res = maximumGcdAndSum(A, B);
			Console.WriteLine(res);
		}
	}


	public static class FastInput
	{
		static System.IO.Stream stream;
		static int idx, bytesRead;
		static byte[] buffer;
		static System.Text.StringBuilder builder;
		const int MonoBufferSize = 4096;


		public static void InitIO(
			int stringCapacity = 16,
			System.IO.Stream input = null)
		{
			builder = new System.Text.StringBuilder(stringCapacity);
			stream = input ?? Console.OpenStandardInput();
			idx = bytesRead = 0;
			buffer = new byte[MonoBufferSize];
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
	}
}