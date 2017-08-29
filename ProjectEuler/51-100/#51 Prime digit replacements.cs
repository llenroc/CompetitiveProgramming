using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Solution {
    
    static BitArray composites;
    
    static void Main(String[] args) {
        
        composites = new BitArray(10000000);
        composites[1] = true;
        composites[0] = true;
        
        for (long i=2; i<composites.Length; i+=i!=2?2:1)
            if (!composites[(int)i])
                for (long j=i*i; j<composites.Length;j+=i)
                    composites[(int)j] = true;
        
        
        var array = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        var n = array[0];
        var k = array[1];
        var L = array[2];
        
        var start = Pow10(n-1);
        var end = Pow10(n);
        
        int[] counts = new int[10];
        var list = new List<int>();
        var best = "";
        
        for (int i=start+1; i<end; i+=2)
        {
            if (composites[i]) continue;
            
            // Handle counts
            for (int j=0; j<10; j++) counts[j] = 0;
            var z = i;
            while (z>0) { counts[z%10]++; z/=10; }

            //Console.Error.WriteLine($"#{i} -> {string.Join(",", counts)}");
            for (int j=0; j<10; j++)
                if (counts[j]>=k)
                {
                    //Console.Error.WriteLine($"Dfs({i}, 0, {k}, {counts[j]})");
                    Dfs(i, 0, k, counts[j], j, (num, mask)=>
                        {
                            list.Clear();
                            for (int index=0; index<10; index++)
                            {
                                var v = num + mask * index;
                                if (v>=start && v<composites.Length && !composites[v]) list.Add(v);
                            }
                            
                            //Console.Error.WriteLine($" -> {i} -> {list.Count} -> [{string.Join(",", list)}]");
                            if (list.Count>=L)
                            {
                                list.Sort();
                                var chosen = string.Join(" ", list.Take(L));
                                if (best=="" || string.CompareOrdinal(chosen, best)<0)
                                    best = chosen;
                            }
                        });
                }
            
        }
        
        Console.WriteLine(best);
    }

    static void Dfs(int num, int mask, int remaining, int total, int target, Action<int, int> action, int pos=1)
    {
        if (remaining == 0)
        {
            action(num, mask);
            return;
        }
        
        while (remaining<=total)
        {
            var d = num/pos%10;

            if (d == target)
            {
                total--;
                Dfs(num-d*pos, mask+pos, remaining-1, total, target, action, pos*10);
            }

            pos *= 10;
        }
    }
    
    public static int Pow10(int n)
    {
        int result = 1;
        while (n-->0)
            result *= 10;
        return result;
    }
}