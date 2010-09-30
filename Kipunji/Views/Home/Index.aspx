<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Kipunji.Models.AssemblyModel>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial ("BreadCrumbBar", ViewData["BreadCrumb"]); %>
        <div class="method-header">
            <%= Html.Encode ("Documented Assemblies") %>
        </div>
     <div class="right-content-pad">
     <p class="indent">Reference API documentation is available for the following assemblies provided
     by the <a href="http://www.mono-project.com">Mono Project</a>.</p>
        <table class="member-list indent">
        <% foreach (var prop in Model) { %>
        <tr>
            <td><img src="<%= Html.ResolveUrl ("~/Images/Types/reference.png") %>" /></td>
            <td style="width: 200px;"><a href="<%= Html.ResolveUrl (prop.AssemblyUrl) %>"><%= prop.Name %></a></td>
            <td><%= prop.Summary %></td>
        </tr>
        <% } %>
        </table>
    </div>

</asp:Content>
