#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

#endregion

namespace Softperson
{
	/// <summary>
	///     Summary description for BaseObject.
	/// </summary>
	[DebuggerStepThrough]
	public static class ReflectionTools
	{
		private static int _displayDepth;
		private static HashSet<string> _ignoreSet;

		public static readonly object Char = '\0';
		public static readonly object Int16 = (short) 0;
		public static readonly object Int32 = 0;
		public static readonly object Int64 = 0L;
		public static readonly object Single = 0f;
		public static readonly object Double = 0d;
		public static readonly object Decimal = 0m;
		public static readonly object DateTime = default(DateTime);
		public static readonly object True = true;
		public static readonly object False = false;

		#region Consistency

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Dump(object obj)
		{
			var type = obj.GetType();
			var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			Console.WriteLine(type.Name);
			Console.WriteLine('{');
			var index = 0;
			foreach (var info in props)
			{
				if (index++ > 0) Console.WriteLine(", ");
				Console.WriteLine("{0}={1}", info.Name, info.GetValue(obj, null));
			}
			Console.WriteLine('}');
		}

		#endregion

		public static bool IsDefault(object obj)
		{
			if (obj == null)
				return true;

			var type = obj.GetType();
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Object:
					return false;

				case TypeCode.String:
					return obj.ToString() == string.Empty;

				case TypeCode.Int64:
				case TypeCode.UInt16:
				case TypeCode.Decimal:
				case TypeCode.Single:
				case TypeCode.Double:
					return Convert.ToDouble(obj) == 0;

				case TypeCode.DateTime:
					return Convert.ToDateTime(obj) == new DateTime();

				default:
					return Convert.ToInt32(obj) == 0;
			}
		}

		public static string Display(object obj)
		{
			if (_displayDepth > 2)
				return obj.GetType().Name;

			try
			{
				_displayDepth++;
				return Display(obj, true);
			}
			finally
			{
				_displayDepth--;
			}
		}

		private static void InitDisplay()
		{
			if (_ignoreSet != null)
				return;

			_ignoreSet = new HashSet<string>
			{"SyncRoot", "Item", "Capacity", "IsFixedSize"};
		}

		public static string Display(object obj, bool inherited)
		{
			InitDisplay();
			var sb = new StringBuilder();
			var type = obj.GetType();

			sb.Append(type.Name);
			sb.Append('(');

			var inheritFlags = inherited ? 0 : BindingFlags.DeclaredOnly;
			var infoList = type.GetProperties(BindingFlags.Instance | BindingFlags.Public
											  | inheritFlags);

			var prev = false;
			foreach (var info in infoList)
			{
				if ((info.Attributes &
					 (PropertyAttributes.RTSpecialName |
					  PropertyAttributes.SpecialName)) != 0)
					continue;

				var name = info.Name;
				if (_ignoreSet.Contains(name))
					continue;

				var atts =
					(BrowsableAttribute[]) info.GetCustomAttributes(typeof (BrowsableAttribute), true);

				if (atts.Length == 1 &&
					!atts[0].Browsable)
					continue;

//				ParameterInfo[] paramList = info.GetIndexParameters();
//				if (paramList != null && paramList.Length>0)
//					continue;

				if (!info.CanRead)
					continue;

				var value = info.GetValue(obj, null);
				if (value == null
					|| value == obj
					|| IsDefault(value)
					|| value is ICollection)
					continue;

				var valueString = value.ToString();
				if (valueString.Length == 0 || valueString == "0")
					continue;

				if (prev) sb.Append(", ");

				if (value is bool)
					sb.Append(name);
				else if (name == "Name")
					sb.Append(valueString);
				else
					sb.AppendFormat("{0}={1}", name, valueString);
				prev = true;
			}

			sb.Append(')');
			return sb.ToString();
		}

		private static Dictionary<int, string> CreateMap(Type type)
		{
			var map = new Dictionary<int, string>();
			foreach (var info in type.GetFields())
			{
				if (!info.IsLiteral || info.FieldType != typeof (int))
					continue;
				map[(int) info.GetValue(null)] = info.Name;
			}
			return map;
		}

		#region Reflection

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static object Call(object obj, string method)
		{
			return Call(obj, method, null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static object Call(object obj, string method, params object[] args)
		{
			var type = obj.GetType();
			return type.InvokeMember(method, 0, null, obj, args);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Pure]
		public static object Read(object obj, string property)
		{
			var type = obj.GetType();
			return type.InvokeMember(property, 0, null, obj, null);
			// return obj.GetType().GetProperty(property).GetValue(obj, null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Write(object obj, string property, object value)
		{
			var type = obj.GetType();
			type.InvokeMember(property, 0, null, obj, new[] {value});
			// obj.GetType().GetProperty(property).SetValue(obj, value, null);
		}

		#endregion
	}
}