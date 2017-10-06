using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.RangeQueries
{
    public class SegmentTreeRMQ
    {
        public int M, H, N;
        public int[] st;

        public SegmentTreeRMQ(int n)
        {
            N = n;
            M = BitTools.HighestOneBit(Math.Max(N - 1, 1)) << 2;
            //JAVA TO VB & C# CONVERTER TODO TASK: There is no '>>>' operator in .NET:
            H = (int)((uint)M >> 1);
            st = new int[M];
            for (int i = 0; i < M; i++)
                st[i] = int.MaxValue;
        }

        public SegmentTreeRMQ(int[] a)
        {
            N = a.Length;
            M = BitTools.HighestOneBit(Math.Max(N - 1, 1)) << 2;
            H = (int)(((uint)M) >> 1);
            st = new int[M];
            for (int i = 0; i < N; i++)
            {
                st[H + i] = a[i];
            }

            for (int i = H + N; i < M; i++)
                st[i] = int.MaxValue;

            for (int i = H - 1; i >= 1; i--)
                propagate(i);
        }

        public virtual void update(int pos, int x)
        {
            st[H + pos] = x;
            for (int i = (int)((uint)(H + pos) >> 1); i >= 1; i = (int)((uint)i >> 1))
                propagate(i);
        }

        private void propagate(int i)
        {
            st[i] = Math.Min(st[2 * i], st[2 * i + 1]);
        }

        public virtual int minx(int l, int r)
        {
            int min = int.MaxValue;
            if (l >= r)
                return min;
            while (l != 0)
            {
                int f = l & -l;
                if (l + f > r)
                    break;
                int v = st[(H + l) / f];
                if (v < min)
                    min = v;
                l += f;
            }

            while (l < r)
            {
                int f = r & -r;
                int v = st[(H + r) / f - 1];
                if (v < min)
                    min = v;
                r -= f;
            }
            return min;
        }

        public virtual int min(int l, int r)
        {
            return l >= r ? 0 : min(l, r, 0, H, 1);
        }

        private int min(int l, int r, int cl, int cr, int cur)
        {
            if (l <= cl && cr <= r)
            {
                return st[cur];
            }
            else
            {
                int mid = (int)((uint)(cl + cr) >> 1);
                int ret = int.MaxValue;
                if (cl < r && l < mid)
                {
                    ret = Math.Min(ret, min(l, r, cl, mid, 2 * cur));
                }
                if (mid < r && l < cr)
                {
                    ret = Math.Min(ret, min(l, r, mid, cr, 2 * cur + 1));
                }
                return ret;
            }
        }

        public virtual int firstle(int l, int v)
        {
            int cur = H + l;
            while (true)
            {
                if (st[cur] <= v)
                {
                    if (cur < H)
                    {
                        cur = 2 * cur;
                    }
                    else
                    {
                        return cur - H;
                    }
                }
                else
                {
                    cur++;
                    if ((cur & cur - 1) == 0)
                        return -1;
                    if ((cur & 1) == 0)
                        cur = (int)((uint)cur >> 1);
                }
            }
        }

        public virtual int lastle(int l, int v)
        {
            int cur = H + l;
            while (true)
            {
                if (st[cur] <= v)
                {
                    if (cur < H)
                    {
                        cur = 2 * cur + 1;
                    }
                    else
                    {
                        return cur - H;
                    }
                }
                else
                {
                    if ((cur & cur - 1) == 0)
                        return -1;
                    cur--;
                    if ((cur & 1) == 1)
                        cur = (int)((uint)cur >> 1);
                }
            }
        }
    }

}
