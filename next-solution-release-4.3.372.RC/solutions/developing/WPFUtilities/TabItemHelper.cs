using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Layout.Core;
using UFInterfaces;

namespace Utilities.WPF
{
    public static class TabItemHelper
    {
        #region Static Helper Methods
        public static void InitItemTemplate(this DXTabItem tabitem)
        {
            FrameworkElementFactory fc = new FrameworkElementFactory(typeof(TextBlock));
            System.Windows.Data.Binding b = new System.Windows.Data.Binding();
            fc.SetBinding(TextBlock.TextProperty, b);
            DataTemplate dt = new DataTemplate();
            dt.VisualTree = fc;
            tabitem.HeaderTemplate = dt;
        }
        #endregion
    }
}
