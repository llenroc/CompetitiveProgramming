using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        int n = int.Parse(Console.ReadLine());
        
        var array = Enumerable.Range(0,n).Select(x=>Console.ReadLine()).OrderBy(x=>x).ToArray();
        
        int t = int.Parse(Console.ReadLine());
        while (t-->0)
        {
            var name = Console.ReadLine();
            int index = Array.BinarySearch(array, name)+1;
            int result = 0;
            foreach(var ch in name)
            {
                result += char.ToUpper(ch)-'A'+1;
            }
            Console.Error.WriteLine($"{result} {index}");
            Console.WriteLine(result * index);
        }
        
        
    }
}