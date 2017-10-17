using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using static System.Array;
using static System.Math;
using static Library;

class Solution
{
	// public const int MOD = 1000000007;
	private int range;

	private int n;
	Guess[] guesses;
	int[] flags = new int[16];
	List<int>[] choices;
	private int[] number;
	private int[] indices;

	public void solve()
	{
		int n = Ni();

		var guessesList = new List<Guess>();
		string s = null;
		for (int i = 0; i < n; i++)
		{
			s = Ns();
			var guess = new Guess { Number = s, Correct = Ni(), Trial = new int[s.Length] };
			if (guess.Correct == 0)
			{
				for (int j = 0; j < s.Length; j++)
					flags[j] |= 1 << s[j] - '0';
			}
			else
			{
				guessesList.Add(guess);
			}
		}

		range = s.Length;
		number = new int[range];
		guesses = guessesList.ToArray();

		FixNumber();
		Dfs2(-1, range);
		FixNumber();


		WriteLine(string.Join("", number));
	}

	public void FixNumber()
	{
		for (int i = 0; i < range; i++)
		{
			if ((flags[i] & 1 << number[i]) == 0) continue;

			for (int j = 0; j < 10; j++)
			{
				if ((flags[i] & 1 << j) == 0)
				{
					number[i] = j;
					break;
				}
			}
		}
	}


	bool Dfs2(int guess = -1, int index = 0, int k = 0)
	{
		if (index < range)
		{
			if (k > range - index) return false;

			var g = guesses[guess];
			var num = g.Number;
			int d = num[index] - '0';

			var save = flags[index];

			if ((flags[index] & 1 << d) == 0)
			{
				if (k > 0)
				{
					number[index] = d;
					flags[index] = ~(1 << d);
					if (Dfs2(guess, index + 1, k - 1))
					{
						//flags[index] = save;
						return true;
					}
				}
				else
				{
					var isave = ~save & 1023;
					if (isave == 1 << d)
						return false;
				}
			}

			flags[index] = save | 1 << d;
			if ((flags[index] & 1023)!=1023 && Dfs2(guess, index + 1, k))
			{
				//flags[index] = save;
				return true;
			}

			flags[index] = save;
			return false;
		}

		if (k > 0) return false;

		guess++;
		if (guess == guesses.Length) return  true; //Check();
		return Dfs2(guess, 0, guesses[guess].Correct);
	}
	
	public class Guess
	{
		public string Number;
		public int Correct;
		public int[] Trial;
	}

	public bool Check()
	{
		FixNumber();
		foreach (var g in guesses)
		{
			int correct = 0;
			for (int i = 0; i < range; i++)
			{
				if (g.Number[i] - '0' == number[i]
					&& ++correct > g.Correct)
					return false;
			}
			if (correct != g.Correct)
				return false;
		}
		return true;
	}

	public int BitCount(int n)
	{
		int count = 0;
		while (n > 0)
		{
			count++;
			n &= n - 1;
		}
		return count;

	}
}
