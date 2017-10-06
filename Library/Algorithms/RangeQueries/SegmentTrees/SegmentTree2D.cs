using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Softperson.Algorithms.RangeQueries
{
	class SegmentTree2D
	{
		int[,] t;
		int[,] a;
	    int n;
        int m;

        public SegmentTree2D(int[,] a)
        {
            this.n = a.GetLength(0);
            this.m = a.GetLength(1);
            t = new int[n, m];
            this.a = a;
        }

		void BuildY(int vx, int lx, int rx, int vy, int ly, int ry)
		{
			if (ly == ry)
				if (lx == rx)
					t[vx,vy] = a[lx,ly];
				else
					t[vx,vy] = t[vx * 2,vy] + t[vx * 2 + 1,vy];
			else
			{
				int my = (ly + ry) / 2;
				BuildY(vx, lx, rx, vy * 2, ly, my);
				BuildY(vx, lx, rx, vy * 2 + 1, my + 1, ry);
				t[vx,vy] = t[vx,vy * 2] + t[vx,vy * 2 + 1];
			}
		}

		void BuildX(int vx, int lx, int rx)
		{
			if (lx != rx)
			{
				int mx = (lx + rx) / 2;
				BuildX(vx * 2, lx, mx);
				BuildX(vx * 2 + 1, mx + 1, rx);
			}
			BuildY(vx, lx, rx, 1, 0, m - 1);
		}

		int SumY(int vx, int vy, int tly, int try_, int ly, int ry)
		{
			if (ly > ry)
				return 0;
			if (ly == tly && try_ == ry)
				return t[vx,vy];
			int tmy = (tly + try_) / 2;
			return SumY(vx, vy * 2, tly, tmy, ly, Min(ry, tmy))
				+ SumY(vx, vy * 2 + 1, tmy + 1, try_, Max(ly, tmy + 1), ry);
		}

		int SumX(int vx, int tlx, int trx, int lx, int rx, int ly, int ry)
		{
			if (lx > rx)
				return 0;
			if (lx == tlx && trx == rx)
				return SumY(vx, 1, 0, m - 1, ly, ry);
			int tmx = (tlx + trx) / 2;
			return SumX(vx * 2, tlx, tmx, lx, Min(rx, tmx), ly, ry)
				+ SumX(vx * 2 + 1, tmx + 1, trx, Max(lx, tmx + 1), rx, ly, ry);
		}

		void UpdateY(int vx, int lx, int rx, int vy, int ly, int ry, int x, int y, int new_val)
		{
			if (ly == ry)
			{
				if (lx == rx)
					t[vx,vy] = new_val;
				else
					t[vx,vy] = t[vx * 2,vy] + t[vx * 2 + 1,vy];
			}
			else
			{
				int my = (ly + ry) / 2;
				if (y <= my)
					UpdateY(vx, lx, rx, vy * 2, ly, my, x, y, new_val);
				else
					UpdateY(vx, lx, rx, vy * 2 + 1, my + 1, ry, x, y, new_val);
				t[vx,vy] = t[vx,vy * 2] + t[vx,vy * 2 + 1];
			}
		}

		void UpdateX(int vx, int lx, int rx, int x, int y, int new_val)
		{
			if (lx != rx)
			{
				int mx = (lx + rx) / 2;
				if (x <= mx)
					UpdateX(vx * 2, lx, mx, x, y, new_val);
				else
					UpdateX(vx * 2 + 1, mx + 1, rx, x, y, new_val);
			}
			UpdateY(vx, lx, rx, 1, 0, m - 1, x, y, new_val);
		}
	}
}
