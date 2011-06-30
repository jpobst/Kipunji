<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class="right-content-pad">
     <% if (ViewData["Message"] != null) { %>
	<div style="border: solid 1px red">
		<%= Html.Encode(ViewData["Message"].ToString())%>
	</div>
	<% } %>
     <p class="indent">
     	Sorry, you are not in the list of approved editors.  
     	Please send the gmail or yahoo email address you would 
     	like to use for editing to <a href="miguel@novell.com">miguel@novell.com</a> 
     	to be added to the list.
     </p>
	
    </div>

</asp:Content>
