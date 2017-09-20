using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Uplift
{
    class LogAggregator : ILogHandler
    {
        private List<string> logs;
        private ILogHandler originalHandler;
        private LogType aggregatedLevel;
        private bool started;

        public LogAggregator()
        {
            logs = new List<string>();
            aggregatedLevel = LogType.Log;
            started = false;
        }

        public void StartAggregating()
        {
            if (started) return;
            originalHandler = Debug.logger.logHandler;
            Debug.logger.logHandler = this;
            aggregatedLevel = LogType.Log;
            started = true;
        }

        public void LogFormat(LogType type, UnityEngine.Object _object, string format, params object[] elements)
        {
            logs.Add(string.Format(type.ToString() + " " + format, elements));
            if ((int)type < (int)aggregatedLevel) aggregatedLevel = type;
        }

        public void LogException(Exception exception, UnityEngine.Object _object)
        {
            logs.Add(exception.ToString());
            aggregatedLevel = LogType.Error;
        }

        public void FinishAggregating(string regularMessage, string warningMessage, string errorMessage)
        {
            if (!started) return;
            Debug.logger.logHandler = originalHandler;
            string concatenated = string.Join("\n", logs.ToArray());
            string regularConcatenated = regularMessage + "\n" + concatenated;
            string warningConcatenated = warningMessage + "\n" + concatenated;
            string errorConcatenated = errorMessage + "\n" + concatenated;
            switch (aggregatedLevel)
            {
                case LogType.Log:
                    Debug.Log(regularConcatenated);
                    break;

                case LogType.Warning:
                    Debug.LogWarning(warningConcatenated);
                    break;

                case LogType.Error:
                    Debug.LogError(errorConcatenated);
                    break;

                case LogType.Assert:
                    Debug.Log(regularConcatenated);
                    break;

                case LogType.Exception:
                    Debug.LogError(errorConcatenated);
                    break;

                default:
                    Debug.Log(regularConcatenated);
                    break;
            }
            logs = new List<string>();
        }

        public void FinishAggregating(string regularFormat, string warningFormat, string errorFormat, params object[] elements)
        {
            FinishAggregating(
                string.Format(regularFormat, elements),
                string.Format(warningFormat, elements),
                string.Format(errorFormat, elements)
                );
        }

        ~LogAggregator()
        {
            Debug.logger.logHandler = originalHandler;
            if(logs.Count > 0)
            {
                FinishAggregating(
                    "The aggregator was destroyed with regular logs not logged yet",
                    "The aggregator was destroyed with warning logs not logged yet",
                    "The aggregator was destroyed with error logs not logged yet"
                    );
            }
        }
    }
}
