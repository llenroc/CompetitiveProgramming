using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using static System.Math;

namespace Softperson.Mathematics
{
	public class FastFourierTransform
	{
		int M = 600005;
		int Logm = 20;

		readonly Complex[] aa;
		readonly Complex[] bb;
		int _revCalced = -1;
		int[] rev;
		Complex[,] ww;

		static int n;

		public FastFourierTransform(int m = 600005)
		{
			M = m;
			Logm = (int)Ceiling(Log(M, 2));
			aa = new Complex[M];
			bb = new Complex[M];
			ww = new Complex[Logm, M];
			rev = new int[M];

		}

		void Swap<T>(ref T a, ref T b)
		{
			var tmp = a;
			a = b;
			b = tmp;
		}

		void Fft(Complex[] a, int n, bool invert = false)
		{
			for (int i = 0; i < n; ++i)
				if (i < rev[i])
					Swap(ref a[i], ref a[rev[i]]);

			for (int len = 2, k = 0; len <= n; len <<= 1, k++)
			{
				int len2 = (len >> 1);

				for (int i = 0; i < n; i += len)
				{
					var pu = i;
					var pv = i + len2;
					var pu_end = i + len2;
					for (var pw = 0; pu != pu_end; pu++, pv++, pw++)
					{
						var t = invert
							? a[pv] * new Complex(ww[k, pw].Real, -ww[k, pw].Imaginary)
							: a[pv] * ww[k, pw];
						a[pv] = a[pu] - t;
						a[pu] = a[pu] + t;
					}
				}
			}
			if (invert)
				for (int i = 0; i < n; ++i)
					a[i] = a[i] / n;
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


		void Precalc(int n)
		{
			if (n == _revCalced)
				return;
			_revCalced = n;
			for (int i = 1, j = 0; i < n; i++)
			{
				int bit = n >> 1;
				for (; j >= bit; bit >>= 1)
					j -= bit;
				j += bit;
				rev[i] = j;
			}
			for (int len = 2, k = 1; len <= n; len <<= 1, k++)
			{
				int len2 = len >> 1;
				double ang = 2 * PI / len;
				for (int i = 0; i < len2; ++i)
					ww[k - 1, i] = new Complex(Cos(ang * i), Sin(ang * i));
			}
		}

		void Multiply2(int[] a, int[] b, int[] c, int sza, int szb)
		{
			int n = 1;
			while (n < sza + szb)
				n <<= 1;

			Debug.Assert(n < M);

			Precalc(n);
			for (int i = 0; i < n; ++i)
				aa[i] = (i < sza ? a[i] : 0);
			Fft(aa, n);

			for (int i = 0; i < n; ++i)
				bb[i] = (i < szb ? b[i] : 0);
			Fft(bb, n);

			for (int i = 0; i < n; ++i)
				aa[i] = aa[i] * bb[i];

			Fft(aa, n, true);

			for (int i = 0; i < n; ++i)
			{
				c[i] = (int)(aa[i].Real + .5);
			}
		}

		public void Multiply(int[] a, int[] b, int[] c, int sza, int szb)
		{
			if (Max(sza, szb) >= 1000)
				Multiply2(a, b, c, sza, szb);
			else
				MultiplyNaive(a, b, c, sza, szb);
		}

		public void MultiplyNaive(int[] a, int[] b, int[] c, int sza, int szb)
		{
			for (int i = sza + szb - 2; i >= 0; i--)
				c[i] = 0;
			for (int i = 0; i < sza; i++)
				for (int j = 0; j < szb; j++)
					c[i + j] += a[i] * b[j];
		}


		#region Previous
#if false
		private const double TwoPi = 2 * Math.PI;

		// in:     input array
		// out:    output array
		// size:   length of the input/output {MUST BE A POWER OF 2}
		// dir:    either plus or minus one (direction of the FFT)
		// RESULT: @out[k] = \sum_{j=0}^{size - 1} in[j] * exp(dir * 2pi * i * j * k / size)
		public static void Transform(Complex[] cpxin, Complex[] cpxout, int size, int dir, int step = 1, int offsetin = 0, int offsetout = 0)
		{
			if (size <= 1)
			{
				if (size == 1) cpxout[offsetout + 0] = cpxin[offsetin + 0];
				return;
			}
			Transform(cpxin, cpxout, size >> 1, dir, step << 1, offsetin, offsetout);
			Transform(cpxin, cpxout, size >> 1, dir, step << 1, offsetin + step, offsetout + size / 2);
			var angle = dir * TwoPi / size;
			var size2 = size >> 1;
			for (var i = 0; i < size2; i++)
			{
				var even = cpxout[offsetout + i];
				var odd = cpxout[offsetout + i + size2];
				cpxout[offsetout + i] = even + Complex.Exp(angle * i) * odd;
				cpxout[offsetout + i + size2] = even + Complex.Exp(angle * (i + size2)) * odd;
			}
		}

		// Usage:
		// f[0...N-1] and g[0..N-1] are numbers
		// Want to compute the convolution h, defined by
		// h[n] = sum of f[k]g[n-k] (k = 0, ..., N-1).
		// Here, the index is cyclic; f[-1] = f[N-1], f[-2] = f[N-2], etc.
		// Let F[0...N-1] be FFT(f), and similarly, define G and H.
		// The convolution theorem says H[n] = F[n]G[n] (element-wise product).
		// To compute h[] in O(N log N) time, do the following:
		//   1. Compute F and G (pass dir = 1 as the argument).
		//   2. Get H by element-wise multiplying F and G.
		//   3. Get h by taking the inverse FFT (use dir = -1 as the argument)
		//      and *dividing by N*. DO NOT FORGET THIS SCALING FACTOR.
#endif

		#endregion


	}


}