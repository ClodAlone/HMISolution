using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;


public static class WpfHelper
{
  public static bool IsDesignMode
  {
    get
    {
      if (!isDesignMode.HasValue)
      {
#if SILVERLIGHT
            _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
        var prop = DesignerProperties.IsInDesignModeProperty;
        isDesignMode
            = (bool)DependencyPropertyDescriptor
            .FromProperty(prop, typeof(FrameworkElement))
            .Metadata.DefaultValue;
#endif
      }

      return isDesignMode.Value;
    }
  }
  private static bool? isDesignMode;


  /// <summary>recursively searches VisualThree</summary>
  public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
  {
    //get parent item
    DependencyObject parentObject = VisualTreeHelper.GetParent(child);

    //we've reached the end of the tree
    if (parentObject == null) return null;

    //check if the parent matches the type we're looking for
    T parent = parentObject as T;
    if (parent != null)
    {
      return parent;
    }
    else
    {
      return FindVisualParent<T>(parentObject);
    }
  }

  /// <summary>recursively searches LogicalThree</summary>
  public static T FindLogicalParent<T>(DependencyObject child) where T : DependencyObject
  {
    //get parent item
    DependencyObject parentObject = LogicalTreeHelper.GetParent(child);

    //we've reached the end of the tree
    if (parentObject == null) return null;

    //check if the parent matches the type we're looking for
    T parent = parentObject as T;
    if (parent != null)
    {
      return parent;
    }
    else
    {
      return FindLogicalParent<T>(parentObject);
    }
  }

  /// <summary>recursively searches VisualThree</summary>
  public static bool IsLogicalParent(DependencyObject parent, DependencyObject child)
  {
    var parentObject = child;

    while (parentObject != null && parentObject != parent)
    {
      parentObject = LogicalTreeHelper.GetParent(parentObject) as DependencyObject;
    }
    return parentObject != null;
  }

  /// <summary>recursively searches VisualThree</summary>
  public static FrameworkElement FindVisualChild(FrameworkElement root, Predicate<FrameworkElement> predicate, int depth)
  {
    if (depth == 0) return null;

    int count = VisualTreeHelper.GetChildrenCount(root);
    for(int i = 0; i < count; i++)
    {
      var elem = VisualTreeHelper.GetChild(root, i) as FrameworkElement;
      if (elem != null && predicate(elem)) return elem;
    }

    for (int j = 0; j < count; j++)
    {
      var elem = VisualTreeHelper.GetChild(root, j) as FrameworkElement;
      if (elem != null)
      {
        var elem2 = FindVisualChild(elem, predicate, depth - 1);
        if (elem2 != null) return elem2;
      }
    }

      
    return null;
  }

  /// <summary>recursively searches LogicalThree</summary>
  public static FrameworkElement FindLogicalChild(FrameworkElement root, Predicate<FrameworkElement> predicate, int depth)
  {
    if (depth == 0) return null;

    foreach(object child in LogicalTreeHelper.GetChildren(root))
    {
      var elem = child as FrameworkElement;
      if (elem != null && predicate(elem)) return elem;
    }

    foreach (object child in LogicalTreeHelper.GetChildren(root))
    {
      var elem = child as FrameworkElement;
      if (elem != null)
      {
        var elem2 = FindLogicalChild(elem, predicate, depth - 1);
        if (elem2 != null) return elem2;
      }
    }


    return null;
  }

  
  
  /// <summary>
  /// returns typed enum value. (no additional retyping is needed)
  /// </summary>
  /// <typeparam name="T">must be enum type</typeparam>
  /// <param name="namedValue"></param>
  public static T ParseEnum<T>(string namedValue)
  {
    if (typeof(T).BaseType != typeof(Enum)) throw new ArgumentException("Parameter T must be enum type");
    return (T)Enum.Parse(typeof(T), namedValue);
  }

}
