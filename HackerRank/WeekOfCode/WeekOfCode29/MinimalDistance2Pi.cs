﻿
namespace HackerRank.WeekOfCode29.MinimalDistance2Pi
{

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Numerics;
	using System.Threading;

	using static System.Math;
	using static System.Console;
	using static HackerRankUtils;
	using BigInt = System.Numerics.BigInteger;

	public class Solution
	{
		#region Variables
		const string PiString = "1415926535897932384626433832795028841971693993751";
		static BigInt PiDenominator = BigInt.Pow(10, PiString.Length);
		static BigInt PiNumerator = BigInt.Parse(PiString);
		static BigInt PiNumerator3 = PiNumerator + 3 * PiDenominator;

		long bestN = int.MaxValue;
		long bestD = 1;
		BigInt diffN = int.MaxValue;
		BigInt diffD = 1;

		long min;
		long max;
		#endregion

		static decimal pi = 3.14159265358979323846264338327950m;

		static long getNum(long den)
		{
			return (long)(pi * den + 0.5m); // floor(x + 0.5) == round(x) for x > 0
		}

		static decimal distToPi(long den)
		{
			long num = getNum(den);
			decimal dist = pi - (decimal)num / den;
			if (dist < 0)
			{
				dist = -dist;
			}
			return dist;
		}

		public class Item
		{
			public long den;
			public Decimal dist;
		}

		public static void Main()
		{
			Stack<Item> stack = new Stack<Item>();
			SortedSet<long> diffs = new SortedSet<long>();
			var START = (long)1e14;
			long LEN = (long)1e7;
			for (long den = START; den < START + LEN; den++)
			{
				Decimal dist = distToPi(den);
				while (stack.Count != 0 && dist < stack.Peek().dist)
				{
					long diff = den - stack.Peek().den;
					if (!diffs.Contains(diff))
						diffs.Add(diff);
					stack.Pop();
				}
				stack.Push(new Item { den = den, dist = dist });
			}

			WriteLine("diffs({0}):", diffs.Count);
			foreach (long diff in diffs)
				WriteLine(" {0}", diff);
		}

		static void Main2()
		{

			var tokens_min = ReadLine().Split();
			var min = long.Parse(tokens_min[0]);
			var max = long.Parse(tokens_min[1]);

			var solution = new Solution(min, max);
			LaunchTimer(solution.ReportAnswer);
			solution.Solve();
			solution.ReportAnswer();
		}

		public Solution(long min, long max)
		{
			this.min = min;
			this.max = max;
		}


		public bool CheckPrecomputed()
		{
			int left = 0;
			int right = Fractions.GetLength(0) - 1;

			while (left <= right)
			{
				int mid = (left + right) / 2;
				if (max >= Fractions[mid, 1])
					left = mid + 1;
				else
					right = mid - 1;
			}

			while (--left >= 0)
			{
				var n = Fractions[left, 0];
				var d = Fractions[left, 1];
				var t = Fractions[left, 2];

				var d2 = (min + d - 1) / d * d;
				var n2 = n * (d2 / d);

				if (d2 <= max)
				{
					TryOut(n2, d2);
					if (t != 2)
						return true;
					continue;
				}

				d2 -= d;
				n2 -= n;

				// loop again because d is larger than min
				if (d2 <= 0)
					continue;

				var sol = new Solution(min - d2, max - d2);
				bool good = sol.CheckPrecomputed();
				if (good)
				{
					n2 += sol.bestN;
					d2 += sol.bestD;
					TryOut(n2, d2);
				}

				if (t != 2)
					return good;
			}

			return false;
		}

		public bool BruteForce(bool check = true)
		{
			if (check && max - min > BruteForceLimit)
				return false;
			for (long d = max; d >= min; d--)
				TryOut(d);
			return true;
		}

		public void SmartSearch()
		{
			var mn = min;
			var mx = max;
			long[,] convergents = Fractions;
			int length = convergents.GetLength(0);
			for (int i = length - 1; i >= 0; i--)
			{
				var d = convergents[i, 1];
				if (d > max) continue;
				var result = Search(d, mn, mx);
				if (result > 0 && PruneSmartSearch)
					mn = result;
			}
		}

