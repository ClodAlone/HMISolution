using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib.Performances
{
    public class ProcessorMonitor
    {
        private readonly Process _currentProcess;
        private TimeSpan _lastProcessTime;

        public ProcessorMonitor()
        {
            _currentProcess = Process.GetCurrentProcess();
            _lastProcessTime = _currentProcess.UserProcessorTime;
        }

        public TimeSpan CalculateProcessingAndReset()
        {
            var currentProcessTime = _currentProcess.UserProcessorTime;
            var result = currentProcessTime.Subtract(_lastProcessTime);
            _lastProcessTime = currentProcessTime;

            return result;
        }

        public bool IsAvailable { get { return true; } }
    }
}
