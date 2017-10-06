using System;
using System.Collections.Generic;
using Softperson.Algorithms;

namespace Softperson.ComputationalGeometry
{

	public class ConvexHull
	{

		readonly List<Point2D> Points;

		/// <summary>
		/// Compute the 2D convex hull of a set of points using the monotone chain  algorithm.  
		/// Points property contains a List of points in the convex hull, counterclockwise, starting 
		/// with bottommost/leftmost point
		/// Running time: O(n log n)
		/// </summary>
		/// <param name="points">a List of input points, unordered.</param>
		public ConvexHull(List<Point2D> points)
		{
			var hashSet = new HashSet<Point2D>(points);
			points.Clear();
			points.AddRange(hashSet);

			points.Sort((a, b) =>
			{
				int cmp = a.X.CompareTo(b.X);
				if (cmp != 0) return cmp;
				return a.Y.CompareTo(b.Y);
			});

			var uniqueEnd = points.unique();
			points.RemoveRange(uniqueEnd, points.Count - uniqueEnd);

			var up = new List<Point2D>();
			var down = new List<Point2D>();
			for (var i = 0; i < points.Count; i++)
			{
				while (up.Count > 1 && Area2(up[up.Count - 2], up[up.Count - 1], points[i]) > 0) up.RemoveAt(up.Count - 1);
				while (down.Count > 1 && Area2(down[down.Count - 2], down[down.Count - 1], points[i]) < 0) down.RemoveAt(down.Count - 1);
				up.Add(points[i]);
				down.Add(points[i]);
			}

			hashSet.Clear();
			hashSet.UnionWith(down);
			for (var i = up.Count - 2; i >= 1; i--)
			{
				if (hashSet.Contains(up[i])) continue;
				down.Add(up[i]);
			}

			Points = down;
		}

		public static List<Point2D> RemoveRedundant(List<Point2D> points)
		{
			if (points.Count <= 2)
				return new List<Point2D>(points);

			var result = new List<Point2D> { points[0], points[1] };
			for (var i = 2; i < points.Count; i++)
			{
				if (Between(result[result.Count - 2], result[result.Count - 1], points[i])) result.RemoveAt(result.Count - 1);
				result.Add(points[i]);
			}
			if (result.Count >= 3 && Between(result[result.Count - 1], result[0], result[1]))
			{
				result[0] = result[result.Count - 1];
				result.RemoveAt(result.Count - 1);
			}

			return result;
		}

		#region Helpers

		const double Eps = 1e-9;

		static bool Between(Point2D a, Point2D b, Point2D c)
		{
			return Math.Abs(Area2(a, b, c)) < Eps && (a.X - b.X) * (c.X - b.X) <= 0 && (a.Y - b.Y) * (c.Y - b.Y) <= 0;
		}


		static double Cross(Point2D p, Point2D q)
		{
			return p.X * q.Y - p.Y * q.X;
		}

		static double Area2(Point2D a, Point2D b, Point2D c)
		{
			return Cross(a, b) + Cross(b, c) + Cross(c, a);
		}

		// Probably the same as Area2
		static double Cross(Point2D a, Point2D b, Point2D c)
		{
			return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
		}

		#endregion
	}
}