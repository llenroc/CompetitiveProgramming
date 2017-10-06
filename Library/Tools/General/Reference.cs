namespace Softperson
{
	public class Reference<T>
	{
		#region Variables

		public T Value;

		#endregion

		#region Constructor

		public Reference()
		{
		}

		public Reference(T value)
		{
			Value = value;
		}

		#endregion
	}
}