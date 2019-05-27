using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public class LogHandler : IDisposable
{
	public bool ShowStackTrace = false;
	public string LogFileName = "log.txt";

	private string output = "";
	private string stack = "";

	private StreamWriter OutputStream;

	public LogHandler(string logFileName = "uplift.log", bool appendToCurrentLogFile = false, bool showStackTrace = false)
	{
		ShowStackTrace = showStackTrace;
		LogFileName = logFileName;

		OutputStream = new StreamWriter(logFileName, appendToCurrentLogFile);

		Application.logMessageReceived += HandleLog;
	}

	public void Dispose()
	{
		Application.logMessageReceived -= HandleLog;

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

		OutputStream.WriteLine("[" + type + "]" + output);
		OutputStream.Flush();

		if (ShowStackTrace)
		{
			OutputStream.WriteLine("[--> Stack Trace <--]");
			OutputStream.WriteLine(stack + "[-------------------]\n");
			OutputStream.Flush();
		}
	}
}