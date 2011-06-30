<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Kipunji.Models.MemberModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

     <div class="right-content-pad">
       
        <p class="indent"><span class="summary-member"><%= Model.FormattedDisplaySignature %></span></p>
       
        <h2>Syntax</h2>
        <pre class="brush: csharp;"><%= Html.Encode (Model.Signature.Value.TrimEnd (';')) %></pre>

        <% if (Model.Parameters.Count > 0) { %>
        <h3>Parameters</h3>
        <table class="indent" style="border: 0px">
        <% foreach (var p in Model.Parameters) { %>
        <tr>
        <td style="padding: 0 20px 0 0"><b><i><%= Html.Encode (p.Name) %></i></b></td>
        <td style="padding: 0 20px 0 0"><%= Kipunji.Formatter.CreateTypeLink (VirtualPathUtility.ToAbsolute ("~/"), p.Type) %></td>
        <td><span class="description-params"><%= Html.Encode (p.FormattedDescription) %></span></td>
        </tr>
        <% } %>
        </table>
        <% } %>

        <% if (Model.FormattedReturnType != "void" && Model.FormattedReturnType != null) { %>
        <h3><%= Model.Type == "Property" ? "Property Value" : "Return Value" %></h3>
        <table class="indent" style="border: 0px">
        <tr>
        <td style="padding: 0 20px 0 0"><%= Kipunji.Formatter.CreateTypeLink (VirtualPathUtility.ToAbsolute ("~/"), Model.ReturnType)%></td>
        <td style="padding: 0 20px 0 0"><span class="summary-return"><%= Model.FormattedReturnSummary %></span></td>
        </tr>
        </table>
        <% } %>

        <h2>Remarks</h2>
        <p class="indent"><span class="remarks-member"><%= Model.FormattedRemarks %></span></p>

        <h2>Version Information</h2>
        <div class="indent"><%= Model.FormattedVersionInfo %></div>

    </div>

</asp:Content>
