//
// Authors:
// Jonathan Pobst (monkey@jpobst.com)
//
// Copyright (C) 2010 Jonathan Pobst
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Kipunji.Models;
using System.Web;
using System.Xml.Xsl;
using System.Xml;
using System.IO;
using System.Web.Mvc;


namespace Kipunji
{
	public static class Formatter
	{
		public static string FormatTypeNoOperators (string type, bool remove_ns)
		{
			string ops;
			type = RemoveTypeOperators (type, out ops);
			if (remove_ns)
				type = RemoveNamespaces (type);
			return TypeToBuiltInType (type);
		}

		public static MvcHtmlString FormatType (string type, bool remove_ns)
		{
			string ops;
			
			type = RemoveTypeOperators (type, out ops);
			type = TypeToBuiltInType (type);
			
			if (remove_ns)
				type = RemoveNamespaces (type);
			
			if (ops != null)
				return new MvcHtmlString (type + ops);
			
			return new MvcHtmlString (type);
		}
		
		private static string TypeToBuiltInType (string type)
		{
			switch (type.ToLowerInvariant ()) {
				case "system.boolean": type = "bool"; break;
				case "system.byte": type = "byte"; break;
				case "system.sbyte": type = "sbyte"; break;
				case "system.char": type = "char"; break;
				case "system.decimal": type = "decimal"; break;
				case "system.double": type = "double"; break;
				case "system.single": type = "float"; break;
				case "system.int32": type = "int"; break;
				case "system.uint32": type = "uint"; break;
				case "system.int64": type = "long"; break;
				case "system.uint64": type = "ulong"; break;
				case "system.object": type = "object"; break;
				case "system.int16": type = "short"; break;
				case "system.uint16": type = "ushort"; break;
				case "system.string": type = "string"; break;
				case "system.void": type = "void"; break;
				default:
					break;
			}	
		
			return type;
		}
		
		private static readonly char [] TypeOperators = new char [] { '*', '&', '[' };
		
		private static string RemoveTypeOperators (string type, out string ops)
		{
			int idx = type.IndexOfAny (TypeOperators);
			if (idx < 0) {
				ops = null;
				return type;
			}
			
			ops = type.Substring (idx, type.Length - idx);
			return type.Substring (0, idx);
		}
		
		private static string RemoveNamespaces (string type)
		{
			bool adding = true;
			StringBuilder builder = new StringBuilder ();
			
			for (int i = type.Length - 1; i >= 0; i--) {
				char c = type [i];
				if (c == '.') {
					adding = false;
					continue;
				}
				
				if (c == '<' || c == ',')
				    adding = true;
				if (adding)
					builder.Append (c);
			}
			
			char [] s = builder.ToString ().ToCharArray ();
        	Array.Reverse (s);
       	 	
			return new string (s);
		}

		public static MvcHtmlString FormatMethodDisplaySignature (string name, List<Parameter> parameters, bool include_spans, bool html_encode)
		{
			return FormatMethodSignature (name, parameters, true, include_spans, html_encode, true);
		}

		public static MvcHtmlString FormatConstructorDisplaySignature (TypeModel parent_type, string name, List<Parameter> parameters, bool include_spans, bool html_encode)
		{
			return FormatMethodDisplaySignature (parent_type.DisplayName, parameters, include_spans, html_encode);
		}

		public static MvcHtmlString FormatMethodUrlSignature (string name, List<Parameter> parameters)
		{
			return FormatMethodSignature (name, parameters, false, false, false, false);
		}

		public static MvcHtmlString FormatConstructorUrlSignature (TypeModel parent_type, string name, List<Parameter> parameters)
		{	
			return FormatMethodUrlSignature (name, parameters);
		}

