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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Displays the title of each data series plotted on a <see cref="Chart"/>.
  /// </summary>
  public class Legend : HeaderedItemsControl
  {
    static Legend()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Legend),
        new FrameworkPropertyMetadata(typeof(Legend)));
    }

    #region LegendPosition property

    /// <summary>
    /// Gets or sets the position of the <see cref="Legend"/>.
    /// This is a dependency property.
    /// </summary>
    public LegendPosition LegendPosition
    {
      get { return (LegendPosition)GetValue(LegendPositionProperty); }
      set { SetValue(LegendPositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LegendPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty LegendPositionProperty =
      DependencyProperty.Register("LegendPosition", typeof(LegendPosition), typeof(Legend),
      new PropertyMetadata(LegendPosition.Right));

    #endregion // LegendPosition property
  }
}
