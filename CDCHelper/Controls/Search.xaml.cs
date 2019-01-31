﻿using CDCToolbox.Logic;
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
			List<string> StuckFiles = new List<string>();
			StuckFiles.AddRange(Directory.GetFiles(@"\\wmsinterface\Projects\XMLFiles\HPBErr").ToList());
			StuckFiles.AddRange(Directory.GetFiles(@"\\wmsinterface\Projects\XMLFiles\CDCErr").ToList());
			StuckFiles.AddRange(Directory.GetFiles(@"\\wmsinterface\Projects\XMLFiles\ILSErrors").ToList());
			numFiles.Text=StuckFiles.Count.ToString();
			foreach(string file in StuckFiles.OrderBy(x => x))
			{
				dirName.Text=Path.GetDirectoryName(file);
				fileName.Text=Path.GetFileName(file);
				Globals.DoEvents();
				if(File.ReadAllText(file).ToUpper().Contains(searchText.Text.ToUpper()))
				{
					this.listBoxFound.Items.Add(file);
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
	}
}