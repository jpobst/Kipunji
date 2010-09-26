<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<string>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial ("BreadCrumbBar", ViewData["BreadCrumb"]); %>
        <div class="method-header">
            <%= Html.Encode ("Unrecognized Request")%>
        </div>
     <div class="right-content-pad">
     <p class="indent">No page could be found that satified your request:</p>
     <p class="indent" style="color: Red;"><%= Html.Encode (Model) %></p>
    </div>

</asp:Content>
