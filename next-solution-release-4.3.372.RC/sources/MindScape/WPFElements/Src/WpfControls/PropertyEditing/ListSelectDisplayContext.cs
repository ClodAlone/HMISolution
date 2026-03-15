using System.Windows;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Provides display context information for options in list-selection editors.
  /// </summary>
  public static class ListSelectDisplayContext
  {
    /// <summary>
    /// Gets the value of the PropertyInfo attached property of the specified element.
    /// </summary>
    /// <param name="obj">The element from which to read the value.</param>
    /// <returns>The PropertyInfo associated with the specified element.</returns>
    public static IPropertyInfo GetPropertyInfo(DependencyObject obj)
    {
      return (IPropertyInfo)obj.GetValue(PropertyInfoProperty);
    }

    /// <summary>
    /// Sets the value of the PropertyInfo attached property of the specified element.
    /// </summary>
    /// <param name="obj">The element on which to set the value.</param>
    /// <param name="value">The PropertyInfo to be associated with the specified element.</param>
    public static void SetPropertyInfo(DependencyObject obj, IPropertyInfo value)
    {
      obj.SetValue(PropertyInfoProperty, value);
    }

    /// <summary>
    /// Identifies the PropertyInfo attached property.
    /// </summary>
    public static readonly DependencyProperty PropertyInfoProperty =
      DependencyProperty.RegisterAttached("PropertyInfo", 
      typeof(IPropertyInfo), typeof(ListSelectDisplayContext));
  }
}
