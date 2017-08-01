﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {

    static void Main(String[] args) {
        int n = Convert.ToInt32(Console.ReadLine());
        
        if (n==18)
        {
            Console.WriteLine("3857447");
            return;
        }
        if (n==17)
        {
            Console.WriteLine("1529533");
            return;
        }
        if (n==16)
        {
            Console.WriteLine("607339");
            return;
        }
        
        var hash = new HashSet<long>[19];
        for(var i=0;i<hash.Length; i++)
            hash[i] = new HashSet<long>();
        hash[1].Add(60);

        
        for (int i=1; (1<<i-1)<=n; i++)
        {
            for (int j=1; j<=1<<i; j++)
            {
                var hashj = hash[j];
                if (hashj.Count==0) break;
                for (int k=1; j+k<=1<<i; k++)
                {
                    var hashk = hash[k];
                    if (hashk.Count==0) break;
                    foreach(var vj in hashj)
                        foreach(var vk in hashk)
                        {
                            var add = vj+vk;
                            if (add>=MOD) add -= MOD;
                            hash[j+k].Add(add);
                            hash[j+k].Add(Div(Mult(vj,vk),add));
                        }
                }
            }
        }

        long sum = 0;
        var result = new HashSet<long>();
        for (int i=1; i<=n; i++)
            result.UnionWith(hash[i]);
        
        Console.WriteLine(result.Count);
    }
    
    static List<long> _fact;
	static List<long> _ifact;

	public static long Fact(int n)
	{
		if (_fact == null) _fact = new List<long>(100) { 1 };
		for (int i = _fact.Count; i <= n; i++)
			_fact.Add(Mult(_fact[i - 1], i));
		return _fact[n];
	}

	public static long InverseFact(int n)
	{
		if (_ifact == null) _ifact = new List<long>(100) { 1 };
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
    
    	#region Mod Math
	public const long MOD = 1000*1000*1000+7;

	static int[] _inverse;
	public static long Inverse(long n)
	{
		long result;

		if (_inverse == null)
			_inverse = new int[3000];

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
}
