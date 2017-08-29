using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    
    static void Main(String[] args) {
        
        int t = int.Parse(Console.ReadLine());
        var ks = Enumerable.Range(0,t).Select(x=>int.Parse(Console.ReadLine())).ToArray();
        var max = ks.Max();
        var sums = new long[max+1];
        
        for (int k=1; k<=max; k++)
        {
            long result1 = 0;
            // Up 
            for (int i=1; i<=9; i++)
            for (int j=i; j<=9; j++)            
            {
                var v = Comb(k-1, j-i) * Fact(9-i, Math.Min(k-1,j-i)) % MOD;
                result1 = (result1 + v) % MOD;
            }
            
            long result2 = 0;
            // Down
            for (int j=9; j>=1; j--)
            for (int i=j; i>=0; i--)
            {
                var v = Comb(k-1, j-i) * Fact(9-i, Math.Min(k-1,j-i)) % MOD;
                result2 = (result2 + v) % MOD;
            }
            
            Console.Error.WriteLine($"{k} {result1} {result2}");
            sums[k] = (sums[k-1] + result1 + result2) % MOD;
        }
        
        foreach(var k in ks)
            Console.WriteLine((Comb(k+10,10) + Comb(k+9,9) - 2 - 10*k) % MOD);
            //Console.WriteLine(sums[k]);            
    }
    
	#region Mod Math
	public const int MOD = 1000 * 1000 * 1000 + 7;

	static int[] _inverse;
	public static long Inverse(long n)
	{
		long result;

		if (_inverse == null)
			_inverse = new int[100001];

		if (n < _inverse.Length && (result = _inverse[n]) != 0)
			return result - 1;

		result = ModPow(n, MOD - 2);
		if (n < _inverse.Length)
			_inverse[n] = (int)(result + 1);
		return result;
	}

	public static long Mult(long left, long right)
	{
		return (left * right) % MOD;
	}

	public static long Div(long left, long divisor)
	{
		return left % divisor == 0
			? left / divisor
			: Mult(left, Inverse(divisor));
	}

	public static long Subtract(long left, long right)
	{
		return (left + (MOD - right)) % MOD;
	}

	public static long Fix(long n)
	{
		return ((n % MOD) + MOD) % MOD;
	}

    public static long ModPow(long n, long p, long mod = MOD)
	{
		long b = n;
		long result = 1;
		while (p != 0)
		{
			if ((p & 1) != 0)
				result = (result * b) % mod;
			p >>= 1;
			b = (b * b) % mod;
		}
		return result;
	}

	public static long Pow(long n, long p)
	{
		long b = n;
		long result = 1;
		while (p != 0)
		{
			if ((p & 1) != 0)
				result *= b;
			p >>= 1;
			b *= b;
		}
		return result;
	}

	#endregion

	#region Combinatorics
	static List<long> _fact;
	static List<long> _ifact;

	public static long Fact(int n)
	{
		if (_fact == null) _fact = new List<long>(100001) { 1 };
		for (int i = _fact.Count; i <= n; i++)
			_fact.Add(Mult(_fact[i - 1], i));
		return _fact[n];
	}
    
    public static long Fact(int n, int k)
    {
        if (k>n) return 0;
        return Fact(n) * InverseFact(n-k);
    }

	public static long InverseFact(int n)
	{
		if (_ifact == null) _ifact = new List<long>(100001) { 1 };
		for (int i = _ifact.Count; i <= n; i++)
			_ifact.Add(Div(_ifact[i - 1], i));
		return _ifact[n];
	}

	public static long Comb(int n, int k)
	{
		if (k <= 1) return k == 1 ? n : k == 0 ? 1 : 0;
		if (k + k > n) return Comb(n, n - k);
		return Mult(Mult(Fact(n), InverseFact(k)), InverseFact(n - k));
	}

    #endregion
    
    
}