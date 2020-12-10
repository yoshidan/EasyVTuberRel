using System;
using System.IO;
using UnityEngine;

namespace App.Main.Scripts.Utils
{
     public class LogOutput
    {
        private const string LogTextName = "log.txt";
        private static LogOutput _instance;
        public static LogOutput Instance
            => _instance ?? (_instance = new LogOutput());
        
        private LogOutput()
        {
            _logFilePath = GetLogFilePath();
            if (File.Exists(_logFilePath))
            {
           //     File.Delete(_logFilePath);
            }
        }

        private readonly object _writeLock = new object();
        private readonly string _logFileDir;
        private readonly string _logFilePath;

        public void Write(string text)
        
        {
            if (!File.Exists(_logFilePath)) { return; }
            
            lock (_writeLock)
            {
                try
                {
                    
                    using (var sw = new StreamWriter(_logFilePath))
                    {
                        sw.WriteLine(text);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    //諦める
                }
            }
        }

        public void Write(Exception ex)
        {
            if (ex == null) { return; }

            Write(
                DateTime.Now.ToString("yyyyMMdd_HHmmss") + "\n" +
                ex.GetType().Name + "\n" +
                ex.Message + "\n" +
                ex.StackTrace
                );
        }

        private string GetLogFilePath() 
            => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/" + LogTextName;
    }
}