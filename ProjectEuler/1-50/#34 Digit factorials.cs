using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        
        int[] fact = new int[10];
        fact[0] = 1;
        for (int i=1; i<fact.Length; i++)
            fact[i] = i * fact[i-1];
        
        int n = int.Parse(Console.ReadLine());
        long sum = 0;
        for (int i=10; i<n; i++)
        {
            long sum2 = 0;
            for (int k=i; k>0; k/=10)
                sum2 += fact[k%10];
            if (sum2 % i == 0)
                sum += i;
        }
        
        Console.WriteLine(sum);
    }
}