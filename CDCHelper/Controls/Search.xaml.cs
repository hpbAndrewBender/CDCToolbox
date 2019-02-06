using CDCToolbox.Logic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CDCToolbox.Controls
{
	/// <summary>
	/// Interaction logic for Search.xaml
	/// </summary>
	public partial class Search : UserControl
	{
		public Search()
		{
			InitializeComponent();
		}

		private void ButtonSearch_Click(object sender,RoutedEventArgs e)
		{
			int numberToSearch = 0;
			List<string> StuckFiles = new List<string>();
			StuckFiles.AddRange(Directory.GetFiles(@"\\wmsinterface\Projects\XMLFiles\HPBErr").ToList());
			StuckFiles.AddRange(Directory.GetFiles(@"\\wmsinterface\Projects\XMLFiles\CDCErr").ToList());
			StuckFiles.AddRange(Directory.GetFiles(@"\\wmsinterface\Projects\XMLFiles\ILSErrors").ToList());
			numberToSearch=StuckFiles.Count;
			numFiles.Text=StuckFiles.Count.ToString();
			foreach(string file in StuckFiles.OrderBy(x => x))
			{
				numFiles.Text=(numberToSearch--).ToString();
				dirName.Text=Path.GetDirectoryName(file);
				fileName.Text=Path.GetFileName(file);
				Globals.DoEvents();
				if(File.ReadAllText(file).ToUpper().Contains(searchText.Text.Trim().ToUpper()))
				{
					this.listBoxFound.Items.Add(file);
				}
			}
			numFiles.Text="0";
			Globals.DoEvents();
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
						case "LISTBOXFOUND":
							s1=this.listBoxFound;
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

		private void ButtonClear_Click(object sender,RoutedEventArgs e)
		{
			this.listBoxFound.Items.Clear();
			this.listBoxFound.Items.Refresh();

			numFiles.Text=string.Empty;
			dirName.Text=string.Empty;
			fileName.Text=string.Empty;
		}
	}
}