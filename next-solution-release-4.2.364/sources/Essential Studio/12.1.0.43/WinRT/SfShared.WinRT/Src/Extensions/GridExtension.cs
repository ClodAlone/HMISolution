#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.WP.Controls
#else
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
namespace Syncfusion.Tools.Controls
#else
#if WPF
using System.Windows.Controls;
using System.Windows;
namespace Syncfusion.Windows.Controls
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Controls
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a class for the Grid Extension
    /// </summary>
    public static class GridExtension
    {
        /// <summary>
        /// Gets the element at the given position.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static UIElement ElementAt(this Grid grid, int row, int column)
        {
#if WPF
            return null;
#else
            return (from d in grid.Children
                    where Grid.GetColumn(d as FrameworkElement) == column
                       && Grid.GetRow(d as FrameworkElement) == row
                    select d).FirstOrDefault();
#endif
        }
    }
}