		public long Search(long factor, long min, long max)
		{
			if (factor < 7) return -3;

			int count = SmartSearchMax;

			var d = (max / factor) * factor;
			if (d < min)
				return -2;

			long result = 0;
			for (; d >= min && count > 0; d -= factor, count--)
			{
				for (int j = -2; j <= 2; j++)
				{
					if (TryOut(d + j))
						result = d + j;
				}
			}

			return result;
		}

		public bool TryOut(long d)
		{
			if (d < min || d > max) return false;
			return TryOut((long)(d * PiNumerator / PiDenominator), d);
		}

		public bool TryOut(long n, long d)
		{
			if (n > d)
				n %= d;

			BigInt diffn2 = n;
			BigInt diffd2 = d;

			DiffPi(ref diffn2, ref diffd2);
			int cmp = Compare(diffn2, diffd2, diffN, diffD);
			if (cmp > 0 || cmp == 0 && d >= bestD)
				return false;

			diffN = diffn2;
			diffD = diffd2;
			bestN = 3 * d + n;
			bestD = d;
			return true;
		}


		public void Solve()
		{
			// Check to see if range has any entries
			if (CheckPrecomputed())
				return;

			if (UseSmartSearch)
				SmartSearch();

			// Check if range can be brute forced
			if (BruteForce())
				return;
		}


		public bool ReportAnswer()
		{

			if (FixUpDivisor)
			{
				var gcd = (long)BigInt.GreatestCommonDivisor(bestN, bestD);
				if (gcd != 1)
				{
					gcd = BestDivisor(bestD, gcd);
					if (gcd != 1)
					{
						bestD /= gcd;
						bestN /= gcd;
					}
				}
			}

			WriteLine($"{bestN}/{bestD}");
			return true;
		}

		public long BestDivisor(long n, long gcd)
		{
			var tryIt = n / gcd;
			if (tryIt >= min)
				return gcd;

			var minn = n;

			if ((gcd & 1) == 0)
				tryIt = n / 2;
			if (tryIt >= min)
				minn = tryIt;

			long limit = n / min;
			for (int i = 3; i * i <= gcd & i <= limit; i += 2)
			{
				if (gcd % i == 0)
				{
					tryIt = n / i;
					if (tryIt >= min && tryIt < minn)
						minn = tryIt;
				}
			}

			return n / minn;
		}


		public class Rational : IComparable<Rational>
		{
			public readonly long Numerator;
			public readonly long Denominator;

			public Rational(long n, long d, bool reduce = false)
			{
				Numerator = n;
				Denominator = d;
				if (reduce)
				{
					long g = Gcd(n, d);
					Numerator /= g;
					Denominator /= g;
				}
			}

			public int CompareTo(Rational other)
			{
				int cmp = Denominator.CompareTo(other.Denominator);
				if (cmp != 0)
					return cmp;
				return Numerator.CompareTo(other.Numerator);
			}
		}

		// https://www.quora.com/What-is-the-best-rational-approximation-of-pi

		public static IEnumerable<Rational> EnumerateConvergents(bool all = true)
		{
			bool s = false;
			long pn = 2;
			long pd = 1;
			long qn = 4;
			long qd = 1;

			while (true)
			{
				var mn = pn + qn;
				var md = pd + qd;
				var gcd = Gcd(mn, md);
				mn /= gcd;
				md /= gcd;

				var olds = s;
				if (mn * PiDenominator < PiNumerator3 * md)
				{
					s = true;
					pn = mn;
					pd = md;
				}
				else
				{
					s = false;
					qn = mn;
					qd = md;
				}

				if (all || s != olds)
					yield return new Rational(mn, md);
			}

			// all
			// 3/1 7/2 10/3 13/4 16/5 19/6 22/7 25/8 47/15 69/22
			// 91/29 113/36 135/43

			// convergents
			// 3/1 22/7 333/106 355/113 103993/33102 104348/33215
			// 208341/66317 312689/99532
		}

		public static int Compare(
			BigInt n1, BigInt d1,
			BigInt n2, BigInt d2)
		{
			return (n1 * d2).CompareTo(n2 * d1);
		}

		public static void DiffPi(ref BigInt n1, ref BigInt d1)
		{
			if (d1 == PiDenominator)
				n1 -= PiNumerator;
			else
			{
				n1 = n1 * PiDenominator - PiNumerator * d1;
				d1 *= PiDenominator;
			}
			if (n1 < 0)
				n1 = -n1;
		}

