#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2005, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#pragma warning disable 414

#endregion

namespace Softperson.Collections
{
	public class SortableBindingList<T> : BindingList<T>
	{
		#region Sorting

		private bool _isSorted;
		private ListSortDirection _sortDirection;
		private PropertyDescriptor _sortProperty;
		private PropertyComparer<T> _comparer;

		public SortableBindingList(List<T> list) : base(list)
		{
		}

		public SortableBindingList()
		{
		}

		protected override bool SupportsSortingCore => true;

		// Missing from Part 2
		protected override ListSortDirection SortDirectionCore => _sortDirection;

		// Missing from Part 2
		protected override PropertyDescriptor SortPropertyCore => _sortProperty;

		protected override bool IsSortedCore => _isSorted;

		protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
		{
			// Get list to sort
			// Note: this.Items is a non-sortable ICollection<T>
			var items = Items as List<T>;

			// Apply and set the sort, if items to sort
			if (items != null)
			{
				if (property != _sortProperty || _comparer == null)
					_comparer = new PropertyComparer<T>(property, direction);

				items.Sort(_comparer);
				_isSorted = true;
			}
			else
			{
				_isSorted = false;
			}

			_sortProperty = property;
			_sortDirection = direction;

			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		protected override void RemoveSortCore()
		{
			_isSorted = false;
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		#endregion

		#region Persistence

		// NOTE: SortableBindingList<T> is not serializable but List<T> is

		public void Save(string filename)
		{
			var formatter = new BinaryFormatter();
			using (var stream = new FileStream(filename, FileMode.Create))
			{
				// Serialize data list items
				formatter.Serialize(stream, Items);
			}
		}

		public void Load(string filename)
		{
			ClearItems();

			if (File.Exists(filename))
			{
				var formatter = new BinaryFormatter();
				using (var stream = new FileStream(filename, FileMode.Open))
				{
					// Deserialize data list items
					((List<T>) Items).AddRange((IEnumerable<T>) formatter.Deserialize(stream));
				}
			}

			// Let bound controls know they should refresh their views
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		#endregion

		#region Searching

		protected override bool SupportsSearchingCore => true;

		protected override int FindCore(PropertyDescriptor property, object key)
		{
			var keyString = key.ToString();

			// Specify search columns
			if (property == null)
				return -1;

			// Get list to search
			var items = Items as List<T>;

			// Traverse list for value
			foreach (var item in items)
			{
				// Test column search value
				var value = property.GetValue(item).ToString();

				// If value is the search value, return the 
				// index of the data item
				if (keyString == value) return IndexOf(item);
			}
			return -1;
		}

		#endregion
	}

	public class PropertyComparer<T> : IComparer<T>, IEqualityComparer<T>
	{
		#region Constructor

		public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
		{
			Property = property;
			_direction = direction;

			var info = typeof (T).GetProperty(property.Name).GetGetMethod();
			_method = FuncTools.CreateGenericDelegate(info);
		}

		#endregion

		#region Properties

		public PropertyDescriptor Property { get; }

		#endregion

		#region Variables

		private readonly ListSortDirection _direction;
		private readonly GenericDelegate _method;

		#endregion

		#region IComparer<T>

		#region IComparer<T> Members

		public int Compare(T xWord, T yWord)
		{
			// Get property values
			var x = _method(xWord);
			var y = _method(yWord);

			if (x == y)
				return 0;

			int cmp;
			if (x == null || y == null)
				cmp = x == null ? -1 : 1;
			else
				cmp = ((IComparable) x).CompareTo(y);

			// Determine sort order
			if (_direction == ListSortDirection.Ascending)
				return cmp;
			return -cmp;
		}

		#endregion

		#region IEqualityComparer<T> Members

		public bool Equals(T xWord, T yWord)
		{
			return xWord.Equals(yWord);
		}

		public int GetHashCode(T obj)
		{
			return obj.GetHashCode();
		}

		#endregion

		#endregion
	}
}