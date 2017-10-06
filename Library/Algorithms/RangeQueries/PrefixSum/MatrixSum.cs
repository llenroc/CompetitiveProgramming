using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Algorithms.RangeQueries
{

    // https://www.topcoder.com/community/data-science/data-science-tutorials/range-minimum-query-and-lowest-common-ancestor/

    public class MatrixSum
    {
        readonly int[,] _matrix;

        public MatrixSum(int[,] matrix, bool inplace = false)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (!inplace)
                matrix = (int[,])matrix.Clone();

            _matrix = matrix;

            for (int i = 1; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] += matrix[i - 1, j];

            for (int i = 0; i < rows; i++)
                for (int j = 1; j < cols; j++)
                    matrix[i, j] += matrix[i, j - 1];
        }

        public int GetSum(int x, int y, int dx, int dy)
        {
            return GetSumInclusive(x, y, x + dx - 1, y + dy - 1);
        }


        public int GetSumInclusive(int x1, int y1, int x2, int y2)
        {
            int result = _matrix[x2, y2];

            if (x1 > 0)
                result -= _matrix[x1 - 1, y2];

            if (y1 > 0)
                result -= _matrix[x2, y1 - 1];

            if (x1 > 0 && y1 > 0)
                result += _matrix[x1 - 1, y1 - 1];

            return result;
        }
    }
}
