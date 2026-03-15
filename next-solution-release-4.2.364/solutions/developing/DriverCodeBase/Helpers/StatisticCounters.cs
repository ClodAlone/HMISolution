using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Opc.Ua;

namespace DriverCodeBase.Helpers
{
    /// <summary>
    /// helper class for statistic counters management
    /// </summary>
    public sealed class StatisticCounters : IDisposable
    {
        #region Constructors

        private readonly object lockObject;
        private readonly Dictionary<string, Variant> Statistics;
        private readonly Stopwatch Watcher;
        
        public StatisticCounters()
        {
            lockObject = new object();
            Statistics = new Dictionary<string, Variant>();
            Watcher = new Stopwatch();
        }
        
        #endregion

        #region Events

        public event EventHandler<ChangedCounterEventArgs> ChangedCounter;
        private void OnChangedCounter(string key, Variant newvalue)
        {
            var temp = ChangedCounter;
            if (temp != null)
            {
                ChangedCounterEventArgs e = new ChangedCounterEventArgs()
                {
                    Key = key,
                    newValue = newvalue,
                };
                temp(this, e);
            }
        }

        #endregion

        #region Public Methods

        public void StartWatch()
        {
            if (!Watcher.IsRunning)
                Watcher.Start();
        }

        public void StopWatch()
        {
            if (Watcher.IsRunning)
                Watcher.Stop();
        }

        public void Update(string key, long newValue)
        {
            bool updateChangedCounter = false;
            lock (lockObject)
            {
                updateChangedCounter = commonUpdate(key, newValue);
            }
            if(updateChangedCounter)
                OnChangedCounter(key, new Variant(newValue));

        }

        public void Update(string key, bool newValue)
        {
            bool updateChangedCounter = false;
            lock (lockObject)
            {
                if (Watcher.IsRunning)
                {
                    if (Statistics.ContainsKey(key))
                    {
                        if (Statistics[key].TypeInfo != TypeInfo.Scalars.Boolean)
                            throw new InvalidOperationException("invalid key type");
                        bool oldValue = (bool)Statistics[key].Value;
                        Statistics[key] = new Variant(newValue);
                        if (oldValue != newValue)
                            updateChangedCounter = true;
                    }
                    else
                    {
                        Statistics[key] = new Variant(newValue);
                        updateChangedCounter = true;
                    }
                }
            }
            if (updateChangedCounter)
                OnChangedCounter(key, new Variant(newValue));
        }


        public void Update(string key, string newValue)
        {
            bool updateChangedCounter = false;
            lock (lockObject)
            {
                if (Watcher.IsRunning)
                {
                    if (Statistics.ContainsKey(key))
                    {
                        if (Statistics[key].TypeInfo != TypeInfo.Scalars.String)
                            throw new InvalidOperationException("invalid key type");
                        string oldValue = (string)Statistics[key].Value;
                        Statistics[key] = new Variant(newValue);
                        if (oldValue != newValue)
                            updateChangedCounter = true;
                    }
                    else
                    {
                        Statistics[key] = new Variant(newValue);
                        updateChangedCounter = true;
                    }
                }
            }
            if (updateChangedCounter)
                OnChangedCounter(key, new Variant(newValue));
        }


        private bool commonUpdate(string key, long newValue)
        {
            bool updateChangedCounter = false;
            if (Watcher.IsRunning) 
            {
                if (Statistics.ContainsKey(key))
                {
                    if (Statistics[key].TypeInfo != TypeInfo.Scalars.Int64)
                        throw new InvalidOperationException("invalid key type");
                    long oldValue = (long)Statistics[key].Value;
                    Statistics[key] = new Variant(newValue);
                    if (oldValue != newValue)
                        updateChangedCounter = true;
                }
                else
                {
                    Statistics[key] = new Variant(newValue);
                    updateChangedCounter = true;
                }
            }

            return updateChangedCounter;
        }

        public void IncreaseCounter(string key, long value = 1)
        {
            bool updateChangedCounter = false;
            long newValue;
            lock (lockObject)
            {
                newValue = value;
                if (Statistics.ContainsKey(key))
                {
                    if (Statistics[key].TypeInfo != TypeInfo.Scalars.Int64)
                        throw new InvalidOperationException("invalid key type");
                    newValue += (long)Statistics[key].Value;
                }
                updateChangedCounter = commonUpdate(key, newValue);
            }
            if (updateChangedCounter)
                OnChangedCounter(key, new Variant(newValue));
        }

        public void DecreaseCounter(string key, long value)
        {
            bool updateChangedCounter = false;
            long newValue;
            lock (lockObject)
            {
                newValue = -value;
                if (Statistics.ContainsKey(key))
                {
                    if (Statistics[key].TypeInfo != TypeInfo.Scalars.Int64)
                        throw new InvalidOperationException("invalid key type");
                    newValue += (long)Statistics[key].Value;
                }
                updateChangedCounter = commonUpdate(key, newValue);
            }
            if (updateChangedCounter)
                OnChangedCounter(key, new Variant(newValue));
        }

        public bool RemoveCounter(string key)
        {
            lock (lockObject)
                return Statistics.Remove(key);
        }

        public void ResetStatistcs()
        {
            lock (lockObject)
            {
                bool watcherRunning = Watcher.IsRunning;
                Statistics.ToList().ForEach(r=>
                {
                    if (Statistics[r.Key].TypeInfo == TypeInfo.Scalars.Int64)
                        Update(r.Key, 0);
                });
                Watcher.Reset();
                if(watcherRunning)
                    Watcher.Start();
            }
        }

        public IDictionary RetrieveStatistcs(out TimeSpan totaltime)
        {
            totaltime = Watcher.Elapsed;
            Dictionary<string, Variant> counters = new Dictionary<string, Variant>();
            //Parallel.ForEach(Statistics.Keys, key =>
            //{
            //    lock (lockObject)
            //        counters[key] = Statistics[key];
            //});
            lock (lockObject)
            {
                Statistics.ToList().ForEach(r =>
                {
                    counters[r.Key] = Statistics[r.Key];
                });
            }

            return counters;
        }

        public int GetTotalCounters()
        {
            lock (lockObject)
                return Statistics.Keys.Count;
        }

        public IList GetListOfCountersName()
        {
            List<String> keyslist = new List<String>();
            lock (lockObject)
                keyslist.AddRange(Statistics.Keys);

            return keyslist;
        }

        public long GetCounterValue(String key)
        {
            lock (lockObject)
            {
                if (Statistics.ContainsKey(key))
                {
                    if (Statistics[key].TypeInfo != TypeInfo.Scalars.Int64)
                        throw new InvalidOperationException("invalid key type");
                    return (long)Statistics[key].Value;
                }
            }
            return 0;
        }

        public String GetStringValue(String key)
        {
            lock (lockObject)
            {
                if (Statistics.ContainsKey(key))
                {
                    if (Statistics[key].TypeInfo != TypeInfo.Scalars.String)
                        throw new InvalidOperationException("invalid key type");
                    return (String)Statistics[key].Value;
                }
            }
            return "";
        }

        public TimeSpan GetTotalTimeOn()
        {
            return Watcher.Elapsed;
        }

        #endregion

        #region Disposable Interface

        public void Dispose()
        {
            Watcher.Reset();
            Statistics.Clear();
        }
        
        #endregion

    }
}
