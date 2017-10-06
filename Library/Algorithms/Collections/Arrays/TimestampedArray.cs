using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softperson.Collections
{
	public class TimestampedArray<T> 
	{
		public T[] Array;
		public int[] TimeStamp;
		public int Time;
		public T DefaultValue;

		public TimestampedArray(int size, T defaultValue = default(T))
		{
			Array = new T[size];
			TimeStamp = new int[size];
			DefaultValue = defaultValue;
		}

		public T this[int x]
		{
			get
			{
				return TimeStamp[x] >= Time ? Array[x] : DefaultValue;
			}
			set
			{
				Array[x] = value;
				TimeStamp[x] = Time;
			}
		}

		public bool ContainsKey(int x)
		{
			return TimeStamp[x] >= Time;
		}

		public void InitializeAll()
		{
			for (int i = 0; i < Array.Length; i++)
			{
				if (TimeStamp[i]>Time) continue;
				Array[i] = DefaultValue;
				TimeStamp[i] = Time;
			}
		}

		public void Clear()
		{
			Time++;
		}
	}
}
