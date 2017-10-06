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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace Softperson
{
	public static class FileTools
	{
		#region Local Files

		public static string GetApplicationFile(string fileName)
		{
			var module = Process.GetCurrentProcess().MainModule;
			if (module == null)
				return null;
			var path = module.FileName;
			//string path = System.Windows.Forms.Application.ExecutablePath;
			var dir = Path.GetDirectoryName(path);
			var file = Path.Combine(dir, "nstatic.txt");
			//string rulesfile = options.GetSwitch("f", rulesfile1);
			return file;
		}

		#endregion

		/// <summary>
		/// Pathes the difference.
		/// </summary>
		/// <param name="path1">The path1.</param>
		/// <param name="path2">The path2.</param>
		/// <returns></returns>
		public static string PathDifference(string path1, string path2)
		{
			var lastSep = -1;
			int lastSame;

			for (lastSame = 0; lastSame < path1.Length && lastSame < path2.Length; lastSame++)
			{
				if (char.ToLower(path1[lastSame], CultureInfo.InvariantCulture)
					!= char.ToLower(path2[lastSame], CultureInfo.InvariantCulture))
					break;

				if (path1[lastSame] == Path.DirectorySeparatorChar)
					lastSep = lastSame;
			}

			if (lastSame == 0)
				return path2;

			if (lastSame == path1.Length && lastSame == path2.Length)
				return string.Empty;

			var builder = new StringBuilder();
			while (lastSame < path1.Length)
			{
				if (path1[lastSame] == Path.DirectorySeparatorChar)
					builder.Append(".." + Path.DirectorySeparatorChar);
				lastSame++;
			}
			return builder + path2.Substring(lastSep + 1);
		}

		public static string MakeFilePathRelative(string basePath, string fileName)
		{
			if (!string.IsNullOrEmpty(basePath))
			{
				if (!basePath.EndsWith("\\"))
					basePath += "\\";
				return PathDifference(basePath, fileName);
			}
			return fileName;
		}

		public static string GetFileName(this TextReader reader)
		{
			var sr = reader as StreamReader;
			string name = null;
			if (sr != null)
			{
				var fs = sr.BaseStream as FileStream;
				if (fs != null)
					name = fs.Name;
			}
			return name;
		}

		public static string GetFileName(this TextWriter reader)
		{
			var sr = reader as StreamWriter;
			string name = null;
			if (sr != null)
			{
				var fs = sr.BaseStream as FileStream;
				if (fs != null)
					name = fs.Name;
			}
			return name;
		}


		public static string PathFromUri(string s)
		{
			if (s == null
				|| !s.StartsWith("file:",
					StringComparison.OrdinalIgnoreCase))
				return s;

			var sb = new StringBuilder(s);
			sb.Remove(0, 5);

			var write = 0;
			var colon = false;
			for (var i = 0; i < sb.Length; i++)
			{
				var ch = sb[i];
				if (ch == '%'
					&& i + 2 < sb.Length
					&& StringTools.HexDigit(sb[i + 1]) >= 0
					&& StringTools.HexDigit(sb[i + 2]) >= 0)
				{
					ch = (char) (
						StringTools.HexDigit(sb[i + 1])*16
						+ StringTools.HexDigit(sb[i + 2]));
					i += 2;
				}
				else if (ch == '+')
					ch = ' ';
				else if (ch == '/')
					ch = '\\';
				else if (ch == ':')
					colon = true;
				sb[write++] = ch;
			}

			sb.Length = write;
			if (sb.Length >= 2 && sb[0] == '\\' && sb[1] == '\\')
			{
				if (sb.Length >= 3 && sb[2] == '\\')
					sb.Remove(0, 3);
				else if (colon)
					sb.Remove(0, 2);
			}

			return sb.ToString();
		}

		public static void Pack(this Stream s, double value)
		{
			s.Pack(BitConverter.DoubleToInt64Bits(value));
		}

		public static void Pack(this Stream s, float value)
		{
			s.Pack(BitConverter.GetBytes(value));
		}

		public static void Pack(this Stream s, byte value)
		{
			s.Pack(value, 1);
		}

		public static void Pack(this Stream s, sbyte value)
		{
			s.Pack(value, 1);
		}

		public static void Pack(this Stream s, short value)
		{
			s.Pack(value, 2);
		}

		public static void Pack(this Stream s, ushort value)
		{
			s.Pack(value, 2);
		}

		public static void Pack(this Stream s, int value)
		{
			s.Pack(value, 4);
		}

		public static void Pack(this Stream s, uint value)
		{
			s.Pack(value, 4);
		}

		public static void Pack(this Stream s, byte[] value)
		{
			s.Write(value, 0, value.Length);
		}

		public static void Pack(this Stream s, long value, int bytes = 8)
		{
			while (bytes-- > 0)
			{
				s.WriteByte((byte) (value & 0xff));
				value = value >> 8;
			}
			Debug.Assert(value == 0 || value == -1, "Database value truncated");
		}

		public static short ReadShort(this Stream s)
		{
			var value = s.ReadByte();
			value += s.ReadByte() << 8;
			return (short) value;
		}

		public static int ReadInt(this Stream s)
		{
			var value = s.ReadByte();
			value += s.ReadByte() << 8;
			value += s.ReadByte() << 16;
			value += s.ReadByte() << 24;
			return value;
		}

		public static unsafe float ReadFloat(this Stream s)
		{
			var value = s.ReadInt();
			return *(float*) &value;
		}

		public static long ReadLong(this Stream s, int bytes = 8)
		{
			Debug.Assert(bytes > 0 && bytes <= 8);
			long value = 0;
			var shift = 0;
			while (bytes-- > 0)
			{
				value += (long) s.ReadByte() << shift;
				shift += 8;
			}
			return value;
		}

		public static double ReadDouble(this Stream s)
		{
			return BitConverter.Int64BitsToDouble(s.ReadLong());
		}

		public static long GetFileSize(string file)
		{
			var f = new FileInfo(file);
			return f.Length;
		}

		public static bool Delete(string file)
		{
			try
			{
				if (Directory.Exists(file))
					Directory.Delete(file, true);
				else if (File.Exists(file))
					File.Delete(file);
				return true;
			}
			catch (IOException)
			{
				return false;
			}
		}

		#region File IO

		public static IEnumerable<string> ReadLines(string file, Encoding encoding = null)
		{
			return ReadLines(new StreamReader(file, encoding ?? Encoding.UTF8));
		}

		public static IEnumerable<string> ReadLines(this TextReader reader, bool close = true)
		{
			try
			{
				while (true)
				{
					var line = reader.ReadLine();
					if (line == null)
						break;
					yield return line;
				}
			}
			finally
			{
				if (close)
					reader.Close();
			}
		}

		public static IEnumerable<string> ReadLines(params string[] files)
		{
			return ReadLines((IEnumerable<string>) files);
		}

		public static IEnumerable<string> ReadLines(IEnumerable<string> files)
		{
			return files.Glob().Select(file => new StreamReader(file, Encoding.UTF8)).SelectMany(reader => ReadLines(reader));
		}

		public static IEnumerable<string> ReadLinesOrStdin(IEnumerable<string> files)
		{
			var enumerable = files as string[] ?? files.ToArray();
			return enumerable.Length == 0 ? ReadLines(Console.In) : ReadLines(enumerable);
		}

		public static GenericTextWriter NewTextWriter(Action<string> action)
		{
			return new GenericTextWriter(action);
		}

		#endregion

		#region File/Directory Scanning

		public static bool MatchWildcards(string pattern, string filename)
		{
			return MatchWildcards(pattern, filename, 0, 0);
		}

		private static bool MatchWildcards(string pattern, string filename, int indexPattern, int indexFilename)
		{
			var patternLength = pattern.Length;
			var fileLength = filename.Length;

			while (true)
			{
				if (indexPattern >= patternLength)
					return indexFilename >= fileLength;

				var ch = char.ToLowerInvariant(pattern[indexPattern++]);
				if (indexFilename >= fileLength)
				{
					if (ch != '*')
						return false;
					continue;
				}

				if (ch == '*')
				{
					if (MatchWildcards(pattern, filename, indexPattern, indexFilename))
						return true;
					indexPattern--;
				}
				else
				{
					if (indexFilename >= fileLength
						|| ch != '?'
						&& char.ToLowerInvariant(filename[indexFilename]) != ch)
						return false;
				}
				indexFilename++;
			}
		}

		public static IEnumerable<string> Glob(this IEnumerable<string> paths)
		{
			return paths.SelectMany(Glob);
		}

		public static IEnumerable<string> Glob(this string pathOriginal)
		{
			var path = pathOriginal;
			var pattern = "";
			var subpath = "";

			var index = path.IndexOfAny(new[] {'?', '*'});
			if (index < 0 || index == 1 && path.Length == 2 && (path == "/?" || path == "-?"))
			{
				yield return path;
				yield break;
			}

			var index2 = -1;
			if (index > 0)
			{
				// TODO: Last Index reads backwards from index
				index2 = Math.Max(
					Math.Max(
						path.LastIndexOf('\\', index),
						path.LastIndexOf('/', index)),
					path.LastIndexOf(':', index));
			}

			if (index2 >= 0)
			{
				pattern = path.Substring(index2 + 1);
				path = path.Substring(0, index2);
			}
			else
			{
				pattern = path;
				path = ".";
			}

			var index3 = pattern.IndexOf('\\');
			if (index3 >= 0)
			{
				subpath = pattern.Substring(index3);
				pattern = pattern.Substring(0, index3);
			}

			if (Directory.Exists(path))
				foreach (var fe in Directory.GetFileSystemEntries(path, pattern))
					foreach (var result in Glob(fe + subpath))
						yield return result;
		}

		#endregion

		#region GenericTextWriter

		public static bool IsClrImage(string fileName)
		{
			try
			{
				using (var fs = new FileStream(fileName, FileMode.Open,
					FileAccess.Read,
					FileShare.Read))
				{
					var dat = new byte[300];
					fs.Read(dat, 0, 128);
					if ((dat[0] != 0x4d) || (dat[1] != 0x5a)) // "MZ" DOS header
						return false;

					var lfanew = BitConverter.ToInt32(dat, 0x3c);
					fs.Seek(lfanew, SeekOrigin.Begin);
					fs.Read(dat, 0, 24); // read signature & PE file header
					if ((dat[0] != 0x50) || (dat[1] != 0x45)) // "PE" signature
						return false;

					fs.Read(dat, 0, 96 + 128); // read PE optional header
					if ((dat[0] != 0x0b) || (dat[1] != 0x01)) // magic
						return false;

					var clihd = BitConverter.ToInt32(dat, 208); // get IMAGE_COR20_HEADER 
					return clihd != 0;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public class GenericTextWriter : TextWriter
		{
			#region Using

			public event Action<string> WriteData;
			public event Action DisposeEvent;

			#endregion

			#region Constructor

			public GenericTextWriter()
			{
			}

			public GenericTextWriter(Action<string> handler)
				: this()
			{
				WriteData += handler;
			}

			#endregion

			#region TextWriter

			public override Encoding Encoding
			{
				get { return Encoding.Unicode; }
			}

			public override void Write(char value)
			{
				WriteData(value.ToString());
			}

			protected override void Dispose(bool disposing)
			{
				if (DisposeEvent != null)
					DisposeEvent();
				base.Dispose(disposing);
			}

			public override void Write(char[] buffer, int index, int count)
			{
				WriteData(new string(buffer, index, count));
			}

			public override void Write(string value)
			{
				WriteData(value);
			}

			#endregion
		}

		#endregion

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
		// ReSharper disable InconsistentNaming
		public struct SHFILEOPSTRUCT
		{
			public IntPtr hwnd;
			[MarshalAs(UnmanagedType.U4)] public int wFunc;
			public string pFrom;
			public string pTo;
			public short fFlags;
			[MarshalAs(UnmanagedType.Bool)] public bool fAnyOperationsAborted;
			public IntPtr hNameMappings;
			public string lpszProgressTitle;
		}

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

		private const int FO_DELETE = 3;
		private const int FOF_ALLOWUNDO = 0x40;
		private const int FOF_NOCONFIRMATION = 0x10; //Don't prompt the user.; 

		public static void DeleteShell(string file, bool confirm = false)
		{
			var newFile = Path.GetFullPath(file);
			var shf = new SHFILEOPSTRUCT
			{
				wFunc = FO_DELETE,
				fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION,
				pFrom = newFile
			};

			if (confirm)
				shf.fFlags &= ~FOF_NOCONFIRMATION;

			SHFileOperation(ref shf);
		}

		public static Encoding GetEncoding(char ch1, char ch2, char ch3)
		{
			if (ch1 == 0xFE && ch2 == 0xFF)
				return Encoding.BigEndianUnicode;
			if (ch1 == 0xFF && ch2 == 0xFE)
				return Encoding.Unicode;
			if (ch1 == 0xEF && ch2 == 0xBB && ch3 == 0xBF)
				return Encoding.UTF8;
			return null;
		}

		public static bool? IsUtf8(Stream reader)
		{
			const int kStart = 0;
			const int kA = 1;
			const int kB = 2;
			const int kC = 3;
			const int kD = 4;
			const int kE = 6;
			const int kF = 7;
			const int kG = 8;

			var state = kStart;
			while (true)
			{
				var ch = reader.ReadByte();
				if (ch < 0)
					return kStart == state ? true : (bool?) null;

				if (state == kStart)
				{
					if (ch < 0x80)
						continue;
					if (ch < 0xC2 || ch >= 0xf5)
						return false;
				}
				else
				{
					if (ch < 0x80 || ch >= 0xc0)
						return false;
				}

				var zone = ch >> 4;
				switch (zone)
				{
					default:
						return false;
					case 0x8:
						if (state == kC || state == kF)
							return false;
						//Debug.Assert(state != kStart);
						state = state == kA ? kStart : (state < kE ? kA : kB);
						break;
					case 0x9:
						if (state == kC || state == kG)
							return false;
						//Debug.Assert(state == kStart);
						state = state == kA ? kStart : (state < kE ? kA : kB);
						break;
					case 0xA:
					case 0xB:
						if (state == kD || state == kG)
							return false;
						//Debug.Assert(state == kStart);
						state = state == kA ? kStart : (state < kE ? kA : kB);
						break;
					case 0xC:
					case 0xD:
						//Debug.Assert(state == kStart);
						state = kA;
						break;
					case 0xE:
						//Debug.Assert(state == kStart);
						state = kB;
						if (ch == 0xE0)
							state = kC;
						else if (ch == 0xED)
							state = kD;
						break;
					case 0xF:
						//Debug.Assert(state == kStart);
						state = kE;
						if (ch == 0xF0)
							state = kF;
						else if (ch == 0xF4)
							state = kG;
						break;
				}
			}
		}

		public static void EnsureDirectory(string dir)
		{
			try
			{
				if (!string.IsNullOrEmpty(dir))
					Directory.CreateDirectory(dir);
			}
			catch (IOException)
			{
			}
		}

		public static void EnsureFile(string file)
		{
			var dir = Path.GetDirectoryName(file);
			EnsureDirectory(dir);
		}

		public static string Quote(string name)
		{
			var quote = name.Any(ch => ch <= ' ' || ch == '"');
			if (!quote)
				return name;

			var sb = new StringBuilder(name.Length + 2);
			sb.Append(name);
			sb.Replace("\"", "\"\"");
			sb.Insert(0, '"');
			sb.Append('"');
			return sb.ToString();
		}

		public static bool RemoveDuplicate(string filename, string backup)
		{
			if (FilesAreSame(filename, backup))
			{
				File.Delete(backup);
				return true;
			}
			return false;
		}

		public static string DateId(DateTime date)
		{
			return date.ToString("YYMMDD-HHmmss");
		}

		public static string InsertBeforeExtension(string name, string insert)
		{
			var nameWithoutExt = Path.ChangeExtension(name, null) ?? "";
			var ext = Path.GetExtension(name) ?? "";
			return nameWithoutExt + insert + ext;
		}

		public static string InsertBeforeFilename(string name, string insert)
		{
			var dirName = Path.GetDirectoryName(name);
			var fileName = Path.GetFileName(name);
			var fullname = insert + fileName;
			if (dirName != null)
				fullname = Path.Combine(dirName, fullname);
			return fullname;
		}

		public static bool FilesAreSame(string file1, string file2, bool checkContent = true)
		{
			try
			{
				if (string.IsNullOrEmpty(file1) || string.IsNullOrEmpty(file2) || !File.Exists(file1) || !File.Exists(file2))
					return false;

				var info1 = new FileInfo(file1);
				var info2 = new FileInfo(file2);

				if (info1.Length != info2.Length)
					return false;

				if (checkContent)
				{
					using (var f1 = File.OpenRead(file1))
					using (var f2 = File.OpenRead(file2))
					{
						while (true)
						{
							var b1 = f1.ReadByte();
							var b2 = f2.ReadByte();
							if (b1 < 0)
								return b2 < 0;
							if (b1 != b2)
								return false;
						}
					}
				}

				return info1.LastWriteTimeUtc == info2.LastWriteTimeUtc;
			}
			catch (IOException)
			{
				return false;
			}
		}

		public static bool FilesAreSame(string filename, TextReader reader)
		{
			try
			{
				if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
					return false;

				using (var file = File.OpenText(filename))
				{
					while (true)
					{
						var b1 = file.Read();
						var b2 = reader.Read();
						if (b1 < 0)
							return b2 < 0;
						if (b1 != b2)
							return false;
					}
				}
			}
			catch (IOException)
			{
				return false;
			}
		}

		public static string UniqueName(string filename)
		{
			var dir = Path.GetDirectoryName(filename);
			var bn = Path.GetFileNameWithoutExtension(filename);
			var ext = Path.GetExtension(filename);

			if (bn == null)
				throw new InvalidOperationException();

			var f = PreviousUniqueName(filename);
			if (f == null)
				return filename;

			long maxNumber = 1;
			var bn2 = Path.GetFileNameWithoutExtension(f);
			var minLength = bn2.Length - bn.Length;
			if (minLength != 0)
				maxNumber = long.Parse(bn2.Substring(bn.Length));

			maxNumber++;
			var numString = maxNumber.ToString();
			if (numString.Length < minLength)
				numString = numString.PadLeft(minLength, '0');

			var uniqueName = bn + numString + ext;
			if (dir != null)
				uniqueName = Path.Combine(dir, uniqueName);
			return uniqueName;
		}

		public static string PreviousUniqueName(string filename)
		{
			var dir = Path.GetDirectoryName(filename) ?? ".";
			var bn = Path.GetFileNameWithoutExtension(filename);

			if (bn == null)
				throw new InvalidOperationException();

			var ext = Path.GetExtension(filename);

			long maxNumber = 0;
			string match = null;

			if (Directory.Exists(dir))
			{
				foreach (var f in Directory.EnumerateFileSystemEntries(dir ?? ".", bn + "*" + ext))
				{
					var bn2 = Path.GetFileNameWithoutExtension(f);

					if (bn2 == null || !bn2.StartsWith(bn, StringComparison.OrdinalIgnoreCase))
					{
						Utility.Break();
						continue;
					}

					long number = 0;
					var diff = bn2.Length - bn.Length;
					if (diff != 0
						&& !long.TryParse(bn2.Substring(bn.Length), out number))
						continue;

					maxNumber = Math.Max(number, maxNumber);
					if (number == maxNumber)
						match = f;
				}
			}

			return match;
		}

		public static bool OutputNeedsUpdate(string input, string output)
		{
			if (!File.Exists(input))
				return false;
			if (!File.Exists(output))
				return true;
			return File.GetLastWriteTime(input) > File.GetLastWriteTime(output);
		}

		public static void BackupFiles(params string[] files)
		{
			foreach (var f in files.Glob())
				Backup(f);
		}

		public static string Backup(string filename, string backup = null)
		{
			if (!File.Exists(filename))
				return null;

			if (backup == null)
			{
				backup = filename;
				backup = InsertBeforeFilename(backup, @"backup\");
				backup = InsertBeforeExtension(backup, "-bkp");
			}

			Debug.Assert(!string.Equals(filename, backup, StringComparison.OrdinalIgnoreCase));

			var previous = PreviousUniqueName(backup);
			if (previous != null
				&& FilesAreSame(filename, previous))
				return null;

			var current = UniqueName(backup);
			EnsureFile(current);
			File.Copy(filename, current);
			return current;
		}

		public static string ToFileSize(long size)
		{
			if (size < 1024)
			{
				return size.ToString("F0") + " bytes";
			}
			if (size < Math.Pow(1024, 2))
			{
				return (size/1024).ToString("F0") + "KB";
			}
			if (size < Math.Pow(1024, 3))
			{
				return (size/Math.Pow(1024, 2)).ToString("F0") + "MB";
			}
			if (size < Math.Pow(1024, 4))
			{
				return (size/Math.Pow(1024, 3)).ToString("F0") + "GB";
			}
			if (size < Math.Pow(1024, 5))
			{
				return (size/Math.Pow(1024, 4)).ToString("F0") + "TB";
			}
			if (size < Math.Pow(1024, 6))
			{
				return (size/Math.Pow(1024, 5)).ToString("F0") + "PB";
			}
			return (size/Math.Pow(1024, 6)).ToString("F0") + "EB";
		}

		#region Stream Functions

		public static Stream ToStream(this string str)
		{
			var byteArray = Encoding.UTF8.GetBytes(str);
			return new MemoryStream(byteArray);
		}

		public static string ToUtf8String(this Stream stream)
		{
			var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}

		/// <summary>
		///     Copy from one stream to another.
		///     Example:
		///     using(var stream = response.GetResponseStream())
		///     using(var ms = new MemoryStream())
		///     {
		///     stream.CopyTo(ms);
		///     // Do something with copied data
		///     }
		/// </summary>
		/// <param name="fromStream">From stream.</param>
		/// <param name="toStream">To stream.</param>
		public static void CopyTo(this Stream fromStream, Stream toStream)
		{
			if (fromStream == null)
				throw new ArgumentNullException("fromStream");
			if (toStream == null)
				throw new ArgumentNullException("toStream");
			var bytes = new byte[8092];
			int dataRead;
			while ((dataRead = fromStream.Read(bytes, 0, bytes.Length)) > 0)
				toStream.Write(bytes, 0, dataRead);
		}

		#endregion

		#region Slashes

		public static string ForwardSlashes(string path)
		{
			return StringTools.Replace(path, '\\', '/');
		}

		public static string BackSlashes(string path)
		{
			return StringTools.Replace(path, '/', '\\');
		}


		#endregion
	}
}