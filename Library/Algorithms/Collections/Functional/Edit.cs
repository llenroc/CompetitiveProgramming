﻿using System.Diagnostics;
using System.Diagnostics.Contracts;
using Softperson.ObjectModel;

namespace Softperson.Collections
{
	public class Edit : Persistent<object>
	{
		#region Helper Classes

		[DebuggerDisplay("{Adjustment,nq}")]
		public class Diff : Change
		{
			public Adjustment Adjustment;

			public Diff(Adjustment adj)
			{
				Adjustment = adj;
			}

			public override Change Apply(object a)
			{
				Adjustment = Adjustment.Invert();
				return this;
			}

			public override string ToString()
			{
				return Adjustment.ToString();
			}
		}

		#endregion

		#region Constructor

		// TODO: Add a datanode so we can track counts
		public Edit()
			: base(null)
		{
#if DEBUG
			Initialize();
#endif
		}

		public Edit(Change change, Edit edit)
			: base(change, edit)
		{
#if DEBUG
			Initialize(change, edit);
#endif
		}

		#endregion

		#region Properties

		public Edit Previous
		{
			get { return (Edit) _previous; }
		}

		public Change ChangeData
		{
			get { return _change; }
		}

		[Pure]
		public Edit Adjust(Adjustment adjustment)
		{
			if (adjustment.Count == 0)
				return this;
			return new Edit(new Diff(adjustment), this);
		}

		[Pure]
		public Edit Insert(int start, int count)
		{
			return Adjust(new Adjustment(start, count));
		}

		[Pure]
		public Edit Delete(int start, int count)
		{
			return Adjust(new Adjustment(start, -count));
		}

		[Pure]
		public Edit ChangeProperty(int start, int count)
		{
			return Adjust(new Adjustment(start, -count, true));
		}

		public static bool AdjustIndex(Edit before, Edit after,
			ref int index, ref int deletions, Direction direction = Direction.Positive)
		{
			if (before == null || after == null)
				return false;

			if (before == after)
				return true;

			if (!after.IsSynced() && !before.IsSynced())
				after.Sync();

			var newIndex = index;
			var newDeletions = deletions;

			if (direction == Direction.Negative)
				Adjustment.NegatePosition(ref newIndex, ref newDeletions);

			var edit = before;
			while (edit != after)
			{
				if (edit == null)
					return false;

				var diff = edit._change as Diff;
				if (diff == null)
				{
					Utility.Assert(edit._change is DataNode);
					if (!AdjustIndexHelper.AdjustIndexReverse(edit, after,
						ref newIndex, ref newDeletions))
						return false;
					break;
				}

				newIndex = diff.Adjustment.Invert().Adjust(newIndex, ref deletions);
				edit = edit.Previous;
			}

			if (direction == Direction.Negative)
				Adjustment.UnnegatePosition(ref newIndex, ref newDeletions);

			index = newIndex;
			deletions = newDeletions;
			return true;
		}

		private struct AdjustIndexHelper
		{
			private Edit _before;
			private int _index;
			private int _deletions;

			public static bool AdjustIndexReverse(Edit before, Edit after,
				ref int index, ref int deletions)
			{
				if (after == before)
					return true;

				var helper = new AdjustIndexHelper
				{
					_before = before,
					_index = index,
					_deletions = deletions
				};

				if (!helper.AdjustIndexReverse(after))
					return false;

				index = helper._index;
				deletions = helper._deletions;
				return true;
			}

			private bool AdjustIndexReverse(Edit after)
			{
				if (after == _before)
					return true;

				if (after == null
					|| !AdjustIndexReverse(after.Previous))
					return false;

				var diff = after._change as Diff;
				if (diff == null)
					return false;

				_index = diff.Adjustment.Adjust(_index, ref _deletions);
				return true;
			}
		}

		//[DebuggerStepThrough]
		public static bool AdjustIndex(IEditable before, IEditable after,
			ref int index, ref int deletions,
			Direction direction = Direction.Positive)
		{
			if (before == null || after == null)
				return false;

			Utility.Assert(before.Edits != null);
			Utility.Assert(after.Edits != null);
			return AdjustIndex(before.Edits, after.Edits, ref index, ref deletions, direction);
		}

		#endregion

		#region Debug

#if DEBUG
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] public int Version;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)] public int Length;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)] private static int _versionCounter;

		[Conditional("DEBUG")]
		private void Initialize(Change change = null, Edit edit = null)
		{
			Version = _versionCounter++;
			var diff = change as Diff;
			var length = 0;
			if (edit != null)
				length = edit.Length;
			if (diff != null && !diff.Adjustment.PropertyOnly)
				Length = length + diff.Adjustment.Count;
		}

		public override string ToString()
		{
			return "Length=" + Length + "  Version=" + Version.ToString("X");
		}
#endif

		#endregion
	}

	public interface IEditable
	{
		Edit Edits { get; }
	}
}