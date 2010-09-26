<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Kipunji.Models.BreadCrumb>" %>

<%--
This is what we are building...

<ul class="breadcrumb">
  <li><a href="/MonkeyWrench/builds">Projects</a></li>
  <li>Mono (Trunk)</li>
</ul>
--%>


<% Kipunji.Models.BreadCrumb breadcrumb = (Kipunji.Models.BreadCrumb)Model; %>

<ul class="breadcrumb">
<% foreach (Kipunji.Models.Crumb crumb in breadcrumb.Crumbs) { %>
<%   if (!string.IsNullOrEmpty (crumb.Url)) { %>
  <li <%= string.IsNullOrEmpty (crumb.CssClass) ? "" : string.Format ("class=\"{0}\"", crumb.CssClass) %>><a href="<%= Html.ResolveUrl (crumb.Url) %>"><%= Html.Encode (crumb.Text)%></a></li>
<%   } else { %>
  <li <%= string.IsNullOrEmpty (crumb.CssClass) ? "" : string.Format ("class=\"{0}\"", crumb.CssClass) %>><%= Html.Encode (crumb.Text)%></li>
<%   } %>
<% } %>
</ul>
