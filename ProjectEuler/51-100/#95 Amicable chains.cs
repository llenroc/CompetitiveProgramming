using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        int n= int.Parse(Console.ReadLine());
        int[] sod = new int[n+1];
        
        for (int i=1; i<sod.Length; i++)
            for (int j=2*i; j<sod.Length; j+=i)
                sod[j] += i;
        
        int[] chainLength = new int[n+1];
        int[] visited = new int[n+1];
        
        for (int i=1; i<=n; i++)
        {
            if (visited[i]!=0) continue;

            int j=i;
            do 
            {
                visited[j] = i;
                j = sod[j];
            } while (j<=n && visited[j]==0);
            
            if (j<i || j>n || visited[j] != i) continue;
            
            int k=j;
            int len=1;
            while (sod[k] != j)
            {
                k = sod[k];
                len++;
            }
            
            k=j;
            for (int x=0; x<len; x++)
            {
                chainLength[k] = len; 
                k = sod[k];
            }
        }
        
        int best = 0;
        for (int i=1; i<=n; i++)
        {
            if (chainLength[i] > chainLength[best])
                best = i;
        }
        Console.WriteLine(best);
    }
}