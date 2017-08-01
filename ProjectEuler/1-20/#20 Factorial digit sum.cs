using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

class Solution {
    static void Main(String[] args) {
        var t = int.Parse(Console.ReadLine());
        while (t-->0)
        {
            var n = BigInteger.Parse(Console.ReadLine());

            BigInteger num = 1;
            
            for (int i=1; i<=n; i++)
                num *= i;
            
            var sum = (BigInteger) 0;
            while (num>0)
            {
                sum += num %10;
                num /= 10;
            }
            
            Console.WriteLine(sum);
        }
        /* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */
    }
}