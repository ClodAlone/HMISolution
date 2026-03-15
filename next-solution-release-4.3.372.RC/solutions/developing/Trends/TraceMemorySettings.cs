using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using UFInterfaces.Constants;
using System.Collections.Specialized;

namespace Trends
{
    public class TSetting : Setting, INotifyPropertyChanged
    {
        #region Private Members;
        private bool useStartTime;
        private DateTime startTime;
        private bool useEndTime;
        private DateTime endTime;
        private uint maxReturnValues;
        private bool useMaxReturnValues;
        #endregion
        #region Public Properties
        public bool UseStartTime
        {
            get { return useStartTime; }
            set
            {
                if (useStartTime == value)
                    return;
                useStartTime = value;
                OnPropertyChanged("UseStartTime");
            }
        }
        public bool UseEndTime
        {
            get { return useEndTime; }
            set
            {
                if (useEndTime == value)
                    return;
                useEndTime = value;
                OnPropertyChanged("UseEndTime");
            }
        }

        public bool UseMaxReturnValues
        {
            get { return useMaxReturnValues; }
            set
            {
                if (useMaxReturnValues == value)
                    return;
                useMaxReturnValues = value;
                OnPropertyChanged("UseMaxReturnValues");
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

        public uint MaxReturnValues
        {
            get { return maxReturnValues; }
            set
            {
                if (maxReturnValues == value)
                    return;
                maxReturnValues = value;
                OnPropertyChanged("MaxReturnValues");
            }
        }
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
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion
    }
    public class TraceMemorySettings : ObservableCollection<TSetting>
    {
        #region Constructors
        public TraceMemorySettings() 
        { 
        }

        public TraceMemorySettings(ObservableCollection<TSetting> collection)
            : base(collection)
        {
        }
        #endregion

        #region Overrides
        //
        // Summary:
        //     Raises the System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged
        //     event with the provided arguments.
        //
        // Parameters:
        //   e:
        //     Arguments of the event being raised.
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            names = null;
            OnPropertyChanged(new PropertyChangedEventArgs("Names"));
        }
        #endregion

        #region Properties
        List<String> names;
        public List<String> Names
        {
            get
            {
                if (names == null)
                {
                    names = new List<String>();
                    foreach (var item in Items)
                        names.Add(item.Name);
                }

                return names.OrderBy(x => x).ToList();
            }
        }
        #endregion
    }
}
