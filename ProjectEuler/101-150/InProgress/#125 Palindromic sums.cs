using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        
        int t = int.Parse(Console.ReadLine());
        while (t-->0)
        {
            var array = Array.ConvertAll(Console.ReadLine().Split(), ulong.Parse);
            var n = array[0];
            var d = array[1];
                  
            var set = new HashSet<ulong>();
            ulong result = 0;
            for (ulong i=1; ; i++)
            {
                ulong sum = i*i;
                if (sum>=n) break;
                
                for (ulong j=i+d; ; j+=d)
                {
                    var jj = j*j;
                    if (jj>=n || sum+jj>=n) break;
                    sum+=jj;
                    if (Reverse(sum)==sum && set.Add(sum))
                        result += sum;
                }
            }
            
            Console.WriteLine(result);
        }
    }
    
    static ulong Reverse(ulong n)
    {
        ulong result = 0;
        while (n>0)
        {
            result = result * 10 + n%10;
            n/=10;
        }
        return result;
    }
    
    
}