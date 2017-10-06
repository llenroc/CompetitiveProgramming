#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2005, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

#endregion

namespace Softperson
{
	public static class CompareTools
	{
		[Pure]
		public static GenericComparer<T> ToComparer<T>(this Comparison<T> comparison)
		{
			return new GenericComparer<T>(comparison);
		}

		[Pure]
		[PublicAPI]
		public static Comparison<T> Descending<T>(this Comparison<T> compare)
		{
			return (a, b) => compare(b, a);
		}

		[Pure]
		[PublicAPI]
		public static Comparison<T> Descending<T, TKey>(this Func<T, TKey> keyFunc)
		{
			var comparable = Comparer<TKey>.Default;
			return (a, b) => comparable.Compare(keyFunc(b), keyFunc(a));
		}

		[Pure]
		[PublicAPI]
		public static Comparison<T> ToComparison<T, TKey>(this Func<T, TKey> keyFunc)
		{
			var comparable = Comparer<TKey>.Default;
			return (a, b) => comparable.Compare(keyFunc(a), keyFunc(b));
		}

		[Pure]
		[PublicAPI]
		public static GenericComparer<T> ToComparer<T, TKey>(this Func<T, TKey> keyFunc)
		{
			return keyFunc.ToComparison().ToComparer();
		}

		public static int StandardCompare(object x, object y)
		{
			if (x == y) return 0;
			if (x == null) return -1;
			if (y == null) return 1;
			if (x.Equals(y))
				return 0;

			return StandardComparePostCheck(x, y);
		}


		public static int StandardComparePostCheck(object x, object y)
		{
			int cmp;
			var t1 = x.GetType();
			var t2 = y.GetType();
			if (t1 != t2)
			{
				cmp = t1.GetHashCode().CompareTo(t2.GetHashCode());
				if (cmp != 0)
					return cmp;
			}

			cmp = x.GetHashCode().CompareTo(y.GetHashCode());
			if (cmp != 0)
				return cmp;

			// At this point, this is as stable as we are ever going to get
			return Utility.IsOlder(x, y) ? -1 : 1;
		}

		//public class ListEqualityComparer<T> : EqualityComparer<IList<T>>
		//{
		//    public override bool Equals(IList<T> item, IList<T> key)
		//    {
		//        if (item == key)
		//            return true;

		//        IList<T> itemList = item as IList<T>;
		//        IList<T> keyList = key as IList<T>;

		//        if (((object)itemList == null) != ((object)keyList == null)) 
		//            return false;

		//        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
		//        if ((object)itemList == null) 
		//            return comparer.Equals(item, key);

		//        int count = itemList.Count;
		//        if (itemList.Count != keyList.Count)
		//            return false;

		//        for (int i = 0; i < count; i++)
		//            if (!comparer.Equals(itemList[i], keyList[i]))
		//                return false;

		//        return true;
		//    }

		//    public override int GetHashCode(T key)
		//    {
		//        if (key == null)
		//            return 0;

		//        IList<T> list = key as IList<T>;
		//        if (list == null)
		//            return key.GetHashCode();

		//        int count = list.Count;
		//        int hash = count ^ (count << 8) ^ (count << 16);
		//        for (int i = 0; i < count; i++)
		//            hash = ((hash >> 10) | (hash << 22)) + (GetHashCode(list[i]) ^ i);
		//        return hash;
		//    }
		//}
		public static int CompareTypes(Type type1, Type type2)
		{
			if (type1 == type2)
				return 0;
			if (type1 == null)
				return -1;
			if (type2 == null)
				return 1;
			return string.CompareOrdinal(type1.Name, type2.Name);
		}

		#region Nested type: GenericComparer

		public class GenericComparer<T> : IComparer<T>, IComparer
		{
			#region Variables

			private readonly Comparison<T> _comparison;

			#endregion

			#region Construction

			public GenericComparer(Comparison<T> comparison)
			{
				_comparison = comparison;
			}

			#endregion

			#region IComparer Members

			[DebuggerStepThrough]
			public int Compare(object x, object y)
			{
				return Compare((T) x, (T) y);
			}

			#endregion

			#region IComparer<T> Members

			[DebuggerStepThrough]
			public int Compare(T x, T y)
			{
				if (x == null)
					return y == null ? 0 : -1;
				if (y == null)
					return 1;
				return _comparison(x, y);
			}

			#endregion
		}

		#endregion

		#region Nested type: GenericEqualityComparer

		public class GenericEqualityComparer<T> : IEqualityComparer<T>
		{
			#region Variables

			private readonly Func<T, T, bool> comparison;

			#endregion

			#region Construction

			public GenericEqualityComparer(Func<T, T, bool> comparison)
			{
				this.comparison = comparison;
			}

			#endregion

			#region IEqualityComparer<T> Members

			[DebuggerStepThrough]
			public bool Equals(T x, T y)
			{
				return comparison(x, y);
			}

			[DebuggerStepThrough]
			public int GetHashCode(T obj)
			{
				return obj.GetHashCode();
			}

			#endregion
		}

		#endregion

		#region Nested type: ReferenceComparer

		public static IEqualityComparer<object> ReferenceComparer = new ReferenceComparerOf<object>();

		public static IEqualityComparer<T> GetReferenceComparer<T>()
			where T : class
		{
			var comparer = ReferenceComparer as IEqualityComparer<T>;
			if (comparer != null)
				return comparer;

			return new ReferenceComparerOf<T>();
		}

		private class ReferenceComparerOf<T> : IEqualityComparer<T>, IEqualityComparer
			where T : class
		{
			[DebuggerStepThrough]
			public new bool Equals(object x, object y)
			{
				return x == y;
			}

			[DebuggerStepThrough]
			public int GetHashCode(object obj)
			{
// ReSharper disable ConditionIsAlwaysTrueOrFalse
				if (obj == null) return 0;
// ReSharper restore ConditionIsAlwaysTrueOrFalse
				return obj.GetHashCode();
			}

			#region IEqualityComparer<T> Members

			[DebuggerStepThrough]
			public bool Equals(T x, T y)
			{
				return x == y;
			}

			[DebuggerStepThrough]
			public int GetHashCode(T obj)
			{
// ReSharper disable ConditionIsAlwaysTrueOrFalse
				if (obj == null) return 0;
// ReSharper restore ConditionIsAlwaysTrueOrFalse
				return obj.GetHashCode();
			}

			#endregion
		}

		#endregion
	}
}