using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;

namespace CDCToolbox
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class TextWindow : Window
	{
		public string FileName { get; set; }

		public string MoveFile { get; internal set; }

		public TextWindow()
		{
			MoveFile=string.Empty;
			InitializeComponent();
		}

		public TextWindow(string FileName)
		{
			MoveFile=string.Empty;
			InitializeComponent();
			this.FileName=FileName;
			if(!string.IsNullOrEmpty(FileName))
			{
				this.Title=FileName;

				string alltext = System.IO.File.ReadAllText(FileName);
				string temp = alltext.ToLower();
				if(temp.Contains("<error>")&&temp.Contains("</error>"))
				{
					this.errorText.Text=temp.Substring(temp.IndexOf("<error>")+"<error>".Length,temp.IndexOf("</error>")-temp.IndexOf("<error>")-"<error>".Length).ToUpper();
				}
				this.allText.Text=alltext.Replace("&gt;",">").Replace("&lt;","<");
			}
		}


		private void Button_Handler(object sender,RoutedEventArgs e)
		{

			if(sender!=null && sender.GetType()==typeof(Button))
			{
				Button senderButton = (Button)sender;

				switch(senderButton.Content.ToString().ToUpper())
				{
					case "REPROCESS":
						DirectoryInfo di = new DirectoryInfo(FileName);
						MoveFile = di.FullName.Replace(di.Parent.ToString()+"\\","");
						try
						{
							File.Move(FileName,MoveFile);
						}
						catch(Exception ex)
						{
							Console.WriteLine(FileName);
						}
						break;

					case "CLOSE":
						Close();
						break;
				}
			}

		}
	}
}
