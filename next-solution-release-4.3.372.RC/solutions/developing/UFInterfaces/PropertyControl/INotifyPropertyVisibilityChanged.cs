using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFInterfaces.PropertyControl
{
    /// <summary>
    /// Provides the functionality to offer visibility information that a user interface can bind to.
    /// </summary>
    public interface INotifyPropertyVisibilityChanged
    {
        // Summary:
        //     Gets the visibility state for the property with the given name.
        //
        // Parameters:
        //   columnName:
        //     The name of the property whose visibility state to get.
        //
        // Returns:
        //     The visibility state for the property. The default value is 'true'.
        bool this[string propertyName] { get; }

        // Summary:
        //     Occurs when a property visibility changed.
        event PropertyChangedEventHandler PropertyVisiblityChanged;
    }
}
