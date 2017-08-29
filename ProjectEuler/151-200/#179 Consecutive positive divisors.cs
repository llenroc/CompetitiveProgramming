using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {

        var input = new StreamReader(Console.OpenStandardInput());
        var output = new StreamWriter(Console.OpenStandardOutput());
        int t = int.Parse(input.ReadLine());
              
        //var ns = Enumerable.Range(0,t).Select(x=>int.Parse(Console.ReadLine())).ToArray();
        //var max = ns.Max()+1;

        int[] sod = new int[10000001];
        
        for (int i=2; i<sod.Length; i++)
            for (int j=2*i; j<sod.Length; j+=i)
                sod[j]++;

        int sum = 0;
        for (int i=2; i+1<sod.Length; i++)
        {
            sum += sod[i]==sod[i+1] ? 1 : 0;
            sod[i] = sum;
        }
        
        for (int tt=0; tt<t; tt++)
        {
            int n = int.Parse(input.ReadLine());
            output.WriteLine(sod[n-1]);
        }
        output.Flush();
    }
}