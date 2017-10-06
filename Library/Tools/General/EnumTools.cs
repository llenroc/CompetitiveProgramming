#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Softperson
{
	public static class EnumTools
	{
		static readonly Dictionary<Type, Dictionary<int, string>> NameMap
			= new Dictionary<Type, Dictionary<int, string>>();

		public static string GetConstantName(Type type, int id)
		{
			if (type == null || type == typeof (object)) // || !type.IsClass)
				return null;

			Dictionary<int, string> d;

			if (!NameMap.TryGetValue(type, out d))
			{
				d = new Dictionary<int, string>();

				foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
				{
					if (!field.IsLiteral)
						continue;
					var value = field.GetValue(null);
					var v = int.MaxValue;

					if (value is int)
						v = (int) value;
					else if (value is byte)
						v = (byte) value;

					if (v != int.MaxValue)
					{
						var s = field.Name;
						if (!s.EndsWith("Base"))
							d[v] = s;
					}
				}

				NameMap[type] = d.Count == 0 ? null : d;
			}

			string result;
			if (d == null || !d.TryGetValue(id, out result))
				return GetConstantName(type.BaseType, id);

			return result;
		}
	}
}