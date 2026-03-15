using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Implements a selectable item within a <see cref="CoverFlow"/>.
  /// </summary>
  public class CoverFlowItem : ContentControl
  {
    static CoverFlowItem()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CoverFlowItem), 
        new FrameworkPropertyMetadata(typeof(CoverFlowItem)));
    }

    /// <summary>
    /// Identifies the template for selection transition animation.
    /// </summary>
    public static ComponentResourceKey TransitionAnimationKey
    {
      get { return new ComponentResourceKey(typeof(CoverFlowItem), "TransitionAnimation"); }
    }

    internal void SetItemHeightWidth(double width, double height, bool showReflection)
    {
      Height = height;
      Width = width;

      if (showReflection)
      {
        height *= 2;
      }

      SetValue(ClipBottomLeftPropertyKey, new Point(0, height));
      SetValue(ClipTopRightPropertyKey, new Point(width, 0));
      SetValue(ClipBottomRightPropertyKey, new Point(width, height));
    }

    internal void SetZIndex(int zIndex)
    {
      // Binding not behaving.  Procedural code FTW *sigh*.
      Canvas.SetZIndex(this, zIndex);
    }

    internal void MoveTo(double scaleX, double scaleY, double skewAngleY, double left, double itemWidth, Duration duration)
    {
      Storyboard baseStoryboard = (Storyboard)(Template.Resources[TransitionAnimationKey]);
      Storyboard storyboard = baseStoryboard.Clone();

      ((DoubleAnimation)storyboard.Children[0]).Duration = duration;
      ((DoubleAnimation)storyboard.Children[1]).Duration = duration;
      ((DoubleAnimation)storyboard.Children[2]).Duration = duration;
      ((DoubleAnimation)storyboard.Children[3]).Duration = duration;
      ((PointAnimation)storyboard.Children[4]).Duration = duration;
      ((PointAnimation)storyboard.Children[5]).Duration = duration;

      ((DoubleAnimation)storyboard.Children[0]).To = scaleX;
      ((DoubleAnimation)storyboard.Children[1]).To = scaleY;
      ((DoubleAnimation)storyboard.Children[2]).To = skewAngleY;
      ((DoubleAnimation)storyboard.Children[3]).To = left;
      ((PointAnimation)storyboard.Children[4]).To = new Point(0, 0);
      ((PointAnimation)storyboard.Children[5]).To = new Point(itemWidth, 0);

      storyboard.Begin(this);
    }

    #region Mouse selection event translation

    internal static readonly RoutedEvent CoverFlowItemMouseActionEvent =
      EventManager.RegisterRoutedEvent("CoverFlowItemMouseAction", RoutingStrategy.Bubble,
        typeof(EventHandler<CoverFlowItemMouseActionEventArgs>), typeof(CoverFlowItem));

    internal enum MouseAction { Entered, Pressed }

    internal class CoverFlowItemMouseActionEventArgs : RoutedEventArgs
    {
      private readonly MouseAction _action;

      public CoverFlowItemMouseActionEventArgs(MouseAction action)
        : base(CoverFlowItem.CoverFlowItemMouseActionEvent)
      {
        _action = action;
      }

      internal bool Matches(CoverFlowMouseSelectionMode mode)
      {
        return (_action == MouseAction.Entered && mode == CoverFlowMouseSelectionMode.MouseEnter)
          || (_action == MouseAction.Pressed && mode == CoverFlowMouseSelectionMode.MousePressed);
      }
    }

    /// <summary>
    /// Responds to a <see cref="UIElement.MouseEnter"/> event.
    /// </summary>
    /// <param name="e">Provides data for <see cref="MouseEventArgs"/>.</param>
    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);

      RaiseEvent(new CoverFlowItemMouseActionEventArgs(MouseAction.Entered));

      e.Handled = false;
    }

    /// <summary>
    /// Responds to a <see cref="UIElement.PreviewMouseDown"/> event.
    /// </summary>
    /// <param name="e">Provides data for <see cref="MouseButtonEventArgs"/>.</param>
    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseDown(e);

      RaiseEvent(new CoverFlowItemMouseActionEventArgs(MouseAction.Pressed));

      e.Handled = false;
    }

    #endregion

    #region Transition clip parallelogram

    /// <summary>
    /// Gets the bottom left of the clip parallelogram.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ClipBottomLeftProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Point ClipBottomLeft
    {
      get { return (Point)GetValue(ClipBottomLeftProperty); }
    }

    private static readonly DependencyPropertyKey ClipBottomLeftPropertyKey =
        DependencyProperty.RegisterReadOnly("ClipBottomLeft", typeof(Point), typeof(CoverFlowItem), new UIPropertyMetadata(new Point()));

    /// <summary>
    /// Identifies the <see cref="ClipBottomLeft"/> property.
    /// </summary>
    public static readonly DependencyProperty ClipBottomLeftProperty =
        ClipBottomLeftPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the top right of the clip parallelogram.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ClipTopRightProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Point ClipTopRight
    {
      get { return (Point)GetValue(ClipTopRightProperty); }
    }

    private static readonly DependencyPropertyKey ClipTopRightPropertyKey =
        DependencyProperty.RegisterReadOnly("ClipTopRight", typeof(Point), typeof(CoverFlowItem), new UIPropertyMetadata(new Point()));

    /// <summary>
    /// Identifies the <see cref="ClipTopRight"/> property.
    /// </summary>
    public static readonly DependencyProperty ClipTopRightProperty =
        ClipTopRightPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the bottom right of the clip parallelogram.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ClipBottomRightProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Point ClipBottomRight
    {
      get { return (Point)GetValue(ClipBottomRightProperty); }
    }

    private static readonly DependencyPropertyKey ClipBottomRightPropertyKey =
        DependencyProperty.RegisterReadOnly("ClipBottomRight", typeof(Point), typeof(CoverFlowItem), new UIPropertyMetadata(new Point()));

    /// <summary>
    /// Identifies the <see cref="ClipBottomRight"/> property.
    /// </summary>
    public static readonly DependencyProperty ClipBottomRightProperty =
        ClipBottomRightPropertyKey.DependencyProperty;

    #endregion
  }
}
