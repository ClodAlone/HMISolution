using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Gauges
{
    public class ClockTimeZone : INotifyPropertyChanged
    {

        #region Private Members;
        private String timeZoneXml;
        #endregion
        #region Public Properties
        public String TimeZoneXml
        {
            get { 
                if(string.IsNullOrEmpty(timeZoneXml))
                    timeZoneXml = TimeZoneInfo.Local.ToSerializedString();
                return timeZoneXml; }
            set
            {
                if (timeZoneXml == value)
                    return;
                timeZoneXml = value;
                OnPropertyChanged("TimeZoneXml");
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeZoneInfo TimeZone
        {
            get
            {
                try
                {
                    if (!String.IsNullOrEmpty(timeZoneXml))
                        return TimeZoneInfo.FromSerializedString(timeZoneXml);
                }
                catch
                { }

                return null;
            }
            set
            {
                if (value != null)
                {
                    TimeZoneXml = value.ToSerializedString();
                    OnPropertyChanged("TimeZone");
                }
                else
                {
                    TimeZoneXml = string.Empty;
                    OnPropertyChanged("TimeZone");
                }
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
