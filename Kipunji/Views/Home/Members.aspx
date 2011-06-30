﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Kipunji.Models.TypeModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

     <div class="right-content-pad">
       
        <p class="indent">The <a href="<%= Html.ResolveUrl (Model.TypeUrl) %>"><%= Model.Name %></a> <%= Model.Kind %> exposes the following members.</p>
		
        <% if (Model.Members.Where (p => p.Type == "Constructor").Count () > 0) { %>
        <h2>Constructors</h2>

        <table class="member-list indent">
        <% foreach (var prop in Model.Members.Where (p => p.Type == "Constructor")) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl (prop.Icon) %>" /></td>
            <td style="width: 400px;"><a href="<%= Html.ResolveUrl (prop.MemberUrl) %>"><%= prop.FormattedDisplaySignatureWithParams %></a></td>
            <td><%= prop.FormattedSummary %></td>
        </tr>
        <% } %>
        </table>
        <% } %>

        <% if (Model.Members.Where (p => p.Type == "Field").Count () > 0) { %>
        <h2>Fields</h2>

        <table class="member-list indent">
        <% foreach (var prop in Model.Members.Where (p => p.Type == "Field")) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl (prop.Icon) %>" /></td>
            <td style="width: 400px;"><a href="<%= Html.ResolveUrl (prop.MemberUrl) %>"><%= prop.FormattedDisplaySignatureWithParams %></a></td>
            <td><span class="summary"><%= prop.FormattedSummary %></span></td>
        </tr>
        <% } %>
        </table>
        <% } %>

        <% if (Model.Members.Where (p => p.Type == "Method" && !p.Signature.IsExplicitInterface).Count () > 0) { %>
        <h2>Methods</h2>

        <table class="member-list indent">
        <% foreach (var prop in Model.Members.Where (p => p.Type == "Method" && !p.Signature.IsExplicitInterface)) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl (prop.Icon) %>" /></td>
            <td class="member-list-signature"><a href="<%= Html.ResolveUrl (prop.MemberUrl) %>"><%= prop.FormattedDisplaySignatureWithParams %></a></td>
            <td><span class="summary"><%= prop.FormattedSummary %></span></td>
        </tr>
        <% } %>
        </table>
        <% } %>

        <% if (Model.Members.Where (p => p.Type == "Property" && !p.Signature.IsExplicitInterface).Count () > 0) { %>
        <h2>Properties</h2>

        <table class="member-list indent">
        <% foreach (var prop in Model.Members.Where (p => p.Type == "Property"  && !p.Signature.IsExplicitInterface)) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl (prop.Icon) %>" /></td>
            <td style="width: 400px;"><a href="<%= Html.ResolveUrl (prop.MemberUrl) %>"><%= prop.FormattedDisplaySignatureWithParams %></a></td>
            <td><span class="summary"><%= prop.FormattedSummary %></span></td>
        </tr>
        <% } %>
        </table>
        <% } %>
        
        <%--
        <% if (Model.Members.Where (p => p.Type == "Method" && p.Signature.IsExplicitInterface).Count () > 0) { %>
        <h2>Explicit Interface Methods</h2>

        <table class="member-list indent">
        <% foreach (var prop in Model.Members.Where (p => p.Type == "Method" && p.Signature.IsExplicitInterface)) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl (prop.Icon) %>" /></td>
            <td class="member-list-signature"><a href="<%= Html.ResolveUrl (prop.MemberUrl) %>"><%= prop.FormattedDisplaySignatureWithParams %></a></td>
            <td><%= prop.FormattedSummary %></td>
        </tr>
        <% } %>
        </table>
        <% } %>
        
        <% if (Model.Members.Where (p => p.Type == "Property" && p.Signature.IsExplicitInterface).Count () > 0) { %>
        <h2>Explicit Interface Properties</h2>

        <table class="member-list indent">
        <% foreach (var prop in Model.Members.Where (p => p.Type == "Property" && p.Signature.IsExplicitInterface)) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl (prop.Icon) %>" /></td>
            <td style="width: 400px;"><a href="<%= Html.ResolveUrl (prop.MemberUrl) %>"><%= prop.FormattedDisplaySignatureWithParams %></a></td>
            <td><%= prop.FormattedSummary %></td>
        </tr>
        <% } %>
        </table>
        <% } %>
        --%>
   </div>

</asp:Content>
