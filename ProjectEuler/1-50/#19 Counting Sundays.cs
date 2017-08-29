using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {
    static void Main(String[] args) {
        
        int t = int.Parse(Console.ReadLine());
        Console.Error.WriteLine(GregorianDay(2017, 7, 2 ));
        
        while (t-->0)
        {
            var fields = Console.ReadLine().Split().Select(x=>(int)(long.Parse(x)%1200)).ToArray();
            var fields2 = Console.ReadLine().Split().Select(x=>(int)(long.Parse(x)%1200)).ToArray();

            var y1 = fields[0];
            var m1 = fields[1];
            var d1 = fields[2];
            var y2 = fields2[0];
            var m2 = fields2[1];
            var d2 = fields2[2];
            
            if (y2 < y1) y2 += 1200;
            int count = Compute(y1, m1, d1, y2, m2, d2);
            Console.WriteLine(count);
        }
    }
    
    static int Compute(int y1, int m1, int d1, int y2, int m2, int d2)
    {
        int count = 0;
        for (int year = y1; year <= y2; year++)
        {
            for (int month = 1; month <= 12; month ++)
            {
                if ( year==y1 && (month < m1 || month == m1 && 1 < d1 )) continue;
                if ( year==y2 && month > m2) continue;

                var day = GregorianDay(year, month, 1);
                if (day == 0) count++;
            }
        }
        return count;
    }
    
    static int GregorianDay(int year, int month, int d)
    {
        int m = month;
        int y = year;
        if (m < 3)  { 
            y--; 
            m += 12;
        }
        
        int k = year % 100;
        int j = year/100;
        //var h = (d + 13 * (m+1) / 5 + k + k/4 + j/4 + 5*j) % 7; // Doesn't work
        var h = (d + 13 * (m+1) / 5 + y + y/4 - y/100 + y/400) % 7;
        //var julian = (d + 13 * (m+1) / 5 + y + y/4 + 5) % 7;
        h = (int)(h+6)%7;
        
        /*
        var date = new DateTime(year, month, d);
        int h2 = (int)(date.DayOfWeek);
        if (h2 != h)
            Console.Error.WriteLine($"Mismatch({date}) {h} {h2}");
          */
        
        return h;
    }
}