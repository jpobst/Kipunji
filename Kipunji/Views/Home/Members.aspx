<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Kipunji.Models.TypeModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial ("BreadCrumbBar", ViewData["BreadCrumb"]); %>
        <div class="method-header">
            <%= Html.Encode (string.Format ("{0} {1} Members", Model.Name, Model.Kind)) %>
        </div>
     <div class="right-content-pad">
       
        <p class="indent"><%= Model.FormattedSummary %></p>

        <% if (Model.Members.Where (p => p.Type == "Constructor").Count () > 0) { %>
        <h2>Constructors</h2>

        <table class="member-list indent">
        <% foreach (var prop in Model.Members.Where (p => p.Type == "Constructor")) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl (prop.Icon) %>" /></td>
            <td style="width: 400px;"><a href="<%= Html.ResolveUrl (prop.MemberUrl) %>"><%= prop.FormattedSignature%></a></td>
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
            <td style="width: 400px;"><a href="<%= Html.ResolveUrl (prop.MemberUrl) %>"><%= prop.FormattedSignature%></a></td>
            <td><%= prop.FormattedSummary %></td>
        </tr>
        <% } %>
        </table>
        <% } %>

        <% if (Model.Members.Where (p => p.Type == "Method").Count () > 0) { %>
        <h2>Methods</h2>

        <table class="member-list indent">
        <% foreach (var prop in Model.Members.Where (p => p.Type == "Method")) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl (prop.Icon) %>" /></td>
            <td style="width: 400px;"><a href="<%= Html.ResolveUrl (prop.MemberUrl) %>"><%= prop.FormattedSignature%></a></td>
            <td><%= prop.FormattedSummary %></td>
        </tr>
        <% } %>
        </table>
        <% } %>

        <% if (Model.Members.Where (p => p.Type == "Property").Count () > 0) { %>
        <h2>Properties</h2>

        <table class="member-list indent">
        <% foreach (var prop in Model.Members.Where (p => p.Type == "Property")) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl (prop.Icon) %>" /></td>
            <td style="width: 400px;"><a href="<%= Html.ResolveUrl (prop.MemberUrl) %>"><%= prop.FormattedSignature%></a></td>
            <td><%= prop.FormattedSummary %></td>
        </tr>
        <% } %>
        </table>
        <% } %>
   </div>

</asp:Content>
