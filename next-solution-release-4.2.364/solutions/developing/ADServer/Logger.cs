using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace ADServer
{
    static class Logger
    {
        public static void WriteToEventLog(String Source, String Message, EventLogEntryType EntryType)
        {
            if (!Environment.UserInteractive)
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

            Console.WriteLine(Message);
        }
    }
}
