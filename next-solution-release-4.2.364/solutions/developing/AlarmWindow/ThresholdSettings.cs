using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AlarmWindow
{
    public class ThresholdSettings : INotifyPropertyChanged
    {
        // Fields...
        #region Ctor
        public ThresholdSettings(Color c, Color f, Double v)
        {
            ThresholdValue = v;
            ThresholdColor = c;
            ThresholdForeColor = f;
            ThresholdAckColor = c;
            ThresholdAckForeColor = f;
            ThresholdOffColor = c;
            ThresholdOffForeColor = f;
            ThresholdOffAckColor = c;
            ThresholdOffAckForeColor = f;
        }
        public ThresholdSettings()
        {
            ThresholdColor = Colors.Red;
            ThresholdForeColor = Colors.White;
            ThresholdAckColor = Colors.Red;
            ThresholdAckForeColor = Colors.White;
            ThresholdOffColor = Colors.Red;
            ThresholdOffForeColor = Colors.White;
            ThresholdOffAckColor = Colors.Red;
            ThresholdOffAckForeColor = Colors.White;
            ThresholdValue = 0;
        }

        public ThresholdSettings(ThresholdSettings instance)
        {
            if (instance == null)
                return;

            ThresholdColor = instance.ThresholdColor;
            ThresholdForeColor = instance.ThresholdForeColor;
            ThresholdAckColor = instance.ThresholdAckColor;
            ThresholdAckForeColor = instance.ThresholdAckForeColor;
            ThresholdOffColor = instance.ThresholdOffColor;
            ThresholdOffForeColor = instance.ThresholdOffForeColor;
            ThresholdOffAckColor = instance.ThresholdOffAckColor;
            ThresholdOffAckForeColor = instance.ThresholdOffAckForeColor;
            ThresholdValue = instance.ThresholdValue;
        }
        #endregion 
        #region Methods

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("ThresholdColor", ThresholdColor);
            res.Add("ThresholdForeColor", ThresholdForeColor);
            res.Add("ThresholdAckColor", ThresholdAckColor);
            res.Add("ThresholdAckForeColor", ThresholdAckForeColor);
            res.Add("ThresholdOffColor", ThresholdOffColor);
            res.Add("ThresholdOffForeColor", ThresholdOffForeColor);
            res.Add("ThresholdOffAckColor", ThresholdOffAckColor);
            res.Add("ThresholdOffAckForeColor", ThresholdOffAckForeColor);
            res.Add("ThresholdValue", ThresholdValue);
            return res;
        }
        #endregion 

        #region Properties

        private Color _ThresholdColor;
        private Color _ThresholdForeColor;
        private Color _ThresholdAckColor;
        private Color _ThresholdAckForeColor;
        private Color _ThresholdOffColor;
        private Color _ThresholdOffForeColor;
        private Color _ThresholdOffAckColor;
        private Color _ThresholdOffAckForeColor;
        private Double _ThresholdValue;

        public Double ThresholdValue
        {
            get { return _ThresholdValue; }
            set { _ThresholdValue = value; OnPropertyChanged("ThresholdValue"); }
        }

        public Color ThresholdColor
        {
            get { return _ThresholdColor; }
            set { _ThresholdColor = value; OnPropertyChanged("ThresholdColor"); }
        }
        
        public Color ThresholdForeColor
        {
            get { return _ThresholdForeColor; }
            set { _ThresholdForeColor = value; OnPropertyChanged("ThresholdForeColor"); }
        }
        public Color ThresholdAckColor
        {
            get { return _ThresholdAckColor; }
            set { _ThresholdAckColor = value; OnPropertyChanged("ThresholdAckColor"); }
        }

        public Color ThresholdAckForeColor
        {
            get { return _ThresholdAckForeColor; }
            set { _ThresholdAckForeColor = value; OnPropertyChanged("ThresholdAckForeColor"); }
        }
        public Color ThresholdOffColor
        {
            get { return _ThresholdOffColor; }
            set { _ThresholdOffColor = value; OnPropertyChanged("ThresholdOffColor"); }
        }

        public Color ThresholdOffForeColor
        {
            get { return _ThresholdOffForeColor; }
            set { _ThresholdOffForeColor = value; OnPropertyChanged("ThresholdOffForeColor"); }
        }
        public Color ThresholdOffAckColor
        {
            get { return _ThresholdOffAckColor; }
            set { _ThresholdOffAckColor = value; OnPropertyChanged("ThresholdOffAckColor"); }
        }

        public Color ThresholdOffAckForeColor
        {
            get { return _ThresholdOffAckForeColor; }
            set { _ThresholdOffAckForeColor = value; OnPropertyChanged("ThresholdOffAckForeColor"); }
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
