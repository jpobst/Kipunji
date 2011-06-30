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
	public class NamespaceModel : BaseDocModel
	{
		public List<TypeModel> Types { get; private set; }
		public string Assembly { get; set; }
		public bool DisplayInIndex { get; set; }

		public NamespaceModel ()
		{
			Types = new List<TypeModel> ();
		}

		public NamespaceModel (string name, string summary) : this ()
		{
			Name = name;
			Summary = summary;
		}

		public string NamespaceUrl { get { return string.Format ("~/{0}", Name); } }

		public override string LongName { get { return Name; } }
		public override string Icon { get { return "namespace.png"; } }
		public override string Url { get { return NamespaceUrl; } }

		public List<BaseDocModel> ParseRequest (string path)
		{
			List<BaseDocModel> results = new List<BaseDocModel> ();
		
			if (!path.StartsWith (Name) || path.Length <= Name.Length + 1)
				return results;
			
			string remainder = path.Substring (Name.Length + 1);
			if (remainder.Length < 1)
				return results;
			
			var type = Types.Where (p=> String.Compare (remainder, p.Name, true) == 0).FirstOrDefault ();
			
			if (type != null) {
				results.Add (type);
				return results;
			}
			
			var matches = Types.Where (p => remainder.StartsWith (p.Name, StringComparison.InvariantCultureIgnoreCase));
			
			foreach (var match in matches) {
				string signature = remainder.Substring (match.Name.Length + 1);
				results.AddRange (match.FindMembers (signature));
			}
			
			return results;
			/*
			// Look for an exact match
			var ns = Types.Where (p => string.Compare (p.Name, path, true) == 0).FirstOrDefault ();

			// We build a new one, as the index is shallow
			if (ns != null) {
				results.Add (ModelFactory.CreateType (Name, path, true));
				return results;
			}

			// Not an exact match, try breaking into pieces and looking for members
			foreach (string piece in ParsePath (path)) {
				Console.WriteLine ("piece:  '{0}'", piece);
				var type = Types.Where (p => string.Compare (p.Name, piece, true) == 0).FirstOrDefault ();

				if (type != null) {
					// Index is shallow, load members
					type = ModelFactory.CreateType (Name, type.Name, false);
					results.AddRange (type.FindMembers (RemovePiece (path, piece)));
				}
			}

			return results;
			*/
		}
	}
}