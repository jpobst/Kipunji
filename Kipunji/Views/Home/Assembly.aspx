<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Kipunji.Models.AssemblyModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial ("BreadCrumbBar", ViewData["BreadCrumb"]); %>
        <div class="method-header">
            <%= Html.Encode (string.Format ("{0} Namespaces", Model.Name)) %>
        </div>
     <div class="right-content-pad">
        <p class="indent"><%= Model.FormattedSummary %></p>
        <table class="member-list indent">
        <% foreach (var prop in Model.Namespaces) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl ("~/Images/Types/namespace.png") %>" /></td>
            <td style="width: 200px;"><a href="<%= Html.ResolveUrl (prop.NamespaceUrl) %>"><%= prop.Name %></a></td>
            <td><%= prop.Summary %></td>
        </tr>
        <% } %>
        </table>
    </div>

</asp:Content>
