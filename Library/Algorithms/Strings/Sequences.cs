using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.Strings
{
	public static class Sequences
	{


		public static int MinimumPeriod(string s)
		{
			return (s + s).IndexOf(s, 1);
		}

		public static int[,] MinimumPeriodOfSubstrings(string s)
		{
			// EncodeStringWithShortestLength

			var period = new int[s.Length, s.Length + 1];
			for (int k = 1; k <= s.Length; k++)
			for (int i = 0; i + k <= s.Length; i++)
				period[i, k] = k;

			for (int k = s.Length - 1; k >= 1; k--)
			{
				int prev = 0;
				for (int i = 0; i + k <= s.Length; i++)
				{
					int j = i + k - 1;
					int curr = (i >= 1 && s[j] == s[i - 1]) ? prev + 1 : 0;
					for (int m = 2, p = curr; p >= k; m++, p -= k)
						period[i - k * (m - 1), k * m] = k;
					prev = curr;
				}
			}

			return period;
		}
	}
}
