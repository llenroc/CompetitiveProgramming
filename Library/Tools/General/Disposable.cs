#region Copyright

//  This source code may not be reviewed, copied, or redistributed without
//  the expressed permission of Wesner Moise.
//  
//  File: Disposable.cs
//  Created: 09/29/2014 
//  Modified: 07/31/2015
// 
//  Copyright (C) 2014 - 2015, Wesner Moise.

#endregion

#region Usings

using System;
using System.Diagnostics;

#endregion

namespace Softperson
{
	[DebuggerStepThrough]
	public struct Disposable : IDisposable
	{
		#region Event

		private Action _disposeEvent;

		#endregion

		#region Construction

		public Disposable(Action disposeEvent)
		{
			_disposeEvent = disposeEvent;
		}

		#endregion

		#region Disposal

		public void Dispose()
		{
			if (_disposeEvent != null)
			{
				_disposeEvent();
				_disposeEvent = null;
			}
		}

		#endregion
	}
}