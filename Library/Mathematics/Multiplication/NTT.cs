using System.Diagnostics;
using static System.Math;


// https://www.hackerrank.com/rest/contests/w28/challenges/definite-random-walks/hackers/uwi/download_solution
namespace HackerRank.Experiment
{
	public class NTT
	{

		public static long[,] PrimeData = new long[,]
		{
			{1053818881, 7, 20, 1048576},
			{1051721729, 6, 20, 1048576},
			{1045430273, 3, 20, 1048576},
			{1012924417, 5, 21, 2097152},
			{1007681537, 3, 20, 1048576},
			{1004535809, 3, 21, 2097152},
			{998244353, 3, 23, 8388608},
			{985661441, 3, 22, 4194304},
			{976224257, 3, 20, 1048576},
			{975175681, 17, 21, 2097152},
			{962592769, 7, 21, 2097152},
			{950009857, 7, 21, 2097152},
			{943718401, 7, 22, 4194304},
			{935329793, 3, 22, 4194304},
			{924844033, 5, 21, 2097152},
		};

		//	public static  int[] Primes = {1053818881, 1051721729, 1045430273, 1012924417, 1007681537, 1004535809, 998244353, 985661441, 976224257, 975175681};
		//	public static  int[] PrimitiveRoots = {7, 6, 3, 5, 3, 3, 3, 3, 3, 17};
		public static int[] Primes = { 1012924417, 1004535809, 998244353, 985661441, 975175681, 962592769, 950009857, 943718401, 935329793, 924844033 };
		public static int[] PrimitiveRoots = { 5, 3, 3, 3, 17, 7, 7, 7, 3, 5 };

		public static int NttSize(int size)
		{
			// int m = Max(2, HighestOneBit(size) << 2);
			int hb = HighestOneBit(size);
			if (size > hb) hb <<= 1;
			return Max(2, hb);
		}

		public static long[] ConvoluteSimply(long[] a, long[] b, int P, int g)
		{
			int m = NttSize(a.Length + b.Length - 1);
			long[] fa = Nttmb(a, m, false, P, g);
			long[] fb = Nttmb(b, m, false, P, g);
			for (int i = 0; i < m; i++)
				fa[i] = fa[i] * fb[i] % P;
			return Nttmb(fa, m, true, P, g);
		}

		public static long[] Exponentiate(long[] a, int n, int use = 0, int mod = 1000000007)
		{
			if (use == 0) use = Min(Primes.Length, n + 2);

			int m = NttSize((a.Length - 1) * n + 1);
			long[][] fs = new long[use][];
			for (int k = 0; k < use; k++)
			{
				int P = Primes[k], g = PrimitiveRoots[k];
				long[] fa = Nttmb(a, m, false, P, g);
				for (int i = 0; i < m; i++)
				{
					long v = fa[i];
					for (int j = 1; j < n; j++)
						fa[i] = fa[i] * v % P;
				}
				fs[k] = Nttmb(fa, m, true, P, g);
			}

			return FixUp(fs, use, mod);
		}

		public static long[] Convolute(long[] a, long[] b, int use = 2, int mod = 1000000007)
		{
			int m = NttSize(a.Length + b.Length - 1);
			long[][] fs = new long[use][];
			for (int k = 0; k < use; k++)
			{
				int P = Primes[k], g = PrimitiveRoots[k];
				long[] fa = Nttmb(a, m, false, P, g);
				long[] fb = a == b ? fa : Nttmb(b, m, false, P, g);
				for (int i = 0; i < m; i++)
					fa[i] = fa[i] * fb[i] % P;
				fs[k] = Nttmb(fa, m, true, P, g);
			}

			return FixUp(fs, use, mod);
		}

