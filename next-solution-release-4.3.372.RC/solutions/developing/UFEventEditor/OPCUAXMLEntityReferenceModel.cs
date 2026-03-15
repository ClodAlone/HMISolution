using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS_UWP
using Windows.UI.Core;
using Windows.UI.Xaml;
#else
using Utilities;
using System.Windows.Threading;
using System.Windows.Controls;
#endif
using UFInterfaces.Constants;

namespace UFEventEditor
{
    public class OPCUAXMLEntityReferenceModel : Object, INotifyPropertyChanged
    {
        string _Value;
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                OnPropertyChanged("Value");
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
        protected
#if WINDOWS_UWP
            async
#endif
            void OnPropertyChanged(string propertyName)
        {
            // VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
#if !WINDOWS_UWP
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
#else
                var dispobj = handler.Target as DependencyObject;
                if (dispobj != null)
                {
                    Utilities.RunOnUIThread.RunIfRequired(() => OnPropertyChanged(propertyName));
                }
                else
                    handler(this, e);
#endif
            }
        }

#endregion
    }
}
