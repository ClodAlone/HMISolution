using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// The graphical representation of data points in a <see cref="BoxplotSeries"/>.
  /// </summary>
  public class BoxAndWhiskers : CartesianDataPoint
  {
    static BoxAndWhiskers()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BoxAndWhiskers),
        new FrameworkPropertyMetadata(typeof(BoxAndWhiskers)));
    }

    internal BoxAndWhiskers(object data, Orientation orientation)
    {
      DataContext = data;
      Orientation = orientation;
    }

    /// <summary>
    /// Gets the <see cref="Orientation"/> of the <see cref="BoxAndWhiskers"/>.
    /// </summary>
    public Orientation Orientation { get; private set; }

    #region Minimum Property

    /// <summary>
    /// Gets or sets the logical minimum value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinimumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double Minimum
    {
      get { return (double)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
      DependencyProperty.Register("Minimum", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnMinimumChanged));

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnMinimumChanged();
    }

    private void OnMinimumChanged()
    {
    }

    #endregion // Minimum Property

    #region Maximum Property

    /// <summary>
    /// Gets or sets the logical maximum value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaximumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double Maximum
    {
      get { return (double)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
      DependencyProperty.Register("Maximum", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnMaximumChanged));

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnMaximumChanged();
    }

    private void OnMaximumChanged()
    {
    }

    #endregion // Maximum Property

    #region LowerQuartile Property

    /// <summary>
    /// Gets or sets the logical lower quartile value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LowerQuartileProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double LowerQuartile
    {
      get { return (double)GetValue(LowerQuartileProperty); }
      set { SetValue(LowerQuartileProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LowerQuartile"/> property.
    /// </summary>
    public static readonly DependencyProperty LowerQuartileProperty =
      DependencyProperty.Register("LowerQuartile", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnLowerQuartileChanged));

    private static void OnLowerQuartileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnLowerQuartileChanged();
    }

    private void OnLowerQuartileChanged()
    {
    }

    #endregion // LowerQuartile Property

    #region UpperQuartile Property

    /// <summary>
    /// Gets or sets the logical upper quartile value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="UpperQuartileProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double UpperQuartile
    {
      get { return (double)GetValue(UpperQuartileProperty); }
      set { SetValue(UpperQuartileProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="UpperQuartile"/> property.
    /// </summary>
    public static readonly DependencyProperty UpperQuartileProperty =
      DependencyProperty.Register("UpperQuartile", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnUpperQuartileChanged));

    private static void OnUpperQuartileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnUpperQuartileChanged();
    }

    private void OnUpperQuartileChanged()
    {
    }

    #endregion // UpperQuartile Property

    #region Median Property

    /// <summary>
    /// Gets or sets the logical median value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MedianProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double Median
    {
      get { return (double)GetValue(MedianProperty); }
      set { SetValue(MedianProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Median"/> property.
    /// </summary>
    public static readonly DependencyProperty MedianProperty =
      DependencyProperty.Register("Median", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnMedianChanged));

    private static void OnMedianChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnMedianChanged();
    }

    private void OnMedianChanged()
    {
    }

    #endregion // Median Property

    #region MinimumPosition Property

    /// <summary>
    /// Gets or sets the physical position of the minimum value from the top of this data point.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinimumPositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double MinimumPosition
    {
      get { return (double)GetValue(MinimumPositionProperty); }
      set { SetValue(MinimumPositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinimumPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumPositionProperty =
      DependencyProperty.Register("MinimumPosition", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnMinimumPositionChanged));

    private static void OnMinimumPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnMinimumPositionChanged();
    }

    private void OnMinimumPositionChanged()
    {
    }

    #endregion // MinimumPosition Property

    #region MaximumPosition Property

    /// <summary>
    /// Gets or sets the physical position of the maximum value from the top of this data point.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaximumPositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double MaximumPosition
    {
      get { return (double)GetValue(MaximumPositionProperty); }
      set { SetValue(MaximumPositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaximumPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty MaximumPositionProperty =
      DependencyProperty.Register("MaximumPosition", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnMaximumPositionChanged));

    private static void OnMaximumPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnMaximumPositionChanged();
    }

    private void OnMaximumPositionChanged()
    {
    }

    #endregion // MaximumPosition Property

    #region LowerQuartilePosition Property

    /// <summary>
    /// Gets or sets the physical position of the lower quartile value from the top of this data point.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LowerQuartilePositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double LowerQuartilePosition
    {
      get { return (double)GetValue(LowerQuartilePositionProperty); }
      set { SetValue(LowerQuartilePositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LowerQuartilePosition"/> property.
    /// </summary>
    public static readonly DependencyProperty LowerQuartilePositionProperty =
      DependencyProperty.Register("LowerQuartilePosition", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnLowerQuartilePositionChanged));

    private static void OnLowerQuartilePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnLowerQuartilePositionChanged();
    }

    private void OnLowerQuartilePositionChanged()
    {
      BoxHeight = Math.Max(0, LowerQuartilePosition - UpperQuartilePosition);
    }

    #endregion // LowerQuartilePosition Property

    #region UpperQuartilePosition Property

    /// <summary>
    /// Gets or sets the physical position of the upper quartile value from the top of this data point.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="UpperQuartilePositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double UpperQuartilePosition
    {
      get { return (double)GetValue(UpperQuartilePositionProperty); }
      set { SetValue(UpperQuartilePositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="UpperQuartilePosition"/> property.
    /// </summary>
    public static readonly DependencyProperty UpperQuartilePositionProperty =
      DependencyProperty.Register("UpperQuartilePosition", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnUpperQuartilePositionChanged));

    private static void OnUpperQuartilePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnUpperQuartilePositionChanged();
    }

    private void OnUpperQuartilePositionChanged()
    {
      BoxHeight = Math.Max(0, LowerQuartilePosition - UpperQuartilePosition);
    }

    #endregion // UpperQuartilePosition Property

    #region MedianPosition Property

    /// <summary>
    /// Gets or sets the physical position of the median value from the top of this data point.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MedianPositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double MedianPosition
    {
      get { return (double)GetValue(MedianPositionProperty); }
      set { SetValue(MedianPositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MedianPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty MedianPositionProperty =
      DependencyProperty.Register("MedianPosition", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnMedianPositionChanged));

    private static void OnMedianPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnMedianPositionChanged();
    }

    private void OnMedianPositionChanged()
    {
    }

    #endregion // MedianPosition Property

    #region BoxWidth Property

    /// <summary>
    /// Gets or sets the width of the box.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="BoxWidthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double BoxWidth
    {
      get { return (double)GetValue(BoxWidthProperty); }
      set { SetValue(BoxWidthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BoxWidth"/> property.
    /// </summary>
    public static readonly DependencyProperty BoxWidthProperty =
      DependencyProperty.Register("BoxWidth", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnBoxWidthChanged));

    private static void OnBoxWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnBoxWidthChanged();
    }

    private void OnBoxWidthChanged()
    {
    }

    #endregion // BoxWidth Property

    #region BoxHeight Property

    /// <summary>
    /// Gets or sets the height of the box.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="BoxHeightProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double BoxHeight
    {
      get { return (double)GetValue(BoxHeightProperty); }
      set { SetValue(BoxHeightProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BoxHeight"/> property.
    /// </summary>
    public static readonly DependencyProperty BoxHeightProperty =
      DependencyProperty.Register("BoxHeight", typeof(double), typeof(BoxAndWhiskers),
      new FrameworkPropertyMetadata(OnBoxHeightChanged));

    private static void OnBoxHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxAndWhiskers)d).OnBoxHeightChanged();
    }

    private void OnBoxHeightChanged()
    {
    }

    #endregion // BoxHeight Property
  }
}
