using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerRank
{
    class ArrayConstruction
    {

        static void Main(String[] args)
        {
            Console.SetIn(new StringReader(
@"1
3 3 4"));

            new ArrayConstruction().Run();
        }

        int n, s, k;
        int[] buffer;

        public void Run()
        {
            int q = int.Parse(Console.ReadLine());
            for (int i = 0; i < q; i++)
            {
                var a = Console.ReadLine().Split().Select(int.Parse).ToArray();
                n = a[0];
                s = a[1];
                k = a[2];
                buffer = new int[n];

                bool good = false;
                for (int j = 0; j <= s; j++)
                {
                    buffer[n - 1] = j;
                    if (Sum(n - 1, s, k, i))
                    {
                        good = true;
                        Array.Reverse(buffer);
                        Console.WriteLine(string.Join(" ", buffer));
                    }
                }

                if (!good)
                    Console.WriteLine(-1);
            }
        }

        public bool Sum(int n, int s, int k, int prev)
        {
            if (n <= 0) return n == 0 && s == 0 && k == 0;
            if (s < 0 || k < 0) return false;
            if (s == 0 && k != prev) return false;

            if (n == 1)
            {
                buffer[0] = s;
                return Math.Abs(s - prev) == k;
            }

            for (int i = 0; i <= s; i++)
            {
                int diff = Math.Abs(prev - i);
                buffer[n - 1] = i;
                if (Sum(n - 1, s - i, k - diff, i))
                    return true;
            }

            return false;
        }
    }
}
