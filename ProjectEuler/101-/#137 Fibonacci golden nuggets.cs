using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        var input = new StreamReader(Console.OpenStandardInput());
        var output = new StreamWriter(Console.OpenStandardOutput());
        var mod = 1000*1000*1000 + 7;
        var fib = new Calculator(mod);
        //for (int i=0; i<100; i++)
        //    Console.Error.WriteLine("{0}->{1}", i, Fib(i, mod));
        
        
        // https://oeis.org/A081018
        var t = int.Parse(input.ReadLine());
        while (t-->0)
        {
            var n = long.Parse(input.ReadLine());
            long x;
            var fib1 = FastFib(2*n, mod, out x); // fib.Fib(2*n-1);
            var fib2 = FastFib(2*n+1, mod, out x); // fib.Fib(2*n);
            //Console.Error.WriteLine($"n={n} fib1={fib1} fib2={fib2}");
            output.WriteLine(fib1*fib2 % mod);
        }
        output.Flush();
    }
    
    public static long Fib(long n, long mod)
    {
        if (n<=2)
        {
            if (n<2) return n>=0 ? n : 0;
            return 1;
        }
        
        var k = n>>1;
        var fk = Fib(k, mod);
        var fk1 = Fib(k+1, mod);
        if ((n&1)==0)
        {
            var result = 2*fk1 - fk + mod;
            while (result >= mod) result -= mod;
            result = result * fk % mod;
            return result;
        }
        else
        {
            var result = fk1*fk1 % mod + fk*fk % mod;
            if (result >= mod) result -= mod;
            return result;
        }
    }
    
    public static long FastFib(long n, long mod, out long fnp1)
    {
        if (n == 0)
        {
            fnp1 = 1;
            return 0;
        }

        long b;
        var a = FastFib(n / 2, mod, out b);
        var c = 2 * b - a;
        if (c < 0) c += mod;
        c = a * c % mod;
        var d = (a * a + b * b) % mod;
        if ((n & 1) == 0)
        {
            fnp1 = d;
            return c;
        }
        fnp1 = c + d;
        return d;
    }
    
    
    public static long Inverse(long n)
    {
        return ModPow(n, MOD - 2);
    }

    public static long Mult(long left, long right)
    {
        return (int)((left * right) % MOD);
    }

    public static long Add(long left, long right)
    {
        return ((left + right) % MOD);
    }

    public static int ModPow(long n, long p)
    {
        long b = n;
        long result = 1;
        while (p != 0)
        {
            if ((p & 1) != 0)
                result = (result * b) % MOD;
            p >>= 1;
            b = (b * b) % MOD;
        }
        return (int)result;
    }



    public const int MOD = 1000 * 1000 * 1000 + 7;

    
    public class Calculator
    {
        private Dictionary<long, long> F = new Dictionary<long, long> { { 0, 1 }, { 1, 1} };
        public int M;

        public Calculator(int mod)
        {
            M = mod;
        }

        public long Fib(long n)
        {
            if (F.Count>1000) F.Clear();
            return Calculate(n);
        }

        public long Calculate(long n)
        {
            if (n<0) return 0;
            if (F.ContainsKey(n)) return F[n];
            long k = n / 2;
            
            var ck = Calculate(k);
            var ck1 = Calculate(k-1);
            
            if (n % 2 == 0)
            { // n=2*k
                return F[n] = (ck*ck % M + ck1*ck1 % M) % M;
            }
            else
            { // n=2*k+1
                return F[n] = (ck * (ck+ck1) % M + ck1 * ck % M) % M;
            }
        }
    }
    
    
        public static long Fibo(long n)
		{
			Matrix m = FibMatrix(n);
			long f1 = 0;
			long f0 = 1;
			m.Apply(ref f1, ref f0);
			return f1;
		}
    
    	public static Matrix FibMatrix(long n)
		{
			if (n < 0)
				return new Matrix();

			return new Matrix(1, 1, 1, 0).Pow(n);
		}

		public struct Matrix
		{
			public long e11;
			public long e12;
			public long e21;
			public long e22;

			public Matrix(long m11, long m12, long m21, long m22)
			{
				e11 = m11;
				e12 = m12;
				e21 = m21;
				e22 = m22;
			}

			public static Matrix operator *(Matrix m1, Matrix m2)
			{
				Matrix m = new Matrix();
				m.e11 = Add(Mult(m1.e11, m2.e11), Mult(m1.e12, m2.e21));
				m.e12 = Add(Mult(m1.e11, m2.e12), Mult(m1.e12, m2.e22));
				m.e21 = Add(Mult(m1.e21, m2.e11), Mult(m1.e22, m2.e21));
				m.e22 = Add(Mult(m1.e21, m2.e12), Mult(m1.e22, m2.e22));
				return m;
			}

			public static Matrix operator +(Matrix m1, Matrix m2)
			{
				Matrix m = new Matrix();
				m.e11 = Add(m1.e11, m2.e11);
				m.e12 = Add(m1.e12, m2.e12);
				m.e21 = Add(m1.e21, m2.e21);
				m.e22 = Add(m1.e22, m2.e22);
				return m;
			}

			public void Apply(ref long x, ref long y)
			{
				long x2 =Mult(e11, x) + Mult(e12, y);
				long y2 = Mult(e21, x) + Mult(e22, y);
				x = x2;
				y = y2;
			}

			public Matrix Pow(long p)
			{
				Matrix b = this;
				Matrix result = new Matrix(1, 0, 0, 1);
				while (p != 0)
				{
					if ((p & 1) != 0)
						result = result* b;
					p >>= 1;
					b *= b;
				}
				return result;
			}

		}
    
}