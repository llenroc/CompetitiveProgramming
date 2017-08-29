using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    
    static void Main(String[] args) {
        
        Console.ReadLine();
        var array = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        
        var list = new HashSet<Data>();
        
        for (int i=0; i<array.Length; i++)
            list.Add(new Data { Flags = 1<<i, Value = array[i]});
    
        for (int i=0; (1<<i-1)<=array.Length; i++)
        {
            var list2 = new HashSet<Data>();
            foreach (var vj in list)
            foreach (var vk in list)
            {
                if ((vj.Flags & vk.Flags) != 0) continue;
                list2.Add(new Data { Flags = vj.Flags | vk.Flags, Value = (vj.Value + vk.Value )%MOD });
                list2.Add(new Data { Flags = vj.Flags | vk.Flags, Value = (vj.Value - vk.Value + MOD)%MOD });
                list2.Add(new Data { Flags = vj.Flags | vk.Flags, Value = (vj.Value * vk.Value)%MOD });
                if (vk.Value != 0) 
                    list2.Add(new Data { Flags = vj.Flags | vk.Flags, Value = Div(vj.Value, vk.Value) });
            }
            list.UnionWith(list2);
        }
        
        var hashSet = new HashSet<long>();
        foreach(var v in list)
        {
            if (v.Flags == (1<<array.Length)-1)
                hashSet.Add(v.Value);
        }
        
        int n=1;
        while (hashSet.Contains(n)) n++;
        
        Console.WriteLine(n-1);
    }
    
    public class Data : IEquatable<Data>
    {
        public int Flags;
        public long Value;
        public override bool Equals(object obj)
        {
            return obj is Data && Equals((Data)obj);
        }
        
        public override int GetHashCode()
        {
            return Flags ^ Value.GetHashCode();
        }
        
        public bool Equals(Data obj)
        {
            return obj.Flags == Flags && Value.Equals(obj.Value);
        }
    }
    
    	#region Mod Math
	public const int MOD = 1000 * 1000 * 1000 + 7;

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