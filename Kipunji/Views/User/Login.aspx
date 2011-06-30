﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

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
	
	 <center>
 
        <p class="indent">Please login with one of the following providers to edit documentation.</p>
     
        <table cellspacing="5px" style="border-collapse: separate">
    	<tr>
    	<td style="border:solid lightgrey 1px;padding:15px">
           <a id="OIDgoogle" style="cursor:pointer" onclick="document.getElementById('openid_identifier').value = 'https://www.google.com/accounts/o8/id';document.getElementById ('openid_form').submit ();" style="cursor:hand"><img border="0" src="../Images/signin-google.png" alt="Google" /></a>
    	</td>
    	<td style="border:solid lightgrey 1px;padding:15px">
        <a id="OIDyahoo" style="cursor:pointer"  onclick="document.getElementById('openid_identifier').value = 'http://yahoo.com/';document.getElementById ('openid_form').submit ();"><img border="0" src="../Images/signin-yahoo.png" alt="Yahoo!" /></a>
    	</td>
        </tr>
        </table>
     </center>
    
        <form id="openid_form" action="Authenticate?ReturnUrl=<%=HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]) %>" method="post">
			<input id="openid_identifier" name="openid_identifier" type="hidden" />
		</form>
	
    </div>

</asp:Content>