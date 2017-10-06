using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerRank.Experiment.FFT
{
	class NTT2
	{
		const int Mod = 7340033;
		const int Root = 3;
		//const int Mod = 1012924417;
		//const int Root = 5;

		int Mul(int x, int y)
		{
			return (int) ( (long)x * y % Mod);
		}

		int Add(int x, int y)
		{
			return (x += y) >= Mod ? x - Mod : x;
		}

		int Sub(int x, int y)
		{
			return (x -= y) < 0 ? x + Mod : x;
		}

		int Modpow(int x, int y)
		{
			int ret = 1;
			while (y > 0)
			{
				if ((y & 1) != 0) 
				{
					ret = Mul(ret, x);
				}
				x = Mul(x, x);
				y /= 2;
			}
			return ret;
		}

		int Modinv(int x)
		{
			return Modpow(x, Mod - 2);
		}

		void Ntt(int[] a, int n, bool rev = false)
		{
			int h = 0;
			for (int i = 0; 1 << i < n; i++)
			{
				h++;
			}
			for (int i = 0; i < n; i++)
			{
				int j = 0;
				for (int k = 0; k < h; k++)
				{
					j |= (i >> k & 1) << (h - 1 - k);
				}
				if (i < j)
				{
					var tmp = a[i];
					a[i] = a[j];
					a[j] = tmp;
				}
			}
			for (int i = 1; i < n; i *= 2)
			{
				int w = Modpow(Root, (Mod - 1) / (i * 2));
				if (rev)
				{
					w = Modinv(w);
				}
				for (int j = 0; j < n; j += i * 2)
				{
					int wn = 1;
					for (int k = 0; k < i; k++)
					{
						int s = a[j + k + 0];
						int t = Mul(a[j + k + i], wn);
						a[j + k + 0] = Add(s, t);
						a[j + k + i] = Add(s, Mod - t);
						wn = Mul(wn, w);
					}
				}
			}
			int v = Modinv(n);
			if (rev)
			{
				for (int i = 0; i < n; i++)
					a[i] = Mul(a[i], v);
			}
		}
	}
}
