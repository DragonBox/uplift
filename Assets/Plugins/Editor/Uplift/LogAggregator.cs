using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Uplift
{
    class LogAggregator : ILogHandler, IDisposable
    {
        private List<string> logs;
        private ILogHandler originalHandler;
        private LogType aggregatedLevel;
        private bool started;
        private string onSuccess;
        private string onWarning;
        private string onError;

        public static LogAggregator InUnity(string onSuccess, string onWarning, string onError)
        {
            LogAggregator LA = new LogAggregator(onSuccess, onWarning, onError);
            LA.StartAggregating();
            return LA;
        }

        public static LogAggregator InUnity(string onSuccesFormat, string onWarningFormat, string onErrorFormat, params object[] args)
        {
            return InUnity(
                string.Format(onSuccesFormat, args),
                string.Format(onWarningFormat, args),
                string.Format(onErrorFormat, args)
                );
        }

        public LogAggregator(string onSuccess, string onWarning, string onError)
        {
            logs = new List<string>();
            started = false;
            this.onSuccess = onSuccess;
            this.onWarning = onWarning;
            this.onError = onError;
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
            logs.Add(string.Format("[" + type.ToString().ToUpper() + "]\t" + format, elements));
            if ((int)type < (int)aggregatedLevel) aggregatedLevel = type;
        }

        public void LogException(Exception exception, UnityEngine.Object _object)
        {
            logs.Add(exception.ToString());
            aggregatedLevel = LogType.Error;
        }

        private void FinishAggregating()
        {
            if (!started) return;
            string concatenated = string.Join("\n", logs.ToArray());
            string regularConcatenated = onSuccess + "\n" + concatenated;
            string warningConcatenated = onWarning + "\n" + concatenated;
            string errorConcatenated = onError + "\n" + concatenated;
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
            started = false;
        }

        public void Dispose()
        {
            if (!started) return;
            Debug.logger.logHandler = originalHandler;
            FinishAggregating();
        }
    }
}
