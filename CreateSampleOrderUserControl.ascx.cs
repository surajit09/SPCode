#region Usings

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using System.IO;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web;

#endregion

namespace Ridgian.Carpetright.Samples.WebParts.CreateSampleOrder
{
	public partial class CreateSampleOrderUserControl : UserControl
	{
		#region Type Level Variable and Constant Declarations

		private string BUYERSSITEURL = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "BuyersSiteUrl", CultureInfo.CurrentCulture);
		private string SAMPLESLIST = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "SamplesList", CultureInfo.CurrentCulture);
		private string STORESLIST = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "StoresList", CultureInfo.CurrentCulture);

		private string COLUMN1FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column1Field", CultureInfo.CurrentCulture);
		private string COLUMN2FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column2Field", CultureInfo.CurrentCulture);
		private string COLUMN3FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column3Field", CultureInfo.CurrentCulture);
		private string COLUMN4FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column4Field", CultureInfo.CurrentCulture);
		private string COLUMN7FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column7Field", CultureInfo.CurrentCulture);
		private string COLUMN8FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column8Field", CultureInfo.CurrentCulture);
		private string COLUMN9FIELD = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "Column9Field", CultureInfo.CurrentCulture);

		private string FILTERBY3 = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "FilterBy3", CultureInfo.CurrentCulture);
		private string ORDERBY1 = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "OrderBy1", CultureInfo.CurrentCulture);
		private string ORDERBY2 = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "OrderBy2", CultureInfo.CurrentCulture);
		private string ORDERBY4 = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "OrderBy4", CultureInfo.CurrentCulture);
		private string ORDERBY5 = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "OrderBy5", CultureInfo.CurrentCulture);
		private string ORDERBYASCENDING = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "OrderByAscending", CultureInfo.CurrentCulture);
		private string GROUPBY = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "GroupBy", CultureInfo.CurrentCulture);

		private string MESSAGENOSAMPLESFILE = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "MessageNoSamplesFile", CultureInfo.CurrentCulture);
		private string MESSAGENOITEMS = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "MessageNoItems", CultureInfo.CurrentCulture);
		private string MESSAGEERROR = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "MessageError", CultureInfo.CurrentCulture);
		private string MESSAGENOSTOREINFO = (string)HttpContext.GetGlobalResourceObject("CRSamplesResources", "MessageNoStoreInfo", CultureInfo.CurrentCulture);

		#endregion

		#region Private Properties

		private int _itemIndex = 0;
		
		#endregion

		#region Public Properties
		
		public int ItemIndex
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

		#endregion

		#region Protected Methods

		protected void Page_Load(object sender, EventArgs e)
		{
			//if (!Page.IsPostBack)
			//{
				// bind events to controls
				rptSampleOrders.ItemDataBound += new RepeaterItemEventHandler(rptSampleOrders_ItemDataBound);

				// get Store data and set hidden fields
				if (GetStoreInfo())
				{
					// get Samples data if Store Info is available. If info is not available, there is no point to continue
					GetData();
				}
			//}
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

				// increment item index to be binded to ID property of some elements to be rendered in the page
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
		/// Get Store information from stores list based in the Store ID from the URL
		/// Sets the hidden fields so the information is available in the client side
		/// </summary>
		private bool GetStoreInfo()
		{
			bool infoAvailable = false;

			try
			{
				// get list from site and dispose objects
				using (SPSite site = new SPSite(SPContext.Current.Site.ID))
				{
					using (SPWeb web = site.RootWeb)
					{
						// get Samples list
						SPList list = web.Lists.TryGetList(STORESLIST);

						if (list != null)
						{
							// get Store Identifier from URL
							string storeNumber = SPContext.Current.Web.ServerRelativeUrl.Trim('/');

							// query the SPList using SPQuery as it seems to be the method with the best performance
							// and we will be potencially dealing with large volumes of items in the list

							// Build a query
							SPQuery query = new SPQuery();
							// filter by relevant fields
							query.Query = string.Concat(
								"<Where>",
									string.Format("<Eq><FieldRef Name='" + FILTERBY3 + "'/><Value Type='Number'>{0}</Value></Eq>", storeNumber),
								"</Where>");

							// get only 1 store - 
							query.RowLimit = 1;

							// get items
							SPListItemCollection items = list.GetItems(query);

							if (items.Count > 0)
							{
								hiddenStoreName.Value = items[0][COLUMN7FIELD].ToString();
								hiddenStoreNumber.Value = storeNumber;
								hiddenStoreEmail.Value = items[0][COLUMN9FIELD].ToString();

								// set infoAvailable flag to true
								infoAvailable = true;
							}
							else
							{
								infoAvailable = false;
								// display error message
								DisplayMessage(MESSAGENOSTOREINFO);
							}
						}
						else
						{
							infoAvailable = false;
							// display error message
							DisplayMessage("Stores List is not available");
						}
					}
				}				
			}
			catch (Exception ex)
			{
				// log exception
				Ridgian.SP.Utilities.TraceLog.WriteException(ex);

				infoAvailable = false;
				// display error message
				DisplayMessage(MESSAGEERROR);
			}

			return infoAvailable;
		}

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
						SPList list = web.Lists.TryGetList(SAMPLESLIST);

						if (list != null)
						{
							// get latest item created in the list
							SPListItem item = GetLastItemFromList(list);

							if (item != null)
							{
								// get file associated with the list item						
								SPFile file = item.File;

								if (file != null)
								{
									// check if the file is a CSV file 
									if (file.Name.EndsWith(".csv", true, CultureInfo.InvariantCulture))
									{
										// convert CSV file to DataTable object
										DataTable dt = ConvertCSVtoDataTable(file);
										// sort data
										dt.DefaultView.Sort = string.Format("{0} {1}", GROUPBY, ORDERBYASCENDING);
										dt = dt.DefaultView.ToTable();

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
										else
										{
											// display error message
											DisplayMessage(MESSAGENOITEMS);
										}
									}
									else
									{
										// display error message
										DisplayMessage(MESSAGENOSAMPLESFILE);
									}
								}
								else
								{
									// display error message
									DisplayMessage(MESSAGENOSAMPLESFILE);
								}
							}
							else
							{
								// display error message
								DisplayMessage(MESSAGENOSAMPLESFILE);
							}
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
		/// Get the last item from a SharePoint List using the Created date field
		/// </summary>
		/// <param name="list">SPList to query</param>
		/// <returns>The latest SPListItem created in the list or null</returns>
		private SPListItem GetLastItemFromList(SPList list)
		{
			SPListItem item = (from SPListItem lastItem in list.Items
							   orderby lastItem[ORDERBY5] descending 
							   select lastItem).FirstOrDefault();

			return item;
		}

		/// <summary>
		/// Convert a CSV file to a DataTable object
		/// </summary>
		/// <param name="file">CSV file to read</param>
		/// <returns>A DataTable object containing the information of the CSV file</returns>
		private DataTable ConvertCSVtoDataTable(SPFile file)
		{
			// Read CSV file
			StreamReader reader = new StreamReader(file.OpenBinaryStream());
			// Reading line by line of the csv file
			string row;
			DataTable dt = new DataTable();
			//create columns in the DataTable
			dt.Columns.Add(COLUMN1FIELD);
			dt.Columns.Add(COLUMN2FIELD);
			dt.Columns.Add(COLUMN3FIELD);
			dt.Columns.Add(COLUMN4FIELD);

			while ((row = reader.ReadLine()) != null)
			{
				// split row by ','
				char[] splitter = { ',' };
				String[] cells = row.ToString().Split(splitter);
				// get only the first 4 cells of the row
				if (cells.Length >= 4)
				{
					String[] copyCells = new String[4];
					Array.Copy(cells, copyCells, 4);
					// add first 4 cells of the row to DataTable
					dt.Rows.Add(copyCells);
				}
				
			}
			return dt;
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
													Column4 = i.Field<object>(COLUMN4FIELD)
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

					// add all items as rows of the DataTable
					foreach (var itemElement in groupElement.Items)
					{
						groupDataTable.Rows.Add(itemElement.Column1, itemElement.Column2, itemElement.Column3, itemElement.Column4);
					}

					// order datatable
					groupDataTable.DefaultView.Sort = string.Format("{0} {1}, {2} {3}, {4} {5}",
						ORDERBY1, ORDERBYASCENDING, ORDERBY2, ORDERBYASCENDING, ORDERBY4, ORDERBYASCENDING);
					groupDataTable = groupDataTable.DefaultView.ToTable();

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
