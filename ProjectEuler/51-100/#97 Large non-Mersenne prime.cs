using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        var input = new StreamReader(Console.OpenStandardInput());
        var output = new StreamWriter(Console.OpenStandardOutput());
        
        const ulong mod = 10000UL * 10000UL * 10000UL;
        int t = int.Parse(input.ReadLine());
        ulong sum = 0;
        while (t-->0)
        {
            var array = Array.ConvertAll(input.ReadLine().Split(), ulong.Parse);
            var a = array[0];
            var b = array[1];
            var c = array[2];
            var d = array[3];
            
            var bc = Pow(b,c,mod);
            var result = (Mult(a, bc, mod) + d)%mod;
            sum += result;
        }
        output.WriteLine((sum%mod).ToString("000000000000"));
        output.Flush();
    }
    
    
    public static ulong Mult(ulong x, ulong y, ulong mod)
    {
        // x,y,mod must fit within 42 bits
        // x and y can be made to fit within 42 bits by modding first
        // 2^42 = 4.39 * 10^12

        // Thirty times faster than MultSlow
        if (x <= 1ul << 22 || y <= 1ul << 22 || x < 1 << 32 && y < 1 << 32)
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

   		public static ulong Pow(ulong n, ulong p, ulong mod)
		{
			ulong result = 1;
			ulong b = n;
			while (p > 0)
			{
				if ((p & 1) == 1) result = Mult(result, b, mod);
				p >>= 1;
				b = Mult(b, b, mod);
			}
			return result;
		}

    
}