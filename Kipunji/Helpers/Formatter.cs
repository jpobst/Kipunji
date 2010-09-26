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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Kipunji.Models;

namespace Kipunji
{
	public static class Formatter
	{
		public static string FormatType (string type)
		{
			switch (type.ToLowerInvariant ()) {
				case "system.int32": return "int";
				case "system.string": return "string";
				case "system.char": return "char";
				case "system.double": return "double";
				case "system.single": return "float";
				case "system.boolean": return "bool";
				case "system.int32[]": return "int[]";
				case "system.string[]": return "string[]";
				case "system.char[]": return "char[]";
				case "system.double[]": return "double[]";
				case "system.single[]": return "float[]";
				case "system.boolean[]": return "bool[]";
				case "system.boolean&": return "ref bool";
				case "system.int32&": return "ref int";
				case "system.string&": return "ref string";
				case "system.char&": return "ref char";
				case "system.double&": return "ref double";
				case "system.single&": return "ref float";
				case "system.void": return "void";
				default:
					return type;
			}
		}

		public static string FormatMethodSignature (string name, List<Parameter> parameters)
		{
			string ret = string.Format ("{0} (", name);

			foreach (var p in parameters)
				ret += string.Format ("{0}, ", FormatType (p.Type));

			ret = ret.TrimEnd (' ', ',');
			ret += ")";

			return ret;
		}

		public static string FormatPropertySignature (string name)
		{
			return name;
		}

		public static string FormatFieldSignature (string name)
		{
			return name;
		}

		public static string FormatClassSignature (string name, string basetype, List<string> interfaces)
		{
			string ret = name;

			if (basetype != "System.Object" || interfaces.Count > 0)
				ret += " : ";

			if (basetype != "System.Object")
				ret += FormatType (basetype) + ", ";

			foreach (var i in interfaces)
				ret += i + ", ";

			ret = ret.TrimEnd (' ', ',');
			ret += ")";

			return ret;
		}

		private static readonly Regex ParamRef = new Regex (@"\<paramref\ name\=\""(?<data>(?:.|\n)*?)\""\ />", RegexOptions.Compiled | RegexOptions.Singleline);
		private static readonly Regex SeeRef = new Regex (@"\<see\ cref\=\""\w:(?<data>(?:.|\n)*?)\""\ />", RegexOptions.Compiled | RegexOptions.Singleline);

		public static string FormatHtml (string raw)
		{
			raw = ParamRef.Replace (raw, "<i><b>${data}</b></i>");
			raw = SeeRef.Replace (raw, "<a href=\"/${data}\">${data}</a>");

			raw = raw.Replace ("<code lang=\"C#\">", "<pre class=\"brush: csharp;\">");
			raw = raw.Replace ("\n</code>", "</pre>");
			raw = raw.Replace ("</code>", "</pre>");

			raw = raw.Replace ("<para>", "<p class=\"indent\">");
			raw = raw.Replace ("</para>", "</p>");

			return raw;
		}

		public static string CreateTypeLink (string type, string text)
		{
			return string.Format ("<a href=\"{0}\">{1}</a>", type, text);
		}
	}
}