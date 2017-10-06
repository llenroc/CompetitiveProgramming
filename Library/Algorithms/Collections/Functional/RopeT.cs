﻿#region Using

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
using System.Threading;
using JetBrains.Annotations;

#endregion

namespace Softperson.Collections
{
	[DebuggerDisplay("Count={Count}, Nodes={NodeCount}")]
	[System.Diagnostics.Contracts.Pure]
	public abstract partial class Rope<T>
		: IList<T>, IEquatable<Rope<T>>, IIndexable<T>
	{
		#region Nested Types: ConcatVector

		[Serializable]
		[DebuggerDisplay("Count={Count}, Nodes={NodeCount}")]
		private sealed class ConcatRope : Rope<T>
		{
			public override int IndexOf(T element, int start, int count = int.MaxValue)
			{
				int v;
				var mid = _left.Count;

				if (start < mid)
				{
					v = _left.IndexOf(element, start, count);
					if (v >= 0)
						return v;
				}

				start -= mid;
				if (start < 0)
				{
					count += start;
					start = 0;
				}

				if (count > 0)
				{
					v = _right.IndexOf(element, start, count);
					if (v >= 0)
						return v + mid;
				}

				return -1;
			}

			#region Variables

			[DebuggerBrowsable(DebuggerBrowsableState.Never)] private const uint UnchangeableBit = 1U << 31;

			[DebuggerBrowsable(DebuggerBrowsableState.Never)] private const uint HeightMask = 0x0000003f;

			[DebuggerBrowsable(DebuggerBrowsableState.Never)] private const uint HashMask = 0x7fffffc0;

			[DebuggerBrowsable(DebuggerBrowsableState.Never)] private const int HashShift = 6;

			[NonSerialized] private uint _flags;

			private Rope<T> _left;
			private Rope<T> _right;

			#endregion

			#region Construction

			[DebuggerStepThrough]
			private ConcatRope(Rope<T> left, Rope<T> right)
			{
				_left = left;
				_right = right;
				Initialize();
			}

			private void Initialize()
			{
				Count = _left.Count + _right.Count;
				SetHeight(Math.Max(_left.Height, _right.Height) + 1);
			}

			private static Rope<T> ConcatTop(Rope<T> left, Rope<T> right, ConcatRope v)
			{
				var c1 = left.Height;
				if (c1 <= 0)
					return right;

				var c2 = right.Height;
				if (c2 <= 0)
					return left;

				var cmp = (c1 & ~1) - (c2 & ~1);
				if (cmp > 0)
				{
					var vl = left as ConcatRope;
					if (vl != null)
					{
						if (vl._left.Height > vl._right.Height)
							return Clone(vl, vl._left, Clone(v, vl._right, right));
						// return vl.MoveUpLeft(v);

						var vlr = vl._right as ConcatRope;
						if (vlr != null)
							return Clone(vlr, Clone(vl, vl._left, vlr._left), Clone(v, vlr._right, right));
						// return vlr.MoveUpRight(vl).MoveUpLeft(v);
					}
				}
				else if (cmp < 0)
				{
					var vr = right as ConcatRope;
					if (vr != null)
					{
						if (vr._right.Height > vr._left.Height)
							return Clone(vr, Clone(v, left, vr._left), vr._right);
						// return t.MoveUpRight(v);

						var vrl = vr._left as ConcatRope;
						if (vrl != null)
							return Clone(vrl, Clone(v, left, vrl._left), Clone(vr, vrl._right, vr._right));
						// return vrl.MoveUpLeft(vr).MoveUpRight(v);
					}
				}

				return Clone(v, left, right);
			}

			public static Rope<T> Concat(Rope<T> left, Rope<T> right, ConcatRope v = null)
			{
				var c1 = left.Height;
				if (c1 <= 0)
					return right;

				var c2 = right.Height;
				if (c2 <= 0)
					return left;

				var cmp = (c1 & ~1) - (c2 & ~1);
				if (cmp > 0)
				{
					var t = left as ConcatRope;
					if (t != null)
						return ConcatTop(t._left, Concat(t._right, right, v), t);
				}
				else if (cmp < 0)
				{
					var t = right as ConcatRope;
					if (t != null)
						return ConcatTop(Concat(left, t._left, v), t._right, t);
				}

				return Clone(v, left, right);
			}

			//private ConcatVector MoveUpLeft(ConcatVector parent)
			//{
			//    return Clone(this, _left, Clone(parent, _right, parent._right));
			//}

			//private ConcatVector MoveUpRight(ConcatVector parent)
			//{
			//    return Clone(this, Clone(parent, parent._left, _left), _right);
			//}

			protected override Rope<T> MapX(Func<T, T> func)
			{
				var left = _left.Map(func);
				var right = _right.Map(func);

				if (IsSealed() && left == _left && right == _right)
					return this;
				var v = new ConcatRope(left, right);
				return v;
			}

			protected override Rope<T2> MapX<T2>(Func<T, T2> func)
			{
				var v = new Rope<T2>.ConcatRope(_left.Map(func), _right.Map(func));
				return v;
			}

			protected override Rope<T> RemoveX(Func<T, bool> func)
			{
				var left = _left.RemoveX(func);
				var right = _right.RemoveX(func);

				if (left == _left && right == _right)
					return this;

				return left + right;
			}

			#endregion

			#region Properties

			protected override int Height => unchecked((int) (_flags & HeightMask) + 2);

			private void SetHeight(int value)
			{
				unchecked
				{
					var insert = value - 2;
					Utility.Assert((insert & ~HeightMask) == 0);
					_flags = (_flags & ~HeightMask)
							 | (uint) insert;
					Utility.Assert(Height == value);
				}
			}

			protected override int NodeCount => _left.NodeCount + _right.NodeCount + 1;

			public override T this[int index]
			{
				get
				{
					var leftCount = _left.Count;
					return index < leftCount ? _left[index] : _right[index - leftCount];
				}
				set
				{
					var leftCount = _left.Count;
					if (index < leftCount)
						_left[index] = value;
					else
						_right[index - leftCount] = value;
				}
			}

			#endregion

			#region Etc

			// DONE
			[DebuggerNonUserCode]
			private ConcatRope Clone()
			{
				var self = (ConcatRope) MemberwiseClone();
				self._flags = 0;
				return self;
			}

			public override int GetHashCode()
			{
				if (!IsSealed())
					Seal();

				var hash = unchecked((int) ((_flags & HashMask) >> HashShift));
				return MakeHashCode(hash);
			}

			[DebuggerNonUserCode]
			protected override Rope<T> Seal()
			{
				if (IsSealed())
					return this;
				OnSeal();
				Utility.Assert(IsSealed());
				return this;
			}

			private void OnSeal()
			{
				unchecked
				{
					_left.Seal();
					var hash = _left.GetHashCode();

					_right.Seal();
					hash ^= _right.GetHashCode();

					// Fill counts and hash code
					_flags = (uint) (UnchangeableBit
									 | (hash & HashMask)
									 | (_flags & HeightMask));

					Utility.Assert(IsSealed());
				}
			}

			// DONE
			[DebuggerNonUserCode]
			protected override bool IsSealed()
			{
				return _flags >= UnchangeableBit;
			}

			#endregion

			#region Methods

			public override void FindNode(int position,
				out Rope<T> node, out int nodeStart)
			{
				var leftCount = _left.Count;
				if (position < leftCount)
				{
					_left.FindNode(position, out node, out nodeStart);
				}
				else
				{
					_right.FindNode(position - leftCount, out node, out nodeStart);
					nodeStart += leftCount;
				}
			}

			[DebuggerStepThrough]
			private static ConcatRope Clone(
				ConcatRope self,
				Rope<T> left,
				Rope<T> right)
			{
				if (self == null)
					return new ConcatRope(left, right);

				if (self.IsSealed())
				{
					if (self._left == left && self._right == right)
						return self;
					Utility.Assert(self._left.IsSealed());
					Utility.Assert(self._right.IsSealed());
					self = self.Clone();
				}
				self._left = left;
				self._right = right;
				self.Initialize();
				return self;
			}

			protected override void Validate()
			{
				if (IsSealed())
				{
					Utility.Assert(_left.IsSealed());
					Utility.Assert(_right.IsSealed());
				}
			}

			[DebuggerStepThrough]
			protected override Rope<T> OnInsert(int index, Rope<T> insertion)
			{
				if (insertion.Count == 0)
					return this;

				var mid = _left.Count;
				if (index < mid)
					return Concat(_left.InsertX(index, insertion),
						_right,
						this);

				return Concat(_left,
					_right.InsertX(index - mid, insertion),
					this);
			}

			protected override Rope<T> OnCopy(int index, int count)
			{
				if (count <= 0)
					return Empty;

				var end = index + count;
				var mid = _left.Count;

				var leftStart = Math.Min(index, mid);
				var leftCount = Math.Min(end, mid) - leftStart;
				var rightStart = Math.Max(index - mid, 0);
				var rightCount = Math.Max(end - mid, 0) - rightStart;

				var left = _left.CopyX(leftStart, leftCount);
				var right = _right.CopyX(rightStart, rightCount);
				return left + right;
			}

			protected override Rope<T> OnRemove(int index, int count)
			{
				if (count <= 0)
					return this;

				var end = index + count;
				var mid = _left.Count;

				var leftStart = Math.Min(index, mid);
				var leftCount = Math.Min(end, mid) - leftStart;
				var rightStart = Math.Max(index - mid, 0);
				var rightCount = Math.Max(end - mid, 0) - rightStart;

				var left = _left.RemoveX(leftStart, leftCount);
				var right = _right.RemoveX(rightStart, rightCount);
				return Concat(left, right, this);
			}

			[DebuggerNonUserCode]
			public override void ForEach(Action<T> action)
			{
				_left.ForEach(action);
				_right.ForEach(action);
			}

			#endregion
		}

		#endregion

		#region Nested type: EmptyVector

		[Serializable]
		private sealed class EmptyRope : Rope<T>
		{
			public override T this[int index]
			{
				get { throw new InvalidOperationException(); }
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			protected override int Height => 0;

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			protected override int NodeCount => 0;

			protected override Rope<T> OnRemove(int position, int length)
			{
				return this;
			}

			protected override Rope<T> OnCopy(int position, int length)
			{
				return this;
			}

			protected override Rope<T> OnInsert(int position, Rope<T> treap)
			{
				return treap;
			}

			public override int IndexOf(T element, int start, int count = int.MaxValue)
			{
				return -1;
			}

			public override int GetHashCode()
			{
				return 0x23456789;
			}

			protected override Rope<T> MapX(Func<T, T> func)
			{
				return this;
			}

			protected override Rope<T2> MapX<T2>(Func<T, T2> func)
			{
				return Rope<T2>.Empty;
			}

			protected override Rope<T> RemoveX(Func<T, bool> func)
			{
				return this;
			}
		}

		#endregion

		#region Nested type: ArrayChunk

		/// <summary>
		///     Summary description for ArrayVector.
		/// </summary>
		[Serializable]
		private class ArrayRope : Rope<T>
		{
			#region Variables

			private ArrayInfo _info;

			#endregion

			[Serializable]
			private class ArrayInfo
			{
				public T[] Data;
				public int Used;
			}

			#region Construction

			public ArrayRope(T[] data, int start, int count)
				: this(data.CopyRange(start, count), count)
			{
			}

			private void Set(T[] array, int count)
			{
				Count = count;
				_info = new ArrayInfo {Data = array, Used = count};
			}

			private ArrayRope(T[] array, int count)
			{
				Set(array, count);
			}

			private ArrayRope(ArrayInfo info, int count)
			{
				_info = info;
				Count = count;
				Debug.Assert(count <= info.Data.Length);
			}

			protected override Rope<T> MapX(Func<T, T> func)
			{
				var comparer = EqualityComparer<T>.Default;
				var list = _info.Data;

				var count = Count;
				for (var i = 0; i < count; i++)
				{
					var elem = list[i];
					var newElem = func(list[i]);
					if (!comparer.Equals(elem, newElem))
					{
						var newArray = list.CopyRange(0, Count);
						newArray[i] = newElem;
						for (i++; i < count; i++)
							newArray[i] = func(newArray[i]);
						return new ArrayRope(newArray, Count);
					}
				}
				return this;
			}

			protected override Rope<T2> MapX<T2>(Func<T, T2> func)
			{
				var newData = new T2[Count];

				var count = Count;
				for (var i = 0; i < count; i++)
					newData[i] = func(this[i]);

				var v = new Rope<T2>.ArrayRope(newData, Count);
				return v;
			}

			#endregion

			#region String Management

			public override int IndexOf(T element, int start, int count = int.MaxValue)
			{
				if (start < 0)
				{
					count += start;
					start = 0;
				}

				if (start + count > Count)
					count = Count - start;

				return Array.IndexOf(_info.Data, element, start, count);
			}

			public override T this[int index]
			{
				get
				{
					if (index >= Count)
						throw new IndexOutOfRangeException();
					return _info.Data[index];
				}
			}

			protected override Rope<T> OnCopy(int position, int length)
			{
				if (length == 0)
					return Empty;
				return new ArrayRope(_info.Data, position, length);
			}

			protected override Rope<T> OnInsert(int position, Rope<T> node)
			{
				if (position == Count && position == _info.Used)
				{
					var nodeCount = node.Count;
					var totalCount = position + nodeCount;
					if (totalCount <= ChunkSize*2)
					{
						var arrayNode = node as ArrayRope;
						if (arrayNode != null
							&& Interlocked.CompareExchange(ref _info.Used,
								int.MaxValue, position) == position)
						{
							try
							{
								var data = Data;
								if (data.Length < totalCount)
								{
									var newCount =
										Math.Min(Math.Max(data.Length*2, totalCount),
											ChunkSize*2);
									Array.Resize(ref data, newCount);
								}
								Array.Copy(arrayNode.Data, 0, data, position, nodeCount);
								_info.Data = data;
								return new ArrayRope(_info, totalCount);
							}
							finally
							{
								_info.Used = totalCount;
							}
						}
					}
				}

				return base.OnInsert(position, node);
			}

			private T[] Data => _info.Data;

			public override int GetHashCode()
			{
				var hash = 0;
				var data = Data;
				for (var i = Count - 1; i >= 0; i--)
					hash ^= Utility.GetHashCode(data[i]);
				return MakeHashCode(hash);
			}

			#endregion
		}

		#endregion

		#region Variables

		[DebuggerBrowsable(DebuggerBrowsableState.Never)] public static readonly Rope<T> Empty = new EmptyRope().Seal();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private const int ChunkSize = 16;

		#endregion

		#region Constructor

		private Rope()
		{
		}

		[System.Diagnostics.Contracts.Pure]
		public static Rope<T> From(params T[] array)
		{
			return From(array, 0, array.Length);
		}

		[System.Diagnostics.Contracts.Pure]
		public static Rope<T> Repeat(int count, T value)
		{
			return RepeatX(count, value).Seal();
		}

		[System.Diagnostics.Contracts.Pure]
		public static Rope<T> Function(int start, int count, Func<int, T> map)
		{
			return FunctionX(start, count, map).Seal();
		}

		[System.Diagnostics.Contracts.Pure]
		protected static Rope<T> RepeatX(int count, T value)
		{
			if (count <= 0)
				return Empty;
			return new RunRope(count, value);
		}

		private static Rope<T> FunctionX(int start, int count, Func<int, T> map)
		{
			if (count <= 0)
				return Empty;
			return new FunctionRope(start, count, map);
		}

		[System.Diagnostics.Contracts.Pure]
		public static Rope<T> From(IEnumerable<T> list)
		{
			return FromX(list).Seal();
		}

		[System.Diagnostics.Contracts.Pure]
		private static Rope<T> FromX(IEnumerable<T> list)
		{
			var array = list as T[];
			if (array != null)
				return From(array);

			var node = list as Rope<T>;
			if (node != null)
				return node;

			var result = Empty;
			if (list == null)
				return result;

			var i = 0;
			var e = list.GetEnumerator();
			var moveNext = e.MoveNext();
			if (!moveNext)
				return result;

			array = new T[ChunkSize];
			while (true)
			{
				if (!moveNext || i == ChunkSize)
				{
					result = result.InsertX(result.Count, new ArrayRope(array, 0, i));
					i = 0;
					if (!moveNext)
						return result;
				}

				array[i++] = e.Current;
				moveNext = e.MoveNext();
			}
		}

		[System.Diagnostics.Contracts.Pure]
		public static Rope<T> From(Func<int, T> func, int start, int count)
		{
			return new FunctionRope(start, count, func);
		}

		[System.Diagnostics.Contracts.Pure]
		public static Rope<T> From(T[] array, int start, int count)
		{
			return FromX(array, start, count).Seal();
		}

		private static Rope<T> FromX(T[] array, int start, int count)
		{
			if (count <= ChunkSize)
				return count == 0 ? Empty : new ArrayRope(array, start, count);

			var mid = count/2;
			var left = From(array, start, mid);
			var right = From(array, start + mid, count - mid);
			return left + right;
		}

		[System.Diagnostics.Contracts.Pure]
		public Rope<T> AsTrackable()
		{
			var ev = this as TrackableRope;
			return ev ?? new TrackableRope(this);
		}

		[DebuggerStepThrough]
		[System.Diagnostics.Contracts.Pure]
		public static Rope<T> operator +(Rope<T> list1, Rope<T> list2)
		{
			return ConcatRope.Concat(list1, list2);
		}

		[Conditional("DEBUG")]
		protected virtual void Validate()
		{
		}

		#endregion

		#region Properties

		public virtual Edit Edits => null;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Count { get; protected set; }

		public virtual T this[int index]
		{
			get { throw new InvalidOperationException(); }
			set { throw new InvalidOperationException(); }
		}

		public virtual void ForEach(Action<T> action)
		{
			var count = Count;
			for (var i = 0; i < count; i++)
				action(this[i]);
		}

		public virtual void FindNode(int position, out Rope<T> node, out int nodeStart)
		{
			node = this;
			nodeStart = 0;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected virtual int NodeCount => 0;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected virtual int Height => 1;

		#endregion

		#region Indexing

		/// <summary>
		///     Checks to see if index is in an acceptable range
		/// </summary>
		/// <param name="index"></param>
		[DebuggerStepThrough]
		protected void CheckIndex(int index)
		{
			if (unchecked((uint) index >= (uint) Count))
				throw new IndexOutOfRangeException();
		}

		/// <summary>
		///     Checks to see if the combination of start and count
		///     is in an acceptable range
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		[DebuggerStepThrough]
		protected void CheckIndex(int start, int count)
		{
			if (start < 0)
				throw new ArgumentOutOfRangeException(nameof(start));
			if (count < 0 || start + count > Count)
				throw new ArgumentOutOfRangeException(nameof(count));
		}

		/// <summary>
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		[System.Diagnostics.Contracts.Pure]
		public int NextRun(int position)
		{
			if (unchecked((uint) position >= (uint) Count))
				return position < 0 ? 0 : position;

			int nStart;
			Rope<T> n;

			FindNode(position, out n, out nStart);
			var r = n as RunRope;
			if (r != null)
				return nStart + r.Count;
			return position + 1;
		}

		/// <summary>
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		[System.Diagnostics.Contracts.Pure]
		public int PreviousRun(int position)
		{
			if (position <= 0)
				return position;

			if (position >= Count)
				return Count - 1;

			int nStart;
			Rope<T> n;

			FindNode(position, out n, out nStart);
			var r = n as RunRope;
			return (r != null ? nStart : position) - 1;
		}

		[System.Diagnostics.Contracts.Pure]
		public Rope<T> Map(Func<T, T> func)
		{
			var result = MapX(func);
			result.Seal();
			return result;
		}

		[System.Diagnostics.Contracts.Pure]
		protected abstract Rope<T> MapX(Func<T, T> func);

		[System.Diagnostics.Contracts.Pure]
		public Rope<T2> Map<T2>(Func<T, T2> func)
		{
			var result = MapX(func);
			result.Seal();
			return result;
		}

		protected abstract Rope<T2> MapX<T2>(Func<T, T2> func);

		[System.Diagnostics.Contracts.Pure]
		public Rope<T> Remove(Func<T, bool> func)
		{
			var result = RemoveX(func);
			return result.Seal();
		}

		[System.Diagnostics.Contracts.Pure]
		protected virtual Rope<T> RemoveX(Func<T, bool> func)
		{
			var result = Empty;
			int s, e;
			for (s = 0, e = 0; e >= Count; e++)
			{
				if (func(this[s]))
				{
					result = result + CopyX(s, e - s);
					s = e + 1;
				}
			}
			return result + CopyX(s, Count - s);
		}

		protected virtual Rope<T> Undecorate()
		{
			return this;
		}

		#endregion

		#region Object Overrides

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			var list = obj as Rope<T>;
			if (list == null)
				return false;
			return Equals(list);
		}

		public abstract override int GetHashCode();

		private int MakeHashCode(int code)
		{
			if (!IsSealed())
				Seal();

			return Utility.CreateHashCode(Count, code);
		}

		public bool Equals(Rope<T> list)
		{
			if (list == this)
				return true;

			if (list == null
				|| list.Count != Count
				|| list.GetHashCode() != GetHashCode())
				return false;

			var iter1 = GetEnumerator();
			var iter2 = list.GetEnumerator();
			var comparer = EqualityComparer<T>.Default;

			// TODO: fast search over nodes
			while (iter1.MoveNext())
			{
				if (!iter2.MoveNext())
					return false;

				if (!comparer.Equals(iter1.Current, iter2.Current))
					return false;
			}

			Debug.Assert(!iter2.MoveNext());
			return true;
		}

		[DebuggerNonUserCode]
		public static bool Equals(ref Rope<T> node1, ref Rope<T> node2)
		{
			if (node1 == node2)
				return true;

			if (!node1.Equals(node2))
				return false;

			if (Utility.IsOlder(node1, node2))
				node2 = node1;
			else
				node1 = node2;
			return true;
		}

		public override string ToString()
		{
			return string.Join(this is IEnumerable<char> ? "" : ",", this);
		}

		#endregion

		#region

		[System.Diagnostics.Contracts.Pure]
		public int IndexOf(T element)
		{
			return IndexOf(element, 0);
		}

		[System.Diagnostics.Contracts.Pure]
		public virtual int IndexOf(T element, int start, int count = int.MaxValue)
		{
			var comparer = EqualityComparer<T>.Default;
			var end = count >= Count - start ? Count : start + count;
			for (var i = start; i < end; i++)
				if (comparer.Equals(this[i], element))
					return i;
			return -1;
		}

		/// <summary>
		///     Returns the index of the last instance of a value in the list,
		///     starting from start, up to count characters
		/// </summary>
		/// <param name="value">value to search for</param>
		/// <param name="start">starting location</param>
		/// <param name="count">up to count characters</param>
		/// <returns></returns>
		[System.Diagnostics.Contracts.Pure]
		public int LastIndexOf(T value,
			int start = 0, int count = int.MaxValue)
		{
			var comparer = EqualityComparer<T>.Default;
			var iter = GetEnumerator();
			iter.Position = start + count;
			while (iter.MovePrevious() && count-- > 0)
				if (comparer.Equals(value, iter.Current))
					return iter.Position;
			return -1;
		}

		private bool FixIndex(ref int position, ref int length)
		{
			if (position < 0)
				position = 0;

			var count = Count;
			var end = position + length;
			if (end >= count)
			{
				if (position > count)
					position = count;
				length = count - position;
			}
			return length == count;
		}

		[System.Diagnostics.Contracts.Pure]
		public Rope<T> Remove(int position, int length)
		{
			var removed = RemoveX(position, length);
			removed.Seal();
			return removed;
		}

		private Rope<T> RemoveX(int position, int length)
		{
			if (FixIndex(ref position, ref length)) return Empty;
			if (length <= 0) return this;
			return OnRemove(position, length);
		}

		protected virtual Rope<T> OnRemove(int position, int length)
		{
			var end = position + length;
			return CopyX(0, position) + CopyX(end, Count - end);
		}

		[System.Diagnostics.Contracts.Pure]
		public Rope<T> Copy(int position, int length)
		{
			var copy = CopyX(position, length);
			return copy.Seal();
		}

		private Rope<T> CopyX(int position, int length)
		{
			if (FixIndex(ref position, ref length)) return Seal();
			if (length <= 0) return Empty;
			var copied = OnCopy(position, length);
			copied.Seal();
			return copied;
		}

		protected abstract Rope<T> OnCopy(int position, int length);

		[DebuggerStepThrough]
		[System.Diagnostics.Contracts.Pure]
		public Rope<T> Append(T value, int count = 1)
		{
			return Insert(Count, value, count);
		}

		[DebuggerStepThrough]
		[System.Diagnostics.Contracts.Pure]
		public Rope<T> Insert(int start, params T[] values)
		{
			return Insert(start, (IEnumerable<T>) values);
		}

		[DebuggerStepThrough]
		[System.Diagnostics.Contracts.Pure]
		public Rope<T> Insert(int index, IEnumerable<T> list)
		{
			return Insert(index, FromX(list));
		}

		[System.Diagnostics.Contracts.Pure]
		public Rope<T> Insert(int position, Rope<T> rope)
		{
			var result = InsertX(position, rope);
			result.Seal();
			return result;
		}

		[DebuggerStepThrough]
		[System.Diagnostics.Contracts.Pure]
		private Rope<T> InsertX(int position, Rope<T> rope)
		{
			if (rope.Count == 0) return this;
			var result = OnInsert(position, rope);
			Utility.Assert(result != null);
			return result;
		}

// ReSharper disable MethodOverloadWithOptionalParameter
		[System.Diagnostics.Contracts.Pure]
		public Rope<T> Insert(int start, T value, int count = 1)
// ReSharper restore MethodOverloadWithOptionalParameter
		{
			var inserted = InsertX(start, value, count);
			inserted.Seal();
			return inserted;
		}

		[DebuggerStepThrough]
		[System.Diagnostics.Contracts.Pure]
		private Rope<T> InsertX(int start, T value, int count = 1)
		{
			if (count == 0)
				return this;

			if (count > 1)
				return InsertX(start, RepeatX(count, value));
			// TODO: Check up on this to make it more efficient
			// The value is copied to an array twice
			// We could also convert a run array to a vector array
			return InsertX(start, new ArrayRope(new[] {value}, 0, 1));
		}

		[System.Diagnostics.Contracts.Pure]
		protected virtual Rope<T> OnInsert(int position, Rope<T> node)
		{
			if (node.Count == 0)
				return this;

			if (position == 0)
				return node.InsertX(node.Count, this);

			var left = CopyX(0, position);
			var right = CopyX(position, Count - position);
			return left + node + right;
		}

		[DebuggerStepThrough]
		[System.Diagnostics.Contracts.Pure]
		public Rope<T> Set(int start, T value, int count = 1)
		{
			var result = SetX(start, value, count);
			result.Seal();
			return result;
		}

		[DebuggerStepThrough]
		[System.Diagnostics.Contracts.Pure]
		protected Rope<T> SetX(int start, T value, int count = 1)
		{
			return RemoveX(start, count)
				.InsertX(start, value, count);
		}

		protected virtual Rope<T> Seal()
		{
			return this;
		}

		[System.Diagnostics.Contracts.Pure]
		protected virtual bool IsSealed()
		{
			return true;
		}

		#endregion

		#region Interfaces

		#region IList<T> Members

		void IList<T>.Insert(int index, T item)
		{
			throw new InvalidOperationException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new InvalidOperationException();
		}

		#endregion

		#region ICollection<T> Members

		void ICollection<T>.Add(T item)
		{
			throw new InvalidOperationException();
		}

		void ICollection<T>.Clear()
		{
			throw new InvalidOperationException();
		}

		public bool Contains(T item)
		{
			return IndexOf(item) >= 0;
		}

		public void CopyTo(T[] array, int index)
		{
			foreach (var obj in this)
				array[index++] = obj;
		}

		[System.Diagnostics.Contracts.Pure]
		public T[] ToArray()
		{
			return ToArray(0, Count);
		}

		[System.Diagnostics.Contracts.Pure]
		public T[] ToArray(int start, int count)
		{
			CheckIndex(start, count);
			var array = new T[count];
			var iter = GetEnumerator();
			iter.Position = start - 1;
			var pos = 0;
			while (count-- > 0)
			{
				if (!iter.MoveNext())
					throw new InvalidOperationException();
				array[pos++] = iter.Current;
			}
			return array;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection<T>.IsReadOnly => true;

		bool ICollection<T>.Remove(T item)
		{
			throw new InvalidOperationException();
		}

		#endregion

		#endregion

		#region EditableVector

		[Serializable]
		[DebuggerDisplay("Count={Count}, Nodes={NodeCount}")]
		public class TrackableRope : Rope<T>
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)] private Edit _edit;

			[NotNull] [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)] private Rope<T> _rope;

