using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        
        var input = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        int n = input[0];
        int k = input[1];
    
        int full = (1<<k)-1<<1;
        Console.Error.WriteLine(full.ToString("X"));
        for (int i=2; i<n; i++)
        {
            int mask = 0;
            int j=1;
            while (mask != full)
            {
                int m = Mask(i*j);
                if (m==0 || (m&mask)!=0 || (m&~full)!=0) break;
                mask |= m;
                j++;
            }
            if (mask == full)
                Console.WriteLine(i);
        }
    }
    
    static int Mask(int n)
    {
        int mask = 0;
        while (n>0)
        {
            int d = n%10;
            n /= 10;
            if ((mask & 1<<d) != 0) return 0;
            mask |= 1<<d;
        }
        return mask;
    }
    
    
}