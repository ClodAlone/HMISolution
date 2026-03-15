using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using log4net;

namespace Utilities
{
    public class StopWatcher : IDisposable
    {
        readonly Stopwatch watcher;
        readonly String Exit;

        static readonly ILog logPerformances = LogManager.GetLogger(Properties.Resources.PerformanceManager);

        public StopWatcher(String exit)
        {
            watcher = Stopwatch.StartNew();
            Exit = exit;
        }

        public void Dispose()
        {
            watcher.Stop();

            var text = String.Format(Exit, watcher.Elapsed);
            logPerformances.Info(text);
            Debug.WriteLine(text);
        }
    }
}