			public TrackableRope(Rope<T> rope, Edit edit = null)
			{
				_rope = rope;
				_edit = edit ?? new Edit();
				Count = _rope.Count;
			}

			public override Edit Edits => _edit;

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public Rope<T> Rope => _rope;

			protected override int Height => _rope.Height;

			protected override int NodeCount => _rope.NodeCount;

			public override T this[int index]
			{
				get { return _rope[index]; }
				set { _rope[index] = value; }
			}

			protected override Rope<T> Undecorate()
			{
				return _rope;
			}

			protected override Rope<T> MapX(Func<T, T> func)
			{
				var vector = _rope.Seal().Map(func);
				if (vector == _rope)
					return this;
				return ReplaceVector(vector, GetChangeEdit());
			}

			protected override Rope<T2> MapX<T2>(Func<T, T2> func)
			{
				return _rope.Map(func);
			}

			protected override Rope<T> OnCopy(int position, int length)
			{
				return _rope.OnCopy(position, length);
			}

			protected override Rope<T> OnRemove(int start, int count)
			{
				var oldCount = _rope.Count;
				var result = _rope.RemoveX(start, count);
				Utility.Assert(oldCount - result.Count == count);
				return ReplaceVector(
					result,
					GetNewEdit(start, oldCount - result.Count, 0));
			}

