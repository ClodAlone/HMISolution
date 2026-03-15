using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Holds information for a pager button displayed by a <see cref="DataGridPager"/>.
  /// </summary>
  public class PagerButtonModel : ViewModelBase
  {
    private object _content;
    private bool _isSelected;

    /// <summary>
    /// Gets the content of the pager button.
    /// </summary>
    public object Content
    {
      get { return _content; }
      internal set { Set(ref _content, value, "Content"); }
    }

    internal int PageIndex { get; set; }

    /// <summary>
    /// Gets or sets whether or not the pager button is selected.
    /// </summary>
    public bool IsSelected
    {
      get { return _isSelected; }
      set { Set(ref _isSelected, value, "IsSelected"); }
    }
  }
}
