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
using OPCUAViewModel;
using Utilities;

namespace Trends
{
    public class PointSetting : INotifyPropertyChanged
    {
        #region Constructors
        public PointSetting()
        { }

        public PointSetting(PointSetting instance)
        {
            if (instance == null)
                return;

            PColor = instance.PColor;
            PLabel = instance.PLabel;
        }
        #endregion

        #region Private Members
        private Color pcolor;
        private String plabel;
        #endregion

        #region Public Properties
        public String PLabel
        {
            get { return plabel; }
            set
            {
                if (plabel == value)
                    return;
                plabel = value;
                OnPropertyChanged("PLabel");
            }
        }
        public Color PColor
        {
            get { return pcolor; }
            set
            {
                if (pcolor == value)
                    return;
                pcolor = value;
                OnPropertyChanged("PColor");
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