		public static long Gcd(long a, long b)
		{
			if (a == 0) return b;
			return Gcd(b % a, a);
		}

		#region Data
		public static long[,] Fractions =
		{
			{                    3,                    1, 1 },  // 1
			{                    7,                    2, 0 },  // 2
			{                   10,                    3, 0 },  // 3
			{                   13,                    4, 0 },  // 4
			{                   16,                    5, 0 },  // 5
			{                   19,                    6, 0 },  // 6
			{                   22,                    7, 1 },  // 7
			{                   25,                    8, 2 },  // 8
			{                   47,                   15, 2 },  // 9
			{                   69,                   22, 2 },  // 10
			{                   91,                   29, 2 },  // 11
			{                  113,                   36, 2 },  // 12
			{                  135,                   43, 2 },  // 13
			{                  157,                   50, 2 },  // 14
			{                  179,                   57, 0 },  // 15
			{                  201,                   64, 0 },  // 16
			{                  223,                   71, 0 },  // 17
			{                  245,                   78, 0 },  // 18
			{                  267,                   85, 0 },  // 19
			{                  289,                   92, 0 },  // 20
			{                  311,                   99, 0 },  // 21
			{                  333,                  106, 1 },  // 22
			{                  355,                  113, 1 },  // 23
			{                  688,                  219, 2 },  // 24
			{                 1043,                  332, 2 },  // 25
			{                 1398,                  445, 2 },  // 26
			{                 1753,                  558, 2 },  // 27
			{                 2108,                  671, 2 },  // 28
			{                 2463,                  784, 2 },  // 29
			{                 2818,                  897, 2 },  // 30
			{                 3173,                 1010, 2 },  // 31
			{                 3528,                 1123, 2 },  // 32
			{                 3883,                 1236, 2 },  // 33
			{                 4238,                 1349, 2 },  // 34
			{                 4593,                 1462, 2 },  // 35
			{                 4948,                 1575, 2 },  // 36
			{                 5303,                 1688, 2 },  // 37
			{                 5658,                 1801, 2 },  // 38
			{                 6013,                 1914, 2 },  // 39
			{                 6368,                 2027, 2 },  // 40
			{                 6723,                 2140, 2 },  // 41
			{                 7078,                 2253, 2 },  // 42
			{                 7433,                 2366, 2 },  // 43
			{                 7788,                 2479, 2 },  // 44
			{                 8143,                 2592, 2 },  // 45
			{                 8498,                 2705, 2 },  // 46
			{                 8853,                 2818, 2 },  // 47
			{                 9208,                 2931, 2 },  // 48
			{                 9563,                 3044, 2 },  // 49
			{                 9918,                 3157, 2 },  // 50
			{                10273,                 3270, 2 },  // 51
			{                10628,                 3383, 2 },  // 52
			{                10983,                 3496, 2 },  // 53
			{                11338,                 3609, 2 },  // 54
			{                11693,                 3722, 2 },  // 55
			{                12048,                 3835, 2 },  // 56
			{                12403,                 3948, 2 },  // 57
			{                12758,                 4061, 2 },  // 58
			{                13113,                 4174, 2 },  // 59
			{                13468,                 4287, 2 },  // 60
			{                13823,                 4400, 2 },  // 61
			{                14178,                 4513, 2 },  // 62
			{                14533,                 4626, 2 },  // 63
			{                14888,                 4739, 2 },  // 64
			{                15243,                 4852, 2 },  // 65
			{                15598,                 4965, 2 },  // 66
			{                15953,                 5078, 2 },  // 67
			{                16308,                 5191, 2 },  // 68
			{                16663,                 5304, 2 },  // 69
			{                17018,                 5417, 2 },  // 70
			{                17373,                 5530, 2 },  // 71
			{                17728,                 5643, 2 },  // 72
			{                18083,                 5756, 2 },  // 73
			{                18438,                 5869, 2 },  // 74
			{                18793,                 5982, 2 },  // 75
			{                19148,                 6095, 2 },  // 76
			{                19503,                 6208, 2 },  // 77
			{                19858,                 6321, 2 },  // 78
			{                20213,                 6434, 2 },  // 79
			{                20568,                 6547, 2 },  // 80
			{                20923,                 6660, 2 },  // 81
			{                21278,                 6773, 2 },  // 82
			{                21633,                 6886, 2 },  // 83
			{                21988,                 6999, 2 },  // 84
			{                22343,                 7112, 2 },  // 85
			{                22698,                 7225, 2 },  // 86
			{                23053,                 7338, 2 },  // 87
			{                23408,                 7451, 2 },  // 88
			{                23763,                 7564, 2 },  // 89
			{                24118,                 7677, 2 },  // 90
			{                24473,                 7790, 2 },  // 91
			{                24828,                 7903, 2 },  // 92
			{                25183,                 8016, 2 },  // 93
			{                25538,                 8129, 2 },  // 94
			{                25893,                 8242, 2 },  // 95
			{                26248,                 8355, 2 },  // 96
			{                26603,                 8468, 2 },  // 97
			{                26958,                 8581, 2 },  // 98
			{                27313,                 8694, 2 },  // 99
			{                27668,                 8807, 2 },  // 100
			{                28023,                 8920, 2 },  // 101
			{                28378,                 9033, 2 },  // 102
			{                28733,                 9146, 2 },  // 103
			{                29088,                 9259, 2 },  // 104
			{                29443,                 9372, 2 },  // 105
			{                29798,                 9485, 2 },  // 106
			{                30153,                 9598, 2 },  // 107
			{                30508,                 9711, 2 },  // 108
			{                30863,                 9824, 2 },  // 109
			{                31218,                 9937, 2 },  // 110
			{                31573,                10050, 2 },  // 111
			{                31928,                10163, 2 },  // 112
			{                32283,                10276, 2 },  // 113
			{                32638,                10389, 2 },  // 114
			{                32993,                10502, 2 },  // 115
			{                33348,                10615, 2 },  // 116
			{                33703,                10728, 2 },  // 117
			{                34058,                10841, 2 },  // 118
			{                34413,                10954, 2 },  // 119
			{                34768,                11067, 2 },  // 120
			{                35123,                11180, 2 },  // 121
			{                35478,                11293, 2 },  // 122
			{                35833,                11406, 2 },  // 123
			{                36188,                11519, 2 },  // 124
			{                36543,                11632, 2 },  // 125
			{                36898,                11745, 2 },  // 126
			{                37253,                11858, 2 },  // 127
			{                37608,                11971, 2 },  // 128
			{                37963,                12084, 2 },  // 129
			{                38318,                12197, 2 },  // 130
			{                38673,                12310, 2 },  // 131
			{                39028,                12423, 2 },  // 132
			{                39383,                12536, 2 },  // 133
			{                39738,                12649, 2 },  // 134
			{                40093,                12762, 2 },  // 135
			{                40448,                12875, 2 },  // 136
			{                40803,                12988, 2 },  // 137
			{                41158,                13101, 2 },  // 138
			{                41513,                13214, 2 },  // 139
			{                41868,                13327, 2 },  // 140
			{                42223,                13440, 2 },  // 141
			{                42578,                13553, 2 },  // 142
			{                42933,                13666, 2 },  // 143
			{                43288,                13779, 2 },  // 144
			{                43643,                13892, 2 },  // 145
			{                43998,                14005, 2 },  // 146
			{                44353,                14118, 2 },  // 147
			{                44708,                14231, 2 },  // 148
			{                45063,                14344, 2 },  // 149
			{                45418,                14457, 2 },  // 150
			{                45773,                14570, 2 },  // 151
			{                46128,                14683, 2 },  // 152
			{                46483,                14796, 2 },  // 153
			{                46838,                14909, 2 },  // 154
			{                47193,                15022, 2 },  // 155
			{                47548,                15135, 2 },  // 156
			{                47903,                15248, 2 },  // 157
			{                48258,                15361, 2 },  // 158
			{                48613,                15474, 2 },  // 159
			{                48968,                15587, 2 },  // 160
			{                49323,                15700, 2 },  // 161
			{                49678,                15813, 2 },  // 162
			{                50033,                15926, 2 },  // 163
			{                50388,                16039, 2 },  // 164
			{                50743,                16152, 2 },  // 165
			{                51098,                16265, 2 },  // 166
			{                51453,                16378, 2 },  // 167
			{                51808,                16491, 2 },  // 168
			{                52163,                16604, 0 },  // 169
			{                52518,                16717, 0 },  // 170
			{                52873,                16830, 0 },  // 171
			{                53228,                16943, 0 },  // 172
			{                53583,                17056, 0 },  // 173
			{                53938,                17169, 0 },  // 174
			{                54293,                17282, 0 },  // 175
			{                54648,                17395, 0 },  // 176
			{                55003,                17508, 0 },  // 177
			{                55358,                17621, 0 },  // 178
			{                55713,                17734, 0 },  // 179
			{                56068,                17847, 0 },  // 180
			{                56423,                17960, 0 },  // 181
			{                56778,                18073, 0 },  // 182
			{                57133,                18186, 0 },  // 183
			{                57488,                18299, 0 },  // 184
			{                57843,                18412, 0 },  // 185
			{                58198,                18525, 0 },  // 186
			{                58553,                18638, 0 },  // 187
			{                58908,                18751, 0 },  // 188
			{                59263,                18864, 0 },  // 189
			{                59618,                18977, 0 },  // 190
			{                59973,                19090, 0 },  // 191
			{                60328,                19203, 0 },  // 192
			{                60683,                19316, 0 },  // 193
			{                61038,                19429, 0 },  // 194
			{                61393,                19542, 0 },  // 195
			{                61748,                19655, 0 },  // 196
			{                62103,                19768, 0 },  // 197
			{                62458,                19881, 0 },  // 198
			{                62813,                19994, 0 },  // 199
			{                63168,                20107, 0 },  // 200
			{                63523,                20220, 0 },  // 201
			{                63878,                20333, 0 },  // 202
			{                64233,                20446, 0 },  // 203
			{                64588,                20559, 0 },  // 204
			{                64943,                20672, 0 },  // 205
			{                65298,                20785, 0 },  // 206
			{                65653,                20898, 0 },  // 207
			{                66008,                21011, 0 },  // 208
			{                66363,                21124, 0 },  // 209
			{                66718,                21237, 0 },  // 210
			{                67073,                21350, 0 },  // 211
			{                67428,                21463, 0 },  // 212
			{                67783,                21576, 0 },  // 213
			{                68138,                21689, 0 },  // 214
			{                68493,                21802, 0 },  // 215
			{                68848,                21915, 0 },  // 216
			{                69203,                22028, 0 },  // 217
			{                69558,                22141, 0 },  // 218
			{                69913,                22254, 0 },  // 219
			{                70268,                22367, 0 },  // 220
			{                70623,                22480, 0 },  // 221
			{                70978,                22593, 0 },  // 222
			{                71333,                22706, 0 },  // 223
			{                71688,                22819, 0 },  // 224
			{                72043,                22932, 0 },  // 225
			{                72398,                23045, 0 },  // 226
			{                72753,                23158, 0 },  // 227
			{                73108,                23271, 0 },  // 228
			{                73463,                23384, 0 },  // 229
			{                73818,                23497, 0 },  // 230
			{                74173,                23610, 0 },  // 231
			{                74528,                23723, 0 },  // 232
			{                74883,                23836, 0 },  // 233
			{                75238,                23949, 0 },  // 234
			{                75593,                24062, 0 },  // 235
			{                75948,                24175, 0 },  // 236
			{                76303,                24288, 0 },  // 237
			{                76658,                24401, 0 },  // 238
			{                77013,                24514, 0 },  // 239
			{                77368,                24627, 0 },  // 240
			{                77723,                24740, 0 },  // 241
			{                78078,                24853, 0 },  // 242
			{                78433,                24966, 0 },  // 243
			{                78788,                25079, 0 },  // 244
			{                79143,                25192, 0 },  // 245
			{                79498,                25305, 0 },  // 246
			{                79853,                25418, 0 },  // 247
			{                80208,                25531, 0 },  // 248
			{                80563,                25644, 0 },  // 249
			{                80918,                25757, 0 },  // 250
			{                81273,                25870, 0 },  // 251
			{                81628,                25983, 0 },  // 252
			{                81983,                26096, 0 },  // 253
			{                82338,                26209, 0 },  // 254
			{                82693,                26322, 0 },  // 255
			{                83048,                26435, 0 },  // 256
			{                83403,                26548, 0 },  // 257
			{                83758,                26661, 0 },  // 258
			{                84113,                26774, 0 },  // 259
			{                84468,                26887, 0 },  // 260
			{                84823,                27000, 0 },  // 261
			{                85178,                27113, 0 },  // 262
			{                85533,                27226, 0 },  // 263
			{                85888,                27339, 0 },  // 264
			{                86243,                27452, 0 },  // 265
			{                86598,                27565, 0 },  // 266
			{                86953,                27678, 0 },  // 267
			{                87308,                27791, 0 },  // 268
			{                87663,                27904, 0 },  // 269
			{                88018,                28017, 0 },  // 270
			{                88373,                28130, 0 },  // 271
			{                88728,                28243, 0 },  // 272
			{                89083,                28356, 0 },  // 273
			{                89438,                28469, 0 },  // 274
			{                89793,                28582, 0 },  // 275
			{                90148,                28695, 0 },  // 276
			{                90503,                28808, 0 },  // 277
			{                90858,                28921, 0 },  // 278
			{                91213,                29034, 0 },  // 279
			{                91568,                29147, 0 },  // 280
			{                91923,                29260, 0 },  // 281
			{                92278,                29373, 0 },  // 282
			{                92633,                29486, 0 },  // 283
			{                92988,                29599, 0 },  // 284
			{                93343,                29712, 0 },  // 285
			{                93698,                29825, 0 },  // 286
			{                94053,                29938, 0 },  // 287
			{                94408,                30051, 0 },  // 288
			{                94763,                30164, 0 },  // 289
			{                95118,                30277, 0 },  // 290
			{                95473,                30390, 0 },  // 291
			{                95828,                30503, 0 },  // 292
			{                96183,                30616, 0 },  // 293
			{                96538,                30729, 0 },  // 294
			{                96893,                30842, 0 },  // 295
			{                97248,                30955, 0 },  // 296
			{                97603,                31068, 0 },  // 297
			{                97958,                31181, 0 },  // 298
			{                98313,                31294, 0 },  // 299
			{                98668,                31407, 0 },  // 300
			{                99023,                31520, 0 },  // 301
			{                99378,                31633, 0 },  // 302
			{                99733,                31746, 0 },  // 303
			{               100088,                31859, 0 },  // 304
			{               100443,                31972, 0 },  // 305
			{               100798,                32085, 0 },  // 306
			{               101153,                32198, 0 },  // 307
			{               101508,                32311, 0 },  // 308
			{               101863,                32424, 0 },  // 309
			{               102218,                32537, 0 },  // 310
			{               102573,                32650, 0 },  // 311
			{               102928,                32763, 0 },  // 312
			{               103283,                32876, 0 },  // 313
			{               103638,                32989, 0 },  // 314
			{               103993,                33102, 1 },  // 315
			{               104348,                33215, 1 },  // 316
			{               208341,                66317, 1 },  // 317
			{               312689,                99532, 1 },  // 318
			{               521030,               165849, 2 },  // 319
			{               833719,               265381, 1 },  // 320
			{              1146408,               364913, 1 },  // 321
			{              1980127,               630294, 2 },  // 322
			{              3126535,               995207, 0 },  // 323
			{              4272943,              1360120, 1 },  // 324
			{              5419351,              1725033, 1 },  // 325
			{              9692294,              3085153, 2 },  // 326
			{             15111645,              4810186, 2 },  // 327
			{             20530996,              6535219, 2 },  // 328
			{             25950347,              8260252, 2 },  // 329
			{             31369698,              9985285, 2 },  // 330
			{             36789049,             11710318, 2 },  // 331
			{             42208400,             13435351, 0 },  // 332
			{             47627751,             15160384, 0 },  // 333
			{             53047102,             16885417, 0 },  // 334
			{             58466453,             18610450, 0 },  // 335
			{             63885804,             20335483, 0 },  // 336
			{             69305155,             22060516, 0 },  // 337
			{             74724506,             23785549, 0 },  // 338
			{             80143857,             25510582, 1 },  // 339
			{             85563208,             27235615, 2 },  // 340
			{            165707065,             52746197, 1 },  // 341
			{            245850922,             78256779, 1 },  // 342
			{            411557987,            131002976, 1 },  // 343
			{            657408909,            209259755, 0 },  // 344
			{           1068966896,            340262731, 1 },  // 345
			{           1480524883,            471265707, 2 },  // 346
			{           2549491779,            811528438, 1 },  // 347
			{           3618458675,           1151791169, 0 },  // 348
			{           6167950454,           1963319607, 1 },  // 349
			{           8717442233,           2774848045, 2 },  // 350
			{          14885392687,           4738167652, 1 },  // 351
			{          21053343141,           6701487259, 1 },  // 352
			{          35938735828,          11439654911, 2 },  // 353
			{          56992078969,          18141142170, 2 },  // 354
			{          78045422110,          24842629429, 2 },  // 355
			{          99098765251,          31544116688, 2 },  // 356
			{         120152108392,          38245603947, 2 },  // 357
			{         141205451533,          44947091206, 2 },  // 358
			{         162258794674,          51648578465, 2 },  // 359
			{         183312137815,          58350065724, 2 },  // 360
			{         204365480956,          65051552983, 2 },  // 361
			{         225418824097,          71753040242, 2 },  // 362
			{         246472167238,          78454527501, 2 },  // 363
			{         267525510379,          85156014760, 2 },  // 364
			{         288578853520,          91857502019, 2 },  // 365
			{         309632196661,          98558989278, 2 },  // 366
			{         330685539802,         105260476537, 2 },  // 367
			{         351738882943,         111961963796, 2 },  // 368
			{         372792226084,         118663451055, 2 },  // 369
			{         393845569225,         125364938314, 2 },  // 370
			{         414898912366,         132066425573, 2 },  // 371
			{         435952255507,         138767912832, 2 },  // 372
			{         457005598648,         145469400091, 2 },  // 373
			{         478058941789,         152170887350, 2 },  // 374
			{         499112284930,         158872374609, 2 },  // 375
			{         520165628071,         165573861868, 2 },  // 376
			{         541218971212,         172275349127, 2 },  // 377
			{         562272314353,         178976836386, 2 },  // 378
			{         583325657494,         185678323645, 2 },  // 379
			{         604379000635,         192379810904, 2 },  // 380
			{         625432343776,         199081298163, 2 },  // 381
			{         646485686917,         205782785422, 2 },  // 382
			{         667539030058,         212484272681, 2 },  // 383
			{         688592373199,         219185759940, 2 },  // 384
			{         709645716340,         225887247199, 2 },  // 385
			{         730699059481,         232588734458, 2 },  // 386
			{         751752402622,         239290221717, 2 },  // 387
			{         772805745763,         245991708976, 2 },  // 388
			{         793859088904,         252693196235, 2 },  // 389
			{         814912432045,         259394683494, 2 },  // 390
			{         835965775186,         266096170753, 2 },  // 391
			{         857019118327,         272797658012, 2 },  // 392
			{         878072461468,         279499145271, 2 },  // 393
			{         899125804609,         286200632530, 0 },  // 394
			{         920179147750,         292902119789, 0 },  // 395
			{         941232490891,         299603607048, 0 },  // 396
			{         962285834032,         306305094307, 0 },  // 397
			{         983339177173,         313006581566, 0 },  // 398
			{        1004392520314,         319708068825, 0 },  // 399
			{        1025445863455,         326409556084, 0 },  // 400
			{        1046499206596,         333111043343, 0 },  // 401
			{        1067552549737,         339812530602, 0 },  // 402
			{        1088605892878,         346514017861, 0 },  // 403
			{        1109659236019,         353215505120, 0 },  // 404
			{        1130712579160,         359916992379, 0 },  // 405
			{        1151765922301,         366618479638, 0 },  // 406
			{        1172819265442,         373319966897, 0 },  // 407
			{        1193872608583,         380021454156, 0 },  // 408
			{        1214925951724,         386722941415, 0 },  // 409
			{        1235979294865,         393424428674, 0 },  // 410
			{        1257032638006,         400125915933, 0 },  // 411
			{        1278085981147,         406827403192, 0 },  // 412
			{        1299139324288,         413528890451, 0 },  // 413
			{        1320192667429,         420230377710, 0 },  // 414
			{        1341246010570,         426931864969, 0 },  // 415
			{        1362299353711,         433633352228, 0 },  // 416
			{        1383352696852,         440334839487, 0 },  // 417
			{        1404406039993,         447036326746, 0 },  // 418
			{        1425459383134,         453737814005, 0 },  // 419
			{        1446512726275,         460439301264, 0 },  // 420
			{        1467566069416,         467140788523, 0 },  // 421
			{        1488619412557,         473842275782, 0 },  // 422
			{        1509672755698,         480543763041, 0 },  // 423
			{        1530726098839,         487245250300, 0 },  // 424
			{        1551779441980,         493946737559, 0 },  // 425
			{        1572832785121,         500648224818, 0 },  // 426
			{        1593886128262,         507349712077, 0 },  // 427
			{        1614939471403,         514051199336, 0 },  // 428
			{        1635992814544,         520752686595, 0 },  // 429
			{        1657046157685,         527454173854, 0 },  // 430
			{        1678099500826,         534155661113, 0 },  // 431
			{        1699152843967,         540857148372, 0 },  // 432
			{        1720206187108,         547558635631, 0 },  // 433
			{        1741259530249,         554260122890, 0 },  // 434
			{        1762312873390,         560961610149, 0 },  // 435
			{        1783366216531,         567663097408, 1 },  // 436
			{        1804419559672,         574364584667, 2 },  // 437
			{        3587785776203,        1142027682075, 1 },  // 438
			{        5371151992734,        1709690779483, 1 },  // 439
			{        8958937768937,        2851718461558, 1 },  // 440
			{       14330089761671,        4561409241041, 2 },  // 441
			{       23289027530608,        7413127702599, 2 },  // 442
			{       32247965299545,       10264846164157, 2 },  // 443
			{       41206903068482,       13116564625715, 2 },  // 444
			{       50165840837419,       15968283087273, 2 },  // 445
			{       59124778606356,       18820001548831, 2 },  // 446
			{       68083716375293,       21671720010389, 2 },  // 447
			{       77042654144230,       24523438471947, 0 },  // 448
			{       86001591913167,       27375156933505, 0 },  // 449
			{       94960529682104,       30226875395063, 0 },  // 450
			{      103919467451041,       33078593856621, 0 },  // 451
			{      112878405219978,       35930312318179, 0 },  // 452
			{      121837342988915,       38782030779737, 0 },  // 453
			{      130796280757852,       41633749241295, 0 },  // 454
			{      139755218526789,       44485467702853, 1 },  // 455
			{      148714156295726,       47337186164411, 2 },  // 456
			{      288469374822515,       91822653867264, 0 },  // 457
			{      428224593349304,      136308121570117, 1 },  // 458
			{      567979811876093,      180793589272970, 2 },  // 459
			{      996204405225397,      317101710843087, 2 },  // 460
			{     1424428998574701,      453409832413204, 2 },  // 461
			{     1852653591924005,      589717953983321, 2 },  // 462
			{     2280878185273309,      726026075553438, 2 },  // 463
			{     2709102778622613,      862334197123555, 2 },  // 464
			{     3137327371971917,      998642318693672, 0 },  // 465		
		};

