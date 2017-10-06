using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms
{
	public class MergeAlgorithms
	{
		private int[] temp;
		private int[] arr;

		/* This function sorts the input array and returns the
           number of inversions in the array */

		public MergeAlgorithms(int[] arr)
		{
			this.arr = arr;
			temp = new int[arr.Length];
		}


		public long MergeSort()
		{
			return MergeSortCore(0, arr.Length - 1);
		}

		public long MergeSortCore(int left, int right)
		{
			long result = 0;
			if (right > left)
			{
				int mid = (right + left) >> 1;
				result = MergeSortCore(left, mid);
				result += MergeSortCore(mid + 1, right);
				result += Merge(left, mid + 1, right);
			}
			return result;
		}

		public long Merge(int left, int mid, int right)
		{
			long result = 0;
			var i = left;
			var j = mid;
			var k = left;
			while (i <= mid - 1 && j <= right)
			{
				if (arr[i] <= arr[j])
					temp[k++] = arr[i++];
				else
				{
					temp[k++] = arr[j++];
					result = result + (mid - i);
				}
			}

			Array.Copy(arr, i, temp, k, mid - i);
			Array.Copy(arr, j, temp, k + mid - i, right - j + 1);
			Array.Copy(temp, left, arr, left, right - left + 1);
			return result;
		}


		/*	
				public void CountOfRangeSum()
				{
					long[] sums = new long[arr.Length + 1];
					for (int i = 1; i < sums.Length; i++)
						sums[i] = sums[i - 1] + arr[i - 1];
					long[] buffer = new long[sums.Length];

					// IMPORTANT: Use start index of 1 instead of 0
					return MergeSort(sums, buffer, 1, sums.Length - 1, upper);
				}

				int MergeSort(long[] sums, long[] buffer, int left, int right, long upper)
				{
					if (left > right) return 0;
					if (left == right) return sums[left] <= upper ? 1 : 0;

					int mid = left + (right - left) / 2;
					int leftHalf = MergeSort(sums, buffer, left, mid, upper);
					int rightHalf = MergeSort(sums, buffer, mid + 1, right, upper);

					int count = 0;

					// Count ranges
					for (int i = left, r = mid + 1; i <= mid; i++)
					{
						while (r <= right && sums[r] - sums[i] <= upper) r++;
						count += r - (mid + 1);
					}

					// Merge ranges
					int j = mid + 1;
					for (int i = left, k = left; k <= right; k++)
						buffer[k] = sums[i <= mid && (j > right || sums[i] < sums[j]) ? i++ : j++];

					Array.Copy(buffer, left, sums, left, right - left + 1);
					return leftHalf + rightHalf + count;
				}
		*/
	}
}
