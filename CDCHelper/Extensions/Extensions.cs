using CDCToolbox.Logic;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Extensions
{
	public static partial class Extension
	{

		public static dynamic Provided<T>(this List<string> items,object ordinal)
		{
			string Current = MethodBase.GetCurrentMethod().Name;
			dynamic result;

			try
			{
				result=(items.Count-1>=(int)ordinal) ? items[(int)ordinal] : null;

			}
			catch(Exception ex)
			{
				Globals.AddMessage(Current,"Error",ex.InnerMostMessage());
				result=null;
			}
			if(result==null)
			{
				if(typeof(T)==typeof(string))
				{
					result =string.Empty;
				}
			}
			return (T)result;
		}
	}
}