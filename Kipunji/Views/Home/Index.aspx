<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Kipunji.Models.NamespaceModel>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class="right-content-pad">
     <p class="indent">Reference API documentation is available for the following namespaces provided
     by the <a href="http://www.mono-project.com">Mono Project</a>.</p>
        <table class="member-list indent">
        <% foreach (var prop in Model) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl ("~/Images/Types/pubnamespace.png") %>" /></td>
            <td style="width: 200px;"><a href="<%= Html.ResolveUrl (prop.NamespaceUrl) %>"><%= prop.Name %></a></td>
            <td><span class="summary"><%= prop.FormattedSummary %></span></td>
        </tr>
        <% } %>
        </table>
    </div>

</asp:Content>
