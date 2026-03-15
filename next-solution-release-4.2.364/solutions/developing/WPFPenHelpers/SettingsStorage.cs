using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
#if NETFRAMEWORK
using System.Windows.Threading;
#endif
using WPFUtilities;

namespace WPFPenHelpers
{
    [DataContract(Name = "SettingsStorage")]
    public class SettingsStorage : INotifyPropertyChanged, IDisposable
    {
        #region Members persistance
        [DataMember]
        public Dictionary<String, SerieSettings> mapSeries;
        [DataMember]
        DateTime dateTimeStartCompare = DateTime.MinValue;
        [DataMember]
        DateTime dateTimeEndCompare = DateTime.MinValue;
        [DataMember]
        DateTime dateTimeStart = DateTime.MinValue;
        [DataMember]
        DateTime dateTimeEnd = DateTime.MinValue;
        [DataMember]
        bool compare = false;
        [DataMember]
        HorizontalComparisonAlignment comparingAlignment;
        [DataMember]
        DateTime startTime = DateTime.MinValue;
        [DataMember]
        DateTime endTime = DateTime.MinValue;
        [DataMember]
        public List<TimeRange> ListRanges;
        [DataMember]
        TimeRangesList<DateTime[]> oldZoomLevels;
        [DataMember]
        TimeRangesList<DateTime[]> prevTimeFrame;

        bool bDisposed;
        DateTime selectedStart = DateTime.MinValue;
        DateTime selectedEnd = DateTime.MinValue;

        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            OnPropertyChanged("DateTimeStart");
            OnPropertyChanged("DateTimeEnd");
            OnPropertyChanged("StartTime");
            OnPropertyChanged("EndTime");
        }

        #endregion

        #region Properties

        public DateTime DateTimeStart
        {
            get { return dateTimeStart; }
            set
            {
                if (dateTimeStart == value)
                    return;
                dateTimeStart = value;
                OnPropertyChanged("DateTimeStart");
            }
        }

        public DateTime DateTimeEnd
        {
            get { return dateTimeEnd; }
            set
            {
                if (dateTimeEnd == value)
                    return;
                dateTimeEnd = value;
                OnPropertyChanged("DateTimeEnd");
            }
        }

        public bool Compare
        {
            get { return compare; }
            set
            {
                if (compare == value)
                    return;
                compare = value;
                OnPropertyChanged("Compare");
            }
        }

        public HorizontalComparisonAlignment ComparingAlignment
        {
            get { return comparingAlignment; }
            set
            {
                if (comparingAlignment == value)
                    return;
                comparingAlignment = value;
                OnPropertyChanged("ComparingAlignment");
            }
        }

        public DateTime DateTimeStartCompare
        {
            get { return dateTimeStartCompare; }
            set
            {
                if (dateTimeStartCompare == value)
                    return;
                dateTimeStartCompare = value;
                OnPropertyChanged("DateTimeStartCompare");
            }
        }

        public DateTime DateTimeEndCompare
        {
            get { return dateTimeEndCompare; }
            set
            {
                if (dateTimeEndCompare == value)
                    return;
                dateTimeEndCompare = value;
                OnPropertyChanged("DateTimeEndCompare");
            }
        }

        public class TimeRangesList<T> : List<DateTime[]>
        {
            SettingsStorage settings;
            int maxStoredItems;
            public TimeRangesList(SettingsStorage s, int maxItems = 20)
            {
                maxStoredItems = maxItems;
                settings = s;
            }
            public int Store()
            {
                if (Count >= maxStoredItems)
                    RemoveAt(0);
                Add(new DateTime[] { settings.DateTimeStart, settings.DateTimeEnd });
                return Count;
            }
            public int Restore(bool keepRestoredItems = false)
            {
                if (Count == 0) return -1;
                settings.DateTimeStart = this[Count - 1][0];
                settings.DateTimeEnd = this[Count - 1][1];
                if (!keepRestoredItems)
                    RemoveAt(Count - 1);
                return Count;
            }
        }

        public TimeRangesList<DateTime[]> OldZoomLevels
        {
            get
            {
                if (oldZoomLevels == null)
                    oldZoomLevels = new TimeRangesList<DateTime[]>(this);
                return oldZoomLevels;
            }
        }

        public TimeRangesList<DateTime[]> PrevTimeFrame
        {
            get
            {
                if (prevTimeFrame == null)
                    prevTimeFrame = new TimeRangesList<DateTime[]>(this, 1);
                return prevTimeFrame;
            }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                if (startTime == value)
                    return;
                startTime = value;
                OnPropertyChanged("StartTime");
            }
        }

        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                if (endTime == value)
                    return;
                endTime = value;
                OnPropertyChanged("EndTime");
            }
        }
        
        //public List<TimeRange> ListRanges
        //{
        //    get
        //    {
        //        if (listRanges == null)
        //            listRanges = new List<TimeRange>();
        //        return listRanges;
        //    }
        //}

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            // VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
#if NETFRAMEWORK
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
#endif
                    handler(this, e);
            }
        }

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            OldZoomLevels.Clear();
            PrevTimeFrame.Clear();
        }

        #endregion
    }
}
