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
using System.Linq;

namespace Mindscape.WpfElements.Charting
{
  internal class LegendItemCollection : ReadOnlyObservableCollection<LegendItem>
  {
    public LegendItemCollection()
      : base(new ObservableCollection<LegendItem>())
    {
    }

    public void Reload(PieChart chart)
    {
      Items.Clear();
      IList<string> visited = new List<string>();
      foreach (PieSeries series in chart.Series)
      {
        foreach (LegendItem item in series.LegendItems)
        {
          if (!visited.Contains(item.LegendLabel))
          {
            visited.Add(item.LegendLabel);
            Items.Add(item);
          }
        }
      }
    }

    public void Reload(Chart chart)
    {
      Items.Clear();
      foreach (LegendItem item in chart.Series.SelectMany(s => s.LegendItems))
      {
        Items.Add(item);
      }
    }

    public void Reload(PolarChart chart)
    {
      Items.Clear();
      foreach (LegendItem item in chart.Series.SelectMany(s => s.LegendItems))
      {
        Items.Add(item);
      }
    }
  }
}
