using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A data point made up of a minimum, maximum, lower quartile, upper quartile and median value used by a <see cref="BoxplotSeries"/>.
  /// </summary>
  public class BoxplotDataPoint : ViewModelBase
  {
    private object _group;
    private double _minimum;
    private double _maximum;
    private double _lowerQuartile;
    private double _upperQuartile;
    private double _median;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoxplotDataPoint"/> class.
    /// </summary>
    /// <param name="group">The group identifier plotted along the dependent axis.</param>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <param name="lowerQuartile">The lower quartile value.</param>
    /// <param name="upperQuartile">The upper quartile value.</param>
    /// <param name="median">The median value.</param>
    public BoxplotDataPoint(object group, double minimum, double maximum, double lowerQuartile, double upperQuartile, double median)
    {
      Group = group;
      Minimum = minimum;
      Maximum = maximum;
      LowerQuartile = lowerQuartile;
      UpperQuartile = upperQuartile;
      Median = median;
    }

    /// <summary>
    /// Gets or sets a group identifier. This defines where the box is plotted along the dependent axis.
    /// </summary>
    public object Group
    {
      get { return _group; }
      set { Set<object>(ref _group, value, "Group"); }
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public double Minimum
    {
      get { return _minimum; }
      set { Set<double>(ref _minimum, value, "Minimum"); }
    }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double Maximum
    {
      get { return _maximum; }
      set { Set<double>(ref _maximum, value, "Maximum"); }
    }

    /// <summary>
    /// Gets or sets the lower quartile value.
    /// </summary>
    public double LowerQuartile
    {
      get { return _lowerQuartile; }
      set { Set<double>(ref _lowerQuartile, value, "LowerQuartile"); }
    }

    /// <summary>
    /// Gets or sets the upper quartile value.
    /// </summary>
    public double UpperQuartile
    {
      get { return _upperQuartile; }
      set { Set<double>(ref _upperQuartile, value, "UpperQuartile"); }
    }

    /// <summary>
    /// Gets or sets the median value.
    /// </summary>
    public double Median
    {
      get { return _median; }
      set { Set<double>(ref _median, value, "Median"); }
    }
  }
}
