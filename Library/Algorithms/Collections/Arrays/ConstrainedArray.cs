using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Collections
{
    public struct ConstrainedArray<T> : IList<T>
    {
        #region Variables
        int _lowerBound;
        T[] _array;
        #endregion

        #region Constructor
        public ConstrainedArray(int min, int max)
        {
            _lowerBound = min;
            _array = new T[max - min + 1];
        }
        #endregion

        #region Properties
        public int Count => _array.Length;

        public int Length => _array.Length;

        bool ICollection<T>.IsReadOnly => false;
        #endregion

        #region Methods
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _array.GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            ((IList<T>)_array).Add(item);
        }

        public void Clear()
        {
            ((IList<T>)_array).Clear();
        }

        public bool Contains(T item)
        {
            return _array.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _array.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item)
        {
            return ((IList<T>)_array).Remove(item);
        }

        
        public int IndexOf(T item)
        {
            return Array.IndexOf(_array, item);
        }

        void IList<T>.Insert(int index, T item)
        {
            ((IList<T>)_array).Insert(index, item);
        }

        void IList<T>.RemoveAt(int index)
        {
            ((IList<T>)_array).RemoveAt(index);
        }

        public T this[int index]
        {
            get { return _array[index-_lowerBound]; }
            set { _array[index-_lowerBound] = value; }
        }
        #endregion
    }
}
