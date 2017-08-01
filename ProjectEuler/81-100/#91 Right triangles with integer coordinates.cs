using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        int size = int.Parse(Console.ReadLine());
        int result = size*size*3;
        for (int x = 1; x <= size; x++) {
            for (int y = 1; y <= size; y++) {
                int fact = gcd(x, y);
                result += Math.Min(y*fact /x, (size - x)*fact /y) * 2;
            }
        }
        Console.WriteLine(result);
    }
    
    static int gcd(int a, int b)
    {
        if (a==0) return b;
        return gcd(b%a, a);
    }
}