using System;
using System.Collections.Generic;
using System.Data;
using CDCHelper.Logic;

namespace Extensions
{
	public static partial class Extension
	{
		public static DataTable UpdateAllRows(this DataTable table, string FieldName, dynamic value)
		{
			if(table!=null && table.Rows.Count > 0 && table.Columns.Contains(FieldName))
			{
				for(int i=0; i<table.Rows.Count;i++)
				{
					table.Rows[i][FieldName]=value;
				}
			}
			return table;

		}

		private static DataTable GetOrdinalsFromSQL(string CommandName)
		{
			DataTable ordinals = null;

			ordinals=CDCHelper.IO.SQLAccess.Table
				(
					CDCHelper.IO.SQLAccess.dbConn[$"edi  -{Globals.Env}"],
					new List<string>
					{
						"select c.name, c.column_id",
						"from sys.tables t",
						"inner join sys.columns c",
						"on c.object_id=t.object_id",
						"where t.name=@tablename;"
					},
					new List<System.Data.SqlClient.SqlParameter>
					{
						new System.Data.SqlClient.SqlParameter() { ParameterName="@tablename", SqlDbType=SqlDbType.VarChar, Value=$"importEDI_{CommandName}" }
					},
					CommandType.Text
				);
			return ordinals;
		}		
	}
}
