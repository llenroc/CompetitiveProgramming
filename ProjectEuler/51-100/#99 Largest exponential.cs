using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        /* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */
        
        int n = int.Parse(Console.ReadLine());
        var array = Enumerable.Range(0,n).Select(x=>Console.ReadLine())
            .Select(x=>x.Split().Select(int.Parse).ToList())
            .Select(x=>new Pair{b=x[0], e=x[1]})
            .ToList();
        var k = int.Parse(Console.ReadLine());
        array.Sort((a,b)=>a.n.CompareTo(b.n));
        Console.WriteLine(array[k-1].b + " " + array[k-1].e);
        
    }
    
    
    public class Pair
    {
        public int b;
        public int e;
        private double _n;
        public double n 
        {
            get {
            if (_n == 0)
                _n = e * Math.Log(b);
            return _n;
            }
        }
    }
}