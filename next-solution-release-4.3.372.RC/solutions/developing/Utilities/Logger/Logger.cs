using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using log4net;

namespace Utilities.Logger
{
    public enum LoggerDestination
    {
        Application,
        Server,
        AlarmDispatcher,
        Scheduler,
        Historian, 
        License,
        User,
        Redundancy,
        ScriptService,
        LogicService,
        RecipeService,
        CommandManager,
        Client,
        Drivers

    }
    
    public static class Logger
    {
        #region Declarations
#if !NET_STANDARD
        static readonly ILog logApplication = LogManager.GetLogger(Properties.Resources.Application);
        static readonly ILog logServer = LogManager.GetLogger(Properties.Resources.Server);
        static readonly ILog logHistorian = LogManager.GetLogger(Properties.Resources.HistorianManager);
        static readonly ILog logAlarmDispatcher = LogManager.GetLogger(Properties.Resources.AlarmDispatcherManager);
        static readonly ILog logScheduler = LogManager.GetLogger(Properties.Resources.SchedulerManager);
        static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);
        static readonly ILog logUser = LogManager.GetLogger(Properties.Resources.UsersManager);
        static readonly ILog logRedundancy = LogManager.GetLogger(Properties.Resources.RedundancyManager);
        static readonly ILog logScriptService = LogManager.GetLogger(Properties.Resources.ScriptService);
        static readonly ILog logLogicService = LogManager.GetLogger(Properties.Resources.LogicService);
        static readonly ILog logRecipeService = LogManager.GetLogger(Properties.Resources.RecipeService);
        static readonly ILog logCommandManager = LogManager.GetLogger(Properties.Resources.CommandManager);
        static readonly ILog logClient = LogManager.GetLogger(Properties.Resources.Client);
        static readonly ILog logDrivers = LogManager.GetLogger(Properties.Resources.Drivers);

#else
        static readonly ILog logApplication = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.Application);
        static readonly ILog logServer = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.Server);
        static readonly ILog logHistorian = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.HistorianManager);
        static readonly ILog logAlarmDispatcher = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.AlarmDispatcherManager);
        static readonly ILog logScheduler = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.SchedulerManager);
        static readonly ILog logLicense = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.LicenseManager);
        static readonly ILog logUser = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.UsersManager);
        static readonly ILog logRedundancy = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.RedundancyManager);
        static readonly ILog logScriptService = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.ScriptService);
        static readonly ILog logLogicService = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.LogicService);
        static readonly ILog logRecipeService = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.RecipeService);
        static readonly ILog logCommandManager = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.CommandManager);
        static readonly ILog logClient = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.Client);
        static readonly ILog logDrivers = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.Drivers);
#endif
        #endregion

        #region Public Static Methods
        public static void WriteToEventLog(String source, String message, EventLogEntryType entryType, params object[] args)
        {
            WriteToEventLog(source, message, entryType, LoggerDestination.Application, args);
        }

        public static void WriteToEventLog(String source, String message, EventLogEntryType entryType, LoggerDestination dest, params object[] args)
        {
            WriteToEventLog(source, String.Format(message, args), entryType, dest);
        }

        public static void WriteToEventLog(String source, String message, EventLogEntryType entryType)
        {
            WriteToEventLog(source, message, entryType, LoggerDestination.Application);
        }

        public static void WriteToEventLog(String source, String message, EventLogEntryType entryType, LoggerDestination dest, bool bEnableEventLogsWindow = true)
        {
            ILog currlog = GetDestinationLog(dest);
            if (currlog != null)
            {
                switch (entryType)
                {
                    case EventLogEntryType.Error:
                        currlog.Error(message); break;
                    case EventLogEntryType.FailureAudit:
                        currlog.Warn(message); break;
                    case EventLogEntryType.Information:
                        currlog.Info(message); break;
                    case EventLogEntryType.Warning:
                        currlog.Warn(message); break;
                    case EventLogEntryType.SuccessAudit:
                        currlog.Info(message); break;
                }
            }

            if (bEnableEventLogsWindow && !Environment.UserInteractive)
            {
                try
                {
                    if (!EventLog.SourceExists(source))
                    {
                        EventLog.CreateEventSource(source, "Application");
                    }

                    EventLog.WriteEntry(source, message, entryType);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("{0} - {1}", DateTime.Now.ToString(System.Threading.Thread.CurrentThread.CurrentUICulture), message);
        }

        public static ILog GetDestinationLog(LoggerDestination dest)
        {
            switch (dest)
            {
                case LoggerDestination.Application:
                    return logApplication;
                case LoggerDestination.Server:
                    return logServer;
                case LoggerDestination.AlarmDispatcher:
                    return logAlarmDispatcher;
                case LoggerDestination.Scheduler:
                    return logScheduler;
                case LoggerDestination.Historian:
                    return logHistorian;
                case LoggerDestination.License:
                    return logLicense;
                case LoggerDestination.User:
                    return logUser;
                case LoggerDestination.Redundancy:
                    return logRedundancy;
                case LoggerDestination.ScriptService:
                    return logScriptService;
                case LoggerDestination.LogicService:
                    return logLogicService;
                case LoggerDestination.RecipeService:
                    return logRecipeService;
                case LoggerDestination.CommandManager:
                    return logCommandManager;
                case LoggerDestination.Client:
                    return logClient;
                case LoggerDestination.Drivers:
                    return logDrivers;
                default:
                    return null;
            }
        }
#endregion
    }
}