		static long[] FixUp(long[][] fs, int use, int mod)
		{
			int[] mods = CopyOf(Primes, use);
			long[] gammas = GarnerPrepare(mods);
			int[] buf = new int[use];
			for (int i = 0; i < fs[0].Length; i++)
			{
				for (int j = 0; j < use; j++) buf[j] = (int)fs[j][i];
				long[] res = GarnerBatch(buf, mods, gammas);
				long ret = 0;
				for (int j = res.Length - 1; j >= 0; j--) ret = (ret * mods[j] + res[j]) % mod;
				fs[0][i] = ret;
			}
			return fs[0];
		}



		public static T[] CopyOf<T>(T[] array, int n)
		{
			var result = new T[n];
			var min = Min(n, array.Length);
			for (int i = 0; i < min; i++)
				result[i] = array[i];
			return result;
		}

		// static int[] wws = new int[270000]; // outer faster

		// Modifed Montgomery + Barrett
		private static long[] Nttmb(long[] src, int n, bool inverse, int P, int g)
		{
			long[] dst = CopyOf(src, n);

			int h = NumberOfTrailingZeros(n);
			long K = HighestOneBit(P) << 1;
			int H = NumberOfTrailingZeros(K) * 2;
			long M = K * K / P;

			int[] wws = new int[1 << h - 1];
			long dw = inverse ? Pow(g, P - 1 - (P - 1) / n, P) : Pow(g, (P - 1) / n, P);
			long w = (1L << 32) % P;
			for (int k = 0; k < 1 << h - 1; k++)
			{
				wws[k] = (int)w;
				w = Modh(w * dw, M, H, P);
			}
			long J = Invl(P, 1L << 32);
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < 1 << i; j++)
				{
					for (int k = 0, s = j << h - i, t = s | 1 << h - i - 1; k < 1 << h - i - 1; k++, s++, t++)
					{
						long u = (dst[s] - dst[t] + 2 * P) * wws[k];
						dst[s] += dst[t];
						if (dst[s] >= 2 * P) dst[s] -= 2 * P;
						long Q = (long)((ulong)((u << 32) * J) >> 32);
						dst[t] = (u >> 32) - (Q * P >> 32) + P;
					}
				}
				if (i < h - 1)
				{
					for (int k = 0; k < 1 << h - i - 2; k++) wws[k] = wws[k * 2];
				}
			}
			for (int i = 0; i < n; i++)
			{
				if (dst[i] >= P) dst[i] -= P;
			}
			for (int i = 0; i < n; i++)
			{
				int rev = (int)((uint)Reverse(i) >> -h);
				if (i < rev)
				{
					long d = dst[i]; dst[i] = dst[rev]; dst[rev] = d;
				}
			}

			if (inverse)
			{
				long inv = Invl(n, P);
				for (int i = 0; i < n; i++) dst[i] = Modh(dst[i] * inv, M, H, P);
			}

