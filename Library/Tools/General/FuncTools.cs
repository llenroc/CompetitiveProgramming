#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Softperson.Collections;
using Softperson.Msil;

#endregion

namespace Softperson
{

	#region Delegates

	public delegate void TransformFunc<A1, A2>(ref A1 a1, ref A2 a2);

	public delegate V GetMethod<out V>(object obj);

	public delegate void SetMethod<in V>(object obj, V value);

	public delegate object GenericDelegate(params object[] args);

	#endregion

	public static class FuncTools
	{
		private const BindingFlags AllInstance =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public static readonly Func<object, object, bool> IsOlder = IsOlderFunc();
		public static readonly Action EmptyAction = DoNothing;

		public static Func<T, T> NullFunc<T>()
			where T : class
		{
			return null;
		}

		public static void Raise(this EventHandler handler,
			object sender, EventArgs arg = null)
		{
            handler?.Invoke(sender, arg ?? EventArgs.Empty);
        }

		public static void Raise<T>(this EventHandler<T> handler, object sender, T arg)
			where T : EventArgs
		{
            handler?.Invoke(sender, arg);
        }

		public static void For(int from, int to, Action<int, Action<int>> code)
		{
			Action<int> continuance = null;
			continuance = delegate(int i)
			{
				i++;
				if (i <= to) code(i, continuance);
			};
			continuance(@from);
		}

		public static T For<T>(int from, int to, Func<int, Func<int, T>, T> code)
		{
			Func<int, T> continuance = null;
			continuance = delegate(int i)
			{
				i++;
				if (i <= to) return code(i, continuance);
				return default(T);
			};
			return continuance(@from);
		}

		public static Func<object, object, bool> IsOlderFunc()
		{
#if !MONO
			var dm = new DynamicMethod("IsOlder", typeof (bool),
				new[] {typeof (object), typeof (object)});

			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Cgt_Un);
			il.Emit(OpCodes.Ret);
			return (Func<object, object, bool>)
				dm.CreateDelegate(typeof (Func<object, object, bool>));
#else
			throw new NotImplementedException();
#endif
		}

#if !MONO
		public static Func<T, V> FieldGetter<T, V>(string name)
		{
			var type = typeof (T);
			var dm = new DynamicMethod(name, typeof (V),
				new[] {type},
				type);

			var fieldInfo = type.GetField(name, AllInstance);
			if (fieldInfo == null)
				return null;
			var il = dm.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, fieldInfo);
			il.Emit(OpCodes.Ret);
			return CreateDelegate<Func<T, V>>(dm);
		}
#endif

		public static Func<T, V> PropertyGetter<T, V>(string name)
		{
			var type = typeof (T);
			var getname = "get_" + name;
			var method = type.GetMethod(getname, AllInstance,
				null, Type.EmptyTypes, null);
			return CreateDelegate<Func<T, V>>(method);
		}

		public static Action<T, V> PropertySetter<T, V>(string name)
		{
			var type = typeof (T);
			var setname = "set_" + name;
			var method = type.GetMethod(setname, AllInstance,
				null, new[] {typeof (V)}, null);
			return CreateDelegate<Action<T, V>>(method);
		}

#if !MONO
		public static T CreateDelegate<T>(DynamicMethod m)
		{
			return (T) (object) m.CreateDelegate(typeof (T));
		}
#endif

		public static T CreateDelegate<T>(MethodInfo method)
		{
			return (T) (object) Delegate.CreateDelegate(typeof (T), method);
		}

		public static Func<A, T> MakeStatic<A, T>(Func<T> func)
		{
			var target = func.Target;
			if (target != null)
				return CreateDelegate<Func<A, T>>(func.Method);
			var tmp = func();
			return r => tmp;
		}

		public static GenericDelegate CreateGenericDelegate(MethodInfo method)
		{
#if !MONO
			var dm = new DynamicMethod(method.Name, typeof (object),
				new[] {typeof (object[])}, method.DeclaringType, true);

			if (method.IsStatic)
				throw new InvalidOperationException();

			if (method.ReturnType == typeof (void))
				throw new InvalidOperationException();

			var il = dm.GetILGenerator();
			var label = il.DefineLabel();
			//           LocalBuilder returnLocal = il.DeclareLocal(typeof(object));

			var parameters = method.GetParameters();
			var length = parameters.Length;

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldlen);
			il.Emit(OpCodes.Conv_I4);
			il.Emit(OpCodes.Ldc_I4, length + 1);
			il.Emit(OpCodes.Beq_S, label);
			il.Emit(OpCodes.Ldstr, "argument count doesn't match");
			il.Emit(OpCodes.Newobj, typeof (ArgumentException).GetConstructor(new[] {typeof (string)}));
			il.Emit(OpCodes.Throw);
			il.MarkLabel(label);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Ldelem_Ref);
			il.Emit(OpCodes.Castclass, method.DeclaringType);

			for (var i = 0; i < length; i++)
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldc_I4, i + 1);
				il.Emit(OpCodes.Ldelem_Ref);

				var param = parameters[i];

				if (param.IsOut)
					throw new InvalidOperationException();

				var paramType = param.ParameterType;
				il.Emit(paramType.IsValueType
					? OpCodes.Unbox
					: OpCodes.Castclass, paramType);
			}

			il.Emit(OpCodes.Call, method);
//            il.Emit(OpCodes.Stloc, returnLocal);
//            il.Emit(OpCodes.Ldloc, returnLocal);
			il.Emit(OpCodes.Ret);
			return CreateDelegate<GenericDelegate>(dm);
#else
			throw new NotSupportedException();

