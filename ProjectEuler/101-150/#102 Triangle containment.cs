using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        int n = int.Parse(Console.ReadLine());
        
        int count = 0;
        var p = new double[]{0,0};
        
        while (n-- > 0)
        {
            var array = Array.ConvertAll(Console.ReadLine().Split(), double.Parse);
            var p0 = new [] { array[0], array[1] };
            var p1 = new [] { array[2], array[3] };
            var p2 = new [] { array[4], array[5] };

            if (PointInTriangle(p, p0, p1, p2))
                count++;
        }
    
        Console.WriteLine(count);
    }
    
    
		public static bool PointInTriangle2(double[] p, double[] p0, double[] p1, double[] p2)
		{
			double s, t;
			BaryocentricCoordinates(p, p0, p1, p2, out s, out t);
			return (s >= 0 && t >= 0 && 1 - s - t >= 0);
		}

		public static void BaryocentricCoordinates(double[] p, double[] p0, double[] p1, double[] p2, out double s, out double t)
		{
			var area = Area(p0, p1, p2);
			double f = 1 / (2 * area);
			s = f * (p0[1] * p2[0] - p0[0] * p2[1] + (p2[1] - p0[1]) * p[0] + (p0[0] - p2[0]) * p[1]);
			t = f * (p0[0] * p1[1] - p0[1] * p1[0] + (p0[1] - p1[1]) * p[0] + (p1[0] - p0[0]) * p[1]);
		}

		public static double Area(double[] p0, double[] p1, double[] p2)
		{
			return 0.5 * (-p1[1] * p2[0] + p0[1] * (-p1[0] + p2[0]) + p0[0] * (p1[1] - p2[1]) + p1[0] * p2[1]);
		}

    
    static double sign (double[] p1, double[] p2, double[] p3)
    {
        return (p1[0] - p3[0]) * (p2[1] - p3[1]) - (p2[0] - p3[0]) * (p1[1] - p3[1]);
    }

    static bool PointInTriangle (double[] pt, double[] v1, double[] v2, double[] v3)
    {
        bool b1, b2, b3;

        b1 = sign(pt, v1, v2) <= 0.0;
        b2 = sign(pt, v2, v3) <= 0.0;
        b3 = sign(pt, v3, v1) <= 0.0;

        return ((b1 == b2) && (b2 == b3));
    }
}