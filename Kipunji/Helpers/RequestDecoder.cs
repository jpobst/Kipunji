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
using Kipunji.Models;

namespace Kipunji
{
	public static class RequestDecoder
	{
		private static Dictionary<string, List<string>> namespaces;

		static RequestDecoder ()
		{
			namespaces = ModelFactory.GetIndex ();
		}

		public static List<BaseDocModel> Parse (string path, bool shallow)
		{
			List<BaseDocModel> results = new List<BaseDocModel> ();

			if (path == null)
				return results;

			string full_path = path;

			// Chop off any method parameters for now
			if (path.Contains ('('))
				path = path.Substring (0, path.IndexOf ('(')).Trim ();

			// The request exactly matches a namespace, like:
			// System.Collections.Generic
			if (namespaces.ContainsKey (path))
				results.Add (ModelFactory.CreateNamespace (path));

			var paths = ParsePath (path);
			
			// Look at each piece of the request to see if it's a namespace
			// If it is, try the whole thing as a type
			foreach (var piece in paths) {
				if (namespaces.ContainsKey (piece)) {
					var types = namespaces[piece];

					if (types.Contains (path))
						results.Add (ModelFactory.CreateType (piece, RemovePiece (path, piece), shallow));
				}
			}

			if (!path.Contains ('.'))
				return results;

			// Try chopping off a member and see if we
			// we recognize the type
			var member = path.Substring (path.LastIndexOf ('.') + 1);
			var full_member = full_path.Substring (path.LastIndexOf ('.') + 1);
			var no_member_path = path.Substring (0, path.Length - member.Length - 1);

			paths = ParsePath (no_member_path);

			foreach (var piece in paths) {
				if (namespaces.ContainsKey (piece)) {
					var types = namespaces[piece];

					// We found a type, check it for the member
					if (types.Contains (no_member_path)) {
						var type = ModelFactory.CreateType (piece, RemovePiece (no_member_path, piece), false);

						results.AddRange (type.FindMembers (full_member));
					}
				}
			}


			return results;
		}

		public static TypeModel ParseType (string path)
		{
			if (path == null)
				return null;

			var paths = ParsePath (path);

			// Look at each piece of the request to see if it's a namespace
			// If it is, try the whole thing as a type
			foreach (var piece in paths) {
				if (namespaces.ContainsKey (piece)) {
					var types = namespaces[piece];

					if (types.Contains (path))
						return ModelFactory.CreateType (piece, RemovePiece (path, piece), false);
				}
			}

			return null;
		}

		// This is going to convert a request down to all possible paths:
		// System.Collections.Generic.List
		//  - System.Collections.Generic.List
		//  - System.Collections.Generic
		//  - System.Collections
		//  - System
		private static List<string> ParsePath (string path)
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
		private static string RemovePiece (string path, string piece)
		{
			path = path.Substring (piece.Length);
			path = path.TrimStart ('.');

			return path;
		}
	}
}