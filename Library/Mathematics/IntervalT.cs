﻿#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;

#endregion

namespace Softperson.Mathematics
{
	/// <summary>
	///     Summary description for Interval.
	/// </summary>
	public struct Interval<T>
		where T : IComparable<T>
	{
		#region Variables

		public T End;
		public T Start;

		#endregion

		#region Construction

		public Interval(T position)
		{
			Start = End = position;
		}

		public Interval(T start, T end)
		{
			Start = start;
			End = end;
		}

		#endregion

		public bool IsEmpty
		{
			get { return End.CompareTo(Start) <= 0; }
		}

		public bool Contains(T position)
		{
			return position.CompareTo(Start) >= 0
				   && position.CompareTo(End) < 0;
		}

		public bool Contains(Interval<T> interval)
		{
			return interval.Start.CompareTo(Start) >= 0
				   && interval.End.CompareTo(End) <= 0;
		}

		public void Union(Interval<T> interval)
		{
			Start = interval.Start.Min(Start);
			End = interval.End.Max(End);
		}

		public void Union(T position)
		{
			if (position.CompareTo(Start) < 0)
				Start = position;
			if (position.CompareTo(End) > 0)
				End = position;
		}

		public void Intersect(Interval<T> interval)
		{
			Start = interval.Start.Max(Start);
			End = interval.End.Min(End);
		}

		public override string ToString()
		{
			return $"({Start},{End})";
		}

		public void Normalize()
		{
			Utility.Normalize(ref Start, ref End);
		}

		public static bool operator <(Interval<T> i1, Interval<T> i2)
		{
			return i1.End.CompareTo(i2.Start) <= 0;
		}

		public static bool operator >(Interval<T> i1, Interval<T> i2)
		{
			return i2.End.CompareTo(i1.Start) <= 0;
		}
	}
}