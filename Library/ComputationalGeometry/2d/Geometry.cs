using System;
using System.Collections.Generic;
using System.Diagnostics;
using Softperson.Algorithms;
using static System.Math;

// From Stanford Notebook
//namespace Softperson.Algorithms.Stanford
namespace Softperson.ComputationalGeometry
{

    public class Geometry
    {
        // C++ routines for computational geometry.

        private const double Inf = double.PositiveInfinity;
        private const double Eps = 1e-12;

        private static double Dot(Point2D p, Point2D q)
        {
            return p.X * q.X + p.Y * q.Y;
        }

        private static double Dist2(Point2D p, Point2D q)
        {
            return (p - q).LengthSquared;
        }

        /// <summary>
        /// rotate a point CCW or CW around the origin
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Vector2D RotateCcw90(Vector2D p)
        {
            return new Vector2D(-p.Y, p.X);
        }

        public static Vector2D RotateCw90(Vector2D p)
        {
            return new Vector2D(p.Y, -p.X);
        }

        public static Vector2D RotateCcw(Vector2D p, double t)
        {
            return new Vector2D(p.X * Cos(t) - p.Y * Sin(t),
                p.X * Sin(t) + p.Y * Cos(t));
        }

        /// <summary>
        /// project point c onto line through a and b
        /// assuming a != b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Point2D ProjectPointLine(Point2D a, Point2D b, Point2D c)
        {
            var ab = b - a;
            return a + ab * (c - a).Dot(ab) / ab.LengthSquared;
        }

        /// <summary>
        /// project point c onto line segment through a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Point2D ProjectPointSegment(Point2D a, Point2D b, Point2D c)
        {
            var ab = b - a;
            var r = ab.LengthSquared;
            if (Abs(r) < Eps) return a;
            r = ab.Dot(c - a) / r;
            return r < 0 ? a : (r > 1 ? b : a + (b - a) * r);
        }

        /// <summary>
        /// compute distance from c to segment between a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double DistancePointSegment(Point2D a, Point2D b, Point2D c)
        {
            return Sqrt(Dist2(c, ProjectPointSegment(a, b, c)));
        }

        /// <summary>
        /// compute distance between point (x,y,z) and plane ax+by+cz=d
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double DistancePointPlane(double x, double y, double z,
            double a, double b, double c, double d)
        {
            return Abs(a * x + b * y + c * z - d) / Sqrt(a * a + b * b + c * c);
        }

        /// <summary>
        /// determine if lines from a to b and c to d are parallel or collinear
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool LinesParallel(Point2D a, Point2D b, Point2D c, Point2D d)
        {
            return Abs((b - a).Cross(c - d)) < Eps;
        }

        public static bool LinesCollinear(Point2D a, Point2D b, Point2D c, Point2D d)
        {
            return LinesParallel(a, b, c, d)
                   && Abs((a - b).Cross(a - c)) < Eps
                   && Abs((c - d).Cross(c - a)) < Eps;
        }

        /// <summary>
        /// determine if line segment from a to b intersects with 
        /// line segment from c to d
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool SegmentsIntersect(Point2D a, Point2D b, Point2D c, Point2D d)
        {
            if (LinesCollinear(a, b, c, d))
            {
                if (Dist2(a, c) < Eps || Dist2(a, d) < Eps ||
                    Dist2(b, c) < Eps || Dist2(b, d) < Eps) return true;
                if ((c-a).Dot(c - b) > 0 && (d-a).Dot(d - b) > 0 && (c-b).Dot(d - b) > 0)
                    return false;
                return true;
            }

            var ab = b - a;
            if ((d-a).Cross(ab) * (c-a).Cross(ab) > 0) return false;
            var cd = d - c;
            return (a-c).Cross(cd) * (b-c).Cross(cd) <= 0;
        }

        /// <summary>
        /// compute intersection of line passing through a and b
        /// with line passing through c and d, assuming that unique
        /// intersection exists; for segment intersection, check if
        /// segments intersect first
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Point2D ComputeLineIntersection(Point2D a, Point2D b, Point2D c, Point2D d)
        {
            var ab = b - a;
            var dc = c - d;
            var ac = c - a;
            Debug.Assert(ab.LengthSquared > Eps && dc.LengthSquared > Eps);
            return a + ab * ac.Cross(dc) / ab.Cross(dc);
        }

        /// <summary>
        /// compute center of circle given three points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Point2D ComputeCircleCenter(Point2D a, Point2D b, Point2D c)
        {
            b = (a + b) / 2;
            c = (a + c) / 2;
            return ComputeLineIntersection(b, b + RotateCw90(a - b), c, c + RotateCw90(a - c));
        }

        /// <summary>
        /// determine if point is in a possibly non-convex polygon (by William
        /// Randolph Franklin); returns 1 for strictly interior points, 0 for
        /// strictly exterior points, and 0 or 1 for the remaining points.
        /// Note that it is possible to convert this into an *exact* test using
        /// integer arithmetic by taking care of the division appropriately
        /// (making sure to deal with signs properly) and then by writing exact
        /// tests for checking point on polygon boundary
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static bool PointInPolygon(List<Point2D> p, Point2D q)
        {
            var c = false;
            for (var i = 0; i < p.Count; i++)
            {
                var j = (i + 1) % p.Count;
                if ((p[i].Y <= q.Y && q.Y < p[j].Y ||
                     p[j].Y <= q.Y && q.Y < p[i].Y) &&
                    q.X < p[i].X + (p[j].X - p[i].X) * (q.Y - p[i].Y) / (p[j].Y - p[i].Y))
                    c = !c;
            }
            return c;
        }

