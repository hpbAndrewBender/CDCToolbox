using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using CDCToolbox.Logic;

namespace CDCToolbox.Controls
{
	/// <summary>
	/// Interaction logic for ErrorFiles.xaml
	/// </summary>
	public partial class ErrorFiles : UserControl
	{
		public ErrorFiles()
		{
			InitializeComponent();

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
				else if(file.EndsWith("_sopILSImport.xml"))
				{
					this.listBoxShip.Items.Add(file);
				}
				else
				{
					this.listBoxOther.Items.Add(file);
				}
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
							s1=this.listBoxPO;
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
						OpenWin.Owner=VisualTreeHelper.GetParent(this) as Window;
						OpenWin.ShowDialog();
					}
				}
			}		
		}
	}
}