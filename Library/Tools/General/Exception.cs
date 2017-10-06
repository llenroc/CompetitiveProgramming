#region Using

using System;

#endregion

namespace Softperson
{
	[Serializable]
	public class Exception<T> : ApplicationException
	{
		public readonly T Info;

		public Exception(T info, string message = "")
			: base(message)
		{
			Info = info;
		}

		public Exception(string message = null)
			: base(message)
		{
		}
	}
}