using System;
using System.Collections.Generic;
using System.IO;
using static System.Math;
class Solution {
    static void Main(String[] args) {

        int n = int.Parse(Console.ReadLine());

        var list = new List<Point2D[]>();
        for (int i=0; i<n; i++)
        {
            list.Add(new [] { new Point2D(Rand500(), Rand500()),  new Point2D(Rand500(), Rand500()) });
            Console.Error.WriteLine(string.Join(" ", list[list.Count-1]));
        }

        int count = 0;
        var hash = new HashSet<Point2D>();        
        for (int i=0; i<list.Count; i++)
            for (int j=i+1; j<list.Count; j++)
                if (SegmentsIntersect(list[i][0], list[i][1], list[j][0], list[j][1]))
                {
                    hash.Add(ComputeLineIntersection(list[i][0], list[i][1], list[j][0], list[j][1]));
                    Console.Error.WriteLine(string.Join("x", new [] {list[i][0], list[i][1], list[j][0], list[j][1]}));
                    count ++;
                }
        
        Console.WriteLine(hash.Count);
        
    }
    
    static long sn = 290797;

    public static int Rand()
    {
        var orig = sn;
        sn = sn * 1L * sn % 50515093;
        return (int)sn;
    }
    
    public static int Rand500()
    {
        return Rand() % 500;
    }
    
    public static Point2D ComputeLineIntersection(Point2D a, Point2D b, Point2D c, Point2D d)
    {
        var ab = b - a;
        var dc = c - d;
        var ac = c - a;
        return a + ab * ac.Cross(dc) / ab.Cross(dc);
    }


    public static bool SegmentsIntersect(Point2D a, Point2D b, Point2D c, Point2D d)
    {
        if (LinesCollinear(a, b, c, d))
            return false;

        if (a == c || a == d || b ==c || b == d)
            return false;
        
        
        var ab = b - a;
        if ((d-a).Cross(ab) * (c-a).Cross(ab) >= 0) return false;
        var cd = d - c;
        return (a-c).Cross(cd) * (b-c).Cross(cd) < 0;
    }


    private const double Inf = double.PositiveInfinity;
    private const double Eps = 1e-12;

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

    private static double Dot(Point2D p, Point2D q)
    {
        return p.X * q.X + p.Y * q.Y;
    }

    private static double Dist2(Point2D p, Point2D q)
    {
        return (p - q).LengthSquared;
    }


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

        public static Point2D operator -(Point2D p0, Point2D p)
        {
            return new Point2D(p0.X - p.X, p0.Y - p.Y);
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
        
        public double LengthSquared => X*X + Y*Y;


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

        public double Cross(Point2D vec2)
        {
            return X*vec2.Y -  Y*vec2.X;
        }

        public double Dot(Point2D vector)
        {
            return X*vector.X + Y*vector.Y;
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