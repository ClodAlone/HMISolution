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
  /// Specifies how <see cref="DataPoint"/> objects can be selected within a <see cref="DataSeries"/>.
  /// </summary>
  public enum DataPointSelectionMode
  {
    /// <summary>
    /// No <see cref="DataPoint"/> objects in the <see cref="DataSeries"/> can be selected.
    /// </summary>
    None,

    /// <summary>
    /// Only a single <see cref="DataPoint"/> within the <see cref="DataSeries"/> can be selected at a time.
    /// </summary>
    Single,

    /// <summary>
    /// Multiple <see cref="DataPoint"/> objects can be selected.
    /// </summary>
    Multiple,

    /// <summary>
    /// All <see cref="DataPoint"/> objects in the <see cref="DataSeries"/> are selected at once.
    /// </summary>
    All
  }
}
