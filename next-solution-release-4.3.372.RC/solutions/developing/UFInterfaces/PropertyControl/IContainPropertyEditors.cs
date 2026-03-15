using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UFInterfaces.PropertyControl
{
    /// <summary>
    /// Define the interface for retriving the data template editor collection of an object.
    /// </summary>
    public interface IContainPropertyEditors
    {
        /// <summary>
        /// The object type of the object that defines the data templates dictionary.
        /// </summary>
        Type ObjectType { get;  }

        /// <summary>
        /// A dictionary that defines the property name and data template of each dependency property that want a data tamplate.
        /// </summary>
        IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates { get; }
    }
}
