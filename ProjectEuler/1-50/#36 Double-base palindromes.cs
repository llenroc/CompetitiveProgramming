using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        var array = Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        int n = array[0];
        int k = array[1];
        
        long sum = 0;
        for (int i=1; i<=n; i++)
        {
            if (IsPalindrome(i,10) && IsPalindrome(i,k))
            {
                sum += i;
            }
        }
        
        Console.WriteLine(sum);
    }

    static List<int> table = new List<int>();

    static bool IsPalindrome(int n, int k)
    {
        table.Clear();
        while (n>0)
        {
            table.Add(n % k);
            n /= k;
        }
        
        
        int left = 0;
        int right = table.Count-1;
        while (left<right)
        {
            if (table[left] != table[right]) return false;
            left++;
            right--;
        }
        
        return true;
    }
}