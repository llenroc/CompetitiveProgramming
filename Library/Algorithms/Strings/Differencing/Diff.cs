
using System;
using System.Text;

namespace Softperson.Algorithms.Strings
{
	public class Diff
	{

		string displayDiff(string oldVersion, string newVersion)
		{
			int[,] dp = new int[oldVersion.Length + 2, newVersion.Length + 2];

			int i, j;
			for (i = oldVersion.Length - 1; i >= 0; i--)
				dp[i, newVersion.Length] = oldVersion.Length - i;

			for (i = newVersion.Length - 1; i >= 0; i--)
				dp[oldVersion.Length, i] = newVersion.Length - i;

			for (i = oldVersion.Length - 1; i >= 0; i--)
			for (j = newVersion.Length - 1; j >= 0; j--)
			{
				var subst = oldVersion[i] == newVersion[j] ? dp[i + 1, j + 1] : int.MaxValue;
				var ins = dp[i, j + 1] + 1;
				var del = dp[i + 1, j] + 1;
				dp[i, j] = Math.Min(subst, Math.Min(ins, del));
			}

			Console.WriteLine(dp[0, 0]);

			var sb = new StringBuilder();

			i = j = 0;
			int state = 0;
			while (i < oldVersion.Length || j < newVersion.Length)
			{
				char ch;

				var keep = i < oldVersion.Length
							&& j < newVersion.Length
							&& oldVersion[i] == newVersion[j]
					? dp[i + 1, j + 1]
					: int.MaxValue;
				var ins = j < newVersion.Length ? dp[i, j + 1] + 1 : int.MaxValue;
				var del = i < oldVersion.Length ? dp[i + 1, j] + 1 : int.MaxValue;
				var min = dp[i, j];

				if (min == int.MaxValue)
					Console.WriteLine("Failed");

				int bestState = state;

				if (bestState == 1 && del > min) bestState = 0;
				else if (bestState == 2 && ins > min) bestState = 0;
				if (bestState == 0 && keep > min || keep == int.MaxValue) bestState = del <= ins ? 1 : 2;

				if (state != bestState)
				{
					if (state == 1) sb.Append(')');
					else if (state == 2) sb.Append(']');
					if (bestState == 1) sb.Append('(');
					else if (bestState == 2) sb.Append('[');
				}

				if (bestState == 2)
					ch = newVersion[j++];
				else
				{
					ch = oldVersion[i++];
					if (bestState == 0)
						j++;
				}

				state = bestState;
				sb.Append(ch);
			}

			if (state == 1) sb.Append(')');
			else if (state == 2) sb.Append(']');
			return sb.ToString();
		}
	}
}