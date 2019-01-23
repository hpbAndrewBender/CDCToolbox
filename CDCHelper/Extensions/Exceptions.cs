using System;

namespace Extensions
{
	public static partial class Extension
	{
		public static string InnerMostMessage(this Exception ex)
		{
			string InnerMessage = string.Empty;
			if(ex.InnerException!=null)
			{
				InnerMessage=ex.InnerException.Message;
			}
			else
			{
				InnerMessage=ex.Message;
			}
			return InnerMessage;
		}
	}
}
