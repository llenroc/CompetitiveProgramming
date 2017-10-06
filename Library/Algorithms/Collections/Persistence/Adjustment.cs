﻿#region

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;

#endregion

namespace Softperson.Collections
{
	public struct Adjustment
	{
		#region Variables

		public readonly int Start;
		public readonly int Count;
		public readonly bool PropertyOnly;
		public readonly bool Revision;

		#endregion

		#region Construction

		[DebuggerStepThrough]
		private Adjustment(int start, int count, bool property, bool revision)
		{
			Count = count;
			PropertyOnly = property;
			Start = start;
			Revision = revision;
		}

		[DebuggerStepThrough]
		public Adjustment(int start, int count, bool property = false)
		{
			Utility.Assert(start >= 0);
			Utility.Assert(!property || count <= 0);
			Count = count;
			PropertyOnly = property;
			Start = start;
			Revision = false;
		}

		#endregion Construction

		#region Properties

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Deleted => Count < 0 ? -Count : 0;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Inserted => Count < 0 ? 0 : Count;

		public bool Deleting => !PropertyOnly && Count < 0;

		public int End => Start - Math.Min(Count, 0);

		#endregion

		#region Adjustment

		[DebuggerHidden]
		public Adjustment Invert()
		{
			if (PropertyOnly)
				return this;
			return new Adjustment(Start, -Count, PropertyOnly, !Revision);
		}

		public bool Affected(int start, int end)
			=> Start <= end && End >= start && Count != 0;

		public static void NegatePosition(ref int position, ref int deletions)
		{
			if (deletions == 0)
				position--;
			else
				deletions++;
		}

		public static void UnnegatePosition(ref int position, ref int deletions)
		{
			if (deletions == 0)
				position++;
			else
				deletions--;
		}

		public int Adjust(int position, ref int deletions)
#if DEBUG
		{
			var deletions1 = deletions;
			var position1 = AdjustCore(position, ref deletions1);

			var deletions2 = deletions1;
			var position2 = Invert().AdjustCore(position1, ref deletions2);

			var deletions3 = deletions2;
			var position3 = AdjustCore(position2, ref deletions3);

			Utility.Assert(position == position2);
			Utility.Assert(deletions == deletions2);
			Utility.Assert(position1 == position3);
			Utility.Assert(deletions1 == deletions3);

			deletions = deletions1;
			return position1;
		}

		private int AdjustCore(int position, ref int deletions)
#endif
		{
			if (position >= int.MaxValue || PropertyOnly)
				return position;

			var start = Start;
			var startPosition = position;
			var endPosition = position + deletions;
			if (endPosition < start)
				return position;

			var count = Count;
			if (count > 0)
			{
				if (Revision && startPosition == start && endPosition < start + count)
				{
					startPosition = endPosition;
				}
				else
				{
					if (startPosition >= start)
						startPosition += count;
					endPosition += count;
				}
			}
			else
			{
				if (!Revision && deletions == 0 &&
					startPosition >= start && endPosition <= start + count)
				{
					startPosition = start;
				}
				else if (endPosition > start)
				{
					if (startPosition > start)
						startPosition += count;
					endPosition += count;
				}
			}

			deletions = endPosition - startPosition;
			return startPosition;
		}

		public override string ToString()
		{
			string operation;

			if (PropertyOnly)
				operation = "Changing";
			else if (Count < 0)
				operation = "Deleting";
			else
				operation = "Inserting";

			var misc = "";

			if (Revision)
				misc = "*";

			return operation + misc + "(" + Start + "," + Math.Abs(Count) + ")";
		}

		#endregion
	}
}