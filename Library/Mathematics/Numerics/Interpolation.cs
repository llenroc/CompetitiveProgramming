using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Softperson.Algorithms.RangeQueries;
using static Softperson.Mathematics.ModularMath;


namespace Softperson.Mathematics.Numerics
{
	public class Interpolation
	{
		public double LagrangeInterpolation(double[][] f, int xi)
		{
			int n = f.Length;
			double result = 0;
			for (int i = 0; i < n; i++)
			{
				var term = f[i][1];
				for (int j = 0; j < n; j++)
				{
					if (j != i)
						term = term * (xi - f[j][0]) / (f[i][0] - f[j][0]);
				}

				result += term;
			}
			return result;
		}

		public long LagrangeInterpolation(long[][] f, int xi, int mod)
		{
			int n = f.Length;
			long result = 0;
			for (int i = 0; i < n; i++)
			{
				var term = f[i][1];
				for (int j = 0; j < n; j++)
				{
					if (j != i)
						term = term * Div(xi - f[j][0], f[i][0] - f[j][0], mod) % mod;
				}

				result += term;
			}
			return result % mod;
		}

		private long Div(long x, long y, long mod)
		{
			return Mult(x, ModInverse(y, mod), mod);
		}


		public class NewtonInterpolation
		{
			readonly double[] x;
			readonly double[] y;
			int n;

			public NewtonInterpolation(IList<double> xvalues, IList<double> yvalues)
			{
				n = xvalues.Count;
				x = xvalues.ToArray();
				y = yvalues.ToArray();

				for (int j = 0; j < n; j++)
					for (int i = n - 1; i > j; i--)
						y[i] = (y[i] - y[i - 1]) / (x[i] - x[i - j - 1]);
			}

			public double Interpolate(double a, int n = 0)
			{
				if (n == 0) n = this.n;

				double sum = 0;
				var factors = new double[n];
				var f = factors[0] = 1;
				for (int i = 1; i < n; i++)
					factors[i] = f *= a - x[i - 1];
				for (int i = n - 1; i >= 0; i--)
					sum += factors[i] * y[i];
				return sum;
			}
		}


		public class NewtonInterpolationMod
		{
			readonly long[] x;
			readonly long[] y;
			int n;
			private int MOD;

			public NewtonInterpolationMod(IList<long> xvalues, IList<long> yvalues, int mod)
			{
				n = xvalues.Count;
				x = xvalues.ToArray();
				y = yvalues.ToArray();
				MOD = mod;

				for (int j = 0; j < n; j++)
					for (int i = n - 1; i > j; i--)
						y[i] = Div(y[i] - y[i - 1], x[i] - x[i - j - 1]);
			}

			public unsafe long Interpolate(long a, int n = 0)
			{
				if (n == 0) n = this.n;
				long sum = 0;
				var factors = stackalloc long[n];
				var f = factors[0] = 1;
				for (int i = 1; i < n; i++)
				{
					var amx = a - x[i - 1];
					if ((ulong) amx >= (ulong) MOD) amx %= MOD;
					factors[i] = f = Mult(f, amx, MOD);
				}
				for (int i = n - 1; i >= 0; i--)
					sum += Mult(factors[i], y[i], MOD);
				return sum % MOD;
			}


			public long Div(long x, long y)
			{
				return Mult(x, ModInverse(y, MOD), MOD);
			}
		}


		public static NewtonInterpolationMod SumPolynomial(Func<long, long> f, int degree, int mod)
		{
			long[] xs = new long[degree + 1];
			long[] ys = new long[degree + 1];

			for (int x = 0; x < degree; x++)
			{
				xs[x] = x;
				ys[x] = f(x);
			}

			return new NewtonInterpolationMod(xs, ys, mod);
		}

		public static Func<long, long> SumPolynomial(Func<long, long, long> f, int degree, int mod)
		{
			long[] xs = new long[degree + 1];
			var ys = new NewtonInterpolationMod[degree + 1];

			for (int x = 0; x < degree; x++)
			{
				var xx = x;
				xs[x] = xx;
				ys[x] = SumPolynomial(y => f(xx, y), degree, mod); 
			}

			return y =>
			{
				long[] zs = new long[degree + 1];
				for (int yy = 0; yy < degree; yy++)
					zs[yy] = ys[yy].Interpolate(yy);

				var newt = new NewtonInterpolationMod(xs, zs, mod);
				return  newt.Interpolate(y);
			};
		}



	}
}
