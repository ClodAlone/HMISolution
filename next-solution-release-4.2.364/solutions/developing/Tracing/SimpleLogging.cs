using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace Tracing
{
    public class SimpleLogging
    {
        static readonly String defExt = ".log";
        static Object lockObject = new Object();
        static Dictionary<String, String> mapLogs = new Dictionary<String, String>();
        static Dictionary<String, String> mapPaths = new Dictionary<String, String>();
        static String defaultLogFileName = String.Format("{0}{1}_{2}{3}", Path.GetTempPath(), GetCodeBaseName(), GetDefaultDateFormat(), defExt);

        private static void VerifyAndCreateDir(String path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
                di.Create();
        }

        private static String GetCodeBaseName()
        {
            var name = new FileInfo(Assembly.GetEntryAssembly().Location).Name;
            return Path.GetFileNameWithoutExtension(name);
        }

        private static String GetDefaultDateFormat()
        {
            return String.Format("{0}_{1}_{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        public static String GenerateDefaultLogFileName(String BaseFileName, Environment.SpecialFolder folderType)
        {
            String path = Environment.GetFolderPath(folderType) + GetCodeBaseName();
            VerifyAndCreateDir(path);

            String ret = String.Format("{0}\\{1}_{2}{3}", path, BaseFileName, GetDefaultDateFormat(), defExt);
            lock (lockObject)
            {
                mapLogs.Add(BaseFileName, ret);
                mapPaths.Add(BaseFileName, path);
            }
            return ret;
        }

        public static String GenerateDefaultLogFileName(String BaseFileName, String RelativePath)
        {
            String path = RelativePath;
            VerifyAndCreateDir(path);

            var name = BaseFileName.Replace('.', '_');
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            String ret = String.Format("{0}\\{1}_{2}{3}", path, name, GetDefaultDateFormat(), defExt);
            lock (lockObject)
            {
                mapLogs.Add(BaseFileName, ret);
                mapPaths.Add(BaseFileName, path);
            }
            return ret;
        }

        public static String GenerateDefaultLogFileName(String BaseFileName)
        {
            String path = Path.GetTempPath() + GetCodeBaseName();
            VerifyAndCreateDir(path);

            var name = BaseFileName.Replace('.', '_');
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            String ret = String.Format("{0}\\{1}_{2}{3}", path, name, GetDefaultDateFormat(), defExt);
            lock (lockObject)
            {
                mapLogs.Add(BaseFileName, ret);
                mapPaths.Add(BaseFileName, path);
            }
            return ret;
        }

        public static String[] GetLogFileNameList(String BaseFileName)
        {
            String path;
            lock (lockObject)
            {
                if (!mapPaths.TryGetValue(BaseFileName, out path))
                    path = Path.GetTempPath();
            }

            try
            {
                var name = BaseFileName.Replace('.', '_');
                foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                    name = name.Replace(c, '_');

                var files = Directory.GetFiles(path, String.Format("*{0}", defExt));
                if (files != null && files.Length > 0)
                    return (from f in files where f.ToLower().Contains(name.ToLower()) select f).ToArray();
            }
            catch { }

            return null;
        }

        public static void WriteToLog(String BaseFileName, String Message)
        {
            WriteToLog(BaseFileName, Message, DateTime.Now);
        }

        public static void WriteToLog(String BaseFileName, String Message, DateTime timeStamp)
        {
            try
            {
                String LogPath;
                lock (lockObject)
                {
                    if (!mapLogs.TryGetValue(BaseFileName, out LogPath))
                    {
                        LogPath = !String.IsNullOrEmpty(BaseFileName) ? GenerateDefaultLogFileName(BaseFileName) : defaultLogFileName;
                    }
                }

                using (StreamWriter s = File.AppendText(LogPath))
                {
                    s.WriteLine(timeStamp + "\t" + Message);
                    s.WriteLine();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                WriteToEventLog(BaseFileName, Message, EventLogEntryType.FailureAudit);
            }
        }

        public static void LogException(String BaseFileName, MethodBase method, Exception e)
        {
            if (String.IsNullOrEmpty(BaseFileName))
            {
                BaseFileName = method.Name;
            }

            String Message = e.ToString();
            WriteToLog(BaseFileName, Message);

            String ret = String.Format("{0}\\{1}_{2}.html", Path.GetTempPath(), Message, GetDefaultDateFormat());
            Message = GetHtmlException(e);

            try
            {
                using (StreamWriter s = File.CreateText(ret))
                {
                    s.Write(Message);
                }
            }
            catch (Exception ex)
            {
                WriteToEventLog(BaseFileName, Message, EventLogEntryType.FailureAudit);
            }
        }

        private static string ReplaceSpecialCharacters(string message)
        {
            message = message.Replace("&", "&#38;");
            message = message.Replace("<", "&lt;");
            message = message.Replace(">", "&gt;");
            message = message.Replace("\"", "&#34;");
            message = message.Replace("'", "&#39;");
            message = message.Replace(Environment.NewLine, "<br/>");

            return message;
        }

        public static String GetHtmlException(Exception e)
        {
            StringBuilder buffer = new StringBuilder();

            buffer.Append("<html><body style='margin:0'>");

            while (e != null)
            {
                String message = e.Message;

                message = ReplaceSpecialCharacters(message);

                buffer.Append("<font style='font:9pt/12pt verdana;color:red'><b>");
                buffer.Append(message);
                buffer.Append("</b></font><br>");

                message = e.StackTrace;

                if (!String.IsNullOrEmpty(message))
                {
                    message = ReplaceSpecialCharacters(message);

                    buffer.Append("<p>");
                    buffer.Append("<font style='font:9pt/12pt verdana;color:black'>");
                    buffer.Append(message);
                    buffer.Append("</font>");
                    buffer.Append("</p>");
                }

                e = e.InnerException;
            }

            buffer.Append("</body></html>");

            return buffer.ToString();
        }

        public static void WriteToEventLog(String Source, String Message, EventLogEntryType EntryType)
        {
            try
            {
                if (!EventLog.SourceExists(Source))
                {
                    EventLog.CreateEventSource(Source, "Application");
                }

                EventLog.WriteEntry(Source, Message, EntryType);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
