#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Softperson.Collections;

#endregion

namespace Softperson
{
	public static class VSTools
	{
		#region VS Projects

		public static string SearchForProject(string path, string project = "*.csproj")
		{
			if (!Directory.Exists(path))
				path = Path.GetDirectoryName(path);

			var currentPath = path;
			for (var i = 1;
				i < 100 && !string.IsNullOrWhiteSpace(currentPath);
				i++)
			{
				var found = Directory.GetFiles(currentPath, project);
				if (found.Length != 0)
					return found[0];
				currentPath = Path.GetDirectoryName(currentPath);
			}

			return null;
		}

		public static string SearchForSolution(string path)
		{
			return SearchForProject(path, "*.sln");
		}

		public static List<string> GetSourceFiles(IEnumerable<string> files)
		{
			var list = new List<string>();
			foreach (var file in files)
			{
				if (file.EndsWith(".sln"))
				{
					foreach (var project in GetProjectsFromSolution(file))
						list.AddRange(GetFilesFromProject(project));
				}
				else if (file.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase)
						 || file.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase))
				{
					list.AddRange(GetFilesFromProject(file));
				}
				else
					list.Add(file);
			}
			return list;
		}

		public static IEnumerable<string> GetProjectsFromSolution(string solutionFile)
		{
			var projectFiles = new List<string>();

			var regex =
				new Regex(
					@"Project \s* \( [^\)]* \) \s* = 
										\s* "" ([^""]*) "" \s*, 
										\s* "" ([^""]*) "" \s*,
										\s* "" ([^""]*) """,
					RegexOptions.Compiled
					| RegexOptions.IgnorePatternWhitespace);

			var directory = Path.GetDirectoryName(solutionFile);

			using (TextReader reader = new StreamReader(solutionFile))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					var match = regex.Match(line);
					if (!match.Success)
						continue;

					var col = match.Groups;
					var projectName = col[1].Value;
					var projectFile = col[2].Value;
					var projectType = col[3].Value;
					projectFiles.Add(Path.Combine(directory, projectFile));
				}
			}
			return projectFiles;
		}

		public static IEnumerable<string> GetFilesFromProject(string project)
		{
			if (!project.EndsWith(".csproj",
				StringComparison.OrdinalIgnoreCase))
				return ListTools.Empty<string>();

			var directory = Path.GetDirectoryName(project);
			if (directory == null)
				return ListTools.Empty<string>();

			var doc = XElement.Load(project);

			// /Project/PropertyGroup/Configuration
			// /Project/PropertyGroup/AssemblyName -- NStatic
			// /Project/PropertyGroup/OutputType -- WinExe
			// /Project/PropertyGroup/StartupObject -- Softperson.Program
			// /Project/PropertyGroup/DefineConstants -- NStatic
			// /Project/PropertyGroup/CheckForOverflowUnderflow -- true
			// /Project/ItemGroup/Reference/@Include -- NStatic

			// VS 2005
			XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
			var sources = doc
				.Elements(ns + "ItemGroup")
				.Elements(ns + "Compile")
				.Attributes("Include")
				.Select(attr => attr.Value)
				.Select(file => Path.Combine(directory, file))
				.ToList();

			// VS 2002 and 2003
			foreach (var element in doc.XPathSelectElements("CSHARP/Files/Include/File"))
			{
				var buildAction = (string) element.Attribute("BuildAction");
				if (buildAction != "Compile") continue;
				var relPath = (string) element.Attribute("RelPath");
				if (relPath == null) continue;
				sources.Add(Path.Combine(directory, relPath));
			}

			return sources.Distinct();
		}

		public static string GetRootNamespace(string project)
		{
			var doc = XElement.Load(project);
			var ns = (XNamespace) "http://schemas.microsoft.com/developer/msbuild/2003";
			var node = doc.Elements(ns + "PropertyGroup")
				.Elements(ns + "RootNamespace").FirstOrDefault();

			return node == null ? null : (string) node;
		}

		public static string GetResourceName(string projectDirectory, string ns, string file)
		{
			var resource = FileTools.MakeFilePathRelative(projectDirectory, file);
			resource = resource.Replace('\\', '.');
			if (!string.IsNullOrEmpty(ns))
				resource = resource + "." + resource;
			return resource;
		}

		public static string GetClassName(string fullClassName)
		{
			var className = fullClassName.AfterLast('.',
				fullClassName);
			return className;
		}

		public static string GetNamespace(string fullClassName)
		{
			var namespaceName = fullClassName.BeforeLast('.', "");
			return namespaceName;
		}

		#endregion
	}
}