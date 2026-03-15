using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Utilities;
using DataReader;

namespace DBControls
{
    public class ControlDataSourceItem : INotifyPropertyChanged
    {
        #region Constructors
        public ControlDataSourceItem()
        { }

        public ControlDataSourceItem(ControlDataSourceItem instance)
        {
            if (instance == null)
                return;

            ControlDataSourceXml = instance.ControlDataSourceXml;
            ColumnListSettingsXml = instance.ColumnListSettingsXml;
            ControlDataSource = new DataReaderModel(instance.ControlDataSource);
            TableName = instance.TableName;
        }
        #endregion

        #region Members;
        private String controlDataSourceXml;
        public String ControlDataSourceXml
        {
            get { return controlDataSourceXml; }
            set
            {
                if (controlDataSourceXml == value)
                    return;
                controlDataSourceXml = value;
                OnPropertyChanged("ControlDataSourceXml");
            }
        }

        private String columnListSettingsXml;
        public String ColumnListSettingsXml
        {
            get { return columnListSettingsXml; }
            set
            {
                if (columnListSettingsXml == value)
                    return;
                columnListSettingsXml = value;
                OnPropertyChanged("ColumnListSettingsXml");
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataReaderModel ControlDataSource
        {
            get
            {
                try
                {
                    if (!String.IsNullOrEmpty(controlDataSourceXml))
                        return controlDataSourceXml.FromXml<DataReaderModel>();
                }
                catch
                { }

                return null;
            }
            set
            {
                ControlDataSourceXml = value.ToXml();
                OnPropertyChanged("ControlDataSource");
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColumnItemList ColumnListSettings
        {
            get 
            {
                try
                {
                    if (!String.IsNullOrEmpty(columnListSettingsXml))
                        return columnListSettingsXml.FromXml<ColumnItemList>(); 
                }
                catch
                { }

                return null;
            }
            set
            {
                ColumnListSettingsXml = value.ToXml();
                OnPropertyChanged("ColumnListSettings");
            }
        }

        private String tableName = string.Empty;
        public String TableName
        {
            get { return tableName; }
            set
            {
                if (tableName == value)
                    return;
                tableName = value;
                OnPropertyChanged("TableName");
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
}
