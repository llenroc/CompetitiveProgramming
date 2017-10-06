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
using System.IO;
using System.Text;

// ReSharper disable InconsistentNaming

#endregion

namespace Softperson
{
	/// <summary>
	///     Summary description for StringTools.
	/// </summary>
	public static class Stdlib
	{
		public static List<string> Scan(string data, string format)
		{
			var len = format.Length;
			var lenData = format.Length;
			var iData = 0;
			List<string> result = null;

			for (var i = 0; i < len && iData < lenData; i++)
			{
				var ch = format[i];
				if (ch == '%')
				{
					var ignore = false;

					// Skip first character
					if (++i == len) break;
					ch = format[i];
					if (ch == '%') goto Test;

					// Examine remaining characters
					if (ch == '*')
					{
						ignore = true;
						if (++i == len) break;
						ch = format[i];
					}

					// Get length
					var maxLen = 0;
					while (ch >= '0' && ch <= '9')
					{
						if (++i == len) break;
						maxLen = maxLen*10 + ch - '0';
						ch = format[i];
					}
					if (i >= len) break;

					object o;
					var start = iData;
					switch (char.ToLower(ch))
					{
						case 'c':
							if (maxLen == 0) maxLen = 1;
							while (iData < lenData && maxLen-- > 0)
								iData++;
							o = data.Substring(start, iData - start);
							break;
						case 's':
							if (data[iData] == '"')
							{
								if (maxLen == 0) maxLen = int.MaxValue;
								while (iData < lenData
									   && maxLen-- != 0
									   && data[iData] != '"')
									iData++;
								o = data.Substring(start + 1, iData - start - 1);
								if (iData < lenData) iData++;
							}
							else
							{
								while (iData < lenData && !char.IsWhiteSpace(data, iData))
									iData++;
								o = data.Substring(start, iData - start);
							}
							break;
						case 'd':
							while (iData < lenData && "1234567890-+".IndexOf(ch) != -1)
								iData++;
							o = Convert.ToInt32(data.Substring(start, iData - start));
							break;
						case 'x':
							while (iData < lenData && "1234567890abcdefABCDEF".IndexOf(ch) != -1)
								iData++;
							o = Convert.ToInt32(data.Substring(start, iData - start), 16);
							break;
						case 'f':
							while (iData < lenData && "1234567890.e+-".IndexOf(ch) != -1)
								iData++;
							o = Convert.ToDouble(data.Substring(start, iData - start));
							break;
						case 'm':
							while (iData < lenData && "$,.1234567890+-".IndexOf(ch) != -1)
								iData++;
							o = Convert.ToDouble(data.Substring(start, iData - start));
							break;
						default:
							throw new FormatException();
					}

					if (!ignore)
					{
						if (result == null)
							result = new List<string>();
						result.Add((string) o);
					}
				}

				if (ch == ' ')
				{
					while (iData < lenData && char.IsWhiteSpace(data[iData]))
						iData++;
					continue;
				}

				Test:
				// If character doesn't match
				if (ch != data[iData])
					break;
				iData++;
			}

			return result;
		}

		#region Scanf

		public static int fscanf<T>(TextReader reader, string message, out T arg1)
		{
			throw new NotImplementedException();
		}

		public static int fscanf<T1, T2>(TextReader reader, string message,
			out T1 arg1, out T2 arg2)
		{
			throw new NotImplementedException();
		}

		public static int fscanf<T1, T2, T3>(TextReader reader, string message,
			out T1 arg1, out T2 arg2, out T3 arg3)
		{
			throw new NotImplementedException();
		}


		public static int sscanf<T>(string reader, string message, out T arg1)
		{
			return fscanf(new StringReader(reader), message, out arg1);
		}

		public static int sscanf<T1, T2>(string reader, string message,
			out T1 arg1, out T2 arg2)
		{
			return fscanf(new StringReader(reader), message, out arg1, out arg2);
		}

		public static int sscanf<T1, T2, T3>(string reader, string message,
			out T1 arg1, out T2 arg2, out T3 arg3)
		{
			return fscanf(new StringReader(reader), message,
				out arg1, out arg2, out arg3);
		}

