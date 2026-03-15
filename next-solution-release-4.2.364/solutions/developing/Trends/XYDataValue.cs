using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using Opc.Ua;

namespace Trends
{
    public class XYDataValue : INotifyPropertyChanged
    {
        [DataMember]
        private DataValue _value1;
        [DataMember]
        private DataValue _value2;

        #region Properties
        public DataValue Value1
        {
            get { return _value1; }
            set
            {
                _value1 = value;
                OnPropertyChanged("Value1");
            }
        }
        public DataValue Value2
        {
            get { return _value2; }
            set
            {
                _value2 = value;
                OnPropertyChanged("Value2");
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
    }
    public class XYPoint : INotifyPropertyChanged
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
                OnPropertyChanged("Value1");
            }
        }
        public object Value2
        {
            get { return _value2; }
            set
            {
                _value2 = value;
                OnPropertyChanged("Value2");
            }
        }
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
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
    }
}
