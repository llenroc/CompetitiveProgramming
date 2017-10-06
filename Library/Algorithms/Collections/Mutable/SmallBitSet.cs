#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the express permission of Wesner Moise.
//
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Diagnostics;

#endregion

namespace Softperson.Collections
{
	/// <summary>
	///     Summary description for SmallBitSet.
	/// </summary>
	[DebuggerStepThrough]
	public unsafe struct SmallBitSet
	{
		#region Variables

		private long long1;
		private long long2;
		private long long3;
		private long long4;

		#endregion

		#region Constructors

		private SmallBitSet(long long1, long long2, long long3, long long4)
		{
			this.long1 = long1;
			this.long2 = long2;
			this.long3 = long3;
			this.long4 = long4;
		}

		#endregion

		#region Properties

		public static readonly SmallBitSet FullSet = new SmallBitSet(-1, -1, -1, -1);

		public bool IsEmpty
		{
			get { return long1 == 0 || long2 == 0 || long3 == 0 || long4 == 0; }
		}

		public bool IsFull
		{
			get { return long1 == -1 && long2 == -1 && long3 == -1 && long4 == -1; }
		}

		public bool this[int index]
		{
			get
			{
				var offset = (index >> 6) & 3;
				var bit = 1L << (index & 0x3f);
				fixed (long* p = &long1)
					return (p[offset] & bit) != 0;
			}
			set
			{
				var offset = (index >> 6) & 3;
				var bit = 1L << (index & 0x3f);

				fixed (long* p = &long1)
				{
					if (value)
						p[offset] |= bit;
					else
						p[offset] &= ~bit;
				}
			}
		}


		public void SetItem(int index, bool value)
		{
			var offset = (index >> 6) & 3;
			var bit = 1L << (index & 0x3f);

			fixed (long* p = &long1)
			{
				if (value)
					p[offset] |= bit;
				else
					p[offset] &= ~bit;
			}
		}

		#endregion

		#region Methods

		public static bool operator ==(SmallBitSet bits1, SmallBitSet bits2)
		{
			return bits1.long1 == bits2.long1 && bits1.long2 == bits2.long2
				   && bits1.long3 == bits2.long3 && bits1.long4 == bits2.long4;
		}

		public static bool operator !=(SmallBitSet bits1, SmallBitSet bits2)
		{
			return bits1.long1 != bits2.long1 || bits1.long2 != bits2.long2
				   || bits1.long3 != bits2.long3 || bits1.long4 != bits2.long4;
		}

		public static SmallBitSet operator |(SmallBitSet bits1, SmallBitSet bits2)
		{
			bits1.long1 |= bits2.long1;
			bits1.long2 |= bits2.long2;
			bits1.long3 |= bits2.long3;
			bits1.long4 |= bits2.long4;
			return bits1;
		}

		public void Union(ref SmallBitSet bits2)
		{
			long1 |= bits2.long1;
			long2 |= bits2.long2;
			long3 |= bits2.long3;
			long4 |= bits2.long4;
		}

		public static SmallBitSet operator &(SmallBitSet bits1, SmallBitSet bits2)
		{
			bits1.long1 &= bits2.long1;
			bits1.long2 &= bits2.long2;
			bits1.long3 &= bits2.long3;
			bits1.long4 &= bits2.long4;
			return bits1;
		}

		public static SmallBitSet operator -(SmallBitSet bits1, SmallBitSet bits2)
		{
			bits1.long1 &= ~bits2.long1;
			bits1.long2 &= ~bits2.long2;
			bits1.long3 &= ~bits2.long3;
			bits1.long4 &= ~bits2.long4;
			return bits1;
		}

		public static SmallBitSet operator ^(SmallBitSet bits1, SmallBitSet bits2)
		{
			bits1.long1 ^= bits2.long1;
			bits1.long2 ^= bits2.long2;
			bits1.long3 ^= bits2.long3;
			bits1.long4 ^= bits2.long4;
			return bits1;
		}

		public static SmallBitSet operator ~(SmallBitSet bits)
		{
			bits.long1 = ~bits.long1;
			bits.long2 = ~bits.long2;
			bits.long3 = ~bits.long3;
			bits.long4 = ~bits.long4;
			return bits;
		}

		#endregion

		#region Object Overrides

		public bool Contains(ref SmallBitSet bits)
		{
			return (~long1 & bits.long1) == 0
				   && (~long2 & bits.long2) == 0
				   && (~long3 & bits.long3) == 0
				   && (~long4 & bits.long4) == 0;
		}

		public bool Contains(SmallBitSet bits)
		{
			return (~long1 & bits.long1) == 0
				   && (~long2 & bits.long2) == 0
				   && (~long3 & bits.long3) == 0
				   && (~long4 & bits.long4) == 0;
		}

		public bool Overlaps(ref SmallBitSet bits)
		{
			return (long1 & bits.long1) != 0
				   || (long2 & bits.long2) != 0
				   || (long3 & bits.long3) != 0
				   || (long4 & bits.long4) != 0;
		}

		public bool Overlaps(SmallBitSet bits)
		{
			return (long1 & bits.long1) != 0
				   || (long2 & bits.long2) != 0
				   || (long3 & bits.long3) != 0
				   || (long4 & bits.long4) != 0;
		}

		public override string ToString()
		{
			return
				long1.ToString("X8")
				+ long2.ToString("X8")
				+ long3.ToString("X8")
				+ long4.ToString("X8");
		}

		/// <summary>
		///     Determines whether two Object instances are equal.
		/// </summary>
		public override bool Equals(object obj)
		{
			return obj is SmallBitSet && Equals((SmallBitSet) obj);
		}

		/// <summary>
		///     Determines whether two SmallBitSet instances are equal.
		/// </summary>
		public bool Equals(SmallBitSet obj)
		{
			return obj.long1 == long1 && obj.long2 == long2
				   && obj.long3 == long3 && obj.long4 == long4;
		}

		/// <summary>
		///     Serves as a hash function for a particular type, suitable for use in
		///     hashing algorithms and data structures like a hash table.
		/// </summary>
		public override int GetHashCode()
		{
			return (long1 ^ long2 ^ long3 ^ long4).GetHashCode();
		}

		#endregion
	}
}