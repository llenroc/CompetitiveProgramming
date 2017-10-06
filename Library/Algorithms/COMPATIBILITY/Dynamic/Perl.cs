using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Softperson.Collections;

namespace Softperson.Scripting
{
	public static class Perl
	{
		public static IEnumerator Out<T>(this string s, out T v)
		{
			return s.Split().Out(out v);
		}

		public static IEnumerator Out<T>(this IEnumerable enumerable, out T v)
		{
			var enumerator = enumerable.GetEnumerator();
			return enumerator.Out(out v);
		}

		public static IEnumerator Out<T>(this IEnumerator enumerator, out T v)
		{
			if (enumerator.MoveNext())
			{
				var obj = enumerator.Current;
				v = Converters.Convert<T>(obj);
				return enumerator;
			}

			v = default(T);
			return enumerator;
		}

		public static IEnumerator<T> Out<T>(this IEnumerable<T> enumerable, out T v)
		{
			var enumerator = enumerable.GetEnumerator();
			return enumerator.Out(out v);
		}

		public static IEnumerator<T> Out<T>(this IEnumerator<T> enumerator, out T v)
		{
			if (enumerator.MoveNext())
			{
				v = enumerator.Current;
				return enumerator;
			}

			v = default(T);
			return enumerator;
		}

		public static IEnumerator<string> Out(this IEnumerator<string> enumerator, out string v)
		{
			if (enumerator.MoveNext())
			{
				v = enumerator.Current;
				return enumerator;
			}

			v = "";
			return enumerator;
		}

		public static IEnumerator<T[]> Out<T>(this IEnumerator<T[]> enumerator, out T[] v)
		{
			if (enumerator.MoveNext())
			{
				v = enumerator.Current;
				return enumerator;
			}

			v = ArrayTools.EmptyArray<T>();
			return enumerator;
		}

		public static IEnumerator<string> Out(this MatchCollection matches, out string v)
		{
			var array = matches.Cast<Match>().Select(m => m.Value).ToArray();
			return array.Out(out v);
		}

		public static IEnumerator Out<T>(this MatchCollection matches, out T v)
		{
			var array = matches.Cast<Match>().Select(m => m.Value).ToArray();
			return array.Out(out v);
		}

		public static IEnumerator<string> Out(this Match match, out string v)
		{
			var array = match.Groups.Cast<Group>().Skip(1).Select(m => m.Value).ToArray();
			return array.Out(out v);
		}

		public static IEnumerator Out<T1, T2>(this MatchCollection matches, out T1 t1, out T2 t2)
		{
			return matches.Out(out t1).Out(out t2);
		}

		public static IEnumerator Out(this Match matches, out string t1, out string t2)
		{
			return matches.Out(out t1).Out(out t2);
		}

		public static string XmlOut(Link<object> obj, string rootname = "Root")
		{
			throw new NotImplementedException();
		}

		public static int SmartCompare(object obj1, object obj2)
		{
			if (Equals(obj1, obj2)) return 0;
			if (obj1 == null) return -1;
			if (obj2 == null) return 1;
			if (obj1.GetType() == obj2.GetType())
			{
				var comparable = obj1 as IComparable;
				if (comparable != null)
					return comparable.CompareTo(obj2);
			}
			return string.CompareOrdinal(obj1.ToString(), obj2.ToString());
		}

		public static bool Not(object obj)
		{
			if (obj == null)
				return true;

			switch (Type.GetTypeCode(obj.GetType()))
			{
				case TypeCode.String:
					return (string) obj == "";
				case TypeCode.Int32:
					return (int) obj == 0;
				case TypeCode.Boolean:
					return (bool) obj == false;
				case TypeCode.Double:
					return (double) obj == 0;
				default:
					var c = obj as IConvertible;
					return c != null && c.ToDouble(null) == 0;
			}
		}

		public static bool Not(string obj)
		{
			return string.IsNullOrEmpty(obj);
		}

