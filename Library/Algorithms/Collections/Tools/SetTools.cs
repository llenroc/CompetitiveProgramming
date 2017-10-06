﻿#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion

namespace Softperson.Collections
{
	public static class SetTools
	{
		public static IEnumerable<T> Union<T>(IEnumerable<T> first, IEnumerable<T> second)
		{
			var set = new HashSet<T>();

			foreach (var tmp in first)
			{
				if (set.Contains(tmp))
					continue;
				set.Add(tmp);
				yield return tmp;
			}

			foreach (var tmp in second)
			{
				if (set.Contains(tmp))
					continue;
				set.Add(tmp);
				yield return tmp;
			}
		}

		public static IEnumerable<T> Intersection<T>(IEnumerable<T> first, IEnumerable<T> second)
			where T : class
		{
			var set = new HashSet<T>();

			foreach (var tmp in second)
				set.Add(tmp);

			foreach (var tmp in first)
				if (set.Contains(tmp))
				{
					set.Remove(tmp);
					yield return tmp;
				}
		}

		[DebuggerStepThrough]
		public static bool Intersects<T>(this IEnumerable<T> first, IList<T> second)
		{
			return first.Any(second.Contains);
		}

		public static IEnumerable<T> Difference<T>(IEnumerable<T> first, IEnumerable<T> second)
		{
			var set = new HashSet<T>();

			foreach (var tmp in second)
				set.Add(tmp);

			return first.Where(tmp => !set.Contains(tmp));
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> Union<T>(this IEnumerable<IEnumerable<T>> listOfLists)
		{
			return listOfLists.Aggregate(Enumerable.Union);
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> Intersection<T>(this IEnumerable<IEnumerable<T>> listOfLists)
		{
			return listOfLists.Aggregate(Enumerable.Intersect);
		}

		[DebuggerStepThrough]
		public static HashSet<T> ToSet<T>(this IEnumerable<T> collection)
		{
			return new HashSet<T>(collection);
		}

		[DebuggerStepThrough]
		public static HashSet<U> ToSet<T, U>(this IEnumerable<T> collection, Func<T, U> func)
		{
			var newSet = new HashSet<U>();
			foreach (var item in collection)
			{
				var newItem = func(item);
				if (newItem != null)
					newSet.Add(newItem);
			}
			return newSet;
		}

		[DebuggerStepThrough]
		public static HashSet<T> Clone<T>(this HashSet<T> collection)
		{
			return new HashSet<T>(collection);
		}

		//}
		//	return b.ToString();
		//	b.Append("{ ").Join(set, ", ", s => s.ToString()).Append(" }");
		//	var b = new StringBuilder();
		//{

		//public static string WriteToString<T>(HashSet<T> set)
	}
}