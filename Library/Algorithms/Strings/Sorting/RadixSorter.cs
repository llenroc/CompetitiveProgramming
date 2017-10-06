using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Softperson.Algorithms
{
	public class RadixSorter
	{

		public static void RadixSort(IList<int> list)
		{
			const long adjust = int.MaxValue + 1L;
			RadixSort(list, 
				x=>x + adjust, 
				null,
				list.Max() + adjust);
		}

		public static void RadixSort<T>(IList<T> list, 
			Func<T,long> func, 
			T[] buffer = null,
			long maxValue = int.MaxValue)
		{
			int shift0 = 8;
			int buckets0 = 1 << shift0;
			int mask = buckets0-1;
			var offsets = new int[buckets0];

			if (buffer == null || buffer.Length<list.Count)
				buffer = new T[list.Count];

			for (int shift = 0; maxValue>>shift > 0; shift += shift0)
			{
				long buckets = Min(buckets0, (maxValue>>shift)+1);
				for (var i = 0; i < buckets; i++)
					offsets[i] = 0;

				for (var i = 0; i < list.Count; i++)
				{
					var radix = func(list[i]) >> shift & mask;
					offsets[radix]++;
				}

				var sum = 0;
				for (var i = 0; i < buckets; i++)
				{
					var newSum = sum + offsets[i];
					offsets[i] = sum;
					sum = newSum;
				}

				for (var i = 0; i < list.Count; i++)
				{
					var index = func(list[i]);
					buffer[offsets[index]++] = list[i];
				}

				for (var i = 0; i < list.Count; i++)
					list[i] = buffer[i];
			}
		}

		public void RadixSort(string[] array, int start, int count)
		{
			var srs = new StringRadixSort
			{
				_array = array,
				_buffer = new string[array.Length]
			};

			srs.Sort(start, start + count - 1, 0);
		}


		struct StringRadixSort
		{
			public string[] _array;
			public string[] _buffer;


			public unsafe void Sort(int left, int right, int index)
			{
				if (left >= right || index >= _array[left].Length)
					return;

				int buckets = 256;
				int* offsets = stackalloc int[buckets];
				int* pos = stackalloc int[buckets];

				while (left < right && index < _array[left].Length)
				{
					for (int i = 0; i < 10; i++)
						offsets[i] = 0;

					for (int i = left; i <= right; i++)
						offsets[_array[i][index] & 0xff]++;

					int sum = left;
					int maxrange = 0;
					int maxradix = 0;
					for (int i = 0; i < 10; i++)
					{
						int range = offsets[i];
						if (range >= maxrange)
						{
							maxrange = range;
							maxradix = i;
						}

						offsets[i] = sum;
						pos[i] = sum;
						sum += range;
					}

					for (int i = left; i <= right; i++)
					{
						int radix = _array[i][index] - '0';
						_buffer[pos[radix]++] = _array[i];
					}

					for (int i = left; i <= right; i++)
						_array[i] = _buffer[i];

					index++;
					for (int i = 0; i < 10; i++)
					{
						if (i != maxradix)
							Sort(offsets[i], pos[i] - 1, index);
					}

					// This reduces recursion since index can go up to 10^6
					left = offsets[maxradix];
					right = pos[maxradix] - 1;
				}
			}
		}
	}
}
