using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A simple control for displaying dialog content in an XBAP.
  /// </summary>
  public class ChildWindow : ContentControl
  {
    static ChildWindow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChildWindow),
        new FrameworkPropertyMetadata(typeof(ChildWindow)));
    }

    /// <summary>
    /// Gets or sets the WindowHeight.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="WindowHeightProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double WindowHeight
    {
      get { return (double)GetValue(WindowHeightProperty); }
      set { SetValue(WindowHeightProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WindowHeight"/> property.
    /// </summary>
    public static readonly DependencyProperty WindowHeightProperty =
      DependencyProperty.Register("WindowHeight", typeof(double), typeof(ChildWindow),
      new FrameworkPropertyMetadata(100.0));

    /// <summary>
    /// Gets or sets the WindowWidth.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="WindowWidthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double WindowWidth
    {
      get { return (double)GetValue(WindowWidthProperty); }
      set { SetValue(WindowWidthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WindowWidth"/> property.
    /// </summary>
    public static readonly DependencyProperty WindowWidthProperty =
      DependencyProperty.Register("WindowWidth", typeof(double), typeof(ChildWindow),
      new FrameworkPropertyMetadata(100.0));

    /// <summary>
    /// Gets or sets the BackgroundBrush.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="BackgroundBrushProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Brush BackgroundBrush
    {
      get { return (Brush)GetValue(BackgroundBrushProperty); }
      set { SetValue(BackgroundBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BackgroundBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty BackgroundBrushProperty =
      DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(ChildWindow),
      new FrameworkPropertyMetadata(new SolidColorBrush()));
  }
}
