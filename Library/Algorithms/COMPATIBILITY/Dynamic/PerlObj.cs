#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2010-2011, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Softperson.Collections;
using Softperson.Scripting;

#endregion

namespace Softperson
{
	public struct PerlObj
		: IEquatable<PerlObj>,
			IEnumerable<PerlObj>,
			IComparable<PerlObj>
	{
		#region Variables

		private static readonly Dictionary<string, PerlObj> _hash = new Dictionary<string, PerlObj>();
		private static bool _dontHash;

		private static readonly List<string> info = new List<string>();

		public object Data;

		#endregion

		#region Constructor

		public PerlObj(object data)
		{
			Data = data;
		}

		#endregion

		#region Properties

		public static readonly PerlObj Null = default(PerlObj);

		public static readonly PerlObj EmptyList = new PerlObj(ArrayTools.EmptyArray<PerlObj>());

		public PerlObj this[int index]
		{
			get
			{
				var list = Data as IList<PerlObj>;
				if (list == null)
					return Null;
				return list[index];
			}
			set
			{
				var list = Data as IList<PerlObj>;
				if (list == null)
					throw new InvalidOperationException();
				list[index] = value;
			}
		}

		public PerlObj this[string index]
		{
			get
			{
				var dict = Data as Dictionary<string, PerlObj>;
				if (dict == null)
					return Null;
				return new PerlObj(dict[index]);
			}
			set
			{
				var list = Data as Dictionary<string, PerlObj>;
				if (list == null)
				{
					if (Data != null)
						throw new InvalidOperationException();
					Data = list = new Dictionary<string, PerlObj>();
				}
				list[index] = value;
			}
		}

		#endregion

		#region Conversions

		public static implicit operator PerlObj(string data)
		{
			if (data != null && data.Length == 0)
				data = null;
			return new PerlObj(data);
		}

		public static implicit operator PerlObj(double data)
		{
			return new PerlObj(data);
		}

		public static explicit operator PerlObj(List<PerlObj> list)
		{
			return new PerlObj(list);
		}

		public static explicit operator double(PerlObj obj)
		{
			var data = obj.Data;
			if (data is double)
				return (double) data;

			if (data is string)
			{
				double d;
				if (double.TryParse(data.ToString(), out d))
					return d;
			}

			return 0;
		}

		public static explicit operator string(PerlObj obj)
		{
			return obj.ToString();
		}

		#endregion

		#region Overrides

		public int CompareTo(PerlObj other)
		{
			throw new NotImplementedException();
		}

		public override int GetHashCode()
		{
			return Data == null ? 0 : Data.GetHashCode();
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			ToStringGuts(builder);
			var result = builder.ToString();
			return result;
		}

		private void ToStringGuts(StringBuilder builder)
		{
			var list = Data as List<PerlObj>;
			if (list == null)
			{
				builder.Append(Data ?? "");
				return;
			}

			var indent = 2;
			var oldLength = builder.Length;
			for (var pos = builder.Length - 1; pos > 0; pos--)
			{
				if (builder[pos] == '\n')
					break;
				indent++;
			}

			builder.Append("(");
			var newline = false;

			for (var i = 0; i < list.Count; i++)
			{
				var node = list[i];
				if (i != 0)
				{
					if (newline)
					{
						builder.AppendLine();
						builder.Append(' ', indent);
					}
					else
					{
						builder.Append(' ');
					}
				}

				node.ToStringGuts(builder);
				if (node.IsList)
					newline = true;

				if (!newline && i == 0)
					indent += builder.Length - oldLength - 1;
			}

			builder.Append(")");
		}

		#endregion

		#region Operators

		public static bool operator ==(PerlObj perlObj1, PerlObj perlObj2)
		{
			throw new NotImplementedException();
		}

		public static bool operator !=(PerlObj perlObj1, PerlObj perlObj2)
		{
			throw new NotImplementedException();
		}

		public bool Equals(PerlObj perlObj)
		{
			return perlObj.Data != null && perlObj.Data.Equals(Data);
		}

		public static PerlObj operator |(PerlObj perlObj1, PerlObj perlObj2)
		{
			throw new NotImplementedException();
		}

		public static PerlObj operator ^(PerlObj perlObj1, PerlObj perlObj2)
		{
			throw new NotImplementedException();
		}

		public static bool operator true(PerlObj perlObj)
		{
			return !!perlObj;
		}

		public static bool operator false(PerlObj perlObj)
		{
			return !perlObj;
		}

		public static double operator +(PerlObj obj1, PerlObj obj2)
		{
			return (double) obj1 + (double) obj2;
		}

		public static string operator +(string obj1, PerlObj obj2)
		{
			return obj1 + obj2.ToString();
		}

		public static string operator +(PerlObj obj1, string obj2)
		{
			return obj1.ToString() + obj2;
		}

		public static double operator -(PerlObj obj1, PerlObj obj2)
		{
			return (double) obj1 - (double) obj2;
		}

		public static double operator /(PerlObj obj1, PerlObj obj2)
		{
			return (double) obj1/(double) obj2;
		}

		public static double operator *(PerlObj obj1, PerlObj obj2)
		{
			return (double) obj1*(double) obj2;
		}

		public static double operator %(PerlObj obj1, PerlObj obj2)
		{
			return (double) obj1%(double) obj2;
		}

		public static bool operator !(PerlObj obj)
		{
			var data = obj.Data;
			if (data == null)
				return true;

			if (data is double)
				return (double) data == 0;

			var d = data as string;
			if (d != null)
			{
				return d.Length == 0
					   || d.Length == 1 && d[0] == 0;
			}

			return false;
		}

		public static PerlObj operator ++(PerlObj obj)
		{
			return obj + 1;
		}

		public static PerlObj operator --(PerlObj obj)
		{
			return obj - 1;
		}

		public static string Ref()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Enumeration

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		public IEnumerator<PerlObj> GetEnumerator()
		{
			var count = Count;
			for (var i = 0; i < count; i++)
				yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof (PerlObj)) return false;
			return Equals((PerlObj) obj);
		}

