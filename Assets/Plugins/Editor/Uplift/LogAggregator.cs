// --- BEGIN LICENSE BLOCK ---
/*
 * Copyright (c) 2017-present WeWantToKnow AS
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// --- END LICENSE BLOCK ---

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Uplift
{
#if !UNITY_5_3_OR_NEWER
    interface ILogHandler {
        void LogException(Exception exception, UnityEngine.Object context);
        void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args);
    }
    class LogHandlerUtils {
        public static ILogHandler ReplaceLogHandler(ILogHandler newLogHandler) {
            Debug.Log("On Unity < 5.3, Uplift doesn't aggregate logs");
            return newLogHandler;
        }
    }
#else
    class LogHandlerUtils {
        public static ILogHandler ReplaceLogHandler(ILogHandler newLogHandler) {
            ILogHandler currentLogHandler = Debug.logger.logHandler;
            Debug.logger.logHandler = newLogHandler;
            return currentLogHandler;
        }
    }
#endif

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
            originalHandler = LogHandlerUtils.ReplaceLogHandler(this);
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
            LogHandlerUtils.ReplaceLogHandler(originalHandler);
            FinishAggregating();
        }
    }
}
