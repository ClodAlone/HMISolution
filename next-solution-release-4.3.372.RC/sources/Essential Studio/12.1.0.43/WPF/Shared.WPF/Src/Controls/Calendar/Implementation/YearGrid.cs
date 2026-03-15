// <copyright file="YearGrid.cs" company="Syncfusion">
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
    /// contain year cell elements.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class YearGrid : CalendarEditGrid
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
        /// Initializes static members of the <see cref="YearGrid"/> class.  It overrides some dependency properties.
        /// </summary>
        static YearGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(YearGrid), new FrameworkPropertyMetadata(typeof(YearGrid)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YearGrid"/> class.
        /// </summary>
        public YearGrid()
            : base(DEFROWSCOUNT, DEFCOLUMNSCOUNT)
        {
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Initializes content of YearGrid.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="culture">Current culture.</param>
        /// <param name="calendar">Current calendar.</param>
        public override void Initialize(VisibleDate date, CultureInfo culture, Calendar calendar)
        {
            this.SetYear(date, calendar);
            this.SetYearCellContent();
            this.SetIsSelected(date);
            this.SetIsBelongToCurrentRange();

            for (int i = 0; i < CellsCollection.Count; i++)
            {
                YearCell yc = (YearCell)CellsCollection[i];
                if (yc.IsSelected)
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
            int currentYear = date.VisibleYear;

            foreach (YearCell yc in CellsCollection)
            {
                if (yc.Year == currentYear)
                {
                    yc.IsSelected = true;
                }
                else
                {
                    yc.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="SetIsBelongToCurrentRange"/> property of the <see cref="Syncfusion.Windows.Shared.YearCell"/>.
        /// </summary>
        protected internal void SetIsBelongToCurrentRange()
        {
            for (int i = 0; i < CellsCollection.Count; i++)
            {
                YearCell yc = (YearCell)CellsCollection[i];
                if (i == 0 || i == CellsCollection.Count - 1)
                {
                    yc.IsBelongToCurrentRange = false;
                }
                else
                {
                    yc.IsBelongToCurrentRange = true;
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="SetYearCellContent"/> property of the <see cref="Syncfusion.Windows.Shared.YearCell"/>.
        /// </summary>
        protected internal void SetYearCellContent()
        {
            foreach (YearCell yc in CellsCollection)
            {
                if (yc.Visibility == Visibility.Visible)
                {
                    yc.Content = yc.Year;
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="SetYear"/> property of the <see cref="Syncfusion.Windows.Shared.YearCell"/>.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="calendar">Current calendar.</param>
        protected internal void SetYear(VisibleDate date, Calendar calendar)
        {
            int startYear = date.VisibleYear;
            Date curDate;
            Date minDate = new Date(calendar.MinSupportedDateTime, calendar);
            Date maxDate = new Date(calendar.MaxSupportedDateTime, calendar);

            while (startYear % 10 != 0)
            {
                startYear--;
            }

            if (startYear == maxDate.Year)
            {
                startYear -= 10;
            }

            startYear--;

            foreach (YearCell yc in CellsCollection)
            {
                curDate = new Date(startYear, date.VisibleMonth, 1);

                if (curDate > maxDate || curDate < minDate)
                {
                    yc.Visibility = Visibility.Hidden;
                    yc.Year = -1;
                }
                else
                {
                    if (yc.Visibility == Visibility.Hidden)
                    {
                        yc.Visibility = Visibility.Visible;
                    }

                    yc.Year = startYear;
                }

                startYear++;
            }
        }

        /// <summary>
        /// Creates new instance of the <see cref="Syncfusion.Windows.Shared.YearCell"/> class.
        /// </summary>
        /// <returns>New instance of the <see cref="Syncfusion.Windows.Shared.YearCell"/> class.</returns>
        protected override Cell CreateCell()
        {
            return new YearCell();
        }

        #endregion Implementation
    }
}