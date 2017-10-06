using System;
using System.Collections.Generic;

namespace Softperson.Algorithms.Strings
{
	public class Kmp
	{
		public readonly int[] Lps;
		readonly int _m;
		public readonly string Pattern;

		public Kmp(string pattern)
		{
			Pattern = pattern;
			_m = pattern.Length;
			Lps = new int[_m + 1];

			int j = Lps[0] = -1;
			for (int i = 0; i < pattern.Length; i++)
			{
				while (j >= 0 && pattern[i] != pattern[j])
					j = Lps[j];
				Lps[i + 1] = ++j;
			}
		}
		
		public IEnumerable<int> Instances(string text, int i = 0)
		{
			int j = 0;
			while (i < text.Length)
			{
				while (j >= 0 && text[i] != Pattern[j]) j = Lps[j];
				i++;
				j++;
				if (j == _m)
				{
					yield return i - j;
					j = Lps[j];
				}
			}
		}
	}

	
}
