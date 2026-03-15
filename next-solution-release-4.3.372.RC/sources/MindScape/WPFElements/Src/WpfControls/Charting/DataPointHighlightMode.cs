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
  /// Sepecifies how a <see cref="DataPoint"/> can be highlighted within its <see cref="DataSeries"/>.
  /// </summary>
  public enum DataPointHighlightMode
  {
    /// <summary>
    /// <see cref="DataPoint"/> objects in the <see cref="DataSeries"/> can not be highlighted.
    /// </summary>
    None,

    /// <summary>
    /// <see cref="DataPoint"/> objects within the <see cref="DataSeries"/> are highlighted when the mouse is over them.
    /// </summary>
    MouseOver,

    /// <summary>
    /// <see cref="DataPoint"/> objects within the <see cref="DataSeries"/> are highlighted when the mouse is close to
    /// their respective X values.
    /// </summary>
    ClosestXValue
  }
}
