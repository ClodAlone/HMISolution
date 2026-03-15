using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utilities;

namespace ImportWizardPlugin
{
    public class ExclusiveAlarm : INotifyPropertyChanged
    {
        private XElement _LowLowLimit;
        public XElement LowLowLimit
        {
            get { return _LowLowLimit; }
            set
            {
                _LowLowLimit = value;
                OnPropertyChanged("LowLowLimit");
            }
        }
        private XElement _LowLimit;
        public XElement LowLimit
        {
            get { return _LowLimit; }
            set
            {
                _LowLimit = value;
                OnPropertyChanged("LowLimit");
            }
        }

        private XElement _HighHighLimit;
        public XElement HighHighLimit
        {
            get { return _HighHighLimit; }
            set
            {
                _HighHighLimit = value;
                OnPropertyChanged("HighHighLimit");
            }
        }

        private XElement _HighLimit;
        public XElement HighLimit
        {
            get { return _HighLimit; }
            set
            {
                _HighLimit = value;
                OnPropertyChanged("HighLimit");
            }
        }



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
