namespace HackerRank.WeekOfCode28.LuckyNumber8
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    class Solution
    {

        static void Driver(string[] args)
        {
            int n = Convert.ToInt32(Console.ReadLine());
            string number = Console.ReadLine();
            var count = CountEights(number);
            Console.WriteLine(count);
        }

        private const long MOD = 1000L * 1000L * 1000L + 7;

        static long CountEights(string number)
        {
            long count = 0;
            var c2 = new long[2];
            var c4 = new long[4];
            c2[0] = 1;
            c4[0] = 1;

            for (int i = 0; i < number.Length; i++)
            {
                var ch = number[i] - '0';
        
                if (ch % 2 == 0)
                {
                    switch (ch)
                    {
                        case 0:
                        case 8:
                            count += c4[0];
                            break;
                        case 2:
                            count += c4[3];
                            break;
                        case 4:
                            count += c4[2];
                            break;
                        case 6:
                            count += c4[1];
                            break;
                    }
                    count %= MOD;
                }

                /* switch (ch)
                {
                    case 0: case 4: case 8:
                        c4[0] = (c4[0] + c2[0]) % MOD;
                        c4[2] = (c4[2] + c2[1]) % MOD;
                        break;
                    case 1: case 5: case 9:
                        c4[1] = (c4[1] + c2[0]) % MOD;
                        c4[3] = (c4[3] + c2[1]) % MOD;
                        break;
                    case 2:  case 6:
                        c4[0] = (c4[0] + c2[1]) % MOD;
                        c4[2] = (c4[2] + c2[0]) % MOD;
                        break;
                    case 3: case 7:
                        c4[1] = (c4[1] + c2[1]) % MOD;
                        c4[3] = (c4[3] + c2[0]) % MOD;
                        break;
                }*/

                c4[ch % 4] = (c4[ch % 4] + c2[0]) % MOD;
                c4[ch % 4 ^ 2] = (c4[ch % 4 ^ 2] + c2[1]) % MOD;
                c2[ch % 2] = (c2[ch % 2] + c2[0] + c2[1]) % MOD;
            }

            return count;
        }
    }
}

