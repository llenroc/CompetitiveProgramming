using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static int[] fact = {1,1,2,6,24,120,720,5040,40320,362880};
    static int[] chain = new int[1000001];
    static int[] factsum = new int[1000001];
    static void Main(String[] args) {

        for (int i=0; i<factsum.Length; i++)
            factsum[i] = FactSum(i);
        
        chain[169]=chain[363601]=chain[1454]=3;
        chain[871]=chain[45361]=2;
        chain[872]=chain[45362]=2;
        chain[40585]=1;
        chain[0]=2;
        
        for (int i=0; i<100; i++)
            Console.Error.WriteLine($"chain[{i}] = {Dfs(i)}");    
        
        int t = int.Parse(Console.ReadLine());
        while (t-->0)
        {
            var array = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
            var n = array[0];
            var l = array[1];
            
            var list = new List<int>();
            for (int i=0; i<=n; i++)
                if (Dfs(i)==l)
                    list.Add(i);
            
            Console.WriteLine(list.Count==0 ? "-1" : string.Join(" ", list));
        }
    }
    
    static HashSet<int> seen = new HashSet<int>();
    
    static int Dfs(int n)
    {
        if (n<chain.Length && chain[n]!=0) return chain[n];
        if (seen.Contains(n)) return 0;
        var fs = n<factsum.Length ? factsum[n] : FactSum(n);
        seen.Add(n);
        var result = Dfs(fs)+1;
        seen.Remove(n);
        if (n<chain.Length) chain[n]=result;
        return result;
    }
    
    static int FactSum(int d)
    {
        if (d==0) return 1;
        int sum = 0;
        for (; d>0; d/=10)
            sum += fact[d%10];
        return sum;
    }
    
}