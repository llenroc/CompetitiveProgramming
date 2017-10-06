#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2005, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

#endregion

namespace Softperson
{
	public static class Xml
	{
		public static XmlDocument CurrentDocument;

		public static string Encode(string s)
		{
			if (s == null)
				return "";

			var sb = new StringBuilder(s.Length);

			var prevChar = ' ';
			foreach (var ch in s)
			{
				switch (ch)
				{
					default:
						sb.Append(ch);
						break;
					case ' ':
						if (!char.IsWhiteSpace(prevChar) || ch == '\n')
							goto default;
						sb.Append("&nbsp;");
						break;
					case '<':
						sb.Append("&lt;");
						break;
					case '>':
						sb.Append("&gt;");
						break;
					case '"':
						sb.Append("&quot;");
						break;
					case '&':
						sb.Append("&amp;");
						break;
				}
				prevChar = ch;
			}

			return sb.ToString();
		}

		public static XmlElement New(string name, params object[] nodes)
		{
			var element = CurrentDocument.CreateElement(name);
			Add(element, nodes);
			return element;
		}

		public static bool Get(XmlElement element, string name, ref string variable)
		{
			var node = element.SelectSingleNode(name);
			if (node == null)
				return false;
			variable = node.InnerText;
			return true;
		}

		public static bool Get(XmlElement element, string name, ref bool variable)
		{
			string v = null;
			if (!Get(element, name, ref v))
				return false;
			variable = XmlConvert.ToBoolean(v);
			return true;
		}

		public static bool Get(XmlElement element, string name, ref int variable)
		{
			string v = null;
			if (!Get(element, name, ref v))
				return false;
			variable = XmlConvert.ToInt32(v);
			return true;
		}

		public static bool Get(XmlElement element, string name, ref long variable)
		{
			string v = null;
			if (!Get(element, name, ref v))
				return false;
			variable = XmlConvert.ToInt64(v);
			return true;
		}

		public static bool Get(XmlElement element, string name, ref double variable)
		{
			string v = null;
			if (!Get(element, name, ref v))
				return false;
			variable = XmlConvert.ToDouble(v);
			return true;
		}

		public static bool Get<T>(XmlElement element, string name, ref T variable)
		{
			string v = null;
			if (!Get(element, name, ref v))
				return false;
			variable = Utility.Convert<T>(v);
			return true;
		}


		public static void Add(XmlElement element, params object[] nodes)
		{
			Add(element, (object) nodes);
		}

		public static void Add(XmlElement element, object content)
		{
			if (content == null)
				return;

			var node = content as XmlNode;
			if (node != null)
			{
				element.AppendChild(node);
				return;
			}

			var text = content as string;
			if (text == null)
			{
				var enumerable = content as IEnumerable;
				if (enumerable != null)
				{
					foreach (var obj in enumerable)
						Add(element, obj);
					return;
				}
				text = GetXmlString(content);
			}

			element.AppendChild(CurrentDocument.CreateTextNode(text));
		}

		public static string GetXmlString(object o)
		{
			if (o == null)
				return "";

			var code = Type.GetTypeCode(o.GetType());
			switch (code)
			{
				default:
					return o.ToString();
				case TypeCode.Boolean:
					return XmlConvert.ToString((bool) o);
				case TypeCode.Byte:
					return XmlConvert.ToString((byte) o);
				case TypeCode.Char:
					return XmlConvert.ToString((char) o);
				case TypeCode.DateTime:
					return XmlConvert.ToString((DateTime) o, XmlDateTimeSerializationMode.Utc);
				case TypeCode.Decimal:
					return XmlConvert.ToString((decimal) o);
				case TypeCode.Single:
					return XmlConvert.ToString((float) o);
				case TypeCode.Double:
					return XmlConvert.ToString((double) o);
				case TypeCode.SByte:
					return XmlConvert.ToString((sbyte) o);
				case TypeCode.Int16:
					return XmlConvert.ToString((short) o);
				case TypeCode.Int32:
					return XmlConvert.ToString((int) o);
				case TypeCode.Int64:
					return XmlConvert.ToString((long) o);
				case TypeCode.UInt16:
					return XmlConvert.ToString((ushort) o);
				case TypeCode.UInt32:
					return XmlConvert.ToString((uint) o);
				case TypeCode.UInt64:
					return XmlConvert.ToString((ulong) o);
			}
		}


		public static void WriteShallowNode(XmlReader reader, XmlWriter writer)
		{
			switch (reader.NodeType)
			{
				case XmlNodeType.Element:
					writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
					writer.WriteAttributes(reader, true);

					if (reader.IsEmptyElement)
						writer.WriteEndElement();

					break;

				case XmlNodeType.Text:
					writer.WriteString(reader.Value);
					break;

				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					writer.WriteWhitespace(reader.Value);
					break;

				case XmlNodeType.CDATA:
					writer.WriteCData(reader.Value);
					break;

				case XmlNodeType.EntityReference:
					writer.WriteEntityRef(reader.Name);
					break;

				case XmlNodeType.XmlDeclaration:
				case XmlNodeType.ProcessingInstruction:
					writer.WriteProcessingInstruction(reader.Name, reader.Value);
					break;

				case XmlNodeType.DocumentType:
					writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"),
						reader.GetAttribute("SYSTEM"), reader.Value);
					break;

				case XmlNodeType.Comment:
					writer.WriteComment(reader.Value);
					break;

				case XmlNodeType.EndElement:
					writer.WriteFullEndElement();
					break;
			}
		}

		public static IXmlNamespaceResolver GetNamespace(string variable, string uri)
		{
			var ns = new XmlNamespaceManager(new NameTable());
			ns.AddNamespace(variable, uri);
			return ns;
		}

		public static string GetFile(XElement element)
		{
			var source = element.BaseUri;
			if (!source.StartsWith("file:///"))
				return null;

			source = source.Replace("file:///", "");
			source = source.Replace("/", @"\");
			return source;
		}

		public static XmlDocument ToXmlDocument(this XDocument xDocument)
		{
			var xmlDocument = new XmlDocument();
			using (var xmlReader = xDocument.CreateReader())
			{
				xmlDocument.Load(xmlReader);
			}
			return xmlDocument;
		}

		public static XDocument ToXDocument(this XmlDocument xmlDocument)
		{
			using (var nodeReader = new XmlNodeReader(xmlDocument))
			{
				nodeReader.MoveToContent();
				return XDocument.Load(nodeReader);
			}
		}

		public static XmlDocument ToXmlDocument(this XElement xElement)
		{
			var sb = new StringBuilder();
			var xws = new XmlWriterSettings {OmitXmlDeclaration = true, Indent = false};
			using (var xw = XmlWriter.Create(sb, xws))
			{
				xElement.WriteTo(xw);
			}
			var doc = new XmlDocument();
			doc.LoadXml(sb.ToString());
			return doc;
		}

		public static Stream ToMemoryStream(this XmlDocument doc)
		{
			var xmlStream = new MemoryStream();
			doc.Save(xmlStream);
			xmlStream.Flush(); //Adjust this if you want read your data 
			xmlStream.Position = 0;
			return xmlStream;
		}
	}
}