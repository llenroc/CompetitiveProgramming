using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.Strings
{

	/// <summary>
	/// implements Z algorithm for pattern searching
	/// </summary>
	public class ZAlgorithm
	{
		//  prints all occurrences of pattern in text using Z algo
		public IEnumerable<int> Search(string text, string pattern)
		{
			// Create concatenated string "P$T"
			string concat = pattern + "$" + text;
			int n = concat.Length;

			// Construct Z array
			var z = GetZarr(concat);

			//  now looping through Z array for matching condition
			for (int i = 0; i < n; ++i)
			{
				// if Z[i] (matched region) is equal to pattern
				// length  we got the pattern
				if (z[i] == pattern.Length)
					yield return i - pattern.Length - 1;
			}
		}

		// https://discuss.leetcode.com/topic/68493/z-function-o-n-solution
		int[] SimplerZFunction(string s)
		{
			int n = s.Length;
			int[] z = new int[n];

			int left = 0;
			int right = 0;
			for (int i = 1; i < n; i++)
			{
				if (i <= right)
					z[i] = Math.Min(right - i + 1, z[i - left]);

				while (i + z[i] < n && s[i + z[i]] == s[z[i]])
					z[i]++;

				if (i + z[i] - 1 > right)
				{
					right = i + z[i] - 1;
					left = i;
				}
			}

			return z;
		}


		//  Fills Z array for given string str[]
		int[] GetZarr(string str)
		{
			int n = str.Length;
			int[] z = new int[str.Length];
			int r;

			// [L,R] make a window which matches with prefix of s
			var l = r = 0;
			for (int i = 1; i < n; ++i)
			{
				// if i>R nothing matches so we will calculate.
				// Z[i] using naive way.
				if (i > r)
				{
					l = r = i;

					// R-L = 0 in starting, so it will start
					// checking from 0'th index. For example,
					// for "ababab" and i = 1, the value of R
					// remains 0 and Z[i] becomes 0. For string
					// "aaaaaa" and i = 1, Z[i] and R become 5
					while (r < n && str[r - l] == str[r])
						r++;
					z[i] = r - l;
					r--;
				}
				else
				{
					// k = i-L so k corresponds to number which
					// matches in [L,R] interval.
					var k = i - l;

					// if Z[k] is less than remaining interval
					// then Z[i] will be equal to Z[k].
					// For example, str = "ababab", i = 3, R = 5
					// and L = 2
					if (z[k] < r - i + 1)
						z[i] = z[k];

					// For example str = "aaaaaa" and i = 2, R is 5,
					// L is 0
					else
					{
						//  else start from R  and check manually
						l = i;
						while (r < n && str[r - l] == str[r])
							r++;
						z[i] = r - l;
						r--;
					}
				}
			}
			return z;
		}

        // Uwi
        public static int[] Z(char[] str)
        {
            int n = str.Length;
            int[] z = new int[n];
            if (n == 0)
                return z;
            z[0] = n;
            int l = 0, r = 0;
            for (int i = 1; i < n; i++)
            {
                if (i > r)
                {
                    l = r = i;
                    while (r < n && str[r - l] == str[r])
                        r++;
                    z[i] = r - l;
                    r--;
                }
                else
                {
                    int k = i - l;
                    if (z[k] < r - i + 1)
                    {
                        z[i] = z[k];
                    }
                    else
                    {
                        l = i;
                        while (r < n && str[r - l] == str[r])
                            r++;
                        z[i] = r - l;
                        r--;
                    }
                }
            }

            return z;
        }


        // Driver program
        public static void Main()
		{
			string text = "GEEKS FOR GEEKS";
			string pattern = "GEEK";

			var z = new ZAlgorithm();
			foreach(var i in z.Search(text, pattern))
			{
				Console.WriteLine($"Pattern found at index {i}");
			}
		}
	}
}
