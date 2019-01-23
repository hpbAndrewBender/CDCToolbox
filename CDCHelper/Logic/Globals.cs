using System;
using System.Collections.Generic;

namespace CDCHelper.Logic
{
	internal class Globals
	{
		public static ProgramProperties Props
		{
			get
			{
				return new ProgramProperties();
			}
		}

		public static string Env { get; set; }

		public static List<(DateTime Date, string Location, string ErrorLEvel, string Message)> Message { get; set; }
		public static List<(DateTime Date, string Location, string Message)> FTPResponseMessage { get; set; }
		public static string CurrentGroupIdentifier = string.Empty;
		public static string CurrentTransactionIdentifier = string.Empty;
		public static bool TDSOrTXI = false;

		public static string NoFilesFound = "{No files found}";

		public static bool ErrorTerminal { get; set; }

		public static string Delimiter = "|";

		public static long CurrentLineNumber = 0;

		public static void AddMessage(string Location, string ErrorLevel, string Message)
		{
			Globals.Message.Add( (DateTime.Now, Location, ErrorLevel, Message));
		}

		public static void AddFTPResponse(string Location, string Message)
		{
			Globals.FTPResponseMessage.Add((DateTime.Now, Location, Message));
		}

		public static List<string> SaveControlFields = new List<string>
		{
			"ControlNumberGroup",
		};

	}
}