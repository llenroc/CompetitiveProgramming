﻿namespace Softperson.Algorithms.RangeQueries
{

    // https://discuss.leetcode.com/topic/30343/java-2d-binary-indexed-tree-solution-clean-and-short-17ms


    // Your NumMatrix object will be instantiated and called as such:
    // NumMatrix numMatrix = new NumMatrix(matrix);
    // numMatrix.SumRegion(0, 1, 2, 3);
    // numMatrix.Update(1, 1, 10);
    // numMatrix.SumRegion(1, 2, 3, 4);

    public class Fenwick2d
    {
        int[,] mat;
        int[,] tree;
        int rows;
        int cols;

        public Fenwick2d(int[,] matrix)
        {

            if (matrix == null)
                matrix = new int[0, 0];

            rows = matrix.GetLength(0);
            cols = matrix.GetLength(1);
            mat = new int[rows, cols];
            tree = new int[rows + 1, cols + 1];

            for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                Update(r, c, matrix[r, c]);
        }

        public void Update(int row, int col, int val)
        {
            int delta = val - mat[row, col];
            mat[row, col] = val;

            for (int i = row + 1; i < rows; i += (i & -i))
            for (int j = col + 1; j < cols; j += (j & -j))
                tree[i, j] += delta;

        }

        public int SumRegion(int row1, int col1, int row2, int col2)
        {
            return Sum(row2 + 1, col2 + 1) + Sum(row1, col1) - Sum(row1, col2 + 1) - Sum(row2 + 1, col1);
        }

        public int Sum(int row, int col)
        {
            int sum = 0;
            for (int i = row; i > 0; i -= i & -i)
            for (int j = col; j > 0; j -= j & -j)
                sum += tree[i, j];
            return sum;
        }
    }
}