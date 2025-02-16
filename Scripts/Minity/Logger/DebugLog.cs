using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Minity.Logger
{
    internal static class DebugLog
    {
#if UNITY_EDITOR
        private const string PREFIX =
            "<b><color=white>[</color>" +
            "<color=#f39dff>M</color>" +
            "<color=#eeb6ff>i</color>" +
            "<color=#d7d2ff>n</color>" +
            "<color=#c3dbff>i</color>" +
            "<color=#b2dfff>t</color>" +
            "<color=#a2e0ff>y</color>" +
            "<color=white>]</color></b> ";
        
        private enum LogLevel
        {
            Info, Warning, Error
        }

        private const string LogLevelKey = "_unity_debug_milutools_log_level";
        
        private static LogLevel _logLevel = LogLevel.Info;

        [InitializeOnEnterPlayMode]
        private static void ReadConfig()
        {
            _logLevel = (LogLevel)EditorPrefs.GetInt(LogLevelKey, (int)LogLevel.Warning);
        }
        
        [MenuItem("Minity/Log Level/Error", false, 2)]
        private static void SwitchLogLevelError()
        {
            SwitchLogLevel(LogLevel.Error);
        }
        
        [MenuItem("Minity/Log Level/Warning", false, 1)]
        private static void SwitchLogLevelWarning()
        {
            SwitchLogLevel(LogLevel.Warning);
        }
        
        [MenuItem("Minity/Log Level/Info", false, 0)]
        private static void SwitchLogLevelInfo()
        {
            SwitchLogLevel(LogLevel.Info);
        }
        
        private static void SwitchLogLevel(LogLevel level)
        {
            _logLevel = level;
            EditorPrefs.SetInt(LogLevelKey, (int)level);
            EditorUtility.DisplayDialog("Minity", "Switched Milutools log level to " + level, "OK");
        }
        
        internal static void Log(string content)
        {
            if (_logLevel > LogLevel.Info)
            {
                return;
            }
            Debug.Log(PREFIX + content);
        }
        
        internal static void LogWarning(string content)
        {
            if (_logLevel > LogLevel.Warning)
            {
                return;
            }
            Debug.LogWarning(PREFIX + content);
        }
        
        internal static void LogError(string content)
        {
            if (_logLevel > LogLevel.Error)
            {
                return;
            }
            Debug.LogError(PREFIX + content);
        }
#else
        internal static void Log(string content)
        {
            
        }
        
        internal static void LogWarning(string content)
        {
            
        }
        
        internal static void LogError(string content)
        {
            
        }
#endif
    }
}
