using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Softperson
{
	public struct StringSection
	{
		#region Variables

		public readonly string Source;
		public readonly int Index;
		public readonly int Length;

		#endregion

		#region Constructor

		public StringSection(string source, int index, int length)
		{
			Source = source;
			Index = index;
			Length = length;
		}

		public StringSection(string source, int index = 0)
			: this(source, index, source?.Length - index ?? 0)
		{
		}

		#endregion

		#region Properties

		public string Value => Source?.Substring(Index, Length);

		public char this[int index]
		{
			get
			{
				if (unchecked((uint) index >= (uint) Length))
					throw new IndexOutOfRangeException();
				return Source[Index + index];
			}
		}

		public StringSection Substring(int index, int length)
		{
			var newLength = Math.Min(Length - index, length);
			if (newLength >= 0)
				return new StringSection(Source, Index + index, newLength);
			throw new ArgumentOutOfRangeException();
		}

		public StringSection Substring(int index)
		{
			return Substring(index, Length - index);
		}

		#endregion

		#region Methods

		public int IndexOf(char ch, int index, int length)
		{
			var i = Source.IndexOf(ch, Index + index, length);
			if (i < 0)
				return -1;
			return Index + i;
		}

		public int IndexOf(char ch, int index = 0)
		{
			return IndexOf(ch, index, Length - index);
		}

		/*public int LastIndexOf(char ch, int index, int length)
		{
			var i = Source.LastIndexOf(ch, Index + Index, length);
			if (i < 0)
				return -1;
			return Index + i;
		}

		public int LastIndexOf(char ch)
		{
			return LastIndexOf(ch, Length - 1, Length);
		}*/

		public int IndexOf(string pattern, int index, int length)
		{
			var i = Source.IndexOf(pattern, Index + Index, length);
			if (i < 0)
				return -1;
			return Index + i;
		}

		public int IndexOf(string pattern, int index = 0)
		{
			return IndexOf(pattern, index, Length - index);
		}

		/*
		public int LastIndexOf(string pattern, int index, int length)
		{
			var i = Source.LastIndexOf(pattern, Index + Index, length);
			if (i < 0)
				return -1;
			return Index + i;
		}

		public int LastIndexOf(string pattern)
		{
			return LastIndexOf(pattern, Length - 1, Length);
		}*/

		public StringSection Next()
		{
			return new StringSection(Source, Index + Length);
		}

		public StringSection Previous()
		{
			return new StringSection(Source, 0, Index);
		}

		public StringSection TrimLeft()
		{
			var i = 0;
			for (; i < Length; i++)
			{
				if (!char.IsWhiteSpace(this[i]))
					break;
			}
			return Substring(i);
		}

		public StringSection TrimRight()
		{
			var i = Length - 1;
			for (; i >= 0; i--)
			{
				if (!char.IsWhiteSpace(this[i]))
					break;
			}
			return Substring(0, i + 1);
		}

		public StringSection Trim()
		{
			return TrimLeft().TrimRight();
		}

		public IEnumerator<char> GetEnumerator()
		{
			var end = Index + Length;
			for (var i = Index; i < end; i++)
				yield return Source[i];
		}

		public bool Contains(string s)
		{
			return IndexOf(s) >= 0;
		}

		public bool StartsWith(string s)
		{
			if (s.Length > Length)
				return false;
			for (var i = 0; i < s.Length; i++)
				if (s[i] != this[i])
					return false;
			return true;
		}

		public bool EndsWith(string s)
		{
			if (s.Length > Length)
				return false;
			var offset = Index + Length - s.Length;
			for (var i = s.Length - 1; i >= 0; i--)
				if (s[i] != Source[offset + i])
					return false;
			return true;
		}

		[DebuggerStepThrough]
		public bool Equals(StringSection string2)
		{
			if (Length != string2.Length)
				return false;
			for (var i = string2.Length - 1; i >= 0; i--)
				if (this[i] != string2[i])
					return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			return obj is StringSection && Equals((StringSection) obj);
		}

		public override int GetHashCode()
		{
			return ((string) this).GetHashCode();
		}

		[DebuggerStepThrough]
		public static bool operator ==(StringSection string1, StringSection string2)
		{
			return string1.Equals(string2);
		}

		[DebuggerStepThrough]
		public static bool operator !=(StringSection string1, StringSection string2)
		{
			return !string1.Equals(string2);
		}

		[DebuggerStepThrough]
		public static bool operator ==(StringSection string1, string string2)
		{
			return string1.Equals(string2);
		}

		[DebuggerStepThrough]
		public static bool operator !=(StringSection string1, string string2)
		{
			return !string1.Equals(string2);
		}

		[DebuggerStepThrough]
		public static bool operator ==(string string2, StringSection string1)
		{
			return string1.Equals(string2);
		}

		[DebuggerStepThrough]
		public static bool operator !=(string string2, StringSection string1)
		{
			return !string1.Equals(string2);
		}

		#endregion

		#region Overrides

		[DebuggerStepThrough]
		public override string ToString()
		{
			return Value;
		}

		#endregion

		#region Operators

		[DebuggerStepThrough]
		public static implicit operator string(StringSection section)
		{
			return section.Value;
		}

		[DebuggerStepThrough]
		public static explicit operator StringSection(string str)
		{
			return (StringSection) str;
		}

		#endregion
	}
}