using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents a single data point to be plotted on cartesian (X/Y) axes.
  /// </summary>
  public class CartesianDataPoint : DataPoint
  {
    internal CartesianDataPoint()
    {
    }

    #region XObject property

    /// <summary>
    /// Gets or sets the object that specifies the X value of the <see cref="CartesianDataPoint"/>.
    /// This is a dependency property.
    /// </summary>
    public object XObject
    {
      get { return GetValue(XObjectProperty); }
      set { SetValue(XObjectProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="XObject"/> property.
    /// </summary>
    public static readonly DependencyProperty XObjectProperty =
      DependencyProperty.Register("XObject", typeof(object), typeof(CartesianDataPoint),
      new PropertyMetadata(new PropertyChangedCallback(OnXObjectChanged)));

    private static void OnXObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CartesianDataPoint)d).OnXObjectChanged();
    }

    // I have made this internal for now, just in case we want to change the type of event handler to include other info such as the old value.
    internal event EventHandler XObjectChanged;

    private void OnXObjectChanged()
    {
      EventHandler handler = XObjectChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    #endregion // XObject property

    #region YObject property

    /// <summary>
    /// Gets or sets the object that specifies the Y value of the <see cref="CartesianDataPoint"/>.
    /// This is a dependency property.
    /// </summary>
    public object YObject
    {
      get { return GetValue(YObjectProperty); }
      set { SetValue(YObjectProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="YObject"/> property.
    /// </summary>
    public static readonly DependencyProperty YObjectProperty =
      DependencyProperty.Register("YObject", typeof(object), typeof(CartesianDataPoint),
      new PropertyMetadata(new PropertyChangedCallback(OnYObjectChanged)));

    private static void OnYObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CartesianDataPoint)d).OnYObjectChanged();
    }

    // I have made this internal for now, just in case we want to change the type of event handler to include other info such as the old value.
    internal event EventHandler YObjectChanged;

    private void OnYObjectChanged()
    {
      EventHandler handler = YObjectChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    #endregion // YObject property

    #region DataSeries Property

    /// <summary>
    /// Gets the <see cref="DataSeries"/> that created this <see cref="CartesianDataPoint"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DataSeriesProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataSeries DataSeries
    {
      get { return (DataSeries)GetValue(DataSeriesProperty); }
      internal set { SetValue(DataSeriesPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey DataSeriesPropertyKey =
        DependencyProperty.RegisterReadOnly("DataSeries", typeof(DataSeries), typeof(CartesianDataPoint), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="DataSeries"/> property.
    /// </summary>
    public static readonly DependencyProperty DataSeriesProperty =
        DataSeriesPropertyKey.DependencyProperty;

    #endregion // DataSeries Property

    /// <summary>
    /// Gets the logical X,Y plotting position of the <see cref="CartesianDataPoint"/>.
    /// </summary>
    public Point LogicalPoint { get; internal set; }
  }
}
