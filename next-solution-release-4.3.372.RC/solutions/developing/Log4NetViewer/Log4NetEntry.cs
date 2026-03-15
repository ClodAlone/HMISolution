using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Interop;
using System.Windows;
using System.ComponentModel;
using System.Xml;
using Utilities;

namespace Log4NetViewer
{
    [Serializable]
    public class LogEntry : Observable
    {
        enum SchemaType
        {
            Unknow,
            log4j,
            log4net
        }

        readonly static Dictionary<String, SchemaType> SupportedSchemaTypes = new Dictionary<String, SchemaType>()
        {
            { "log4j:event", SchemaType.log4j },
            { "log4net:event", SchemaType.log4net }
        };

        static readonly DateTime UnixEpochDateTme = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        #region Static Methods
        public static LogEntry Parse(XmlTextReader xmlTextReader)
        {
            if ((xmlTextReader.NodeType != XmlNodeType.Element) || !SupportedSchemaTypes.ContainsKey(xmlTextReader.Name))
                return null;

            var schemaType = SupportedSchemaTypes[xmlTextReader.Name];
            var logentry = new LogEntry();

            if (schemaType == SchemaType.log4j)
            {
                var dSeconds = Convert.ToDouble(xmlTextReader.GetAttribute("timestamp"));
                logentry.TimeStamp = UnixEpochDateTme.AddMilliseconds(dSeconds);
                logentry.Thread = xmlTextReader.GetAttribute("thread");
                logentry.Logger = xmlTextReader.GetAttribute("logger");
                logentry.Level = xmlTextReader.GetAttribute("level");

                while (xmlTextReader.Read())
                {
                    switch (xmlTextReader.Name)
                    {
                        case "log4j:event":
                            return logentry;
                        default:
                            switch (xmlTextReader.Name)
                            {
                                case ("log4j:message"):
                                    {
                                        logentry.Message = xmlTextReader.ReadString();
                                        break;
                                    }
                                case ("log4j:data"):
                                    {
                                        switch (xmlTextReader.GetAttribute("name"))
                                        {
                                            case ("log4jmachinename"):
                                                {
                                                    logentry.MachineName = xmlTextReader.GetAttribute("value");
                                                    break;
                                                }
                                            case ("log4net:HostName"):
                                                {
                                                    logentry.HostName = xmlTextReader.GetAttribute("value");
                                                    break;
                                                }
                                            case ("log4net:UserName"):
                                                {
                                                    logentry.UserName = xmlTextReader.GetAttribute("value");
                                                    break;
                                                }
                                            case ("log4net:Identity"):
                                                {
                                                    logentry.Identity = xmlTextReader.GetAttribute("value");
                                                    break;
                                                }
                                            case ("NDC"):
                                                {
                                                    logentry.NDC = xmlTextReader.GetAttribute("value");
                                                    break;
                                                }
                                            case ("log4japp"):
                                                {
                                                    logentry.App = xmlTextReader.GetAttribute("value");
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                case ("log4j:throwable"):
                                    {
                                        logentry.Details = xmlTextReader.ReadString();
                                        break;
                                    }
                                case ("log4j:locationInfo"):
                                    {
                                        logentry.Class = xmlTextReader.GetAttribute("class");
                                        logentry.Method = xmlTextReader.GetAttribute("method");
                                        logentry.File = xmlTextReader.GetAttribute("file");
                                        logentry.Line = xmlTextReader.GetAttribute("line");
                                        break;
                                    }
                            }
                            break;
                    }
                }
            }
            else if (schemaType == SchemaType.log4net)
            {
                logentry.TimeStamp = DateTime.Parse(xmlTextReader.GetAttribute("timestamp"));
                logentry.Thread = xmlTextReader.GetAttribute("thread");
                logentry.Logger = xmlTextReader.GetAttribute("logger");
                logentry.Level = xmlTextReader.GetAttribute("level");
                logentry.App = xmlTextReader.GetAttribute("domain");
                logentry.UserName = xmlTextReader.GetAttribute("username");

                while (xmlTextReader.Read())
                {
                    switch (xmlTextReader.Name)
                    {
                        case "log4net:event":
                            return logentry;
                        default:
                            switch (xmlTextReader.Name)
                            {
                                case ("log4net:message"):
                                    {
                                        logentry.Message = xmlTextReader.ReadString();
                                        break;
                                    }
                                case ("log4net:data"):
                                    {
                                        switch (xmlTextReader.GetAttribute("name"))
                                        {
                                            case ("log4net:HostName"):
                                                {
                                                    logentry.MachineName =
                                                        logentry.HostName = xmlTextReader.GetAttribute("value");
                                                    break;
                                                }
                                            case ("log4net:UserName"):
                                                {
                                                    logentry.UserName = xmlTextReader.GetAttribute("value");
                                                    break;
                                                }
                                            case ("log4net:Identity"):
                                                {
                                                    logentry.Identity = xmlTextReader.GetAttribute("value");
                                                    break;
                                                }
                                            case ("NDC"):
                                                {
                                                    logentry.NDC = xmlTextReader.GetAttribute("value");
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                case ("log4net:exception"):
                                    {
                                        logentry.Details = xmlTextReader.ReadString();
                                        break;
                                    }
                                case ("log4net:locationInfo"):
                                    {
                                        logentry.Class = xmlTextReader.GetAttribute("class");
                                        logentry.Method = xmlTextReader.GetAttribute("method");
                                        logentry.File = xmlTextReader.GetAttribute("file");
                                        logentry.Line = xmlTextReader.GetAttribute("line");
                                        break;
                                    }
                            }
                            break;
                    }
                }
            }
            else
                return null;

            return logentry;
        }


        #endregion

        #region Public Properties
        int item;
        public int Item 
        { 
            get
            {
                return item;
            }
            set
            {
                Set(ref item, value, "Item");
            }
        }

        DateTime timeStamp;
        public DateTime TimeStamp
        {
            get 
            { 
                return timeStamp; 
            }
            set 
            {
                Set(ref timeStamp, value, "TimeStamp");
            }
        }

        public String TimeStampString
        {
            get 
            { 
                return timeStamp.ToString();
            }
        }

        string level;
        public string Level
        {
            get 
            {
                if (level == null)
                    return String.Empty;

                return level; 
            }
            set 
            {
                Set(ref level, value, "Level");
            }
        }

        string thread;
        public string Thread
        {
            get 
            {
                if (thread == null)
                    return string.Empty;
                return thread; 
            }
            set 
            {
                Set(ref thread, value, "Thread");
            }
        }

        string message;
        public string Message
        {
            get 
            {
                if (message == null)
                    return string.Empty;
                return message; 
            }
            set 
            {
                Set(ref message, value, "Message");
            }
        }
        
        string machineName;
        public string MachineName
        {
            get 
            {
                if (machineName == null)
                    return string.Empty;
                return machineName; 
            }
            set 
            {
                Set(ref machineName, value, "MachineName");
            }
        }

        string userName;
        public string UserName
        {
            get 
            { 
                return userName; 
            }
            set 
            {
                Set(ref userName, value, "UserName");
            }
        }

        string identity;
        public string Identity
        {
            get 
            {
                if (identity == null)
                    return string.Empty;

                return identity;
            }
            set
            {
                Set(ref identity, value, "Identity");
            }
        }

        string ndc;
        public string NDC
        {
            get
            {
                if (ndc == null)
                    return string.Empty;

                return ndc;
            }
            set
            {
                Set(ref ndc, value, "NDC");
            }
        }

        string hostName;
        public string HostName
        {
            get
            {
                if (hostName == null)
                    return string.Empty;

                return hostName;
            }
            set
            {
                Set(ref hostName, value, "HostName");
            }
        }

        string app;
        public string App
        {
            get
            {
                if (app == null)
                    return string.Empty;

                return app;
            }
            set
            {
                Set(ref app, value, "App");
            }
        }

        string details;
        public string Details
        {
            get
            {
                if (details == null)
                    return string.Empty;

                return details;
            }
            set
            {
                Set(ref details, value, "Details");
            }
        }

        string _class;
        public string Class
        {
            get
            {
                if (_class == null)
                    return string.Empty;

                return _class;
            }
            set
            {
                Set(ref _class, value, "Class");
            }
        }

        string method;
        public string Method
        {
            get
            {
                if (method == null)
                    return string.Empty;

                return method;
            }
            set
            {
                Set(ref method, value, "Method");
            }
        }

        string file;
        public string File
        {
            get
            {
                if (file == null)
                    return string.Empty;

                return file;
            }
            set
            {
                Set(ref file, value, "File");
            }
        }

        string line;
        public string Line
        {
            get
            {
                if (line == null)
                    return string.Empty;

                return line;
            }
            set
            {
                Set(ref line, value, "Line");
            }
        }

        string logger;
        public string Logger
        {
            get
            {
                return logger;
            }
            set
            {
                Set(ref logger, value, "Logger");
            }
        }
        #endregion
    }
}
