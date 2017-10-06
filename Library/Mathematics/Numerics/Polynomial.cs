#region Using
/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2005, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Softperson.Collections;

#endregion

namespace Softperson.Mathematics
{
	public struct Polynomial
	{
		#region Fields

		public double[] poly;

		#endregion

		#region Constructors
		public Polynomial(params double[] polynomials)
		{
			poly = polynomials;

			int newLength = poly.Length;
			while (newLength > 0 && poly[newLength - 1] == 0)
				newLength--;

			if (poly.Length != newLength)
				poly = ListTools.CopyRange(poly, 0, newLength);
			else if (poly.Length == 0)
				poly = ArrayTools.EmptyArray<double>();
		}
		#endregion

		#region Properties

		public static readonly Polynomial Zero = new Polynomial();

		public static readonly Polynomial One = new Polynomial(1);

		public int Order
		{
			get { return Length - 1; }
		}

		public int Length
		{
			get { return poly==null ? 0 : poly.Length; }
		}

		public double this[int i]
		{
			get { return i<poly.Length ? poly[i] : 0; }
		}

		#endregion

		#region Methods


		public Polynomial Derivative()
		{
			if (poly.Length == 0)
				return this;

			double[] newPoly = new double[poly.Length - 1];
			for (int i = 0; i < newPoly.Length; i++)
				newPoly[i] = poly[i + 1] * (i + 1);
			return new Polynomial(newPoly);
		}

		public Polynomial Integrate()
		{
			if (poly.Length == 0)
				return this;

			double[] newPoly = new double[poly.Length + 1];
			for (int i = 0; i < poly.Length; i++)
				newPoly[i + 1] = poly[i] / (i + 1);
			return new Polynomial(newPoly);
		}

		public double Evaluate(double x)
		{
			double result = 0;
			for (int i = poly.Length-1; i >= 0; i--)
				result = result * x + poly[i];
			return result;
		}

		public IEnumerable<double> FindRoots()
		{
			switch (poly.Length)
			{
				case 0:
				case 1: return ListTools.Empty<double>();
				case 2: return ListTools.Singleton(-poly[0]/poly[1]);
				case 3: return QuadraticRoots(poly[2], poly[1], poly[0]);
				default:
					// ComplexRootFinding
					return null;
			}
		}

		public static double[] QuadraticRoots(double a, double b, double c)
		{
			double d = b * b - 4 * a * c;
			if (d < 0)
				return ArrayTools.EmptyArray<double>();
			double q = -0.5 * (b + Math.Sign(b) * Math.Sqrt(d));
			int count = (q != 0 && a != 0) ? 2 : 1;
			double[] result = new double[count];
			if (a != 0)
				result[0] = q / a;
			if (q != 0)
				result[count - 1] = c / q;
			return result;
		}

	    #endregion

		#region Operators

		public bool Equals(Polynomial poly2)
		{
			if (poly.Length != poly2.poly.Length) 
				return false;
			for (int i = 0; i < poly.Length; i++)
				if (!poly[i].AreClose(poly2.poly[i]))
					return false;
			return true;
		}

		public static implicit operator Polynomial(double number)
		{
			return new Polynomial(number);
		}

		public static bool operator ==(Polynomial poly1, Polynomial poly2)
		{
			return poly1.Equals(poly2);
		}

		public static bool operator !=(Polynomial poly1, Polynomial poly2)
		{
			return !poly1.Equals(poly2);
		}

		public static Polynomial operator +(Polynomial poly1, Polynomial poly2)
		{
			double[] result = new double[Math.Max(poly1.Order, poly2.Order)];

			for (int i = 0; i < result.Length; i++)
				result[i] = poly1[i] + poly2[i];

			return new Polynomial(result);
		}

		public static Polynomial operator -(Polynomial poly1, Polynomial poly2)
		{
			double[] result = new double[Math.Max(poly1.Length, poly2.Length)];

			for (int i = 0; i < result.Length; i++)
				result[i] = poly1[i] - poly2[i];

			return new Polynomial(result);
		}

		public static Polynomial operator *(Polynomial poly1, Polynomial poly2)
		{
			double[] result = new double[poly1.Length + poly2.Length -1];

			for (int i = 0; i < poly1.Length; i++)
				for (int j = 0; j < poly2.Length; j++)
					result[i + j] += poly1[i] * poly2[j];

			return new Polynomial(result);
		}

		public static Polynomial operator /(Polynomial poly1, Polynomial poly2)
		{
			Polynomial quot, rem;
			DivRem(poly1, poly2, out quot, out rem);
			return quot;
		}

		public static Polynomial operator %(Polynomial poly1, Polynomial poly2)
		{
			Polynomial quot, rem;
			DivRem(poly1, poly2, out quot, out rem);
			return rem;
		}

		public Polynomial Pow(int n)
		{
			if (n > 1)
			{
				Polynomial tmp = Pow(n / 2);
				return tmp * (n % 2 == 0 ? tmp : this);
			}

			return n == 0 ? 1 : this;
		}
				
		public static void DivRem(Polynomial poly1, Polynomial poly2, 
			out Polynomial quot, out Polynomial rem)
		{
			quot = Zero;
			rem = poly1;
			int order1 = poly1.Order;
			int order2 = poly2.Order;
			if (order1 < order2)
				return;

			if (poly2.Length == 0)
				throw new DivideByZeroException();

			double[] result = new double[order1 - order2 + 1]; 
			double[] tmp = poly1.poly.CloneArray();

			while (order1 >= order2)
			{
				double factor = tmp[order1] / poly2[order2]; 
				result[order1 - order2] = factor;
				for (int i=1; i<=order2; i++)
					tmp[order1-i] -= factor*tmp[order2-i];
				order1--;
			}

			quot = new Polynomial(result);
			rem = new Polynomial(ListTools.CopyRange(tmp,0,poly2.Length-1));
		}
		
		#endregion

		#region Overrides

		public override bool Equals(object obj)
		{
			return obj is Polynomial && Equals((Polynomial) obj );
		}

		public override int GetHashCode()
		{
			return Utility.CreateHashCode(poly);
		}

		public override string ToString()
		{
			return string.Join(", ", poly);
		}

		#endregion
	}
}