		#region Helper Methods

		private static string ReadToken()
		{
			string result;
			do
			{
				while (info.Count == 0)
				{
					var line = Console.In.ReadLine();
					if (line == null)
						return null;
					info.AddRange(Regex.Matches(line, @"[()]|;.*|""[^""]*""|[^\s()"";]+").Cast<Match>().Select(m => m.Value));
				}

				result = info.PopFront();
			} while (Regex.IsMatch(result, @"^[;,]"));
			return result;
		}

		private static string PeekToken()
		{
			var result = ReadToken();
			if (result == null)
				return null;
			info.Insert(0, result);
			return result;
		}

		public static void DontHash()
		{
			_dontHash = true;
		}

		public bool IsList
		{
			get { return Data is IList<PerlObj>; }
		}

		public static PerlObj SearchList(PerlObj list, PerlObj item)
		{
			if (!list.IsList)
				return null;

			var itemList = item.Data as List<PerlObj>;
			foreach (var line in list)
			{
				if (itemList != null)
				{
					// TODO: Check that this equals itemList[line]
					foreach (var i in itemList)
					{
						if (i.Count > 1 && i[0] == line)
							return list;
					}
				}
				else
				{
					if (line == item)
						return list;
				}

				var lineRef = line.Data as List<PerlObj>;
				if (lineRef != null)
				{
					var result = Search(line, item);
					if (result)
						return result;
				}
			}

			return null;
		}

		public static PerlObj Search(PerlObj list, params PerlObj[] args)
		{
			foreach (var line in args)
			{
				list = SearchList(list, line);
				if (list)
					break;
			}
			return list;
		}

		public static PerlObj ReadList()
		{
			var list = new List<PerlObj>();
			while (true)
			{
				PerlObj element;
				var ch = ReadToken();
				if (ch == null)
					break;

				if (ch == "(")
					element = ReadList();
				else if (ch == ")")
					break;
				else
					element = ch;

				if (!"".Equals(element))
					list.Add(element);
			}

			var result = new PerlObj(list);
			if (list.Count() > 20 || _dontHash)
				return result;

			var join = string.Join("|", list);
			if (_hash.ContainsKey(join))
				return _hash[join];

			_hash[join] = result;
			return result;
		}

