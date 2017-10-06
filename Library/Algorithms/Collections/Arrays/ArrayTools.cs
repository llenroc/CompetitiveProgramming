#region Copyright

//  This source code may not be reviewed, copied, or redistributed without
//  the expressed permission of Wesner Moise.
//  
//  File: ListTools.cs
//  Created: 05/31/2012 
//  Modified: 09/26/2012
// 
//  Copyright (C) 2012 - 2012, Wesner Moise.

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Softperson.Collections;

#endregion

namespace Softperson.Collections
{
	[DebuggerStepThrough]
	public static class ArrayTools
	{
		public static T[][] NewArray<T>(int n, int m)
		{
			var array = new T[n][];
			for (var i = 0; i < n; i++)
				array[i] = new T[m];
			return array;
		}

		public static T[][] ConvertArrayForm<T>(this T[,] array)
		{
			var n = array.GetLength(0);
			var m = array.GetLength(1);

			var result = NewArray<T>(n, m);
			for (var i = 0; i < n; i++)
				Array.Copy(array, i*n, result[n], 0, m);

			return result;
		}

		public static T[,] ConvertArrayForm<T>(this T[][] array)
		{
			var n = array.Length;
			var m = n > 0 ? array[0].Length : 0;

			var result = new T[n, m];
			for (var i = 0; i < n; i++)
				Array.Copy(array[n], 0, result, i*n, m);

			return result;
		}

		public static void SwapRow<T>(this T[,] array, int r1, int r2)
		{
			var m = array.GetLength(1);
			for (var i = 0; i < m; i++)
			{
				var tmp = array[r1, i];
				array[r1, i] = array[r2, i];
				array[r2, i] = tmp;
			}
		}

		[System.Diagnostics.Contracts.Pure]
		public static T[] CloneArray<T>(this T[] list)
		{
			if (list == null || list.Length == 0)
				return list;

			if (list.GetType() == typeof (T[]))
				return (T[]) list.Clone();

			var newList = new T[list.Length];
			Array.Copy(list, newList, list.Length);
			return newList;
		}

		public static T[] Map<T>(this T[] list, Func<T, T> func,
			IEqualityComparer<T> comparer = null)
		{
			if (list == null)
				return null;

			if (comparer == null)
				comparer = EqualityComparer<T>.Default;
			for (var i = 0; i < list.Length; i++)
			{
				var elem = list[i];
				var newElem = func(list[i]);
				if (!comparer.Equals(elem, newElem))
				{
					var newArray = list.CloneArray();
					newArray[i] = newElem;
					for (i++; i < newArray.Length; i++)
						newArray[i] = func(newArray[i]);
					return newArray;
				}
			}
			return list;
		}

		[JetBrains.Annotations.Pure]
		[NotNull]
		public static T[] EmptyArray<T>()
		{
			return EmptyList<T>.Array;
		}

		public static void RemoveRange<T>(T[] array, ref int size, int start, int count)
		{
			Utility.Assert(size >= 0);
			if (count == 0) return;
			if (start < 0 || start > size) throw new ArgumentOutOfRangeException("start");
			if (count < 0 || start + count > size) throw new ArgumentOutOfRangeException("count");

			size -= count;
			if (size > start)
				Array.Copy(array, start + count, array, start, size - start);
			Array.Clear(array, size, count);
		}

		[DebuggerStepThrough]
		public static void InsertRange<T>(ref T[] array, ref int size, int start, ICollection<T> collection)
		{
			InsertRange(ref array, ref size, start, collection.Count);
			collection.CopyTo(array, start);
		}

		public static void Insert<T>(ref T[] array, ref int size, int start, T obj)
		{
			InsertRange(ref array, ref size, start, 1);
			array[start] = obj;
		}

		[Pure]
		public static T[] Remove<T>(this T[] list, int position, int dropCount)
		{
			return Replace(list, position, dropCount);
		}

		[Pure]
		[PublicAPI]
		public static T[] ReplaceAt<T>(this T[] list, int position, T item)
		{
			var array = (T[]) list.Clone();
			array[position] = item;
			return array;
		}

		[Pure]
		public static T[] Insert<T>(this T[] list, int position, T obj)
		{
			if (list == null)
				return new[] {obj};
			var count = 0;
			ListTools.NormalizeRange(list, ref position, ref count);
			var result = new T[list.Length + 1];
			Array.Copy(list, 0, result, 0, position);
			result[position] = obj;
			Array.Copy(list, position, result, position + 1, list.Length - position);
			return result;
		}

