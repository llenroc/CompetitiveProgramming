using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        var input = new StreamReader(Console.OpenStandardInput());
        var output = new StreamWriter(Console.OpenStandardOutput());
        var array = Array.ConvertAll(input.ReadLine().Split(), int.Parse);
        var n = array[0];
        var k = array[1];
        var lines = new long[n][];
        var sums = new long[n][];
        var sortedset = new List<long>();
        for (int i=0; i<n; i++)
        {
            lines[i] = Array.ConvertAll(input.ReadLine().Split(), long.Parse);
            sums[i] = (long[]) lines[i].Clone();
            foreach(var v in lines[i])
                sortedset.Add(v);
            
            long sum = 0;
            for (int j=0; j<=i; j++)
                sums[i][j] = sum = sum + sums[i][j];
        }
        
        long max = long.MaxValue;

        if (k<sortedset.Count)
        {
            sortedset.Sort();
            sortedset.RemoveRange(k, sortedset.Count-k);
            max = sortedset[sortedset.Count-1];
        }
        //Console.Error.WriteLine(string.Join(" ", sortedset));

        for (int d = 1; d<n; d++)
        {
            //Console.Error.WriteLine($"Depth #{d+1}");
            for (int i=0; i+d<n; i++)
            {
                var line = lines[i];
                for (int j=0; j<line.Length; j++)
                {
                    var v = line[j] + sums[i+d][j+d] - (j>0 ? sums[i+d][j-1] : 0);
                    //Console.Error.WriteLine($"[{i}][{j}] = {v}");
                    line[j] = v;
                    if (v<max)
                    {
                        sortedset.Add(v);
                        if (k+k<sortedset.Count)
                        {
                            sortedset.Sort();
                            sortedset.RemoveRange(k, sortedset.Count-k);
                            max = sortedset[sortedset.Count-1];
                        }
                    }
                }
            }

            //Console.Error.WriteLine(string.Join(" ", sortedset));
        }

        sortedset.Sort();
        sortedset.RemoveRange(k, sortedset.Count-k);
        foreach(var v in sortedset)
            output.WriteLine(v);
        output.Flush();
    }
}