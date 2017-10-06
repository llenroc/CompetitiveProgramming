#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2003-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

#endregion

namespace Softperson
{
	public static class ResourceTools
	{
		#region Microsoft ResX Schema

		//  Version 2.0

		//  The primary goals of this format is to allow a simple XML format 
		//  that is mostly human readable. The generation and parsing of the 
		//  various data types are done through the TypeConverter classes 
		//  associated with the data types.

		//  Example:

		//  ... ado.net/XML headers & schema ...
		//  <resheader name='resmimetype'>text/microsoft-resx</resheader>
		//  <resheader name='version'>2.0</resheader>
		//  <resheader name='reader'>System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
		//  <resheader name='writer'>System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
		//  <data name='Name1'><value>this is my long string</value><comment>this is a comment</comment></data>
		//  <data name='Color1' type='System.Drawing.Color, System.Drawing'>Blue</data>
		//  <data name='Bitmap1' mimetype='application/x-microsoft.net.object.binary.base64'>
		//      <value>[base64 mime encoded serialized .NET Framework object]</value>
		//  </data>
		//  <data name='Icon1' type='System.Drawing.Icon, System.Drawing' mimetype='application/x-microsoft.net.object.bytearray.base64'>
		//      <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
		//      <comment>This is a comment</comment>
		//  </data>

		//  There are any number of 'resheader' rows that contain simple 
		//  name/value pairs.

		//  Each data row contains a name, and value. The row also contains a 
		//  type or mimetype. Type corresponds to a .NET class that support 
		//  text/value conversion through the TypeConverter architecture. 
		//  Classes that dont support this are serialized and stored with the 
		//  mimetype set.

		//  The mimetype is used for serialized objects, and tells the 
		//  ResXResourceReader how to depersist the object. This is currently not 
		//  extensible. For a given mimetype the value must be set accordingly:

		//  Note - application/x-microsoft.net.object.binary.base64 is the format 
		//  that the ResXResourceWriter will generate, however the reader can 
		//  read any of the formats listed below.

		//  mimetype: application/x-microsoft.net.object.binary.base64
		//  value   : The object must be serialized with 
		//          : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
		//          : and then encoded with base64 encoding.

		//  mimetype: application/x-microsoft.net.object.soap.base64
		//  value   : The object must be serialized with 
		//          : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
		//          : and then encoded with base64 encoding.

		//  mimetype: application/x-microsoft.net.object.bytearray.base64
		//  value   : The object must be serialized into a byte array 
		//          : using a System.ComponentModel.TypeConverter
		//          : and then encoded with base64 encoding.

		public static string ResXSchema = @"
<?xml version='1.0' encoding='utf-8'?>
<root>
  <xsd:schema id='root' xmlns='' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:msdata='urn:schemas-microsoft-com:xml-msdata'>
    <xsd:import namespace='http://www.w3.org/XML/1998/namespace' />
    <xsd:element name='root' msdata:IsDataSet='true'>
      <xsd:complexType>
        <xsd:choice maxOccurs='unbounded'>
          <xsd:element name='metadata'>
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name='value' type='xsd:string' minOccurs='0' />
              </xsd:sequence>
              <xsd:attribute name='name' use='required' type='xsd:string' />
              <xsd:attribute name='type' type='xsd:string' />
              <xsd:attribute name='mimetype' type='xsd:string' />
              <xsd:attribute ref='xml:space' />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name='assembly'>
            <xsd:complexType>
              <xsd:attribute name='alias' type='xsd:string' />
              <xsd:attribute name='name' type='xsd:string' />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name='data'>
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name='value' type='xsd:string' minOccurs='0' msdata:Ordinal='1' />
                <xsd:element name='comment' type='xsd:string' minOccurs='0' msdata:Ordinal='2' />
              </xsd:sequence>
              <xsd:attribute name='name' type='xsd:string' use='required' msdata:Ordinal='1' />
              <xsd:attribute name='type' type='xsd:string' msdata:Ordinal='3' />
              <xsd:attribute name='mimetype' type='xsd:string' msdata:Ordinal='4' />
              <xsd:attribute ref='xml:space' />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name='resheader'>
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name='value' type='xsd:string' minOccurs='0' msdata:Ordinal='1' />
              </xsd:sequence>
              <xsd:attribute name='name' type='xsd:string' use='required' />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name='resmimetype'>
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name='version'>
    <value>2.0</value>
  </resheader>
  <resheader name='reader'>
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name='writer'>
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <assembly alias='System.Windows.Forms' name='System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089' />
	<!-- Placeholder -->
</root>
";

