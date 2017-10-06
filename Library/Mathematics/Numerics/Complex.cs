#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2003-2004, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#endregion

namespace Softperson.Mathematics.Redundant
{
    public struct Complex
    {
        #region Field

        private double real;
        private double imag;

        #endregion

        #region Constructors

        public Complex(double re, double im)
        {
            real = re;
            imag = im;
        }

        #endregion

        #region Properties

        public Complex Conjugate
        {
            get { return new Complex(real, -imag); }
        }

        public double Real
        {
            [DebuggerStepThrough]
            get { return real; }
            set { real = value; }
        }

        public double Imaginary
        {
            [DebuggerStepThrough]
            get { return imag; }
            set { imag = value; }
        }

        public bool IsZero
        {
            get { return imag == 0 && real == 0; }
        }

        #endregion

        #region Operators

        public static Complex operator +(Complex arg1, Complex arg2)
        {
            return (new Complex(arg1.real + arg2.real, arg1.imag + arg2.imag));
        }

        public static Complex operator -(Complex arg1)
        {
            return (new Complex(-arg1.real, -arg1.imag));
        }

        public static Complex operator -(Complex arg1, Complex arg2)
        {
            return (new Complex(arg1.real - arg2.real, arg1.imag - arg2.imag));
        }

        public static Complex operator *(Complex arg1, Complex arg2)
        {
            return (new Complex(arg1.real*arg2.real - arg1.imag*arg2.imag, arg1.real*arg2.imag + arg2.real*arg1.imag));
        }

        public static Complex operator /(Complex arg1, Complex arg2)
        {
            double d = arg2.real*arg2.real + arg2.imag*arg2.imag;
            if (d == 0)
                return new Complex(0, 0);

            d = 1/d;
            double c1 = arg1.real*arg2.real + arg1.imag*arg2.imag;
            double c2 = arg1.imag*arg2.real - arg1.real*arg2.imag;
            return (new Complex(c1*d, c2*d));
        }

        public static bool operator ==(Complex c1, Complex c2)
        {
            return c1.real == c2.real && c1.imag == c2.imag;
        }

        public static bool operator !=(Complex c1, Complex c2)
        {
            return c1.real != c2.real || c1.imag != c2.imag;
        }

        public static bool operator ==(Complex c1, double c2)
        {
            return c1.real == c2 && c1.imag == 0;
        }

        public static bool operator !=(Complex c1, double c2)
        {
            return c1.real != c2 || c1.imag != 0;
        }

        public static implicit operator Complex(double d)
        {
            return new Complex(d, 0);
        }

        #endregion

        #region Functions

        public double Abs()
        {
            return Math.Sqrt(real*real + imag*imag);
        }

        public double Arg()
        {
            if (real == 0)
                return 0;
            return (180/Math.PI)*Math.Atan(imag/real);
        }


        public Complex CubeRoot()
        {
            return Pow(1/3.0);
        }

        public Complex SquareRoot()
        {
            return Pow(.5);
        }


        public Complex Exp()
        {
            double r = Math.Exp(Real);
            return new Complex(r*Math.Cos(Imaginary),
                r*Math.Sin(Imaginary));
        }

        public Complex Pow(double n)
        {
            double r = Math.Pow(Abs(), n);
            double t = Arg();
            return new Complex(r*Math.Cos(n*t), r*Math.Sign(n*t));
        }

        public static Complex Polar(double r, double theta)
        {
            return new Complex(r*Math.Cos(theta), r*Math.Sin(theta));
        }

        public override string ToString()
        {
            if (imag == 0) return real.ToString();
            if (real == 0) return imag.ToString() + "i";
            return $"{real} + {imag}i";
        }

        public override bool Equals(object obj)
        {
            if (obj is Complex)
                return (Complex)obj == this;
            return false;
        }

        public override int GetHashCode()
        {
            return (imag * Math.PI).GetHashCode() ^ real.GetHashCode();
        }

        #endregion
    }
}