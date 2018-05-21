using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Ridgian.Carpetright.Samples.WebParts.ViewSampleOrder
{
	[ToolboxItemAttribute(false)]
	public class ViewSampleOrder : WebPart
	{
		// Visual Studio might automatically update this path when you change the Visual Web Part project item.
		private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/Ridgian.Carpetright.Samples.WebParts/ViewSampleOrder/ViewSampleOrderUserControl.ascx";


		[Personalizable(PersonalizationScope.Shared),
		WebBrowsable(true),
		WebDisplayName("Filter Ordered/Not Ordered Samples"),
		WebDescription("Filter by Ordered/Not Ordered Samples"),
		Category("Carpetright")]
		public bool FilterOrderedSamples
		{
			get;
			set;
		}

		protected override void CreateChildControls()
		{
			ViewSampleOrderUserControl control = Page.LoadControl(_ascxPath) as ViewSampleOrderUserControl;
			control.FilterOrderedSamples = FilterOrderedSamples;
			Controls.Add(control);
		}
	}
}
