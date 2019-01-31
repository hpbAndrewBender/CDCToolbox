using CDCToolbox.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Extensions
{
	public static partial class Extension
	{
		public static List<(string FullName, string Name, Type DataType)> PropertyInfo<T>(this T item)
		{
			string Current = MethodBase.GetCurrentMethod().Name;

			List<(string FullName, string Name, Type DataType)> props = new List<(string FullName, string Name, Type DataType)>();
			if(item!=null)
			{
				foreach(FieldInfo field in item.GetType().BaseType.BaseType.GetRuntimeFields())
				{
					if(field.Name.Contains("<")&&field.Name.Contains(">"))
					{
						props.Add((field.Name, field.Name.Substring(field.Name.IndexOf("<")+1,field.Name.IndexOf(">")-1), field.FieldType));
					}
					else
					{
						props.Add((field.Name, field.Name, field.FieldType));
					}
				}
				if(item.GetType().BaseType.BaseType.FullName.Contains(".Abstract."))
				{
					foreach(FieldInfo field in item.GetType().BaseType.GetRuntimeFields().Where(e => Globals.SaveControlFields.Any(a => e.Name.Contains(a))).ToList())
					{
						props.Add((field.Name, field.Name.Substring(field.Name.IndexOf("<")+1,field.Name.IndexOf(">")-1), field.FieldType));
					}
				}
				var removals = (from p in props where (p.Name.Contains("Meta_") || p.Name.Contains("Reserved_")) select p).ToList();
				if(removals.Count>0)
				{
					foreach((string FullName, string Name, Type DataType) removal in removals)
					{
						props.Remove(removal);
					}
				}
			}
			else
			{
				Globals.AddMessage(Current,"Error",$"    Item is null--{item.GetType().FullName}");
			}
			return props;
		}
	}
}