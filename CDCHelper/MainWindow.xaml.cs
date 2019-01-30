using CDCHelper.Logic;
using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace CDCHelper
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			string Current = MethodBase.GetCurrentMethod().Name;

			InitializeComponent();
			this.Title="CDC Toolbox";
			this.reopenButtonReopen.IsEnabled=false;			
			List<string> ProcessReceivedFiles = new List<string>();
			string Path = string.Empty;

			Globals.Env=Globals.Props.Production ? "prd" : "dev";
			Globals.ErrorTerminal=false;
			Globals.Message=new List<(DateTime Date, string Location, string ErrorLEvel, string Message)>();


			IO.SQLAccess.InsertServer
			(
				new List<(string name, string env, string server, string catalog, string security, string user, string password, string integrated)>
				{
						( "dips ", "prd", " sequoia   ", "HPB_DB         ", "True", "", "", "True"),
						( "dips ", "dev", " chipmunk  ", "HPB_DB         ", "True", "", "", "True"),
				}
			);

			List<DataGridTextColumn> xmlReopenColumns = new List<DataGridTextColumn>
			{
				new DataGridTextColumn
				{
					Header="XML File",
					Binding=new Binding("ReopenFileName"),
					Width=reopenGridXMLFiles.Width-10,
					CanUserResize=false,
				}
			};
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

			reopenGridXMLFiles.AddMultipleColumns(xmlReopenColumns);
			reopenGridOrderHeader.AddMultipleColumns(orderReopenColumns);
			reopenGridProductMaster.AddMultipleColumns(masterReopenColumns);

			List<string> StuckFiles = new List<string>();
			StuckFiles=Directory.GetFiles(@"\\wmsinterface\Projects\XMLFiles\HPBErr").ToList();
			foreach(string file in StuckFiles)
			{
				if(file.EndsWith("_popImport.xml"))
				{
					this.listBoxPO.Items.Add(file);
				}
				else if(file.EndsWith("_itemImport.xml"))
				{
					this.listBoxItem.Items.Add(file);
				}
				else if (file.EndsWith("_sopILSImport.xml"))
				{
					this.listBoxShip.Items.Add(file);
				}
				else
				{
					this.listBoxOther.Items.Add(file);
				}
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
						if(reopenGridXMLFiles.Items.Count>0)
						{
							reopenGridXMLFiles.Items.Clear();
						}
						if(reopenGridOrderHeader.Items.Count>0)
						{
							reopenGridOrderHeader.Items.Clear();
						}
						if(reopenGridProductMaster.Items.Count>0)
						{
							reopenGridProductMaster.Items.Clear();
						}
						try
						{
							Files.AddRange(Directory.GetFiles($"{XMLSearch}\\CDCErr").ToList());
							Files.AddRange(Directory.GetFiles($"{XMLSearch}\\HPBErr").ToList());
							Files.AddRange(Directory.GetFiles($"{XMLSearch}\\ILSErrors").ToList());

							foreach(string FileName in Files)
							{
								if(File.ReadAllText(FileName).Contains(reopenTextBox.Text))
								{
									reopenGridXMLFiles.Items.Add
									(
										new Models.gridFile
										{
											ReopenFileName=FileName
										}
									);
								}
							}
							if(reopenGridXMLFiles.Items.Count==0)
							{
							
								reopenGridXMLFiles.Items.Add
								(
									new Models.gridFile
									{
										ReopenFileName=Globals.NoFilesFound
									}
								);
							}
						}
						catch(Exception ex)
						{
							Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
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

									reopenGridOrderHeader.Items.Add
									(
										new Models.gridOrderHeader
										{
											ReopenPONumber=row["PONUmber"].ToString(),
											ReopenStatus=row["Status"].ToString(),
											ReopenWMSType=row["WMSType"].ToString(),
										}
									);
									reopenGridOrderHeader.IsEnabled=true;
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

									reopenGridProductMaster.Items.Add
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

		private void ButtonClear(object sender,RoutedEventArgs e)
		{
			if(sender.GetType().Name=="Button")
			{				
				reopenGridXMLFiles.Items.Clear();
				reopenGridOrderHeader.Items.Clear();
				reopenGridProductMaster.Items.Clear();
				this.reopenCheckBox.IsChecked=false;
				this.reopenButtonReopen.IsEnabled=false;
				reopenTextBox.Text="";
			}
		}

		private void ButtonResend(object sender,RoutedEventArgs e)
		{
			string delException = string.Empty;
			if(IO.SQLAccess.NonQuery
			(
				IO.SQLAccess.dbConn[$"dips -{Globals.Env}"],
				new List<string>
				{
					"SET XACT_ABORT ON",
					"BEGIN TRY",
					"BEGIN TRANSACTION",
					"UPDATE pm",
					"SET [UserInt1]=20",
					"FROM dbo.[ProductMaster] pm",
					"INNER JOIN dbo.[OrderDetail] od",
					"ON pm.[ItemCode]=od.[ItemCode]",
					"WHERE od.[PONUmber]=@PO",
					"",
					"UPDATE dbo.[OrderHeaderDist]",
					"SET [Status]=20",
					"WHERE [PONumber]=@PO",
					"COMMIT TRANSACTION",
					"END TRY",
					"BEGIN CATCH",
					"IF (XACT_STATE()) = -1 ROLLBACK TRANSACTION",
					"ELSE IF(XACT_STATE()) = 1 COMMIT TRANSACTION",
					"END CATCH"
				},
				new List<SqlParameter>()
				{
					new SqlParameter() {ParameterName="@PO", SqlDbType=SqlDbType.VarChar, Value=this.reopenTextBox.Text}
				}
				,CommandType.Text
			))
			{
				try
				{
					foreach(Models.gridFile row in reopenGridXMLFiles.Items)
					{
						if(File.Exists(row.ReopenFileName))
						{
							File.Delete(row.ReopenFileName);
						}
					}
				}
				catch (Exception ex)
				{
					delException=ex.InnerException!=null ? ex.InnerException.Message : ex.Message;
				}
				MessageBox.Show("Successful update", "Success", MessageBoxButton.OK, MessageBoxImage.None);
			}
			else
			{
				MessageBox.Show("Failed update","Failure", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		private void XMLDoubleClick(object sender,MouseButtonEventArgs e)
		{
			if(sender.GetType()==typeof(ListBox))
			{
				ListBox s = sender as ListBox;
				if(s.Items.Count>0)
				{
					ListBox s1 = null;
					string Type = ((ListBox)sender).Name.ToUpper();
					switch(Type)
					{
						case "LISTBOXPO":
							s1 = this.listBoxPO;
							break;
						case "LISTBOXITEM":
							s1=this.listBoxItem;
							break;
						case "LISTBOXSHIP":
							s1=this.listBoxShip;
							break;
						case "LISTBOXOTHER":
							s1=this.listBoxOther;
							break;
					}
					if(s1!=null)
					{
						TextWindow OpenWin = new TextWindow(s1.Items[s1.SelectedIndex].ToString());
						OpenWin.Owner=this;
						OpenWin.ShowDialog();
					}
				}


			}
			else if(sender.GetType()==typeof(DataGrid))
			{
				DataGrid g = sender as DataGrid;
				if(g.Items.Count>-0)
				{
					string Type = ((DataGrid)sender).Name.ToUpper().Substring(0,4);
					var cell = g.SelectedCells[0];

					Models.gridFile name = (Models.gridFile)cell.Item;
					if(name.ReopenFileName!=Globals.NoFilesFound)
					{
						TextWindow OpenWin = new TextWindow(name.ReopenFileName);
						OpenWin.Owner=this;
						OpenWin.ShowDialog();
					}
				}
			}
		}

		private void CheckBox_Checked(object sender,RoutedEventArgs e)
		{
			if(this.reopenGridXMLFiles.Items.Count>0&&this.reopenGridOrderHeader.Items.Count>0&&this.reopenGridProductMaster.Items.Count>0)
			{
				this.reopenButtonReopen.IsEnabled=true;
			}
			else
			{
				MessageBox.Show("Search criteria not met","Invalid Selection",MessageBoxButton.OK,MessageBoxImage.Asterisk);
				this.reopenCheckBox.IsChecked=false;
			}
		}

		private void CheckBox_Unchecked(object sender,RoutedEventArgs e)
		{
			this.reopenButtonReopen.IsEnabled=false;
		}

		private void ReopenGridXMLFiles_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{

		}
	}
}
