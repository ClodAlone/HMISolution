using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

//namespace Monogram.WpfUtils
//without namespace will be available anywhere where WpfUtils is referenced

public static class Extensions
{
  /// <returns>index of item</returns>
  public static int IndexOf<T>(this IEnumerable<T> enumerable, T item)
  {
    int i = 0;
    foreach (T current in enumerable)
    {
      if (current.Equals(item)) return i;
      i++;
    }
    return -1;
  }


  /// <summary>
  /// recursively searches VisualTree for child of specific name and type
  /// </summary>
  public static T FindVisualChild<T>(this FrameworkElement root, string name) where T : FrameworkElement
  {
    return (T)WpfHelper.FindVisualChild(root, e => e.Name == name && e is T, 256);
  }

  /// <summary>
  /// recursively searches LogicalTree for child of specific name and type
  /// </summary>
  public static T FindLogicalChild<T>(this FrameworkElement root, string name) where T : FrameworkElement
  {
    return (T)WpfHelper.FindLogicalChild(root, e => e.Name == name && e is T, 256);
  }
}

