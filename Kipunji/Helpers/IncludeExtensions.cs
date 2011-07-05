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
using System.Web;
using System.Web.Mvc;
using Kipunji;

public static class IncludeExtensions
{
	public static MvcHtmlString IncludeCSS (this HtmlHelper html, string cssFile)
	{
		string cssPath = cssFile.Contains ("~") ? cssFile : ("~/Content/" + cssFile);
		string url = html.ResolveUrl (cssPath);
		return new MvcHtmlString (string.Format ("<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}\" />\n", url));
	}

	public static MvcHtmlString IncludeJS (this HtmlHelper html, string jsFile)
	{
		string jsPath = jsFile.Contains ("~") ? jsFile : ("~/Scripts/" + jsFile);
		string url = html.ResolveUrl (jsPath);
		return new MvcHtmlString (string.Format ("<script type=\"text/javascript\" src=\"{0}\"></script>\n", url));
	}

	public static MvcHtmlString IncludeFavicon (this HtmlHelper html, string icon)
	{
		string iconPath = icon.Contains ("~") ? icon : ("~/" + icon);
		string url = html.ResolveUrl (iconPath);
		return new MvcHtmlString (string.Format ("<link rel=\"shortcut icon\" href=\"{0}\" />\n", url));
	}

	public static MvcHtmlString IncludeScript (this HtmlHelper html, string script)
	{
		return new MvcHtmlString (string.Format ("<script type=\"text/javascript\">{0}</script>\n", script));
	}

	public static string ResolveUrl (this HtmlHelper html, string relativeUrl)
	{
		if (relativeUrl == null) { return null; }
		if (!relativeUrl.StartsWith ("~")) { return relativeUrl; }

		return (html.ViewContext.HttpContext.Request.ApplicationPath + relativeUrl.Substring (1)).Replace ("//", "/");
	}

	public static string GetImage (this HtmlHelper html, string image)
	{
		return (html.ResolveUrl (string.Format ("~/Media/{0}", image)));
	}

	public static string GetImage (this HtmlHelper html, bool conditional, string trueImage, string falseImage)
	{
		if (conditional)
			return html.GetImage (trueImage);
		else
			return html.GetImage (falseImage);
	}

	public static string ToFriendlyAge (this DateTime dt)
	{
		TimeSpan diff = DateTime.Now - dt;

		if (diff.TotalDays >= 365)
			return Plural ("{0} {1}", "year", diff.Days / 365);
		if (diff.TotalDays >= 60)
			return Plural ("{0} {1}", "month", diff.Days / 30);
		if (diff.TotalDays >= 14)
			return Plural ("{0} {1}", "week", diff.Days / 7);
		if (diff.TotalDays >= 1)
			return Plural ("{0} {1}", "day", diff.Days);
		if (diff.TotalHours >= 1)
			return Plural ("{0} {1}", "hour", (int)diff.TotalHours);
		if (diff.TotalMinutes >= 1)
			return Plural ("{0} {1}", "minute", (int)diff.TotalMinutes);

		return Plural ("{0} {1}", "second", diff.Seconds);
	}

	private static string Plural (string format, string singular, int value)
	{
		if (value == 1)
			return string.Format (format, value, singular);

		return string.Format (format + "s", value, singular);
	}
	
	public static string ToFriendlySpan (this TimeSpan diff)
	{
		if (diff.TotalDays >= 365)
			return Plural ("{0} {1}", "year", diff.Days / 365);
		if (diff.TotalDays >= 60)
			return Plural ("{0} {1}", "month", diff.Days / 30);
		if (diff.TotalDays >= 14)
			return Plural ("{0} {1}", "week", diff.Days / 7);
		if (diff.TotalDays >= 1)
			return Plural ("{0} {1}", "day", diff.Days);
		if (diff.TotalHours >= 1)
			return Plural ("{0} {1}", "hour", (int)diff.TotalHours);
		if (diff.TotalMinutes >= 1)
			return Plural ("{0} {1}", "minute", (int)diff.TotalMinutes);

		return Plural ("{0} {1}", "second", diff.Seconds);
	}

	public static string ToHoursMinSec (this TimeSpan diff)
	{
		return string.Format ("{0:00}:{1:00}:{2:00}", diff.Hours, diff.Minutes, diff.Seconds);
	}

	/// <summary>
	/// Create a member link and optionally a return type link: "Substring (int start, int end) : string"
	/// </summary>
	public static MvcHtmlString FormatMemberListDisplay (this HtmlHelper html, Kipunji.Models.MemberModel member)
	{
		// The link for the member signature
		var member_tag = new TagBuilder ("a");

		member_tag.MergeAttribute ("href", html.ResolveUrl (member.MemberUrl));
		member_tag.InnerHtml = member.FormattedDisplaySignatureWithParams.ToString ();

		// If we have a return type, add it
		if (!string.IsNullOrWhiteSpace (member.ReturnType) && member.ReturnType != "System.Void") {
			var return_link = Formatter.CreateTypeLink (VirtualPathUtility.ToAbsolute ("~/"), member.ReturnType).ToString ();

			// We want to replace namespaced stuff with just a type name:
			// System.String -> String
			if (return_link.Contains ("<")) {
				int close = return_link.IndexOf ('>');
				int open = return_link.IndexOf ('<', close);

				return_link = return_link.Remove (close + 1, open - close - 1);
				return_link = return_link.Insert (close + 1, member.FormattedReturnType.ToString ());
			}

			return new MvcHtmlString (member_tag.ToString () + " : " + return_link);
		}

		return new MvcHtmlString (member_tag.ToString ());
	}
}
