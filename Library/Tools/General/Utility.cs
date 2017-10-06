#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

#endregion

namespace Softperson
{
#pragma warning disable 414
	// ReSharper disable MethodOverloadWithOptionalParameter

	[DebuggerNonUserCode]
	[DebuggerStepThrough]
	public static class Utility
	{
		public static long C(long n, long m)
		{
			var n2 = n;

			if (m + m > n)
				m = n - m;

			long product = 1;
			for (var i = 1; i <= m; i++)
				product = product*n2--/i;
			return product;
		}

		public static long Fact(long n)
		{
			if (n > 50)
				n = 50;

			long prod = 1;
			for (var i = 2; i <= n; i++)
				prod *= i;
			return prod;
		}

		#region General

		/// <summary>
		///     Swaps two variables
		/// </summary>
		/// <param name="var1">first variable</param>
		/// <param name="var2">second variable</param>
		public static void Swap<T>(ref T var1, ref T var2)
		{
			var tmp = var1;
			var1 = var2;
			var2 = tmp;
		}

		public static void Swap<T>(IList<T> list, int index1, int index2)
		{
			var tmp = list[index1];
			list[index1] = list[index2];
			list[index2] = tmp;
		}

		public static void Normalize<T>(ref T var1, ref T var2)
			where T : IComparable<T>
		{
			if (var1 != null && var1.CompareTo(var2) > 0)
				Swap(ref var1, ref var2);
		}

		private static Func<object, object> _cloner;

		public static T MemberwiseClone<T>(T o)
		{
			if (_cloner == null)
			{
				var info = typeof (object).GetMethod("MemberwiseClone",
					BindingFlags.Instance | BindingFlags.NonPublic);
				_cloner = (Func<object, object>)
					Delegate.CreateDelegate(typeof (Func<object, object>), info);
			}
			// ReSharper disable PossibleNullReferenceException
			return (T) _cloner(o);
			// ReSharper restore PossibleNullReferenceException
		}

		public static T Convert<T>(object o)
		{
			if (o == null) return default(T);
			return (T) Converters.Convert(o, o.GetType(), typeof (T));
		}

		public static T Parse<T>(string text)
		{
			return (T) Converters.Parse(text, typeof (T), null);
		}

		public static T New<T>()
		{
			return (T) FormatterServices.GetUninitializedObject(typeof (T));
		}

		#endregion

		#region Math

		public static int Hex(string text)
		{
			return int.Parse(text, NumberStyles.HexNumber);
		}

		public static T Max<T>(params T[] numbers)
			where T : IComparable<T>
		{
			if (numbers.Length == 0)
				return default(T);

			var result = numbers[0];
			for (var i = 1; i < numbers.Length; i++)
				if (result.CompareTo(numbers[i]) < 0)
					result = numbers[i];

			return result;
		}

		public static T Min<T>(params T[] numbers)
			where T : IComparable<T>
		{
			if (numbers.Length == 0)
				return default(T);

			var result = numbers[0];
			for (var i = 1; i < numbers.Length; i++)
				if (result.CompareTo(numbers[i]) > 0)
					result = numbers[i];

			return result;
		}

		[DebuggerStepThrough]
		public static int Sqr(int number)
		{
			return number*number;
		}

		[DebuggerStepThrough]
		public static double Sqr(double number)
		{
			return number*number;
		}

		#endregion

		#region Logical

		[DebuggerStepThrough]
		public static bool IsOlder(object obj1, object obj2)
		{
			if (obj1 == obj2 || obj2 == null) return false;
			if (obj1 == null) return true;

			var cmp = GC.GetGeneration(obj1) - GC.GetGeneration(obj2);
			if (cmp != 0)
				return cmp > 0;

			return FuncTools.IsOlder(obj1, obj2);
		}

		[DebuggerStepThrough]
		public static bool IsOlder2(object obj1, object obj2)
		{
			if (obj1 == obj2 || obj2 == null) return false;
			if (obj1 == null) return true;
			return FuncTools.IsOlder(obj1, obj2);
		}

