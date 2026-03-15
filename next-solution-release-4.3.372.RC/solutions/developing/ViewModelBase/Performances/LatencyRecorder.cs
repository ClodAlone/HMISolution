using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLib.Performances
{
    public class LatencyRecorder
    {
        private readonly Histogram _uiLatency;
        private readonly Histogram _serverLatency;
        private readonly Histogram _combinedLatency;
        private readonly object _histogramLock = new object();

        public LatencyRecorder()
        {
            _uiLatency = GetHistogram();
            _serverLatency = GetHistogram();
            _combinedLatency = GetHistogram();
        }

        public void OnRendered(Latency latency)
        {
            latency.DisplayedOnUi();
            _uiLatency.AddObservation((long)latency.UiProcessingTimeMs);
            _combinedLatency.AddObservation((long)latency.TotalLatencyMs);
        }

        public void OnReceived(Latency latency)
        {
            latency.ReceivedInGuiProcess();
            lock (_histogramLock)
            {
                _serverLatency.AddObservation((long)latency.ServerToClientMs);
            }
        }

        public Statistics CalculateAndReset()
        {
            var stats = new Statistics();

            lock (_histogramLock)
            {
                stats.RenderedCount = _uiLatency.Count;
                stats.ReceivedCount = _serverLatency.Count;
                stats.ServerLatencyMax = _serverLatency.Max;
                stats.UiLatencyMax = _uiLatency.Max;
                stats.TotalLatencyMax = _combinedLatency.Max;
                stats.Histogram = _combinedLatency.ToString();

                _uiLatency.Clear();
                _combinedLatency.Clear();
                _serverLatency.Clear();
            }

            return stats;
        }

        private Histogram GetHistogram()
        {
            var intervals = new long[13];
            var intervalUpperBound = 1L;
            for (var i = 0; i < intervals.Length - 1; i++)
            {
                intervalUpperBound *= 2;
                intervals[i] = intervalUpperBound;
            }

            intervals[intervals.Length - 1] = long.MaxValue;
            return new Histogram(intervals);
        }
    }
}
