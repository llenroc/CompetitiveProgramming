using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        int t = int.Parse(Console.ReadLine());
        var ns = Enumerable.Range(0,t).Select(x=>long.Parse(Console.ReadLine())).ToArray();
        var max = 1000000L * 1000000L * 1000000L; 

        var table = new List<long>();
        
        /*for (int i=2; ; i++)
        {
            var x = A(i);
            if (x>=max) break;
            Console.Error.WriteLine(x);
            table.Add(x);
        }*/
        
        if (true)
        table.AddRange(new long[] {
            12,
            70,
            408,
            2378,
            13860,
            80782,
            470832,
            2744210,
            15994428,
            93222358,
            543339720,
            3166815962,
            18457556052,
            107578520350,
            627013566048,
            3654502875938,
            21300003689580,
            124145519261542,
            723573111879672,
            4217293152016490,
            24580185800219268,
            143263821649299118,
            835002744095575440,
            //123456789012345678
        }); 
        
        /*
        for (long m=1; 2*m*m<max;m++)
        {
            for (long n=1; n<m; n++)
            {
                if ( (n&1)==1 && (m&1)== 1 || Gcd(m,n) != 1) continue;
                
                var a = m*m - n*n;
                var b = 2*m*n;
                var c = m*m + n*n;
                var per = a+b+c;
                if (per>=max) break;
                if (a == b || c % Math.Abs(a-b) != 0) continue;

                Console.Error.WriteLine(per);
                table.Add(per);
            }
        }*/

        for (int i=0; i<t; i++)
        {
            var n = ns[i];
            
            long count = 0;
            foreach(var v in table)
            {
                if (v < n) count += (n-1)/v; 
            }
            Console.WriteLine( count );                  
        }
        
    }
    
    // https://oeis.org/A001542
    static long A(long n)
    {
        long prev = 0;
        long curr = 2;
        
        while (n-->1)
        {
            var next = 6*curr - prev;
            prev = curr;
            curr = next;
        }
        
        return curr;
    }
    
    static long Gcd(long a, long b)
    {
        return (a == 0) ? b : Gcd(b % a, a);
    }
}