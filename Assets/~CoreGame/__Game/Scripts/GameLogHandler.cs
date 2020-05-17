using System;
using System.IO;
using UnityEngine;

namespace CoreGame
{
    public class GameLogHandler : ILogHandler
    {
        public static FileStream m_FileStream;
        
        private StreamWriter m_StreamWriter;
        private ILogHandler m_DefaultLogHandler = Debug.unityLogger.logHandler;

        public GameLogHandler()
        {
            if(GameLogHandler.m_FileStream == null)
            {
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);

                string filePath = Application.persistentDataPath + "/GameLogs.txt";

                Debug.Log(filePath);

                GameLogHandler.m_FileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                m_StreamWriter = new StreamWriter(m_FileStream);

                // Replace the default debug log handler
                Debug.unityLogger.logHandler = this;
            }
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {

#if !UNITY_EDITOR
            if (logType == LogType.Log || logType == LogType.Warning)
            {
                return;
            }
#endif

            m_StreamWriter.WriteLine(String.Format(format, args));
            m_StreamWriter.Flush();
            m_DefaultLogHandler.LogFormat(logType, context, format, args);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            m_DefaultLogHandler.LogException(exception, context);
        }
    }

}
