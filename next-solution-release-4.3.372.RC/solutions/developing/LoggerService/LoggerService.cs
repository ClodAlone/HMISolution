using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using LoggerSvc;

namespace LoggerService
{
    public partial class LoggerService : ServiceBase
    {
        public LoggerService()
        {
            InitializeComponent();
        }

        public void StartService(string[] args)
        {
        }

        public void StopService()
        {
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry(Properties.Resource.ServiceStarting,
                     System.Diagnostics.EventLogEntryType.Information);

            StartService(args);

            EventLog.WriteEntry(Properties.Resource.ServiceStarted,
                     System.Diagnostics.EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry(Properties.Resource.ServiceStopping,
                     System.Diagnostics.EventLogEntryType.Information);

            StopService();

            EventLog.WriteEntry(Properties.Resource.ServiceStopped,
                     System.Diagnostics.EventLogEntryType.Information);
        }
    }
}
