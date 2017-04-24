using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerRank.CodeSprint
{
    public class ReturnOfNim
    {
        static void Driver(String[] args)
        {
            int errors = 0;
            for (int i = 1; i < 10; i++)
            {
                var sherlock = new HashSet<int>();
                var w = new HashSet<int>();
                BuildPiles(new List<int>(), 1, 10, i, piles =>
                {
                    var match = Solve(piles, true);
                    var match2 = Solve(piles, false);

                    var xor = piles.Aggregate((a, b) => (a + b));
                    if (match && match2)
                    {
                        if (xor == 0)
                        {
                            //Console.Write("(" + string.Join(",", piles) + $"): xor[{xor}] ");
                            //ReportWin(match);
                        }
                        sherlock.Add(xor);
                        return;
                    }


                    Console.Write("(" + string.Join(",", piles) + $"): xor[{xor}] ");
                    if (match != match2)
                    {
                        Console.WriteLine($" Optimized={match} NonOptimized={match2} ");
                        errors++;
                    }
                    else
                    {
                        ReportWin(false);
                        w.Add(xor);

                    }
                });

                Console.WriteLine("S-sums: " + string.Join(",", sherlock.OrderBy(x=>x)));
                Console.WriteLine("W-sums: " + string.Join(",", w.OrderBy(x=>x)));
            }
            Console.WriteLine("Errors: " + errors);
        }


        public void Evaluate()
        {
            
        }

        public static void BuildPiles(List<int> work, int start, int end, int remaining,
            Action<int[]> action)
        {
            if (remaining == 0)
            {
                action(work.ToArray());
                return;
            }

            for (int i = start; i <= end; i++)
            {
                work.Add(i);
                BuildPiles(work, i, end, remaining - 1, action);
                work.RemoveAt(work.Count - 1);
            }
        }

        public static bool Solve(int[] piles, bool optimize=true)
        {
            Array.Sort(piles);

            if (optimize)
            {

                // Win same piles by reducing them all to zero
                if (piles[0] == piles[piles.Length - 1])
                    return true;

                // Pile game of size 2
                if (piles.Length == 2)
                    return WythoffsGame(piles);

                // Win by emptying out a pile to produce an nim game with zero nimsum
                var xor = piles.Aggregate((a, b) => a ^ b);
                if (Array.IndexOf(piles, xor) >= 0)
                    return true;

                if (piles.Length%2 == 1)
                    return xor != 0;
            }

            return (new Solver(piles).CanWin());
        }

        public class Solver
        {
            int[] ranks;
            int size;
            int[] piles;
            //Dictionary<long, int> dict = new Dictionary<long, int>();

            static byte[] g = new byte[10000000];

            public Solver(int[] pilesParam)
            {
                piles = pilesParam;
                ranks = (int[])piles.Clone();
                size = piles.Length;

                long code = MaxCode();
                Array.Clear(g, 0, g.Length);
                // g = new byte[code];
            }


            int depth = 0;
            public bool CanWin()
            {
                if (piles.Length == 0)
                    return true;

                depth++;
                /*if (depth > 300)
                    return true; */

                long code = Code(piles, size);

                if (code < g.Length && code >= 0 && g[code] != 0)
                    return g[code] == 1 ? true : false;

                bool canWin = false;

                // Remove a single pile or all piles to achieve nimsum
                int min = piles[0];
                int xor = 0;
                int xor2 = 0;
                for (int i = 0; i < size; i++)
                {
                    xor ^= piles[i];
                    xor2 ^= piles[i] - min;
                }

                if (xor2 == 0)
                    canWin = true;
                else
                {
                    for (int i = 0; i < size; i++)
                        if (piles[i] == xor)
                        {
                            canWin = true;
                            break;
                        }
                }

                // Remove amount j=1..min-1 from all pile
                for (int j = min - 1; j >= 1 && !canWin; j--)
                {
                    for (int i = 0; i < size; i++) piles[i] -= j;
                    canWin = !CanWin();
                    for (int i = 0; i < size; i++) piles[i] += j;
                }

                // Remove amount from single pile
                for (int i = 0; i < size && !canWin; i++)
                {
                    for (int j = piles[i] - 1; j >= 1 && !canWin; j--)
                    {
                        int k = Bump(piles, i, -j, size);
                        canWin = !CanWin();
                        Bump(piles, k, +j, size);
                    }
                }

                if (code < g.Length && code >= 0)
                    g[code] = (byte)(canWin ? 1 : 2);

                depth--;
                return canWin;
            }


            public long MaxCode()
            {
                long factor = 1;
                for (int i = 0; i < ranks.Length; i++)
                    factor *= ranks[i];
                return factor;
            }

            public long Code(int[] piles, int size)
            {
                long value = 0;
                long factor = 1;
                for (int i = 0; i < size; i++)
                {
                    value = value * factor + piles[size - 1 - i] - 1;
                    factor *= ranks[ranks.Length - 1 - i];
                }
                return value;
            }
        }

        public static int[] Squeeze(int[] piles)
        {
            var list = new List<int>();
            for (int read = 0; read < piles.Length; read++)
            {
                // Remove duplicates
                if (read + 1 < piles.Length && piles[read + 1] == piles[read])
                {
                    read++;
                    continue;
                }
                list.Add(piles[read]);
            }
            return list.ToArray();
        }

        public static int Bump(int[] piles, int index, int amount, int size)
        {
            int oldValue = piles[index];
            int newValue = oldValue + amount;

            if (amount < 0)
            {
                while (index > 0 && piles[index - 1] >= newValue)
                {
                    piles[index] = piles[index - 1];
                    index--;
                }
            }
            else
            {
                while (index + 1 < size && piles[index + 1] < newValue)
                {
                    piles[index] = piles[index + 1];
                    index++;
                }
            }

            piles[index] = newValue;
            return index;
        }

        public static bool ReportWin(bool win)
        {
            if (win == true)
                Console.WriteLine("Sherlock");
            else
                Console.WriteLine("Watson");
            return win;
        }

        public static bool WythoffsGame(int[] piles)
        {
            var gr = (1 + Math.Sqrt(5)) / 2;
            var m0 = piles[0];
            var m1 = piles[1];

            for (int n = 0; true; n++)
            {
                long c0 = (long)(Math.Floor(n * gr));
                long c1 = (long)(Math.Floor(n * gr * gr));
                if (m0 < c0) break;
                if (m0 == c0 && m1 == c1) return false;
            }

            return true;
        }


    }
}
