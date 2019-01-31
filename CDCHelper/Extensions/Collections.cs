using CDCToolbox.Logic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace Extensions
{
	public static partial class Extension
	{
		public static Dictionary<string,string> Update(this Dictionary<string,string> dict,string key,string value)
		{
			string Current = MethodBase.GetCurrentMethod().Name;

			try
			{
				if(dict.ContainsKey(key))
				{
					dict[key]=value;
				}
				else
				{
					dict.Add(key,value);
				}
			}
			catch (Exception ex)
			{				
				Globals.AddMessage(Current,"Error",$"Key: {key}, Value: {value}, Error: {ex.InnerMostMessage()}");
			}
			return dict;
		}
		public static SqlBulkCopyColumnMappingCollection AddRange(this SqlBulkCopyColumnMappingCollection collection, List<(string ColumnSource ,string ColumnDest)> Mappings)
		{
			string Current = MethodBase.GetCurrentMethod().Name;

			try
			{
				foreach((string ColumnSource, string ColumnDest) in Mappings)
				{
					collection.Add(ColumnSource,ColumnDest);
				}
			}
			catch (Exception ex)
			{
				Globals.AddMessage(Current,"Error",$"Error: {ex.InnerMostMessage()}");
			}
			return collection;
		}

		public static DataGrid AddMultipleColumns(this DataGrid collection, List<DataGridTextColumn> list)
		{
			string Current = MethodBase.GetCurrentMethod().Name;

			try
			{
				foreach(DataGridTextColumn col in list)
				{
					collection.Columns.Add(col);
				}
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",$"Error: {ex.InnerMostMessage()}");
			}
			return collection;
		}


		public static bool ItemsInList(this string ItemsInList, List<string> Find, char Delimiter)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			bool InList = false;
			try
			{
				string[] items =ItemsInList.ToUpper().Split(',');
				InList = items.Where(find => Find.Contains(find)).Count()>0;
			}
			catch (Exception ex)
			{
				Globals.AddMessage(Current,"Error",$"Items: {ItemsInList}, Find: {Find}, Delimiter: {Delimiter}, Error: {ex.InnerMostMessage()}");
				InList=false;
			}
			return InList;
		}

		public static bool ItemInList(this string ItemsInList, string Find, char Delimiter)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			bool InList = false;
			try
			{
				string[] items = ItemsInList.ToUpper().Split(',');
				InList=items.Where(find => find==Find).Count()>0;
			}
			catch (Exception ex)
			{
				Globals.AddMessage(Current,"Error",$"Items: {ItemsInList}, Find: {Find}, Delimiter: {Delimiter}, Error: {ex.InnerMostMessage()}");
				InList=false;
			}
			return InList;
		}

		public static List<(int Order, string User, string Pass)> UserSplit(this string ItemList)
		{
			int Ord = 0;
			int SetCount = 0;
			string Usr = "";
			string Pwd = "";

			string[] items1 = ItemList.Split(new char[] { ']','[' }, StringSplitOptions.RemoveEmptyEntries);
			List<string> items2 = new List<string>();
			List<(int Order, string User, string Pass)> list = new List<(int Order, string User, string Pass)>();
			foreach(string item in items1)
			{
				if(item.StartsWith("[")&&!item.EndsWith("]"))
				{
					items2.Add(item.Substring(item.IndexOf("[")+1));
				}
				else if(item.EndsWith("]")&&!item.StartsWith("["))
				{
					items2.Add(item.Substring(0,item.Length-1));
				}
			}
			string[] param;
			foreach(string item in items2)
			{
				param=item.Split(',');
				if(param.Length==3)
				{
					SetCount=0;
					Ord=-1;
					Usr=string.Empty;
					Pwd=string.Empty;
					for(int i=0;i<param.Length;i++)
					{
						if(param[i].Contains("="))
						{
							switch(param[i].Substring(0,param[i].IndexOf("=")))
							{
								case "ORDER":
									Ord=int.Parse(param[i].Substring(param[i].IndexOf("=")+1));
									SetCount++;
									break;
								case "USER":
									Usr=param[i].Substring(param[i].IndexOf("=")+1);
									if(Usr.StartsWith("\"")&&Usr.EndsWith("\""))
									{
										Usr=Usr.Substring(1,Usr.Length-2);
									}
									SetCount++;
									break;
								case "PASSWORD":
									Pwd=param[i].Substring(param[i].IndexOf("=")+1);
									if(Pwd.StartsWith("\"")&&Pwd.EndsWith("\""))
									{
										Pwd=Pwd.Substring(1,Pwd.Length-2);
									}
									SetCount++;
									break;
							}
						}
						if(SetCount==3)
						{
							list.Add((Ord, Usr, Pwd));
						}
					}
				}
			}
			return list;

		}

		public static T ForEnvironment<T>(this string ItemList, string Environment="", string Delimiter = "")
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			string[] Items;
			dynamic EnvValue = null;
			string temp = string.Empty;
			string Env = string.Empty;
			string Del = string.Empty;

			try
			{
				Env= string.IsNullOrEmpty(Environment) ? Globals.Env : Environment;
				Del= string.IsNullOrEmpty(Delimiter) ? Globals.Delimiter : Delimiter;
				Items=ItemList.Split(Del[0]);
				if(Items.Length> 0)
				{
					temp=(from f in Items.AsEnumerable()
						  where f.StartsWith($"{Env.ToUpper()}=")
						  select f).First().ToString();
					if(temp.Length > 0)
					{
						temp=temp.Replace($"{Env.ToUpper()}=","");
						
						if(temp.StartsWith("{")&&temp.EndsWith("}")&&temp.Length>2)
						{
							temp=temp.Substring(1,temp.Length-2);
						}
						else if(temp.StartsWith("{")&&temp.EndsWith("}")&&temp.Length==2)
						{
							temp=string.Empty;
						}
					}
					if(typeof(T)==typeof(string))
					{
						EnvValue=temp;
					}
					else if(typeof(T)==typeof(bool))
					{
						if(temp!="")
						{
							if(bool.TryParse(temp,out bool t))
							{
								EnvValue=t;
							}
							else
							{
								EnvValue=false;
							}
						}
						else
						{
							EnvValue=default(bool);
						}
					}
					else if(typeof(T)==typeof(int))
					{
						if(temp!="")
						{
							if(int.TryParse(temp,out int t))
							{
								EnvValue=t;
							}
							else
							{
								EnvValue=default(int);
							}
						}
						else
						{
							EnvValue=default(int);
						}
					}
				}
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",$"ItemList: {ItemList}, Env: {Env}, Del: {Del}, Error: {ex.InnerMostMessage()}");
			}
			return (T)EnvValue;
		}


		public static string[] FTPResponseToArray(this List<(DateTime Date, string Location, string Message)> Responses)
		{
			string[] result;
		
			try
			{
				result=(from r in Responses.AsEnumerable()
						orderby r.Date
						select r.Date.ToShortTimeString()+" "+r.Location+" "+r.Message).ToArray();

			}
			catch(Exception ex)
			{
				result=new string[] { "" };
			}
			return result;
		}

		public static string FTPResponseToString(this List<(DateTime Date, string Location, string Message)> Responses, string JoinChar)
		{
			return string.Join(JoinChar,Responses.FTPResponseToArray());
		}	
	}
}
