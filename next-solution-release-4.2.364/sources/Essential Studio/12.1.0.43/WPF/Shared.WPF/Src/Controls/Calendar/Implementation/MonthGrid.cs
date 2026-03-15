// <copyright file="MonthGrid.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Globalization;
using System.Windows;
using Calendar = System.Globalization.Calendar;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a grid that consists of columns and rows which
    /// contain month cell elements.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class MonthGrid : CalendarEditGrid
    {
        #region Constants

        /// <summary>
        /// Default number of rows.
        /// </summary>
        internal const int DEFROWSCOUNT = 3;

        /// <summary>
        /// Default number of columns.
        /// </summary>
        internal const int DEFCOLUMNSCOUNT = 4;

        #endregion Constants

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="MonthGrid"/> class.  It overrides some dependency properties.
        /// </summary>
        static MonthGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthGrid), new FrameworkPropertyMetadata(typeof(MonthGrid)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthGrid"/> class.
        /// </summary>
        public MonthGrid()
            : base(DEFROWSCOUNT, DEFCOLUMNSCOUNT)
        {
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Initializes content of MonthGrid.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="culture">Current culture.</param>
        /// <param name="calendar">Current calendar.</param>
        public override void Initialize(VisibleDate date, CultureInfo culture, Calendar calendar)
        {
            this.SetMonthNumber(date, calendar);
            this.SetMonthCellContent(culture);
            this.SetIsSelected(date);

            for (int i = 0; i < CellsCollection.Count; i++)
            {
                MonthCell mc = (MonthCell)CellsCollection[i];
                if (mc.IsSelected)
                {
                    FocusedCellIndex = i;
                }
            }
        }

        /// <summary>
        /// Sets the selected cell.
        /// </summary>
        /// <param name="date">Current date.</param>
        public override void SetIsSelected(VisibleDate date)
        {
            foreach (MonthCell mc in CellsCollection)
            {
                if (mc.MonthNumber == date.VisibleMonth)
                {
                    mc.IsSelected = true;
                }
                else
                {
                    mc.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="SetMonthCellContent"/> property of the <see cref="Syncfusion.Windows.Shared.MonthCell"/>.
        /// </summary>
        /// <param name="culture">Current culture.</param>
        protected internal void SetMonthCellContent(CultureInfo culture)
        {
            DateTimeFormatInfo format = culture.DateTimeFormat;

            foreach (MonthCell mc in CellsCollection)
            {
                if (mc.Visibility == Visibility.Visible)
                {
                    if (culture.Name == "ja-JP" || culture.Name == "zh-CN")
                    {
                        mc.Content = format.MonthNames[mc.MonthNumber - 1];
                    }
                    else
                        mc.Content = format.AbbreviatedMonthNames[mc.MonthNumber - 1];
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="SetMonthNumber"/> property of the <see cref="Syncfusion.Windows.Shared.MonthCell"/>.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="calendar">Current calendar.</param>
        protected internal void SetMonthNumber(VisibleDate date, Calendar calendar)
        {
            int k = 1;
            Date curDate;
            Date minDate = new Date(calendar.MinSupportedDateTime, calendar);
            Date maxDate = new Date(calendar.MaxSupportedDateTime, calendar);

            foreach (MonthCell mc in CellsCollection)
            {
                curDate = new Date(date.VisibleYear, k, 1);

                if (curDate > maxDate || curDate < minDate)
                {
                    mc.Visibility = Visibility.Hidden;
                    mc.MonthNumber = -1;
                }
                else
                {
                    if (mc.Visibility == Visibility.Hidden)
                    {
                        mc.Visibility = Visibility.Visible;
                    }

                    mc.MonthNumber = k;
                }

                k++;
            }
        }

        /// <summary>
        /// Creates new instance of the <see cref="Syncfusion.Windows.Shared.MonthCell"/> class.
        /// </summary>
        /// <returns>New instance of the <see cref="Syncfusion.Windows.Shared.MonthCell"/> class.</returns>
        protected override Cell CreateCell()
        {
            return new MonthCell();
        }

        #endregion Implementation
    }
}