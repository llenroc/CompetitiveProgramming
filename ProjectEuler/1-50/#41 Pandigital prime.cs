using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        int t = int.Parse(Console.ReadLine());

        var list = new List<int>();
        
        new PermutationIterator(4, x=>list.Add(x));
        new PermutationIterator(7, x=>list.Add(x));
        list.Sort();
        list.RemoveAll(x=>!FermatProbablyPrime(x));
        
        Console.Error.WriteLine(string.Join(" ", list));
        
        while (t-->0)
        {
            long n = long.Parse(Console.ReadLine());        
            long found = -1;
            
            
            if (n < list[0]) 
            {
            }
            else if(n >= list[list.Count-1])
                found = list[list.Count-1];
            else
            {                
                found = list.BinarySearch((int)n);
                found = found < 0 ? list[(int)~found-1] : list[(int)found];
            }
            
            Console.WriteLine(found);
        }
    }
    
		public class PermutationIterator
		{
			private readonly Action<int> _action;
			private readonly int _nums;

			public PermutationIterator(int nums, Action<int> action)
			{
				_action = action;
				_nums = nums;
				BacktrackPermute();
			}

			private void BacktrackPermute(long used = 0, int n=0, int depth=0)
			{
				if (depth == _nums) _action(n);
				else
				{
					for (int i = 1; i <= _nums; i++)
						if ((used << ~i) >= 0) 
                            BacktrackPermute(used | 1L<<i, n*10+i, depth+1);
				}
			}
		}
        
   		public static int Pow(int n, int p, int mod)
		{
			long result = 1;
			long b = n;
			while (p > 0)
			{
				if ((p & 1) == 1) result = result * b % mod;
				p >>= 1;
				b = b * b % mod;
			}
			return (int)result;
		}

   		static int[] PrimesBetween7And61 = new int[] { 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61 };

		public static bool FermatProbablyPrime(int n)
		{
			return n > 2 && MayBePrime(n)
				? Pow(2,n-1,n) == 1
				: n == 2;
		}
    
    	public static bool MayBePrime(long n)
		{
			const int PrimeFilter235 = 0
									   | 1 << 1 | 1 << 7
									   | 1 << 11 | 1 << 13 | 1 << 17 | 1 << 19
									   | 1 << 23 | 1 << 29;

			if ((PrimeFilter235 & 1 << (int)(n % 30)) == 0)
				return false;

			// Quick test
			foreach (var v in PrimesBetween7And61)
				if (n % v == 0)
					return false;

			return true;
		}
		
    
}