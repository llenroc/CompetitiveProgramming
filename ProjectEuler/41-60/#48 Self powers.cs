using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    
    const ulong MOD = 100000UL*100000UL;
    
    static void Main(String[] args) {
        ulong n = ulong.Parse(Console.ReadLine());
        
        ulong sum = 0;
        for (uint i=1; i<=n; i++)
            sum = (sum + ModPow(i,i)) % MOD;
        
        Console.WriteLine(sum);
    }

    public static ulong ModPow(ulong n, ulong p, ulong mod = MOD)
	{
		ulong b = n;
		ulong result = 1;
		while (p != 0)
		{
			if ((p & 1) != 0)
				result = Mult2(result, b, mod);
			p >>= 1;
			b = Mult2(b,b,mod);
		}
		return result;
	}
    
    
    public static ulong Mult2(ulong x, ulong y, ulong mod)
    {
			// x,y,mod must fit within 42 bits
			// x and y can be made to fit within 42 bits by modding first
			// 2^42 = 4.39 * 10^12

			// Thirty times faster than MultSlow
			if (x < 1<<22 || y < 1<<22 || x < 1 << 32 && y < 1 << 32)
			{
				var z = x * y;
				if (z >= mod) z %= mod;
				return z;
			}

			// First term = Xhi * (Y  % mod)
			// (maxbit-bits) + maxbit <= 64
			// Second term = XLo (30-bits) * Y (34-bits)
			// bits + maxbit = 64
			return ((x >> 22) * ((y << 22) % mod) + y * (x & ((1ul << 22) - 1))) % mod;
    }
    
    public static long Mult(long a, long b, long mod)
    {
        if ((ulong)(a) >= (ulong)mod) a %= mod;
        if (a < 0) a += mod;
        if ((ulong)(b) >= (ulong)mod) b %= mod;
        if (b < 0) b += mod;

        long ret = 0;
        int step = 62 - Log2(mod);
        for (int x = Log2(b); x >= 0; x -= step)
        {
            int shift = Math.Min(x + 1, step);
            ret <<= shift;
            ret %= mod;
            ret += a * ((long)((ulong)b >> (x - shift + 1)) & (1L << shift) - 1);
            ret %= mod;
        }
        return ret;
    }
    
        public static int Log2(long value)
		{
			if (value <= 0)
				return value == 0 ? -1 : 63;

			var log = 0;
			if (value >= 0x100000000L)
			{
				log += 32;
				value >>= 32;
			}
			if (value >= 0x10000)
			{
				log += 16;
				value >>= 16;
			}
			if (value >= 0x100)
			{
				log += 8;
				value >>= 8;
			}
			if (value >= 0x10)
			{
				log += 4;
				value >>= 4;
			}
			if (value >= 0x4)
			{
				log += 2;
				value >>= 2;
			}
			if (value >= 0x2)
			{
				log += 1;
			}
			return log;
		}
}