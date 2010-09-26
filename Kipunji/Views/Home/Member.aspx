<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Kipunji.Models.MemberModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= ViewData["Title"] %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial ("BreadCrumbBar", ViewData["BreadCrumb"]); %>
        <div class="method-header">
            <%= Html.Encode(string.Format ("{0}.{1} {2}", Model.ParentType, Model.Name, Model.Type)) %>
        </div>
     <div class="right-content-pad">
       
        <p class="indent"><%= Model.FormattedSummary %></p>
        <h2>Syntax</h2>
        <pre class="brush: csharp;"><%= Html.Encode (Model.Signature.Value.TrimEnd (';')) %></pre>

        <% if (Model.Parameters.Count > 0) { %>
        <h3>Parameters</h3>
        <table class="indent" style="border: 0px">
        <% foreach (var p in Model.Parameters) { %>
        <tr>
        <td style="padding: 0 20px 0 0"><b><i><%= Html.Encode (p.Name) %></i></b></td>
        <td style="padding: 0 20px 0 0"><%= Kipunji.Formatter.CreateTypeLink (p.Type, p.FormattedType) %></td>
        <td><%= Html.Encode (p.Description) %></td>
        </tr>
        <% } %>
        </table>
        <% } %>

        <% if (Model.FormattedReturnType != "void") { %>
        <h3><%= Model.Type == "Property" ? "Property Value" : "Return Value" %></h3>
        <table class="indent" style="border: 0px">
        <tr>
        <td style="padding: 0 20px 0 0"><%= Kipunji.Formatter.CreateTypeLink (Model.ReturnType, Model.ReturnType)%></td>
        <td style="padding: 0 20px 0 0"><%= Model.FormattedReturnSummary %></td>
        </tr>
        </table>
        <% } %>

        <h2>Remarks</h2>
        <p class="indent"><%= Model.FormattedRemarks %></p>

        <h2>Version Information</h2>
        <div class="indent"><%= Model.FormattedVersionInfo %></div>

    </div>

</asp:Content>
