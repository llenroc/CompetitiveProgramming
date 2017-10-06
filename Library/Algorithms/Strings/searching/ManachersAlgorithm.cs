using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.Strings
{
	public class ManachersAlgorithm
	{

		// http://www.geeksforgeeks.org/manachers-algorithm-linear-time-longest-palindromic-substring-part-4/

		public string FindLongestPalindromicString2(string text)
		{
			int n = text.Length;
			if (n == 0)
				return "";
			n = 2 * n + 1; //Position count
			int[] l = new int[n]; //LPS Length Array
			l[0] = 0;
			l[1] = 1;
			int c = 1; //centerPosition 
			int r = 2; //centerRightPosition
			int maxLpsLength = 0;
			int maxLpsCenterPosition = 0;

			// i = currentRightPosition
			for (int i = 2; i < n; i++)
			{
				//get currentLeftPosition iMirror for currentRightPosition i
				var iMirror = 2 * c - i; //currentLeftPosition
				//Reset expand - means no expansion required
				var expand = 0;
				var diff = r - i;
				//If currentRightPosition i is within centerRightPosition R
				if (diff > 0)
				{
					if (l[iMirror] < diff) // Case 1
						l[i] = l[iMirror];
					else if (l[iMirror] == diff && i == n - 1) // Case 2
						l[i] = l[iMirror];
					else if (l[iMirror] == diff && i < n - 1)  // Case 3
					{
						l[i] = l[iMirror];
						expand = 1;  // expansion required
					}
					else if (l[iMirror] > diff)  // Case 4
					{
						l[i] = diff;
						expand = 1;  // expansion required
					}
				}
				else
				{
					l[i] = 0;
					expand = 1;  // expansion required
				}

				if (expand == 1)
				{
					//Attempt to expand palindrome centered at currentRightPosition i
					//Here for odd positions, we compare characters and 
					//if match then increment LPS Length by ONE
					//If even position, we just increment LPS by ONE without 
					//any character comparison
					while (((i + l[i]) < n && (i - l[i]) > 0) &&
						(((i + l[i] + 1) % 2 == 0) ||
						(text[(i + l[i] + 1) / 2] == text[(i - l[i] - 1) / 2])))
					{
						l[i]++;
					}
				}

				if (l[i] > maxLpsLength)  // Track maxLPSLength
				{
					maxLpsLength = l[i];
					maxLpsCenterPosition = i;
				}

				// If palindrome centered at currentRightPosition i 
				// expand beyond centerRightPosition R,
				// adjust centerPosition C based on expanded palindrome.
				if (i + l[i] > r)
				{
					c = i;
					r = i + l[i];
				}
			}
			var start = (maxLpsCenterPosition - maxLpsLength) / 2;
			var end = start + maxLpsLength - 1;
			return text.Substring(start, end - start + 1);
		}

		// https://discuss.leetcode.com/topic/12944/22-line-c-manacher-s-algorithm-o-n-solution

		public string AlternativeLongestPalindrome(string s)
		{
			var t = new StringBuilder();
			foreach(var i in s)
			{
				t.Append('#');
				t.Append(i);
			}
			t.Append('#');

			int[] p = new int[t.Length]; // Array to record longest palindrome
			int center = 0, boundary = 0, maxLen = 0, resCenter = 0;
			for (int i = 1; i < t.Length - 1; i++)
			{
				int iMirror = 2 * center - i; // calc mirror i = center-(i-center)
				p[i] = (boundary > i) ? Math.Min(boundary - i, p[iMirror]) : 0; // shortcut
				while (i - 1 - p[i] >= 0 && i + 1 + p[i] <= t.Length- 1 && t[i + 1 + p[i]] == t[i - 1 - p[i]]) // Attempt to expand palindrome centered at i
					p[i]++;
				if (i + p[i] > boundary)
				{ // update center and boundary
					center = i;
					boundary = i + p[i];
				}
				if (p[i] > maxLen)
				{ // update result
					maxLen = p[i];
					resCenter = i;
				}
			}
			return s.Substring((resCenter - maxLen) / 2, maxLen);
		}



		public string FindLongestPalindromicString(string text)
		{
			int n = text.Length;
			if (n == 0)
				return "";

			n = 2 * n + 1; //Position count
			var l = new int[n]; //LPS Length Array
			l[0] = 0;
			l[1] = 1;
			int c = 1; //centerPosition 
			int r = 2; //centerRightPosition
			int i; //currentRightPosition
			int maxLpsLength = 0;
			int maxLpsCenterPosition = 0;

			for (i = 2; i < n; i++)
			{
				//get currentLeftPosition iMirror for currentRightPosition i
				var iMirror = 2 * c - i; //currentLeftPosition
				l[i] = 0;
				var diff = r - i;

				//If currentRightPosition i is within centerRightPosition R
				if (diff > 0)
					l[i] = Math.Min(l[iMirror], diff);

				//Attempt to expand palindrome centered at currentRightPosition i
				//Here for odd positions, we compare characters and 
				//if match then increment LPS Length by ONE
				//If even position, we just increment LPS by ONE without 
				//any character comparison
				while (((i + l[i]) < n && (i - l[i]) > 0) &&
					(((i + l[i] + 1) % 2 == 0) ||
					(text[(i + l[i] + 1) / 2] == text[(i - l[i] - 1) / 2])))
				{
					l[i]++;
				}

				if (l[i] > maxLpsLength)  // Track maxLPSLength
				{
					maxLpsLength = l[i];
					maxLpsCenterPosition = i;
				}

				//If palindrome centered at currentRightPosition i 
				//expand beyond centerRightPosition R,
				//adjust centerPosition C based on expanded palindrome.
				if (i + l[i] > r)
				{
					c = i;
					r = i + l[i];
				}
			}

			var start = (maxLpsCenterPosition - maxLpsLength) / 2;
			var end = start + maxLpsLength - 1;
			return text.Substring(start, end - start + 1);
		}

		static int Main()
		{

			var strs = new[]
			{
			"babcbabcbaccba",
			"abaaba",
			"abababa",
			"abcbabcbabcba",
			"forgeeksskeegfor",
			"caba",
			"abacdfgdcaba",
			"abacdfgdcabba",
			"abacdedcaba",

		};


			var ma = new ManachersAlgorithm();
			foreach(var s in strs)
			{
				var lps = ma.FindLongestPalindromicString(s);
				Console.Write($"LPS of string {s} : {lps}");
			}
			return 0;
		}

	}
}
