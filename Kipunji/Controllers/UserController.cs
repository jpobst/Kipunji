
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;
using Kipunji.Models;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;


namespace Kipunji
{
	public class UserController : Controller
	{
		private static OpenIdRelyingParty openid = new OpenIdRelyingParty ();

		public ActionResult Index ()
		{
			if (!User.Identity.IsAuthenticated) {
				Console.WriteLine ("Fuck you, you are not authenticated.");
				Response.Redirect ("/User/Login?ReturnUrl=Index");
			} else 
				Console.WriteLine ("Good news, you are authenticated");
			

			return View ("Index");
		}

		public ActionResult Logout ()
		{
			FormsAuthentication.SignOut ();
			Session ["FriendlyName"] = null;
			return Redirect ("/");
		}

		public ActionResult Login (string returnUrl)
		{		
			BreadCrumb bc = new BreadCrumb ();

			bc.Crumbs.Add (new Crumb ("Home", "~", "home"));
			bc.Crumbs.Add (new Crumb ("Login", null, "Login"));

			ViewData["BreadCrumb"] = bc;
			return View ("Login");
		}

		public ActionResult NotAnEditor ()
		{
			BreadCrumb bc = new BreadCrumb ();

			bc.Crumbs.Add (new Crumb ("Home", "~", "home"));
			bc.Crumbs.Add (new Crumb ("Not An Editor", null, "Not An Editor"));
			
			ViewData["BreadCrumb"] = bc;
			return View ("NotAnEditor");	
		}
		
		[ValidateInput(false)]
		public ActionResult Authenticate(string returnUrl) 
		{			
			BreadCrumb bc = new BreadCrumb ();

			bc.Crumbs.Add (new Crumb ("Home", "~", "home"));
			bc.Crumbs.Add (new Crumb ("Login", null, "Login"));

			ViewData["BreadCrumb"] = bc;
			
			var response = openid.GetResponse();
			if (response == null) {
				// Stage 2: user submitting Identifier
				Identifier id;
				if (Identifier.TryParse(Request.Form["openid_identifier"], out id)) {
					try {
						var r = openid.CreateRequest(Request.Form["openid_identifier"]);
						
						var ax = new FetchRequest();
						ax.Attributes.AddRequired("http://axschema.org/contact/email");
						ax.Attributes.AddRequired("http://axschema.org/namePerson");

						r.AddExtension(ax);
	
						return r.RedirectingResponse.AsActionResult();
					} catch (ProtocolException ex) {
						ViewData["Message"] = ex.Message;
						return View("Login");
					}
				} else {
					ViewData["Message"] = "Invalid identifier";
					return View("Login");
				}
			} else {
				// Stage 3: OpenID Provider sending assertion response
				switch (response.Status) {
					case AuthenticationStatus.Authenticated:			
						
						var fr = response.GetExtension<FetchResponse> ();
					
						if (fr == null || !fr.Attributes.Contains ("http://axschema.org/contact/email"))
							return Redirect ("NoEmailAddress");
						
						var emails = fr.Attributes ["http://axschema.org/contact/email"];
						var email = emails.Values.Where (e => ModelFactory.IsUserAnEditor (e)).FirstOrDefault ();
						if (email == null)
							return Redirect ("NotAnEditor");
						
						FormsAuthentication.SetAuthCookie(response.ClaimedIdentifier, false);
						Session ["FriendlyName"] = email;
						
						if (!string.IsNullOrEmpty(returnUrl)) {
							return Redirect(returnUrl);
						} else {
							return RedirectToAction("Index", "Home");
						}
					case AuthenticationStatus.Canceled:
						ViewData["Message"] = "Canceled at provider";
						return View("Login");
					case AuthenticationStatus.Failed:
						ViewData["Message"] = response.Exception.Message;
						return View("Login");
				}
			}
			return new EmptyResult();
		}
	}
}
