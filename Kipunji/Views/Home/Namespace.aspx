<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Kipunji.Models.NamespaceModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

     <div class="right-content-pad">
       
        <p class="indent"><%= Model.FormattedSummary %></p>
        
        <% if (Model.HasRemarks) { %>
	        <h2>Remarks</h2>
	        <p class="indent"><%= Model.FormattedRemarks%></p>
	    <% } %>

		<% 
			var classes = Model.Types.Where (p => p.Kind == "Class");
			if (classes.Count () > 0) { 
		%>
		        <h2>Types</h2>
		
		        <table class="member-list indent">
		        <% foreach (var prop in classes) { %>
		        <tr>
		            <td><img src="<%= Html.ResolveUrl ("~/Images/Types/pubclass.png") %>" /></td>
					<td style="width: 150px;"><a href="<%= Html.ResolveUrl (prop.TypeUrl) %>"><%= HttpUtility.HtmlEncode (prop.DisplayName) %></a></td>
		            <td><span class="summary"><%= prop.FormattedSummary %></span></td>
		        </tr>
		        <% } %>
		        </table>
	    <% } %>

		<%
			var structures = Model.Types.Where (p => p.Kind == "Structure");
			if (structures.Count () > 0) {
		%>
		        <h2>Structures</h2>
		
		        <table class="member-list indent">
		        <% foreach (var prop in structures) { %>
		        <tr>
		            <td><img src="<%= Html.ResolveUrl ("~/Images/Types/pubstructure.png") %>" /></td>
		            <td style="width: 150px;"><a href="<%= Html.ResolveUrl (prop.TypeUrl) %>"><%= HttpUtility.HtmlEncode (prop.DisplayName) %></a></td>
		            <td><span class="summary"><%= prop.FormattedSummary %></span></td>
		        </tr>
		        <% } %>
		        </table>
        <% } %>

		<%
			var interfaces = Model.Types.Where (p => p.Kind == "Interface");
			if (interfaces.Count () > 0) {
		%>
				<h2>Interfaces</h2>
		
		        <table class="member-list indent">
		        <% foreach (var prop in interfaces) { %>
		        <tr>
		            <td><img src="<%= Html.ResolveUrl ("~/Images/Types/pubinterface.png") %>" /></td>
		            <td style="width: 150px;"><a href="<%= Html.ResolveUrl (prop.TypeUrl) %>"><%= HttpUtility.HtmlEncode (prop.DisplayName) %></a></td>
		            <td><span class="summary"><%= prop.FormattedSummary %></span></td>
		        </tr>
		        <% } %>
		        </table>
		<% } %>

		<% 
			var enumerations = Model.Types.Where (p => p.Kind == "Enumeration");
			if (enumerations.Count () > 0) {
		%>
		        <h2>Enumerations</h2>
		
		        <table class="member-list indent">
		        <% foreach (var prop in enumerations) { %>
		        <tr>
		            <td><img src="<%= Html.ResolveUrl ("~/Images/Types/pubenumeration.png") %>" /></td>
		            <td style="width: 150px;"><a href="<%= Html.ResolveUrl (prop.TypeUrl) %>"><%= HttpUtility.HtmlEncode (prop.DisplayName) %></a></td>
		            <td><span class="summary"><%= prop.FormattedSummary %></span></td>
		        </tr>
		        <% } %>
		        </table>
		<% } %>
		
    </div>

</asp:Content>
