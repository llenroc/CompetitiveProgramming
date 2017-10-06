using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using static System.Array;
using static System.Math;

namespace Softperson.Mathematics.Numerics
{

	public class FastFourierTransformMod
	{
		long MaxN;
		private double[] wr;
		private double[] wi;
		private double[] ar;
		private double[] ai;
		private double[] br;
		private double[] bi;
		private double[] nar;
		private double[] nai;
		private double[] nbr;
		private double[] nbi;
		private const int sz = 16;
		const long msk = (1L << sz) - 1;
		private int MOD;

		public FastFourierTransformMod(int shift = 20, int mod = 1000000007)
		{
			MOD = mod;
			MaxN = 1L << shift;
			var ff = 2 * PI / MaxN;
			wr = new double[MaxN];
			wi = new double[MaxN];
			ar = new double[MaxN];
			ai = new double[MaxN];
			br = new double[MaxN];
			bi = new double[MaxN];
			nar = new double[MaxN];
			nai = new double[MaxN];
			nbr = new double[MaxN];
			nbi = new double[MaxN];
			for (long i = 0; i < MaxN; i++)
			{
				var ang = ff * i;
				wr[i] = Cos(ang);
				wi[i] = Sin(ang);
			}
		}

		// TODO: Eliminate recursion
		unsafe void DoFFT(double* ir, double* ii, double* or, double* oi, long n, long k)
		{
			if (n == 1)
			{
				or[0] = ir[0];
				oi[0] = ii[0];
				return;
			}
			long t = MaxN / n;
			n >>= 1;
			DoFFT(ir, ii, or, oi, n, 2 * k);
			DoFFT(ir + k, ii + k, or + n, oi + n, n, 2 * k);
			for (long i = 0, j = 0; i < n; i++, j += t)
			{
				// Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
				// tmp = w[j] * output[i + n];
				var tmpr = wr[j] * or[i + n] - wi[j] * oi[i + n];
				var tmpi = wi[j] * or[i + n] + wr[j] * oi[i + n];
				or[i + n] = or[i] - tmpr;
				oi[i + n] = oi[i] - tmpi;
				or[i] += tmpr;
				oi[i] += tmpi;
			}
		}


		public unsafe List<long> Multiply(List<long> a, List<long> b)
		{

			// Try out both, sometimes the third one is faster even though the first is constant time.
#if false
			int n = Fftsize(a.Count + b.Count - 1);
#elif false
			int n = a.Count + b.Count - 1;
			while ((n & n-1) != 0) n++;
			// slowest
#else
			int n = a.Count + b.Count;
			while (BitCount(n) != 1) n++;
			// 	Why is this fastest of the three?
#endif

			for (int i = 0; i < n; i++)
			{
				var va = i < a.Count ? a[i] : 0;
				var vb = i < b.Count ? b[i] : 0;

				ar[i] = va & msk;
				ai[i] = va >> sz;
				br[i] = vb & msk;
				bi[i] = vb >> sz;
			}

			fixed (double* par = ar, pai = ai)
			fixed (double* pbr = br, pbi = bi)
			fixed (double* pnar = nar, pnai = nai)
			fixed (double* pnbr = nbr, pnbi = nbi)
			{
				DoFFT(par, pai, pnar, pnai, n, 1);
				DoFFT(pbr, pbi, pnbr, pnbi, n, 1);

				for (int i = 0, nin = 0; i < n; i++, nin = n - i)
				{
					var lAr = nar[i] + nar[nin];
					var lAi = nai[i] - nai[nin];
					var gAr = nai[i] + nai[nin];
					var gAi = nar[nin] - nar[i];

					var lBr = nbr[i] + nbr[nin];
					var lBi = nbi[i] - nbi[nin];
					var gBr = nbi[i] + nbi[nin];
					var gBi = nbr[nin] - nbr[i];

					ar[i] = (lAr * lBr - gAr * gBi - gAi * gBr - lAi * lBi) * 0.25;
					ai[i] = (lAi * lBr - gAi * gBi + gAr * gBr + lAr * lBi) * 0.25;
					br[i] = (gAr * lBr - gBi * lAi + gBr * lAr - gAi * lBi) * 0.25;
					bi[i] = (gBr * lAi + gBi * lAr + gAr * lBi + gAi * lBr) * 0.25;
				}

				DoFFT(par, pai, pnar, pnai, n, 1);
				DoFFT(pbr, pbi, pnbr, pnbi, n, 1);
			}

			Reverse(nar, 1, n - 1);
			Reverse(nai, 1, n - 1);
			Reverse(nbr, 1, n - 1);
			Reverse(nbi, 1, n - 1);
			var max = Min(n, 100001);
			var ans = new List<long>(max);
			for (long i = 0; i < max; i++)
			{
				long aa = (long)(Round(nar[i] / n) % MOD);
				long bb = (long)(Round(nbr[i] / n) % MOD);
				long cc = (long)(Round(nai[i] / n) % MOD);
				ans.Add((aa + (bb << sz) + (cc << 2 * sz)) % MOD);
			}
			while (ans.Count > 1 && ans[ans.Count - 1] == 0)
				ans.RemoveAt(ans.Count - 1);
			return ans;
		}

		public static int BitCount(long x)
		{
			int count;
			var y = unchecked((ulong)x);
			for (count = 0; y != 0; count++)
				y &= y - 1;
			return count;
		}

		public List<long> Pow(List<long> x, long n)
		{
			//if (n == 0) return new List<long> { 1 };
			//var t = PolyPow(x, n / 2);
			//return (n & 1) == 0 ? Multiply(t, t) : Multiply(x, Multiply(t, t));

			if (n <= 1) return n == 1 ? x : new List<long> { 1 };
			var t = Pow(x, n >> 1);
			var sq = Multiply(t, t);
			return (n & 1) == 0 ? sq : Multiply(x, sq);
		}


		public static int Fftsize(int size)
		{
			// int m = Max(2, HighestOneBit(size) << 2);
			int hb = HighestOneBit(size);
			if (size > hb) hb <<= 1;
			return Max(2, hb);
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
	}
}