        /// <summary>
        /// determine if point is on the boundary of a polygon
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static bool PointOnPolygon(List<Point2D> p, Point2D q)
        {
            for (var i = 0; i < p.Count; i++)
                if (Dist2(ProjectPointSegment(p[i], p[(i + 1) % p.Count], q), q) < Eps)
                    return true;
            return false;
        }

        /// <summary>
        /// compute intersection of line through points a and b with
        /// circle centered at c with radius r > 0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static List<Point2D> CircleLineIntersection(Point2D a, Point2D b, Point2D c, double r)
        {
            var ret = new List<Point2D>();
            var ab = b - a;
            var ca = a - c;
            var A = ab.LengthSquared;
            var B = ca.Dot(ab);
            var C = ca.LengthSquared - r * r;
            var d = B * B - A * C;
            if (d < -Eps) return ret;
            ret.Add(c + ca + ab * (-B + Sqrt(d + Eps)) / A);
            if (d > Eps)
                ret.Add(c + ca + ab * (-B - Sqrt(d)) / A);
            return ret;
        }

        /// <summary>
        /// compute intersection of circle centered at a with radius r
        /// with circle centered at b with radius R
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="r"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static List<Point2D> CircleCircleIntersection(Point2D a, Point2D b, double r, double R)
        {
            var ret = new List<Point2D>();
            var d = Sqrt(Dist2(a, b));
            if (d > r + R || d + Min(r, R) < Max(r, R)) return ret;
            var x = (d * d - R * R + r * r) / (2 * d);
            var y = Sqrt(r * r - x * x);
            var v = (b - a) / d;
            ret.Add(a + v * x + RotateCcw90(v) * y);
            if (y > 0)
                ret.Add(a + v * x - RotateCcw90(v) * y);
            return ret;
        }

        /// <summary>
        /// This code computes the area or centroid of a (possibly nonconvex)
        /// polygon, assuming that the coordinates are listed in a clockwise or
        /// counterclockwise fashion.  Note that the centroid is often known as
        /// the "center of gravity" or "center of mass".
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double ComputeSignedArea(List<Point2D> p)
        {
            double area = 0;
            for (var i = 0; i < p.Count; i++)
            {
                var j = (i + 1) % p.Count;
                area += p[i].X * p[j].Y - p[j].X * p[i].Y;
            }
            return area / 2.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double ComputeArea(List<Point2D> p)
        {
            return Abs(ComputeSignedArea(p));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Point2D ComputeCentroid(List<Point2D> p)
        {
            var c = new Point2D(0, 0);
            var scale = 6.0 * ComputeSignedArea(p);
            for (var i = 0; i < p.Count; i++)
            {
                var j = (i + 1) % p.Count;
                c = c + (p[i] + p[j]) * (p[i].X * p[j].Y - p[j].X * p[i].Y);
            }
            return c / scale;
        }

        /// <summary>
        /// tests whether or not a given polygon (in CW or CCW order) is simple
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsSimple(List<Point2D> p)
        {
            for (var i = 0; i < p.Count; i++)
            {
                for (var k = i + 1; k < p.Count; k++)
                {
                    var j = (i + 1) % p.Count;
                    var l = (k + 1) % p.Count;
                    if (i == l || j == k) continue;
                    if (SegmentsIntersect(p[i], p[j], p[k], p[l]))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="r"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        public static void ConvertToLatLong(double x, double y, double z,
            out double r, out double lat, out double lon)
        {
            r = Sqrt(x * x + y * y + z * z);
            lat = 180 / PI * Asin(z / r);
            lon = 180 / PI * Acos(x / Sqrt(x * x + y * y));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void ConvertToRect(double r, double lat, double lon,
            out double x, out double y, out double z)
        {
            x = r * Cos(lon * PI / 180) * Cos(lat * PI / 180);
            y = r * Sin(lon * PI / 180) * Cos(lat * PI / 180);
            z = r * Sin(lat * PI / 180);
        }

        /// <summary>
        /// Slow but simple Delaunay triangulation. Does not handle
        /// degenerate cases (from O'Rourke, Computational Geometry in C)
        ///
        /// Running time: O(n^4)
        /// </summary>
        /// <param name="x">x-coordinates</param>
        /// <param name="y">y-coordinates</param>
        /// <returns>
        /// triples = a vector containing m triples of indices
        ///                     corresponding to triangle vertices
        /// </returns>

        public static List<Tuple<int, int, int>> DelaunayTriangulation(List<double> x, List<double> y)
        {
            var n = x.Count;
            var z = STL.Repeat(n, 0.0);
            var ret = new List<Tuple<int, int, int>>();

            for (var i = 0; i < n; i++)
                z[i] = x[i] * x[i] + y[i] * y[i];

            for (var i = 0; i < n - 2; i++)
            {
                for (var j = i + 1; j < n; j++)
                {
                    for (var k = i + 1; k < n; k++)
                    {
                        if (j == k) continue;
                        var xn = (y[j] - y[i]) * (z[k] - z[i]) - (y[k] - y[i]) * (z[j] - z[i]);
                        var yn = (x[k] - x[i]) * (z[j] - z[i]) - (x[j] - x[i]) * (z[k] - z[i]);
                        var zn = (x[j] - x[i]) * (y[k] - y[i]) - (x[k] - x[i]) * (y[j] - y[i]);
                        var flag = zn < 0;
                        for (var m = 0; flag && m < n; m++)
                            flag = flag && ((x[m] - x[i]) * xn +
                                            (y[m] - y[i]) * yn +
                                            (z[m] - z[i]) * zn <= 0);
                        if (flag)
                        {
                            ret.Add(new Tuple<int, int, int>(i, j, k));
                        }
                    }
                }
            }
            return ret;
        }



    }
}