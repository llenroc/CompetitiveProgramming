using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
class Solution {
    static void Main(String[] args) {
        int t = int.Parse(Console.ReadLine());
        
        while (t-->0)
        {
            var array = Array.ConvertAll(Console.ReadLine().Split(), BigInteger.Parse);
            var c = array[0];
            var d = array[1];
            var n = array[2];
            
            var b = Inverse(c, d);
            
            var f = n/d;
            b += f*d;
            if (b > n)
                b -= d;
            
            var a = (b*c-1)/d;
            Console.WriteLine($"{a} {b}");
        }
    }
    
    public static BigInteger Inverse(BigInteger x, BigInteger mod)
    {
        BigInteger xFactor, modFactor;
        var gcd = ExtendedGcd(x, mod, out xFactor, out modFactor);

        var inverse = ((xFactor % mod) + mod) %mod;
        return inverse;
    }

    public static BigInteger ExtendedGcd(BigInteger x, BigInteger y, out BigInteger xFactor, out BigInteger yFactor)
    {
        BigInteger gcd;

        if (x > y)
            return ExtendedGcd(y, x, out yFactor, out xFactor);

        if (x < 0)
        {
            gcd = ExtendedGcd(-x, y, out xFactor, out yFactor);
            xFactor = -xFactor;
            yFactor = -yFactor;
            return gcd;
        }

        if (x == y || x <= 1)
        {
            xFactor = 1;
            yFactor = 0;
            gcd = x;
            return gcd;
        }

        var quot = y/x;
        var rem = y%x;

        BigInteger xFactor2;
        gcd = ExtendedGcd(rem, x, out yFactor, out xFactor2);

        // yFactor2 * x + xfactor2 * rem = gcd
        // quot * x + rem = y 
        // ---------------------
        // xFactor * x + yFactor2 * (y - quot *x) = gcd
        // (xFactor2 - yFactor * quot)*x + yFactor * y = gcd

        xFactor = xFactor2 - yFactor*quot;
        return gcd;
    }


}