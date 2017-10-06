#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2005, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.ComponentModel;
using System.Threading;

#endregion

namespace Softperson.Threading
{
	public class Worker : BackgroundWorker
	{
		#region Constructors

#if !MONO
		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_pauseEvent != null)
					_pauseEvent.Close();
			}
			base.Dispose(disposing);
		}
#endif

		#endregion

		#region Overrides

		public override string ToString()
		{
			return Name;
		}

		#endregion

		#region Fields

		private volatile AutoResetEvent _pauseEvent;
		private volatile int _pauseState;

		#endregion

		#region Properties

		public string Name = "";

		public bool IsPaused
		{
			get { return _pauseState != 0; }
		}

		#endregion

		#region Methods

		protected AutoResetEvent PauseEvent
		{
			get
			{
				if (_pauseEvent == null)
					lock (_pauseEvent)
					{
						if (_pauseEvent == null)
							_pauseEvent = new AutoResetEvent(false);
					}

				return _pauseEvent;
			}
		}

		public void Pause()
		{
			lock (this)
			{
				PauseEvent.Reset();
				_pauseState = 1;
			}
		}

		public void Resume()
		{
			if (!IsPaused)
				return;

			lock (this)
			{
				_pauseState = 0;
				PauseEvent.Set();
			}
		}

		protected void CheckPause()
		{
			if (IsPaused)
				_pauseEvent.WaitOne();
		}

		#endregion
	}
}