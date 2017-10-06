using System;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace Softperson.Algorithms
{
	public static class STL
	{
		public const int RandMax = int.MaxValue - 1;

		public static int size<T>(this IList<T> list)
		{
			return list.Count;
		}

		public static bool empty<T>(this ICollection<T> list)
		{
			return list.Count == 0;
		}

		public static Iterator<T> erase<T>(this IList<T> list, Iterator<T> position)
		{
			list.RemoveAt(position.Index);
			return position;
		}

		public static Iterator<T> erase<T>(this List<T> list, Iterator<T> begin, Iterator<T> end)
		{
			list.RemoveRange(begin.Index, end.Index - begin.Index);
			return begin;
		}

		public static void clear<T>(this ICollection<T> list)
		{
			list.Clear();
		}

		public static void push_back<T>(this IList<T> list, T value)
		{
			list.Add(value);
		}

		public static void push_front<T>(this IList<T> list, T value)
		{
			list.Insert(0, value);
		}

		public static T pop_back<T>(this IList<T> list)
		{
			var last = list.Count - 1;
			var pop = list[last];
			list.RemoveAt(last);
			return pop;
		}

		public static T pop_front<T>(this IList<T> list)
		{
			var pop = list[0];
			list.RemoveAt(0);
			return pop;
		}

		public static T back<T>(this IList<T> list)
		{
			return list[list.Count - 1];
		}

		public static T front<T>(this IList<T> list)
		{
			return list[0];
		}

		public static void resize<T>(this List<T> list, int count, T value = default(T))
		{
			var oldcount = list.Count;
			if (count <= oldcount)
			{
				list.RemoveRange(count, oldcount - count);
				return;
			}

			if (count > list.Capacity)
				list.Capacity = count;

			for (var i = oldcount; i < count; i++)
				list.Add(value);
		}

		public static List<T> Repeat<T>(int n, T value = default(T))
		{
			var list = new List<T>(n);
			for (var i = 0; i < n; i++)
				list.Add(value);
			return list;
		}

		public static List<T> Repeat<T>(int n, Func<T> action)
		{
			var list = new List<T>(n);
			for (var i = 0; i < n; i++)
				list.Add(action());
			return list;
		}

		public static void Fill<T>(this IList<T> list, T value)
		{
			Fill(list, 0, list.Count, value);
		}

		public static void Fill<T>(this IList<T> list, int start, int end, T value)
		{
			for (var i = start; i < end; i++)
				list[i] = value;
		}

		public static Iterator<T> begin<T>(this IList<T> list)
		{
			return new Iterator<T>(list, 0);
		}

		public static Iterator<T> end<T>(this IList<T> list)
		{
			return new Iterator<T>(list, list.Count);
		}

		public static int distance<T>(Iterator<T> begin, Iterator<T> end)
		{
			return end.Index - begin.Index;
		}

		public static void Advance<T>(Iterator<T> iterator)
		{
			iterator.Index++;
		}

		public static Iterator<T> prev<T>(Iterator<T> iterator)
		{
			return iterator.Previous;
		}

		public static Iterator<T> next<T>(Iterator<T> iterator)
		{
			return iterator.Next;
		}


		public static int lower_bound<T>(this IList<T> list, int start, int end, T val)
		{
			var comparer = Comparer<T>.Default;
			var min = start;
			var max = end - 1;
			while (min <= max)
			{
				var mid = (min + max)/2;
				var cmp = comparer.Compare(list[mid], val);
				if (cmp < 0)
					min = mid + 1;
				else
					max = mid - 1;
			}
			return min;
		}

		/// <summary>
		///     Returns an iterator pointing to the first element in the range
		///     [first,last) which compares greater than val.
		///     The elements are compared using operator &lt; for the
		///     first version, and comp for the second. The elements
		///     in the range shall already be sorted according to this
		///     same criterion (operator lt or comp), or at least
		///     partitioned with respect to val.
		///     The function optimizes the number of comparisons performed
		///     by comparing non-consecutive elements of the sorted range,
		///     which is specially efficient for random-access iterators.
		///     Unlike lower_bound, the value pointed by the iterator
		///     returned by this function cannot be equivalent to val, only greater.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public static int upper_bound<T>(this IList<T> list, int start, int end, T val)
		{
			var comparer = Comparer<T>.Default;
			var min = start;
			var max = end - 1;
			while (min <= max)
			{
				var mid = (min + max)/2;
				var cmp = comparer.Compare(list[mid], val);
				if (cmp <= 0)
					min = mid + 1;
				else
					max = mid - 1;
			}
			return max;
		}


		public static int unique<T>(this IList<T> list)
		{
			return list.unique(0, list.Count);
		}

		public static int unique<T>(this IList<T> list, int start, int end)
		{
			var read = start + 1;

			var comparer = EqualityComparer<T>.Default;
			while (read < end && !comparer.Equals(list[read - 1], list[read]))
			{
				read++;
			}

			var write = read;
			while (read < end)
			{
				if (!comparer.Equals(list[write - 1], list[read]))
					list[write++] = list[read];
				read++;
			}

			return write;
		}

		public static Iterator<T> unique<T>(Iterator<T> begin, Iterator<T> end)
		{
			var list = begin.List;
			var start = begin.Index;
			var count = end.Index - begin.Index;
			var newEnd = unique(list, start, count);
			return new Iterator<T>(list, newEnd);
		}

		public static void swap<T>(this IList<T> list, int i, int j)
		{
			var tmp = list[i];
			list[i] = list[j];
			list[j] = tmp;
		}

		public static int rand()
		{
			var random = new Random();
			return random.Next();
		}

		public static Pair<T1, T2> make_pair<T1, T2>(T1 item1, T2 item2)
		{
			return new Pair<T1, T2> {First = item1, Second = item2};
		}
	}
}