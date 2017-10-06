using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Softperson.MVVM
{
	[Serializable]
	public class ObservableObject : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		protected new object MemberwiseClone()
		{
			var clone = (ObservableObject) base.MemberwiseClone();
			clone._propertyChanged = null;
			return clone;
		}

		[NonSerialized] private PropertyChangedEventHandler _propertyChanged;

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			var handler = _propertyChanged;
			handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged<T>(Expression<Func<T>> propertySelector)
		{
			if (_propertyChanged != null)
			{
				if (propertySelector.Body is MemberExpression memberExpression)
					OnPropertyChanged(memberExpression.Member.Name);
				else
					Debug.Fail("PropertyChanged has bad expression tree");
			}
		}

		[NotifyPropertyChangedInvocator]
		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		#endregion
	}
}