using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Collections
{
    public static class ArrayOperations
    {
        // Reverse Algorithm
        public static void Rotate<T>(T[] array, int first, int nFirst, int last)
        {
            var next = nFirst;
            while (first != next)
            {
                Swap(ref array[first++], ref array[next++]);
                if (next == last)
                    next = nFirst;
                else if (first == nFirst)
                    nFirst = next;
            }
        }

        // Juggling algorithm
        public static void Rotate2<T>(T[] x, int first, int nFirst, int last)
        {
            int n = last-first;
            int d = nFirst - first;
            for (int i = 0; i < Gcd(d, n); i++)
            {
                // Move ith value of blocks
                T t = x[i];
                int j = i;
                while (true)
                {
                    int k = j + d;
                    if (k >= n) k -= n;
                    if (k == i) break;
                    x[j] = x[k];
                    j = k;
                }
                x[j] = t;
            }

        }

        // Blockswap Algorithm

        // Initialize A = arr[0..d - 1] and B = arr[d..n - 1]
        // 1) Do following until size of A is equal to size of B
           
        //   a)  If A is shorter, divide B into Bl and Br such that Br is of same
        //        length as A.Swap A and Br to change ABlBr into BrBlA.Now A
        //        is at its final place, so recur on pieces of B.
           
        //    b)  If A is longer, divide A into Al and Ar such that Al is of same
        //        length as B Swap Al and B to change AlArB into BArAl.Now B
        //        is at its final place, so recur on pieces of A.
           
        // 2)  Finally when A and B are of equal size, block swap them.

        public static void LeftRotate(int[] arr, int d, int n)
        {
            if (d == 0 || d == n)
                return;
            int i = d;
            int j = n - d;
            while (i != j)
            {
                if (i < j) /*A is shorter*/
                {
                    Swap(arr, d - i, d + j - i, i);
                    j -= i;
                }
                else /*B is shorter*/
                {
                    Swap(arr, d - i, d, j);
                    i -= j;
                }
                // printArray(arr, 7);
            }
            /*Finally, block swap A and B*/
            Swap(arr, d - i, d, i);
        }


        /// <summary>
        /// This function swaps d elements starting at index fi
        ///  with d elements starting at index si
        /// </summary>
        /// <param name="arr">The arr.</param>
        /// <param name="fi">The fi.</param>
        /// <param name="si">The si.</param>
        /// <param name="size">The d.</param>
        public static void Swap(int[] arr, int fi, int si, int size)
        {
            for (int i = 0; i < size; i++)
            {
                var temp = arr[fi + i];
                arr[fi + i] = arr[si + i];
                arr[si + i] = temp;
            }
        }

        public static int Gcd(int a, int b)
        {
            if (a>b) Swap(ref a, ref b);

            while (a != 0)
            {
                int tmp = b%a;
                b = a;
                a = tmp;
            }

            return b;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }



        public static void Fill<T>(List<T> list, int count)
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(default(T));
            }
        }


        public static void Fill<T>(List<T> list, int count, T value)
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(value);
            }
        }

        public static void Fill<T>(List<T> list, int count, Func<T> func)
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(func());
            }
        }


        public static List<T> Repeat<T>(int n, T value)
        {
            var list = new List<T>();
            Fill(list, n, value);
            return list;
        }

        public static void InPlaceMerge<T>(T[] array, int start2)
        {

        }


        // TODO: Untested and buggy
        public static int StablePartition<T>(T[] array, 
            T value, int start, int count)
            where T : IComparable<T>
        {
            if (count < 2)
            {
                if (count == 1 && array[0].CompareTo(value) < 0)
                    return -1;
                return 0;
            }

            int end = start + count - 1;
            int mid = (start + count) / 2;

            int index1 = StablePartition(array, value, start, mid);
            int index2 = StablePartition(array, value, mid, end-mid+1);
            int pivot = mid;
            Rotate(array, index1, pivot, index2);
            return pivot;
        }


    }

}
