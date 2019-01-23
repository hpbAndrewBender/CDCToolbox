using CDCHelper.Logic;
using Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace CDCHelper.IO
{
	internal class SQLAccess
	{
		public static Dictionary<string,string> dbConn = new Dictionary<string,string>();

		public static string GenerateConnect(string server,string catalog,string security,string user,string password,string integrated)
		{
			if(integrated=="True")
			{
				return $"Data Source={server};Initial Catalog={catalog};Integrated Security={security};";
			}
			else
			{
				return $"Data Source={server};Initial Catalog={catalog};Persist Security Info={security};User ID={user};Password={password}";
			}
		}

		public static void InsertServer(List<(string name, string env, string server, string catalog, string security, string user, string password, string integrated)> dbservers)
		{
			foreach(var (name, env, server, catalog, security, user, password, integrated) in dbservers)
			{
				dbConn.Update
					(
						$"{name}-{env.Trim()}",
						GenerateConnect
						(
							server.Trim(),
							catalog.Trim(),
							security.Trim(),
							user,
							password,
							integrated
						)
					);
			}
		}

		public static DataSet Set(string connection,List<string> query,List<SqlParameter> prms,CommandType type = CommandType.StoredProcedure)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			DataSet data = null;
			SqlConnection connect = null;
			SqlCommand command = null;
			SqlDataAdapter adapter = null;
			try
			{
				using(connect=new SqlConnection(connection))
				{
					command=new SqlCommand(string.Join(" ",query.ToArray()),connect)
					{
						CommandType=type
					};
					if(prms!=null&&prms.Count>0)
					{
						command.Parameters.AddRange(prms.ToArray());
					}
					connect.Open();
					adapter=new SqlDataAdapter(command);
					data=new DataSet();
					adapter.Fill(data);
					adapter.Dispose();
					connect.Close();
				}
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
				data=null;
			}
			return data;
		}

		public static DataTable Table(string connection,List<string> query,List<SqlParameter> prms,CommandType type = CommandType.StoredProcedure)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			DataTable data = null;
			SqlConnection connect = null;
			SqlCommand command = null;
			SqlDataAdapter adapter = null;
			try
			{
				using(connect=new SqlConnection(connection))
				{
					command=new SqlCommand(string.Join(" ",query.ToArray()),connect)
					{
						CommandType=type
					};
					if(prms!=null&&prms.Count>0)
					{
						command.Parameters.AddRange(prms.ToArray());
					}
					connect.Open();
					adapter=new SqlDataAdapter(command);
					data=new DataTable();
					adapter.Fill(data);
					adapter.Dispose();
					connect.Close();
				}
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
				data=null;
			}
			return data;
		}

		public static DataRow SingleRow(string connection,List<string> query,List<SqlParameter> prms,int rowNumber = 0,CommandType type = CommandType.StoredProcedure)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			DataRow row = null;
			DataTable tbl = null;

			try
			{
				tbl=Table(connection,query,prms,type);
				if(tbl!=null&&tbl.Rows.Count>0)
				{
					row=tbl.Rows[rowNumber];
				}
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
				row=null;
			}
			return row;
		}

		public static int RowCount(string connection,List<string> query,List<SqlParameter> prms,CommandType type = CommandType.StoredProcedure)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			int count = 0;

			try
			{
				count=Table(connection,query,prms,type).Rows.Count;
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
				count=-1;
			}
			return count;
		}

		public static bool NonQuery(string connection,List<string> query,List<SqlParameter> prms,CommandType type = CommandType.StoredProcedure)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			bool data = false;
			SqlConnection connect = null;
			SqlCommand command = null;

			try
			{
				using(connect=new SqlConnection(connection))
				{
					command=new SqlCommand(string.Join(" ",query.ToArray()),connect)
					{
						CommandType=type
					};
					if(prms!=null&&prms.Count>0)
					{
						command.Parameters.AddRange(prms.ToArray());
					}
					connect.Open();
					command.ExecuteNonQuery();
					connect.Close();
					data=true;
				}
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
				data=false;
			}
			return data;
		}

		public static T Scalar<T>(string connection,List<string> query,List<SqlParameter> prms,T defaultValue,CommandType type = CommandType.StoredProcedure)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			T data = defaultValue;
			SqlConnection connect = null;
			SqlCommand command = null;

			try
			{
				using(connect=new SqlConnection(connection))
				{
					command=new SqlCommand(string.Join(" ",query.ToArray()),connect)
					{
						CommandType=type
					};
					if(prms!=null&&prms.Count>0)
					{
						command.Parameters.AddRange(prms.ToArray());
					}
					connect.Open();
					data=(T)command.ExecuteScalar();
					connect.Close();
				}
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
				data=defaultValue;
			}
			return data;
		}

		public static SqlDataReader Read(string connection,List<string> query,List<SqlParameter> prms,CommandType type = CommandType.StoredProcedure)
		{
			string Current = MethodBase.GetCurrentMethod().Name;

			try
			{
				SqlConnection connect = new SqlConnection(connection);
				using(SqlCommand command = new SqlCommand(string.Join(" ",query.ToArray()),connect))
				{
					command.CommandType=type;
					if(prms!=null&&prms.Count>0)
					{
						command.Parameters.AddRange(prms.ToArray());
					}
					connect.Open();
					SqlDataReader data = command.ExecuteReader(CommandBehavior.CloseConnection);
					return data;
				}
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
				return null;
			}
		}

		public static List<object> OutParam(string connection,List<string> query,List<SqlParameter> prms,CommandType type = CommandType.StoredProcedure)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			SqlConnection connect = null;
			SqlCommand command = null;
			List<object> data = new List<object>();
			List<string> parameterNames = new List<string>();

			try
			{
				foreach(SqlParameter p in prms)
				{
					if(p.Direction==ParameterDirection.Output)
					{
						parameterNames.Add(p.ParameterName);
						break;
					}
				}
				using(connect=new SqlConnection(connection))
				{
					command=new SqlCommand(string.Join(" ",query.ToArray()),connect)
					{
						CommandType=type
					};
					if(prms!=null&&prms.Count>0)
					{
						command.Parameters.AddRange(prms.ToArray());
					}
					connect.Open();
					command.ExecuteNonQuery();
					if(parameterNames.Count>0)
					{
						foreach(string paramName in parameterNames)
						{
							data.Add(command.Parameters[paramName].Value);
						}
					}
					connect.Close();
				}
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
				data=null;
			}
			return data;
		}

		public static DataTable CreateDataTable(List<(string FullName, string Name, Type DataType)> props)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			DataTable table = new DataTable();
			DataColumn column = null;

			try
			{
				foreach((string FullName, string Name, Type DataType) in props)
				{
					column=new DataColumn
					{
						ColumnName=Name,
						DataType=DataType,
					};
					table.Columns.Add(column);
				}
			}
			catch (Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
			}
			return table;
		}

		public static DataTable CreateDataTable(List<(string FullName, string Name, Type DataType)> props,DataTable ordinals)
		{
			string Current = MethodBase.GetCurrentMethod().Name;

			DataTable table = new DataTable();

			table=CreateDataTable(props);
			if(ordinals!=null&&ordinals.Rows.Count>0)
			{
				string fieldname = string.Empty;
				int ordinalnum = 0;
				foreach(DataRow row in ordinals.Rows)
				{
					fieldname=row["name"].ToString();
					ordinalnum=(int)row["column_id"]-1; // zero base the array
					if(table.Columns.Contains(fieldname))
					{
						table.Columns[fieldname].SetOrdinal(ordinalnum);
						Globals.AddMessage(Current, "Info", $"    Setting ordinal {ordinalnum} to {fieldname}");
					}
					else
					{
						Globals.AddMessage(Current, "Info", $"    Missing field name={fieldname}");
					}
				}
			}
			return table;
		}

		public static bool BulkUploadTable(string connection,List<string> query,List<SqlParameter> prms,DataTable table,CommandType type = CommandType.StoredProcedure)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			bool result = false;

			try
			{
				SqlConnection connect = new SqlConnection(connection);
				if(table!=null&&table.Rows.Count>0)
				{
					using(SqlCommand command = new SqlCommand(string.Join(" ",query.ToArray()),connect))
					{
						command.CommandType=type;
						if(prms!=null&&prms.Count>0)
						{
							command.Parameters.AddRange(prms.ToArray());
						}
						connect.Open();
						using(SqlBulkCopy bulk = new SqlBulkCopy(connect))
						{
							bulk.DestinationTableName=string.Join(" ",query.ToArray());
							bulk.WriteToServer(table);
						}
						result=true;
					}
				}
				else
				{
					result=false;
				}
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
				result=false;
			}
			return result;
		}

		public static bool BulkUploadTableWithMappings(string connection,List<string> query,List<(string ColumnSource, string ColumnDesc)> Mapping,List<SqlParameter> prms,DataTable table,CommandType type = CommandType.StoredProcedure)
		{
			bool result = false;
			string Current = MethodBase.GetCurrentMethod().Name;

			try
			{
				SqlConnection connect = new SqlConnection(connection);
				if(table!=null&&table.Rows.Count>0)
				{
					using(SqlCommand command = new SqlCommand(string.Join(" ",query.ToArray()),connect))
					{
						command.CommandType=type;
						if(prms!=null&&prms.Count>0)
						{
							command.Parameters.AddRange(prms.ToArray());
						}
						connect.Open();
						using(SqlBulkCopy bulk = new SqlBulkCopy(connect))
						{
							bulk.DestinationTableName=string.Join(" ",query.ToArray());
							if(Mapping!=null&&Mapping.Count>0)
							{
								foreach((string Source, string Dest) in Mapping)
								{
									bulk.ColumnMappings.Add(Source,Dest);
								}
							}
							bulk.WriteToServer(table);
						}
						result=true;
					}
				}
				else
				{
					result=false;
				}
			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
				result=false;
			}
			return result;
		}
	}
}