/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the express permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Text;
using System.Windows;
using Softperson.Mathematics;

namespace Softperson.ComputationalGeometry
{
    /// <summary>
    /// Summary description for Point3D.
    /// </summary>
    public struct Point3D : ICloneable
    {
        #region Point3D

        private float x;
        private float y;
        private float z;

        #endregion

        #region Construction

        [DebuggerStepThrough]
        public Point3D(double x, double y, double z)
        {
            this.x = (float) x;
            this.y = (float) y;
            this.z = (float) z;
        }

        [DebuggerStepThrough]
        public Point3D(double x, double y)
        {
            this.x = (float) x;
            this.y = (float) y;
            z = 0;
        }

        #endregion

        #region Object Overrides

        public override bool Equals(object obj)
        {
            if (!(obj is Point3D))
                return false;
            return Equals((Point3D) obj);
        }

        public bool Equals(Point3D pt)
        {
            return x == pt.x && y == pt.y && z == pt.z;
        }

        public bool IsCloseTo(Point3D pt)
        {
            return Numbers.AreClose(x, pt.x)
                   && Numbers.AreClose(y, pt.y)
                   && Numbers.AreClose(z, pt.z);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0},{1},{2}", x, y, z);
        }

        #endregion

        #region Properties

        public string Text
        {
            get { return ToString(); }
        }

        public float this[Axis axis]
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
                    return z;
                case Axis.W:
                    return 1;
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
                case Axis.Z:
                    z = value;
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public float X
        {
            [DebuggerStepThrough]
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            [DebuggerStepThrough]
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            [DebuggerStepThrough]
            get { return z; }
            set { z = value; }
        }

        public bool IsOrigin
        {
            get { return x == 0 && y == 0 && z == 0; }
        }

        public static Point3D Origin
        {
            get { return new Point3D(); }
        }

        #endregion

        #region Conversions

        [DebuggerStepThrough]
        public static implicit operator Point3D(Point2D pt)
        {
            return new Point3D(pt.X, pt.Y);
        }

        public static explicit operator Vector3D(Point3D pt)
        {
            return new Vector3D(pt.x, pt.y, pt.z);
        }

        public static explicit operator Point2D(Point3D pt)
        {
            return new Point2D(pt.x, pt.y);
        }

        public static Point2D[] ToPointArray(Point3D[] points)
        {
            var array = new Point2D[points.Length];
            for (int i = 0; i < array.Length; i++)
                array[i] = (Point2D) points[i];
            return array;
        }

        [DebuggerStepThrough]
        public static Point3D[] ToPoint3DArray(Point2D[] point2Ds)
        {
            var array = new Point3D[point2Ds.Length];
            for (int i = 0; i < array.Length; i++)
                array[i] = point2Ds[i];
            return array;
        }

        #endregion

        #region Operations

        public static Point3D Interpolate(Point3D pt1, Point3D pt2, double a)
        {
            return new Point3D(pt1.x + a*(pt2.x - pt1.x),
                               pt1.y + a*(pt2.y - pt1.y),
                               pt1.z + a*(pt2.z - pt1.z));
        }

        public static Point3D Combine(double factor1, Point3D pt1,
                                      double factor2, Point3D pt2)
        {
            return new Point3D(factor1*pt1.x + factor2*pt2.x,
                               factor1*pt1.y + factor2*pt2.y,
                               factor1*pt1.z + factor2*pt2.z);
        }

        public static double Distance(Point3D pt1, Point3D pt2)
        {
            return (pt2 - pt1).Length;
        }

        #endregion

        #region Utility

        public static Point3D Max(Point3D pt1, Point3D pt2)
        {
            return new Point3D(Math.Max(pt1.x, pt2.x),
                               Math.Max(pt1.y, pt2.y),
                               Math.Max(pt1.z, pt2.z));
        }

        public static Point3D Min(Point3D pt1, Point3D pt2)
        {
            return new Point3D(Math.Min(pt1.x, pt2.x),
                               Math.Min(pt1.y, pt2.y),
                               Math.Min(pt1.z, pt2.z));
        }

        public static void Swap(ref Point3D pt1, ref Point3D pt2)
        {
            var tmp = pt1;
            pt1 = pt2;
            pt2 = tmp;
        }

        #endregion

        #region Math

        public static Vector3D operator -(Point3D pt, Point3D pt2)
        {
            return new Vector3D(pt.x - pt2.x, pt.y - pt2.y, pt.z - pt2.z);
        }

        public static Point3D operator -(Point3D pt, Vector3D vec)
        {
            return new Point3D(pt.x - vec.X, pt.y - vec.Y, pt.z - vec.Z);
        }

        public static Point3D operator -(Vector3D vec, Point3D pt)
        {
            return new Point3D(vec.X - pt.x, vec.Y - pt.y, vec.Z - pt.z);
        }

        public static Point3D operator +(Point3D pt, Vector3D vec)
        {
            return new Point3D(pt.x + vec.X, pt.y + vec.Y, pt.z + vec.Z);
        }

        public static Point3D operator +(Vector3D vec, Point3D pt)
        {
            return new Point3D(pt.x + vec.X, pt.y + vec.Y, pt.z + vec.Z);
        }

        public static Point3D operator -(Point3D pt)
        {
            return new Point3D(-pt.x, -pt.y, -pt.z);
        }

        public static Point3D operator *(double s, Point3D pt)
        {
            return new Point3D(pt.x*s, pt.y*s, pt.z*s);
        }

        public static Point3D operator *(Point3D pt, double s)
        {
            return new Point3D(pt.x*s, pt.y*s, pt.z*s);
        }

        public static Point3D operator /(Point3D pt, double s)
        {
            return new Point3D(pt.x/s, pt.y/s, pt.z/s);
        }

        public static bool operator ==(Point3D pt1, Point3D pt2)
        {
            return pt1.Equals(pt2);
        }

        public static bool operator !=(Point3D pt1, Point3D pt2)
        {
            return !pt1.Equals(pt2);
        }

        public static Point3D Parse(string text)
        {
            var terms = text.Trim().Split(',');
            if (terms.Length != 3)
                throw new FormatException();
            return new Point3D(
                float.Parse(terms[0]),
                float.Parse(terms[1]),
                float.Parse(terms[2]));
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}