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
using System.Linq;
using System.Xml;

namespace Kipunji.Models
{
	public class Signature
	{
		public string Language { get; set; }
		public string Value { get; set; }

		public Signature ()
		{
		}

		public Signature (XmlElement elem)
		{
			Language = elem.GetAttribute ("Language");
			Value = elem.GetAttribute ("Value");
		}

		public override string ToString ()
		{
			return string.Format ("{0}", Value);
		}

		public string Visibility {
			get {
				if (string.IsNullOrEmpty (Value))
					return null;

				// Generally for an Enum member
				if (!Value.Contains (' '))
					return null;

				return Value.Substring (0, Value.IndexOf (' ')).Trim ();
			}
		}

		public bool IsExplicitInterface {
			get {
				string[] pieces = Value.Split (' ');

				bool res;
				if (pieces.Length < 1)
					return false;

				switch (pieces [0]) {
				case "public":
				case "protected":
				case "private":
					res = false;
					break;
				default:
					// No visibility modifier means explicit interface...I think, will CWL these for now to see
					// if there is anything suspicious
					res = true;
					break;
				}
				
				return res;
			}
		}
		
		public string TypeKind {
			get {
				if (string.IsNullOrEmpty (Value))
					return null;

				string[] pieces = Value.Split (' ');

				if (pieces.Length < 2)
					return null;

				switch (pieces [0]) {
				case "public":
				case "protected":
				case "private":
					break;
				default:
					// No visibility modifier means explicit interface, so we return null and let it
					// use the MemberType value.
					// CWL it so we can easily track down anything fishy.
					return null;
				}
				
				string kind = pieces[1];

				if (kind == "static" || kind == "abstract" || kind == "sealed")
					kind = pieces[2];

				switch (kind) {
					case "enum": return "Enumeration";
					case "class": return "Class";
					case "struct": return "Structure";
					case "interface": return "Interface";
					
					default: 
						return kind;
				}
			}
		}
	}
}