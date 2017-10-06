using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Softperson.Algorithms.RangeQueries
{

	// SOURCE: (Alternative Impl) http://www.geeksforgeeks.org/sqrt-square-root-decomposition-technique-set-1-introduction/

	public class SqrtDecomposition<T>
    {
        #region Variables

        readonly int _blockSize;
        readonly T[] _cache;
        readonly BitArray _inited;
        #endregion

        #region Constructor
        public SqrtDecomposition(int start, int length)
        {
            Start = start;
            Length = length;
            _blockSize = (int) Max(1, Ceiling(Sqrt(length)));
            var blockCount = (int) Ceiling(length/ (double)_blockSize);
            _cache = new T[blockCount];
            _inited = new BitArray(blockCount);
        }
        #endregion


        #region Properties
        public int Start { get; }

        public int Length { get; }
        #endregion


        #region Methods
        public int GetBlockStart(int blockIndex)
        {
            return Start + _blockSize*blockIndex;
        }

        public int GetBlock(int index)
        {
            return (index - Start)/ _blockSize;
        }

        public int GetPositionInBlock(int index)
        {
            return (index - Start)%_blockSize;
        }

        T ComputeBlock(int index)
        {
            if (_inited[index])
                return _cache[index];

            var result = ComputeRange(Start + index*_blockSize, _blockSize);

            _cache[index] = result;
            _inited[index] = true;
            return result;
        }

        public T Compute(int start, int size)
        {
            int end = start + size;
            int index1 = GetBlock(start);
            int index2 = GetBlock(end);

            // Case of one block
            if (index1 == index2)
            {
                return size==_blockSize 
                    ? ComputeBlock(index1) 
                    : ComputeRange(start, size);
            }

            int pos1 = GetPositionInBlock(start);
            T result = pos1 > 0 
                ? ComputeRange(start, _blockSize - pos1) 
                : ComputeBlock(index1);


            // Whole blocks in the middle
            for (int i=index1; i < index2; i++)
                result = CombineRanges(result, ComputeBlock(i));

            int pos2 = GetPositionInBlock(end);
            if (pos2 > 0)
            {
                result = CombineRanges(result,
                    ComputeRange(GetBlockStart(index2), pos2));
            }

            return result;
        }

        public Func<int, int, T> ComputeRange;

        public Func<T, T, T> CombineRanges;

        public void ComputeByPoint(Func<int, T> computePoint)
        {
            ComputeRange = (int start, int end) =>
            {
                var result = computePoint(start);
                for (int i = start + 1; i <= end; i++)
                {
                    T tmp = computePoint(i);
                    result = CombineRanges(result, tmp);
                }
                return result;
            };
        }
        #endregion
    }
}
