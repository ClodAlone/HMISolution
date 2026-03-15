using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// The graphical data point of a <see cref="CandlestickSeries"/>.
  /// </summary>
  public class Candlestick : StockDataPointBase
  {
    static Candlestick()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Candlestick),
        new FrameworkPropertyMetadata(typeof(Candlestick)));
    }

    internal Candlestick(object data)
    {
      DataContext = data;
    }

    #region BoxWidth Property

    /// <summary>
    /// Gets or sets the width of the open-close box.
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
      DependencyProperty.Register("BoxWidth", typeof(double), typeof(Candlestick),
      new FrameworkPropertyMetadata(OnBoxWidthChanged));

    private static void OnBoxWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Candlestick)d).OnBoxWidthChanged();
    }

    private void OnBoxWidthChanged()
    {
    }

    #endregion // BoxWidth Property


  }
}