		[DebuggerStepThrough]
		public static bool Equals<T>(ref T obj1, ref T obj2)
			where T : class, IEquatable<T>
		{
			if (obj1 == obj2)
				return true;
			if (obj1 == null || !obj1.Equals(obj2))
				return false;
			Exchange(ref obj1, ref obj2);
			return true;
		}

		public static void Exchange<T>(ref T obj1, ref T obj2) where T : class
		{
			if (obj1 == obj2)
				return;

			if (IsOlder(obj1, obj2))
				obj2 = obj1;
			else
				obj1 = obj2;
		}

		/// <summary>
		///     Implication operator
		/// </summary>
		/// <param name="premise">premise</param>
		/// <param name="conclusion">conclusion</param>
		/// <returns>returns true iff premise is not true or conclusion is true</returns>
		public static bool Implies(this bool premise, bool conclusion)
		{
			return !premise || conclusion;
		}

		public static T Cast<T>(object value, T defaultValue = default(T))
		{
			if (value == null || value == DBNull.Value)
				return defaultValue;

			if (value is T)
				return (T) value;

			Break();
			return (T) System.Convert.ChangeType(value, typeof (T));
		}

		[DebuggerStepThrough]
		public static T Coerce<T>(T value, T defaultValue) where T : IComparable<T>
		{
			if (value.Equals(default(T)))
				return defaultValue;
			return value;
		}

		#endregion

		#region Random Numbers

		private static readonly Random _random = new Random();

		[DebuggerStepThrough]
		public static double RandomDouble()
		{
			return _random.NextDouble();
		}

		[DebuggerStepThrough]
		public static int RandomInteger()
		{
			return _random.Next();
		}

		[DebuggerStepThrough]
		public static int RandomInteger(int minValue, int maxValue)
		{
			return _random.Next(minValue, maxValue);
		}

		#endregion

		#region Hash Code

		public const double GoldenRatio = 1.618033988749895;
		// Important for number to be odd and have the high bit set
		private const long GoldenRatioBits = 2654435769;


		[DebuggerStepThrough]
		public static int CreateHashCodeRange(int value, int range)
		{
			unchecked
			{
				long hash = (uint) (value*GoldenRatioBits);
				return (int) ((hash*(uint) range) >> 32);
			}
		}

		[DebuggerStepThrough]
		public static int CreateHashCode(int value)
		{
			return (int) ((value*GoldenRatioBits) & int.MaxValue);
		}

		[DebuggerStepThrough]
		public static int CreateHashCode(int first, int second)
		{
			return CreateHashCode(CreateHashCode(first) ^ second);
		}

		[DebuggerStepThrough]
		public static int CreateHashCode(int first, int second, int third)
		{
			return CreateHashCode(first, CreateHashCode(second, third));
		}

		public static int GetHashCode(object obj)
		{
			return obj == null ? 0x1234567 : obj.GetHashCode();
		}

		[DebuggerStepThrough]
		public static int CreateHashCode<T>(IList<T> array)
		{
			if (array == null) return 0;
			return CreateHashCode(array, 0, array.Count);
		}

		[DebuggerStepThrough]
		public static int CreateHashCode<T>(IList<T> array, int start, int count)
		{
			if (array == null) return 0;
			var hashcode = CreateHashCode(count);
			for (var i = 0; i < count; i++)
				hashcode = CreateHashCode(hashcode ^ array[start + i].GetHashCode());
			return hashcode;
		}

		[DebuggerStepThrough]
		public static string ComputeMd5Hash(string text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;
			var bytes = Encoding.UTF8.GetBytes(text);
			return ComputeMd5Hash(bytes);
		}

		[DebuggerStepThrough]
		public static string ComputeMd5Hash(byte[] bytes)
		{
			if (bytes == null || bytes.Length == 0)
				return string.Empty;
			var md = MD5.Create();
			return BitConverter.ToString(md.ComputeHash(bytes));
		}

		private static int _counter;

		public static int UniqueId()
		{
			return Interlocked.Increment(ref _counter);
		}

		#endregion

		#region Min and Max

