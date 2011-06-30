<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Kipunji.Models.BaseDocModel>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class="right-content-pad">
     <p class="indent">Your request matched more than one page.  Please choose your desired page:</p>
        <table class="member-list indent">
        <% foreach (var prop in Model) { %>
        <tr>
            <td style="width: 16px;"><img src="<%= Html.ResolveUrl (prop.Icon) %>" /></td>
            <td><a href="<%= Html.ResolveUrl (prop.Url) %>"><%= prop.LongName %></a></td>
            <td><a href="<%= Html.ResolveUrl (prop.Url) %>"><%= ((Kipunji.Models.TypeModel) prop).Assembly %></a></td>
        </tr>
       <% } %>
        </table>
    </div>

</asp:Content>