		public static IEnumerable<PerlObj> ReadElements(string file)
		{
			var oldReader = Console.In;
			TextReader newReader = null;

			if (file != null)
			{
				newReader = File.OpenText(file);
				Console.SetIn(newReader);
			}

			if (ReadToken() == "(")
			{
				if (PeekToken() != "(")
					info.PushFront("(");

				while (true)
				{
					var token = ReadToken();
					if (token == null || token == ")")
						break;

					var element = token == "("
						? ReadList()
						: token;
					if (element)
						yield return element;
				}
			}

			if (newReader != null)
			{
				Console.SetIn(oldReader);
				newReader.Close();
			}
		}

		public static void ClearHash()
		{
			_hash.Clear();
		}


		public PerlObj Recurse(Func<PerlObj, PerlObj> code, bool all = false)
		{
			var list = this;

			// TODO: End in-place modification
			if (IsList)
			{
				var clean = false;
				var count = Count;
				for (var index = 0; index < count; index++)
				{
					var e = this[index];
					var result = e.Recurse(code, all);
					if (!result)
						clean = true;
					this[index] = result;
				}

				if (clean)
				{
					var newList = new List<PerlObj>();
					foreach (var e in this)
					{
						if (!e)
							continue;
						newList.Add(e);
					}

					list = new PerlObj(list);
				}
			}
			else
			{
				if (!all)
					return list;
			}

			return code(list);
		}

		public void Recurse(Action<PerlObj> code, bool all = false)
		{
			Recurse(x =>
			{
				code(x);
				return x;
			}, all);
		}

		public static void ReadEnums(Dictionary<string, int> enumHash, string file)
		{
			file = file ?? "/dict/obj/lexids.cs/";
			var lexids = File.OpenText(file);
			foreach (var line in lexids.ReadLines())
			{
				var m = Regex.Match(line, @"(\w+)\s*=\s*(\d+)");
				if (!m.Success)
					continue;
				var name = m.Groups[1].Value;
				var id = Convert.ToInt32(m.Groups[2].Value);
				enumHash[name] = id;
			}
			lexids.Close();
		}

		public static void RedirectInput(ref string[] argv)
		{
			if (argv.Length == 0)
				return;

			Console.SetIn(File.OpenText(argv[0]));
			argv = argv.CopyRange(1, argv.Length - 1);
		}

		public static PerlObj GetList(string file = null)
		{
			if (file != null)
				Console.SetIn(File.OpenText(file));

			var list = ReadList();
			if (list.Count == 1 && list[0].IsList)
				list = list[0];
			return list;
		}

		public static PerlObj Merge(PerlObj dest, PerlObj src)
		{
			if (!dest)
				return src;
			if (!src)
				return dest;

			var changed = false;
			var result = dest.ToList();

			Debug.Assert(dest[0] == src[0], "Heads are not equal");

			for (var i = 1; i < src.Count; i++)
			{
				var elem = src[i];

				// Test containment
				var good = result.Any(line => line == elem);
				if (good)
					continue;

				if (elem.IsList)
				{
					var head = elem[0];
					for (var j = 0; j < result.Count; j++)
					{
						var elem2 = result[j];
						if (elem2.IsList && head == elem2[0])
						{
							var merge = Merge(elem2, elem);
							if (merge != result[j])
								changed = true;
							result[j] = merge;
							good = true;
							break;
						}
					}
					if (good)
						continue;
				}

				result.Add(elem);
				changed = true;
			}

			if (changed)
				return new PerlObj(result);
			return dest;
		}

		public int CompareList(PerlObj perlObj1, PerlObj perlObj2)
		{
			var list1 = perlObj1.Data as List<PerlObj>;
			var list2 = perlObj2.Data as List<PerlObj>;

			if (list1 == null)
				return list2 != null
					? -1
					: perlObj1.ToString().CompareTo(perlObj2.ToString());
			if (list2 == null)
				return 1;

			var min = Math.Min(list1.Count, list2.Count);
			for (var i = 0; i < min; i++)
			{
				var cmp = CompareList(list1[i], list2[i]);
				if (cmp != 0)
					return cmp;
			}

			return list1.Count - list2.Count;
		}

