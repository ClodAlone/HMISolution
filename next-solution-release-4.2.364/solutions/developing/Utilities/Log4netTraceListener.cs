using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Log4netTraceListener : System.Diagnostics.TraceListener
    {
        private readonly log4net.ILog _log;

        public Log4netTraceListener()
        {
#if !NET_STANDARD
            _log = log4net.LogManager.GetLogger("System.Diagnostics.Redirection");
#else
            _log = log4net.LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), "System.Diagnostics.Redirection");
#endif
        }

        public Log4netTraceListener(log4net.ILog log)
        {
            _log = log;
        }

        public override void Write(string message)
        {
            if (_log != null)
            {
                _log.Debug(message);
            }
        }

        public override void WriteLine(string message)
        {
            if (_log != null)
            {
                _log.Debug(message);
            }
        }

        public override void TraceEvent(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, string message)
        {
            if (_log != null)
            {
                switch (eventType)
                {
                    case System.Diagnostics.TraceEventType.Verbose:
                    {
                        _log.Debug(message);
                        break;
                    }
                    case System.Diagnostics.TraceEventType.Information:
                    {
                        _log.Info(message);
                        break;
                    }
                    case System.Diagnostics.TraceEventType.Warning:
                    {
                        _log.Warn(message);
                        break;
                    }
                    case System.Diagnostics.TraceEventType.Error:
                    {
                        _log.Error(message);
                        break;
                    }
                    case System.Diagnostics.TraceEventType.Critical:
                    {
                        _log.Fatal(message);
                        break;
                    }
                    default:
                        throw new ArgumentException(string.Format("LogLevel does not support value {0}.", eventType), "logLevel");
                }
            }
        }

        public override void TraceEvent(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, string format, params object[] args)
        {
            if (_log != null)
            {
                switch (eventType)
                {
                    case System.Diagnostics.TraceEventType.Verbose:
                        {
                            _log.DebugFormat(format, args);
                            break;
                        }
                    case System.Diagnostics.TraceEventType.Information:
                        {
                            _log.InfoFormat(format, args);
                            break;
                        }
                    case System.Diagnostics.TraceEventType.Warning:
                        {
                            _log.WarnFormat(format, args);
                            break;
                        }
                    case System.Diagnostics.TraceEventType.Error:
                        {
                            _log.ErrorFormat(format, args);
                            break;
                        }
                    case System.Diagnostics.TraceEventType.Critical:
                        {
                            _log.FatalFormat(format, args);
                            break;
                        }
                    default:
                        throw new ArgumentException(string.Format("LogLevel does not support value {0}.", eventType), "logLevel");
                }
            }
        }
    }
}