#endif
		}

		public static Func<A, T> Memorize<A, T>(this Func<A, T> func)
		{
			return Memorize(new Dictionary<A, T>(), func);
		}

		public static Func<A, T> Memorize<A, T>(Dictionary<A, T> dictionary, Func<A, T> func)
		{
			var dict = dictionary;
			return delegate(A arg)
			{
                if (dict.TryGetValue(arg, out T result))
                    return result;
                result = func(arg);
				dict.Add(arg, result);
				return result;
			};
		}

		public static Func<A0, A1, T> Memorize<A0, A1, T>(Func<A0, A1, T> func)
		{
			return Unroll(Memorize(Roll(func)));
		}

		public static Func<A0, A1, A2, T> Memorize<A0, A1, A2, T>(Func<A0, A1, A2, T> func)
		{
			return Unroll(Memorize(Roll(func)));
		}

		public static Func<Pair<A0, A1>, T> Roll<A0, A1, T>(Func<A0, A1, T> func)
		{
			return tuple => func(tuple.First, tuple.Second);
		}

		public static Func<Triple<A0, A1, A2>, T> Roll<A0, A1, A2, T>(Func<A0, A1, A2, T> func)
		{
			return tuple => func(tuple.First, tuple.Second, tuple.Third);
		}

		public static Func<A0, A1, T> Unroll<A0, A1, T>(Func<Pair<A0, A1>, T> func)
		{
			return (a0, a1) => func(new Pair<A0, A1>(a0, a1));
		}

		public static Func<A0, A1, A2, T> Unroll<A0, A1, A2, T>(Func<Triple<A0, A1, A2>, T> func)
		{
			return (a0, a1, a2) => func(new Triple<A0, A1, A2>(a0, a1, a2));
		}

		public static void FireAndForget(this Action action)
		{
			action.BeginInvoke(action.EndInvoke, null);
		}

		public static T Fire<T>(this Func<T> action)
		{
			return action();
		}

		public static T ConvertDelegate<T>(this Delegate function)
		{
			if (function == null || function is T)
				return (T) (object) function;
			return (T) (object) Delegate.CreateDelegate(typeof (T), function.Target, function.Method);
		}

		private static void DoNothing()
		{
		}

		public static Func<T, T> Identity<T>()
		{
			return e => e;
		}

		public static Func<X, T> Fix<X, T>(Func<Func<X, T>, Func<X, T>> func)
		{
			Func<X, T> fix = null;
// ReSharper disable PossibleNullReferenceException
			fix = func(x => fix(x));
// ReSharper restore PossibleNullReferenceException
			return fix;
		}

		[Obsolete("Other fix is 12% faster")]
		public static Func<X, T> Fix<X, T>(Func<Func<X, T>, X, T> func)
		{
			Func<X, T> fix = null;
			fix = x => func(fix, x);
			return fix;
		}

		public static Func<A, T> YCombinator<A, T>(Func<Func<A, T>, Func<A, T>> f)
		{
			// return f(x(x)); -- only works with call by name
			YFunc<A, T> lambda = x => f(arg => x(x)(arg));
			return lambda(lambda);
		}

		public static IEnumerable<MemberInfo> ListDependents(MethodBase method)
		{
#if !MONO
			foreach (var inst in MsilReader.GetIl(method))
			{
                if (inst is MsilMemberInstruction m)
                {
                    var member = m.Member;
                    if (member != null)
                        yield return member;
                }
            }
#else
			yield break;
#endif
		}

		public static HashSet<MemberInfo> ListAllCallers(params MemberInfo[] memberList)
		{
			var work = new HashSet<MemberInfo>(memberList);
			var stack = new Stack<MemberInfo>();
			var members = new HashSet<MemberInfo>();

			while (true)
			{
				foreach (var w in work)
				{
					if (!members.Contains(w))
					{
// ReSharper disable ConstantNullCoalescingCondition
						var type = w.DeclaringType ?? w as Type;
						Debug.Assert(w.DeclaringType == w.ReflectedType);
// ReSharper restore ConstantNullCoalescingCondition

						var ns = type.FullName;
						if (ns != null && ns.StartsWith("System"))
							continue;

						members.Add(w);
						stack.Push(w);
					}
				}

				work.Clear();

				if (stack.Count == 0)
					break;

				var pop = stack.Pop();

				switch (pop.MemberType)
				{
					case MemberTypes.Event:
						var ei = (EventInfo) pop;
						work.Add(ei.GetAddMethod());
						work.Add(ei.GetRemoveMethod());
						break;
					case MemberTypes.Property:
						var pi = (PropertyInfo) pop;
						work.Add(pi.GetGetMethod());
						work.Add(pi.GetSetMethod());
						break;
					case MemberTypes.Field:
						break;
					case MemberTypes.Constructor:
					case MemberTypes.Method:
						work.UnionWith(ListDependents((MethodBase) pop));
						break;
					case MemberTypes.TypeInfo:
					case MemberTypes.NestedType:
						var t = (Type) pop;
						work.UnionWith(t.GetMethods(
							BindingFlags.Public
							| BindingFlags.NonPublic
							| BindingFlags.Instance
							| BindingFlags.DeclaredOnly));

						var sc = t.GetConstructor(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
							null, ArrayTools.EmptyArray<Type>(), null);

						if (sc != null)
							work.Add(sc);

						var b = t.BaseType;
						if (b != null)
							work.Add(b);
						break;
				}

				members.Add(pop);
			}

			return members;
		}

		#region Nested type: YFunc

		private delegate Func<A, T> YFunc<A, T>(YFunc<A, T> f);

		#endregion
	}
}