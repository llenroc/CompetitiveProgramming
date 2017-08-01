using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        int t = 1; //int.Parse(Console.ReadLine());

        var composites = new bool[1000001];
        composites[1] = true;
        composites[0] = true;
        
        for (int i=2; i*i<composites.Length; i+=i!=2?2:1)
            for (long j=i*i; j<composites.Length;j+=i)
                composites[j] = true;

        //for (int i=1; i<100; i++)
        //    Console.Error.WriteLine($"{i} {composites[i]}");
        
        while (t-->0)
        {
            long n = long.Parse(Console.ReadLine());        

            long sum = 0;
            
            for (long i=n-1; i>=11; i--)
            {
                if (composites[i]) continue;

                bool good = true;
                for (long k = 10; k<i; k *= 10)
                {
                    if (composites[i%k] || composites[i/k]) { good = false;  break; }
                }

                if (good) {
                    Console.Error.WriteLine($"{i}");
                    sum += i;
                }
            }
        
            Console.WriteLine(sum);
        }
    }
}