#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Softperson
{
	public static class TypeTools
	{
		public static IEnumerable<Type> FindType(string typeName, bool ignoreCase = false)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			return assemblies
				.Select(a => a.GetType(typeName, false, ignoreCase))
				.Where(t => t != null && t.IsPublic);
		}

		public static IEnumerable<Type> FindTypeIgnoringNamespace(string typeName, bool ignoreCase = false)
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var comparison = ignoreCase
				? StringComparison.OrdinalIgnoreCase
				: StringComparison.Ordinal;

			return from a in assemblies
				from t in a.GetTypes()
				where t.IsPublic && string.Equals(t.Name, typeName, comparison)
				select t;
		}

		public static object[] ConvertArguments(string[] args, ParameterInfo[] plist)
		{
			var result = new object[plist.Length];

			for (var i = 0; i < result.Length; i++)
			{
				var type = plist[i].ParameterType;

				if (i >= args.Length)
				{
					result[i] = plist[i].IsOptional
						? plist[i].DefaultValue
						: (type.IsArray
							? Array.CreateInstance(type.GetElementType(), 0)
							: Activator.CreateInstance(plist[i].ParameterType));
					continue;
				}

				if (i == plist.Length - 1 && type.IsArray)
				{
					var elementType = type.GetElementType();
					var newArray = args
						.Skip(i)
						.Select(e => Convert.ChangeType(e, elementType))
						.ToArray();

					var convertedArray = Array.CreateInstance(elementType, newArray.Length);
					newArray.CopyTo(convertedArray, 0);
					result[i] = convertedArray;
					break;
				}

				result[i] = Convert.ChangeType(args[i], type);
			}

			return result;
		}
	}
}