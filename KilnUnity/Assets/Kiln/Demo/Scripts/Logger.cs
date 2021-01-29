
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kiln
{
    public static class Logger {

#if !UNITY_EDITOR        
        // [Conditional("DEVELOPMENT_BUILD")]
#endif
        public static void Log(string logMsg, LogType logType = LogType.Log, Object obj = null)
        {
            switch (logType)
            {
                case LogType.Assert:
                    UnityEngine.Debug.LogAssertion(logMsg, obj);
                    break;
                
                case LogType.Error:
                    UnityEngine.Debug.LogError(logMsg, obj);
                    break;

                case LogType.Warning:
                    UnityEngine.Debug.LogWarning(logMsg, obj);
                    break;

                default:
                    UnityEngine.Debug.Log(logMsg, obj);
                    break;
            }
        }

#if !UNITY_EDITOR        
        // [Conditional("DEVELOPMENT_BUILD")]
#endif
        public static void Log(Exception exception, Object obj = null)
        {
            UnityEngine.Debug.LogException(exception, obj);
        }
    }
}
