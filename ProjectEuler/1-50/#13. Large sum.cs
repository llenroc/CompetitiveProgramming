namespace ProjectEuler.Problem13
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Numerics;

	class Solution
	{
		static void Main(String[] args)
		{
			/* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */
			int n = int.Parse(Console.ReadLine());
			var arr = Enumerable.Range(0, n).Select(x => BigInteger.Parse(Console.ReadLine())).ToArray();
			var sum = arr.Aggregate((a, b) => a + b);
			Console.WriteLine(sum.ToString().Substring(0, 10));

		}
	}
}
