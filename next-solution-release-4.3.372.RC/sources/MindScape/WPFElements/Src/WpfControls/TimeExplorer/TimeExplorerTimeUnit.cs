using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Contains information about a time unit button within a <see cref="TimeExplorer"/> control.
  /// </summary>
  public class TimeExplorerTimeUnit : ViewModelBase
  {
    private double _width;
    private string _label;
    private DateTime _startDate;
    private DateTime _endDate;
    private bool _isSelected;

    /// <summary>
    /// Gets or sets the width of the time unit button.
    /// </summary>
    public double Width
    {
      get { return _width; }
      set { Set<double>(ref _width, value, "Width"); }
    }

    /// <summary>
    /// Gets or sets the label displayed by the time unit button.
    /// </summary>
    public string Label
    {
      get { return _label; }
      set { Set<string>(ref _label, value, "Label"); }
    }

    /// <summary>
    /// Gets or sets the starting <see cref="DateTime"/> of the time unit button.
    /// </summary>
    public DateTime StartDateTime
    {
      get { return _startDate; }
      set { Set<DateTime>(ref _startDate, value, "StartDateTime"); }
    }

    /// <summary>
    /// Gets or sets the ending <see cref="DateTime"/> of the time unit button.
    /// </summary>
    public DateTime EndDateTime
    {
      get { return _endDate; }
      set { Set<DateTime>(ref _endDate, value, "EndDateTime"); }
    }

    /// <summary>
    /// Gets or sets the selection state of the time unit button.
    /// </summary>
    public bool IsSelected
    {
      get { return _isSelected; }
      internal set { Set<bool>(ref _isSelected, value, "IsSelected"); }
    }
  }
}
