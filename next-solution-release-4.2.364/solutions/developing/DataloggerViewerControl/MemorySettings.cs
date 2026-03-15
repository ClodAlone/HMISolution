using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using System.Collections.Specialized;

namespace DataloggerViewerControl
{
    public class Setting : INotifyPropertyChanged
    {
        #region Private Members;
        private bool readOnly;
        private String name;
        private String gridLayout = string.Empty;
        private String dataLoggerName;
        #endregion
        #region Public Properties
        public String Name
        {
            get { return name; }
            set
            {
                if (name == value)
                    return;
                name = value;
                OnPropertyChanged("Name");
            }
        }
        [SvgValueConverter(false)]
        public String GridLayout
        {
            get { return gridLayout; }
            set
            {
                if (gridLayout == value)
                    return;
                gridLayout = value;
                OnPropertyChanged("PenList");
            }
        }
        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                if (readOnly == value)
                    return;
                readOnly = value;
                OnPropertyChanged("ReadOnly");
            }
        }
        public String DataLoggerName
        {
            get { return dataLoggerName; }
            set
            {
                if (dataLoggerName == value)
                    return;
                dataLoggerName = value;
                OnPropertyChanged("DataLoggerName");
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
    public class MemorySettings : ObservableCollection<Setting>
    {
        #region Constructors
        public MemorySettings() 
        { 
        }

        public MemorySettings(ObservableCollection<Setting> collection)
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
