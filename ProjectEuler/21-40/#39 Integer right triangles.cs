using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        int t = int.Parse(Console.ReadLine());
        var ns = Enumerable.Range(0,t).Select(x=>int.Parse(Console.ReadLine())).ToArray();
        var max = ns.Max(); 

        int [] table = new int[max+1];
        for (int m=1; 2*m*m<table.Length;m++)
        {
            for (int n=1; n<m; n++)
            {
                if ( (n&1)==1 && (m&1)== 1 || Gcd(m,n) != 1) continue;
                
                var a = m*m - n*n;
                var b = 2*m*n;
                var c = m*m + n*n;
                var per = a+b+c;
                if (c>=table.Length) break;

                for (int j=per; j<table.Length; j+=per)
                    table[j] ++;                    
            }
        }
        
        int pmax = int.MinValue;
        int p = 0;
        for (int i=0; i<table.Length; i++)
        {
            //Console.Error.WriteLine($"#{i} {table[i]}");
            if (table[i]>pmax)
            {
                pmax = table[i];
                p = i;
            }
            table[i] = p;
        }
        
        for (int i=0; i<t; i++)
        {
            Console.WriteLine( table[ns[i]] );                  
        }
        
    }
    
    static long Gcd(long a, long b)
    {
        return (a == 0) ? b : Gcd(b % a, a);
    }
}