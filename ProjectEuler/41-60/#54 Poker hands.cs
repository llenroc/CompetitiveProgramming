using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        
        int t = int.Parse(Console.ReadLine());
        while (t-->0)
        {
            var array = Console.ReadLine().Split();
            var first = array.Take(5).OrderBy(x=>Value(x)).ToArray();
            var second = array.Skip(5).OrderBy(x=>Value(x)).ToArray();
            var f = RankIt(first);
            var s = RankIt(second);
            
            Console.Error.WriteLine($"{f:x016} {s:x016}");
            
            Console.WriteLine(f>s?"Player 1" : "Player 2");
        }
    }
    
    public static int Value(string s)
    {
        switch(s[0])
        {
            case 'A': return 14;
            case 'K': return 13;
            case 'Q': return 12;
            case 'J': return 11;
            case 'T':
            case '1': return 10;
            default: return s[0] - '0';
        }
    }
    
    public static int Suit(string s)
    {
        switch(s[s.Length-1])
        {
            case 'D': return 0;
            case 'C': return 1;
            case 'S': return 2;
            default:
            return 3;
        }
    }

    public static long RankIt(string[] array)
    {
        var vFlags = 0;
        var sFlags = 0;
        var pairFlags = 0;
        var tripleFlags = 0;
        var fourFlags = 0;
        var fiveFlags = 0;
        
        foreach(var s in array)
        {
            var v = Value(s);
            var suit = Suit(s);
            fiveFlags |= fourFlags & 1<<v;
            fourFlags |= tripleFlags & 1<<v;
            tripleFlags |= pairFlags & 1<<v;
            pairFlags |= vFlags & 1<<v;
            vFlags |= 1<<v;
            sFlags |= 1<<suit;
        }

        if (vFlags == (1<<14|1<<2|1<<3|1<<4|1<<5)) vFlags = (1<<1|1<<2|1<<3|1<<4|1<<5);

        var tmp = vFlags;
        while (tmp>0 && (tmp&1)==0) tmp >>= 1;
        
        bool straight = tmp == (1<<5)-1;
        bool flush = (sFlags & sFlags-1) == 0 ;
            
        long rank = 0;
        
        if (pairFlags != 0)
        {
            rank = Pair*Shift + pairFlags;
            
            if ((pairFlags & pairFlags-1) > 0)
                rank = PairTwo*Shift + pairFlags;
        }
        
        if (tripleFlags != 0)
            rank = Three*Shift + tripleFlags;
        
        if (straight)
            rank = Straight*Shift + vFlags;
        
        if ( flush )
            rank = Flush*Shift + tripleFlags;
        
        if (tripleFlags != 0 && (pairFlags & ~tripleFlags) != 0)
            rank = FullHouse*Shift + tripleFlags;
        
        if (fourFlags != 0)
            rank = (flush?FourFlush : Four)*Shift + fourFlags;
        
        if (straight && flush)
        {
            rank = StraightFlush * Shift + vFlags;
            
            if ((vFlags & (1<<14|1<<13))==(1<<14|1<<13))
                rank = RoyalFlush * Shift;
        }
        
        for (int i=array.Length-1; i>=0; i--)
            rank = rank * 16 + Value(array[i]);
        
        return rank;
    }
    
    
    const int Shift = 1<<24;
    const int HighCard = 0;
    const int Pair = 1;
    const int PairTwo = 2;
    const int Three = 3;
    const int Straight = 4;
    const int Flush = 5;
    const int FullHouse = 6;
    const int Four = 7;
    const int FourFlush = 8;
    const int StraightFlush = 9;
    const int RoyalFlush = 10;
}