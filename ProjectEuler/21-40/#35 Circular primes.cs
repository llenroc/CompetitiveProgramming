using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        int t = 1; //int.Parse(Console.ReadLine());

        var composites = new bool[1000001];
        
        for (int i=2; i*i<composites.Length; i+=i!=2?2:1)
            for (long j=i*i; j<composites.Length;j+=i)
                composites[j] = true;

        //for (int i=1; i<100; i++)
        //    Console.Error.WriteLine($"{i} {composites[i]}");
        
        while (t-->0)
        {
            long n = long.Parse(Console.ReadLine());        

            long sum = 0;
            
            for (long i=n; i>=2; i--)
            {
                if (composites[i]) continue;

                bool good = true;
                long factor = (long)Math.Pow(10, (long)Math.Log(i,10));
                for (long k = (i/10)+(i%10)*factor; k!=i; k = (k/10)+(k%10)*factor)
                {
                    if (k>composites.Length || composites[k]) { good = false;  break; }
                }

                if (good) {
                    //Console.Error.WriteLine($"{i}");
                    sum += i;
                }
            }
        
            Console.WriteLine(sum);
        }
    }
}