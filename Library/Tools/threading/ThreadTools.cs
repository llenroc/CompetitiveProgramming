#region Using

using System;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace Softperson.Threading
{
	public static class ThreadTools
	{
		public static int InterlockedAndOr(ref int target, int and, int or)
		{
			int startVal, currentVal = target;

			// Don't access target in the loop except in an attempt 
			// to change it because another thread may be touching it 
			do
			{
				// Record this iteration's starting value
				startVal = currentVal;

				// Calculate the desired value (this example ANDs the bits)
				var desiredVal = (startVal & and) | or;

				// if (target == startVal) target = desiredVal;
				// Value prior to potential change is returned
				currentVal = Interlocked.CompareExchange(
					ref target, desiredVal, startVal);

				// If the starting value changed during this iteration, repeat 
			} while (startVal != currentVal);

			// Return this iteration's starting value
			return currentVal;
		}

		public static long Update(ref long target, Func<long, long> func)
		{
			var currentVal = target;
			long startVal;

			do
			{
				startVal = currentVal;
				var desiredVal = func(startVal);
				if (desiredVal == startVal)
					break;
				currentVal = Interlocked.CompareExchange(
					ref target, desiredVal, startVal);
			} while (startVal != currentVal);
			return startVal;
		}

		public static int Update(ref int target, Func<int, int> func)
		{
			var currentVal = target;
			int startVal;

			do
			{
				startVal = currentVal;
				var desiredVal = func(startVal);
				if (desiredVal == startVal)
					break;
				currentVal = Interlocked.CompareExchange(
					ref target, desiredVal, startVal);
			} while (startVal != currentVal);
			return startVal;
		}

		public static T Update<T>(ref T target, Func<T, T> func)
			where T : class
		{
			var currentVal = target;
			T startVal;

			do
			{
				startVal = currentVal;
				var desiredVal = func(startVal);
				if (desiredVal == startVal)
					break;
				currentVal = Interlocked.CompareExchange(
					ref target, desiredVal, startVal);
			} while (startVal != currentVal);
			return startVal;
		}

		public static void ForEach<T>(IEnumerable<T> list, Action<T> action)
		{
			var tasks = 1;
			using (var e = new ManualResetEvent(false))
			{
				foreach (var value in list)
				{
					Interlocked.Increment(ref tasks);
					action.BeginInvoke(value, delegate(IAsyncResult ar)
					{
						if (Interlocked.Decrement(ref tasks) == 0)
							e.Set();
						action.EndInvoke(ar);
					},
						null);
				}

				if (0 == Interlocked.Decrement(ref tasks))
					e.Set();
				e.WaitOne();
			}
		}
#pragma warning disable 0420 
	}

	public class Counting<T>
	{
		#region Constructor

		public Counting(T obj, int initialCount, Action<T> action)
		{
			Obj = obj;
			m_count = initialCount;
			m_action = action;
		}

		#endregion

		#region Properties

		public T Obj { get; }

		#endregion

		#region Variables

		private readonly Action<T> m_action;
		private volatile int m_count;

		#endregion

		#region Methods

		public int AddRef()
		{
			int c;
			if ((c = Interlocked.Increment(ref m_count)) == 1)
				throw new Exception();
			return c;
		}

		public int Release()
		{
			int c;
			if ((c = Interlocked.Decrement(ref m_count)) == 0)
				m_action(Obj);
			return c;
		}

		#endregion
	}
}