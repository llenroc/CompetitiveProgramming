using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        
        var array = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        
        var n = array[0];
        var k = array[1];

        var dict = new Dictionary<string, List<long>>();
        for (long i=1; i<n; i++)
        {
            var cube = i*i*i;
            var code = Code(cube);
            if (dict.ContainsKey(code)==false) dict.Add(code, new List<long>());
            dict[code].Add(cube);
        }

       /* foreach(var v in dict)
        {
            if (v.Value.Count==0) continue;
            Console.Error.WriteLine($"{v.Key} "+string.Join(" ", v.Value));
                
        }*/
        
        foreach(var v in dict.Values.Where(x=>x.Count==k).OrderBy(x=>x[0]).Select(x=>x[0]))
            Console.WriteLine(v);
        
    }
    
    static string Code(long n)
    {
        var s = n.ToString().ToCharArray();
        Array.Sort(s);
        return new string(s);
    }
}