using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Mindscape.WpfElements
{
  internal static class VisualTreeUtils
  {
    public static T FindAncestor<T>(DependencyObject from)
      where T : class
    {
      if (from == null)
      {
        return null;
      }

      T candidate = from as T;
      if (candidate != null)
      {
        return candidate;
      }

      return FindAncestor<T>(VisualTreeHelper.GetParent(from));
    }

    public static T GetAncestorOfFocusedElement<T>() where T : UIElement
    {
      return FindAncestor<T>(Keyboard.FocusedElement as UIElement);
    }

    public static Popup FindPopup(DependencyObject from)
    {
      DependencyObject popupRoot = FindAncestor("PopupRoot", from);
      Popup popup = FindLogicalAncestor<Popup>(popupRoot);
      return popup;
    }

    public static Window FindWindow(FrameworkElement from)
    {
      Popup popup = FindPopup(from);
      while (popup != null)
      {
        from = popup;
        popup = FindPopup(popup);
      }
      Window window = FindAncestor<Window>(from);
      return window;
    }

    public static DependencyObject FindAncestor(string typeName, DependencyObject from)
    {
      if (from == null)
      {
        return null;
      }

      if (from.GetType().Name.Equals(typeName))
      {
        return from;
      }

      return FindAncestor(typeName, VisualTreeHelper.GetParent(from));
    }

    public static T FindLogicalAncestor<T>(DependencyObject from)
      where T : class
    {
      if (from == null)
      {
        return null;
      }

      DependencyObject parent = LogicalTreeHelper.GetParent(from);
      T candidate = parent as T;
      if (candidate != null)
      {
        return candidate;
      }

      return FindLogicalAncestor<T>(parent);
    }

    public static T GetChild<T>(DependencyObject startingFrom)
      where T : class
    {
      int count = VisualTreeHelper.GetChildrenCount(startingFrom);
      for (int i = 0; i < count; i++)
      {
        DependencyObject o = VisualTreeHelper.GetChild(startingFrom, i);
        T result = o as T;
        if (result != null)
        {
          return result;
        }
        result = GetChild<T>(o);
        if (result != null)
        {
          return result;
        }
      }
      return null;
    }

    public static IList<T> GetChildren<T>(DependencyObject startingFrom)
      where T : class
    {
      IList<T> children = new List<T>();
      int count = VisualTreeHelper.GetChildrenCount(startingFrom);
      for (int i = 0; i < count; i++)
      {
        DependencyObject o = VisualTreeHelper.GetChild(startingFrom, i);
        T result = o as T;
        if (result != null)
        {
          children.Add(result);
          return children;
        }
        IList<T> list = new List<T>();
        list = GetChildren<T>(o);
        foreach (T t in list)
        {
          children.Add(t);
        }
      }
      return children;
    }

    public static T FindContaining<T>(DependencyObject startingFrom)
      where T : class
    {
      DependencyObject obj = startingFrom;
      while (obj != null && obj is Visual)
      {
        T t = obj as T;
        if (t != null)
        {
          return t;
        }
        obj = VisualTreeHelper.GetParent(obj);
      }
      return null;
    }
  }
}
