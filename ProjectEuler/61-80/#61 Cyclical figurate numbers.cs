using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    
    static int[] properties;
    static List<int>[] map;
    static bool[] visited;
    
    static void Main(String[] args) {

        visited = new bool[10000];
        properties = new int[10000];
        map = new List<int>[100];
        
        Fill(properties, 3, x=>x*(x+1)/2);
        Fill(properties, 4, x=>x*x);
        Fill(properties, 5, x=>x*(3*x-1)/2);
        Fill(properties, 6, x=>x*(2*x-1));
        Fill(properties, 7, x=>x*(5*x-3)/2);
        Fill(properties, 8, x=>x*(3*x-2));
        
        Console.ReadLine();
        
        for (int i=0; i<100; i++)
            map[i] = new List<int>();
        
        for (int i=1000; i<10000; i++)
        {
            if (properties[i]==0) continue;
            map[i/100].Add(i);
        }
        
        //for (int i=0; i<100; i++) Console.Error.WriteLine(i + ") " + string.Join(",", map[i]));

        var array = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        
        int mask = 0;
        foreach(var x in array)
            mask |= 1<<x;             
             
        var hashset = new HashSet<int>();
        for (int i=1000; i<10000; i++)
        {
            if (properties[i]==0) continue;
            Dfs(i, i, mask, 0, x=>hashset.Add(x));
        }
        
        foreach(var x in hashset.OrderBy(y=>y))
        {
            Console.WriteLine(x);
        }
            
    }
    
    
    static void Dfs(int start, int target, int mask, int sum, Action<int> action, int depth=0)
    {
        visited[target] = true;
        var m = properties[target] & mask;
        sum += target;
        var indent = new string(' ',depth);
        Console.Error.WriteLine($"{indent}-> Dfs({start}, {target}, {mask:x}, {sum}) m={m:x}");
        while ( m > 0)
        {
            var bit = m & -m;
            m -= bit;

            if (mask==bit)
            {
                if (target%100 == start/100) action(sum);
                break;
            }

            var next = map[target%100];
            //if (next.Count>0) Console.Error.WriteLine(indent + string.Join(",", map[m%100]));
            foreach(var v in next)
            {
                if (visited[v] || v<start) continue;
                Dfs(start, v, mask & ~bit, sum, action, depth+1);                
            }
        }
        visited[target] = false;
    }
    
    static void Fill(int[] properties, int n, Func<int, int> func)
    {
        for (int i=1; ; i++)
        {
            var v = func(i);
            if (v>=properties.Length) break;
            properties[v] |= 1<<n;
        }        
    }
    
}