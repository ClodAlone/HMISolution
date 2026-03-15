using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// An attached behavior for clipping a <see cref="FrameworkElement"/> by the relative bounds of another <see cref="FrameworkElement"/>.
  /// </summary>
  public static class ClipToElement
  {
    /// <summary>
    /// Gets the ClipToElement property value for the given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The <see cref="DependencyObject"/> to get the property value from.</param>
    /// <returns>The <see cref="FrameworkElement"/> to use as the clipping source.</returns>
    public static FrameworkElement GetClipToElement(DependencyObject obj)
    {
      return (FrameworkElement)obj.GetValue(ClipToElementProperty);
    }

    /// <summary>
    /// Sets the ClipToElement property value for the given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The <see cref="DependencyObject"/> to set the property value.</param>
    /// <param name="clipToBounds">The <see cref="FrameworkElement"/> to use as the clipping source.</param>
    public static void SetClipToElement(DependencyObject obj, FrameworkElement clipToBounds)
    {
      obj.SetValue(ClipToElementProperty, clipToBounds);
    }

    /// <summary>
    /// Identifies the ClipToElement attached property.
    /// </summary>
    public static readonly DependencyProperty ClipToElementProperty = DependencyProperty.RegisterAttached("ClipToElement", typeof(FrameworkElement),
        typeof(ClipToElement), new PropertyMetadata(null, OnClipToElementPropertyChanged));

    private static void OnClipToElementPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = obj as FrameworkElement;
      if (element != null)
      {
        FrameworkElement clipElement = (FrameworkElement)e.NewValue;
        
        if (clipElement != null)
        {
          ClipToBounds(element);
          element.Loaded += new RoutedEventHandler(Element_Loaded);
          element.SizeChanged += new SizeChangedEventHandler(Element_SizeChanged);
          clipElement.Loaded += new RoutedEventHandler(ClipElement_Loaded);
          clipElement.SizeChanged += new SizeChangedEventHandler(ClipElement_SizeChanged);
        }
        else
        {
          element.Clip = null;
          element.Loaded -= new RoutedEventHandler(Element_Loaded);
          element.SizeChanged -= new SizeChangedEventHandler(Element_SizeChanged);
          FrameworkElement oldClipElement = (FrameworkElement)e.OldValue;
          if (oldClipElement != null)
          {
            oldClipElement.Loaded -= new RoutedEventHandler(ClipElement_Loaded);
            oldClipElement.SizeChanged -= new SizeChangedEventHandler(ClipElement_SizeChanged);
          }
        }
      }
    }

    private static void ClipToBounds(FrameworkElement element)
    {
      FrameworkElement clipElement = GetClipToElement(element);
      if (clipElement != null)
      {
        GeneralTransform transform = element.TransformToVisual(clipElement);
        Point offset = transform.Transform(new Point(0, 0));
        double x = Math.Max(0, -offset.X);
        double y = Math.Max(0, -offset.Y);
        double width = Math.Min(clipElement.ActualWidth, clipElement.ActualWidth - offset.X);
        double height = Math.Min(clipElement.ActualHeight, clipElement.ActualHeight - offset.Y);
        element.Clip = new RectangleGeometry() { Rect = new Rect(x, y, width, height) };
      }
      else
      {
        element.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, element.ActualWidth, element.ActualHeight) };
      }
    }

    private static void Element_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ClipToBounds(sender as FrameworkElement);
    }

    private static void Element_Loaded(object sender, RoutedEventArgs e)
    {
      ClipToBounds(sender as FrameworkElement);
    }

    private static void ClipElement_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ClipToBounds(sender as FrameworkElement);
    }

    private static void ClipElement_Loaded(object sender, RoutedEventArgs e)
    {
      ClipToBounds(sender as FrameworkElement);
    }
  }
}
