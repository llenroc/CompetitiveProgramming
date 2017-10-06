﻿#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Text;
using System.Windows;

#endregion

namespace Softperson.ComputationalGeometry
{
    /// <summary>
    /// Summary description for Perspective.
    /// </summary>
    public class Perspective
    {
        #region Variables

        private Matrix3D transform = new Matrix3D();
        private Vector3D eye;

        #endregion

        #region Construction

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// 
        public Perspective() : this(0, 0, 0)
        {
        }

        public Perspective(double x, double y, double z)
        {
            eye = new Vector3D(x, y, z);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Transform to apply prior to perspective transformation
        /// </summary>
        public Matrix3D Matrix
        {
            get { return transform; }
            set { transform = value; }
        }

        /// <summary>
        /// Depth of scene imagery
        /// </summary>
        public Vector3D Eye
        {
            get { return eye; }
            set { eye = value; }
        }

        public double Depth
        {
            get { return -eye.Z; }
            set { eye.Z = (float) (-value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public void Transform(Point3D[] points)
        {
            for (int i = 0; i < points.Length; i++)
                points[i] *= this;
        }

        /// <summary>
        /// Transforms 3D points to 2D counterparts
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public Point2D[] Transform2D(Point3D[] points)
        {
            var result = new Point2D[points.Length];
            for (int i = 0; i < points.Length; i++)
                result[i] = (Point2D) (points[i]*this);
            return result;
        }

        public Point2D Transform2D(Point3D point)
        {
            return (Point2D) (point*this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="perspective"></param>
        /// <returns></returns>
        public static Point3D operator *(Point3D point, Perspective perspective)
        {
            if (perspective.transform != null)
                point *= perspective.transform;

            var eye = perspective.eye;
            point -= eye;
            double factor = perspective.Depth/point.Z;
            return new Point3D(point.X*factor + eye.X, point.Y*factor + eye.Y, eye.Z);
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Determines whether two Object instances are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;

            if (obj is Perspective var)
                return Equals(var);

            return false;
        }

        /// <summary>
        /// Determines whether two Perspective instances are equal.
        /// </summary>
        public bool Equals(Perspective persp)
        {
            return eye.IsCloseTo(persp.eye)
                   && Equals(persp.transform, transform);
        }

        /// <summary>
        /// Serves as a hash function for a particular type, suitable for use in 
        /// hashing algorithms and data structures like a hash table.
        /// </summary>
        public override int GetHashCode()
        {
            int hash = transform.GetHashCode();
            if (transform != null)
                hash ^= transform.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns a String that represents the current Object.
        /// </summary>
        public override string ToString()
        {
            return eye + "; " + transform;
        }

        #endregion
    }
}