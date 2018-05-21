#region Usings

using Microsoft.SharePoint;
using System;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Linq;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

#endregion

namespace Ridgian.Carpetright.Samples.WebParts.ViewSampleOrder
{
	public partial class ViewSampleOrderUserControl : UserControl
	{
		#region Type Level Variable and Constant Declarations

		private string BUYERSSITEURL = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "BuyersSiteUrl", CultureInfo.CurrentCulture);
		private string SAMPLESORDERLIST = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "SamplesOrderList", CultureInfo.CurrentCulture);
		
		private string COLUMN1FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column1Field", CultureInfo.CurrentCulture);
		private string COLUMN2FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column2Field", CultureInfo.CurrentCulture);
		private string COLUMN3FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column3Field", CultureInfo.CurrentCulture);
		private string COLUMN4FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column4Field", CultureInfo.CurrentCulture);
		private string COLUMN5FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column5Field", CultureInfo.CurrentCulture);
		private string COLUMN6FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column6Field", CultureInfo.CurrentCulture);
		private string COLUMN10FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column10Field", CultureInfo.CurrentCulture);
		private string COLUMN11FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column11Field", CultureInfo.CurrentCulture);

		private string ORDERBY1 = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "OrderBy1", CultureInfo.CurrentCulture);
		private string ORDERBY2 = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "OrderBy2", CultureInfo.CurrentCulture);
		private string ORDERBY3 = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "OrderBy3", CultureInfo.CurrentCulture);		
		private string ORDERBY5 = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "OrderBy5", CultureInfo.CurrentCulture);
		private string ORDERBYASCENDING = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "OrderByAscending", CultureInfo.CurrentCulture);
		private string ORDERBYDESCENDING = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "OrderByDescending", CultureInfo.CurrentCulture);
		private string FILTERBY1 = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "FilterBy1", CultureInfo.CurrentCulture);
		private string FILTERBY2 = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "FilterBy2", CultureInfo.CurrentCulture);
		private string GROUPBY = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "GroupBy", CultureInfo.CurrentCulture);

		private string MESSAGENOITEMS = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "MessageNoItems", CultureInfo.CurrentCulture);
		private string MESSAGEERROR = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "MessageError", CultureInfo.CurrentCulture);
		private string MESSAGENOSTOREINFO = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "MessageNoStoreInfo", CultureInfo.CurrentCulture);

		#endregion

		#region Private Properties

		private int _itemIndex = 0;
		private bool _filterOrderedSamples = false;

		#endregion

		#region Public Properties

		public int itemIndex
		{
			get
			{
				return _itemIndex;
			}
			set
			{
				_itemIndex = value;
			}
		}

		public bool FilterOrderedSamples
		{
			get
			{
				return _filterOrderedSamples;
			}
			set
			{
				_filterOrderedSamples = value;
			}
		}

		#endregion

		#region Protected Methods

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				// bind events to controls

				rptSampleOrders.ItemDataBound += new RepeaterItemEventHandler(rptSampleOrders_ItemDataBound);

				GetData();
			}
		}

		protected void rptSampleOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			// Execute the following logic for Items and Alternating Items.
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				// get number of items (Rows) and set the right CSS class in the counter located at the right
				HtmlGenericControl divNrOfItems = e.Item.FindControl("divNrOfItems") as HtmlGenericControl;
				int nrOfrows = ((DataTable)e.Item.DataItem).Rows.Count;
				divNrOfItems.InnerText = string.Format("({0})", nrOfrows.ToString());			
				//divNrOfItems.Attributes["class"] = GetNrOfItemsCSSClass(nrOfrows);

				// increment index to be binded to ID property of some elements to be rendered in the page
				_itemIndex++;

				// get inner Repeater to display items
				Repeater rptSampleItems = (Repeater)(e.Item.FindControl("rptSampleItems"));
				if (rptSampleItems != null)
				{
					rptSampleItems.DataSource = (DataTable)e.Item.DataItem;
					rptSampleItems.DataBind();
				}


			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Get data to be rendered in the page
		/// </summary>
		private void GetData()
		{
			try
			{
				// get list from site and dispose objects
				using (SPSite site = new SPSite(SPContext.Current.Site.ID))
				{
					using (SPWeb web = site.OpenWeb(BUYERSSITEURL))
					{
						// get Samples list
						SPList list = web.Lists.TryGetList(SAMPLESORDERLIST);

						if (list != null)
						{
							// get Store Identifier from URL
							string storeNumber = SPContext.Current.Web.ServerRelativeUrl.Trim('/');

							// query the SPList using SPQuery as it seems to be the method with the best performance
							// and we will be potencially dealing with large volumes of items in the list

							// Build a query
							SPQuery query = new SPQuery();
							// filter by relevant fields
							string queryString = string.Concat(
								"<Where><And>",
									string.Format("<Eq><FieldRef Name='" + FILTERBY1 + "'/><Value Type='Text'>{0}</Value></Eq>", storeNumber),
									string.Format("<{0}><FieldRef Name='" + FILTERBY2 + "'/></{0}>", GetFilterCondition(_filterOrderedSamples)),
								"</And></Where>");

							// NOTE: ordering is applied only after grouping the results

							query.Query = queryString;

							query.ViewFields = string.Concat(
									   string.Format("<FieldRef Name='{0}' />", COLUMN1FIELD),
									   string.Format("<FieldRef Name='{0}' />", COLUMN2FIELD),
									   string.Format("<FieldRef Name='{0}' />", COLUMN3FIELD),
									   string.Format("<FieldRef Name='{0}' />", COLUMN4FIELD),
									   string.Format("<FieldRef Name='{0}' />", COLUMN5FIELD),
									   string.Format("<FieldRef Name='{0}' />", COLUMN6FIELD),
									   string.Format("<FieldRef Name='{0}' />", COLUMN10FIELD),
									   string.Format("<FieldRef Name='{0}' />", COLUMN11FIELD)
									   );

							// get items
							SPListItemCollection items = list.GetItems(query);

							if (items.Count > 0)
							{
								// get items into a DataTable object
								DataTable dt = new DataTable();

								dt = items.GetDataTable();

								if (dt.Rows.Count > 0)
								{
									// group DataTable information and create a DataSet from it
									// this is required as binding the repeater with untyped object will cause problems when
									// trying to bind the Items to the second repeater
									DataSet ds = ConvertDataTableToGroupedDataSet(dt, GROUPBY);

									if (ds != null)
									{
										// bind data to main repeater
										rptSampleOrders.DataSource = ds.Tables;
										rptSampleOrders.DataBind();
									}
								}
							}
							else
							{
								// display error message
								DisplayMessage(MESSAGENOITEMS);
							}
						}
						else
						{
							// display error message
							DisplayMessage(MESSAGEERROR);
						}
					}
				}				
			}
			catch (Exception ex)
			{
				// log exception
				Ridgian.SP.Utilities.TraceLog.WriteException(ex);
				// display error message
				DisplayMessage(MESSAGEERROR);
			}
		}

		/// <summary>
		/// Convert Filter property to 0/1 values to use in the CAML query
		/// </summary>
		/// <param name="_filterOrderedSamples">Enum variable</param>
		/// <returns></returns>
		private string GetFilterCondition(bool _filterOrderedSamples)
		{
			// default to no
			string filter = string.Empty;

			switch (_filterOrderedSamples)
			{
				case true:
				{
					// check if Ordered date is not null
					filter = "IsNotNull";
				}
				break;
				case false:
				{
					// check if Ordered date is null
					filter = "IsNull";
				}
				break;
				default:
				{
					filter = "IsNull";
				}
				break;
			}

			return filter;
		}

		/// <summary>
		/// Converts a DataTable object to a grouped DataSet object
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="groupByField">Field to group by</param>
		/// <returns></returns>
		private DataSet ConvertDataTableToGroupedDataSet(DataTable dt, string groupByField)
		{
			DataSet ds = null;

			// group results using Linq
			var groupedCollection = from table in dt.AsEnumerable()
									group table by table.Field<string>(groupByField) into groupby
									select new
									{
										Key = groupby.Key,
										Items = from i in groupby
												select new
												{
													Column1 = i.Field<object>(COLUMN1FIELD),
													Column2 = i.Field<object>(COLUMN2FIELD),
													Column3 = i.Field<object>(COLUMN3FIELD),
													Column4 = i.Field<object>(COLUMN4FIELD),
													Column5 = i.Field<object>(COLUMN5FIELD),
													Column6 = i.Field<object>(COLUMN6FIELD),
													Column10 = i.Field<object>(COLUMN10FIELD),
													Column11 = i.Field<object>(COLUMN11FIELD)
												}
									};

			if (groupedCollection.Count() > 0)
			{
				ds = new DataSet();

				// Create a DataTable for each group
				foreach (var groupElement in groupedCollection)
				{
					DataTable groupDataTable = new DataTable(groupElement.Key);
					groupDataTable.Columns.Add(COLUMN1FIELD);
					groupDataTable.Columns.Add(COLUMN2FIELD);
					groupDataTable.Columns.Add(COLUMN3FIELD);
					groupDataTable.Columns.Add(COLUMN4FIELD);
					groupDataTable.Columns.Add(COLUMN5FIELD);
					groupDataTable.Columns.Add(COLUMN6FIELD);
					groupDataTable.Columns.Add(COLUMN10FIELD);
					groupDataTable.Columns.Add(COLUMN11FIELD);

					// add all items as rows of the DataTable
					foreach (var itemElement in groupElement.Items)
					{
						groupDataTable.Rows.Add(
							itemElement.Column1, 
							itemElement.Column2, 
							itemElement.Column3, 
							itemElement.Column4, 
							itemElement.Column5, 
							itemElement.Column6,
							itemElement.Column10,
							itemElement.Column11);
					}

					// order data table
					if (_filterOrderedSamples)
					{
						// when filtering ordered samples
						groupDataTable.DefaultView.Sort = string.Format("{0} {1}, {2} {3}, {4} {5}", 
							ORDERBY1, ORDERBYASCENDING, ORDERBY2, ORDERBYASCENDING, ORDERBY3, ORDERBYDESCENDING);
						groupDataTable = groupDataTable.DefaultView.ToTable();
					}
					else
					{
						// when filtering Requested - Not Ordered samples
						// when filtering ordered samples
						groupDataTable.DefaultView.Sort = string.Format("{0} {1}, {2} {3}, {4} {5}",
							ORDERBY1, ORDERBYASCENDING, ORDERBY2, ORDERBYASCENDING, ORDERBY5, ORDERBYDESCENDING);
						groupDataTable = groupDataTable.DefaultView.ToTable();
					}					

					// Add DataTable containing the information to the DataSet
					ds.Tables.Add(groupDataTable);
				}
			}

			return ds;
		}

		/// <summary>
		/// Set status message
		/// </summary>
		/// <param name="message">Message to be displayed in the page</param>
		private void DisplayMessage(string message)
		{
			//hide repeater
			rptSampleOrders.Visible = false;
			// show message
			lblMessage.Visible = true;
			lblMessage.Text = message;
		}

		#endregion
	}
}
