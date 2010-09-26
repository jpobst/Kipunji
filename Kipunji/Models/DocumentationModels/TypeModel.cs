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
using System.Linq;

namespace Kipunji.Models
{
	public class TypeModel : BaseDocModel
	{
		public string ParentClass { get; set; }
		public string Namespace { get; set; }
		public string BaseType { get; set; }
		public List<string> Interfaces { get; private set; }
		public AssemblyInfo AssemblyInfo { get; set; }
		public Signature Signature { get; set; }
		public List<MemberModel> Members { get; private set; }
		public string Kind { get; set; }
		public string Visibility { get; set; }

		public TypeModel ()
		{
			Interfaces = new List<string> ();
			Members = new List<MemberModel> ();
		}

		public string FormattedBaseType {
			get { return Formatter.FormatClassSignature (Name, BaseType, Interfaces); }
		}

		public string FormattedSummary {
			get { return Formatter.FormatHtml (Summary); }
		}

		public string FormattedReturnType {
			get { return Formatter.FormatType (BaseType); }
		}

		public string FormattedRemarks {
			get { return Formatter.FormatHtml (Remarks.TrimEnd ('\n')); }
		}

		public string FormattedVersionInfo {
			get {
				string ret = "This class is available in: {0}";
				string ver = string.Empty;

				foreach (var ai in AssemblyInfo.Versions)
					ver += string.Format ("{0}, ", ai);

				ver = ver.TrimEnd (',', ' ');

				return string.Format (ret, ver); 
			}
		}

		public override string ToString ()
		{
			return Name;
		}

		public string NamespaceUrl { get { return string.Format ("~/{0}", Namespace); } }
		public string TypeUrl { get { return string.Format ("~/{0}.{1}", Namespace, Name); } }
		public string MembersUrl { get { return string.Format ("~/{0}.{1}/Members", Namespace, Name); } }

		public string TypeIcon {
			get {
				string value = string.Empty;

				switch (Visibility.ToLowerInvariant ()) {
					case "public": value = "pub"; break;
					case "protected": value = "prot"; break;
					case "private": value = "priv"; break;
				}

				return string.Format ("{0}{1}", value, Kind.ToLowerInvariant ());
			}
		}

		public List<BaseDocModel> FindMembers (string signature)
		{
			List<BaseDocModel> results = new List<BaseDocModel> ();

			signature = signature.Trim ();

			// If it's just a member name, take the easy path
			if (!signature.Contains ('(')) {
				results.AddRange (Members.Where (p => p.Name.TrimStart ('.') == signature).Cast<BaseDocModel> ());
				return results;
			}

			string member_name = signature.Substring (0, signature.IndexOf ('(')).Trim ();
			string parameters = signature.Substring (signature.IndexOf ('(')).Trim ('(', ')', ' ');

			// Empty parameter list, another short circuit path
			if (parameters.Length == 0) {
				results.AddRange (Members.Where (p => p.Name.TrimStart ('.') == member_name && p.Parameters.Count == 0).Cast<BaseDocModel> ());
				return results;
			}

			// The hard path, we have to check the types of each of the parameters
			string[] paras = parameters.Split (',');

			foreach (var member in Members.Where (p => p.Name.TrimStart ('.') == member_name && p.Parameters.Count == paras.Length)) {
				bool match = true;

				for (int i = 0; i < paras.Length; i++) {
					string req_param = Formatter.FormatType (paras[i].Trim ());
					string member_param = Formatter.FormatType (member.Parameters[i].Type.Trim ());

					if (string.Compare (req_param, member_param, true) != 0) {
						match = false;
						break;
					}
				}

				// We assume there cannot be more than one match
				if (match) {
					results.Add (member);
					return results;
				}

			}

			return results;

		}

		public override string LongName { get { return string.Format ("{0}.{1}", Namespace, Name); } }
		public override string Icon { get { return string.Format ("{0}.png", TypeIcon); } }
		public override string Url { get { return string.Format ("~/{0}.{1}", Namespace, Name); } }
	}
}