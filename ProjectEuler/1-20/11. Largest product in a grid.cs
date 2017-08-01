namespace ProjectEuler.Problem11
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	class Solution
	{

		static void Main(String[] args)
		{
			int[][] grid = new int[20][];
			for (int grid_i = 0; grid_i < 20; grid_i++)
			{
				string[] grid_temp = Console.ReadLine().Split(' ');
				grid[grid_i] = Array.ConvertAll(grid_temp, Int32.Parse);
			}

			long max = 0;
			for (int i = 0; i < 20; i++)
				for (int j = 0; j < 20; j++)
				{
					var d = i <= 16 && j <= 16 ? 1 : 0;
					long h = j <= 16 ? 1 : 0, v = i <= 16 ? 1 : 0, d1 = d, d2 = d;
					for (int k = 0; k < 4; k++)
					{
						if (h > 0) h *= grid[i][j + k];
						if (v > 0) v *= grid[i + k][j];
						if (d1 > 0) d1 *= grid[i + k][j + k];
						if (d2 > 0) d2 *= grid[i + 3 - k][j + k];
					}
					var t = Math.Max(Math.Max(h, v), Math.Max(d1, d2));
					max = Math.Max(max, t);
				}
			Console.WriteLine(max);
		}
	}

}
