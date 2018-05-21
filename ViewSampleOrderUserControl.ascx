<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewSampleOrderUserControl.ascx.cs" Inherits="Ridgian.Carpetright.Samples.WebParts.ViewSampleOrder.ViewSampleOrderUserControl" %>

<!-- Sample Ordering Start -->
<asp:Repeater ID="rptSampleOrders" runat="server" Visible="true">
	<HeaderTemplate>
		<div id="rgsampleordering">
			<div class="clear"></div>
	</HeaderTemplate>
	<ItemTemplate>
		<div id="<%# FilterOrderedSamples.ToString() %>sample<%#(Container.ItemIndex + 1).ToString()%>" class="rgheader3 haspointer">
			<span><%#Container.DataItem.ToString()%></span>
			<div id="divNrOfItems" class="fright" runat="server"></div>
		</div>
		<table id="detail<%# FilterOrderedSamples.ToString() %>sample<%#(Container.ItemIndex + 1).ToString()%>" class="displayhide">
			<asp:Repeater ID="rptSampleItems" runat="server">
				<HeaderTemplate>
					<tr>
						<th class="range">
							<asp:Literal ID="ltrColumn1" runat="server" Text="<%$ Resources:CRSamplesResources,Column4 %>" /></th>
						<th class="article">
							<asp:Literal ID="ltrColumn2" runat="server" Text="<%$ Resources:CRSamplesResources,Column2 %>" /></th>
						<th class="description">
							<asp:Literal ID="ltrColumn3" runat="server" Text="<%$ Resources:CRSamplesResources,Column3 %>" /></th>
						<th class="quantity">
							<asp:Literal ID="ltrQuantityOfSamples" runat="server" Text="<%$ Resources:CRSamplesResources,Column5 %>" /></th>
						<th class="reason">
							<asp:Literal ID="ltrReasonForNewSamples" runat="server" Text="<%$ Resources:CRSamplesResources,Column6 %>" /></th>
						<th class="date">
						<% if (FilterOrderedSamples) { %> 
							<asp:Literal ID="ltrOrderedDate" runat="server" Text="<%$ Resources:CRSamplesResources,Column11 %>" />
						<% } else { %>
							<asp:Literal ID="ltrCreatedDate" runat="server" Text="<%$ Resources:CRSamplesResources,Column10 %>" />
						<% } %>
						</th>
					</tr>
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
						<td><%# DataBinder.Eval(Container.DataItem, (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column4Field", System.Globalization.CultureInfo.CurrentCulture)) %></td>
						<td><%# DataBinder.Eval(Container.DataItem, (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column2Field", System.Globalization.CultureInfo.CurrentCulture)) %></td>
						<td><%# DataBinder.Eval(Container.DataItem, (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column3Field", System.Globalization.CultureInfo.CurrentCulture)) %></td>
						<td><%# DataBinder.Eval(Container.DataItem, (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column5Field", System.Globalization.CultureInfo.CurrentCulture)) %></td>
						<td><%# DataBinder.Eval(Container.DataItem, (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column6Field", System.Globalization.CultureInfo.CurrentCulture)) %></td>
						<td>
							<% if (FilterOrderedSamples) { %> 
								<%# DataBinder.Eval(Container.DataItem, (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column11Field", System.Globalization.CultureInfo.CurrentCulture)).ToString().Split(' ')[0] %>
							<% } else { %>
								<%# DataBinder.Eval(Container.DataItem, (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column10Field", System.Globalization.CultureInfo.CurrentCulture)).ToString().Split(' ')[0] %>
							<% } %>
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</table>
	</ItemTemplate>
	<FooterTemplate>
		</div>
	</FooterTemplate>
</asp:Repeater>

<asp:Label ID="lblMessage" runat="server" Text="" Visible="false"></asp:Label>
<!-- Sample Ordering End -->
