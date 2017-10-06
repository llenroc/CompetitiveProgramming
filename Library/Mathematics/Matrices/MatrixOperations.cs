using System;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using static System.Math;
using T = System.Int64;


// MORE RESOURCES: https://github.com/sharpdx/SharpDX/blob/master/Source/SharpDX.Mathematics/Matrix3x3.cs
//				SharpDS: Mit License

namespace Softperson.Mathematics.Matrices
{
	//[PublicAPI]
	public static class MatrixOperations
	{
		const double Epsilon = 1e-9;

		#region Construction

		public static T[,] Diagonal(int n, T d = 1)
		{
			var id = new T[n, n];
			for (int i = 0; i < n; i++)
				id[i, i] = d;
			return id;
		}

		public static T[,] Clone(this T[,] m)
		{
			return (T[,]) m.Clone();
		}

		public static void Assign(this T[,] dest, T[,] src)
		{
			Array.Copy(src, dest, src.Length);
		}

		#endregion

		#region Addition and Subtraction

		public static T[,] Add(T[,] a, T[,] b, T[,] c = null)
		{
			int rows = a.GetLength(0);
			int cols = a.GetLength(1);
			if (c == null) c = new T[rows, cols];

			for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
				c[i, j] = a[i, j] + b[i, j];

			return c;
		}

		public static T[,] Subtract(T[,] a, T[,] b, T[,] c = null)
		{
			int rows = a.GetLength(0);
			int cols = a.GetLength(1);
			if (c == null) c = new T[rows, cols];

			for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
				c[i, j] = a[i, j] - b[i, j];

			return c;
		}

		public static T[,] Add(T[,] a, T[,] b, T mod, T[,] c = null)
		{
			int rows = a.GetLength(0);
			int cols = a.GetLength(1);
			if (c == null) c = new T[rows, cols];

			for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
				c[i, j] = (a[i, j] + b[i, j]) % mod;

			return c;
		}

		public static T[,] Substract(T[,] a, T[,] b, T mod, T[,] c = null)
		{
			int rows = a.GetLength(0);
			int cols = a.GetLength(1);
			if (c == null) c = new T[rows, cols];

			for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
				c[i, j] = (a[i, j] - b[i, j]) % mod;

			return c;
		}

		#endregion

		#region Scaling

		public static void Scale(T[,] a, T s, T[,] c)
		{
			int arows = a.GetLength(0);
			int bcols = a.GetLength(1);

			for (int i = 0; i < arows; i++)
			for (int j = 0; j < bcols; j++)
				c[i, j] = a[i, j] * s;
		}

		public static void Scale(T[,] a, T s, T[,] c, T p)
		{
			int arows = a.GetLength(0);
			int bcols = a.GetLength(1);

			for (int i = 0; i < arows; i++)
			for (int j = 0; j < bcols; j++)
				c[i, j] = (a[i, j] * s) % p;
		}

		#endregion

		#region Multiplication

		public static T[,] Mult(T[,] a, T[,] b, T[,] c = null)
		{
			int arows = a.GetLength(0);
			int bcols = b.GetLength(1);
			if (c == null) c = new T[arows, bcols];

			int mid = a.GetLength(1);
			for (int i = 0; i < arows; i++)
			for (int j = 0; j < bcols; j++)
			{
				c[i, j] = 0;
				for (int k = 0; k < mid; k++)
					c[i, j] = c[i, j] + a[i, k] * b[k, j];
			}

			return c;
		}

		public static T[,] Mult(T[,] a, T[,] b, T p, T[,] c = null)
		{
			int arows = a.GetLength(0);
			int bcols = b.GetLength(1);
			int mid = a.GetLength(1);
			if (c == null) c = new T[arows, bcols];

			for (int i = 0; i < arows; i++)
			for (int j = 0; j < bcols; j++)
			{
				T t = 0;
				for (int k = 0; k < mid; k++)
					t += (T) (1L * a[i, k] * b[k, j] % p);
				c[i, j] = t % p;
			}

			return c;
		}

		#endregion

		#region Sparse Multiplication

		public static T[,] MultSparse(T[,] a, T[,] b, T[,] result = null)
		{
			int m = a.GetLength(0);
			int n = a.GetLength(1);
			int p = b.GetLength(1);

			if (result == null) result = new T[m, p];
			else
			{
				for (int i = 0; i < m; i++)
				for (int j = 0; j < p; j++)
					result[i, j] = 0;
			}

			for (int i = 0; i < m; i++)
			for (int k = 0; k < n; k++)
				if (a[i, k] != 0)
					for (int j = 0; j < p; j++)
						result[i, j] += a[i, k] * b[k, j];

			return result;
		}

		public static T[,] MultSparse(T[,] a, T[,] b, T mod, T[,] result = null)
		{
			int m = a.GetLength(0);
			int n = a.GetLength(1);
			int p = b.GetLength(1);

			if (result == null) result = new T[m, p];

			for (int i = 0; i < m; i++)
			for (int k = 0; k < n; k++)
				if (a[i, k] != 0)
					for (int j = 0; j < p; j++)
					{
						var tmp = result[i, j];
						tmp += a[i, k] * b[k, j] % mod;
						if (tmp >= mod) tmp -= mod;
						result[i, j] = tmp;
					}
			return result;
		}

		#endregion

		#region Vector Multiplication

		public static void MultVector(T[,] a, T[] b, T[] c)
		{
			int n = a.GetLength(0);
			int m = a.GetLength(1);

			for (int i = 0; i < n; i++)
			{
				T t = 0;
				for (int k = 0; k < m; k++)
					t += a[i, k] * b[k];
				c[i] = t;
			}
		}

		public static T[] MultVector(T[,] a, T[] b, T p, T[] c = null)
		{
			int n = a.GetLength(0);
			int m = a.GetLength(1);

			if (c == null) c = new T[n];
			for (int i = 0; i < n; i++)
			{
				T t = 0;
				for (int k = 0; k < m; k++)
					t += (T) (1L * a[i, k] * b[k] % p);
				c[i] = t % p;
			}

			return c;
		}

		public static void MultVector(T[] a, T[,] b, T[] c, T m)
		{
			int bcols = b.GetLength(1);
			int mid = b.GetLength(1);

			for (int j = 0; j < bcols; j++)
			{
				T t = 0;
				for (int k = 0; k < mid; k++)
					t += (T) (1L * a[k] * b[k, j] % m);
				c[j] = t % m;
			}
		}

		public static void MultVector(T[] a, T[,] b, T[] c)
		{
			int bcols = b.GetLength(1);
			int mid = b.GetLength(1);

			for (int j = 0; j < bcols; j++)
			{
				T t = 0;
				for (int k = 0; k < mid; k++)
					t += (T) (1L * a[k] * b[k, j]);
				c[j] = t;
			}
		}

