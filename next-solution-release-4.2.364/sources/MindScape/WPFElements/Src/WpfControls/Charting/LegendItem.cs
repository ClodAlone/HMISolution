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
using System.Collections.Generic;
using System.ComponentModel;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents an item to be displayed in a legend.
  /// </summary>
  public class LegendItem : ViewModelBase
  {
    private DataTemplate _iconTemplate;
    private string _legendLabel;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegendItem"/> class.
    /// </summary>
    /// <param name="label">The text to be displayed.</param>
    /// <param name="iconTemplate">The legend icon template.</param>
    /// <param name="iconBrush">The <see cref="Brush"/> used to color the legend icon.</param>
    internal LegendItem(string label, DataTemplate iconTemplate, Brush iconBrush)
    {
      LegendLabel = label;
      LegendIconTemplate = iconTemplate;
      LegendIconBrush = iconBrush;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LegendItem"/> class.
    /// </summary>
    /// <param name="label">The text to be displayed.</param>
    /// <param name="iconTemplate">The legend icon template.</param>
    /// <param name="iconBrush">The <see cref="Brush"/> used to color the legend icon.</param>
    /// <param name="dataSeries">The <see cref="DataSeries"/> that created the <see cref="LegendItem"/>.</param>
    internal LegendItem(string label, DataTemplate iconTemplate, Brush iconBrush, DataSeries dataSeries)
      : this(label, iconTemplate, iconBrush)
    {
      DataSeries = dataSeries;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LegendItem"/> class.
    /// </summary>
    /// <param name="label">The text to be displayed.</param>
    /// <param name="iconTemplate">The legend icon template.</param>
    /// <param name="iconBrush">The <see cref="Brush"/> used to color the legend icon.</param>
    /// <param name="polarSeries">The <see cref="PolarSeries"/> that created the <see cref="LegendItem"/>.</param>
    internal LegendItem(string label, DataTemplate iconTemplate, Brush iconBrush, PolarSeries polarSeries)
      : this(label, iconTemplate, iconBrush)
    {
      PolarSeries = polarSeries;
    }

    /// <summary>
    /// Returns a string representation of this <see cref="LegendItem"/>.
    /// </summary>
    /// <returns>A string representation of this <see cref="LegendItem"/>.</returns>
    public override string ToString()
    {
      return LegendLabel;
    }

    /// <summary>
    /// Gets the text to be displayed by the <see cref="LegendItem"/>.
    /// </summary>
    public string LegendLabel
    {
      get { return _legendLabel; }
      internal set
      {
        Set<string>(ref _legendLabel, value, "LegendLabel");
      }
    }

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> used to display the legend icon.
    /// </summary>
    public DataTemplate LegendIconTemplate
    {
      get { return _iconTemplate; }
      internal set
      {
        Set<DataTemplate>(ref _iconTemplate, value, "LegendIconTemplate");
      }
    }

    /// <summary>
    /// Gets the <see cref="Brush"/> used to color the legend icon.
    /// </summary>
    public Brush LegendIconBrush { get; private set; }

    /// <summary>
    /// Gets the <see cref="DataSeries"/> that created the <see cref="LegendItem"/>.
    /// </summary>
    public DataSeries DataSeries { get; private set; }

    /// <summary>
    /// Gets the <see cref="PolarSeries"/> that created the <see cref="LegendItem"/> if applicable.
    /// </summary>
    public PolarSeries PolarSeries { get; private set; }

    internal static readonly IEqualityComparer<LegendItem> ByLabelComparer = new ByLabelEqualityComparer();

    private class ByLabelEqualityComparer : IEqualityComparer<LegendItem>
    {
      public bool Equals(LegendItem x, LegendItem y)
      {
        return x.LegendLabel == y.LegendLabel;
      }

      public int GetHashCode(LegendItem obj)
      {
        return (obj.LegendLabel == null ? 0 : obj.LegendLabel.GetHashCode());
      }
    }
  }
}
