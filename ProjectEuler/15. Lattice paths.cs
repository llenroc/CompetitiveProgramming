namespace ProjectEuler.Problem15
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Numerics;
	class Solution
	{
		const long MOD = 1000 * 1000 * 1000 + 7;
		static void Main(String[] args)
		{
			int tc = int.Parse(Console.ReadLine());
			while (tc-- > 0)
			{
				var arr = Array.ConvertAll(Console.ReadLine().Split(), long.Parse);

				var r = arr[0];
				var c = arr[1];
				var n = r + c;
				var k = Math.Min(r, c);

				BigInteger result = 1;
				for (int i = 0; i < k; i++)
					result = (result * (n - i) / (i + 1));
				Console.WriteLine(result % MOD);
			}

		}
	}
}
