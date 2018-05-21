using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Ridgian.Carpetright.Samples.WebParts.CreateSampleOrder
{
	[ToolboxItemAttribute(false)]
	public class CreateSampleOrder : WebPart
	{
		// Visual Studio might automatically update this path when you change the Visual Web Part project item.
		private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/Ridgian.Carpetright.Samples.WebParts/CreateSampleOrder/CreateSampleOrderUserControl.ascx";

		protected override void CreateChildControls()
		{
			Control control = Page.LoadControl(_ascxPath);
			Controls.Add(control);
		}
	}
}