		public static T[,] OuterProduct(T[] v1, T[] v2, T[,] m = null)
		{
			int n = v1.Length;
			if (m == null) m = new T[n, n];
			for (int i = 0; i < n; i++)
			for (int j = 0; j < n; j++)
				m[i, j] = v1[i] * v2[j];
			return m;
		}

		#endregion

		#region Optimized Multiplication

		public static void MultX(int n, string mod = null, bool useWhile = false)
		{
			var writer = new System.CodeDom.Compiler.IndentedTextWriter(Console.Error);
			var modParam = mod == null ? "" : $" T {mod},";
			writer.WriteLine($"public static T[,] Mult{n}(T[,] a, T[,] b,{modParam} T[,] c = null)");
			writer.WriteLine("{");
			writer.Indent++;
			writer.WriteLine($"if (c==null) c = new T[{n},{n}];");
			var suf = mod != null ? "%mod" : "";
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					var cij = $"c[{i},{j}]";
					writer.Write(cij);
					for (int k = 0; k < n; k++)
					{
						var c = k == 0 ? '=' : '+';
						writer.Write($" {c} a[{i},{k}]*b[{k},{j}]{suf}");
					}
					writer.WriteLine(';');
					if (mod != null)
					{
						if (useWhile)
							writer.WriteLine($"while ({cij}>=mod) {cij} -= mod;");
						else
							writer.WriteLine($"{cij} %= {mod};");
					}
				}
			}
			writer.WriteLine($"return c;");
			writer.Indent--;
			writer.WriteLine("}\n");

