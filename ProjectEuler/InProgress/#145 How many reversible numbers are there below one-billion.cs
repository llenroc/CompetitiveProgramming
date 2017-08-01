using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        
        int t = int.Parse(Console.ReadLine());
        
        var list = new List<Query>();
        for (int i=0; i<t; i++)
            list.Add(new Query { I=i, N=long.Parse(Console.ReadLine())});

        list.Sort((a,b)=>a.N.CompareTo(b.N));

        var table = new BitArray((int)list[list.Count-1].N+1);
        
        for (int i=1; i<table.Length; i++)
        {
            if (table[i] || i%10==0) continue;
            var r = Reverse(i);
            if (CheckOdd(r+i))
            {
                table[i] = true;
                if (r<table.Length) table[(int)r] = true;
            }
        }

        int[] presum = new int[table.Length];
        int sum = 0;
        for (int i=1; i<table.Length; i++)
        {
            presum[i] = sum += (table[i] ? 1 : 0);
        }
        
        
        foreach(var q in list)
        {
            q.A = presum[q.N-1];
        }        
        
        list.Sort((a,b)=>a.I.CompareTo(b.I));
        foreach(var a in list)
            Console.WriteLine(a.A);
    }
    
    public static long Reverse(long n)
    {
        long result = 0;
        while (n>0)
        {
            result = result * 10 + n%10;
            n/=10;
        }
        return result;
    }
       
    public static bool CheckOdd(long n)
    {
        while (n>0)
        {
            if ((n & 1) == 0) return false;
            n/=10;
        }
        return true;
    }
    
    public class Query
    {
        public int I;
        public long N;
        public long A;
    }
}