using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib.Performances
{
    public class Latency
    {
        private readonly long _serverTimestamp;
        private long _receivedTimestamp;
        private long _renderedTimestamp;

        public double ServerToClientMs
        {
            get { return GetElapsedMs(_serverTimestamp, _receivedTimestamp); }
        }

        public double UiProcessingTimeMs
        {
            get { return GetElapsedMs(_receivedTimestamp, _renderedTimestamp); }
        }

        public void DisplayedOnUi()
        {
            _renderedTimestamp = Stopwatch.GetTimestamp();
        }

        public void ReceivedInGuiProcess()
        {
            _receivedTimestamp = Stopwatch.GetTimestamp();
        }

        public double TotalLatencyMs
        {
            get { return UiProcessingTimeMs + ServerToClientMs; }
        }

        private static double GetElapsedMs(long start, long end)
        {
            return (double)(end - start) / Stopwatch.Frequency * 1000;
        }
    }
}
