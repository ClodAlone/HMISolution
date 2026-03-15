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
  /// A base class for polar data series which plot using points, such as radar series or
  /// polar plot charts.
  /// </summary>
  public abstract class PolarPointSeriesBase : PolarSeries
  {
    #region SymbolStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> applied to the data points.
    /// This is a dependency property.
    /// </summary>
    public Style SymbolStyle
    {
      get { return (Style)GetValue(SymbolStyleProperty); }
      set { SetValue(SymbolStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SymbolStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty SymbolStyleProperty =
      DependencyProperty.Register("SymbolStyle", typeof(Style), typeof(PolarPointSeriesBase),
      new PropertyMetadata(new PropertyChangedCallback(OnSymbolStyleChanged)));

    private static void OnSymbolStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((PolarPointSeriesBase)d).OnSymbolStyleChanged();
    }

    private void OnSymbolStyleChanged()
    {
      Rebuild();
    }

    #endregion // SymbolStyle property
    
    /// <summary>
    /// Gets the <see cref="PolarChartSymbol"/> for the given data object.
    /// </summary>
    /// <param name="o">The data object that the <see cref="PolarChartSymbol"/> displays.</param>
    /// <param name="index">The index of the data.</param>
    /// <param name="point">The logical plot position of the data.</param>
    /// <returns>The <see cref="PolarChartSymbol"/> to display the given data object.</returns>
    protected PolarChartSymbol GetChartSymbol(object o, int index, out PolarPoint point)
    {
      PolarChartSymbol symbol = GetDataPoint(o, index) as PolarChartSymbol;
      if (symbol == null)
      {
        symbol = new PolarChartSymbol(o);
        point = GetPoint(symbol);
      }
      else
      {
        point = symbol.LogicalPoint;      
      }
      return symbol;
    }
  }
}
