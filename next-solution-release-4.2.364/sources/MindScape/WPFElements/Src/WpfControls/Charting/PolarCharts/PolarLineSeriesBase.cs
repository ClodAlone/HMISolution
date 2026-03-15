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
  /// Base class for polar series that can have either an open or closed loop.
  /// </summary>
  public abstract class PolarLineSeriesBase : PolarLineAreaSeriesBase
  {
    #region IsClosed property

    /// <summary>
    /// Gets or sets whether or not this <see cref="PolarLineSeriesBase"/> is a closed loop. The default is false.
    /// This is a dependency property.
    /// </summary>
    public bool IsClosed
    {
      get { return (bool)GetValue(IsClosedProperty); }
      set { SetValue(IsClosedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsClosed"/> property.
    /// </summary>
    public static readonly DependencyProperty IsClosedProperty =
      DependencyProperty.Register("IsClosed", typeof(bool), typeof(PolarLineSeriesBase),
      new PropertyMetadata(true, new PropertyChangedCallback(OnIsClosedChanged)));

    private static void OnIsClosedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarLineSeriesBase)d).OnIsClosedChanged();
    }

    private void OnIsClosedChanged()
    {
      // TODO: update chart.
    }

    #endregion // IsClosed property
  }
}
