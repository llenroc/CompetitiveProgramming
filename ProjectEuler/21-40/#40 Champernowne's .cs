using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        
        var input = new StreamReader(Console.OpenStandardInput());
        var output = new StreamWriter(Console.OpenStandardOutput());
        int t = int.Parse(input.ReadLine());
        
        var digits = new int [7];
        while (t-->0)
        {
            var arr = Array.ConvertAll(input.ReadLine().Split(), long.Parse);            
        
            long prod = 1;
            for (int i=0; i<7; i++)
                prod *= FindNthDigit(arr[i]);
            output.WriteLine(prod);        
        }
        output.Flush();
    }
    
    public static int FindNthDigit(long n)
    {
        if (n == 0)
            return 0;

        int digits = 1;
        long factor = 1;
        n--;
        while (true)
        {
            var addend = 9 * digits * factor;
            if (n < addend) break;
            n -= addend;
            factor *= 10;
            digits++;
        }

        var mod = n % digits;
        n /= digits;
        n += factor;
        while (mod + 1 < digits)
        {
            mod++;
            n /= 10;
        }

        return (int)(n%10);
    }
}