		public static bool Not(long obj)
		{
			return obj == 0;
		}

		public static bool Valid(object obj)
		{
			return !Not(obj);
		}

		public static bool Valid(long value)
		{
			return value != 0;
		}

		public static bool Valid(string value)
		{
			return string.IsNullOrEmpty(value);
		}

		public static string Ref(object data)
		{
			if (data is IList)
				return "ARRAY";
			if (data is IDictionary)
				return "HASH";
			return null;
		}

		public static object Or(this object obj1, object obj2)
		{
			if (Valid(obj1))
				return obj1;
			return obj2;
		}

		public static T Or<T>(this T obj1, T obj2)
		{
			if (Valid(obj1))
				return obj1;
			return obj2;
		}

		public static int Or(this int obj1, int obj2)
		{
			if (obj1 == 0)
				return obj2;
			return obj1;
		}

		public static int Or(params int[] numbers)
		{
			foreach (var n in numbers)
			{
				if (n != 0)
					return n;
			}

			return 0;
		}

		#region Perl

		public static object XmlOut(object data, string rootname = "root")
		{
			if (data is ICollection)
			{
				var xml = new XElement(rootname);
				var collection = (ICollection) data;
				xml.Add(collection.Cast<object>().Select(x => XmlOut(x)));
				return xml;
			}

			if (Type.GetTypeCode(data.GetType()) == TypeCode.Object)
			{
				var xml = new XElement(rootname);
				var type = data.GetType();
				var dict = new Dictionary<string, object>();
				foreach (var prop in type.GetProperties())
					dict[prop.Name] = prop.GetValue(data, null);
				foreach (var field in type.GetFields())
					dict[field.Name] = field.GetValue(data);

				foreach (var pair in dict)
				{
					var key = pair.Key;
					var value = pair.Value;
					if (IsSimple(data))
						xml.SetAttributeValue(key, value);
					else
						xml.Add(XmlOut(data, key));
				}
				return xml;
			}

			return data;
		}

		public static bool IsSimple(object data)
		{
			if (data == null) return true;
			var type = data.GetType();
			return Type.GetTypeCode(type) == TypeCode.Object;
		}

		#endregion

		#region Errors

		public static void Warn(string s)
		{
			Console.Error.WriteLine(s);
		}

		public static void Error(string s)
		{
			Die(s);
		}

		public static void Die(string s)
		{
			Console.Error.WriteLine(s);
			throw new Exception(s);
		}

		public static void Fail(string s)
		{
			Die(s);
		}

		public static void Assert(object b, string s = "")
		{
			if (Not(b))
				Die(s);
		}

		public static string[] QuotedWords(string s)
		{
			var split = Regex.Split(s, @"\s+");
			return Array.FindAll(split, x => x.Length > 0);
		}

		#endregion

		#region X

		[ThreadStatic] [DebuggerBrowsable(DebuggerBrowsableState.Never)] public static Match LastMatch;

		public static string Match(int match)
		{
			return LastMatch.Groups[match].Value;
		}

		public static object Replace(this object obj, string pattern, string replace, RegexOptions options = 0)
		{
			var data = obj as string;
			if (data == null)
				return obj;
			data = Regex.Replace(data, pattern, replace, options);
			return data;
		}

		public static Match Match(this object obj, string pattern, RegexOptions options = 0)
		{
			var data = obj as string;
			var match = data == null ? System.Text.RegularExpressions.Match.Empty : Regex.Match(data, pattern, options);
			LastMatch = match;
			return match;
		}

		public static bool IsMatch(this object obj, string pattern, RegexOptions options = 0)
		{
			return obj.Match(pattern, options).Success;
		}

		public static bool StartsWith(this object obj, string pattern)
		{
			var data = obj as string;
			return data != null && data.StartsWith(pattern);
		}

		public static bool EndsWith(this object obj, string pattern)
		{
			var data = obj as string;
			return data != null && data.EndsWith(pattern);
		}

		#endregion
	}
}