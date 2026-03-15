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
  /// Represents a label for data points on a <see cref="Chart"/>.
  /// </summary>
  public class DataLabel : Control
  {
    static DataLabel()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataLabel),
        new FrameworkPropertyMetadata(typeof(DataLabel)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataLabel"/> class.
    /// </summary>
    /// <param name="dataPoint">The data point control holding all the plotted data information.</param>
    /// <param name="dependentData">The plotted data value along the dependent axis.</param>
    internal DataLabel(DataPoint dataPoint, object dependentData)
    {
      DataPoint = dataPoint;
      DependentData = dependentData;
    }

    #region DataPoint Property

    /// <summary>
    /// Gets the <see cref="DataPoint"/> control that the <see cref="DataLabel"/> maps to.
    /// This can be used to obtain any piece of data that the data point is made up of.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DataPointProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataPoint DataPoint
    {
      get { return (DataPoint)GetValue(DataPointProperty); }
      internal set { SetValue(DataPointPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey DataPointPropertyKey =
        DependencyProperty.RegisterReadOnly("DataPoint", typeof(DataPoint), typeof(DataLabel), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="DataPoint"/> property.
    /// </summary>
    public static readonly DependencyProperty DataPointProperty =
        DataPointPropertyKey.DependencyProperty;

    #endregion // DataPoint Property

    #region DependentData Property

    /// <summary>
    /// Gets the plotted data value along the dependent axis.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DependentDataProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object DependentData
    {
      get { return GetValue(DependentDataProperty); }
      set { SetValue(DependentDataPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey DependentDataPropertyKey =
        DependencyProperty.RegisterReadOnly("DependentData", typeof(object), typeof(DataLabel), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="DependentData"/> property.
    /// </summary>
    public static readonly DependencyProperty DependentDataProperty =
        DependentDataPropertyKey.DependencyProperty;

    #endregion // DependentData Property
  }
}
