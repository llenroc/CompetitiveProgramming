using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using static System.Array;
using static System.Math;
using static Library;
using T = System.Int64;



class Solution
{
	public void solve()
	{
		int m = Ni(), t = Ni();
        
        long [] fact = new long[100000+1];
        long [] ifact = new long[100000+1];

        long f = 1;
        fact[0]=fact[1]=1;
        for (int i=1; i<fact.Length; i++)
            fact[i] = fact[i-1] * i % MOD;
        ifact[0]=ifact[1]=1;
        for (int i=2; i<fact.Length; i++)
            ifact[i] = ModPow(fact[i], MOD-2, MOD);        
        
        var x = new long[m + 1];
        for (int i = 0; i < x.Length; i++) x[i] = ifact[i];

        var x5 = Exponentiate(x, 5, 7, MOD);
        var x10 = Exponentiate(x5, 2, 3, MOD);
        var inv = 9 * Inverse(10)%MOD;

        while (t-->0)
		{
            int n = Ni();
			WriteLine(inv * x10[n] % MOD * fact[n] % MOD );
		}

	}

	public static long[] MultiplyPolynomials(long[] a, long[] b, int size = 0)
	{
		if (size == 0) size = a.Length + b.Length - 1;
		size = Min(a.Length + b.Length - 1, size);
		var result = new long[size];
		for (int i = 0; i < a.Length; i++)
			for (int j = Min(size - i, b.Length) - 1; j >= 0; j--)
				result[i + j] += a[i] * b[j] % MOD;

		for (int i = 0; i < result.Length; i++)
			result[i] %= MOD;

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


	public static unsafe T[] KaratsubaFast(T[] a, T[] b, int size = 0, int mod = 1000000007)
	{
		var expSize = a.Length + b.Length - 1;
		if (size == 0 || expSize < size) size = expSize;
		var result = new T[size];
		fixed (T* presult = &result[0])
		fixed (T* pa = &a[0])
		fixed (T* pb = &b[0])
		{
			KaratsubaFastCore(presult, result.Length, pa, Min(size, a.Length), pb, Min(size, b.Length), mod);
			//KaratsubaFastCore(presult, result.Length, pa, Min(size, a.Length), pb, Min(size, b.Length), mod);
			for (int i = 0; i < result.Length; i++)
				//result[i] %= MOD;
				if (result[i] < 0) result[i] += mod;
		}
		return result;
	}

	private static unsafe void KaratsubaFastCore(
		T* result, int resultLen,
		T* p, int plen,
		T* q, int qlen, int mod)
	{
		if (plen < qlen)
		{
			var tp = p; p = q; q = tp;
			var t = plen; plen = qlen; qlen = t;
		}

		int n = plen;
		int j;
		if (qlen <= 35 || resultLen <= 35)
		{
			if (qlen > 0 && resultLen > 0)
				for (int i = 0; i < plen; i++)
				{
					var pi = p[i];
					var presult = &result[i];
					int end = Math.Min(qlen, resultLen - i);
					for (j = 0; j < end; j++)
						presult[j] = (presult[j] + pi * q[j]) % mod;
				}
			return;
		}

		int m = (n + 1) >> 1;

		if (qlen <= m)
		{
			KaratsubaFastCore(result, resultLen, p, Math.Min(m, plen), q, Math.Min(m, qlen), mod);
			KaratsubaFastCore(result + m, resultLen - m, p + m, plen - m, q, qlen, mod);
			return;
		}

		var tmpLength = 2 * m - 1;
		var tmp = stackalloc T[2 * m];

		// Step 1
		// NOTE: StackAlloc may be preinitialized if local-init is set true
		for (T* ptmp = tmp, pMac = tmp + tmpLength; ptmp < pMac; ptmp++) *ptmp = 0;
		KaratsubaFastCore(tmp, tmpLength, p, Math.Min(m, plen), q, Math.Min(m, qlen), mod);

		for (int i = Math.Min(resultLen, tmpLength) - 1; i >= 0; i--)
			result[i] += tmp[i];

		var ptr = result + m;
		for (int i = Math.Min(resultLen - m, tmpLength) - 1; i >= 0; i--)
			ptr[i] -= tmp[i];

        if (resultLen > m)
        {
            int crop = resultLen - m;
            // Step 2
            for (T* ptmp = tmp, pMac = tmp + tmpLength; ptmp < pMac; ptmp++) *ptmp = 0;
            KaratsubaFastCore(tmp, Math.Min(crop,tmpLength), p + m, Math.Min(crop, plen - m), q + m, Math.Min(crop, qlen - m), mod);

            ptr = result + 2 * m;
            for (int i = Math.Min(resultLen - m * 2, tmpLength) - 1; i >= 0; i--)
                ptr[i] += tmp[i];
            ptr = result + m;
            for (int i = Math.Min(crop, tmpLength) - 1; i >= 0; i--)
                ptr[i] -= tmp[i];

            // Step 3
            for (T* ptmp = tmp, pMac = tmp + tmpLength; ptmp < pMac; ptmp++) *ptmp = 0;

            var f1 = tmp;
            var f2 = tmp + m;

            for (j = Math.Min(plen, n) - 1; j >= m; j--)
                f1[j - m] += p[j];
            for (; j >= 0; j--) f1[j] = (f1[j] + p[j]) % mod;
            for (j = Math.Min(qlen, n) - 1; j >= m; j--)
                f2[j - m] += q[j];
            for (; j >= 0; j--) f2[j] = (f2[j] + q[j]) % mod;

            KaratsubaFastCore(result + m, Math.Min(2 * m - 1, crop), f1, Math.Min(crop, m), f2, Math.Min(crop, m), mod);
        }
        
		for (int i = Math.Min(plen + qlen - 1, resultLen) - 1; i >= 0; i--)
			result[i] %= mod;
	}


	#region Mod Math
	public const int MOD = 1000 * 1000 * 1000 + 7;

	static int[] _inverse;
	public static long Inverse(long n)
	{
		long result;

		if (_inverse == null)
			_inverse = new int[3000];

		if (n < _inverse.Length && (result = _inverse[n]) != 0)
			return result - 1;

		result = ModPow(n, MOD - 2);
		if (n < _inverse.Length)
			_inverse[n] = (int)(result + 1);
		return result;
	}

	public static long Mult(long left, long right)
	{
		return (left * right) % MOD;
	}

	public static long Div(long left, long divisor)
	{
		return left % divisor == 0
			? left / divisor
			: Mult(left, Inverse(divisor));
	}

	public static long Subtract(long left, long right)
	{
		return (left + (MOD - right)) % MOD;
	}

	public static long Fix(long m)
	{
		var result = m % MOD;
		if (result < 0) result += MOD;
		return result;
	}

	public static long ModPow(long n, long p, long mod = MOD)
	{
		long b = n;
		long result = 1;
		while (p != 0)
		{
			if ((p & 1) != 0)
				result = (result * b) % mod;
			p >>= 1;
			b = (b * b) % mod;
		}
		return result;
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

	#endregion

	#region Combinatorics
	static List<long> _fact;
	static List<long> _ifact;

	public static long Fact(int n)
	{
		if (_fact == null) _fact = new List<long>(100) { 1 };
		for (int i = _fact.Count; i <= n; i++)
			_fact.Add(Mult(_fact[i - 1], i));
		return _fact[n];
	}

	public static long InverseFact(int n)
	{
		if (_ifact == null) _ifact = new List<long>(100) { 1 };
		for (int i = _ifact.Count; i <= n; i++)
			_ifact.Add(Div(_ifact[i - 1], i));
		return _ifact[n];
	}

	public static long Comb(int n, int k)
	{
		if (k <= 1) return k == 1 ? n : k == 0 ? 1 : 0;
		if (k + k > n) return Comb(n, n - k);
		return Mult(Mult(Fact(n), InverseFact(k)), InverseFact(n - k));
	}

	#endregion


	// Karatsuba Multiplication
	// a = p[0,m)
	// b = q[0,m)
	// c = p[m,n)
	// d = q[m,n]

	// hi = c * d
	// lo = a * b

	// rem2 = (a+c)*(b+d)
	// (a+c)*(b+d) - ab - cd = ab + ad + bc + cd - ab - cd = ad + bc

	public static T[] Karatsuba(T[] a, T[] b, int mod = 1000000007)
	{
		var result = new T[a.Length + b.Length - 1];
		KaratsubaCore(result, a, 0, a.Length, b, 0, b.Length, mod);
		return result;
	}

	private static void KaratsubaCore(
		T[] result,
		T[] p, int ip, int plen,
		T[] q, int iq, int qlen, int mod)
	{
		int resultLen = result.Length;
		if (ip >= plen || iq >= qlen) return;

		int n = Math.Max(plen - ip, qlen - iq);
		int j;
		if (n <= 35)
		{
			if (plen < qlen)
			{
				Swap(ref p, ref q);
				var t = ip;
				ip = iq;
				iq = t;
				t = plen;
				plen = qlen;
				qlen = t;
			}

			for (int i = ip; i < plen; i++)
			{
				int ind = i - ip - iq;
				int end = Math.Min(qlen, resultLen - ind);
				for (j = iq; j < end; j++)
					result[ind + j] = (result[ind + j] + p[i] * q[j]) % mod;
			}
			return;
		}

		int m = (n + 1) >> 1;

		KaratsubaCore(result, p, ip, Math.Min(ip + m, plen), q, iq, Math.Min(iq + m, qlen), mod);

		for (int i = Math.Min(resultLen - m, m * 2 - 1) - 1; i >= 0; i--)
			result[m + i] -= result[i];

		var tmp = new T[m * 2 - 1];
		KaratsubaCore(tmp, p, ip + m, plen, q, iq + m, qlen, mod);

		for (int i = Math.Min(resultLen - m, tmp.Length) - 1; i >= 0; i--)
			result[m + i] -= tmp[i];
		for (int i = Math.Min(resultLen - m * 2, tmp.Length) - 1; i >= 0; i--)
			result[m * 2 + i] += tmp[i];

		var f1 = new T[m];
		var f2 = new T[m];

		for (j = Math.Min(plen - ip, n) - 1; j >= m; j--)
			f1[j - m] += p[j + ip];
		for (; j >= 0; j--) f1[j] = (f1[j] + p[j + ip]) % mod;
		for (j = Math.Min(qlen - iq, n) - 1; j >= m; j--)
			f2[j - m] += q[j + iq];
		for (; j >= 0; j--) f2[j] = (f2[j] + q[j + iq]) % mod;

		Array.Clear(tmp, 0, tmp.Length);
		KaratsubaCore(tmp, f1, 0, m, f2, 0, m, mod);

		for (int i = Math.Min(tmp.Length, resultLen - m) - 1; i >= 0; i--)
			result[m + i] += tmp[i];

		for (int i = Math.Min(plen + qlen - 1, resultLen) - 1; i >= 0; i--)
		{
			result[i] %= mod;
			if (result[i] < 0) result[i] += mod;
		}
	}


//	public static  int[] Primes = {1053818881, 1051721729, 1045430273, 1012924417, 1007681537, 1004535809, 998244353, 985661441, 976224257, 975175681};
//	public static  int[] PrimitiveRoots = {7, 6, 3, 5, 3, 3, 3, 3, 3, 17};
	public static  int[] Primes = {1012924417, 1004535809, 998244353, 985661441, 975175681, 962592769, 950009857, 943718401, 935329793, 924844033};
	public static  int[] PrimitiveRoots = {5, 3, 3, 3, 17, 7, 7, 7, 3, 5};

	static long[] inputed;
	static long[] saved;
	public static long[] ConvoluteSimply(long[] a, long[] b, int P, int g)
	{
		int m = Max(2, HighestOneBit(Max(a.Length, b.Length) - 1) << 2);
		long[] fa = Nttmb(a, m, false, P, g);
		long[] fb = Nttmb(b, m, false, P, g);
		for (int i = 0; i < m; i++)
			fa[i] = fa[i] * fb[i] % P;
		return Nttmb(fa, m, true, P, g);
	}

    public static int NttSize(int size)
    {
        int m = Max(2, HighestOneBit(size) << 1);
        return m;
    }

    
    public static long[] Exponentiate(long[] a, int n, int use=0, int mod = 1000000007)
	{
        if (use == 0) use = Math.Min(Primes.Length, n + 2);
        
		int m = NttSize((a.Length - 1) * n);
		long[][] fs = new long[use][];
		for (int k = 0; k < use; k++)
        {
			int P = Primes[k], g = PrimitiveRoots[k];
			long[] fa = Nttmb(a, m, false, P, g);
			for(int i = 0;i < m;i++){
                long v = fa[i];
                for (int j=1; j<n; j++)
				    fa[i] = fa[i]*v%P;
			}
			fs[k] = Nttmb(fa, m, true, P, g);
		}

        return FixUp(fs, use, mod);		
	}    
    
    public static long[] Convolute(long[] a, long[] b, int use = 2, int mod = 1000000007)
	{
		int m = Max(2, HighestOneBit(Max(a.Length, b.Length) - 1) << 2);
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
    
    static long [] FixUp(long[][] fs, int use, int mod)
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
		long dw = inverse ? ModPow(g, P - 1 - (P - 1) / n, P) : ModPow(g, (P - 1) / n, P);
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
					long Q = (long)( (ulong)((u << 32) * J) >> 32 );
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
        var log = 0;
        if ((ulong)value >= (1UL << 24))
        {
            if ((ulong)value >= (1UL << 48))
            {
                log = 48;
                value = (long)((ulong)value >> 48);
            }
            else
            {
                log = 24;
                value >>= 24;
            }
        }
        if (value >= (1 << 12))
        {
            log += 12;
            value >>= 12;
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
        return log + (int)(value >> 1 & ~value >> 2);
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

	static long ShiftRight(long v, int b)
	{
		return (long)((ulong)v >> b);
	}

	static int ShiftRight(int v, int b)
	{
		return (int)((uint)v >> b);
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


}class CaideConstants {
    public const string InputFile = null;
    public const string OutputFile = null;
}

static partial class Library
{

	#region Common

	public static void Swap<T>(ref T a, ref T b)
	{
		var tmp = a;
		a = b;
		b = tmp;
	}

	public static void Clear<T>(T[] t, T value = default(T))
	{
		for (int i = 0; i < t.Length; i++)
			t[i] = value;
	}

	public static int BinarySearch<T>(T[] array, T value, int left, int right, bool upper = false)
		where T : IComparable<T>
	{
		while (left <= right)
		{
			int mid = left + (right - left) / 2;
			int cmp = value.CompareTo(array[mid]);
			if (cmp > 0 || cmp == 0 && upper)
				left = mid + 1;
			else
				right = mid - 1;
		}
		return left;
	}

	#endregion

	#region  Input
	static System.IO.Stream inputStream;
	static int inputIndex, bytesRead;
	static byte[] inputBuffer;
	static System.Text.StringBuilder builder;
	const int MonoBufferSize = 4096;

	public static void InitInput(System.IO.Stream input = null, int stringCapacity = 16)
	{
		builder = new System.Text.StringBuilder(stringCapacity);
		inputStream = input ?? Console.OpenStandardInput();
		inputIndex = bytesRead = 0;
		inputBuffer = new byte[MonoBufferSize];
	}

	static void ReadMore()
	{
		inputIndex = 0;
		bytesRead = inputStream.Read(inputBuffer, 0, inputBuffer.Length);
		if (bytesRead <= 0) inputBuffer[0] = 32;
	}

	public static int Read()
	{
		if (inputIndex >= bytesRead) ReadMore();
		return inputBuffer[inputIndex++];
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
		if (neg) { c = Read(); }

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
		for (int i = 0, c = SkipSpaces(); i < n; i++, c = Read()) list[i] = (byte)c;
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
	#endregion

	#region Output

	static System.IO.Stream outputStream;
	static byte[] outputBuffer;
	static int outputIndex;

	public static void InitOutput(System.IO.Stream output = null)
	{
		outputStream = output ?? Console.OpenStandardOutput();
		outputIndex = 0;
		outputBuffer = new byte[65535];
		AppDomain.CurrentDomain.ProcessExit += delegate { Flush(); };
	}

	public static void WriteLine(object obj = null)
	{
		Write(obj);
		Write('\n');
	}

	public static void WriteLine(long number)
	{
		Write(number);
		Write('\n');
	}

	public static void Write(long signedNumber)
	{
		ulong number = (ulong)signedNumber;
		if (signedNumber < 0)
		{
			Write('-');
			number = (ulong)(-signedNumber);
		}

		Reserve(20 + 1); // 20 digits + 1 extra
		int left = outputIndex;
		do
		{
			outputBuffer[outputIndex++] = (byte)('0' + number % 10);
			number /= 10;
		}
		while (number > 0);

		int right = outputIndex - 1;
		while (left < right)
		{
			byte tmp = outputBuffer[left];
			outputBuffer[left++] = outputBuffer[right];
			outputBuffer[right--] = tmp;
		}
	}

	public static void Write(object obj)
	{
		if (obj == null) return;

		var s = obj.ToString();
		Reserve(s.Length);
		for (int i = 0; i < s.Length; i++)
			outputBuffer[outputIndex++] = (byte)s[i];
	}

	public static void Write(char c)
	{
		Reserve(1);
		outputBuffer[outputIndex++] = (byte)c;
	}

	public static void Write(byte[] array, int count)
	{
		Reserve(count);
		Array.Copy(array, 0, outputBuffer, outputIndex, count);
		outputIndex += count;
	}

	static void Reserve(int n)
	{
		if (outputIndex + n <= outputBuffer.Length)
			return;

		Dump();
		if (n > outputBuffer.Length)
			Array.Resize(ref outputBuffer, Math.Max(outputBuffer.Length * 2, n));
	}

	static void Dump()
	{
		outputStream.Write(outputBuffer, 0, outputIndex);
		outputIndex = 0;
	}

	public static void Flush()
	{
		Dump();
		outputStream.Flush();
	}

	#endregion

}


public class Program
{
	public static void Main(string[] args)
	{
		InitInput(Console.OpenStandardInput());
		InitOutput(Console.OpenStandardOutput());
		Solution solution = new Solution();
		solution.solve();
		Flush();
#if DEBUG
		Console.Error.WriteLine(System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime);
#endif
	}
}