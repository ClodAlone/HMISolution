#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents the Day grid.
    /// </summary>
    public class DayGrid : CalendarEditGrid
    {
        internal const int DEF_COLUMNS_COUNT = 7;
        internal const int DEF_ROWS_COUNT = 6;

        /// <summary>
        /// Selection border.
        /// </summary>
        private Border mSelectionBorder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DayGrid"/> class.
        /// </summary>
        public DayGrid()
            : base(DEF_ROWS_COUNT, DEF_COLUMNS_COUNT)
        {
            SelectionBorder = new Border();
            SelectionBorder.Opacity = 0;
            SelectionBorder.BorderBrush = new SolidColorBrush(Colors.Yellow);
            SelectionBorder.BorderThickness = new Thickness(5);
            Grid.SetRowSpan((FrameworkElement)SelectionBorder, RowsCount);
            AddToInnerGrid((UIElement)SelectionBorder);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [day grid loaded].
        /// </summary>
        /// <value><c>true</c> if [day grid loaded]; otherwise, <c>false</c>.</value>
        public bool DayGridLoaded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selection border.
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        protected internal Border SelectionBorder
        {
            get
            {
                return mSelectionBorder;
            }

            set
            {
                mSelectionBorder = value;
            }
        }

        /// <summary>
        /// Creates instance of the single cell.
        /// </summary>
        /// <returns>New instance of the cell.</returns>
        protected override Cell CreateCell()
        {
            return new DayCell();
        }

        /// <summary>
        /// Populateds the day grid.
        /// </summary>
        /// <returns></returns>
        public Grid PopulatedDayGrid()
        {
            return DayGrid;
        }
    }
}