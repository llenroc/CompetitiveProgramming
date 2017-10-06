using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.RangeQueries
{

    // https://www.topcoder.com/community/data-science/data-science-tutorials/range-minimum-query-and-lowest-common-ancestor/

    public class ArraySum
    {
        public readonly int[] _array;

        public ArraySum(int[] array, bool inplace=false)
        {
            if (inplace == false)
                array = (int[]) array.Clone();

            _array = array;
            int sum = 0;
            for (int i = 1; i < array.Length; i++)
            {
                array[i] += sum;
                sum = array[i];
            }
        }
        
        public int GetSum(int start, int count)
        {
            return GetSumInclusive(start, start + count - 1);
        }

        public int GetSumExclusive(int x1, int x2)
        {
            return GetSumInclusive(x1, x2 - 1);
        }

        public int GetSumInclusive(int x1, int x2)
        {
            int result = _array[x2];
            if (x1 > 0) result -= _array[x1];
            return result;
        }

    }
}
