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
using Kipunji.Adapters;
using Kipunji.Models;

namespace Kipunji
{
	public static class ModelFactory
	{
		// TODO: This should create an adapter specified from a
		// config file, once we have more than MDoc support.
		private static BaseAdapter adapter = new MdocAdapter ();

		public static void Initialize (string docDirectory)
		{
			adapter.Initialize (docDirectory);
		}

		public static Dictionary<string, List<string>> GetIndex ()
		{
			return adapter.GetIndex ();
		}

		public static NamespaceModel CreateNamespace (string name)
		{
			return adapter.ReadNamespace (name);
		}

		public static TypeModel CreateType (string ns, string type, bool shallow)
		{
			return adapter.ReadType (ns, type, shallow);
		}

		public static MemberModel CreateMember (string ns, string type, string member)
		{
			return adapter.ReadMember (ns, type, member);
		}
	}
}