		public static T Max<T>(this T a, T b) where T : IComparable<T>
		{
			if (a == null) return b;
			if (b == null) return a;
			return a.CompareTo(b) >= 0 ? a : b;
		}

		public static T Min<T>(this T a, T b) where T : IComparable<T>
		{
			if (a == null) return default(T);
			if (b == null) return default(T);
			return a.CompareTo(b) <= 0 ? a : b;
		}

		public static DateTime MinDate(DateTime t1, DateTime t2)
		{
			var min = Min(t1, t2);
			if (min != default(DateTime))
				return min;
			return Max(t1, t2);
		}

		public static DateTime Truncate(this DateTime dateTime, long timeSpan)
		{
			return dateTime.AddTicks(-(dateTime.Ticks%timeSpan));
		}

		public static int NullCompareTo<T>(this T a, T b) where T : IComparable<T>
		{
			if (a == null) return b == null ? 0 : -1;
			if (b == null) return 1;
			return a.CompareTo(b);
		}

		#endregion

		#region Reflection

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static object InvokeMember(this object obj, string method,
			params object[] args)
		{
			var type = obj.GetType();
			return type.InvokeMember(method, 0, null, obj, args);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static object GetMember(this object obj, string property)
		{
			return obj.GetType().GetProperty(property).GetValue(obj, null);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetMember(this object obj, string property, object value)
		{
			obj.GetType().GetProperty(property).SetValue(obj, value, null);
		}

		#endregion

		#region Diagnostics

		[DebuggerStepThrough]
		[Conditional("DEBUG")]
		public static void Ignore(object o)
		{
		}

#if DEBUG
		private static readonly Dictionary<string, int> _breakpoints = new Dictionary<string, int>();
		private static string _lastFileLine;
#endif

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void Break(int count = 1, int skip = 1)
		{
#if DEBUG
			var frame = new StackFrame(skip, true);
			var text = frame.GetFileName() + ":" + frame.GetFileLineNumber();
			_lastFileLine = text;
			if (!_breakpoints.ContainsKey(text))
				_breakpoints.Add(text, count);

			var passes = _breakpoints[text];
			if (passes <= 0)
				return;

			_breakpoints[text] = passes - 1;
			if (Debugger.IsAttached)
				Debugger.Break();
#endif
		}


		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void Assert(bool expr, string message = null)
		{
			if (!expr) Break(2, 3);
		}

		[DebuggerStepThrough]
		public static bool NotNull<T>(T obj)
		{
			return obj != null;
		}

		[DebuggerStepThrough]
		[Conditional("DEBUG")]
		// ReSharper disable once UnusedParameter.Global
		public static void AssertNotNull<T>(T obj)
		{
			Debug.Assert(obj != null);
		}

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void Fail(string message = null)
		{
			Break(2, 3);
		}

		[Conditional("DEBUG")]
		private static void Indent(int space)
		{
			Debug.Write(new string(' ', space));
		}

		[Conditional("DEBUG")]
		public static void WriteProperties(object obj, int indent = 1,
			bool fields = false, bool baseType = false)
		{
			var type = obj.GetType();

			Indent(indent);
			Debug.WriteLine("{0} ({1})", type.Name);
			do
			{
				if (fields)
				{
					foreach (var fieldInfo in type.GetFields())
					{
						object result;
						try
						{
							result = fieldInfo.GetValue(obj);
						}
						catch (Exception e)
						{
							result = e.Message;
						}
						Indent(indent);
						Debug.WriteLine("  {0} = {1}", fieldInfo.Name, result);
					}
				}

				foreach (var propertyInfo in type.GetProperties())
				{
					if (!propertyInfo.CanRead || propertyInfo.GetIndexParameters().GetLength(0) > 0)
						continue;
					object result;
					try
					{
						result = propertyInfo.GetValue(obj, null);
					}
					catch (Exception e)
					{
						result = e.Message;
					}

					if (result == null
						|| result is int && (int) result == 0
						|| result is bool && (bool) result == false
						|| result as string == string.Empty)
						continue;

					Indent(indent);
					Type type2;
					if ((type2 = result.GetType()).IsPrimitive && !type2.IsArray)
						Debug.WriteLine("{0} := {1}", propertyInfo.Name, result);
					else if (type2.IsArray)
						Debug.WriteLine("{0} := {1}[{2}]", propertyInfo.Name, type2.BaseType, ((Array) result).Length);
				}

				type = type.BaseType;
			} while (baseType && type != null && type != typeof (object));
			Debug.WriteLine("");
		}

		#endregion

		#region NOOP

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void NoOp(object obj = null)
		{
		}

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void NoOp(params object[] array)
		{
		}

		[DebuggerHidden]
		public static bool TryCatch(Action action)
		{
			try
			{
				action();
				return true;
			}
			catch (Exception e)
			{
				NoOp(e);
				return false;
			}
		}

		[DebuggerHidden]
		public static T TryCatch<T>(Func<T> action)
		{
			try
			{
				return action();
			}
			catch (Exception e)
			{
				NoOp(e);
				return default(T);
			}
		}

		#endregion

		#region System

		public static void LaunchBrowser(string url)
		{
			try
			{
				Process.Start(url);
			}
			catch
			{
			}
		}

		public static int Shell(string cmd)
		{
			using (var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo
				{
					FileName = "CMD.exe ",
					RedirectStandardError = false,
					RedirectStandardOutput = false,
					RedirectStandardInput = false,
					UseShellExecute = false,
					CreateNoWindow = true,
					Arguments = "/D /c " + cmd
				}
			})
			{
				process.Start();
				process.WaitForExit(int.MaxValue); //or the wait time you want
				var errorCode = process.ExitCode;

				//Now we need to see if the process was successful
				if (errorCode > 0 & !process.HasExited)
					process.Kill();

				//now clean up after ourselves
				return errorCode;
			}
		}

		#endregion

		#region Reflection

		public static Exception InnerException(Exception e)
		{
			var inner = e.InnerException;
			if (inner == null)
				return e;
			return InnerException(inner);
		}

		public static bool SetProperty(object obj, string prop, string value)
		{
			var info = GetPropertyInfo(obj, prop);
			object newValue = value;
			if (info.PropertyType != typeof (string))
			{
				var newType = TypeDescriptor.GetConverter(info.PropertyType);
				newValue = newType.ConvertFrom(value);
			}
			info.SetValue(obj, newValue, null);
			return true;
		}

		public static PropertyInfo GetPropertyInfo(object obj, string prop)
		{
			if (obj == null)
				return null;

			// ReSharper disable AssignNullToNotNullAttribute
			var type = obj.GetType();
			var info = type.GetProperty(prop,
				BindingFlags.Public
				| BindingFlags.Instance
				| BindingFlags.FlattenHierarchy
				| BindingFlags.IgnoreCase,
				null, null, null, null);
			return info;
		}

		#endregion

		#region Errors

		[StringFormatMethod("message")]
		public static void Warning(string message, params object[] args)
		{
			Error(ConsoleColor.Yellow, message, args);
		}

		[StringFormatMethod("message")]
		public static void Info(string message = "", params object[] args)
		{
			Error(ConsoleColor.Cyan, message, args);
		}

		[StringFormatMethod("message")]
		public static void Error(string message = "", params object[] args)
		{
			Error(ConsoleColor.Red, message, args);
		}

		[StringFormatMethod("message")]
		public static void Error(ConsoleColor color, string message, params object[] args)
		{
#if !MONO
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
#endif
			Console.Error.WriteLine(StringTools.Format(message, args));
			Console.Error.Flush();

#if !MONO
			Console.ForegroundColor = oldColor;
#endif
		}

		#endregion

		#region Debugging

		public static bool? IsBrowsable(MemberInfo info)
		{
			var attr = info.GetCustomAttribute<BrowsableAttribute>();

			return attr?.Browsable;
		}

		public static string GetDescription(MemberInfo info)
		{
			var attr = info.GetCustomAttribute<DescriptionAttribute>();

			return attr?.Description;
		}

		#endregion
	}
}