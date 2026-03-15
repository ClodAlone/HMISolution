using System;
using System.ComponentModel;
using System.Runtime.Serialization;
#if !NET_STANDARD
    using System.Windows.Threading;
#endif
using Opc.Ua;

namespace WPFUtilities.HistoricalHelpers
{
    public class XYDataValue
#if !NET_STANDARD
        : INotifyPropertyChanged
#endif
    {
        [DataMember]
        private MyDataValue _value1;
        [DataMember]
        private MyDataValue _value2;

        #region Properties
        public MyDataValue Value1
        {
            get { return _value1; }
            set
            {
                _value1 = value;
#if !NET_STANDARD
                OnPropertyChanged("Value1");
#endif
            }
        }
        public MyDataValue Value2
        {
            get { return _value2; }
            set
            {
                _value2 = value;
#if !NET_STANDARD
                OnPropertyChanged("Value2");
#endif
            }
        }
        #endregion

#if !NET_STANDARD
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
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }

        #endregion
#endif
    }
    public class XYPoint
#if !NET_STANDARD
        : INotifyPropertyChanged
#endif
    {
        [DataMember]
        private object _value1;
        [DataMember]
        private object _value2;
        [DataMember]
        private DateTime _date;

        #region ctor
        public XYPoint(object value1, object value2, DateTime date)
        {
            _value1 = value1;
            _value2 = value2;
            _date = date;
        }
        #endregion

        #region Properties
        public object Value1
        {
            get { return _value1; }
            set
            {
                _value1 = value;
#if !NET_STANDARD
                OnPropertyChanged("Value1");
#endif
            }
        }
        public object Value2
        {
            get { return _value2; }
            set
            {
                _value2 = value;
#if !NET_STANDARD
                OnPropertyChanged("Value2");
#endif
            }
        }
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
#if !NET_STANDARD
                OnPropertyChanged("Date");
#endif
            }
        }
        #endregion

#if !NET_STANDARD
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
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }

        #endregion
#endif
    }
}