		private static MvcHtmlString FormatMethodSignature (string name, List<Parameter> parameters, bool include_name, bool include_spans, bool html_encode, bool remove_ns)
		{
			name = html_encode ? HttpUtility.HtmlEncode (name) : name;
			
			string ret =include_spans ? String.Format ("<span>{0} (</span>", name) : string.Format ("{0} (", name);

			string format = include_name ? "{0}{1} {2}{3}, " : "{0}{1}{3}, ";
			string span_open = include_spans ? "<span>" : String.Empty;
			string span_close = include_spans ? "</span>" : String.Empty;
			foreach (var p in parameters) {
				string param_type = html_encode ? HttpUtility.HtmlEncode (FormatType (p.Type, remove_ns).ToString ()) : FormatType (p.Type, remove_ns).ToString ();
				string param_name = html_encode ? HttpUtility.HtmlEncode (p.Name) : p.Name;
				ret += string.Format (format, span_open, param_type, param_name, span_close);
			}

			ret = ret.TrimEnd (' ', ',');
			ret += ")";

			return new MvcHtmlString (ret);
		}


		public static MvcHtmlString FormatPropertySignature (string name)
		{
			return new MvcHtmlString (name);
		}

		public static MvcHtmlString FormatFieldSignature (string name)
		{
			return new MvcHtmlString (name);
		}

		public static string FormatClassSignature (string name, string basetype, List<string> interfaces)
		{
			string ret = name;

			if (basetype != "System.Object" || interfaces.Count > 0)
				ret += " : ";

			if (basetype != "System.Object")
				ret += FormatType (basetype, false) + ", ";

			foreach (var i in interfaces)
				ret += i + ", ";

			ret = ret.TrimEnd (' ', ',');
			ret += ")";

			return ret;
		}

		public static MvcHtmlString FormatHtml (string raw)
		{		
			//EnsureTransform ();
			
			XsltArgumentList args = new XsltArgumentList();
			args.AddParam("show", "", "namespace");
			
			var output = new StringBuilder ();
			//ecma_transform.Transform (XmlReader.Create (new StringReader (raw)), 
			//        args, 
			//        XmlWriter.Create (output, ecma_transform.OutputSettings), null);

			//return MakeLinks (output.ToString ());
			return new MvcHtmlString (MakeLinks (raw));
		}

		public static MvcHtmlString FormatTypeSignature (string root, TypeModel model)
		{
			int p = 0;
			string signature = model.Signature.Value;
			StringBuilder res = new StringBuilder ();
			
			res.Append ("<table class='type-signature'><tr><td class='type-signature-start'>");
			
			while (p < signature.Length && signature [p] != ':') {
				res.Append (signature [p]);
				++p;
			}
						
			bool bt_added = false;
			string bt = model.BaseType;
			if (!String.IsNullOrEmpty (bt) && bt != "System.Object" && bt != "System.ValueType") {
				res.Append (" : </td><td class='type-signature-cell'>");
				bt = HttpUtility.HtmlDecode (bt);
				res.Append ("<span style='white-space:nowrap;'>" + CreateTypeLink (root, bt) + "</span>");
				bt_added = true;
			}
			
			bool first = true;
			foreach (string intf in model.Interfaces) {
				if (bt_added && first) 
					res.Append (", ");
				if (!bt_added)
					res.Append (" : </td><td class='type-signature-cell'> ");
				if (!first)
					res.Append (", ");

				string _intf = HttpUtility.HtmlDecode (intf);
				res.Append ("<span style='white-space:nowrap;'>" + CreateTypeLink (root, _intf) + "</span>");
	
				bt_added = true;
				first = false;
			}

			if (!bt_added)
				res.Append ("</span>");
			res.Append ("</td></tr></table>");

			return new MvcHtmlString (res.ToString ());
		}
		
		private static string MakeLinks(string content)
		{
			MatchEvaluator linkUpdater=new MatchEvaluator(MakeLink);
			if(content.Trim().Length<1|| content==null)
				return content;
			try
			{
				string updatedContents=Regex.Replace(content,"(<a[^>]*href=['\"].:)([^'\"]+)(['\"][^>]*)(>)", linkUpdater);
				return(updatedContents);
			}
			catch(Exception e)
			{
				return "LADEDA" + content+"!<!--Exception:"+e.Message+"-->";
			}
		}
		
