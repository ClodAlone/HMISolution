using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Utilities
{
    /// <summary>
    /// Notifies the clients that a property has changed
    /// </summary>
    public interface ICollectionItemNotifyPropertyChanged : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        event PropertyChangedEventHandler CollectionItemPropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="e"></param>
        void NotifyPropertyChanged(PropertyChangedEventArgs e);
    }
}
