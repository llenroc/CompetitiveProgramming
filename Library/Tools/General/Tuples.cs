#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Softperson
{

	#region Pair

	public struct Pair<F, L> : IComparable<Pair<F, L>>
	{
		#region Variables

		public F First;
		public L Second;

		#endregion

		#region Construction

		public Pair(F First, L last)
		{
			this.First = First;
			Second = last;
		}

		#endregion

		#region Methods

		[DebuggerStepThrough]
		public override bool Equals(object obj)
		{
			return obj is Pair<F, L> && Equals((Pair<F, L>) obj);
		}

		[DebuggerStepThrough]
		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			var hash1 = First == null ? 1 : First.GetHashCode();
			var hash2 = Second == null ? 2 : Second.GetHashCode();
			return Utility.CreateHashCode(hash1, hash2);
		}

		[DebuggerStepThrough]
		public override string ToString()
		{
			return First + ", " + Second;
		}

		[DebuggerStepThrough]
		public Pair<L, F> Swap()
		{
			return new Pair<L, F>(Second, First);
		}

		#endregion

		#region IComparable<Pair<F,L>> Members

		public int CompareTo(Pair<F, L> other)
		{
			var cmp = Comparer<F>.Default.Compare(First, other.First);
			if (cmp != 0)
				return cmp;
			return Comparer<L>.Default.Compare(Second, other.Second);
		}

		#endregion

		public bool Equals(Pair<F, L> other)
		{
			return EqualityComparer<F>.Default.Equals(First, other.First)
				   && EqualityComparer<L>.Default.Equals(Second, other.Second);
		}

		#region Operators


		public static implicit operator KeyValuePair<F, L>(Pair<F, L> tuple)
		{
			return new KeyValuePair<F, L>(tuple.First, tuple.Second);
		}

		public static implicit operator Pair<F, L>(KeyValuePair<F, L> pair)
		{
			return new Pair<F, L>(pair.Key, pair.Value);
		}

		public static bool operator <(Pair<F, L> left, Pair<F, L> right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator >(Pair<F, L> left, Pair<F, L> right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator <=(Pair<F, L> left, Pair<F, L> right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >=(Pair<F, L> left, Pair<F, L> right)
		{
			return left.CompareTo(right) >= 0;
		}

		public static bool operator ==(Pair<F, L> left, Pair<F, L> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Pair<F, L> left, Pair<F, L> right)
		{
			return !left.Equals(right);
		}
		#endregion
	}

	#endregion

	#region Triple

	public struct Triple<F, S, T> : IComparable<Triple<F, S, T>>
	{
		#region Variables

		public F First;
		public S Second;
		public T Third;

		#endregion

		#region Construction

		[DebuggerStepThrough]
		public Triple(F First, S Second, T Third)
		{
			this.First = First;
			this.Second = Second;
			this.Third = Third;
		}

		#endregion

		#region Methods

		[DebuggerStepThrough]
		public override bool Equals(object obj)
		{
			return obj is Triple<F, S, T> && Equals((Triple<F, S, T>) obj);
		}

		[DebuggerStepThrough]
		public override int GetHashCode()
		{
			var hash1 = First == null ? 1 : First.GetHashCode();
			var hash2 = Second == null ? 2 : Second.GetHashCode();
			var hash3 = Third == null ? 3 : Third.GetHashCode();
			return Utility.CreateHashCode(hash1, hash2, hash3);
		}

		[DebuggerStepThrough]
		public override string ToString()
		{
			return First + ", " + Second + ", " + Third;
		}

		#endregion

		#region IComparable<Triple<F,S,T>> Members

		[DebuggerStepThrough]
		public int CompareTo(Triple<F, S, T> other)
		{
			var cmp = Comparer<F>.Default.Compare(First, other.First);
			if (cmp != 0) return cmp;
			cmp = Comparer<S>.Default.Compare(Second, other.Second);
			if (cmp != 0) return cmp;
			return Comparer<T>.Default.Compare(Third, other.Third);
		}

		#endregion

		[DebuggerStepThrough]
		public bool Equals(Triple<F, S, T> other)
		{
			return EqualityComparer<F>.Default.Equals(other.First, First)
				   && EqualityComparer<S>.Default.Equals(other.Second, Second)
				   && EqualityComparer<T>.Default.Equals(other.Third, Third);
		}

		#region Operators

		public static bool operator ==(Triple<F, S, T> a, Triple<F, S, T> b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Triple<F, S, T> a, Triple<F, S, T> b)
		{
			return !a.Equals(b);
		}

		#endregion
	}

	#endregion

	#region Tuple

	public static class Ntuple
	{
		[DebuggerStepThrough]
		public static Pair<F, S> New<F, S>(F First, S Second)
		{
			return new Pair<F, S>(First, Second);
		}

		[DebuggerStepThrough]
		public static Triple<F, S, T> New<F, S, T>(F First, S Second, T third)
		{
			return new Triple<F, S, T>(First, Second, third);
		}

		#endregion
	}
}