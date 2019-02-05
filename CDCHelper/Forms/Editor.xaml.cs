using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CDCToolbox
{
	/// <summary>
	/// Interaction logic for TextEditor.xaml
	/// </summary>
	public partial class Editor : Window
	{

		internal string EditFileName = string.Empty;

		public Editor()
		{
			InitializeComponent();
			this.Title=string.Empty;
		}

		public Editor(string file)
		{
			InitializeComponent();
			EditFileName=file;

			editor.Load(file);
			this.Title=file;
		}

		private void Button_Click(object sender,RoutedEventArgs e)
		{

		}

		private void Button_Click_1(object sender,RoutedEventArgs e)
		{

		}

		private void Button_Click_2(object sender,RoutedEventArgs e)
		{

		}
	}
}
