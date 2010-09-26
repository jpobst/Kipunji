//
// Authors:
// Jonathan Pobst (monkey@jpobst.com)
//
// Copyright (C) 2009 Jonathan Pobst
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
	public class BreadCrumb
	{
		public List<Crumb> Crumbs { get; private set; }
		
		public BreadCrumb ()
		{
			Crumbs = new List<Crumb> ();
		}
	}
	
	public class Crumb
	{
		public string Text { get; set; }
		public string Url { get; set; }
		public string CssClass { get; set; }

		public Crumb ()
		{
		}

		public Crumb (string text, string url)
		{
			Text = text;
			Url = url;
		}

		public Crumb (string text)
		{
			Text = text;
		}

		public Crumb (string text, string url, string cssClass)
		{
			Text = text;
			Url = url;
			CssClass = cssClass;
		}
	}
}
