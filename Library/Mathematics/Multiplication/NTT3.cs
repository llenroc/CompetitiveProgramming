using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Mathematics.Multiplication
{
	class NTT3
	{
		private const int mod = 7340033;
		private const int root = 5;
		private const int root_1 = 4404020;
		private const int root_pw = 1 << 20;

		public static void DiscreteFft(IList<int> a, bool invert)
		{
			int n = a.Count;

			for (int i = 1, j = 0; i < n; ++i)
			{
				int bit = n >> 1;
				for (; j >= bit; bit >>= 1)
					j -= bit;
				j += bit;
				if (i < j)
				{
					Utility.Swap(a, i, j);
				}
			}

			for (int len = 2; len <= n; len <<= 1)
			{
				int wlen = invert ? root_1 : root;
				for (int i = len; i < root_pw; i <<= 1)
					wlen = (int)(wlen * 1L * wlen % mod);
				for (int i = 0; i < n; i += len)
				{
					int w = 1;
					for (int j = 0; j < len / 2; ++j)
					{
						int u = a[i + j], v = (int)(a[i + j + len / 2] * 1L * w % mod);
						a[i + j] = u + v < mod ? u + v : u + v - mod;
						a[i + j + len / 2] = u - v >= 0 ? u - v : u - v + mod;
						w = (int)(w * 1L * wlen % mod);
					}
				}
			}
			if (invert)
			{
				int nrev = ModularMath.ModInverse(n, mod);
				for (int i = 0; i < n; ++i)
					a[i] = (int)(a[i] * 1L * nrev % mod);
			}
		}
	}
}
