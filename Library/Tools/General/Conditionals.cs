using System;
using JetBrains.Annotations;

namespace Softperson
{
	public static class Conditionals
	{
		[PublicAPI]
		public static bool IsDebug
		{
			get
			{
#if DEBUG
				return true;
#else
				return false;
#endif
			}
		}

		[PublicAPI]
		public static bool IsMono
		{
			get
			{
#if MONO
				return true;
#else
				return false;
#endif
			}
		}

		[PublicAPI]
		public static bool IsMonotouch
		{
			get
			{
#if MONOTOUCH
				return true;
#else
				return false;
#endif
			}
		}

		[PublicAPI]
		public static bool Is64Bit()
		{
			return Environment.Is64BitProcess;
		}
	}
}