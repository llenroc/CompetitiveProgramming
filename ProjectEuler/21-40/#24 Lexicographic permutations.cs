using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class Solution {
    static void Main(String[] args) {
        int t = int.Parse(Console.ReadLine());
        while (t-->0)
        {
            int k = int.Parse(Console.ReadLine());
            Console.WriteLine(GetPermutation(13, k));
        }
    }

    public static string GetPermutation(int n, long k) {
        var list = new StringBuilder(n);
        for (int i=0; i<n; i++) list.Append((char)('a'+i));
        
        long perm = 1;
        for (int i=1; i<=n; i++) perm *= i;
        
        k--;
        k %= perm;
        int start = 0;
        for (; k > 0 && start < n; start++)
        {
            perm /= n-start;
            
            // if (k<perm) continue;
                
            int i = (int)(k/perm);
            k %= perm;
            
            var x = list[start+i];
            list.Remove(start+i,1);
            list.Insert(start, x);
        }
        
        return list.ToString();
    }

}

