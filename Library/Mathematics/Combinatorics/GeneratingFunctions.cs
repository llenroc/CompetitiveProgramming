using System;
using static System.Math;
using T = System.Int64;

namespace Softperson.Mathematics.Combinatorics
{
	public class GeneratingFunctions
	{
		public static long[] MultiplyPolynomials(long[] a, long[] b, int size = 0)
		{
			if (size == 0) size = a.Length + b.Length - 1;
			size = Min(a.Length + b.Length - 1, size);
			var result = new long[size];
			for (int i = 0; i < a.Length; i++)
				for (int j = Min(size-i, b.Length)-1; j >=0; j--)
					result[i + j] += a[i] * b[j];
			return result;
		}


		public static T ConvolutionTerm(T[] a, T[] b, int term, int mod = 1000000007)
		{
			if (a.Length > b.Length)
				return ConvolutionTerm(b, a, term, mod);

			long sum = 0;
			for (int i = Min(a.Length - 1, term); i >= 0; i--)
			{
				int j = term - i;
				if (j >= b.Length) break;
				sum += a[i] * b[j] % mod;
			}

			return (T)(sum % mod);
		}

	}
}
