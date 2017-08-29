using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    
    static bool[] valid;
    
    static void Main(String[] args) {
        
        string chars = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ() ;:,.'?-!";
        valid = new bool[256];
        foreach(var ch in chars)
            valid[ch] = true;
        
        Console.ReadLine();
        var data =  Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
        
        var key = new char[3];
        
        for (int i=0; i<3; i++)
        for (char ch = 'a'; ch<='z'; ch++)
        {
            if (Check(data, ch, i))
            {
                key[i] = ch;
                break;
            }
        }
        
        var keyString = new string(key);
        Console.WriteLine(keyString);
    }
    
    public static bool Check(int[] data, int xor, int offset)
    {
        for (int i=offset; i<data.Length; i+=3)
            if (!valid[data[i] ^ xor]) return false;
        return true;
    }
}