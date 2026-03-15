using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  public class TrueFilter : IFilter
  {
    public bool IsMatch(object o)
    {
      return true;
    }
  }
}