		[Pure]
		public static T[] InsertRange<T>(T[] list, int position, params T[] range)
		{
			return Replace(list, position, 0, range);
		}

		[Pure]
		public static T[] Append<T>(this T[] list, T obj)
		{
			return Insert(list, list.Length, obj);
		}

		[Pure]
		public static T[] Replace<T>(this T[] list, int position, int count, params T[] insert)
		{
			return Replace(list, position, count, (IEnumerable<T>) insert);
		}

		[Pure]
		public static T[] Replace<T>(this T[] list, int position, int count, IEnumerable<T> insert)
		{
			if (list == null)
				list = EmptyArray<T>();

			ListTools.NormalizeRange(list, ref position, ref count);
			var end = position + count;

			var insertLength = insert != null ? insert.Count() : 0;
			if (count == 0 && insertLength == 0)
				return list;

			var newLength = list.Length - count + insertLength;
			if (newLength == 0)
				return EmptyArray<T>();

			var result = new T[list.Length - count + insertLength];
			Array.Copy(list, 0, result, 0, position);
			Array.Copy(list, end, result, position + insertLength, list.Length - end);

			if (insertLength != 0)
				insert.CopyTo(result, position);

			return result;
		}

		public static void Ensure<T>(ref T[] array, int n)
		{
			if (n < array.Length) return;
			var capacity = array.Length*2;
			if (capacity < n) capacity = n;
			Array.Resize(ref array, capacity + 4);
		}

		public static void InsertRange<T>(ref T[] array, ref int size, int start, int count)
		{
			Utility.Assert(size >= 0);
			if (count == 0) return;
			if (start < 0 || start > size) throw new ArgumentOutOfRangeException("start");
			if (count < 0) throw new ArgumentOutOfRangeException("count");

			var oldSize = size;
			size += count;
			Ensure(ref array, size);
			Array.Copy(array, start, array, start + count, oldSize - start);
			Array.Clear(array, start, count);
		}

		public static bool ArrayEqual<T>(T[] list1, T[] list2)
		{
			if (list1 == list2) return true;
			if (list1 == null || list2 == null) return false;

			var count = list1.Length;
			if (count != list2.Length)
				return false;

			var comparer = EqualityComparer<T>.Default;
			for (var i = 0; i < count; i++)
				if (!comparer.Equals(list1[i], list2[i]))
					return false;
			return true;
		}

		public static bool ArrayEqual<T>(ref T[] list1, ref T[] list2)
		{
			if (list1 == list2) return true;

			if (!ArrayEqual(list1, list2)) return false;
			Utility.Exchange(ref list1, ref list2);
			return true;
		}

		public static T[] New<T>(int size)
		{
			if (size == 0)
				return EmptyArray<T>();
			return new T[size];
		}

		public static T[] New<T>(int size, Func<int, T> func)
		{
			var array = New<T>(size);
			for (var i = 0; i < size; i++)
				array[i] = func(i);
			return array;
		}

		public static void InsertionSort(int[] arr, int n)
		{
			for (int i = 1; i < n; i++)
			{
				int j, key = arr[i];
				for (j = i - 1; j >= 0 && arr[j] > key; j--)
					arr[j + 1] = arr[j];
				arr[j + 1] = key;
			}
		}




		public static int FindKthElement(int[] nums, int k1Indexed)
		{
			int lo = 0;
			int hi = nums.Length - 1;
			int pos = k1Indexed-1;

			while (lo < hi)
			{
				int index = Partition(nums, lo, hi);
				if (pos < index)
					hi = index - 1;
				else if (pos > index)
					lo = index + 1;
				else
					break;
			}

			return nums[pos];
		}

		static int Partition(int[] nums, int rangeStart, int rangeEnd)
		{
			int index = (rangeStart + rangeEnd) >> 1;
			int start = rangeStart + 1;
			int end = rangeEnd;

			int median = nums[index];
			nums[index] = nums[rangeStart];

			while (start <= end)
			{
				while (start <= end && nums[start] <= median) start++;
				while (start <= end && nums[end] > median) end--;
				if (start < end)
				{
					int tmp = nums[start];
					nums[start++] = nums[end];
					nums[end--] = tmp;
				}
			}

			int pivotLoc = start - 1;
			nums[rangeStart] = nums[pivotLoc];
			nums[pivotLoc] = median;
			return pivotLoc;
		}
	}
}