		public static int scanf<T>(string message, out T arg1)
		{
			return fscanf(Console.In, message, out arg1);
		}

		public static int scanf<T1, T2>(string message,
			out T1 arg1, out T2 arg2)
		{
			return fscanf(Console.In, message, out arg1, out arg2);
		}

		public static int scanf<T1, T2, T3>(string message,
			out T1 arg1, out T2 arg2, out T3 arg3)
		{
			return fscanf(Console.In, message,
				out arg1, out arg2, out arg3);
		}

		#endregion

		#region PrintF

		public static string ssprintf(string format, params object[] args)
		{
			var sb = new StringWriter();
			fprintf(sb, format, args);
			return sb.ToString();
		}

		public static void printf(string format, params object[] args)
		{
			fprintf(Console.Out, format, args);
		}

		public static void fprintf(this TextWriter writer, string format,
			params object[] args)
		{
			var i = 0;
			var arg = 0;

			if (args.Length == 0)
			{
				writer.Write(format);
				return;
			}

			while (i < format.Length)
			{
				var index = format.IndexOf('%', i);
				if (index < 0)
				{
					writer.Write(format, i, format.Length - i);
					break;
				}

				writer.Write(format, i, index - i);
				i = index + 1;
				if (i < format.Length && format[i] == '%')
				{
					writer.Write('%');
					continue;
				}

				var quit = false;
				var dec = false;
				var rightAlign = true;
				var width = 0;
				var precision = 0;
				var pad = ' ';
				var data = args[arg++] ?? "(null)";
				string plus = null;
				for (; i < format.Length; i++)
				{
					var ch = format[i];
					switch (ch)
					{
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							var c = ch - '0';
							if (dec)
								precision = precision*10 + c;
							else
							{
								width = width*10 + c;
								if (width == 0)
									pad = '0';
							}
							break;
						case 'l':
						case 'L':
						case 'h':
							break;
						case '.':
							if (dec)
								throw new FormatException();
							dec = true;
							break;
						case '*':
							c = Convert.ToInt32(data);
							data = args[arg++] ?? "(null)";
							if (dec)
								precision = c;
							else
								width = c;
							break;
						case '-':
							rightAlign = false;
							break;
						case '+':
							plus = "+";
							break;
						case ' ':
							plus = " ";
							break;
						case 'c':
							if (!dec)
								data = Convert.ToChar(data);
							else
							{
								var cdata = data as char[];
								if (cdata != null)
									data = new string(cdata, 0,
										cdata.Length);
							}
							goto case 's';
						case 'q':
						case 'Q':
							data = EncodeSql(data.ToString(),
								ch == 'Q');
							goto case 's';
						case 's':
						case 'p':
							var s = data.ToString();
							if (dec && s.Length > precision)
								s = s.Remove(precision);
							data = s;
							quit = true;
							break;
						case 'i':
						case 'u':
							ch = 'D';
							goto case 'D';
						case 'd':
						case 'D':
						case 'e':
						case 'E':
						case 'g':
						case 'G':
						case 'f':
						case 'F':
						case 'x':
						case 'X':
							var d = data;
							var iformat = data as IFormattable;
							if (iformat != null)
							{
								var f = ch.ToString();
								if (dec)
									f += precision;
								data = iformat.ToString(f, null);
							}
							if (plus != null && Convert.ToDouble(d) > 0)
								data = plus + data;
							quit = true;
							break;
						default:
							throw new FormatException();
					}

					if (quit)
					{
						var result = data.ToString();
						if (width > result.Length)
						{
							result = rightAlign
								? result.PadLeft(width, pad)
								: result.PadRight(width);
						}
						writer.Write(result);
						break;
					}
				}

				if (i == format.Length)
					throw new FormatException();

				i++;
			}

#if DEBUG
			if (arg < args.Length)
				Debugger.Break();
#endif
		}

		public static string EncodeSql(this string s,
			bool quoteFully = true)
		{
			var sb = new StringBuilder(s);
			sb.Replace("'", "''");
			if (quoteFully)
			{
				sb.Insert(0, '\'');
				sb.Append('\'');
			}
			return sb.ToString();
		}

		#endregion

		#region String Functions

		#endregion
	}
}