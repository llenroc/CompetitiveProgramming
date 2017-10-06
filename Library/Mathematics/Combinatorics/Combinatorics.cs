﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Mathematics
{
	public class Combinatorics2
	{

		// https://www.hackerrank.com/contests/projecteuler/challenges/euler076/copy-from/1302323130
		public static BigInteger[] IntegerPartitionTable(int n)
		{
			var table = new BigInteger[n + 1];

			table[0] = 1;
			for (int i = 1; i < table.Length; i++)
			{
				for (int j = 1; ; j++)
				{
					var p = GeneralizedPentagonalNumber(j);
					if (p > i) break;
					var sign = ((j - 1) >> 1 & 1) == 0 ? 1 : -1;
					//if (i<100)
					//    Console.Error.WriteLine($"table[{i}] += table[{i}-{p}] * {sign}");
					table[i] += table[i - p] * sign;
				}

				//if (i<100||i%10==0)
				//Console.Error.WriteLine($"{i}->{table[i]}");
			}

			return table;
		}

		public static long PentagonalNumber(long k)
		{
			return k * (3 * k - 1) / 2;
		}

		public static long GeneralizedPentagonalNumber(long k)
		{
			if (k == 0) return k;
			var kk = (k + 1) >> 1;
			if ((k & 1) == 0) kk = -kk;
			return kk * (3 * kk - 1) / 2;
		}

		public static long SumIFrom01ToN(long n)
		{
			return n * (n + 1) >> 1;
		}

		public static long SumI(long m, long n)
		{
			// ArithmeticSeries
			// Also, (n*(n+1) - m*(m-1)/2
			return (n + 1 - m) * (n + m) >> 1;
		}

		public static long SumI2(long n)
		{
			// Square Pyramidal Number
			// Alternative, n * (n + 1) * (2 * n + 1) / 6;
			// Alternative, n*n*n/3 + n*n/2 + n/6
			return (n * (n + 1) >> 1) * (2 * n + 1) / 3;
		}

		public static long SumI3(long n)
		{
			// Alternative, n*n*n*n/4 + n*n*n/2 + n*n/4
			var tmp = n * (n + 1) / 2;
			return tmp * tmp;
		}


		public static long SumI4(long n)
		{
			// Alternative, n*n*n*n/4 + n*n*n/2 + n*n/4 (real numbers)
			// Alternative, ((((6 * n + 15) * n + 10) * n) * n - 1) * n / 30;
			return n * (n + 1) * (2 * n + 1) / 6 * (3 * n * n + 3 * n - 1) / 5;

		}

		public static long SumI5(long n)
		{
			// Alternatively, SumI3From01ToN(n) * (2*n*n + 2*n - 1) / 3
			var n2 = n * n;
			return (((2 * n + 6) * n + 5) * n2 - 1) * n2 / 12;
		}

		// https://en.wikipedia.org/wiki/Summation
		// https://www.math.uh.edu/~ilya/class/useful_summations.pdf

		public static long SumPowers(long a, long n)
		{
			// i from 0 to n
			return (1 - Pow(a, n + 1)) / (1 - a);
		}


		public static long SumPowers(long a, long m, long n)
		{
			// i from m to n
			return (Pow(a, m) - Pow(a, n + 1)) / (1 - a);
		}


		public static long SumIPowers(long a, long n)
		{
			var a1 = 1 - a;
			var pow = Pow(a, n);
			return a * (1 - (n + 1) * pow + a * n * pow) / (a1 * a1);
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

	}
}
