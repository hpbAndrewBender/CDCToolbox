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
	}


}
