using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  // TODO: should this be in the PropertyEditing namespace? The DataGrid doesn't use it.

  /// <summary>
  /// Contains standard property sorting strategies.
  /// </summary>
  public static class PropertySorting
  {
    /// <summary>
    /// Gets an IComparer which sorts properties by their display names.
    /// </summary>
    public static IComparer ByHumanName
    {
      get { return ByHumanNameComparer.Instance; }
    }

    private class ByHumanNameComparer : IComparer
    {
      public static readonly IComparer Instance = new ByHumanNameComparer();

      #region IComparer Members

      public int Compare(object x, object y)
      {
        PropertyGridRow row1 = x as PropertyGridRow;
        PropertyGridRow row2 = y as PropertyGridRow;

        if (row1 == null || row1.Node == null || row2 == null || row2.Node == null)
        {
          return 0;
        }

        return String.Compare(row1.Node.HumanName, row2.Node.HumanName, StringComparison.CurrentCultureIgnoreCase);
      }

      #endregion
    }
  }
}
