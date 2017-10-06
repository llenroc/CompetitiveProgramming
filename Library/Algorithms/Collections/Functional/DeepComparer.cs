using System;
using System.Collections;
using System.Collections.Generic;

namespace Softperson.Collections
{
	public class DeepComparer : IComparer, IComparer<object>
	{
		public int Compare(object data1, object data2)
		{
			if (data1 == data2)
				return 0;
			if (data1 == null)
				return -1;
			if (data2 == null)
				return 1;
			if (data1.Equals(data2))
				return 0;

			var cmp = CompareTools.CompareTypes(data1.GetType(), data2.GetType());
			if (cmp != 0)
				return cmp;

			var comparable = data1 as IComparable;
			if (comparable == null)
			{
				comparable = data1.ToString();
				data2 = data2.ToString();
			}

			return comparable.CompareTo(data2);
		}

		int IComparer<object>.Compare(object x, object y)
		{
			return Compare(x, y);
		}
	}
}