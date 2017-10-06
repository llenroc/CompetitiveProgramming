using System;

namespace Softperson.Algorithms.RangeQueries
{
	public class SegmentTreeRangeUpdate
	{
		private readonly long[] _leaf;
		private readonly int _origSize;
		private readonly long[] _update;

		public SegmentTreeRangeUpdate(int[] list)
		{
			_origSize = list.Length;
			_leaf = new long[4*list.Length];
			_update = new long[4*list.Length];
			Build(1, 0, list.Length - 1, list);
		}

		public void Build(int curr, int begin, int end, int[] list)
		{
			if (begin == end)
				_leaf[curr] = list[begin];
			else
			{
				var mid = (begin + end)/2;
				Build(2*curr, begin, mid, list);
				Build(2*curr + 1, mid + 1, end, list);
				_leaf[curr] = _leaf[2*curr] + _leaf[2*curr + 1];
			}
		}

		public void Update(int begin, int end, int val)
		{
			Update(1, 0, _origSize - 1, begin, end, val);
		}

		public void Update(int curr, int tBegin, int tEnd, int begin, int end, int val)
		{
			if (tBegin >= begin && tEnd <= end)
				_update[curr] += val;
			else
			{
				_leaf[curr] += (Math.Min(end, tEnd) - Math.Max(begin, tBegin) + 1)*val;
				var mid = (tBegin + tEnd)/2;
				if (mid >= begin && tBegin <= end)
					Update(2*curr, tBegin, mid, begin, end, val);
				if (tEnd >= begin && mid + 1 <= end)
					Update(2*curr + 1, mid + 1, tEnd, begin, end, val);
			}
		}

		public long Query(int begin, int end)
		{
			return Query(1, 0, _origSize - 1, begin, end);
		}

		public long Query(int curr, int tBegin, int tEnd, int begin, int end)
		{
			if (tBegin >= begin && tEnd <= end)
			{
				if (_update[curr] != 0)
				{
					_leaf[curr] += (tEnd - tBegin + 1)*_update[curr];
					if (2*curr < _update.Length)
					{
						_update[2*curr] += _update[curr];
						_update[2*curr + 1] += _update[curr];
					}
					_update[curr] = 0;
				}
				return _leaf[curr];
			}
			_leaf[curr] += (tEnd - tBegin + 1)*_update[curr];
			if (2*curr < _update.Length)
			{
				_update[2*curr] += _update[curr];
				_update[2*curr + 1] += _update[curr];
			}
			_update[curr] = 0;
			var mid = (tBegin + tEnd)/2;
			long ret = 0;
			if (mid >= begin && tBegin <= end)
				ret += Query(2*curr, tBegin, mid, begin, end);
			if (tEnd >= begin && mid + 1 <= end)
				ret += Query(2*curr + 1, mid + 1, tEnd, begin, end);
			return ret;
		}
	}
}