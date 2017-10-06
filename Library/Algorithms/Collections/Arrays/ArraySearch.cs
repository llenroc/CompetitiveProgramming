using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Collections
{
    public class ArraySearch
    {

        /// <summary>
        /// Finds the maximum of a unimodal function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static int TernarySearch(int start, int count, Func<int, int> func)
        {
            int left = start;
            int right = count;
            while (left < right)
            {
                int third = (right - left) / 3;
                var leftThird = left + third;
                var rightThird = right - third; ;
                if (func(leftThird) < func(rightThird))
                    left = leftThird;
                else
                    right = rightThird;
            }
            return (left + right) / 2;
        }

        public static double TernarySearch(double left, double right,
            Func<double, double> f,
            double precision = 1e-15)
        {
            while (Math.Abs(right - left) >= precision)
            {

                var leftThird = (2 * left + right) / 3;
                var rightThird = (left + 2 * right) / 3;
                if (f(leftThird) < f(rightThird))
                    left = leftThird;
                else
                    right = rightThird;
            }
            return (left + right) / 2;
        }


        public static int UpperBound(int[] nums, int left, int right, int k)
        {
            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (k >= nums[mid])
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            return left;
        }

        public static int LowerBound(int[] nums, int left, int right, int k)
        {
            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (k > nums[mid])
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            return left;
        }

        public static int MooresVotingAlgorithm(int[] a)
        {
            int maj_index = 0;
            int count = 1;

            for (int i = 1; i < a.Length; i++)
            {
                if (a[maj_index] == a[i])
                    count++;
                else
                    count--;
                if (count == 0)
                {
                    maj_index = i;
                    count = 1;
                }
            }

            return a[maj_index];
        }

        public static int MooresVotingAlgorithm3(int[] a)
        {
            int maj_index = 0;
            int count = 1;

            for (int i = 1; i < a.Length; i++)
            {
                if (a[maj_index] == a[i])
                    count++;
                else
                    count--;
                if (count == 0)
                {
                    maj_index = i;
                    count = 1;
                }
            }

            return a[maj_index];
        }


        // https://discuss.leetcode.com/topic/17396/boyer-moore-majority-vote-algorithm-generalization
        // TODO: Looks: buggy
        List<int> MajorityElementFor2(int[] a)
        {
            int y = 0, z = 1, cy = 0, cz = 0;
            foreach (var x in a)
            {
                if (x == y)
                    cy++;
                else if (x == z)
                    cz++;
                else if (cy == 0)
                {
                    y = x;
                    cy = 1;
                }
                else if (cz == 0)
                {
                    z = x;
                    cz = 1;
                }
                else
                {
                    cy--;
                    cz--;
                }
            }
            cy = cz = 0;
            foreach (var x in a)
                if (x == y) cy++;
                else if (x == z) cz++;

            List<int> r = new List<int>();
            if (cy > a.Length / 3) r.Add(y);
            if (cz > a.Length / 3) r.Add(z);
            return r;
        }

    }
}
