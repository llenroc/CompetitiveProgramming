using System;
using System.Collections.Generic;
using System.Numerics;

namespace Softperson.Mathematics
{

	// fft code taken from e-maxx.ru/algo/fft_multiply

	public class NumberTheoreticTransform
	{
		//const int Root = 1489;
		//const int Root1 = 296201594;
		//const int RootPw = (1 << 19);
		//const long Mod = 663224321; // arbitrary

		const int Root = 3;
		const int Root1 = 2446678;
		const int RootPw = (1 << 20);
		const int Mod = 7340033;


		long Power(long a, long b)
		{
			if (b == 0)
				return 1;
			long ans = Power(a, b / 2);
			ans = (ans * ans) % Mod;
			if (b % 2 != 0)
				ans = (ans * a) % Mod;
			return ans;
		}

		public void Fft(long[] a, bool invert)
		{
			int n = a.Length;
			for (int i = 1, j = 0; i < n; ++i)
			{
				int bit = n >> 1;
				for (; j >= bit; bit >>= 1)
					j -= bit;
				j += bit;
				if (i < j)
				{
					var tmp = a[i];
					a[i] = a[j];
					a[j] = tmp;
				}
			}

			for (int len = 2; len <= n; len <<= 1)
			{
				long wlen = invert ? Root1 : Root;
				for (int i = len; i < RootPw; i <<= 1)
					wlen = (wlen * wlen) % Mod;
				for (int i = 0; i < n; i += len)
				{
					long w = 1;
					for (int j = 0; j < len / 2; ++j)
					{
						long u = a[i + j];
						long v = (a[i + j + len / 2] * w) % Mod;
						a[i + j] = u + v < Mod ? u + v : u + v - Mod;
						a[i + j + len / 2] = u - v >= 0 ? u - v : u - v + Mod;
						w = (w * wlen) % Mod;
					}
				}
			}
			if (invert)
			{
				long ninv = Power(n, Mod - 2);
				for (int i = 0; i < n; ++i)
					a[i] = (a[i] * ninv) % Mod;
			}
		}

		void Multiply(IList<long> a, IList<long> b, out long[] c)
		{
			int sz = 2 * a.Count;
			var ta = new long[sz];
			var tb = new long[sz];
			a.CopyTo(ta, 0);
			b.CopyTo(tb, 0);
			Fft(ta, false);
			Fft(tb, false);
			for (int i = 0; i < sz; ++i)
				ta[i] = (1L * ta[i] * tb[i]) % Mod;

			Fft(ta, true);
			c = ta;
		}

	}

}
