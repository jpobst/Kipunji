﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Kipunji.Models.TypeModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

     <div class="right-content-pad">
        <table class="indent"><tr>
            <td style="width:16px; padding:0;"><a href="<%= Html.ResolveUrl (Model.MembersUrl) %>"><img src="<%= Html.ResolveUrl ("~/Images/Types/members.png") %>" /></a></td>
            <td><a href="<%= Html.ResolveUrl (Model.MembersUrl) %>">Members</a></td>
        </tr></table>
        <p class="indent"><span class="summary-type"><%= Model.FormattedSummary %></span></p>

        <h2>Syntax</h2>
        <div class="brush: csharp;"><%= Model.FormattedTypeSignature (VirtualPathUtility.ToAbsolute ("~/")) %></div>

        <h2>Remarks</h2>
        <span class="remarks-type"><%= Model.FormattedRemarks %></span>

        <h2>Version Information</h2>
        <div class="indent"><%= Model.FormattedVersionInfo %></div>

    </div>

</asp:Content>
