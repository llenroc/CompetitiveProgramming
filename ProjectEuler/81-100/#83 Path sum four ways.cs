using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;
using Tup = System.Tuple<long, long>;

class Solution {
    
    
    static int[][]m;
    
    static void Main(String[] args) {
        int n = int.Parse(Console.ReadLine());
        m = Enumerable.Range(0,n).Select(x=>Array.ConvertAll(Console.ReadLine().Split(), int.Parse)).ToArray();
        
        var v = new long[n,n];
        var q = new SimpleHeap<Tup>();

        for (int i = 0; i<n; i++)
        for (int j = 0; j<n; j++)
            v[i,j] = long.MaxValue;
        
        int [] dir = new int[] { 0, 1, 0, -1, 0} ;
        
        q.Enqueue(new Tup(0,0, m[0][0]));
        v[0,0] = m[0][0];
        while (q.Count>0)
        {
            var min = q.Dequeue();
            var x = min.X;
            var y = min.Y;
            var c = min.Cost;
            
            //if (c != v[x,y]) continue;
            
            for (int d=0; d<4; d++ )
            {
                var x2 = x+dir[d];
                var y2 = y+dir[d+1];
                if (x2<0 || y2<0 || x2>=n || y2>=n) continue;
                var d2 = v[x,y] + m[x2][y2];
                if (d2 >= v[x2,y2]) continue;
                v[x2,y2] = d2;
                if (x2==n-1 && y2==n-1) break;
                q.Enqueue(new Tup(x2,y2,d2));
            }
        }
        
        Console.WriteLine(v[n-1, n-1]);
    }
    
    public class Tup : IComparable<Tup>
    {
        public int X;
        public int Y;
        public long Cost;
        
        public Tup(int x, int y, long cost)
        {
            X=x;
            Y=y;
            Cost=cost;
        }
        
        public int CompareTo(Tup tup)
        {
            int cmp = Cost.CompareTo(tup.Cost);
            if (cmp != 0) return cmp;
            cmp = X.CompareTo(tup.X);
            if (cmp != 0) return cmp;
            cmp = Y.CompareTo(tup.Y);
            return cmp;
        }
    }    
    
    public class SimpleHeap<T>
    {
        Comparison<T> comparison;
        List<T> list = new List<T>();

        public SimpleHeap(Comparison<T> comparison = null)
        {
            this.comparison = comparison ?? Comparer<T>.Default.Compare;
        }

        public int Count => list.Count;

        public T FindMin() => list[0];

        public T Dequeue()
        {
            var pop = list[0];
            var elem = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			if (list.Count > 0) ReplaceTop(elem); 
            return pop;
        }


        public void ReplaceTop(T elem)
        {
            int i = 0;
            list[i] = elem;
            while (true)
            {
                int child = 2 * i + 1;
                if (child >= list.Count)
                    break;

                if (child + 1 < list.Count && comparison(list[child], list[child + 1]) > 0)
                    child++;

                if (comparison(list[child], elem) >= 0)
                    break;

                Swap(i, child);
                i = child;
            }
        }

        public void Enqueue(T push)
        {
            list.Add(push);
            int i = list.Count - 1;
            while (i > 0)
            {
                int parent = (i - 1)/2;
                if (comparison(list[parent], push) <= 0)
                    break;
                Swap(i, parent);
                i = parent;
            }
        }

        void Swap(int t1, int t2)
        {
            T tmp = list[t1];
            list[t1] = list[t2];
            list[t2] = tmp;
        }


    }

}