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
        private ILogHandler defaultHandler;
        private LogType level;

        public LogAggregator()
        {
            logs = new List<string>();
            level = LogType.Log;
        }

        public void StartAggregating()
        {
            defaultHandler = Debug.logger.logHandler;
            Debug.logger.logHandler = this;
            level = LogType.Log;
        }

        public void LogFormat(LogType type, UnityEngine.Object _object, string format, params object[] elements)
        {
            logs.Add(string.Format(format, elements));
            if ((int)type < (int)level) level = type;
        }

        public void LogException(Exception exception, UnityEngine.Object _object)
        {
            logs.Add(exception.ToString());
            level = LogType.Error;
        }

        public void FinishAggregating(string regularMessage, string warningMessage, string errorMessage)
        {
            Debug.logger.logHandler = defaultHandler;
            string agglomerated = string.Join("\n", logs.ToArray());
            switch (level)
            {
                case LogType.Log:
                    agglomerated = regularMessage + "\n" + agglomerated;
                    Debug.Log(agglomerated);
                    break;

                case LogType.Warning:
                    agglomerated = warningMessage + "\n" + agglomerated;
                    Debug.LogWarning(agglomerated);
                    break;

                case LogType.Error:
                    agglomerated = errorMessage + "\n" + agglomerated;
                    Debug.LogError(agglomerated);
                    break;

                case LogType.Assert:
                    agglomerated = regularMessage + "\n" + agglomerated;
                    Debug.Log(agglomerated);
                    break;

                case LogType.Exception:
                    agglomerated = errorMessage + "\n" + agglomerated;
                    Debug.LogError(agglomerated);
                    break;

                default:
                    agglomerated = regularMessage + "\n" + agglomerated;
                    Debug.Log(agglomerated);
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
            Debug.logger.logHandler = defaultHandler;
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
