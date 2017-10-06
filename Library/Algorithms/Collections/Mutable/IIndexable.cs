﻿#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace Softperson.Collections
{
	public interface IIndexable<out T>
	{
		T this[int index] { get; }

		int Count { get; }

		IEnumerator<T> GetEnumerator();
	}

	[DebuggerDisplay("{ToString(),nq}", Name = "{Name,nq}")]
	public class IndexableView<P>
		where P : IIndexable<P>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] protected readonly P Main;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)] public Action<P> Freeze;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)] public string Name;


		public IndexableView(P main, Action<P> freeze = null)
		{
			Main = main;
			Freeze = freeze;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public virtual object[] InnerList
		{
			get
			{
				var main = Main;
				if (main == null || main.Count == 0)
					return ArrayTools.EmptyArray<object>();

				// TODO: Add <More> item later
				var count = Math.Min(20, main.Count);
				var list = new object[count];
				for (var i = 0; i < count; i++)
				{
					var item = main[i];
                    Freeze?.Invoke(item);
                    list[i] = item;
				}
				return list;
			}
		}

		public P this[int index]
		{
			get { return Main[index]; }
		}

		public override string ToString()
		{
			return Main != null ? Main.ToString() : "";
		}

		public override bool Equals(object obj)
		{
			var dv = obj as IndexableView<P>;
			return dv != null && Equals(dv.Main, Main);
		}

		public override int GetHashCode()
		{
			return Main == null ? 0 : Main.GetHashCode();
		}

		public static implicit operator P(IndexableView<P> path)
		{
			return path == null ? default(P) : path.Main;
		}
	}
}