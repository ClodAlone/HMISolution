using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using UFInterfaces.Constants;
using WPFUtilities;
using WPFPenHelpers;
using Utilities;
using System.Collections.Specialized;

namespace DataAnalisysControl
{
    public class Setting : INotifyPropertyChanged
    {
        #region Private Members;
        private String name;
        private SerieDataList penList;
        private bool readOnly;
        private DateSpan filterType;
        private bool useAbsoluteRanges;
        private String gridLayout = string.Empty;
        private String dockLayout = string.Empty;
        private string listViewLayout = string.Empty;
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
        public SerieDataList PenList
        {
            get { return penList; }
            set
            {
                if (penList == value)
                    return;
                penList = value;
                OnPropertyChanged("PenList");
            }
        }
        public String DockLayout
        {
            get { return dockLayout; }
            set
            {
                if (dockLayout == value)
                    return;
                dockLayout = value;
                OnPropertyChanged("DockLayout");
            }
        }

        public DateSpan FilterType
        {
            get { return filterType; }
            set
            {
                if (filterType == value)
                    return;
                filterType = value;
                OnPropertyChanged("FilterType");
            }
        }

        public bool UseAbsoluteRanges
        {
            get { return useAbsoluteRanges; }
            set
            {
                if (useAbsoluteRanges == value)
                    return;
                useAbsoluteRanges = value;
                OnPropertyChanged("UseAbsoluteRanges");
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

        public String ListViewLayout
        {
            get { return listViewLayout; }
            set
            {
                if (listViewLayout == value)
                    return;
                listViewLayout = value;
                OnPropertyChanged("ListViewLayout");
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