		public bool EqualsList(PerlObj perlObj1, PerlObj perlObj2)
		{
			if (Equals(perlObj1, perlObj2))
				return true;

			var list1 = perlObj1.Data as List<PerlObj>;
			var list2 = perlObj2.Data as List<PerlObj>;
			if (list1 == null || list2 == null || list1.Count != list2.Count)
				return false;

			var min = list1.Count;
			for (var i = 0; i < min; i++)
			{
				if (!EqualsList(list1[i], list2[i]))
					return false;
			}

			return true;
		}

		public static void ProcessList(PerlObj perlObj, PerlObj item, Action<IEnumerable<PerlObj>> process)
		{
			var list = perlObj.Data as List<PerlObj>;
			if (list == null || list.Count < 2)
				return;

			if (list[0] != item)
			{
				foreach (var child in list)
				{
					ProcessList(child, item, process);
				}

				return;
			}

			process(list.Skip(1));
		}

		public static PerlObj RemoveList(PerlObj perlObj, PerlObj item)
		{
			return perlObj.Recurse(delegate(PerlObj line)
			{
				if (line[0].Equals(item))
					return null;
				return line;
			});
		}

		public static void ExplodeArray(PerlObj perlObj, string brace)
		{
			var list = perlObj.Data as List<PerlObj>;
			if (list == null)
				return;

			brace = brace ?? "[";
			for (var index = 0; index < list.Count; index++)
			{
				var item = list[index];
				if (item.IsList)
				{
					ExplodeArray(item, brace);
					continue;
				}

				string head;
				string args;

				Regex.Match((string) item, @"^(.+)\[(.+)\]$").Out(out head).Out(out args);
				if (head == null || args == null)
					continue;
				list[index] = List
					(
						brace,
						head,
						args.Split(',')
					);
			}
		}

		public static PerlObj List(params object[] list)
		{
			throw new NotImplementedException();
		}

		public static void RestoreArray(PerlObj perlObj, string brace = "[")
		{
			var list = perlObj.Data as List<PerlObj>;
			if (list == null)
				return;

			for (var index = 0; index < list.Count; index++)
			{
				var item = list[index];
				var itemList = item.Data as IList<PerlObj>;
				if (itemList == null)
					continue;

				if (!brace.Equals(itemList[0]))
				{
					RestoreArray(item, brace);
					continue;
				}

				var join = string.Join(",", itemList.Skip(2));
				list[index] = itemList[1] + "[" + join + "]";
			}
		}

		public bool IsRef()
		{
			return Data is IList<PerlObj>;
		}

		[ThreadStatic] public static Match LastMatch;

		public static string Match(int match)
		{
			return LastMatch.Groups[match].Value;
		}

		public PerlObj Replace(string pattern, string replace, RegexOptions options = 0)
		{
			var data = Data as string;

			if (data == null)
				return this;

			data = Regex.Replace(data, pattern, replace, options);
			return data;
		}

		public Match Match(string pattern, RegexOptions options = 0)
		{
			var data = Data as string;

			var match = data == null
				? System.Text.RegularExpressions.Match.Empty
				: Regex.Match(data, pattern, options);

			LastMatch = match;
			return match;
		}

		public bool IsMatch(string pattern, RegexOptions options = 0)
		{
			return Match(pattern, options).Success;
		}

		public bool StartsWith(string pattern)
		{
			var data = Data as string;
			return data != null && data.StartsWith(pattern);
		}

		public bool EndsWith(string pattern)
		{
			var data = Data as string;
			return data != null && data.EndsWith(pattern);
		}

		#endregion

		public PerlObj ConvertElements(Func<PerlObj, PerlObj> func)
		{
			// Converts elements
			// null - removes elements
			throw new NotImplementedException();
		}

		public PerlObj RemoveElements(Func<PerlObj, bool> pred)
		{
			return ConvertElements(elem => pred(elem) ? null : elem);
		}

		public PerlObj Replace(int position, int count, IEnumerable<PerlObj> insertData)
		{
			throw new NotImplementedException();
		}

		public PerlObj SortList(Comparison<PerlObj> sortFunc = null)
		{
			throw new NotImplementedException();
		}
	}
}