			writer.WriteLine($"public static T[] Mult{n}(T[,] a, T[] b,{modParam} T[] c = null)");
			writer.WriteLine("{");
			writer.Indent++;
			writer.WriteLine($"if (c==null) c = new T[{n}];");
			for (int i = 0; i < n; i++)
			{
				var cij = $"c[{i}]";
				writer.Write(cij);
				for (int k = 0; k < n; k++)
				{
					var c = k == 0 ? '=' : '+';
					writer.Write($" {c} a[{i},{k}]*b[{k}]{suf}");
				}
				writer.WriteLine(';');
				if (mod != null)
				{
					if (useWhile)
						writer.WriteLine($"while ({cij}>=mod) {cij} -= mod;");
					else
						writer.WriteLine($"{cij} %= {mod};");
				}
			}
			writer.WriteLine($"return c;");
			writer.Indent--;
			writer.WriteLine("}\n");
			writer.Flush();
		}

		public static T[,] Mult2(T[,] a, T[,] b, T mod, T[,] c = null)
		{
			if (c == null) c = new T[2, 2];
			c[0, 0] = a[0, 0] * b[0, 0] % mod + a[0, 1] * b[1, 0] % mod;
			c[0, 0] %= mod;
			c[0, 1] = a[0, 0] * b[0, 1] % mod + a[0, 1] * b[1, 1] % mod;
			c[0, 1] %= mod;
			c[1, 0] = a[1, 0] * b[0, 0] % mod + a[1, 1] * b[1, 0] % mod;
			c[1, 0] %= mod;
			c[1, 1] = a[1, 0] * b[0, 1] % mod + a[1, 1] * b[1, 1] % mod;
			c[1, 1] %= mod;
			return c;
		}

		public static T[] Mult2(T[,] a, T[] b, T mod, T[] c = null)
		{
			if (c == null) c = new T[2];
			c[0] = a[0, 0] * b[0] % mod + a[0, 1] * b[1] % mod;
			c[0] %= mod;
			c[1] = a[1, 0] * b[0] % mod + a[1, 1] * b[1] % mod;
			c[1] %= mod;
			return c;
		}

		public static T[,] Mult3(T[,] a, T[,] b, T mod, T[,] c = null)
		{
			if (c == null) c = new T[3, 3];
			c[0, 0] = a[0, 0] * b[0, 0] % mod + a[0, 1] * b[1, 0] % mod + a[0, 2] * b[2, 0] % mod;
			c[0, 0] %= mod;
			c[0, 1] = a[0, 0] * b[0, 1] % mod + a[0, 1] * b[1, 1] % mod + a[0, 2] * b[2, 1] % mod;
			c[0, 1] %= mod;
			c[0, 2] = a[0, 0] * b[0, 2] % mod + a[0, 1] * b[1, 2] % mod + a[0, 2] * b[2, 2] % mod;
			c[0, 2] %= mod;
			c[1, 0] = a[1, 0] * b[0, 0] % mod + a[1, 1] * b[1, 0] % mod + a[1, 2] * b[2, 0] % mod;
			c[1, 0] %= mod;
			c[1, 1] = a[1, 0] * b[0, 1] % mod + a[1, 1] * b[1, 1] % mod + a[1, 2] * b[2, 1] % mod;
			c[1, 1] %= mod;
			c[1, 2] = a[1, 0] * b[0, 2] % mod + a[1, 1] * b[1, 2] % mod + a[1, 2] * b[2, 2] % mod;
			c[1, 2] %= mod;
			c[2, 0] = a[2, 0] * b[0, 0] % mod + a[2, 1] * b[1, 0] % mod + a[2, 2] * b[2, 0] % mod;
			c[2, 0] %= mod;
			c[2, 1] = a[2, 0] * b[0, 1] % mod + a[2, 1] * b[1, 1] % mod + a[2, 2] * b[2, 1] % mod;
			c[2, 1] %= mod;
			c[2, 2] = a[2, 0] * b[0, 2] % mod + a[2, 1] * b[1, 2] % mod + a[2, 2] * b[2, 2] % mod;
			c[2, 2] %= mod;
			return c;
		}

		public static T[] Mult3(T[,] a, T[] b, T mod, T[] c = null)
		{
			if (c == null) c = new T[3];
			c[0] = a[0, 0] * b[0] % mod + a[0, 1] * b[1] % mod + a[0, 2] * b[2] % mod;
			c[0] %= mod;
			c[1] = a[1, 0] * b[0] % mod + a[1, 1] * b[1] % mod + a[1, 2] * b[2] % mod;
			c[1] %= mod;
			c[2] = a[2, 0] * b[0] % mod + a[2, 1] * b[1] % mod + a[2, 2] * b[2] % mod;
			c[2] %= mod;
			return c;
		}

		public static T[,] Mult4(T[,] a, T[,] b, T mod, T[,] c = null)
		{
			if (c == null) c = new T[4, 4];
			c[0, 0] = a[0, 0] * b[0, 0] % mod + a[0, 1] * b[1, 0] % mod + a[0, 2] * b[2, 0] % mod + a[0, 3] * b[3, 0] % mod;
			c[0, 0] %= mod;
			c[0, 1] = a[0, 0] * b[0, 1] % mod + a[0, 1] * b[1, 1] % mod + a[0, 2] * b[2, 1] % mod + a[0, 3] * b[3, 1] % mod;
			c[0, 1] %= mod;
			c[0, 2] = a[0, 0] * b[0, 2] % mod + a[0, 1] * b[1, 2] % mod + a[0, 2] * b[2, 2] % mod + a[0, 3] * b[3, 2] % mod;
			c[0, 2] %= mod;
			c[0, 3] = a[0, 0] * b[0, 3] % mod + a[0, 1] * b[1, 3] % mod + a[0, 2] * b[2, 3] % mod + a[0, 3] * b[3, 3] % mod;
			c[0, 3] %= mod;
			c[1, 0] = a[1, 0] * b[0, 0] % mod + a[1, 1] * b[1, 0] % mod + a[1, 2] * b[2, 0] % mod + a[1, 3] * b[3, 0] % mod;
			c[1, 0] %= mod;
			c[1, 1] = a[1, 0] * b[0, 1] % mod + a[1, 1] * b[1, 1] % mod + a[1, 2] * b[2, 1] % mod + a[1, 3] * b[3, 1] % mod;
			c[1, 1] %= mod;
			c[1, 2] = a[1, 0] * b[0, 2] % mod + a[1, 1] * b[1, 2] % mod + a[1, 2] * b[2, 2] % mod + a[1, 3] * b[3, 2] % mod;
			c[1, 2] %= mod;
			c[1, 3] = a[1, 0] * b[0, 3] % mod + a[1, 1] * b[1, 3] % mod + a[1, 2] * b[2, 3] % mod + a[1, 3] * b[3, 3] % mod;
			c[1, 3] %= mod;
			c[2, 0] = a[2, 0] * b[0, 0] % mod + a[2, 1] * b[1, 0] % mod + a[2, 2] * b[2, 0] % mod + a[2, 3] * b[3, 0] % mod;
			c[2, 0] %= mod;
			c[2, 1] = a[2, 0] * b[0, 1] % mod + a[2, 1] * b[1, 1] % mod + a[2, 2] * b[2, 1] % mod + a[2, 3] * b[3, 1] % mod;
			c[2, 1] %= mod;
			c[2, 2] = a[2, 0] * b[0, 2] % mod + a[2, 1] * b[1, 2] % mod + a[2, 2] * b[2, 2] % mod + a[2, 3] * b[3, 2] % mod;
			c[2, 2] %= mod;
			c[2, 3] = a[2, 0] * b[0, 3] % mod + a[2, 1] * b[1, 3] % mod + a[2, 2] * b[2, 3] % mod + a[2, 3] * b[3, 3] % mod;
			c[2, 3] %= mod;
			c[3, 0] = a[3, 0] * b[0, 0] % mod + a[3, 1] * b[1, 0] % mod + a[3, 2] * b[2, 0] % mod + a[3, 3] * b[3, 0] % mod;
			c[3, 0] %= mod;
			c[3, 1] = a[3, 0] * b[0, 1] % mod + a[3, 1] * b[1, 1] % mod + a[3, 2] * b[2, 1] % mod + a[3, 3] * b[3, 1] % mod;
			c[3, 1] %= mod;
			c[3, 2] = a[3, 0] * b[0, 2] % mod + a[3, 1] * b[1, 2] % mod + a[3, 2] * b[2, 2] % mod + a[3, 3] * b[3, 2] % mod;
			c[3, 2] %= mod;
			c[3, 3] = a[3, 0] * b[0, 3] % mod + a[3, 1] * b[1, 3] % mod + a[3, 2] * b[2, 3] % mod + a[3, 3] * b[3, 3] % mod;
			c[3, 3] %= mod;
			return c;
		}

		public static T[] Mult4(T[,] a, T[] b, T mod, T[] c = null)
		{
			if (c == null) c = new T[4];
			c[0] = a[0, 0] * b[0] % mod + a[0, 1] * b[1] % mod + a[0, 2] * b[2] % mod + a[0, 3] * b[3] % mod;
			c[0] %= mod;
			c[1] = a[1, 0] * b[0] % mod + a[1, 1] * b[1] % mod + a[1, 2] * b[2] % mod + a[1, 3] * b[3] % mod;
			c[1] %= mod;
			c[2] = a[2, 0] * b[0] % mod + a[2, 1] * b[1] % mod + a[2, 2] * b[2] % mod + a[2, 3] * b[3] % mod;
			c[2] %= mod;
			c[3] = a[3, 0] * b[0] % mod + a[3, 1] * b[1] % mod + a[3, 2] * b[2] % mod + a[3, 3] * b[3] % mod;
			c[3] %= mod;
			return c;
		}

		public static T[,] Mult5(T[,] a, T[,] b, T mod, T[,] c = null)
		{
			if (c == null) c = new T[5, 5];
			c[0, 0] = a[0, 0] * b[0, 0] % mod + a[0, 1] * b[1, 0] % mod + a[0, 2] * b[2, 0] % mod + a[0, 3] * b[3, 0] % mod +
					  a[0, 4] * b[4, 0] % mod;
			c[0, 0] %= mod;
			c[0, 1] = a[0, 0] * b[0, 1] % mod + a[0, 1] * b[1, 1] % mod + a[0, 2] * b[2, 1] % mod + a[0, 3] * b[3, 1] % mod +
					  a[0, 4] * b[4, 1] % mod;
			c[0, 1] %= mod;
			c[0, 2] = a[0, 0] * b[0, 2] % mod + a[0, 1] * b[1, 2] % mod + a[0, 2] * b[2, 2] % mod + a[0, 3] * b[3, 2] % mod +
					  a[0, 4] * b[4, 2] % mod;
			c[0, 2] %= mod;
			c[0, 3] = a[0, 0] * b[0, 3] % mod + a[0, 1] * b[1, 3] % mod + a[0, 2] * b[2, 3] % mod + a[0, 3] * b[3, 3] % mod +
					  a[0, 4] * b[4, 3] % mod;
			c[0, 3] %= mod;
			c[0, 4] = a[0, 0] * b[0, 4] % mod + a[0, 1] * b[1, 4] % mod + a[0, 2] * b[2, 4] % mod + a[0, 3] * b[3, 4] % mod +
					  a[0, 4] * b[4, 4] % mod;
			c[0, 4] %= mod;
			c[1, 0] = a[1, 0] * b[0, 0] % mod + a[1, 1] * b[1, 0] % mod + a[1, 2] * b[2, 0] % mod + a[1, 3] * b[3, 0] % mod +
					  a[1, 4] * b[4, 0] % mod;
			c[1, 0] %= mod;
			c[1, 1] = a[1, 0] * b[0, 1] % mod + a[1, 1] * b[1, 1] % mod + a[1, 2] * b[2, 1] % mod + a[1, 3] * b[3, 1] % mod +
					  a[1, 4] * b[4, 1] % mod;
			c[1, 1] %= mod;
			c[1, 2] = a[1, 0] * b[0, 2] % mod + a[1, 1] * b[1, 2] % mod + a[1, 2] * b[2, 2] % mod + a[1, 3] * b[3, 2] % mod +
					  a[1, 4] * b[4, 2] % mod;
			c[1, 2] %= mod;
			c[1, 3] = a[1, 0] * b[0, 3] % mod + a[1, 1] * b[1, 3] % mod + a[1, 2] * b[2, 3] % mod + a[1, 3] * b[3, 3] % mod +
					  a[1, 4] * b[4, 3] % mod;
			c[1, 3] %= mod;
			c[1, 4] = a[1, 0] * b[0, 4] % mod + a[1, 1] * b[1, 4] % mod + a[1, 2] * b[2, 4] % mod + a[1, 3] * b[3, 4] % mod +
					  a[1, 4] * b[4, 4] % mod;
			c[1, 4] %= mod;
			c[2, 0] = a[2, 0] * b[0, 0] % mod + a[2, 1] * b[1, 0] % mod + a[2, 2] * b[2, 0] % mod + a[2, 3] * b[3, 0] % mod +
					  a[2, 4] * b[4, 0] % mod;
			c[2, 0] %= mod;
			c[2, 1] = a[2, 0] * b[0, 1] % mod + a[2, 1] * b[1, 1] % mod + a[2, 2] * b[2, 1] % mod + a[2, 3] * b[3, 1] % mod +
					  a[2, 4] * b[4, 1] % mod;
			c[2, 1] %= mod;
			c[2, 2] = a[2, 0] * b[0, 2] % mod + a[2, 1] * b[1, 2] % mod + a[2, 2] * b[2, 2] % mod + a[2, 3] * b[3, 2] % mod +
					  a[2, 4] * b[4, 2] % mod;
			c[2, 2] %= mod;
			c[2, 3] = a[2, 0] * b[0, 3] % mod + a[2, 1] * b[1, 3] % mod + a[2, 2] * b[2, 3] % mod + a[2, 3] * b[3, 3] % mod +
					  a[2, 4] * b[4, 3] % mod;
			c[2, 3] %= mod;
			c[2, 4] = a[2, 0] * b[0, 4] % mod + a[2, 1] * b[1, 4] % mod + a[2, 2] * b[2, 4] % mod + a[2, 3] * b[3, 4] % mod +
					  a[2, 4] * b[4, 4] % mod;
			c[2, 4] %= mod;
			c[3, 0] = a[3, 0] * b[0, 0] % mod + a[3, 1] * b[1, 0] % mod + a[3, 2] * b[2, 0] % mod + a[3, 3] * b[3, 0] % mod +
					  a[3, 4] * b[4, 0] % mod;
			c[3, 0] %= mod;
			c[3, 1] = a[3, 0] * b[0, 1] % mod + a[3, 1] * b[1, 1] % mod + a[3, 2] * b[2, 1] % mod + a[3, 3] * b[3, 1] % mod +
					  a[3, 4] * b[4, 1] % mod;
			c[3, 1] %= mod;
			c[3, 2] = a[3, 0] * b[0, 2] % mod + a[3, 1] * b[1, 2] % mod + a[3, 2] * b[2, 2] % mod + a[3, 3] * b[3, 2] % mod +
					  a[3, 4] * b[4, 2] % mod;
			c[3, 2] %= mod;
			c[3, 3] = a[3, 0] * b[0, 3] % mod + a[3, 1] * b[1, 3] % mod + a[3, 2] * b[2, 3] % mod + a[3, 3] * b[3, 3] % mod +
					  a[3, 4] * b[4, 3] % mod;
			c[3, 3] %= mod;
			c[3, 4] = a[3, 0] * b[0, 4] % mod + a[3, 1] * b[1, 4] % mod + a[3, 2] * b[2, 4] % mod + a[3, 3] * b[3, 4] % mod +
					  a[3, 4] * b[4, 4] % mod;
			c[3, 4] %= mod;
			c[4, 0] = a[4, 0] * b[0, 0] % mod + a[4, 1] * b[1, 0] % mod + a[4, 2] * b[2, 0] % mod + a[4, 3] * b[3, 0] % mod +
					  a[4, 4] * b[4, 0] % mod;
			c[4, 0] %= mod;
			c[4, 1] = a[4, 0] * b[0, 1] % mod + a[4, 1] * b[1, 1] % mod + a[4, 2] * b[2, 1] % mod + a[4, 3] * b[3, 1] % mod +
					  a[4, 4] * b[4, 1] % mod;
			c[4, 1] %= mod;
			c[4, 2] = a[4, 0] * b[0, 2] % mod + a[4, 1] * b[1, 2] % mod + a[4, 2] * b[2, 2] % mod + a[4, 3] * b[3, 2] % mod +
					  a[4, 4] * b[4, 2] % mod;
			c[4, 2] %= mod;
			c[4, 3] = a[4, 0] * b[0, 3] % mod + a[4, 1] * b[1, 3] % mod + a[4, 2] * b[2, 3] % mod + a[4, 3] * b[3, 3] % mod +
					  a[4, 4] * b[4, 3] % mod;
			c[4, 3] %= mod;
			c[4, 4] = a[4, 0] * b[0, 4] % mod + a[4, 1] * b[1, 4] % mod + a[4, 2] * b[2, 4] % mod + a[4, 3] * b[3, 4] % mod +
					  a[4, 4] * b[4, 4] % mod;
			c[4, 4] %= mod;
			return c;
		}

		public static T[] Mult5(T[,] a, T[] b, T mod, T[] c = null)
		{
			if (c == null) c = new T[5];
			c[0] = a[0, 0] * b[0] % mod + a[0, 1] * b[1] % mod + a[0, 2] * b[2] % mod + a[0, 3] * b[3] % mod +
				   a[0, 4] * b[4] % mod;
			c[0] %= mod;
			c[1] = a[1, 0] * b[0] % mod + a[1, 1] * b[1] % mod + a[1, 2] * b[2] % mod + a[1, 3] * b[3] % mod +
				   a[1, 4] * b[4] % mod;
			c[1] %= mod;
			c[2] = a[2, 0] * b[0] % mod + a[2, 1] * b[1] % mod + a[2, 2] * b[2] % mod + a[2, 3] * b[3] % mod +
				   a[2, 4] * b[4] % mod;
			c[2] %= mod;
			c[3] = a[3, 0] * b[0] % mod + a[3, 1] * b[1] % mod + a[3, 2] * b[2] % mod + a[3, 3] * b[3] % mod +
				   a[3, 4] * b[4] % mod;
			c[3] %= mod;
			c[4] = a[4, 0] * b[0] % mod + a[4, 1] * b[1] % mod + a[4, 2] * b[2] % mod + a[4, 3] * b[3] % mod +
				   a[4, 4] * b[4] % mod;
			c[4] %= mod;
			return c;
		}

		#endregion

		#region Pow

		public static T[,] Pow(this T[,] a, int p, int mod)
		{
			int n = a.GetLength(0);
			var tmp = new T[n, n];
			var result = Diagonal(n);
			var b = Clone(a);

			while (p > 0)
			{
				if ((p & 1) != 0)
				{
					Mult(result, b, mod, tmp);
					Assign(result, tmp);
				}
				p >>= 1;
				Mult(b, b, mod, tmp);
				Assign(b, tmp);
			}
			return result;
		}

		public static T[,] Pow(this T[,] a, int p)
		{
			int n = a.GetLength(0);
			var tmp = new T[n, n];
			var result = Diagonal(n);
			var b = Clone(a);

			while (p > 0)
			{
				if ((p & 1) != 0)
				{
					Mult(result, b, tmp);
					Assign(result, tmp);
				}
				p >>= 1;
				Mult(b, b, tmp);
				Assign(b, tmp);
			}
			return result;
		}

		#endregion

		#region Transpose

		public static T[,] Transpose(this T[,] a, T[,] result = null)
		{
			int n = a.GetLength(0);
			int m = a.GetLength(1);
			if (result == null) result = new T[m, n];
			for (int i = 0; i < n; i++)
			for (int j = i + 1; j < m; j++)
			{
				// Supports inplace transpose
				T tmp = a[i, j];
				a[i, j] = a[j, i];
				a[j, i] = tmp;
			}
			return result;
		}

		#endregion

		#region Inverses

		public static T Determinant2x2(T[,] m)
		{
			return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
		}

		// SOURCE: https://stackoverflow.com/questions/983999/simple-3x3-matrix-inverse-code-c

		public static T Determinant3x3(T[,] m)
		{
			return m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) -
				   m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) +
				   m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
		}

		public static T[,] Inverse3x3(T[,] m)
		{
			var invdet = 1 / Determinant3x3(m);
			return new[,]
			{
				{
					(m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) * invdet,
					(m[0, 2] * m[2, 1] - m[0, 1] * m[2, 2]) * invdet,
					(m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1]) * invdet
				},
				{
					(m[1, 2] * m[2, 0] - m[1, 0] * m[2, 2]) * invdet,
					(m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0]) * invdet,
					(m[1, 0] * m[0, 2] - m[0, 0] * m[1, 2]) * invdet
				},
				{
					(m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]) * invdet,
					(m[2, 0] * m[0, 1] - m[0, 0] * m[2, 1]) * invdet,
					(m[0, 0] * m[1, 1] - m[1, 0] * m[0, 1]) * invdet
				}
			};
		}



		// SOURCE: http://www.geeksforgeeks.org/adjoint-inverse-matrix/

		static void GetCofactor(T[,] a, T[,] temp, int p, int q, int n)
		{
			int i = 0, j = 0;

			for (int row = 0; row < n; row++)
			for (int col = 0; col < n; col++)
			{
				if (row != p && col != q)
				{
					temp[i, j++] = a[row, col];
					if (j == n - 1)
					{
						j = 0;
						i++;
					}
				}
			}
		}

		public static T Determinant(T[,] a, int n)
		{
			if (n == 1)
				return a[0, 0];

			T d = 0;
			var temp = new T[n, n];
			int sign = 1;
			for (int f = 0; f < n; f++)
			{
				GetCofactor(a, temp, 0, f, n);
				d += sign * a[0, f] * Determinant(temp, n - 1);
				sign = -sign;
			}

			return d;
		}

		public static void Adjoint(T[,] a, T[,] adj)
		{
			int n = a.GetLength(0);
			if (n == 1)
			{
				adj[0, 0] = 1;
				return;
			}

			var temp = new T[n, n];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					GetCofactor(a, temp, i, j, n);
					var sign = ((i + j) % 2 == 0) ? 1 : -1;
					adj[j, i] = (sign) * (Determinant(temp, n - 1));
				}
			}
		}

		public static bool Inverse(T[,] a, T[,] inverse)
		{
			int n = a.GetLength(0);
			var det = Determinant(a, n);
			if (det == 0)
				return false;

			var adj = new T[n, n];
			Adjoint(a, adj);

			for (int i = 0; i < n; i++)
			for (int j = 0; j < n; j++)
				inverse[i, j] = adj[i, j] / det;

			return true;
		}

		public static T[,] Inverse(T[,] a)
		{
			// Laplace Expansion O(n!)
			int n = a.GetLength(0);
			var result = new T[n, n];
			return Inverse(a, result) ? result : null;
		}

		// https://chi3x10.wordpress.com/2008/05/28/calculate-matrix-inversion-in-c/

		public static T[,] MatrixInversion(T[,] mat)
		{
			// Laplace Expansion O(n!)
			int n = mat.GetLength(0);
			var inverse = new T[n, n];
			T invdet = 1 / CalcDeterminantByPivotalCondensation(mat);
			var minor = new T[n - 1, n - 1];
			for (int j = 0; j < n; j++)
			for (int i = 0; i < n; i++)
			{
				GetMinor(mat, minor, j, i);
				inverse[i, j] = invdet * CalcDeterminantByPivotalCondensation(minor);
				if (((i + j) & 1) == 1)
					inverse[i, j] = -inverse[i, j];
			}
			return inverse;
		}

		static void GetMinor(T[,] src, T[,] dest, int row, int col)
		{
			// O(n^2) time
			int n = src.GetLength(0);
			int rowCount = 0;
			for (int i = 0; i < n; i++)
				if (i != row)
				{
					var colCount = 0;
					for (int j = 0; j < n; j++)
						if (j != col)
						{
							dest[rowCount, colCount] = src[i, j];
							colCount++;
						}
					rowCount++;
				}
		}

		/*
		// Exponential
		static T CalcDeterminant(T[,] mat)
		{
			// T(n) = 
			int n = mat.GetLength(0);
			if (n == 1)
				return mat[0, 0];

			T det = 0;
			var minor = new T[n - 1, n - 1];
			for (int i = 0; i < n; i++)
			{
				GetMinor(mat, minor, 0, i);
				det += (i % 2 == 1 ? -1 : 1)
					* mat[0, i]
					* CalcDeterminant(minor);
			}
			return det;
		}*/

		// http://mathworld.wolfram.com/ChioPivotalCondensation.html
		public static T CalcDeterminantByPivotalCondensation(T[,] matrix)
		{
			// O(n^3) time
			// O(n^2) space
			int n = matrix.GetLength(0);
			var m = Clone(matrix);
			T div = 1;
			for (int k = n; k >= 2; k--)
			{
				int c = n - k + 1;
				int f = c - 1;
				T mff = m[f, f];
				for (int i = k - 2; i >= 0; i--)
				for (int j = k - 2; j >= 0; j--)
					m[c + i, c + j] = mff * m[c + i, c + j] - m[f, c + j] * m[c + i, f];

				for (int i = 0; i < k - 2; i++)
					div *= mff;
			}
			return m[n - 1, n - 1] / div;
		}

		// https://en.wikipedia.org/wiki/LU_decomposition
		// More code there with permutation matrix

		public static T[,] LUDecompose(T[,] matrix)
		{
			int n = matrix.GetLength(0);
			T[,] lu = new T[n, n];
			T sum = 0;
			for (int i = 0; i < n; i++)
			{
				for (int j = i; j < n; j++)
				{
					sum = 0;
					for (int k = 0; k < i; k++)
						sum += lu[i, k] * lu[k, j];
					lu[i, j] = matrix[i, j] - sum;
				}
				for (int j = i + 1; j < n; j++)
				{
					sum = 0;
					for (int k = 0; k < i; k++)
						sum += lu[j, k] * lu[k, i];
					lu[j, i] = (1 / lu[i, i]) * (matrix[j, i] - sum);
				}
			}
			return lu;
		}

		public static T[] LUSolve(T[,] lu, T[] b)
		{
			// find solution of Ly = b
			int n = lu.GetLength(0);
			T sum;
			var y = new T[n];
			for (int i = 0; i < n; i++)
			{
				sum = 0;
				for (int k = 0; k < i; k++)
					sum += lu[i, k] * y[k];
				y[i] = b[i] - sum;
			}
			// find solution of Ux = y
			var x = new T[n];
			for (int i = n - 1; i >= 0; i--)
			{
				sum = 0;
				for (int k = i + 1; k < n; k++)
					sum += lu[i, k] * x[k];
				x[i] = (1 / lu[i, i]) * (y[i] - sum);
			}
			return x;
		}

		public static T[] Solve(T[,] matrix, T[] b)
		{
			var lu = LUDecompose(matrix);
			return LUSolve(lu, b);
		}


		// SOURCE: https://github.com/genekogan/ofxKinectProjectorToolkit/blob/master/calibration/bin/CALIBRATIONDebug.app/Contents/Frameworks/GLUT.framework/Versions/A/Headers/vvector.h

		/// <summary> determinant of matrix
		/// Computes determinant of matrix m, returning d
		/// </summary>

		public static T Determinant_2X2(T[,] m)
		{
			return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
		}

		/// <summary> determinant of matrix
		/// Computes determinant of matrix m, returning d
		/// </summary>

		public static T Determinant_3X3(T[,] m)
		{
			var d = m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]);
			d -= m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]);
			d += m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
			return d;
		}

		/// <summary> i,j,th cofactor of a 4x4 matrix
		/// </summary>

		public static T Cofactor_4X4Ij(T[,] m, int i, int j)
		{
			T[] ii = new T[4];
			T[] jj = new T[4];
			int k;

			/* compute which row, columnt to skip */
			for (k = 0; k < i; k++) ii[k] = k;
			for (k = i; k < 3; k++) ii[k] = k + 1;
			for (k = 0; k < j; k++) jj[k] = k;
			for (k = j; k < 3; k++) jj[k] = k + 1;

			var fac = m[ii[0], jj[0]] * (m[ii[1], jj[1]] * m[ii[2], jj[2]]
										 - m[ii[1], jj[2]] * m[ii[2], jj[1]]);
			fac -= m[ii[0], jj[1]] * (m[ii[1], jj[0]] * m[ii[2], jj[2]]
									  - m[ii[1], jj[2]] * m[ii[2], jj[0]]);
			fac += m[ii[0], jj[2]] * (m[ii[1], jj[0]] * m[ii[2], jj[1]]
									  - m[ii[1], jj[1]] * m[ii[2], jj[0]]);

			/* compute sign */
			k = i + j;
			if (k != (k / 2) * 2)
				fac = -fac;
			return fac;
		}

		/// <summary> determinant of matrix
		/// Computes determinant of matrix m, returning d
		/// </summary>

		public static T Determinant_4X4(T[,] m)
		{
			var d = m[0, 0] * Cofactor_4X4Ij(m, 0, 0);
			d += m[0, 1] * Cofactor_4X4Ij(m, 0, 1);
			d += m[0, 2] * Cofactor_4X4Ij(m, 0, 2);
			d += m[0, 3] * Cofactor_4X4Ij(m, 0, 3);
			return d;
		}

		/// <summary> cofactor of matrix
		/// Computes cofactor of matrix m, returning a
		/// </summary>

		public static void Cofactor_2X2(T[,] a, T[,] m)
		{
			a[0, 0] = m[1, 1];
			a[0, 1] = -m[1, 0];
			a[1, 0] = -m[0, 1];
			a[1, 1] = m[0, 0];
		}

		/// <summary> cofactor of matrix
		/// Computes cofactor of matrix m, returning a
		/// </summary>

		public static void Cofactor_3X3(T[,] a, T[,] m)
		{
			a[0, 0] = m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1];
			a[0, 1] = -(m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]);
			a[0, 2] = m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0];
			a[1, 0] = -(m[0, 1] * m[2, 2] - m[0, 2] * m[2, 1]);
			a[1, 1] = m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0];
			a[1, 2] = -(m[0, 0] * m[2, 1] - m[0, 1] * m[2, 0]);
			a[2, 0] = m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1];
			a[2, 1] = -(m[0, 0] * m[1, 2] - m[0, 2] * m[1, 0]);
			a[2, 2] = m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
		}


		/// <summary> cofactor of matrix
		/// Computes cofactor of matrix m, returning a
		/// </summary>

		public static void Cofactor_4X4(T[,] a, T[,] m)
		{
			int i, j;

			for (i = 0; i < 4; i++)
			for (j = 0; j < 4; j++)
				a[i, j] = Cofactor_4X4Ij(m, i, j);
		}


		/// <summary> adjoint of matrix
		///
		/// Computes adjoint of matrix m, returning a
		/// (Note that adjoint is just the transpose of the cofactor matrix)
		/// </summary>

		public static void Adjoint_2X2(T[,] a, T[,] m)
		{
			a[0, 0] = (m)[1, 1];
			a[1, 0] = -(m)[1, 0];
			a[0, 1] = -(m)[0, 1];
			a[1, 1] = (m)[0, 0];
		}


		/// <summary> adjoint of matrix
		/// Computes adjoint of matrix m, returning a
		/// (Note that adjoint is just the transpose of the cofactor matrix)
		/// </summary>

		public static void Adjoint_3X3(T[,] a, T[,] m)
		{
			a[0, 0] = m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1];
			a[1, 0] = -(m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]);
			a[2, 0] = m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0];
			a[0, 1] = -(m[0, 1] * m[2, 2] - m[0, 2] * m[2, 1]);
			a[1, 1] = m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0];
			a[2, 1] = -(m[0, 0] * m[2, 1] - m[0, 1] * m[2, 0]);
			a[0, 2] = m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1];
			a[1, 2] = -(m[0, 0] * m[1, 2] - m[0, 2] * m[1, 0]);
			a[2, 2] = m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
		}


		/// <summary> adjoint of matrix
		///
		/// Computes adjoint of matrix m, returning a
		/// (Note that adjoint is just the transpose of the cofactor matrix)
		/// </summary>

		public static void Adjoint_4X4(T[,] a, T[,] m)
		{
			int i, j;
			for (i = 0; i < 4; i++)
			for (j = 0; j < 4; j++)
				a[j, i] = Cofactor_4X4Ij(m, i, j);
		}


		/// <summary> compute adjoint of matrix and scale
		/// Computes adjoint of matrix m, scales it by s, returning a
		/// </summary>

		public static void ScaleAdjoint_2X2(T[,] a, T s, T[,] m)
		{
			a[0, 0] = s * m[1, 1];
			a[1, 0] = -s * m[1, 0];
			a[0, 1] = -s * m[0, 1];
			a[1, 1] = s * m[0, 0];
		}


		/// <summary> compute adjoint of matrix and scale
		/// Computes adjoint of matrix m, scales it by s, returning a
		/// </summary>

		public static void ScaleAdjoint_3X3(T[,] a, T s, T[,] m)
		{
			a[0, 0] = s * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]);
			a[1, 0] = s * (m[1, 2] * m[2, 0] - m[1, 0] * m[2, 2]);
			a[2, 0] = s * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);

			a[0, 1] = s * (m[0, 2] * m[2, 1] - m[0, 1] * m[2, 2]);
			a[1, 1] = s * (m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0]);
			a[2, 1] = s * (m[0, 1] * m[2, 0] - m[0, 0] * m[2, 1]);

			a[0, 2] = s * (m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1]);
			a[1, 2] = s * (m[0, 2] * m[1, 0] - m[0, 0] * m[1, 2]);
			a[2, 2] = s * (m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0]);
		}


		/// <summary> compute adjoint of matrix and scale
		///
		/// Computes adjoint of matrix m, scales it by s, returning a
		/// </summary>

		public static void ScaleAdjoint_4X4(T[,] a, T s, T[,] m)
		{
			int i, j;

			for (i = 0; i < 4; i++)
			for (j = 0; j < 4; j++)
				a[j, i] = Cofactor_4X4Ij(m, i, j) * s;
		}

		/// <summary> inverse of matrix 
		///
		/// Compute inverse of matrix a, returning determinant m and 
		/// inverse b
		/// </summary>

		public static void Invert_2X2(T[,] b, T[,] a)
		{
			var tmp = 1 / Determinant_2X2(a);
			ScaleAdjoint_2X2(b, tmp, a);
		}

		/// <summary> inverse of matrix 
		///
		/// Compute inverse of matrix a, returning determinant m and 
		/// inverse b
		/// </summary>

		public static void Invert_3X3(T[,] b, T[,] a)
		{
			var tmp = 1 / Determinant_3X3(a);
			ScaleAdjoint_3X3(b, tmp, a);
		}

		/// <summary> inverse of matrix 
		///
		/// Compute inverse of matrix a, returning determinant m and 
		/// inverse b
		/// </summary>

		public static void Invert_4X4(T[,] b, T[,] a)
		{
			var tmp = 1 / Determinant_4X4(a);
			ScaleAdjoint_4X4(b, tmp, a);
		}

		#endregion

		#region Vector Arithmetic

		public static T L1Norm(T[] a)
		{
			T result = 0;
			foreach (var v in a)
				result += Abs(v);
			return result;
		}

		public static double L2Norm(T[] a)
		{
			T result = 0;
			foreach (var v in a)
				result += v * v;
			return Sqrt(result);
		}

		public static T DotProduct(T[] a, T[] b)
		{
			T result = 0;
			for (int i = 0; i < a.Length; i++)
				result += a[i] * b[i];
			return result;
		}

		public static void NormalizeX(T[] v)
		{
			var d = L2Norm(v);
			if (d != 0 && d != 1)
			{
				for (int i = 0; i < v.Length; i++)
					v[i] = (T) (v[i] / d);
			}
		}

		public static T[] Blend(T[] a, T[] b, T fa, T fb, T[] c = null)
		{
			int n = a.Length;
			if (c == null) c = new T[n];
			for (int i = 0; i < n; i++)
				c[i] = fa * a[i] + fb * b[i];
			return c;
		}

		public static T[] CrossProduct(T[] a, T[] b, T[] c = null)
		{
			if (c == null) c = new T[3];
			c[0] = a[1] * b[2] - a[2] * b[1];
			c[1] = a[2] * b[0] - a[0] * b[2];
			c[2] = a[0] * b[1] - a[1] * b[0];
			return c;
		}

		/// <summary>
		/// Vector perp -- assumes that n is of unit length 
		/// accepts vector v, subtracts out any component parallel to n 
		/// </summary>
		/// <param name="vp"></param>
		/// <param name="v"></param>
		/// <param name="n"></param>

		public static void Perpendicular(T[] vp, T[] v, T[] n)
		{
			var vdot = DotProduct(v, n);
			vp[0] = v[0] - vdot * n[0];
			vp[1] = v[1] - vdot * n[1];
			vp[2] = v[2] - vdot * n[2];
		}

		/// <summary>
		/// Vector parallel -- assumes that n is of unit length
		/// accepts vector v, subtracts out any component perpendicular to n*/
		/// </summary>

		public static void Parallel(T[] vp, T[] v, T[] n)
		{
			var vdot = DotProduct(v, n);
			vp[0] = vdot * n[0];
			vp[1] = vdot * n[1];
			vp[2] = vdot * n[2];
		}

		public static void Reflect(T[] vr, T[] v, T[] n)
		{
			var vdot = DotProduct(v, n);
			vr[0] = v[0] - 2 * vdot * n[0];
			vr[1] = v[1] - 2 * vdot * n[1];
			vr[2] = v[2] - 2 * vdot * n[2];
		}

		#endregion

		#region Misc

		public static string Text(T[,] mat)
		{
			StringBuilder sb = new StringBuilder();
			if (mat == null)
				return null;

			int n = mat.GetLength(0);
			int m = mat.GetLength(1);

			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					if (j > 0) sb.Append(' ');
					sb.Append(mat[i, j]);
				}
				sb.AppendLine();
			}

			return sb.ToString();
		}

		public static string Text(T[] m)
		{
			return "[" + string.Join(", ", m) + "]";
		}

		public static bool Equals(T[,] a, T[,] b)
		{
			if (a.GetLength(0) != b.GetLength(0)
				|| a.GetLength(1) != b.GetLength(1))
				return false;

			int n = a.GetLength(0);
			int m = a.GetLength(1);
			for (int i = 0; i < n; i++)
			for (int j = 0; j < m; j++)
				if (a[i, j] != b[i, j]) return false;
			return true;
		}

		public static bool NearEquals(T[,] a, T[,] b)
		{
			if (a.GetLength(0) != b.GetLength(0)
				|| a.GetLength(1) != b.GetLength(1))
				return false;

			int n = a.GetLength(0);
			int m = a.GetLength(1);
			for (int i = 0; i < n; i++)
			for (int j = 0; j < m; j++)
				if (!NearEquals(a[i, j], b[i, j])) return false;
			return true;
		}

		public static bool Equals(T[] a, T[] b)
		{
			if (a.Length != b.Length)
				return false;

			for (int i = 0; i < a.Length; i++)
				if (a[i] != b[i]) return false;
			return true;
		}

		public static bool NearEquals(T[] a, T[] b)
		{
			if (a.Length != b.Length)
				return false;

			for (int i = 0; i < a.Length; i++)
				if (!NearEquals(a[i], b[i])) return false;
			return true;
		}

		public static bool NearEquals(T a, T b)
		{
			return Abs(a - b) < Epsilon;
		}

		public static unsafe int GetHashCode(T[,] m)
		{
			unchecked
			{
				var hashCode = 0;
				fixed (T* p = &m[0, 0])
				{
					for (int i = 0; i < m.Length; i++)
						hashCode = (hashCode * 397) ^ p[i].GetHashCode();
				}
				return hashCode;
			}
		}

		public static int GetHashCode(int[] m)
		{
			unchecked
			{
				var hashCode = 0;
				for (int i = 0; i < m.Length; i++)
					hashCode = (hashCode * 397) ^ m[i].GetHashCode();
				return hashCode;
			}
		}

		public static unsafe T[,] Reinterpret(T[,] mat, int n, int m)
		{
			if (n * m != mat.Length) throw new InvalidOperationException();

			T[,] result = new T[n, m];
			fixed (T* psrc = &mat[0, 0])
			fixed (T* pdest = &result[0, 0])
			{
				for (int i = 0; i < result.Length; i++)
					pdest[i] = psrc[i];
			}
			return result;
		}

		public static unsafe T[,] Reinterpret(T[] mat, int n, int m)
		{
			if (n * m != mat.Length) throw new InvalidOperationException();

			T[,] result = new T[n, m];
			fixed (T* presult = &result[0, 0])
			{
				for (int i = 0; i < result.Length; i++)
					presult[i] = mat[i];
			}
			return result;
		}

		public static unsafe T[] Flatten(T[,] mat)
		{
			T[] result = new T[mat.Length];
			fixed (T* pstart = &mat[0, 0])
			{
				for (int i = 0; i < mat.Length; i++)
					result[i] = pstart[i];
			}
			return result;
		}

		public static T[,] Submatrix(T[,] mat, int x, int y, int n, int m)
		{
			var result = new T[n, m];
			for (int i = 0; i < n; i++)
			for (int j = 0; j < m; j++)
				result[i, j] = mat[i + x, j + y];
			return result;
		}


		/// <summary>
		/// Produces a right matrix for transforming row vectors (points) 
		/// using the same affine transformation that tranforms src points to dest points
		/// For n dimensions, n+1 points required or (use cross product to get n+1 point)
		/// </summary>
		/// <param name="srcPoints">The source points.</param>
		/// <param name="destPoints">The dest points.</param>
		/// <returns></returns>
		public static T[,] MapPoints(T[][] srcPoints, T[][] destPoints)
		{
			int dim = srcPoints[0].Length;
			int n = dim + 1;

			if (srcPoints.Length != n || destPoints.Length != n)
				throw new InvalidOperationException();

			var matrix = new T[n, n];
			Array.Copy(srcPoints, matrix, srcPoints.Length);
			for (int i = 0; i < n; i++)
				matrix[i, n - 1] = 1;

			var matrix2 = new T[n, n - 1];
			Array.Copy(destPoints, matrix2, destPoints.Length);
			for (int i = 0; i < n; i++)
				matrix2[i, n - 1] = 1;

			var inverse = Inverse(matrix);
			var result = Mult(inverse, matrix2);
			return result;
		}

		public static void Copy(T[][] src, T[,] dest)
		{
			for (int i = 0; i < src.Length; i++)
			{
				var row = src[i];
				for (int j = 0; j < row.Length; j++)
					dest[i, j] = row[j];
			}
		}

		#endregion

		#region Recurrences

		public static T[,] RecurrenceMatrix(T[] coefficients)
		{
			int n = coefficients.Length;
			var result = new T[n, n];
			result[0, 0] = coefficients[0];
			for (int i = 1; i < n; i++)
			{
				result[0, i] = coefficients[i];
				result[i, i - 1] = 1;
			}
			return result;
		}


		public static T[,] RecurrenceMatrixWithConstant(T[] coefficients)
		{
			int n = coefficients.Length;
			var result = new T[n + 1, n + 1];
			result[0, n] = result[n, n] = 1;
			result[0, 0] = coefficients[0];
			for (int i = 1; i < n; i++)
			{
				result[0, i] = coefficients[i];
				result[i, i - 1] = 1;
			}
			return result;
		}

		public static T[,] CombineMatrices(T[,] a, T[,] b)
		{
			int am = a.GetLength(0);
			int an = a.GetLength(1);
			int bm = b.GetLength(0);
			int bn = b.GetLength(1);

			var result = new T[am + bm, bn + bm];

			for (int i = 0; i < am; i++)
			for (int j = 0; j < an; j++)
				result[i, j] = a[i, j];

			for (int i = 0; i < bm; i++)
			for (int j = 0; j < bn; j++)
				result[am + i, an + j] = b[i, j];

			return result;
		}

		#endregion

		#region Fast Matrix Pow

		public struct FastMatrixPow
		{
			private const int shift = 4;
			private const long mask = (1L << shift) - 1;

			readonly T[][][,] _cache;
			readonly long _mod;
			readonly T[,] _tmp;
			readonly int _n;

			public FastMatrixPow(T[,] a, long mod)
			{
				_n = a.GetLength(0);
				_cache = new T[64 / shift][][,];
				_mod = mod;
				_tmp = new T[_n, _n];

				for (int j = 0; j < _cache.Length; j++)
				{
					var t = _cache[j] = new T[mask + 1][,];
					t[1] = j == 0 ? Clone(a) : Mult(_cache[j - 1][1], _cache[j - 1][mask], mod);
					for (int i = 2; i < t.Length; i++)
						t[i] = Mult(t[1], t[i - 1], mod);
				}
			}

			public T[,] Pow(long p, T[,] buffer = null)
			{
				var result = buffer ?? new T[_n, _n];

				if (p == 0)
				{
					if (buffer != null) Array.Clear(buffer, 0, buffer.Length);
					for (int i = result.GetLength(0) - 1; i >= 0; i--)
						result[i, i] = 1;
					return result;
				}

				var asst = 0;
				int bit = 0;
				while (p > 0)
				{
					if ((p & mask) != 0)
					{
						Assign(result, asst++ > 0
							? Mult(result, _cache[bit][p & mask], _mod, _tmp)
							: _cache[bit][p & mask]);
					}
					p >>= shift;
					bit++;
				}
				return result;
			}


			#endregion
		}
	}
}