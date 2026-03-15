using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// 
  /// </summary>
  public class GroupHeaderCellCollectionConverter : IMultiValueConverter
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      ObservableCollection<DataGridColumn> columns = (ObservableCollection<DataGridColumn>)values[0];
      DataGridGroup group = (DataGridGroup)values[1];
      List<GroupHeaderCell> groupHeaderCells = new List<GroupHeaderCell>();
      foreach (DataGridColumn column in columns)
      {
        groupHeaderCells.Add(new GroupHeaderCell(group, column, null));
      }
      return groupHeaderCells;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetTypes"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