			return dst;
		}

		public static int NumberOfLeadingZeros(long n)
		{
			return 64 - 1 - Log2(n);
		}

		public static int NumberOfTrailingZeros(long v)
		{
			var lastBit = v & -v;
			return lastBit != 0 ? Log2(lastBit) : 32;
		}

		public static int Reverse(int value)
		{
			unchecked
			{
				var n = unchecked((uint)value);
				n = n >> 16 | n << 16;
				n = n >> 0x8 & 0x00ff00ff | n << 0x8 & 0xff00ff00;
				n = n >> 0x4 & 0x0f0f0f0f | n << 0x4 & 0xf0f0f0f0;
				n = n >> 0x2 & 0x33333333 | n << 0x2 & 0xcccccccc;
				n = n >> 0x1 & 0x55555555 | n << 0x1 & 0xaaaaaaaa;
				return unchecked((int)n);
			}
		}

		public static int HighestOneBit(int n)
		{
			return n != 0 ? 1 << Log2(n) : 0;
		}

		public static int Log2(int value)
		{
			// TESTED
			var log = 0;
			if ((uint)value >= (1U << 12))
			{
				log = 12;
				value = (int)((uint)value >> 12);
				if (value >= (1 << 12))
				{
					log += 12;
					value >>= 12;
				}
			}
			if (value >= (1 << 6))
			{
				log += 6;
				value >>= 6;
			}
			if (value >= (1 << 3))
			{
				log += 3;
				value >>= 3;
			}
			return log + (value >> 1 & ~value >> 2);
		}

		public static int Log2(long value)
		{
			if (value <= 0)
				return value == 0 ? -1 : 63;

			var log = 0;
			if (value >= 0x100000000L)
			{
				log += 32;
				value >>= 32;
			}
			if (value >= 0x10000)
			{
				log += 16;
				value >>= 16;
			}
			if (value >= 0x100)
			{
				log += 8;
				value >>= 8;
			}
			if (value >= 0x10)
			{
				log += 4;
				value >>= 4;
			}
			if (value >= 0x4)
			{
				log += 2;
				value >>= 2;
			}
			if (value >= 0x2)
			{
				log += 1;
			}
			return log;
		}


		const long mask = (1L << 31) - 1;

		public static long Modh(long a, long M, int h, int mod)
		{
			long r = a - ((M * (a & mask) >> 31) + M * (a >> 31) >> h - 31) * mod;
			return r < mod ? r : r - mod;
		}

		private static long[] GarnerPrepare(int[] m)
		{
			int n = m.Length;
			Debug.Assert(n == m.Length);
			if (n == 0) return new long[0];
			long[] gamma = new long[n];
			for (int k = 1; k < n; k++)
			{
				long prod = 1;
				for (int i = 0; i < k; i++)
					prod = prod * m[i] % m[k];
				gamma[k] = Invl(prod, m[k]);
			}
			return gamma;
		}


		private static long Invl(long a, long mod)
		{
			long b = mod;
			long p = 1, q = 0;
			while (b > 0)
			{
				long c = a / b;
				long d;
				d = a;
				a = b;
				b = d % b;
				d = p;
				p = q;
				q = d - c * q;
			}
			return p < 0 ? p + mod : p;
		}

		private static long[] GarnerBatch(int[] u, int[] m, long[] gamma)
		{
			int n = u.Length;
			Debug.Assert(n == m.Length);
			long[] v = new long[n];
			v[0] = u[0];
			for (int k = 1; k < n; k++)
			{
				long temp = v[k - 1];
				for (int j = k - 2; j >= 0; j--)
				{
					temp = (temp * m[j] + v[j]) % m[k];
				}
				v[k] = (u[k] - temp) * gamma[k] % m[k];
				if (v[k] < 0) v[k] += m[k];
			}
			return v;
		}


		#region Mod Math

		private static long Pow(long a, long n, long mod)
		{
			//		a %= mod;
			long ret = 1;
			int x = 63 - NumberOfLeadingZeros(n);
			for (; x >= 0; x--)
			{
				ret = ret * ret % mod;
				if (n << 63 - x < 0)
					ret = ret * a % mod;
			}
			return ret;
		}

		#endregion

		public class SplitResult
		{
			public bool[] incycle;
			public int[] ord;
		}

		public SplitResult Split(int[] f)
		{
			int n = f.Length;
			bool[] incycle = new bool[n];
			for (int i = 0; i < n; i++)
				incycle[i] = true;

			int[] indeg = new int[n];
			for (int i = 0; i < n; i++) indeg[f[i]]++;
			int[] q = new int[n];
			int qp = 0;
			for (int i = 0; i < n; i++)
			{
				if (indeg[i] == 0) q[qp++] = i;
			}
			for (int r = 0; r < qp; r++)
			{
				int cur = q[r];
				indeg[cur] = -9999999;
				incycle[cur] = false;
				int e = f[cur];
				indeg[e]--;
				if (indeg[e] == 0) q[qp++] = e;
			}
			for (int i = 0; i < n; i++)
			{
				if (indeg[i] == 1)
				{
					q[qp++] = i;
				}
			}
			Debug.Assert(qp == n);
			var ret = new SplitResult
			{
				incycle = incycle,
				ord = q
			};
			return ret;
		}
	}
}
