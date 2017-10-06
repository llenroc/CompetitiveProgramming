#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using Softperson.Mathematics;

#endregion

namespace Softperson.ComputationalGeometry
{
    /// <summary>
    /// Summary description for Vector.
    /// </summary>
    public struct Vector2D : IEquatable<Vector2D>
    {
        #region Vector

        double x;
        double y;

        #endregion

        #region Construction

        [DebuggerStepThrough]
        public Vector2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        #endregion

        #region Object Overrides

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2D))
                return false;
            return Equals((Vector2D) obj);
        }

        public bool Equals(Vector2D vec)
        {
            return x == vec.x && y == vec.y;
        }

        public bool IsCloseTo(Vector2D vec)
        {
            return Numbers.AreClose(x, vec.x)
                   && Numbers.AreClose(y, vec.y);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override string ToString()
        {
            return $"{x},{y}";
        }

        public static Vector2D Parse(string text)
        {
            var terms = text.Trim().Split(',');
            if (terms.Length != 2)
                throw new FormatException();
            return new Vector2D(
                double.Parse(terms[0]),
                double.Parse(terms[1]));
        }

        #endregion

        #region Properties

        public string Text => ToString();

        public static Vector2D XAxis
        {
            [DebuggerStepThrough]
            get { return new Vector2D(1f, 0); }
        }

        public static Vector2D YAxis
        {
            [DebuggerStepThrough]
            get { return new Vector2D(0, 1f); }
        }

        public double this[Axis axis]
        {
            [DebuggerStepThrough]
            get
            {
                switch (axis)
                {
                case Axis.X:
                    return x;
                case Axis.Y:
                    return y;
                case Axis.Z:
                case Axis.W:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
            [DebuggerStepThrough]
            set
            {
                switch (axis)
                {
                case Axis.X:
                    x = value;
                    return;
                case Axis.Y:
                    y = value;
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public double X
        {
            [DebuggerStepThrough]
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            [DebuggerStepThrough]
            get { return y; }
            set { y = value; }
        }

        public bool IsEmpty => x == 0 && y == 0;

        public double Length => Math.Sqrt(x*x + y*y);

        public double LengthSquared => x*x + y*y;

        #endregion

        #region Conversions

        public static explicit operator Vector2D(double d)
        {
            return new Vector2D(d, d);
        }

 

        public static explicit operator Point3D(Vector2D vector)
        {
            return new Point3D(vector.x, vector.y);
        }

        #endregion

        #region Operations

        public void Negate()
        {
            x = -x;
            y = -y;
        }

        public static Vector2D Interpolate(Vector2D vec1, Vector2D vec2,
                                         double a)
        {
            return new Vector2D(vec1.x + a*(vec2.x - vec1.x),
                              vec1.y + a*(vec2.y - vec1.y));
        }

        public static Vector2D Combine(double factor1, Vector2D pt1,
                                     double factor2, Vector2D pt2)
        {
            return new Vector2D(factor1*pt1.x + factor2*pt2.x,
                              factor1*pt1.y + factor2*pt2.y);
        }


        public void Normalize()
        {
            double d = x*x + y*y;
            if (d > Numbers.Epsilon && d != 1.0)
            {
                d = Math.Sqrt(d);
                x = (double) (x/d);
                y = (double) (y/d);
            }
        }

        public double AngleBetween(Vector2D v1, Vector2D v2)
        {
            return Math.Acos(v1.Dot(v2)/(v1.Length*v2.Length))*(180/Math.PI);
        }

        public static Vector2D Projection(Vector2D vector, Vector2D onto)
        {
            return (vector.Dot(onto)/onto.LengthSquared)*onto;
        }


        public static Vector2D Perpendicular(Vector2D vector, Vector2D onto)
        {
            return vector - Projection(vector, onto);
        }

        public static double Area(Vector2D v1, Vector2D v2)
        {
            return v1.Cross(v2);
        }

        public bool IsPerpendicularTo(Vector2D vector)
        {
            return Numbers.IsZeroed(Dot(vector));
        }

        public bool IsParallelTo(Vector2D vector)
        {
            return Numbers.IsZeroed(Cross(vector));
        }

        #endregion

        #region Utility

        public Vector2D Max(Vector2D vec2)
        {
            return new Vector2D(Math.Max(x, vec2.x), Math.Max(y, vec2.y));
        }

        public Vector2D Min(Vector2D vec2)
        {
            return new Vector2D(Math.Min(x, vec2.x), Math.Min(y, vec2.y));
        }

        public static void Swap(ref Vector2D vec1, ref Vector2D vec2)
        {
            var tmp = vec1;
            vec1 = vec2;
            vec2 = tmp;
        }

        #endregion

        #region Math

        public static Vector2D operator -(Vector2D vec, Vector2D vec2)
        {
            return new Vector2D(vec.x - vec2.x, vec.y - vec2.y);
        }

        public static Vector2D operator +(Vector2D vec, Vector2D vec2)
        {
            return new Vector2D(vec.x + vec2.x, vec.x + vec2.y);
        }

        public static Vector2D operator -(Vector2D vec)
        {
            return new Vector2D(-vec.x, -vec.y);
        }

        public static Vector2D operator *(double s, Vector2D vec)
        {
            return new Vector2D(vec.x*s, vec.y*s);
        }

        public static Vector2D operator *(Vector2D vec, double s)
        {
            return new Vector2D(vec.x*s, vec.y*s);
        }

        public static Vector2D operator /(Vector2D vec, double s)
        {
            s = 1/s;
            return new Vector2D(vec.x*s, vec.y*s);
        }

        public static bool operator ==(Vector2D vec1, Vector2D vec2)
        {
            return vec1.Equals(vec2);
        }

        public static bool operator !=(Vector2D vec1, Vector2D vec2)
        {
            return !vec1.Equals(vec2);
        }

        public static double operator *(Vector2D vec1, Vector2D vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y;
        }


        public double Cross(Vector2D vec2)
        {
            return X*vec2.Y -  Y*vec2.X;
        }

        public double Dot(Vector2D vector)
        {
            return x*vector.x + y*vector.y;
        }

        public double Dot(Point2D point)
        {
            return x*point.X + y*point.Y;
        }

        #endregion
    }
}