			protected override Rope<T> OnInsert(int position, Rope<T> node)
			{
				var ev = node as TrackableRope;
				if (ev != null)
					node = ev.Rope;

				var vector = _rope.OnInsert(position, node);
				return ReplaceVector(
					vector,
					GetNewEdit(position, 0, vector.Count));
			}

			[System.Diagnostics.Contracts.Pure]
			private TrackableRope ReplaceVector(Rope<T> rope, Edit edit)
			{
				if (!_rope.IsSealed())
				{
					_rope = rope;
					_edit = edit;
					return this;
				}

				if (edit == _edit && rope == _rope)
					return this;

				return new TrackableRope(rope, edit);
			}

			private Edit GetNewEdit(int start, int deleted, int inserted)
			{
				Debug.Assert(_rope != null);

				if (_edit == null)
					return null;

				var edit = _edit;

				if (deleted != 0)
					edit = edit.Delete(start, deleted);

				if (inserted != 0)
					edit = edit.Insert(start, inserted);

				return edit;
			}

			public Edit GetChangeEdit()
			{
				var changed = Count;
				if (_edit == null || changed == 0)
					return _edit;
				return _edit.ChangeProperty(0, changed);
			}

			public Rope<T> ChangeX(int start, int changed)
			{
				if (_edit == null || changed == 0)
					return this;
				return ReplaceVector(_rope, _edit.ChangeProperty(start, changed));
			}

