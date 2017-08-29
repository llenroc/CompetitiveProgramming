using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

class Solution {
    
    static long limit;
    static void Main(String[] args) {
        var array = Array.ConvertAll(Console.ReadLine().Split(), long.Parse);
        var a = array[0];
        limit = array[1];
        
        //var factors = PrimeFactorsUpTo((int)n);
        
        var count = SternBrocotCount(a+1,a);

        /*
        long preva;
        long prevb;
        BigInteger pa, pb;
        PrevFarey(1,a+1, limit, out pa, out pb);
        preva = (long)pa;
        prevb = (long)pb;
        long cura = 1;
        long curb = a+1;
        
        while (false)
        {
            var f = (limit+prevb)/curb;
            var nexta = f*cura - preva;
            var nextb = f*curb - prevb;
            
            var g = Gcd(nexta, nextb);
            nexta /= g;
            nextb /= g;
            
            if (nexta==1 && nextb==a)
                break;
            
            preva= cura;
            prevb = curb;
            cura = nexta;
            curb = nextb;
            count++;
        }*/
       
        Console.WriteLine(count);
    }

    
    public static void PrevFarey(BigInteger c, BigInteger d, BigInteger n, out BigInteger a, out BigInteger b)
    {
        b = Inverse(c, d);

        var f = n/d;
        b += f*d;
        if (b > n)
            b -= d;

        a = (b*c-1)/d;

        Console.Error.WriteLine($"prev {c} {d} -> {a} {b}");

    }
    
    static long SternBrocotCount(long d1, long d2) 
    {
        long result = 0;
        while (true)
        {
            var d = d1 + d2;
            if (d > limit) break;
            result ++;
            result += SternBrocotCount(d1, d);
            d1 = d;
        }
        return result;
    }
        
    public static int TotientFunction(int[] table, int n)
    {
        int prev = 0;
        int result = n;
        for (int k = n; k > 1; k /= prev)
        {
            int next = table[k];
            if (next != prev) result -= result / next;
            prev = next;
        }
        return result;
    }

    public static int[] PrimeFactorsUpTo(int n)
    {
        var factors = new int[n + 1];

        for (int i = 2; i <= n; i += 2)
            factors[i] = 2;

        var sqrt = (int)Math.Sqrt(n);
        for (int i = 3; i <= sqrt; i += 2)
        {
            if (factors[i] != 0) continue;
            for (int j = i * i; j <= n; j += i + i)
            {
                if (factors[j] == 0)
                    factors[j] = i;
            }
        }

        for (int i = 3; i <= n; i += 2)
        {
            if (factors[i] == 0)
                factors[i] = i;
        }

        return factors;
    }
  
    

    static long Gcd(long a, long b)
    {
        if (a==0) return b;
        return Gcd(b%a, a);
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