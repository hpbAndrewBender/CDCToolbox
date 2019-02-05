using CDCToolbox.Logic;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace CDCToolbox
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
		}
	}
}