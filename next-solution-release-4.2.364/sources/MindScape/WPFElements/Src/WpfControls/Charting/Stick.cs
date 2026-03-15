using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// The graphical data point of a <see cref="StockSeries"/>.
  /// </summary>
  public class Stick : StockDataPointBase
  {
    static Stick()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Stick),
        new FrameworkPropertyMetadata(typeof(Stick)));
    }

    internal Stick(object data)
    {
      DataContext = data;
    }
  }
}
