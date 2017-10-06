using System.Diagnostics.CodeAnalysis;

namespace Softperson.ComputationalGeometry
{
	[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
	public struct Point2D
	{
		public double X;
		public double Y;

		public Point2D(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		public static Point2D operator +(Point2D p, Point2D p2)
		{
			return new Point2D(p.X + p2.X, p.Y + p2.Y);
		}

        public static Point2D operator +(Point2D p, Vector2D p2)
        {
            return new Point2D(p.X + p2.X, p.Y + p2.Y);
        }

        public static Point2D operator -(Point2D vec, Vector2D vec2)
        {
            return new Point2D(vec.X - vec2.X, vec.Y - vec2.Y);
        }

        public static Vector2D operator -(Point2D p0, Point2D p)
		{
			return new Vector2D(p0.X - p.X, p0.Y - p.Y);
		}

		public static Point2D operator *(Point2D p, double c)
		{
			return new Point2D(p.X*c, p.Y*c);
		}

		public static Point2D operator /(Point2D p, double c)
		{
			return new Point2D(p.X/c, p.Y/c);
		}

		public override string ToString()
		{
			return "(" + X + "," + Y + ")";
		}

		public static bool operator <(Point2D lhs, Point2D rhs)
		{
			if (lhs.Y < rhs.Y)
				return true;

			return lhs.Y == rhs.Y && lhs.X < rhs.X;
		}

		public static bool operator >(Point2D lhs, Point2D rhs)
		{
			if (lhs.Y > rhs.Y)
				return true;

			return lhs.Y == rhs.Y && lhs.X > rhs.X;
		}

		public static bool operator ==(Point2D lhs, Point2D rhs)
		{
			return lhs.Y == rhs.Y && lhs.X == rhs.X;
		}

		public static bool operator !=(Point2D lhs, Point2D rhs)
		{
			return lhs.Y != rhs.Y || lhs.X != rhs.X;
		}

		public bool Equals(Point2D other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Point2D && Equals((Point2D)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X.GetHashCode() * 397) ^ Y.GetHashCode();
			}
		}
	}
}