using System;
using System.Diagnostics;
using System.Reflection;
using Utilities;

namespace Utilities.Logger
{
    public class StopWatcherLogger : IDisposable
    {
        #region Declarations
        Stopwatch watcher;
        String source;
        LoggerDestination logger;
        readonly String enter;
        readonly String exit;
        #endregion

        #region Constructors
        public StopWatcherLogger(String source, LoggerDestination logger, String enter, String exit, params object[] args)
        {
            this.source = source;
            this.logger = logger;
            this.enter = String.Format(enter, args);
            this.exit = String.Format(exit, args);

            CommonConstructor();
        }

        public StopWatcherLogger(LoggerDestination logger, String enter, String exit, params object[] args)
        {
            this.logger = logger;
            this.enter = String.Format(enter, args);
            this.exit = String.Format(exit, args);

            CommonConstructor();
        }

        public StopWatcherLogger(String enter, String exit, params object[] args) 
        {
            this.enter = String.Format(enter, args);
            this.exit = String.Format(exit, args);

            CommonConstructor();
        }

        public StopWatcherLogger(String source, LoggerDestination logger, String enter, String exit) 
        {
            this.source = source;
            this.logger = logger;
            this.enter = enter;
            this.exit = exit;

            CommonConstructor();
        }

        public StopWatcherLogger(LoggerDestination logger, String enter, String exit) :
            this(enter, exit)
        {
            watcher = Stopwatch.StartNew();
            this.logger = logger;
            this.enter = enter;
            this.exit = exit;

            CommonConstructor();
        }

        public StopWatcherLogger(String enter, String exit)
        {
            this.enter = enter;
            this.exit = exit;

            CommonConstructor();
        }

        void CommonConstructor()
        {
            watcher = Stopwatch.StartNew();

            EnsureMissingValues();
            WriteEnterMessage();
        }
        #endregion

        #region Methods
        void EnsureMissingValues()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
                source = assembly.GetName().Name;
            else
                source = Properties.Resources.LoggerSource;

            if (logger == null)
                logger = LoggerDestination.Application;
        }

        void WriteEnterMessage()
        {
            Logger.WriteToEventLog(source, enter, System.Diagnostics.EventLogEntryType.Information, logger);
        }

        void WriteExitMessage()
        {
            Logger.WriteToEventLog(source, String.Format(exit, watcher.Elapsed), System.Diagnostics.EventLogEntryType.Information, logger);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            watcher.Stop();

            WriteExitMessage();
        }
        #endregion
    }
}
