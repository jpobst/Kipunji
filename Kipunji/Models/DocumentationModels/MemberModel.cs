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

namespace Kipunji.Models
{
	public class MemberModel : BaseDocModel
	{
		public string Assembly { get; set; }
		public string ParentType { get; set; }
		public string Namespace { get; set; }
		public string Type { get; set; }
		public string ReturnType { get; set; }
		public string ReturnSummary { get; set; }
		public Signature Signature { get; set; }
		public List<Parameter> Parameters { get; private set; }
		public AssemblyInfo AssemblyInfo { get; set; }
		public string Visibility { get; set; }

		public MemberModel ()
		{
			Parameters = new List<Parameter> ();
		}

		public string FormattedSignature {
			get {
				if (Type == "Property")
					return Formatter.FormatPropertySignature (Name);
				if (Type == "Field")
					return Formatter.FormatFieldSignature (Name);

				return Formatter.FormatMethodSignature (Name, Parameters);
			}
		}

		public string FormattedSummary {
			get { return Formatter.FormatSummary (Summary); }
		}

		public string FormattedReturnType {
			get { return Formatter.FormatType (ReturnType); }
		}

		public string FormattedRemarks {
			get { return Formatter.FormatHtml (Remarks.TrimEnd ('\n')); }
		}

		public string FormattedReturnSummary {
			get { return Formatter.FormatHtml (ReturnSummary.TrimEnd ('\n')); }
		}

		public string FormattedVersionInfo {
			get {
				string ret = "This {0} is available in: {1}";
				string ver = string.Empty;

				foreach (var ai in AssemblyInfo.Versions)
					ver += string.Format ("{0}, ", ai);

				ver = ver.TrimEnd (',', ' ');

				return string.Format (ret, Type.ToLowerInvariant (), ver); 
			}
		}

		public override string ToString ()
		{
			return Name;
		}

		public string AssemblyUrl { get { return string.Format ("~/{0}", Assembly); } }
		public string NamespaceUrl { get { return string.Format ("~/{0}/{1}", Assembly, Namespace); } }
		public string TypeUrl { get { return string.Format ("~/{0}/{1}.{2}", Assembly, Namespace, ParentType); } }
		public string MembersUrl { get { return string.Format ("~/{0}/{1}.{2}/Members", Assembly, Namespace, ParentType); } }
		public string MemberUrl { get { return string.Format ("~/{0}/{1}.{2}.{3}", Assembly, Namespace, ParentType, FormattedSignature.TrimStart ('.')); } }

		public string MemberIcon {
			get {
				string value = string.Empty;

				if (Type == "Field" && Visibility == null)
					value = "pub";	// Generally an Enum member
				else {
					switch (Visibility.ToLowerInvariant ()) {
						case "public": value = "pub"; break;
						case "protected": value = "prot"; break;
						case "private": value = "priv"; break;
					}
				}

				string value2 = string.Empty;

				switch (Type.ToLowerInvariant ()) {
					case "constructor" : value2 = "method"; break;
					default: value2 = Type.ToLowerInvariant (); break;
				}

				return string.Format ("{0}{1}", value, value2);
			}
		}

		public override string LongName { get { return string.Format ("{0}.{1}.{2}", Namespace, ParentType, FormattedSignature); } }
		public override string Icon { get { return string.Format ("~/Images/Types/{0}.png", MemberIcon); } }
		public override string Url { get { return MemberUrl; } }
	}
}