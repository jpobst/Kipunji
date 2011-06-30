//
// Authors:
//  Jackson Harper (jackson@novell.com)
// 
//  Copyright (C) 2010 Jackson Harper
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
using System.Web;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace Kipunji.Controllers
{
	[HandleError]
	[ValidateInput (false)]
	public class EditController : Controller
	{
		XmlReaderSettings settings;
		
		private bool IsAuthenticated ()
		{
			return Request.IsAuthenticated && Session ["FriendlyName"] != null;
		}
		
		private bool GetParams (out string id, out string type)
		{
			id = Request.Form ["identifier"];
			type = Request.Form ["type"];
			
			if (String.IsNullOrEmpty (id) || String.IsNullOrEmpty (type))
				return false;
			
			id = VirtualPathUtility.MakeRelative ("~", id);
			return true;
		}
		
		private BaseDocModel GetDocModel (string id, out string error)
		{
			var reqs = RequestDecoder.Parse (id, true);

			// We couldn't find anything that matched the user's request
			if (reqs.Count == 0) {
				error = "Unable to parse identifier '" + id + "'";
				return null;
			}

			// We aren't sure what the user wants,
			// return a disambiguation page
			if (reqs.Count > 1) {
				error = "Identifier was not unique.";
				return null;
			}

			error = null;
			return reqs [0];
		}
		
		private string GetContent (BaseDocModel model, string id, string type, bool formatted, string text)
		{
			if (type == "summary" || type == "summary-type") {
				if (text != null)
					model.Summary = text;
				if (formatted)
					return model.FormattedSummary;
				return model.Summary;
			} else if (type == "remarks" || type == "remarks-type") {
				if (text != null)
					model.Remarks = text;
				if (formatted)
					return model.FormattedRemarks;
				return model.Remarks;
			} else if (type == "description-params") {
				string indexstr = Request.Form ["index"];
				int index;
				if (!Int32.TryParse (indexstr, out index) || index < 0) {
					Response.StatusCode = 500;
					Response.StatusDescription = "Invalid parameter index. It was not a positive integer.";
					return null;
				}
				MemberModel method_model = model as MemberModel;
				if (method_model == null) {
					Response.StatusCode = 500;
					Response.StatusDescription = "Invalid parameter. Model was not a member.";
					return null;
				}
				
				if (text != null)
					method_model.Parameters [index].Description = text;
				return method_model.Parameters [index].Description;
			} else if (type == "summary-return") {
				MemberModel method_model = model as MemberModel;
				if (method_model == null) {
					Response.StatusCode = 500;
					Response.StatusDescription = "Invalid return item. Model was not a member.";
					return null;
				}

				if (text != null)
					method_model.ReturnSummary = text;
				return method_model.ReturnSummary;
			} else if (type == "remarks-member") {
				MemberModel method_model = model as MemberModel;
				if (method_model == null) {
					Response.StatusCode = 500;
					Response.StatusDescription = "Invalid member remarks. Model was not a member.";
					return null;
				}
				
				if (text != null)
					method_model.Remarks = text;
				if (formatted)
					return method_model.FormattedRemarks;
				return method_model.Remarks;
			} else if (type == "summary-member") {
				MemberModel method_model = model as MemberModel;
				if (method_model == null) {
					Response.StatusCode = 500;
					Response.StatusDescription = "Invalid member remarks. Model was not a member.";
					return null;
				}
				
				if (text != null)
					method_model.Summary = text;
				if (formatted)
					return method_model.FormattedSummary;
				return method_model.Summary;
			} 
			
			Response.StatusCode = 500;
			Response.StatusDescription = String.Format ("Invalid type '{0}'. Must be summary or remarks.", type);	
			
			return null;
		}
		
		public ActionResult Data ()
		{
			if (!IsAuthenticated ()) {
				Response.StatusCode = 403;
				return Json (false);
			}
				
			string id = null;
			string type = null;
			
			if (!GetParams (out id, out type)) {
				Response.StatusCode = 500;
				return Json ("Missing id or type.");
			}
						
			string error;
			BaseDocModel model = GetDocModel (id, out error);
			if (model == null) {
				Response.StatusCode = 500;
				Response.StatusDescription = error;
				return Json (error);
			}
			
			string content = GetContent (model, id, type, false, null);
			if (content == null) {
				return Content (String.Empty);	
			}
			
			return Content (content);
		}
		
		
		public ActionResult Submit ()
		{
			if (!IsAuthenticated ()) {
				Response.StatusCode = 403;
				return Json (false);
			}
				
			Thread.Sleep (1000);
			
			string id = null;
			string type = null;
			string text = Request.Form ["text"];
			Dictionary<string,string> errordesc = new Dictionary<string, string> ();
			
			if (!GetParams (out id, out type)) {
				Response.StatusCode = 500;
				return Json ("Missing id or type.");
			}
			
			// Empty text is OK
			if (text == null) {
				Response.StatusCode = 500;
				return Json ("Missing text.");
			}

			if (!VerifyText (text, errordesc)) {
				Response.StatusCode = 500;
				return Json (errordesc);
			}
			
			string error;
			BaseDocModel model = GetDocModel (id, out error);
			if (model == null) {
				Response.StatusCode = 500;
				Response.StatusDescription = error;
				return Json (error);
			}
			
			string content = GetContent (model, id, type, true, text);
			if (content == null) {
				return Content (String.Empty);	
			}
			
			return Content (content);
		}
		
		private bool VerifyText (string text, Dictionary<string,string> errordesc)
		{		
			if (settings == null) {
				settings = new XmlReaderSettings ();
				settings.Schemas.Add (XmlSchema.Read (new StreamReader ("Resources/monodoc-ecma.xsd"), null));
				settings.Schemas.Compile ();
				settings.ValidationType = ValidationType.Schema;
			}
			
			try {
				using (var reader = XmlReader.Create (new StringReader ("<remarks>" + text + "</remarks>"), settings)) {
					while (reader.Read ()) {
						// do nothing
					}
				}
			} catch (XmlException e) {
				errordesc.Add ("error_line", e.LineNumber.ToString ());
				errordesc.Add ("error_pos", e.LinePosition.ToString ());
				errordesc.Add ("error", e.Message);
				return false;
			} catch (XmlSchemaValidationException se) {
				errordesc.Add ("error_line", se.LineNumber.ToString ());
				errordesc.Add ("error_pos", se.LinePosition.ToString ());
				errordesc.Add ("error", se.Message);
				return false;
			}
			
			return true;
		}
			
	}
}

