using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;

namespace DriverCodeBase.Helpers
{
    /// <summary>
    /// helper class for statistic holder of static counters
    /// </summary>
    public sealed class StatisticsHolder
    {
        #region Constructors

        private readonly Dictionary<Object, StatisticCounters> StatisticsData;
        readonly object lockStatisticsData;
        public StatisticsHolder()
        {
            StatisticsData = new Dictionary<object, StatisticCounters>();
            lockStatisticsData = new object();
        }        
        #endregion

        #region Public Methods

        public void RemoveAllStatistics()
        {
            if (StatisticsData != null)
            {
                Parallel.ForEach(StatisticsData.Keys, key =>
                {
                    RemoveStatisticObject(key);
                });
            }
        }

        public void ResumeAllStatistics()
        {
            if (StatisticsData != null)
            {
                Parallel.ForEach(StatisticsData.Keys, key =>
                {
                    ResumeStatisticObject(key);
                });
            }
        }

        public void SuspendAllStatistics()
        {
            if (StatisticsData != null)
            {
                Parallel.ForEach(StatisticsData.Keys, key =>
                {
                    SuspendStatisticObject(key);
                });
            }
        }

        public void ResetAllStatistics()
        {
            if (StatisticsData != null)
            {
                Parallel.ForEach(StatisticsData.Keys, key =>
                {
                    ResetStatisticCounters(key);
                });
            }
        }

        public void AddStatisticObject(object refobject, bool bStart = true)
        {
            lock (lockStatisticsData)
            {
                if (!StatisticsData.ContainsKey(refobject))
                {
                    StatisticCounters counters = new StatisticCounters();
                    StatisticsData[refobject] = counters;
                    if (bStart)
                        StatisticsData[refobject].StartWatch();
                }
            }

        }

        public void RemoveStatisticObject(object refobject)
        {
            lock (lockStatisticsData)
            {
                if (StatisticsData.ContainsKey(refobject))
                {
                    StatisticsData[refobject].Dispose();
                    StatisticsData.Remove(refobject);
                }
            }
        }

        public void ResumeStatisticObject(object refobject)
        {
            lock (lockStatisticsData)
            {
                if (StatisticsData.ContainsKey(refobject))
                    StatisticsData[refobject].StartWatch();
            }
        }

        public void SuspendStatisticObject(object refobject)
        {
            lock (lockStatisticsData)
            {
                if (StatisticsData.ContainsKey(refobject))
                    StatisticsData[refobject].StopWatch();
            }
        }

        public void UpdateStatisticCounter(object refobject, string key, long value)
        {
            lock (lockStatisticsData)
            {
                if (StatisticsData.ContainsKey(refobject))
                    StatisticsData[refobject].Update(key, value);
            }
        }

        public void IncreaseStatisticCounter(object refobject, string key, long value = 1)
        {
            lock (lockStatisticsData)
            {
                if (StatisticsData.ContainsKey(refobject))
                    StatisticsData[refobject].IncreaseCounter(key, value);
            }
        }

        public void DecreaseStatisticCounter(object refobject, string key, long value = 1)
        {
            lock (lockStatisticsData)
            {
                if (StatisticsData.ContainsKey(refobject))
                    StatisticsData[refobject].DecreaseCounter(key, value);
            }
        }

        public void RemoveStatisticCounter(object refobject, string key)
        {
            lock (lockStatisticsData)
            {
                if (StatisticsData.ContainsKey(refobject))
                    StatisticsData[refobject].RemoveCounter(key);
            }
        }

        public void ResetStatisticCounters(object refobject)
        {
            lock (lockStatisticsData)
            {
                if (StatisticsData.ContainsKey(refobject))
                    StatisticsData[refobject].ResetStatistcs();
            }
        }

        public IDictionary RetrieveStatisticCounters(object refobject, out TimeSpan totaltime)
        {
            lock (lockStatisticsData)
            {
                if (StatisticsData.ContainsKey(refobject))
                    return StatisticsData[refobject].RetrieveStatistcs(out totaltime);
            }

            throw new ArgumentException("No statistic data for this object.");
        }
        #endregion
        
    }
}
