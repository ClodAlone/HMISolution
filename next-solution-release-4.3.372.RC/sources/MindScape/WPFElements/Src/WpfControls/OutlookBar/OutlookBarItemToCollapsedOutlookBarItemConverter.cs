//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Data;
//using System.Windows.Controls;

//namespace Mindscape.WpfElements
//{
//  /// <summary>
//  /// Creates a list of <see cref="CollapsedOutlookBarItem"/> from a collection of <see cref="OutlookBarItem"/>.
//  /// </summary>
//  public class OutlookBarItemToCollapsedOutlookBarItemConverter : IValueConverter
//  {
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="value"></param>
//    /// <param name="targetType"></param>
//    /// <param name="parameter"></param>
//    /// <param name="culture"></param>
//    /// <returns></returns>
//    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//    {
//      ItemCollection items = (ItemCollection)value;
      
//      List<CollapsedOutlookBarItem> result = new List<CollapsedOutlookBarItem>();

//      foreach (OutlookBarItem item in items)
//      {
//        result.Add(new CollapsedOutlookBarItem(item));
//      }

//      return result;
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="value"></param>
//    /// <param name="targetType"></param>
//    /// <param name="parameter"></param>
//    /// <param name="culture"></param>
//    /// <returns></returns>
//    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//    {
//      throw new NotImplementedException();
//    }
//  }
//}
