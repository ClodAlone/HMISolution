using DocumentManager.ComponentService;
using System;
using System.Windows.Data;
using UFProjectManager.ComponentService;
using WPFUtilities;

namespace UFProjectManager.Converters
{
    public class TagToContextMenuConverter : IMultiValueConverter
    {
        #region IValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 2)
                return null;
            
            if (values[0] is FolderTreeControl)
                return ((FolderTreeControl)values[0]).TypeContextMenu;
            else if (values[0] is ResourceTreeControl)
                return ((ResourceTreeControl)values[0]).TypeContextMenu;
            else if (values[0] is ChildProjectControl)
                return ((ChildProjectControl)values[0]).TypeContextMenu;
            else if (values[1] is IDocumentManager)
                return ((IDocumentManager)values[1]).TypeContextMenu;

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
