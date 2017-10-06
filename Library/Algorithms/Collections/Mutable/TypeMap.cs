﻿#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the express permission of Wesner Moise.
//
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

#endregion

namespace Softperson.Collections
{
	/// <summary>
	///     Summary description for TypeMap.
	/// </summary>
	public class TypeMap<V>
	{
		#region Properties

		/// <summary>
		///     Indicates whether caching is enabled for the operation
		/// </summary>
		public bool CachingEnabled { get; set; } = true;

		#endregion

		#region Variables

		private readonly Dictionary<Type, V> hashtable = new Dictionary<Type, V>();
		private bool disallowRemove;

		#endregion

		#region Construction

		#endregion

		#region Methods

		public ICollection<Type> Keys
		{
			get { return hashtable.Keys; }
		}

		/// <summary>
		///     Gets the object for the given type
		/// </summary>
		public V this[Type type]
		{
			get
			{
				// Check for a direct type
				V result;
				if (hashtable.TryGetValue(type, out result))
					return result;
				return ScanParents(type);
			}

			set { hashtable[type] = value; }
		}

		/// <summary>
		///     Get Value association the type of the passed object
		/// </summary>
		/// <param name="obj">obj to pass in</param>
		public V GetValue(object obj)
		{
			Utility.Assert(!(obj is Type));
			return this[obj.GetType()];
		}

		public V GetDirectValue(Type type)
		{
			V result;
			hashtable.TryGetValue(type, out result);
			return result;
		}

		private V ScanParents(Type type)
		{
			V result;
			// Check all parents for the right object
			var current = type;
			do
			{
				current = current.BaseType;
				if (current == null)
					return default(V);
			} while (!hashtable.TryGetValue(current, out result));

			// Set all descendents of type to the right object
			if (CachingEnabled)
			{
				disallowRemove = true;
				while (type != current)
				{
					hashtable[type] = result;
					type = type.BaseType;
				}
			}

			return result;
		}

		/// <summary>
		///     Removes the type from the type mapping
		/// </summary>
		/// <param name="type"></param>
		public void Remove(Type type)
		{
			if (disallowRemove)
				throw new InvalidOperationException("Cannot removing when caching is enabled");
			hashtable.Remove(type);
		}

		#endregion
	}
}