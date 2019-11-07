using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

namespace Uplift
{
	public class LogHandler : IDisposable
	{
		public bool showStackTrace = false;
		public string logFileName = "log.txt";

		private string output = "";
		private string stack = "";

		private StreamWriter OutputStream;
		private LogType previouslyFilteredType;

		public LogHandler(
			string fileName = "uplift.log",
			bool appendToCurrentLogFile = false,
			bool showStack = false,
			LogType logType = LogType.Log)
		{
			previouslyFilteredType = Debug.logger.filterLogType;
			Debug.logger.filterLogType = logType;

			showStackTrace = showStack;
			logFileName = fileName;

			OutputStream = new StreamWriter(logFileName, appendToCurrentLogFile);

			Application.logMessageReceived += HandleLog;
		}

		public void Dispose()
		{
			Application.logMessageReceived -= HandleLog;
			Debug.logger.filterLogType = previouslyFilteredType;

			if (OutputStream != null)
			{
				OutputStream.Close();
				OutputStream = null;
			}
		}

		void HandleLog(string logString, string stackTrace, LogType type)
		{
			output = logString;
			stack = stackTrace;

			OutputStream.WriteLine("[" + type.ToString().ToUpper() + "] " + output);
			OutputStream.Flush();

			if (showStackTrace)
			{
				OutputStream.WriteLine("[--> Stack Trace <--]");
				OutputStream.WriteLine(stack + "[-------------------]\n");
				OutputStream.Flush();
			}
		}
	}
}