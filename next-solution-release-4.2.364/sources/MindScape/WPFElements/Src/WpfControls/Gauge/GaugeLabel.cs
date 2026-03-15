using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents a label on a <see cref="RadialGauge"/> control.
  /// </summary>
  public class GaugeLabel : Control
  {
    static GaugeLabel()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(GaugeLabel), new FrameworkPropertyMetadata(typeof(GaugeLabel)));
    }

    #region Label Property

    /// <summary>
    /// Gets the content of the <see cref="GaugeLabel"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LabelProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object Label
    {
      get { return GetValue(LabelProperty); }
      internal set
      {
        if (value == null || !value.Equals(Label))
        {
          SetValue(LabelPropertyKey, value);
        }
      }
    }

    private static readonly DependencyPropertyKey LabelPropertyKey =
        DependencyProperty.RegisterReadOnly("Label", typeof(object), typeof(GaugeLabel), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="Label"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelProperty =
        LabelPropertyKey.DependencyProperty;

    #endregion // Label Property

    /*
    /// <summary>
    /// Gets the <see cref="DataTemplate"/> used to display the gauge label.
    /// </summary>
    public DataTemplate LabelTemplate { get; internal set; }
    */
  }
}
