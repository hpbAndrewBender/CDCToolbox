using CDCToolbox.Logic;
using Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CDCToolbox.Controls
{
	/// <summary>
	/// Interaction logic for ViewPO.xaml
	/// </summary>
	public partial class ViewPO : UserControl
	{
		public ViewPO()
		{
			InitializeComponent();

			List<DataGridTextColumn> orderReopenColumns = new List<DataGridTextColumn>
			{
					new DataGridTextColumn
					{
						Header="PO Number",
						Binding=new Binding("ReopenPONumber"),
						Width=200,
						CanUserResize=false,
					},
					new DataGridTextColumn
					{
						Header="Status",
						Binding=new Binding("ReopenStatus"),
						Width=200,
						CanUserResize=false,
					},
					new DataGridTextColumn
					{
						Header="WMS Type",
						Binding=new Binding("ReopenWMSType"),
						Width=200,
						CanUserResize=false,
					},
			};
			List<DataGridTextColumn> masterReopenColumns = new List<DataGridTextColumn>
			{
				new DataGridTextColumn
				{
					Header = "Status",
					Binding=new Binding("ReopenUserInt1"),
					Width=50,
					CanUserResize=false,
				},
				new DataGridTextColumn
				{
					Header = "Item Code",
					Binding=new Binding("ReopenItemCode"),
					Width=135
				},
				new DataGridTextColumn
				{
					Header = "Product",
					Binding=new Binding("ReopenProductType"),
					Width=55
				},
				new DataGridTextColumn
				{
					Header = "Vendor",
					Binding=new Binding("ReopenVendorID"),
					Width=80
				},
				new DataGridTextColumn
				{
					Header = "Title",
					Binding=new Binding("ReopenTitle"),
					Width=150
				},
				new DataGridTextColumn
				{
					Header = "ISBN",
					Binding=new Binding("ReopenISBN"),
					Width=125
				},
				new DataGridTextColumn
				{
					Header = "Last PO",
					Binding=new Binding("ReopenLastPurchaseOrder"),
					Width=50
				},
			};

			gridOrderHeader.AddMultipleColumns(orderReopenColumns);
			gridProductMaster.AddMultipleColumns(masterReopenColumns);
		}

		private void ButtonClear(object sender,RoutedEventArgs e)
		{
			if(sender.GetType().Name=="Button")
			{
				gridOrderHeader.Items.Clear();
				gridProductMaster.Items.Clear();
				reopenTextBox.Text="";
			}
		}

		private void ButtonSearch(object sender,RoutedEventArgs e)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			if(sender.GetType().Name=="Button")
			{
				string Type = ((Button)sender).Name.ToUpper().Substring(0,4);

				if(!string.IsNullOrEmpty(reopenTextBox.Text)&&reopenTextBox.Text.Length==6)
				{
					string XMLSearch = Globals.Props.XMLSearch.ForEnvironment<string>();
					List<string> Files = new List<string>();

					// Get XML Files
					using(new WaitCursor())
					{
						if(gridOrderHeader.Items.Count>0)
						{
							gridOrderHeader.Items.Clear();
						}
						if(gridProductMaster.Items.Count>0)
						{
							gridProductMaster.Items.Clear();
						}
						// Get Database
						try
						{
							DataTable OrderHeader = IO.SQLAccess.Table
							(
								IO.SQLAccess.dbConn[$"dips -{Globals.Env}"],
								new List<string>
								{
									"SELECT [PONumber],[Status],ISNULL([WMSType],'') AS [WMSType]",
									"FROM OrderHeaderDist",
									"WHERE PONumber=@PO"
								},
								new List<SqlParameter>
								{
							new SqlParameter() {ParameterName="@PO", SqlDbType=SqlDbType.VarChar, Value=this.reopenTextBox.Text }
								},
								CommandType.Text
							);
							if(OrderHeader!=null&&OrderHeader.Rows.Count>0)
							{
								foreach(DataRow row in OrderHeader.Rows)
								{
									gridOrderHeader.Items.Add
									(
										new Models.gridOrderHeader
										{
											ReopenPONumber=row["PONUmber"].ToString(),
											ReopenStatus=row["Status"].ToString(),
											ReopenWMSType=row["WMSType"].ToString(),
										}
									);
									gridOrderHeader.IsEnabled=true;
								}
							}
							DataTable ProductMaster = IO.SQLAccess.Table
							(
								IO.SQLAccess.dbConn[$"dips -{Globals.Env}"],
								new List<string>
								{
									"SELECT [UserInt1],pm.[ItemCode],[ProductType],[VendorID],[Title],pm.[ISBN],[LastPurchaseOrder]",
									"FROM dbo.[ProductMaster] pm",
									"INNER JOIN dbo.[OrderDetail] od",
									"ON pm.[ItemCode]=od.[ItemCode]",
									"WHERE od.[PONumber]=@PO"
								},
								new List<SqlParameter>
								{
									new SqlParameter() {ParameterName="@PO", SqlDbType=SqlDbType.VarChar, Value=this.reopenTextBox.Text}
								},
								CommandType.Text
							);
							if(ProductMaster!=null&&ProductMaster.Rows.Count>0)
							{
								foreach(DataRow row in ProductMaster.Rows)
								{
									gridProductMaster.Items.Add
									(
										new Models.gridProductMaster
										{
											ReopenUserInt1=row["userInt1"].ToString(),
											ReopenISBN=row["ISBN"].ToString(),
											ReopenItemCode=row["ItemCode"].ToString(),
											ReopenLastPurchaseOrder=row["LastPurchaseOrder"].ToString(),
											ReopenProductType=row["ProductType"].ToString(),
											ReopenTitle=row["Title"].ToString(),
											ReopenVendorID=row["VendorID"].ToString(),
										}
									);
								}
							}
						}
						catch(Exception ex)
						{
							Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
						}
					}
				}
				else
				{
					MessageBox.Show("Invalid Purchase Order Number length. Must be 6 characters","Invalid Entry",MessageBoxButton.OK,MessageBoxImage.Asterisk);
				}
				Console.WriteLine(Globals.Message.Count());
			}
		}
	}


}
