using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    
    static bool[] composites, seen;
    static int[] buffer;
    static Action<int> action;
    static void Main(String[] args) {

        composites = new bool[1000001];
        seen = new bool[1000001];
        composites[1] = true;
        composites[0] = true;
        
        for (int i=2; i*i<composites.Length; i+=i!=2?2:1)
            for (long j=i*i; j<composites.Length;j+=i)
                composites[j] = true;
        
        var array = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        var n = array[0];
        var k = array[1];
        
        var list = new List<int>();
        var results = new List<string>();
        
        action = x=>
        {
            if (composites[x] || seen[x]) return;
            //Console.Error.WriteLine("->" + x);
            seen[x] = true;
            list.Add(x);
        };
            
        buffer = new int[10];
        for (int i=2; i<n; i++)
        {
            if (composites[i] || seen[i]) continue;
            
            int j=0, num=i;
            while (num>0)
            {
                buffer[j++] = num%10;
                num/=10;
            }

            //Console.Error.WriteLine($"i={i} j={j}  " + string.Join(",", buffer));
            
            list.Clear();
            BacktrackPermute((1<<j) - 1 , 0, j);
            if (list.Count<k) continue;
            list.Sort();
            
            //Console.Error.WriteLine(string.Join(" ", list));
            for (j=0; j<list.Count; j++)
            {
                var v = list[j];
                if (v >= n) break;
                for (int j2=j+1; j2<list.Count; j2++)
                {
                    var v2 = list[j2];
                    int d = v2-v;
                    if (!list.Contains(v2+d)) continue;
                    if (k==4 && !list.Contains(v2+2*d)) continue;
                    
                    if (k==3)
                        results.Add("" + v + v2 + (v2+d));
                    else
                        results.Add("" + v + v2 + (v2+d) + (v2+2*d));
                }
            }
        }
        
        results.Sort((a,b)=>
                     {
                         if (a.Length != b.Length) return a.Length - b.Length;
                         return a.CompareTo(b);
                     });
        foreach(var r in results)
            Console.WriteLine(r);
    }
    
    
    static void BacktrackPermute(long available, int n, int depth)
    {
        if (depth == 0) action(n);
        else
        {
            depth--;
            for (int i=0, bit=1; i <= available; i++,bit<<=1)
                if ((available & bit) != 0)
                    BacktrackPermute(available & ~bit, n * 10 + buffer[i], depth);
        }
    }
    
}