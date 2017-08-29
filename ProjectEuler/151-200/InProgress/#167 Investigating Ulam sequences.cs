using System;
using System.Collections.Generic;
using System.IO;
class Solution {

    // https://projecteuclid.org/download/pdf_1/euclid.em/1048709116
    
    static void Main(String[] args) {

        var array = Array.ConvertAll(Console.ReadLine().Split(), long.Parse);
        var n = (int) array[0];
        var k = array[1];
        
        var b = 2 * n + 1;
        var list = new List<long>(30000) {2,b};
        int[] set = new int[170000];

        set[b+2] = 1;
            
        for (int i=b+2; i<set.Length; i++)
        {
            if (set[i] != 1)
                continue;

            for (int j=0; j<list.Count; j++)
            {
                var v = list[j];
                if (i + v >= set.Length) break;
                set[i + v] ++;
            }
            
            list.Add(i);
        }
        
        
        if (k<=list.Count)
        {
            Console.WriteLine(list[(int)k-1]);
            return;        
        }
        
        /*
        var pat = new List<long>(list.Count);
        for (int i=list.Count-1; i>=1; i++)
            diff.Add(list[i] - list[i-1]);
        
        var kmp = new int[diff.Count+1];
        kmp[0] = -1;
        for (int i=0, j=-1; i<diff.Count; i++)
        {
            while (j>=0 && pat[i] != pat[j]) j=kmp[j];
            kmp[i+1] = ++j;
        }
       
        
        //Console.Error.WriteLine(string.Join(" ", list));
        
        Console.Error.WriteLine(System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime);
        Console.Error.WriteLine(list.Count);
        */
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
            ReplaceTop(elem);
            list.RemoveAt(list.Count - 1);
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