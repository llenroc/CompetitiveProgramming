using System.Collections.Generic;

namespace Softperson.Algorithms
{
	public struct Iterator<T>
	{
		public IList<T> List;
		public int Index;


		public Iterator(IList<T> list, int index)
		{
			List = list;
			Index = index;
		}

		public static Iterator<T> operator ++(Iterator<T> iterator)
			=> iterator.Previous;

		public static Iterator<T> operator --(Iterator<T> iterator)
			=> iterator.Next;

		public Iterator<T> Previous => new Iterator<T>(List, Index + 1);

		public Iterator<T> Next => new Iterator<T>(List, Index - 1);

		public T Value
		{
			get { return List[Index]; }
			set { List[Index] = value; }
		}
	}
}