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
using System.Web.Mvc;
using Kipunji.Models;

namespace Kipunji.Controllers
{
	[HandleError]
	public class HomeController : Controller
	{
		private string title = "Mono API Reference";
		
		// This is basically "search", as the user didn't give us an assembly
		public ActionResult Index (string path)
		{	
			// For root requests, return the home index page
			if (string.IsNullOrEmpty (path))
				return Namespaces ();

			// Decode the user's request
			var reqs = RequestDecoder.Parse (path, true);

			// We couldn't find anything that matched the user's request
			if (reqs.Count == 0)
				return UnrecognizedRequest (path);

			// We aren't sure what the user wants,
			// return a disambiguation page
			if (reqs.Count > 1)
				return Disambiguation (reqs);

			var req = reqs[0];
			
			// Return the correct page for the request type
			if (req is NamespaceModel)
				return Namespace ((NamespaceModel)req);
			if (req is TypeModel)
				return Type ((TypeModel)req);
			if (req is MemberModel)
				return Member ((MemberModel)req);

			// This should never be hit, but just in case
			return UnrecognizedRequest (path);
		}

		// The user has given us a type ("System.String"), and
		// wants to see the members on the type
		// ex: http://localhost/mscorlib.dll/System.String/Members
		public ActionResult Members (string type_path)
		{
			var types = RequestDecoder.Parse (type_path, true);
			
			if (types.Count > 1)
				return Disambiguation (types);
			
			if (types.Count < 1)
				return UnrecognizedRequest (type_path);

			// Make a complete type, not a shallow one
			var type = (TypeModel) types [0];

			// Build the breadcrumb menu
			BreadCrumb bc = new BreadCrumb ();

			bc.Crumbs.Add (new Crumb ("Home", "~", "home"));
			bc.Crumbs.Add (new Crumb (type.Namespace, type.NamespaceUrl, "namespace"));
			bc.Crumbs.Add (new Crumb (type.Name, type.TypeUrl, type.TypeIcon));
			bc.Crumbs.Add (new Crumb ("Members", null, "members"));

			ViewData["BreadCrumb"] = bc;
			ViewData["Title"] = string.Format ("{0} Members", type.DisplayName);

			return View ("Members", type);
		}
		
		// The user has not requested anything, show the
		// list of assemblies we provide documentation for
		// ex: http://localhost/
		private ActionResult Namespaces ()
		{
			List<NamespaceModel> index;

			if (Request.QueryString ["display_all"] != null)
			     index = ModelFactory.GetIndex ();
			else
		             index = ModelFactory.GetDisplayIndex ();

			BreadCrumb bc = new BreadCrumb ();
			bc.Crumbs.Add (new Crumb ("Home", "", "home"));

			ViewData["BreadCrumb"] = bc;
			ViewData["Title"] = title;

			return View ("Index", index);
		}

		// The user has passed us a namespace ("System"),
		// show the types in that namespace
		// ex: http://localhost/mscorlib.dll/System
		private ActionResult Namespace (NamespaceModel ns)
		{
			// Build the breadcrumb menu
			BreadCrumb bc = new BreadCrumb ();

			bc.Crumbs.Add (new Crumb ("Home", "~", "home"));
			bc.Crumbs.Add (new Crumb (ns.Name, null, "namespace"));

			ViewData["BreadCrumb"] = bc;
			ViewData["Title"] = string.Format ("{0} Namespace", ns.Name);

			return View ("Namespace", ns);
		}

		// The user has passed us a type ("System.String"),
		// show the documentation of the type
		// ex: http://localhost/mscorlib.dll/System.String
		private ActionResult Type (TypeModel type)
		{
			// Build the breadcrumb menu
			BreadCrumb bc = new BreadCrumb ();

			bc.Crumbs.Add (new Crumb ("Home", "~", "home"));
			bc.Crumbs.Add (new Crumb (type.Namespace, type.NamespaceUrl, "namespace"));
			bc.Crumbs.Add (new Crumb (type.Name, null, type.TypeIcon));

			ViewData["BreadCrumb"] = bc;
			ViewData["Title"] = string.Format ("{0} Type", type.DisplayName);

			return View ("Type", type);
		}

		// The user has passed us a type member ("System.String.Trim ()"),
		// show the documentation of the member
		// ex: http://localhost/mscorlib.dll/System.String.Trim
		private ActionResult Member (MemberModel member)
		{
			// Build the breadcrumb menu
			BreadCrumb bc = new BreadCrumb ();
			
			bc.Crumbs.Add (new Crumb ("Home", "~", "home"));
			bc.Crumbs.Add (new Crumb (member.Namespace, member.NamespaceUrl, "namespace"));
			bc.Crumbs.Add (new Crumb (member.ParentType.DisplayName, member.TypeUrl, "pubclass"));
			bc.Crumbs.Add (new Crumb ("Members", member.MembersUrl, "members"));
			bc.Crumbs.Add (new Crumb (member.FormattedDisplaySignature, null, member.MemberIcon));

			ViewData["BreadCrumb"] = bc;
			ViewData["Title"] = string.Format ("{0}.{1}", member.ParentType.DisplayName, member.FormattedDisplaySignature);

			return View ("Member", member);
		}

		// The user hasn't given us enough information to narrow down
		// their request to a specific namespace/type/member.  Show a
		// disambiguation page for them to choose from.
		private ActionResult Disambiguation (List<BaseDocModel> options)
		{
			// Build the breadcrumb menu
			BreadCrumb bc = new BreadCrumb ();

			bc.Crumbs.Add (new Crumb ("Home", "~", "home"));
			bc.Crumbs.Add (new Crumb ("Disambiguation", null, "help"));

			ViewData["BreadCrumb"] = bc;
			ViewData["Title"] = title;

			return View ("Disambiguation", options);
		}

		// We could not find anything that matched the user's request
		private ActionResult UnrecognizedRequest (string request)
		{
			// Build the breadcrumb menu
			BreadCrumb bc = new BreadCrumb ();

			bc.Crumbs.Add (new Crumb ("Home", "~", "home"));
			bc.Crumbs.Add (new Crumb ("Unrecognized Request", null, "unrecognized"));

			ViewData["BreadCrumb"] = bc;
			ViewData["Title"] = string.Format ("{0} - Unrecognized Request", title);

			return View ("UnrecognizedRequest", (object)request);
		}
	}
}
