using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
class Solution {
    static void Main(String[] args) {
        int t = int.Parse(Console.ReadLine());
        var ns = Enumerable.Range(0,t).Select(x=>long.Parse(Console.ReadLine())).ToArray();
        var max = Math.Max(1000000L * 1000000L * 1000000L, ns.Max()); 

        /*
            8) 6 5 16
            30) 16 17 50
            112) 66 65 196
            418) 240 241 722
            1560) 902 901 2704
            5822) 3360 3361 10082
            21728) 12546 12545 37636
            81090) 46816 46817 140450
            302632) 174726 174725 524176
            1129438) 652080 652081 1956242
            4215120) 2433602 2433601 7300804
            15731042) 9082320 9082321 27246962
            58709048) 33895686 33895685 101687056
            219105150) 126500416 126500417 379501250
            817711552) 472105986 472105985 1416317956
            3051741058) 1761923520 1761923521 5285770562
            11389252680) 6575588102 6575588101 19726764304
            42505269662) 24540428880 24540428881 73621286642
            158631825968) 91586127426 91586127425 274758382276
            592022034210) 341804080816 341804080817 1025412242450
            2209456310872) 1275630195846 1275630195845 3826890587536
            8245803209278) 4760716702560 4760716702561 14282150107682
            30773756526240) 17767236614402 17767236614401 53301709843204
            114849222895682) 66308229755040 66308229755041 198924689265122
            428623135056488) 247465682405766 247465682405765 742397047217296
            1599643317330270) 923554499868016 923554499868017 2770663499604050
            5969950134264592) 3446752317066306 3446752317066305 10340256951198916
            22280157219728098) 12863454768397200 12863454768397201 38590364305191602
            83150678744647800) 48007066756522502 48007066756522501 144021200269567504
            310322557758863102) 179164812257692800 179164812257692801 537494436773078402
        */
        
        
        // an == 14an-1 - an-2 - 4 for triangles where single side is 1 unit shorter
        // an == 14an-1 - an-2 + 4 for triangles where single side is 1 unit longer
        
        var list = new List<BigInteger>{
        };
        
        var trial = new List<BigInteger>();
        //for (long x=1; x*x<=max; x++)
        BigInteger prev = 2;
        BigInteger xsav;
        for (BigInteger x=8; x<=max; xsav=x, x=4*x-prev, prev=xsav)
        {
            var x2 = x*x;
            var rad = 3*x2 + 4;
            var sqrt = Sqrt(rad);
            //Console.Error.WriteLine($"x={x} rad={rad} sqrt={sqrt}");
            if (sqrt*sqrt != rad) continue;
            
            trial.Clear();
            trial.Add((-8 + 2*sqrt));
            trial.Add((+8 + 2*sqrt));
            trial.Add((-8 - 2*sqrt));
            trial.Add((+8 - 2*sqrt));
           
            foreach(var tr in trial)
            {
                //Console.Error.WriteLine($"tr = {tr}");
                if (tr <= 0 || tr % 6 != 0) continue;
                var a = tr / 6;
                var area = a * x;
                if ((area % 4) != 0) continue;
                area /= 4;

                var b = 3*a*a+4+8*a == x2 ? a+1 : a-1;
                var per = a + 2 * b;
                Console.Error.WriteLine($"{x}) {a} {b} {per}");
                list.Add(per);
            }
            
            
        }
        
        foreach(var n in ns)
        {
            BigInteger per = 0;
            foreach(var p in list)
            {
                if (p<=n)
                    per+=p;
            }
            Console.WriteLine(per);
        }
        
        
    }
    
    	public static BigInteger Sqrt(BigInteger x)
		{
			if (x == 0 || x == 1)
				return x;

			BigInteger start = 1, end = x, ans = 1;
			while (start <= end)
			{
				var mid = (start + end)/2;
				var sqr = mid * mid;
				if (sqr == x)
					return mid;

				if (sqr < x)
				{
					start = mid + 1;
					ans = mid;
				}
				else
					end = mid - 1;
			}
			return ans;
		}
}