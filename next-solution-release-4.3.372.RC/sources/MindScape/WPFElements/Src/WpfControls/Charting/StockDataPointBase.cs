using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A base class for data points used by stock charts.
  /// </summary>
  public abstract class StockDataPointBase : CartesianDataPoint
  {
    #region Low Property

    /// <summary>
    /// Gets or sets the logical low value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LowProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double Low
    {
      get { return (double)GetValue(LowProperty); }
      set { SetValue(LowProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Low"/> property.
    /// </summary>
    public static readonly DependencyProperty LowProperty =
      DependencyProperty.Register("Low", typeof(double), typeof(StockDataPointBase),
      new FrameworkPropertyMetadata(OnLowChanged));

    private static void OnLowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StockDataPointBase)d).OnLowChanged();
    }

    private void OnLowChanged()
    {
    }

    #endregion // Low Property

    #region High Property

    /// <summary>
    /// Gets or sets the logical high value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HighProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double High
    {
      get { return (double)GetValue(HighProperty); }
      set { SetValue(HighProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="High"/> property.
    /// </summary>
    public static readonly DependencyProperty HighProperty =
      DependencyProperty.Register("High", typeof(double), typeof(StockDataPointBase),
      new FrameworkPropertyMetadata(OnHighChanged));

    private static void OnHighChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StockDataPointBase)d).OnHighChanged();
    }

    private void OnHighChanged()
    {
    }

    #endregion // High Property

    #region Open Property

    /// <summary>
    /// Gets or sets the logical open value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="OpenProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double Open
    {
      get { return (double)GetValue(OpenProperty); }
      set { SetValue(OpenProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Open"/> property.
    /// </summary>
    public static readonly DependencyProperty OpenProperty =
      DependencyProperty.Register("Open", typeof(double), typeof(StockDataPointBase),
      new FrameworkPropertyMetadata(OnOpenChanged));

    private static void OnOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StockDataPointBase)d).OnOpenChanged();
    }

    private void OnOpenChanged()
    {
    }

    #endregion // Open Property

    #region Close Property

    /// <summary>
    /// Gets or sets the logical close value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CloseProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double Close
    {
      get { return (double)GetValue(CloseProperty); }
      set { SetValue(CloseProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Close"/> property.
    /// </summary>
    public static readonly DependencyProperty CloseProperty =
      DependencyProperty.Register("Close", typeof(double), typeof(StockDataPointBase),
      new FrameworkPropertyMetadata(OnCloseChanged));

    private static void OnCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StockDataPointBase)d).OnCloseChanged();
    }

    private void OnCloseChanged()
    {
    }

    #endregion // Close Property

    #region OpenPosition Property

    /// <summary>
    /// Gets or sets the physical position of the open point from the top of this data point.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="OpenPositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double OpenPosition
    {
      get { return (double)GetValue(OpenPositionProperty); }
      set { SetValue(OpenPositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="OpenPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty OpenPositionProperty =
      DependencyProperty.Register("OpenPosition", typeof(double), typeof(StockDataPointBase),
      new FrameworkPropertyMetadata(OnOpenPositionChanged));

    private static void OnOpenPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StockDataPointBase)d).OnOpenPositionChanged();
    }

    private void OnOpenPositionChanged()
    {
    }

    #endregion // OpenPosition Property

    #region ClosePosition Property

    /// <summary>
    /// Gets or sets the physical position of the close point from the top of this data point.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ClosePositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ClosePosition
    {
      get { return (double)GetValue(ClosePositionProperty); }
      set { SetValue(ClosePositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ClosePosition"/> property.
    /// </summary>
    public static readonly DependencyProperty ClosePositionProperty =
      DependencyProperty.Register("ClosePosition", typeof(double), typeof(StockDataPointBase),
      new FrameworkPropertyMetadata(OnClosePositionChanged));

    private static void OnClosePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StockDataPointBase)d).OnClosePositionChanged();
    }

    private void OnClosePositionChanged()
    {
    }

    #endregion // ClosePosition Property

    #region LowPosition Property

    /// <summary>
    /// Gets or sets the physical position of the low value from the top of this data point. In other words, this is the height of the data point graphics.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LowPositionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double LowPosition
    {
      get { return (double)GetValue(LowPositionProperty); }
      set { SetValue(LowPositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LowPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty LowPositionProperty =
      DependencyProperty.Register("LowPosition", typeof(double), typeof(StockDataPointBase),
      new FrameworkPropertyMetadata(OnLowPositionChanged));

    private static void OnLowPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StockDataPointBase)d).OnLowPositionChanged();
    }

    private void OnLowPositionChanged()
    {
    }

    #endregion // LowPosition Property

    #region VisualStyle Property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> used for rendering the data point.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="VisualStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style VisualStyle
    {
      get { return (Style)GetValue(VisualStyleProperty); }
      set { SetValue(VisualStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="VisualStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty VisualStyleProperty =
      DependencyProperty.Register("VisualStyle", typeof(Style), typeof(StockDataPointBase),
      new FrameworkPropertyMetadata(OnVisualStyleChanged));

    private static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StockDataPointBase)d).OnVisualStyleChanged();
    }

    private void OnVisualStyleChanged()
    {
    }

    #endregion // VisualStyle Property
  }
}
