﻿#region Using



#endregion

namespace Softperson.Collections
{
	public class VersionStamp
	{
		private VersionStamp _next;
		private VersionStamp _previous;
		public uint Label { get; private set; }

		public static VersionStamp Zero()
		{
			var v = new VersionStamp();
			v._next = v;
			v._previous = v;
			return v;
		}

		public static int Compare(VersionStamp first, VersionStamp left, VersionStamp right)
		{
			unchecked
			{
				var leftPos = left.Label - first.Label;
				var rightPos = right.Label - first.Label;
				return ((long) leftPos).CompareTo(rightPos);
			}
		}

		public static uint Gap(VersionStamp e, VersionStamp f)
		{
			return unchecked(f.Label - e.Label);
		}

		public static uint Gap2(VersionStamp e, VersionStamp f)
		{
			if (e == f)
				return uint.MaxValue;
			return unchecked(f.Label - e.Label);
		}

		public VersionStamp NewAfter(VersionStamp e)
		{
			unchecked
			{
				var f = e._next;

				var mid = Gap2(e, f)/2;
				if (mid == 0)
					Resize();

				var v = new VersionStamp
				{
					Label = e.Label + mid,
					_previous = e,
					_next = f
				};
				e._next = v;
				f._previous = v;
				return v;
			}
		}

		private void Resize()
		{
			unchecked
			{
				uint j = 0;
				VersionStamp f;
				for (f = _next; f != this; f = f._next)
				{
					j++;
					if (Gap(this, f) > j*j)
						break;
				}

				var start = Label;
				long gap = Gap(this, f);
				var i = j - 1L;
				for (f = _previous; f != this; i--, f = f._previous)
					f.Label = (uint) (start + i*gap/j);
			}
		}

		public VersionStamp NewBefore(VersionStamp e)
		{
			return NewAfter(e._previous);
		}

		public void Delete()
		{
			_next._previous = _previous;
			_previous._next = _next;
		}
	}
}