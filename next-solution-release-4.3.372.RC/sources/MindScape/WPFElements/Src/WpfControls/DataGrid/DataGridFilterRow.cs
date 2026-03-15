using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A row of filtering editors that sync up with the columns of a <see cref="DataGrid"/>.
  /// </summary>
  public class DataGridFilterRow : ItemsControl
  {
    static DataGridFilterRow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridFilterRow), new FrameworkPropertyMetadata(typeof(DataGridFilterRow)));
    }
  }
}