		private static string MakeLink (Match theMatch)
		{
			string updated_link = null;

			// Return the link without change if it of the form
			//	$protocol:... or #...
			string link = theMatch.Groups[2].ToString();
			updated_link = String.Format ("<a href=\"{0}\" {1}",
					VirtualPathUtility.ToAbsolute ("~/" + HttpUtility.UrlEncode (link.Replace ("file://",""))),
					theMatch.Groups[4].ToString());
			return updated_link;
		}

		private static XslCompiledTransform ecma_transform;
		private static void EnsureTransform ()
		{
			if (ecma_transform == null) {
				ecma_transform = new XslCompiledTransform ();
				
				XmlReader xml_reader = new XmlTextReader (ModelFactory.GetEcmaTransformXslPath ());
	//			XmlResolver r = new ManifestResourceResolver (".");
				ecma_transform.Load (xml_reader); // , XsltSettings.TrustedXslt);
				
				
			}
		}
			
		
		
		
		
		
		private class GenericTypeData {
			public int start;
			public int end = -1;
			public int args_start = -1;
			public int args_count;
			
			public GenericTypeData Parent = null;
			public List<GenericTypeData> Args = new List<GenericTypeData> ();
			
			public GenericTypeData (int start)
			{
				this.start = start;
			}
		}
		
#if _TESTING
		static Formatter ()
		{
			CreateTypeLink ("List<Foobar<Baz>>", String.Empty);	
			CreateTypeLink ("List<Foobar1, Foobar2<Baz>>", String.Empty);	
			CreateTypeLink ("List<Foobar1, Foobar2<Baz<Baz2>>>", String.Empty);	
			CreateTypeLink ("List<Foobar1<Bazer>, Foobar2<Baz<Baz2>>>", String.Empty);	
		}
#endif
		private static void Dump (string root, string str, StringBuilder buffer, GenericTypeData item, int depth)
		{

			int end = item.end;
			if (item.Args.Count > 0)
				end = item.args_start;
			
			string ops;
			string type_name = RemoveTypeOperators (str.Substring (item.start, end - item.start), out ops);
			string type_link = type_name;
			
			if (type_link == "TKey" || type_link == "TValue" || type_link == "T") {
				buffer.Append (type_link);
			} else {
				// Mdoc removes the System. part of some namespaces
				if (type_link.IndexOf ('.') == -1)
					type_link = "System." + type_link;
				
				if (item.Args.Count > 0)
					type_link += "`" + item.Args.Count;
				
				buffer.AppendFormat ("<a href=\"{0}{1}\">{2}{3}</a>", root, HttpUtility.UrlEncode (type_link), TypeToBuiltInType (type_name), ops);
			}
	
			if (item.Args.Count > 0) {
				buffer.Append (HttpUtility.HtmlEncode ("<"));
				for (int i = 0; i < item.Args.Count; i++) {
					Dump (root, str, buffer, item.Args [i], depth + 1);
					if (i < item.Args.Count - 1)
						buffer.Append (", ");
				}
				buffer.Append (HttpUtility.HtmlEncode (">"));
			}
		}

		public static MvcHtmlString CreateTypeLink (string root, string type)
		{
			StringBuilder res = new StringBuilder ();
			
			GenericTypeData current = new GenericTypeData (0);
			GenericTypeData first = current;
					
			switch (type) {
			case "struct":
			case "TKey":
			case "TValue":
			case "T":
				return new MvcHtmlString (type);
			}
			
			for (int i = 0; i < type.Length; i++) {
				if (type [i] == '<') {
					current.args_start = i;
					GenericTypeData child = new GenericTypeData (i + 1);
					child.Parent = current;
					current.Args.Add (child);
					current = child;
				}
				if (type [i] == '>') {
					current.end = i;
					current = current.Parent;	
				}
				if (type [i] == ',') {
					current.end = i;
					GenericTypeData child = new GenericTypeData (i + 1);
					child.Parent = current.Parent;
					current.Parent.Args.Add (child);
					current = child;
				}
			}
			
			// No generics found.
			if (first.end == -1)
				first.end = type.Length;
			
			Dump (root, type, res, first, 0);

			return new MvcHtmlString (res.ToString ());
		}
	}
}