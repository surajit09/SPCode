<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateSampleOrderUserControl.ascx.cs" Inherits="Ridgian.Carpetright.Samples.WebParts.CreateSampleOrder.CreateSampleOrderUserControl" %>

<!-- Sample Ordering Start -->
<asp:Repeater ID="rptSampleOrders" runat="server" Visible="true">
	<HeaderTemplate>		
		<div id="rgsampleordering">
			<div class="clear"></div>
	</HeaderTemplate>
	<ItemTemplate>
		<div id="range<%#(Container.ItemIndex + 1).ToString()%>" class="rgheader3 haspointer">
			<span><%#Container.DataItem.ToString()%></span>
			<div id="divNrOfItems" class="fright" runat="server"></div>
		</div>
		<table id="detailrange<%#(Container.ItemIndex + 1).ToString()%>" class="displayhide">
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
					</tr>
				</HeaderTemplate>
				<ItemTemplate>
					<tr>
						<td><%# DataBinder.Eval(Container.DataItem, (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column4Field", System.Globalization.CultureInfo.CurrentCulture)) %></td>
						<td><%# DataBinder.Eval(Container.DataItem, (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column2Field", System.Globalization.CultureInfo.CurrentCulture)) %></td>
						<td><%# DataBinder.Eval(Container.DataItem, (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column3Field", System.Globalization.CultureInfo.CurrentCulture)) %></td>
						<td>
							<input id="quantity<%#(Container.ItemIndex + 1).ToString()%>range<%#ItemIndex.ToString() %>" class="quantity" type="text" value="0" maxlength="2" /></td>
						<td>
							<input id="reason<%#(Container.ItemIndex + 1).ToString()%>range<%#ItemIndex.ToString() %>" class="reason" type="text" value="" maxlength="255" disabled="disabled" /></td>
							
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

<asp:HiddenField runat="server" ID="hiddenStoreName" value="" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="hiddenStoreNumber" value="" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="hiddenStoreEmail" value="" ClientIDMode="Static" />

<div class="clear"></div>

<a id="linkSubmit" class="linkbutton fright" href="#" onclick="javascript:SubmitSamplesOrder()">
	<asp:Literal ID="ltrSubmitButton" runat="server" Text="<%$ Resources:CRSamplesResources,SubmitButton %>" /></a>

<a id="linkReset" class="linkbutton fright rspacing" href="#" onclick="javascript:ResetSamplesOrder()">
	<asp:Literal ID="ltrReset" runat="server" Text="<%$ Resources:CRSamplesResources,ResetButton %>" /></a>

<div id="modalDialogMessage" class="fleft"></div>




<script type="text/javascript" src="/_catalogs/masterpage/Ridgian.Baseline.JS/jquery.inputmask.bundle.min.js"></script>
<script type="text/javascript" src="/_catalogs/masterpage/Ridgian.Carpetright.JS/cr-createsampleorder.js"></script>

<!-- Sample Ordering End -->
