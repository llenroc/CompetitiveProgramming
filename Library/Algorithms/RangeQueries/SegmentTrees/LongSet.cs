using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.RangeQueries
{
	public class LongSet
	{
		readonly ulong[][] _bits;
		readonly int _capacity;
		public int Size { get; private set; }

		public LongSet(int capacity)
		{
			this._capacity = capacity;
			int d = 1;
			for (int m = capacity; m > 1; m >>= 6, d++) ;

			_bits = new ulong[d][];
			for (int i = 0, m = capacity >> 6; i < d; i++, m >>= 6)
				_bits[i] = new ulong[m + 1];
			Size = 0;
		}

		public LongSet SetRangeExclusive(int r)
		{
			for (int i = 0; i < _bits.Length; i++, r = r + 63 >> 6)
			{
				for (int j = 0; j < (r >> 6); j++)
					_bits[i][j] = ulong.MaxValue;
				if ((r & 63) != 0) _bits[i][r >> 6] |= (1UL << r) - 1;
			}
			return this;
		}

		// [0,r)
		public LongSet UnsetRange(int r)
		{
			if (r >= 0)
			{
				for (int i = 0; i < _bits.Length; i++, r = r + 63 >> 6)
				{
					for (int j = 0; j < r + 63 >> 6; j++)
						_bits[i][j] = 0;
					if ((r & 63) != 0) _bits[i][r >> 6] &= ~((1UL << r) - 1);
				}
			}
			return this;
		}

		public bool this[int pos]
		{
			get { return pos >= 0 && pos < _capacity && (long)(_bits[0][pos >> 6] << (~pos&63) ) < 0; }
			set
			{
				if (value)
				{
					if (pos >= 0 && pos < _capacity)
					{
						if (!this[pos]) Size++;
						for (int i = 0; i < _bits.Length; i++, pos >>= 6)
							_bits[i][pos >> 6] |= 1UL << pos;
					}
				}
				else
				{
					if (pos >= 0 && pos < _capacity)
					{
						if (this[pos]) Size--;
						for (int i = 0; i < _bits.Length && (i == 0 || _bits[i - 1][pos] == 0L); i++, pos >>= 6)
							_bits[i][pos >> 6] &= ~(1UL << pos);
					}

				}

			}
		}

		public int Prev(int pos)
		{
			for (int i = 0; i < _bits.Length && pos >= 0; i++, pos >>= 6, pos--)
			{
				int pre = Prev(_bits[i][pos >> 6], pos & 63);
				if (pre != -1)
				{
					pos = pos >> 6 << 6 | pre;
					while (i > 0) pos = pos << 6 | 63 - NumberOfLeadingZeros(_bits[--i][pos]);
					return pos;
				}
			}
			return -1;
		}

		public int Next(int pos)
		{
			for (int i = 0; i < _bits.Length && pos >> 6 < _bits[i].Length; i++, pos >>= 6, pos++)
			{
				int nex = Next(_bits[i][pos >> 6], pos & 63);
				if (nex != -1)
				{
					pos = pos >> 6 << 6 | nex;
					while (i > 0) pos = pos << 6 | NumberOfTrailingZeros(_bits[--i][pos]);
					return pos;
				}
			}
			return -1;
		}

		static int Prev(ulong set, int n)
		{
			ulong h = HighestOneBit(set << ~n);
			if (h == 0L) return -1;
			return NumberOfTrailingZeros(h) - (63 - n);
		}

		static int Next(ulong set, int n)
		{
			ulong h = LowestOneBit(set >> n);
			if (h == 0L) return -1;
			return NumberOfTrailingZeros(h) + n;
		}

		public override string ToString()
		{
			var list = new List<int>();
			for (int pos = Next(0); pos != -1; pos = Next(pos + 1))
				list.Add(pos);
			return string.Join(" ", list);
		}

		public static int NumberOfTrailingZeros(ulong v)
		{
            ulong lastBit = v & (ulong)-(long)v;
            return lastBit != 0 ? Log2(lastBit) : 64;
        }

        public static int NumberOfLeadingZeros(ulong n)
		{
            return 32 - 1 - Log2(n);
        }

        public static ulong LowestOneBit(ulong n)
		{
            return n & unchecked((ulong)(-(long)n));
		}

		public static ulong HighestOneBit(ulong n)
		{
		    return n != 0 ? 1UL << Log2(n) : 0;
		}

        public static int AlternateLog2(ulong value)
        {
            // UNTESTED
            var log = 0;
            if (value >= (1UL << 24))
            {
                if (value >= (1UL << 48))
                {
                    log = 48;
                    value = (value >> 48);
                }
                else
                {
                    log = 24;
                    value >>= 24;
                }
            }
            if (value >= (1 << 12))
            {
                log += 12;
                value >>= 12;
            }
            if (value >= (1 << 6))
            {
                log += 6;
                value >>= 6;
            }
            if (value >= (1 << 3))
            {
                log += 3;
                value >>= 3;
            }
            return log + (int)(value >> 1 & ~value >> 2);
        }


        public static int Log2(ulong value)
        {
            if (value <= 0)
                return -1;

            var log = 0;
            if (value >= 0x100000000L)
            {
                log += 32;
                value >>= 32;
            }
            if (value >= 0x10000)
            {
                log += 16;
                value >>= 16;
            }
            if (value >= 0x100)
            {
                log += 8;
                value >>= 8;
            }
            if (value >= 0x10)
            {
                log += 4;
                value >>= 4;
            }
            if (value >= 0x4)
            {
                log += 2;
                value >>= 2;
            }
            if (value >= 0x2)
            {
                log += 1;
            }
            return log;
        }
    }
}
