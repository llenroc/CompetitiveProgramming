using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    
    static void Main(String[] args) {
        int[] lpf = new int[50000];
        
        for (int i=2; i<lpf.Length; i++)
            for (int j=i; j<lpf.Length; j+=i)
                if (lpf[j]==0) lpf[j] = i;
        
        int t = int.Parse(Console.ReadLine());
        for (int ti=0; ti<t; ti++)
        {
            int n = int.Parse(Console.ReadLine());
            for (int i=2; true; i++)
            {
                long tn = i*1L*(i+1)/2;
                var fac1 = i;
                var fac2 = i+1;
                if ((i&1)==0) fac1/=2; else fac2/=2;
                
                long divisors = 1;
                while (fac1 != 1 || fac2 != 1)
                {
                    int m = 0;
                    int p = fac1==1 ? lpf[fac2] : fac2==1 ? lpf[fac1] : Math.Min(lpf[fac1], lpf[fac2]);
                    while (lpf[fac1] == p)
                    {
                        m++;
                        fac1 /= p;
                    }
                    while (lpf[fac2] == p)
                    {
                        m++;
                        fac2 /= p;
                    }
                    divisors *= (m+1);
                }
                
                if (divisors>n)
                {
                    Console.WriteLine(tn);
                    break;
                }
            }
        }
    }
}