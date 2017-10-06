#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Diagnostics;
using JetBrains.Annotations;

#endregion

namespace Softperson.Diagnostics
{
	[DebuggerDisplay("{Text,nq}", Type = "{Type,nq}")]
	public class DebugString
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] public readonly string Text;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)] [PublicAPI] public string Type;

		[DebuggerStepThrough]
		public DebugString(object text, string type = "")
		{
			Text = text?.ToString();
			Type = type;
		}

		[DebuggerStepThrough]
		public override string ToString()
		{
			return Text;
		}

		public static implicit operator DebugString(string text)
		{
			return new DebugString(text);
		}
	}

	[DebuggerDisplay("{Text,nq}", Name = "{Name,nq}", Type = "{Type,nq}")]
	public class DebugStringWithName : DebugString
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] public string Name;

		[DebuggerStepThrough]
		public DebugStringWithName(object text, string name, string type = "")
			: base(text, type)
		{
			Name = name;
		}
	}
}