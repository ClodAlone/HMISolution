// <copyright file="YearRangeGrid.cs" company="Syncfusion">
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
    /// contain year range cell elements.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class YearRangeGrid : CalendarEditGrid
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
        /// Initializes static members of the <see cref="YearRangeGrid"/> class.  It overrides some dependency properties.
        /// </summary>
        static YearRangeGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(YearRangeGrid), new FrameworkPropertyMetadata(typeof(YearRangeGrid)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YearRangeGrid"/> class.
        /// </summary>
        public YearRangeGrid()
            : base(DEFROWSCOUNT, DEFCOLUMNSCOUNT)
        {
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Initializes content of YearRangeGrid.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="culture">Current culture.</param>
        /// <param name="calendar">Current calendar.</param>
        public override void Initialize(VisibleDate date, CultureInfo culture, Calendar calendar)
        {
            this.SetYearRange(date, calendar);
            this.SetYearRangeCellContent();
            this.SetIsSelected(date);
            this.SetIsBelongToCurrentRange();

            for (int i = 0; i < CellsCollection.Count; i++)
            {
                YearRangeCell yrc = (YearRangeCell)CellsCollection[i];
                if (yrc.IsSelected)
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
            bool set = false;

            foreach (YearRangeCell yrc in CellsCollection)
            {
                if (currentYear >= yrc.Years.StartYear && currentYear <= yrc.Years.EndYear)
                {
                    yrc.IsSelected = true;
                    set = true;
                }
                else
                {
                    yrc.IsSelected = false;
                }
            }

            if (!set)
            {
                (CellsCollection[CellsCollection.Count - 1] as Cell).IsSelected = true;
            }
        }

        /// <summary>
        /// Sets the content of the year range cell.
        /// </summary>
        protected internal void SetYearRangeCellContent()
        {
            YearsRange years;
            int startYear, endYear;

            foreach (YearRangeCell yrc in CellsCollection)
            {
                if (yrc.Visibility == Visibility.Visible)
                {
                    years = yrc.Years;
                    startYear = years.StartYear;
                    endYear = years.EndYear;
                    yrc.Content = startYear.ToString() + "-" + "\n" + endYear.ToString();
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="SetYearRange"/> property of the <see cref="Syncfusion.Windows.Shared.YearRangeCell"/>.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="calendar">Current calendar.</param>
        protected internal void SetYearRange(VisibleDate date, Calendar calendar)
        {
            int startYear = date.VisibleYear;
            Date startDate, endDate;
            Date minDate = new Date(calendar.MinSupportedDateTime, calendar);
            Date maxDate = new Date(calendar.MaxSupportedDateTime, calendar);

            while (startYear % 10 != 0)
            {
                startYear--;
            }

            while (startYear % 100 != 0)
            {
                startYear -= 10;
            }

            startYear -= 10;

            foreach (YearRangeCell yrc in CellsCollection)
            {
                if (startYear == 0)
                {
                    startDate = new Date(1, date.VisibleMonth, 1);
                }
                else
                {
                    startDate = new Date(startYear, date.VisibleMonth, 1);
                }

                endDate = new Date(startYear + 9, date.VisibleMonth, 1);
                if (startDate > maxDate || startDate < minDate || endDate > maxDate || endDate < minDate)
                {
                    yrc.Visibility = Visibility.Hidden;
                    yrc.Years = new YearsRange(-1, -1);
                }
                else
                {
                    if (yrc.Visibility == Visibility.Hidden)
                    {
                        yrc.Visibility = Visibility.Visible;
                    }

                    yrc.Years = new YearsRange(startDate.Year, endDate.Year);
                }

                startYear += 10;
            }
        }

        /// <summary>
        /// Sets the <see cref="SetIsBelongToCurrentRange"/> property of the <see cref="Syncfusion.Windows.Shared.YearRangeCell"/>.
        /// </summary>
        protected internal void SetIsBelongToCurrentRange()
        {
            for (int i = 0; i < CellsCollection.Count; i++)
            {
                YearRangeCell yrc = (YearRangeCell)CellsCollection[i];
                if (i == 0 || i == CellsCollection.Count - 1)
                {
                    yrc.IsBelongToCurrentRange = false;
                }
                else
                {
                    yrc.IsBelongToCurrentRange = true;
                }
            }
        }

        /// <summary>
        /// Creates new instance of the <see cref="Syncfusion.Windows.Shared.YearRangeCell"/> class.
        /// </summary>
        /// <returns>New instance of the <see cref="Syncfusion.Windows.Shared.YearRangeCell"/> class.</returns>
        protected override Cell CreateCell()
        {
            return new YearRangeCell();
        }

        #endregion Implementation
    }
}