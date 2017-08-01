using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        
        int n = int.Parse(Console.ReadLine());
        int[] digits = new int[6];
        int count = 0;
        
        for (int i=0, sum1=0; i<=9; i++, sum1=Pow(i,n))
        for (int i2=i, sum2=sum1; i2<=9; i2++, sum2=Pow(i2,n))
        for (int i3=i2, sum3=sum2; i3<=9; i3++, sum3=Pow(i3,n))
        for (int i4=i3, sum4=sum3; i4<=9; i4++, sum4=Pow(i4,n))
        for (int i5=i4, sum5=sum4; i5<=9; i5++, sum5=Pow(i5,n))
        for (int i6=i5, sum6=sum5; i6<=9; i6++, sum6=Pow(i6,n))
        {
            int sum = sum1 + sum2 + sum3 + sum4 + sum5 + sum6;
            if (sum<2 ||sum >= 1000000) continue;
            digits[0] = sum % 10;
            digits[1] = sum / 10 % 10;
            digits[2] = sum / 100 % 10;
            digits[3] = sum / 1000 % 10;
            digits[4] = sum / 10000 % 10;
            digits[5] = sum / 100000 % 10;
            Array.Sort(digits);
            if (digits[0]==i && digits[1]==i2 && digits[2]==i3 && digits[3]==i4 && digits[4]==i5 && digits[5]==i6)
            {
                Console.Error.WriteLine(sum);
                count+=sum;
            }
        }
        
        Console.WriteLine(count);
    }
    
    
    static int Pow(int n, int p)
    {
        int result = n;
        while (p-- >1) result *= n;
        return result;
    }
}