﻿#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2006 - 2008, Wesner Moise.
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

#endregion

namespace Softperson.Collections
{
	[Pure]
	public class Version<T>
	{
		#region Properties

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public T Data
		{
			get
			{
				var n = _change as Core;
				return n != null ? n.Value : Sync();
			}
		}

		#endregion

		#region Helper Classes

		public abstract class Change
		{
			public abstract Change Apply(T unknown);
		}

		#endregion

		#region Nested type: Core

		[DebuggerStepThrough]
		private class Core : Change
		{
			public readonly T Value;
#if DEBUG
			public bool Reentrant;
#endif

			[DebuggerStepThrough]
			public Core(T value)
			{
				Value = value;
			}

			public override Change Apply(T unknown)
			{
				return this;
			}
		}

		#endregion

		#region Variables

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)] protected Change _change;
		private Version<T> _previous;

		#endregion

		#region Constructor

		[DebuggerStepThrough]
		public Version(T value)
			: this(new Core(value), null)
		{
		}

		[DebuggerStepThrough]
		private Version(Change change, Version<T> previous)
		{
#if DEBUG
			if (previous != null)
				previous.Validate();
			else
				Utility.Assert(change is Core);
#endif

			_change = change;
			_previous = previous;
			Validate();
		}

		[Conditional("DEBUG")]
		public void Validate()
		{
#if DEBUG
			var n = _change;
			var p = _previous;
			while (p != null)
			{
				n = p._change;
				p = p._previous;
			}
			Debug.Assert(n is Core);
#endif
		}

		[DebuggerStepThrough]
		public static Version<T> operator +(Version<T> version, Change change)
		{
			return new Version<T>(change, version);
		}

		#endregion

		#region Methods

		public T Sync()
		{
			Version<T> current;
			Version<T> previous;

			// Quick success
			var core = _change as Core;
			if (core != null)
				return core.Value;

			current = this;
			previous = null;
#if DEBUG
			var count1 = 0;
			var count2 = 0;
#endif

			do
			{
#if DEBUG
				count1++;
#endif
				var tmp = current._previous;
				current._previous = previous;
				previous = current;
				current = tmp;
			} while (current != null);


			var last = previous;
			core = (Core) previous._change;
			var s = core.Value;

#if DEBUG
			if (core.Reentrant)
				throw new InvalidOperationException();
			core.Reentrant = true;
#endif

			while (true)
			{
#if DEBUG
				count2++;
#endif
				current = previous;
				previous = previous._previous;
				if (previous == null)
					break;
				current._change = previous._change.Apply(s);
			}


			_change = core;
#if DEBUG
			core.Reentrant = false;
			Utility.Assert(current == this);
			Utility.Assert(_previous == null);
			Utility.Assert(count1 == count2);
			current.Validate();
			last.Validate();
#endif
			return s;
		}

		[DebuggerStepThrough]
		public static IEnumerable<Change> GetChanges(Version<T> p1, Version<T> p2)
		{
			return GetChanges(p1, p2, null);
		}

		[DebuggerStepThrough]
		public static IEnumerable<Change> GetChanges(
			Version<T> branch1, Version<T> branch2, Version<T> old)
		{
			branch1.Sync();

			for (var current = branch2;
				current != null && current != old;
				current = current._previous)
			{
				yield return current._change;
			}
		}

		[DebuggerStepThrough]
		public static void ForEachhange(Version<T> p1, Version<T> p2,
			Action<Change> action)
		{
			ForEachChange(p1, p2, null, action);
		}

		[DebuggerStepThrough]
		public static void ForEachChange(
			Version<T> branch1, Version<T> branch2, Version<T> old,
			Action<Change> action)
		{
			branch1.Sync();

			for (var current = branch2;
				current != null && current != old;
				current = current._previous)
			{
				action(current._change);
			}
		}

		[DebuggerStepThrough]
		public override string ToString()
		{
			return _change.ToString();
		}

		#endregion
	}
}