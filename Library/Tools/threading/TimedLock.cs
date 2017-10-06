#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2005, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;

#endregion

namespace Softperson.Threading
{
	/// <summary>
	///     Class provides a nice way of obtaining a lock that will time out
	///     with a cleaner syntax than using the whole Monitor.TryEnter() method.
	/// </summary>
	/// <remarks>
	///     Adapted from Ian Griffiths article http://www.interact-sw.co.uk/iangblog/2004/03/23/locking
	///     and incorporating suggestions by Marek Malowidzki as outlined in this blog post
	///     http://www.interact-sw.co.uk/iangblog/2004/05/12/timedlockstacktrace
	/// </remarks>
	/// <example>
	///     Instead of:
	///     <code>
	///  lock(obj)
	///  {
	/// 		//Thread safe operation
	///  }
	///  
	///  do this:
	///  
	///  using(TimedLock.Lock(obj))
	///  {
	/// 		//Thread safe operations
	///  }
	///  
	///  or this:
	///  
	///  try
	///  {
	/// 		TimedLock timeLock = TimedLock.Lock(obj);
	/// 		//Thread safe operations
	/// 		timeLock.Dispose();
	///  }
	///  catch(LockTimeoutException e)
	///  {
	/// 		Console.WriteLine("Couldn't get a lock!");
	/// 		StackTrace otherStack = e.GetBlockingThreadStackTrace(5000);
	/// 		if(otherStack == null)
	/// 		{
	/// 			Console.WriteLine("Couldn't get other stack!");
	/// 		}
	/// 		else
	/// 		{
	/// 			Console.WriteLine("Stack trace of thread that owns lock!");
	/// 		}
	/// 		
	///  }
	///  </code>
	/// </example>
	public struct TimedLock : IDisposable
	{
		#region Factory Methods

		private readonly object _target;

		private TimedLock(object o)
		{
			_target = o;
		}

		/// <summary>
		///     Disposes of this lock.
		/// </summary>
		public void Dispose()
		{
			// Owning thread is done.
#if DEBUG
			try
			{
				//This shouldn't throw an exception.
				LockTimeoutException.ReportStackTraceIfError(_target);
			}
			finally
			{
				//But just in case...
				Monitor.Exit(_target);
			}
#else
			Monitor.Exit(_target);
#endif
		}

		/// <summary>
		///     Attempts to obtain a lock on the specified object for up
		///     to 10 seconds.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static TimedLock Lock(object o)
		{
			return Lock(o, TimeSpan.FromSeconds(10));
		}

		/// <summary>
		///     Attempts to obtain a lock on the specified object for up to
		///     the specified timeout.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public static TimedLock Lock(object o, TimeSpan timeout)
		{
			var tl = new TimedLock(o);
			if (!Monitor.TryEnter(o, timeout))
			{
				// Failed to acquire lock.

				throw new LockTimeoutException(o.ToString());
			}
			return tl;
		}

		#endregion
	}

	/// <summary>
	///     Thrown when a lock times out.
	/// </summary>
	[Serializable]
	public class LockTimeoutException : ApplicationException
	{
		/// <summary>
		///     Creates a new <see cref="LockTimeoutException" /> instance.
		/// </summary>
		public LockTimeoutException()
			: base("Timeout waiting for lock")
		{
		}

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="message"></param>
		public LockTimeoutException(string message)
			: base(message)
		{
		}

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public LockTimeoutException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected LockTimeoutException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		///     Returns a string representation of the exception.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var toString = base.ToString();
#if DEBUG
			if (_blockingStackTrace != null)
			{
				toString += "\n-------Blocking Stack Trace--------\n" + _blockingStackTrace;
			}
#endif
			return toString;
		}

#if DEBUG
		private readonly object _lockTarget;
		private StackTrace _blockingStackTrace;
		private static readonly Hashtable _failedLockTargets = new Hashtable();

		/// <summary>
		///     Sets the stack trace for the given lock target
		///     if an error occurred.
		/// </summary>
		/// <param name="lockTarget">Lock target.</param>
		public static void ReportStackTraceIfError(object lockTarget)
		{
			lock (_failedLockTargets)
			{
				if (_failedLockTargets.ContainsKey(lockTarget))
				{
					var waitHandle = _failedLockTargets[lockTarget] as ManualResetEvent;
					if (waitHandle != null)
					{
						waitHandle.Set();
					}
					_failedLockTargets[lockTarget] = new StackTrace();
					//Now's a good time to use your favorite logging 
					//framework and log this.  Be sure to note that this 
					//is the owning thread.

					//Also. if you don't call GetBlockingStackTrace()
					//the lockTarget doesn't get removed from the hash 
					//table and so we'll always think there's an error
					//here (though no locktimeout exception is thrown).
				}
			}
		}

		/// <summary>
		///     Creates a new <see cref="LockTimeoutException" /> instance.
		/// </summary>
		/// <remarks>Use this exception.</remarks>
		/// <param name="lockTarget">Object we tried to lock.</param>
		public LockTimeoutException(object lockTarget)
			: base("Timeout waiting for lock")
		{
			lock (_failedLockTargets)
			{
				// This is safer in case somebody forgot to remove 
				// the lock target.
				var waitHandle = new ManualResetEvent(false);
				_failedLockTargets[lockTarget] = waitHandle;
			}
			_lockTarget = lockTarget;
		}

		/// <summary>
		///     Stack trace of the thread that holds a lock on the object
		///     this lock is attempting to acquire when it fails.
		/// </summary>
		/// <param name="timeout">Number of milliseconds to wait for the blocking stack trace.</param>
		public StackTrace GetBlockingStackTrace(int timeout)
		{
			if (timeout < 0)
				throw new InvalidOperationException(
					"We'd all like to be able to go back in time, but this is not allowed. Please choose a positive wait time.");

			ManualResetEvent waitHandle = null;
			lock (_failedLockTargets)
			{
				waitHandle = _failedLockTargets[_lockTarget] as ManualResetEvent;
			}
			if (timeout > 0 && waitHandle != null)
			{
				waitHandle.WaitOne(timeout, false);
			}
			lock (_failedLockTargets)
			{
				//Hopefully by now we have a stack trace.
				_blockingStackTrace = _failedLockTargets[_lockTarget] as StackTrace;
			}

			return _blockingStackTrace;
		}
#endif
	}
}