#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
namespace Syncfusion.Windows.Grid.Olap
#else
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// OlapGridUtils
    /// </summary>
    public class OlapGridUtils
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridUtils"/> class.
        /// </summary>
        public OlapGridUtils()
        {
        }

        /// <summary>
        /// Gets the default table.
        /// </summary>
        /// <returns></returns>
        public static Grid GetDefaultTable()
        {
            Grid defaultGrid = new Grid();
            defaultGrid.HorizontalAlignment = HorizontalAlignment.Left;
            defaultGrid.VerticalAlignment = VerticalAlignment.Top;
            defaultGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(75) });
            for (int i = 0; i < 2; i++)
            {
                defaultGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(75) });
            }

            for (int i = 0; i < 2; i++)
            {
                defaultGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(23) });
            }

            Border border = new Border
            {
                Width = 75,
                Height = 23,
#if !SILVERLIGHT
                Background = Brushes.LightGray
#else
                Background= new SolidColorBrush(Colors.LightGray)
#endif
            };

            Grid.SetColumn(border, 0);
            Grid.SetRow(border, 0);
            defaultGrid.Children.Add(border);
            return defaultGrid;
        }
    }
}