		#endregion


#if DEBUG
		public static bool Verbose = true;
#else
		public static bool Verbose = false;
#endif

	}


	public static class HackerRankUtils
	{

		#region Reporting Answer

		static volatile bool _reported;
		static Timer _timer;
		static Func<bool> _timerAction;

		public static void LaunchTimer(Func<bool> action, long ms = 2800)
		{
			_timerAction = action;
			_timer = new Timer(
				delegate
				{
					if (!UseTimer)
						return;
#if !DEBUG
					Report();
				if (_reported)
					Environment.Exit(0);
#endif
				}, null, ms, 0);
		}

		public static void Report()
		{
			if (_reported) return;
			_reported = true;
			_reported = _timerAction();
		}

#if DEBUG
		static Stopwatch _stopWatch = new Stopwatch();
#endif

		public static void Run(string name, Action action)
		{
#if DEBUG
			Write(name + ": ");
			_stopWatch.Restart();
			action();
			_stopWatch.Stop();
			WriteLine($"Elapsed Time: {_stopWatch.Elapsed}\n");
#else
		action();
#endif
		}

		[Conditional("DEBUG")]
		public static void Run2(string name, Action action)
		{
			// Ignore
		}

		#endregion



		public static int Timeout = 2000; // Greater timeouts were less effective
		public static int BruteForceLimit = 1000000; // 1e6; only lower if #7 passes
		public static int SmartSearchMax = 1000; // or lower b/cconvergents
		public static bool UseSmartSearch = true;
		public static bool PruneSmartSearch = true;
		public static bool FixUpDivisor = false;
		public static bool UseTimer = true;

	}

}