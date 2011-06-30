﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated && Session ["FriendlyName"] != null) {
%>
        [ <%= Html.ActionLink("Log Off", "Logout", "User") %> ]
<%
    }
    else {
%> 
        [ <%= Html.ActionLink("Log On", "Login", "User", new { returnUrl = Request.Path }, null) %> ]
<%
    }
%>

