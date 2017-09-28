using System;
using System.Collections.Generic;
using System.IO;
using static System.Math;
class Solution {
    static void Main(String[] args) {
        var n = double.Parse(Console.ReadLine());
        Console.WriteLine((1678.0*Pow(n,3) + 3117*Pow(n,2) + 88*n - n%2*345 - n%3*320 - n%4*90 - 
          (n*n*n - n*n + n)%5 * 288)/240);
    }
}