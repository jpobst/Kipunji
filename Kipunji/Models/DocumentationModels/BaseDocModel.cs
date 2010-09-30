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
	public abstract class BaseDocModel
	{
		public virtual string Name { get; set; }
		public string Summary { get; set; }
		public string Remarks { get; set; }

		public abstract string LongName { get; }
		public abstract string Icon { get; }
		public abstract string Url { get; }

		protected BaseDocModel ()
		{
			Summary = string.Empty;
			Remarks = string.Empty;
		}

		// This is going to convert a request down to all possible paths:
		// System.Collections.Generic.List
		//  - System.Collections.Generic.List
		//  - System.Collections.Generic
		//  - System.Collections
		//  - System
		protected List<string> ParsePath (string path)
		{
			//// Chop off any method parameters for now
			//if (path.Contains ('('))
			//        path = path.Substring (0, path.IndexOf ('(')).Trim ();

			string[] pieces = path.Split ('.');

			List<string> paths = new List<string> ();
			string current = string.Empty;

			foreach (var p in pieces) {
				current = string.Format ("{0}.{1}", current, p);
				current = current.TrimStart ('.');

				paths.Add (current);
			}

			paths.Reverse ();

			return paths;
		}

		// Removes a leading namespace/type piece from a path:
		// System.String - System = String
		protected string RemovePiece (string path, string piece)
		{
			path = path.Substring (piece.Length);
			path = path.TrimStart ('.');

			return path;
		}
	}
}