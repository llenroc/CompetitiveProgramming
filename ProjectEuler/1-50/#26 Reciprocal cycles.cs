using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        int tc = int.Parse(Console.ReadLine());
        
        int[] len = new int[10001];// int[10001];
        for (int i=1;i<len.Length; i++)
            len[i] = RepeatingCycle(i);
        
        int d = 1;
        int bestlen = len[1];
        for (int i=2; i<len.Length; i++)
        {
            if (len[i] > bestlen)
            {
                bestlen = len[i];
                len[i] = d;      
                d = i;
            }
            else
            {
                len[i] = d;
            }
        }
        
        while (tc-->0)
        {
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine(len[n]);
        }
    
    }
    
    static int RepeatingCycle(int d)
    {
        int num = 1;
        int den = d;
        
        var dict = new Dictionary<int, int>();
        
        int pos = 0;
        while (!dict.ContainsKey(num))
        {
            dict[num] = pos++;
            num = 10*num%d;
        }
        int result = pos - dict[num];
        
        if (result == 1 && num == 0) result = 0;
        //Console.Error.WriteLine($"{d} -> {result}");
        return result;
    }
}