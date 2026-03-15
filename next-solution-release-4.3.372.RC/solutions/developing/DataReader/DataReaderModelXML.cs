using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DevExpress.Xpo.DB.Helpers;
using System.ComponentModel;
using Utilities;

namespace DataReader
{
    public class DataReaderModelXML : INotifyPropertyChanged
    {
        #region Constructors
        public DataReaderModelXML()
        { }
        public DataReaderModelXML(DataReaderModel readermodel)
        {
            ReaderModel = readermodel;
        }
        #endregion
        #region Properties
        private String readerModelXml;
        public String ReaderModelXml
        {
            get { return readerModelXml; }
            set
            {
                if (readerModelXml == value)
                    return;
                readerModelXml = value;
                OnPropertyChanged("ReaderModelXml");
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataReaderModel ReaderModel
        {
            get
            {
                try
                {
                    if (!String.IsNullOrEmpty(readerModelXml))
                        return readerModelXml.FromXml<DataReaderModel>();
                }
                catch
                { }

                return null;
            }
            set
            {
                ReaderModelXml = value.ToXml();
                OnPropertyChanged("ReaderModel");
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
