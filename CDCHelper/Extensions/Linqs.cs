using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
	public static partial class Extension
	{
		public static List<T> RetrieveAll<T>(this List<(string Filename, string ModelName, dynamic Model)> data,string find)
		{
			return (from d in data.AsEnumerable()
					where d.ModelName.StartsWith(find)
					select (T)d.Model).ToList();
		}

		public static T RetrieveOne<T>(this List<(string Filename, string ModelName, dynamic Model)> data,string find)
		{
			return (from d in data.AsEnumerable()
					where d.ModelName.StartsWith(find)
					select (T)d.Model).FirstOrDefault();
		}
	}
}