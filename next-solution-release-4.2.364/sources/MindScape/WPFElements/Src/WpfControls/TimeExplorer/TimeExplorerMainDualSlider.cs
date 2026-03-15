using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// The main dual slider used by a <see cref="TimeExplorer"/>. Contains additional functioanlity for displaying
  /// a marker representing the logic position of the internal dual slider.
  /// </summary>
  public class TimeExplorerMainDualSlider : DualSlider
  {
    private FrameworkElement _internalMarker;
    private double _spacing;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeExplorerMainDualSlider"/> class.
    /// </summary>
    public TimeExplorerMainDualSlider()
    {
      SizeChanged += new SizeChangedEventHandler(TimeExplorerMainDualSlider_SizeChanged);
    }

    private void TimeExplorerMainDualSlider_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdateInternalMarker();
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _internalMarker = GetTemplateChild("PART_InternalMarker") as FrameworkElement;
    }

    private void UpdateInternalMarker()
    {
      if (_internalMarker != null && !Double.IsNaN(Maximum) && !Double.IsNaN(Minimum))
      {
        double physicalWidth = ActualWidth;
        double logicalWidth = Maximum - Minimum + Spacing;
        double widthRatio = physicalWidth / logicalWidth;

        double left = logicalWidth == 0 ? 0 : (InternalRangeStart - Minimum) * widthRatio;
        double right = logicalWidth == 0 ? 0 : (InternalRangeEnd - Minimum) * widthRatio;

        _internalMarker.Margin = new Thickness(left, 0, 0, 0);
        // TODO: the minus 2 below solves an issue with the OfficeBlue style, etc.
        // This needs to be resolved.
        // The placement logic used in the DualSlider control needs to be revised.
        // The ActualWidth is 2 pixels larger than it should be, this is due to the End thumb being 11 pixels wide, but having a negative 2 margin on the left side. (See OfficeBlue)
        _internalMarker.Width = Math.Max(0, right - left - 2);
      }
    }

    internal double Spacing
    {
      get { return _spacing; }
      set
      {
        if (_spacing != value)
        {
          _spacing = value;
          UpdateInternalMarker();
        }
      }
    }

    #region InternalRangeStart property

    /// <summary>
    /// Gets or sets the start position of the internal dual slider marker.
    /// This is a dependency property.
    /// </summary>
    public double InternalRangeStart
    {
      get { return (double)GetValue(InternalRangeStartProperty); }
      set { SetValue(InternalRangeStartProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="InternalRangeStart"/> property.
    /// </summary>
    public static readonly DependencyProperty InternalRangeStartProperty =
      DependencyProperty.Register("InternalRangeStart", typeof(double), typeof(TimeExplorerMainDualSlider),
      new PropertyMetadata(new PropertyChangedCallback(OnInternalRangeStartChanged)));

    private static void OnInternalRangeStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorerMainDualSlider)d).OnInternalRangeStartChanged();
    }

    private void OnInternalRangeStartChanged()
    {
      UpdateInternalMarker();
    }

    #endregion // InternalRangeStart property

    #region InternalRangeEnd property

    /// <summary>
    /// Gets or sets the end position of the internal dual slider marker.
    /// This is a dependency property.
    /// </summary>
    public double InternalRangeEnd
    {
      get { return (double)GetValue(InternalRangeEndProperty); }
      set { SetValue(InternalRangeEndProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="InternalRangeEnd"/> property.
    /// </summary>
    public static readonly DependencyProperty InternalRangeEndProperty =
      DependencyProperty.Register("InternalRangeEnd", typeof(double), typeof(TimeExplorerMainDualSlider),
      new PropertyMetadata(new PropertyChangedCallback(OnInternalRangeEndChanged)));

    private static void OnInternalRangeEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorerMainDualSlider)d).OnInternalRangeEndChanged();
    }

    private void OnInternalRangeEndChanged()
    {
      UpdateInternalMarker();
    }

    #endregion // InternalRangeEnd property
  }
}
