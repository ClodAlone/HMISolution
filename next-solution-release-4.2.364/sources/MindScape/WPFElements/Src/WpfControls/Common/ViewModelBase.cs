using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Provides property change notification servics for view model objects.
  /// </summary>
  public abstract class ViewModelBase : INotifyPropertyChanged
  {
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// Sets the specified field, raising the <see cref="PropertyChanged"/> event if required.
    /// </summary>
    /// <typeparam name="T">The type of field.</typeparam>
    /// <param name="field">The field to set.</param>
    /// <param name="value">The value to which to set the field.</param>
    /// <param name="propertyName">The name of the property for the <see cref="PropertyChanged"/> event.</param>
    protected void Set<T>(ref T field, T value, string propertyName)
    {
      if (!Object.Equals(field, value))
      {
        field = value;
        OnPropertyChanged(propertyName);
      }
    }
  }
}
