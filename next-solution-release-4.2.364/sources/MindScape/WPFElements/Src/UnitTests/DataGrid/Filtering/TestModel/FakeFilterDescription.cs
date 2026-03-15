using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  public class FakeFilterDescription : IFilterDescription
  {
    private IFilter _filter = new FalseFilter();

    public IFilter Filter
    {
      get { return _filter; }
    }

    public void SetFilter(IFilter filter)
    {
      _filter = filter;
      EventHandler handler = FilterChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    public event EventHandler FilterChanged;

    public void SetAs(IFilter filter)
    {
      if (filter is FalseFilter || filter is TrueFilter)
      {
        SetFilter(filter);
      }
      else
      {
        SetFilter(null);
      }
    }
  }
}