		//   <data name='Image1' type='System.Resources.ResXFileRef, System.Windows.Forms'>
		//  <value>Resources\Image1.png;System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a</value>
		//</data>

		#endregion

		#region Methods

		public static string CopyResourceToTempFile(Type type, string name)
		{
			var temp = "";
			var nsdot = type.Namespace;
			if (nsdot == null)
				nsdot = "";
			else if (nsdot != "")
				nsdot += ".";
			var stream = type.Module.Assembly
				.GetManifestResourceStream(nsdot + name);
			if (stream != null)
			{
				var sr = new StreamReader(stream);
				temp = Path.GetTempFileName();
				var sw = new StreamWriter(temp, false);
				sw.Write(sr.ReadToEnd());
				sw.Flush();
				sw.Close();
			}
			return temp;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Stream GetResource(string name)
		{
			//name should be Softperson.file.ext
			return Assembly.GetEntryAssembly().GetManifestResourceStream(name);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Stream GetResource(string name, Type type)
		{
			return type.Assembly.GetManifestResourceStream(name);
		}

		//public static Bitmap GetBitmap(string name)
		//{
		//    return new Bitmap(GetResource(name));
		//    // return Image.FromStream( GetNetResource(name) );
		//}

		public static string PackUriString(string folderPath, string assembly = null)
		{
			var sb = new StringBuilder();
			sb.Append("pack://application:,,,");
			if (assembly != null)
			{
				sb.Append('/');
				sb.Append(assembly);
				sb.Append(";component");
			}
			if (!folderPath.StartsWith("/"))
				sb.Append('/');
			sb.Append(folderPath);
			return sb.ToString();
		}

		public static Uri PackUri(string folderPath, string assembly = null)
		{
			return new Uri(PackUriString(folderPath, assembly),
				UriKind.RelativeOrAbsolute);
		}

		// Regular resources are stored in AssemblyName.g.resources
		// A resource located in Root/Folder/File.Ext
		// has the resource name folder/file.ext in lowercase

		// Class-specific resources are store in Namespace.Class.resources
		// ResourceName.Resx resources are stored in RootNamespace.Folder.ResourceName.resources
		// Embedded resources are in RootNameSpace.Folder.Filename.ext

		public static UnmanagedMemoryStream LoadEmbeddedResource(string resourcePath, Assembly assembly = null)
		{
			var path = resourcePath;
			if (path.IndexOf('\\')>=0)
				path = path.Replace('\\', '.');
			if (path.IndexOf('/') >= 0)
				path = path.Replace('/', '.');

			if (assembly == null)
				assembly = Assembly.GetCallingAssembly();
			return (UnmanagedMemoryStream) assembly.GetManifestResourceStream(path);
		}

		public static string MapResourceName(string root, string path, string namespc)
		{
			var resourcePath = path.Length < 1 || path[0] != '/' && path[0] != '\\'
				? Path.Combine(root, path)
				: path.Substring(1);

			var sb = new StringBuilder(resourcePath.Length + namespc.Length);
			if (!string.IsNullOrEmpty(namespc))
			{
				sb.Append(namespc);
				sb.Append('.');
			}
			sb.Append(resourcePath);
			sb.Replace('/', '.');
			sb.Replace('\\', '.');
			resourcePath = sb.ToString();
			return resourcePath;
		}

		public static string MapResourceNameCallerPath(string root, string path, string namespc, [CallerFilePath] string callerPath = null)
		{
			callerPath = Path.GetDirectoryName(callerPath);
			var relCallerPath = FileTools.MakeFilePathRelative(root, callerPath);
			return MapResourceName(relCallerPath, path, namespc);
		}

		public static UnmanagedMemoryStream LoadEmbeddedResource(string root, string path, [CallerFilePath] string callerPath = null)
		{
			var resourcePath = MapResourceNameCallerPath(root, path, "", callerPath);
			return LoadEmbeddedResource(resourcePath);
		}


		public static ResourceManager LoadDefaultResource(Assembly assembly = null)
		{
			if (assembly == null)
				assembly = Assembly.GetCallingAssembly();
			var manager = new ResourceManager(assembly.GetName().Name + ".g", assembly);
			return manager;
		}

		public static IEnumerable<string> ListResources(Assembly assembly)
		{
			var rm = LoadDefaultResource(assembly);
			var rs = rm.GetResourceSet(CultureInfo.CurrentCulture, false, false);
			foreach (DictionaryEntry pair in rs)
			{
				yield return pair.ToString();
			}
		}

		#endregion
	}
}