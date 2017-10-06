using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.RangeQueries
{
	// http://ideone.com/zWJ0pq
	// http://codeforces.com/blog/entry/18051

	/// <summary>
	/// Fast bottom up segment tree
	/// </summary>
	public class SegmentTree
	{
		private readonly int[] _tree;

		public SegmentTree(int size)
		{
			_tree = new int[size * 2];
		}

		public SegmentTree(int[] array)
			: this(array.Length)
		{
			int size = array.Length;
			Array.Copy(array, 0, _tree, size, array.Length);
			for (int i = size - 1; i > 0; i--)
				_tree[i] = _tree[i << 1] + _tree[i << 1 | 1];
		}

		public void Update(int index, int value)
		{
			int i =  index + _tree.Length/2;
			_tree[i] += value;
			for (; i > 1; i >>= 1)
				_tree[i >> 1] = _tree[i] + _tree[i ^ 1];
		}

		public int this[int index]
		{
			get
			{
				return _tree[index + (_tree.Length >> 1)];
			}
			set
			{
				int i = index + (_tree.Length >> 1);
				_tree[i] = value;

				for (; i > 1; i >>= 1)
					_tree[i >> 1] = _tree[i] + _tree[i ^ 1];

				// For O(1) updates for min, max queries
				//var min = value;
				//for (; i > 1; i >>= 1)
				//{
				//	var newMin = Math.Min(_tree[i], _tree[i ^ 1]);
				//	if (_tree[i >> 1] == newMin) break;
				//	_tree[i >> 1] = newMin;
				//}
			}
		}

		public int Query(int start, int count)
		{
			return QueryExclusive(start, start + count);
		}

		public int QueryExclusive(int left, int right)
		{
			// Version in http://codeforces.com/blog/entry/18051
			int size = _tree.Length >> 1;
			left += size;
			right += size; 

			int result = 0;
			for (; left < right; left >>= 1, right >>= 1)
			{
				if ((left & 1) == 1) result += _tree[left++]; 
				if ((right & 1) == 1) result += _tree[--right];
			}
			return result;
		}

		public int QueryInclusive(int left, int right)
		{
			int size = _tree.Length >> 1;
			left += size;
			right += size; 

			int result = 0;
			for (; left <= right; left >>= 1, right >>= 1)
			{
				if ((left & 1) == 1) result += _tree[left++]; 
				if ((right & 1) == 0) result += _tree[right--]; 
			}
			return result;
		}

		public void RangeModify(int start, int count, int value)
		{
			RangeModifyExclusive(start, start + count, value);
		}

		public void RangeModifyExclusive(int left, int right, int value)
		{
			int size = _tree.Length >> 1;
			left += size;
			right +=  size; 
			for (; left < right; left >>= 1, right >>= 1)
			{
				if ((left & 1) == 1) _tree[left++] += value;
				if ((right & 1) == 1) _tree[--right] += value;
			}
		}

		public int RangeQuery(int index)
		{
			int result = 0;
			int i = index + _tree.Length >> 1;
			for (; i > 0; i >>= 1) result += _tree[i];
			return result;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach (int t in _tree)
			{
				sb.Append(t);
				sb.Append(',');
			}
			return sb.ToString();
		}
	}
}
