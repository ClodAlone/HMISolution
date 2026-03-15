using System;
using System.Runtime.Serialization;
using System.ComponentModel;
#if NETFRAMEWORK
using System.Windows.Threading;
#endif

namespace HistoricalHelpers
{
    public class SettingsStorageBase : INotifyPropertyChanged
    {
        #region Members persistance
        [DataMember]
        DateTime dateTimeStart = DateTime.MinValue;
        [DataMember]
        DateTime dateTimeEnd = DateTime.MinValue;
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

        #endregion
    }
}
