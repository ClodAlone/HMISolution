using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A thumb used to resize popups.
  /// </summary>
  [TemplatePart(Name = ThumbPartName, Type = typeof(Thumb))]
  public class PopupResizer : Control
  {
    private const string ThumbPartName = "PART_Thumb";

    private Thumb _thumb;
    private Popup _popup;

    static PopupResizer()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupResizer),
        new FrameworkPropertyMetadata(typeof(PopupResizer)));
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _thumb = GetTemplateChild(ThumbPartName) as Thumb;
      if (_thumb != null)
      {
        _thumb.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
      }
      if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
      {
        UpdatePosition();
      }
    }

    private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
      Popup popup = Popup;
      if (popup != null)
      {
        popup.Width = Double.NaN;
        popup.Height = Double.NaN;
        UIElement child = popup.Child;
        if (child != null)
        {
          FrameworkElement element = child as FrameworkElement;
          if (element != null)
          {
            double maxWidth;
            double maxHeight;
            CalculateMaxDimensions(popup, element, out maxWidth, out maxHeight);

            double height = element.ActualHeight;
            double verticalMargin = 0;
            if (element.Margin != null)
            {
              verticalMargin += element.Margin.Top + element.Margin.Bottom;
            }
            if (IsTop)
            {
              height = Math.Max(0, Math.Min(maxHeight, height - e.VerticalChange));
            }
            else
            {
              height = Math.Max(0, Math.Min(maxHeight, height + e.VerticalChange));
            }
            
            element.Height = Double.NaN;
            element.Width = Double.NaN;
            element.InvalidateMeasure();
            element.Measure(new Size(Double.MaxValue, Double.PositiveInfinity));
            if (height < element.DesiredSize.Height - verticalMargin)
            {
              height = element.DesiredSize.Height - verticalMargin;
            }
            element.Height = height;

            if (ResizeMode == PopupResizeMode.Corner)
            {
              double width = element.ActualWidth;
              double minWidth = popup.MinWidth;
              double horizontalMargin = 0;
              if (element.Margin != null)
              {
                horizontalMargin = element.Margin.Left + element.Margin.Right;
                minWidth -= horizontalMargin;
              }
              if (Position == PopupResizerPosition.BottomRight || Position == PopupResizerPosition.TopRight)
              {
                width = Math.Max(0, Math.Min(maxWidth, width + e.HorizontalChange));
              }
              else if (Position == PopupResizerPosition.BottomLeft || Position == PopupResizerPosition.TopLeft)
              {
                width = Math.Max(0, Math.Min(maxWidth, width - e.HorizontalChange));
              }

              element.InvalidateMeasure();
              element.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
              if (width < element.DesiredSize.Width - horizontalMargin)
              {
                width = element.DesiredSize.Width - horizontalMargin;
              }
              element.Width = Math.Max(minWidth, width);
            }
          }
        }
      }
      Dispatcher.BeginInvoke(new Action(UpdatePositionAfterDrag), DispatcherPriority.Render);
    }

    private void UpdatePositionAfterDrag()
    {
      if (_thumb != null)
      {
        PopupResizerPosition currentPosition = Position;
        UpdatePosition();
        if (Position != currentPosition)
        {
          _thumb.CancelDrag();
        }
      }
    }

    private void CalculateMaxDimensions(Popup popup, FrameworkElement element, out double maxWidth, out double maxHeight)
    {
      maxWidth = Double.MaxValue;
      maxHeight = Double.MaxValue;
      Window window = VisualTreeUtils.FindWindow(popup);
      if (window != null)
      {
        Point pointFromWindow = popup.Child.TranslatePoint(new Point(), window);
        pointFromWindow.X = window.Left + pointFromWindow.X + element.Margin.Right + 8; // TODO: these values are the size of the top left corner of the window chrome. Should be a better way to get these.
        pointFromWindow.Y = window.Top + pointFromWindow.Y + element.Margin.Bottom + 30;
        // TODO: support multiple monitors.
        if (Position == PopupResizerPosition.BottomRight || Position == PopupResizerPosition.TopRight)
        {
          maxWidth = System.Windows.SystemParameters.PrimaryScreenWidth - pointFromWindow.X;
        }
        else if (Position == PopupResizerPosition.BottomLeft || Position == PopupResizerPosition.TopLeft)
        {
          maxWidth = element.ActualWidth + pointFromWindow.X - 2;
        }

        if (IsTop)
        {
          maxHeight = element.ActualHeight + pointFromWindow.Y - 2;
        }
        else
        {
          maxHeight = System.Windows.SystemParameters.PrimaryScreenHeight - pointFromWindow.Y;
        }
      }
    }

    private Popup Popup
    {
      get
      {
        if (_popup == null)
        {
          _popup = VisualTreeUtils.FindPopup(this);
          if (_popup != null)
          {
            _popup.Opened += new EventHandler(Popup_Opened);
          }
          UpdatePosition();
        }
        return _popup;
      }
    }

    // TODO current issues:
    // Simply setting element.Width and Height to NaN causes WrapPanel issues in the dropdown Gallery.
    // So we also do some measuring logic and set the width to be the desired width.
    // But it is not perfect - Moving a bottom-edge resizer can cause a simple menu to suddenly shrink in width by a few pixels.
    //
    // Dragging a TopLeft resizer down to cause the popup to jump below the parent popup causes the popup to close when the mouse is released.
    //
    // Same as above, when the popup is opened again, the resizer is now on the bottom edge whereas it should be on the top edge.
    //
    // If the popup appears above it's parent, resizing it upwards seems to cause the popup to be clipped by an amount relative to the
    // offset between the popup and it's parent. This seems like a bug in WPF not the resize logic. Should look into this.
    private void Popup_Opened(object sender, EventArgs e)
    {
      Popup popup = Popup;
      if (popup != null)
      {
        popup.Width = Double.NaN;
        popup.Height = Double.NaN;
        FrameworkElement element = popup.Child as FrameworkElement;
        if (element != null)
        {
          element.Width = Double.NaN;
          element.Height = Double.NaN;
          element.InvalidateMeasure();
          element.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
          element.Width = Math.Max(popup.MinWidth - element.Margin.Right, element.DesiredSize.Width);
          //element.Height = element.DesiredSize.Height;
        }
      }
      UpdatePosition();
      //Dispatcher.BeginInvoke(new Action(UpdatePosition), DispatcherPriority.Render);
    }

    private void UpdatePosition()
    {
      PopupResizerPosition? position = null;
      Popup popup = Popup;
      if (popup != null && popup.Child != null)
      {
        Popup parentPopup = VisualTreeUtils.FindPopup(popup);
        UIElement parent = null;
        if (parentPopup != null)
        {
          parent = parentPopup.Child;
        }
        else
        {
          parent = VisualTreeHelper.GetParent(popup) as UIElement;
        }
        if (parent != null)
        {
          Point relativePosition = parent.TranslatePoint(new Point(), popup.Child);
          if (ResizeMode == PopupResizeMode.Corner)
          {
            if (relativePosition.X > 0)
            {
              if (relativePosition.Y < 5)
              {
                position = PopupResizerPosition.BottomLeft;
              }
              else
              {
                position = PopupResizerPosition.TopLeft;
              }
            }
            else
            {
              if (relativePosition.Y < 5)
              {
                position = PopupResizerPosition.BottomRight;
              }
              else
              {
                position = PopupResizerPosition.TopRight;
              }
            }
          }
          else
          {
            if (relativePosition.Y < 5)
            {
              position = PopupResizerPosition.Bottom;
            }
            else
            {
              position = PopupResizerPosition.Top;
            }
          }
        }
      }
      if (position != null)
      {
        SetValue(PositionPropertyKey, position);
      }
      else if (ResizeMode == PopupResizeMode.Corner)
      {
        SetValue(PositionPropertyKey, PopupResizerPosition.BottomRight);
      }
      else
      {
        SetValue(PositionPropertyKey, PopupResizerPosition.Bottom);
      }
      PopupMenuPanel panel = VisualTreeUtils.FindAncestor<PopupMenuPanel>(this);
      if (panel != null)
      {
        panel.InvalidateMeasure();
      }
    }

    internal bool IsTop
    {
      get
      {
        return Position == PopupResizerPosition.Top || Position == PopupResizerPosition.TopLeft || Position == PopupResizerPosition.TopRight;
      }
    }

    #region ResizeMode Property

    /// <summary>
    /// Gets or sets the resize mode of this <see cref="PopupResizer"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ResizeModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public PopupResizeMode ResizeMode
    {
      get { return (PopupResizeMode)GetValue(ResizeModeProperty); }
      set { SetValue(ResizeModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ResizeMode"/> property.
    /// </summary>
    public static readonly DependencyProperty ResizeModeProperty =
      DependencyProperty.Register("ResizeMode", typeof(PopupResizeMode), typeof(PopupResizer),
      new FrameworkPropertyMetadata(PopupResizeMode.Edge, OnResizeModeChanged));

    private static void OnResizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PopupResizer)d).OnResizeModeChanged();
    }

    private void OnResizeModeChanged()
    {
    }

    #endregion // ResizeMode Property

    #region Position Property

    /// <summary>
    /// Gets the position of this <see cref="PopupResizer"/> within the popup.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public PopupResizerPosition Position
    {
      get { return (PopupResizerPosition)GetValue(PositionProperty); }
    }

    private static readonly DependencyPropertyKey PositionPropertyKey =
        DependencyProperty.RegisterReadOnly("Position", typeof(PopupResizerPosition), typeof(PopupResizer), new UIPropertyMetadata(PopupResizerPosition.Bottom));

    /// <summary>
    /// Identifies the <see cref="Position"/> property.
    /// </summary>
    public static readonly DependencyProperty PositionProperty =
        PositionPropertyKey.DependencyProperty;

    #endregion // Position Property
  }
}