			protected override Rope<T> Seal()
			{
				_rope.Seal();
				return base.Seal();
			}

			public override void ForEach(Action<T> action)
			{
				_rope.ForEach(action);
			}

			public override void FindNode(int position, out Rope<T> node, out int nodeStart)
			{
				_rope.FindNode(position, out node, out nodeStart);
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				var list = obj as TrackableRope;
				if (list == null)
					return false;

				return Equals(ref _rope, ref list._rope);
			}

			public override int GetHashCode()
			{
				// ReSharper disable once NonReadonlyMemberInGetHashCode
				return _rope.GetHashCode();
			}

			protected override Rope<T> RemoveX(Func<T, bool> func)
			{
				var vector = _rope.Seal().RemoveX(func);
				if (vector != _rope)
					return this;
				return ReplaceVector(vector, GetChangeEdit());
			}

			protected override void Validate()
			{
				_rope.Validate();
			}

			public override string ToString()
			{
				return _rope.ToString();
			}

			public override int IndexOf(T element, int start, int count = int.MaxValue)
			{
				return _rope.IndexOf(element, start, count);
			}

			protected override bool IsSealed()
			{
				return _rope.IsSealed();
			}
		}

		#region Merging

		public bool IsConnected(Rope<T> v)
		{
			return Edit.IsConnected(Edits, v.Edits);
		}

		public Rope<T> Change(int start, int count)
		{
			// Don't bother making this virtual, since we only want it to work in one case
			// and cause an exception for all others.
			var track = (TrackableRope) this;
			return track.ChangeX(start, count).Seal();
		}

		public bool GetChanges(Rope<T> previous,
			out Rope<bool> setbits,
			out Rope<int> mapIndex,
			Action<Adjustment> action = null)
		{
			setbits = Rope<bool>.RepeatX(previous.Count, false);
			mapIndex = Rope<int>.FunctionX(0, previous.Count, x => x);

			foreach (var adj in GetChanges(previous))
			{
				var start = adj.Start;
				var changed = adj.Deleted;

				if (action != null)
					action(adj);

				if (changed != 0)
				{
					if (adj.PropertyOnly || changed == adj.Inserted)
					{
						setbits = setbits.SetX(start, true, changed);
						continue;
					}

					setbits = setbits.RemoveX(start, changed);
					mapIndex = mapIndex.RemoveX(start, changed);
				}

				changed = adj.Inserted;
				if (changed != 0)
				{
					setbits = setbits.InsertX(start, true, changed);
					mapIndex = mapIndex.InsertX(start, -1, changed);
				}
			}

			setbits.Seal();
			mapIndex.Seal();
			return true;
		}

		public bool GetChanges<T2>(Rope<T> previous,
			out Rope<bool> setbits,
			out Rope<int> mapIndex,
			ref Rope<T2> rope)
		{
			var v = rope.Seal();
			var result = GetChanges(previous, out setbits, out mapIndex, adj =>
			{
				var start = adj.Start;
				var changed = adj.Deleted;
				if (changed != 0)
				{
					if (adj.PropertyOnly || changed == adj.Inserted)
					{
						v = v.Set(start, default(T2), changed);
						return;
					}
					v = v.RemoveX(start, changed);
				}

				changed = adj.Inserted;
				if (changed != 0)
					v = v.InsertX(start, default(T2), changed);
			});

			rope = v.Seal();
			return result;
		}

		public IEnumerable<Adjustment> GetChanges(Rope<T> v, bool check = false)
		{
			if (check && !IsConnected(v))
				return ListTools.Empty<Adjustment>();
			return GetChangesCore(v);
		}

		private IEnumerable<Adjustment> GetChangesCore(Rope<T> v)
		{
			foreach (var chg in Edit.GetChanges(Edits, v.Edits))
			{
				var diff = chg as Edit.Diff;
				if (diff == null)
					break;
				yield return diff.Adjustment;
			}
		}

		#endregion

		#endregion

		#region Nested type: RunVector

		/// <summary>
		///     Summary description for Node.
		/// </summary>
		[Serializable]
		private class RunRope : Rope<T>
		{
			#region Variables

			private T _data;

			#endregion

			#region Flat

			protected override Rope<T> OnInsert(int index, Rope<T> inserted)
			{
				var node = inserted as RunRope;
				var comparer = EqualityComparer<T>.Default;

				if (node == null || !comparer.Equals(_data, node._data))
					return base.OnInsert(index, inserted);

				return new RunRope(Count + node.Count, _data);
			}

			#endregion

			#region Construction

			[DebuggerStepThrough]
			public RunRope(int count, T data)
			{
				Set(count, data);
			}

			[DebuggerStepThrough]
			private void Set(int count, T data)
			{
				Count = count;
				_data = data;
			}

			protected override Rope<T> MapX(Func<T, T> func)
			{
				var data = func(_data);
				if (IsSealed() && EqualityComparer<T>.Default.Equals(data, _data))
					return this;
				return new RunRope(Count, data);
			}

			protected override Rope<T2> MapX<T2>(Func<T, T2> func)
			{
				return new Rope<T2>.RunRope(Count, func(_data));
			}

			protected override Rope<T> RemoveX(Func<T, bool> func)
			{
				if (func(_data))
					return Empty;
				return this;
			}

			#endregion

			#region Edit Management

			public override T this[int index]
			{
				get
				{
					if (unchecked((uint) index >= (uint) Count))
						throw new IndexOutOfRangeException();
					return _data;
				}
			}

			public override int IndexOf(T element, int start, int count = int.MaxValue)
			{
				if (EqualityComparer<T>.Default.Equals(_data, element))
					return 0;
				return -1;
			}

			protected override Rope<T> OnCopy(int position, int length)
			{
				if (length == 0)
					return Empty;
				return new RunRope(length, _data);
			}

			protected override Rope<T> OnRemove(int position, int length)
			{
				return new RunRope(Count - length, _data);
			}

			#endregion

			#region Object Routines

			public override string ToString()
			{
				object obj = _data;
				var text = obj == null
					? "(null)"
					: _data.ToString();
				return $"Run('{text}' * {Count})";
			}

			// TODO: Hash codes are performance issues
			public override int GetHashCode()
			{
				var hash = 0;
				// ReSharper disable NonReadonlyMemberInGetHashCode
				var v = _data == null ? 0 : _data.GetHashCode();
				for (var i = Count - 1; i >= 0; i--)
					hash ^= v;
				return MakeHashCode(hash);
			}

			#endregion
		}

		#endregion

		#region Nested type: FunctionVector

		/// <summary>
		///     Summary description for Node.
		/// </summary>
		[Serializable]
		private class FunctionRope : Rope<T>
		{
			#region Variables

			private readonly Func<int, T> _func;
			private readonly int _start;

			#endregion

			#region Construction

			[DebuggerStepThrough]
			public FunctionRope(int start, int count, Func<int, T> func)
			{
				_start = start;
				Count = count;
				_func = func;
			}

			protected override Rope<T> MapX(Func<T, T> func)
			{
				return new FunctionRope(_start, Count, x => func(_func(x)));
			}

			protected override Rope<T2> MapX<T2>(Func<T, T2> func)
			{
				return new Rope<T2>.FunctionRope(
					_start, Count, x => func(_func(x)));
			}

			#endregion

			#region Edit Management

			public override T this[int index]
			{
				get
				{
					if (unchecked((uint) index >= (uint) Count))
						throw new IndexOutOfRangeException();
					return _func(index + _start);
				}
			}

			protected override Rope<T> OnCopy(int position, int length)
			{
				if (length == 0)
					return Empty;
				return new FunctionRope(_start + position, Count, _func);
			}

			protected override Rope<T> OnInsert(int index, Rope<T> inserted)
			{
				var f = inserted as FunctionRope;

				if (f == null
					|| !f._func.Equals(_func)
					|| f._start != _start + Count)
					return base.OnInsert(index, inserted);

				return new FunctionRope(_start, Count + f.Count, _func);
			}

			#endregion

			#region Object Routines

			public override string ToString()
			{
				return $"{_func}({_start}, {Count})";
			}

			public override int GetHashCode()
			{
				var hash = 0;
				for (var i = Count - 1; i >= 0; i++)
					hash ^= this[i].GetHashCode();
				return MakeHashCode(hash);
			}

			#endregion
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerable<T> Range(int start, int count)
		{
			var enumerator = GetEnumerator();
			// TODO: Optimize with find node
			enumerator.Position = start;
			while (count-- > 0 && enumerator.MoveNext())
				yield return enumerator.Current;
		}

		[DebuggerStepThrough]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		[DebuggerStepThrough]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		[DebuggerStepThrough]
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		[DebuggerStepThrough]
		IEnumerator<T> IIndexable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion

		#region Find

		/// <summary>
		///     Finds characters at the given direction
		/// </summary>
		/// <param name="ch"></param>
		/// <param name="index"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public bool FindCharacter(T ch, int index,
			Direction direction = Direction.Positive)
		{
			var i = index;
			var comparer = EqualityComparer<T>.Default;
			while (true)
			{
				if (comparer.Equals(this[i], ch))
					return true;

				if (direction == Direction.Positive)
				{
					i++;
					if (i >= Count)
						return false;
				}
				else
				{
					i--;
					if (i < 0)
						return false;
				}
			}
		}

		/// <summary>
		///     Finds several characters at the given direction
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="index"></param>
		/// <param name="delimiters"></param>
		/// <returns></returns>
		public bool FindCharacters(Direction direction,
			int index, ICollection<T> delimiters)
		{
			var i = index;
			while (true)
			{
				if (delimiters.Contains(this[i]))
					return true;

				if (direction == Direction.Positive)
				{
					i++;
					if (i >= Count)
						return false;
				}
				else
				{
					i--;
					if (i < 0)
						return false;
				}
			}
		}

		#endregion

		#region Builder

		[DebuggerStepThrough]
		[System.Diagnostics.Contracts.Pure]
		public List GetBuilder()
		{
			return new List(this);
		}

		#endregion
	}
}