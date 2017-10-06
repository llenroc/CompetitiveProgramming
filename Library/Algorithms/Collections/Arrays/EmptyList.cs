#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace Softperson.Collections
{
	[DebuggerStepThrough]
	public sealed class EmptyList<T> : IList<T>, IEnumerator<T>, IList
	{
		public static readonly EmptyList<T> Instance = new EmptyList<T>();
		public static readonly T[] Array = new T[0];

		#region Construction

		private EmptyList()
		{
		}

		#endregion

		#region IEnumerator<T> Members

		public T Current
		{
			get { throw new InvalidOperationException(); }
		}

		public void Dispose()
		{
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public bool MoveNext()
		{
			return false;
		}

		public void Reset()
		{
		}

		#endregion

		#region IList Members

		public int Add(object value)
		{
			throw new InvalidOperationException();
		}

		public bool Contains(object value)
		{
			return false;
		}

		public int IndexOf(object value)
		{
			return -1;
		}

		public void Insert(int index, object value)
		{
			throw new InvalidOperationException();
		}

		public bool IsFixedSize
		{
			get { return true; }
		}

		public void Remove(object value)
		{
		}

		object IList.this[int index]
		{
			get { throw new IndexOutOfRangeException(); }
			set { throw new IndexOutOfRangeException(); }
		}

		public void CopyTo(Array array, int index)
		{
		}

		public bool IsSynchronized
		{
			get { return true; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		#endregion

		#region IList<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}

		public void Add(T item)
		{
			throw new InvalidOperationException();
		}

		public void Clear()
		{
		}

		public bool Contains(T item)
		{
			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
		}

		public int Count
		{
			get { return 0; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool Remove(T item)
		{
			return false;
		}

		public int IndexOf(T item)
		{
			return -1;
		}

		public void Insert(int index, T item)
		{
			throw new InvalidOperationException();
		}

		public void RemoveAt(int index)
		{
			throw new InvalidOperationException();
		}

		public T this[int index]
		{
			get { throw new IndexOutOfRangeException(); }
			set { throw new IndexOutOfRangeException(); }
		}

		#endregion
	}
}