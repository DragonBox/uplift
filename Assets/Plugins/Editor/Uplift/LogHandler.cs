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
#if UNITY_2017_1_OR_NEWER
			previouslyFilteredType = Debug.unityLogger.filterLogType;
			Debug.unityLogger.filterLogType = logType;
#else
			previouslyFilteredType = Debug.logger.filterLogType;
			Debug.logger.filterLogType = logType;
#endif

			showStackTrace = showStack;
			logFileName = fileName;

			OutputStream = new StreamWriter(logFileName, appendToCurrentLogFile);

			Application.logMessageReceived += HandleLog;
		}

		public void Dispose()
		{
			Application.logMessageReceived -= HandleLog;
#if UNITY_2017_1_OR_NEWER
			Debug.unityLogger.filterLogType = previouslyFilteredType;
#else
			Debug.logger.filterLogType = previouslyFilteredType;
#endif

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