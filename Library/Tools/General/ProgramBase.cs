#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2005, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

#endregion

namespace Softperson
{
	public abstract class ProgramBase
	{
		#region Construction

		protected ProgramBase()
		{
			var arguments = Environment.GetCommandLineArgs();
			args = new List<string>(arguments);
			properties = new List<PropertyInfo>(GetType().GetProperties());

			properties.RemoveAll(delegate(PropertyInfo p)
			{
				if (p.DeclaringType == typeof (ProgramBase))
					return true;

				var get = p.GetGetMethod();
				var set = p.GetSetMethod();
				return get == null || set == null;
			});

			properties.Sort((p1, p2) => string.CompareOrdinal(p1.Name, p2.Name));

			if (GetSwitch("?") || GetSwitch("help"))
			{
				Help();
				Environment.Exit(0);
			}
		}

		#endregion

		#region Variables

		private readonly List<string> args;
		private readonly List<PropertyInfo> properties;

		#endregion

		#region Properties

		public IEnumerable<string> Arguments
		{
			get { return args; }
		}

		public IEnumerable<string> Files
		{
			get { return args.Glob(); }
		}

		#endregion

		#region Methods

		protected abstract void Execute();

		protected virtual void Help()
		{
			Environment.Exit(0);
		}

		protected bool GetSwitch(string name)
		{
			for (var i = 0; i < args.Count; i++)
			{
				var arg = args[i];
				if (arg.Length == 0 || arg[0] != '/' && arg[0] != '-')
					continue;

				arg = arg.Substring(1);
				if (name == null)
					return true;

				if (string.Equals(name, arg, StringComparison.OrdinalIgnoreCase))
				{
					args.Remove(name);
					return true;
				}
			}

			return false;
		}

		protected T GetSwitch<T>(string name, T defaultValue)
		{
			var arg = GetTextSwitch(name);
			if (arg == null)
				return defaultValue;

			var converter = TypeDescriptor.GetConverter(typeof (T));
			try
			{
				var value = (T) converter.ConvertFrom(arg);
				return value;
			}
			catch (Exception e)
			{
				var message = $"The switch '{name}' encountered an invalid argument '{args}'";
				throw new CommandOptionsException(message, e);
			}
		}

		protected string GetTextSwitch(string name)
		{
			for (var i = 0; i < args.Count; i++)
			{
				var arg = args[i];
				if (arg.Length == 0 || arg[0] != '/' && arg[0] != '-')
					continue;

				arg = arg.Substring(1);

				if (!arg.StartsWith(name, StringComparison.OrdinalIgnoreCase))
					continue;

				string p;
				if (arg.Length > name.Length)
				{
					if (arg[name.Length] == '=')
					{
						args.RemoveAt(i);
						p = arg.Substring(name.Length + 1);
						return p;
					}
				}
				else if (i < args.Count - 1)
				{
					p = args[i + 1];
					if (!p.StartsWith("-") && !p.StartsWith("/"))
					{
						args.RemoveRange(i, 2);
						return p;
					}
				}
			}
			return null;
		}

		protected bool AnyMoreSwitches()
		{
			return GetSwitch(null);
		}

		public static bool CheckExclusive(string appname)
		{
			object m = new Mutex(true, appname + "-TrickedOut", out bool exclusive);
			if (!exclusive)
			{
				Environment.Exit(0);
			}
			return true;
		}

		/// <summary>
		///     Returns another running instance of the program
		/// </summary>
		/// <returns></returns>
		public static Process RunningInstance()
		{
			var current = Process.GetCurrentProcess();
			var processes = Process.GetProcessesByName(current.ProcessName);

			//Loop through the running processes in with the same name 

			var location = Assembly.GetExecutingAssembly().Location;
			if (location != null)
				return (from process in processes
					where process.Id != current.Id
					let module = current.MainModule
					where module != null && location.Replace("/", "\\") == module.FileName
					select process).FirstOrDefault();

			//No other instance was found, return null. 
			return null;
		}

		#endregion
	}
}