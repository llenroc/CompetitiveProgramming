#region Using

using System;
using System.Diagnostics;
using JetBrains.Annotations;

#endregion

namespace Softperson
{
	public interface IWriteable
	{
		void Write(string message);
	}

	// ReSharper disable MethodOverloadWithOptionalParameter

	public static class Writeable
	{
		public static void Write(this IWriteable w, string message)
		{
			w.Write(message);
		}

		[DebuggerStepThrough]
		public static void WriteLine(this IWriteable w, string message)
		{
			w.Write(message);
			w.WriteLine();
		}

		[DebuggerStepThrough]
		public static void WriteLine(this IWriteable w)
		{
			w.Write(Environment.NewLine);
		}

		[DebuggerStepThrough]
		[StringFormatMethod("format")]
		public static void WriteLine(this IWriteable w, string format, params object[] args)
		{
// ReSharper disable once RedundantStringFormatCall
			w.WriteLine(string.Format(format, args));
		}

		[DebuggerStepThrough]
		[StringFormatMethod("format")]
		public static void Write(this IWriteable w, string format, params object[] args)
		{
// ReSharper disable once RedundantStringFormatCall
			w.Write(string.Format(format, args));
		}
	}
}