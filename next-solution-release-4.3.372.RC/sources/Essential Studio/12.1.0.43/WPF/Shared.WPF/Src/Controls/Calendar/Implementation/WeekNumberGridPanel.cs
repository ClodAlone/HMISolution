// <copyright file="WeekNumberGridPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using Calendar = System.Globalization.Calendar;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a grid that consists of columns and rows which
    /// contain WeekNumber cell elements.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class WeekNumberGridPanel : CalendarEditGrid
    {
        #region Constants

        /// <summary>
        /// Default number of rows.
        /// </summary>
        private const int DEFROWSCOUNT = 7;

        /// <summary>
        /// Default number of columns.
        /// </summary>
        private const int DEFCOLUMNSCOUNT = 8;

        #endregion Constants

        #region Static members

        /// <summary>
        /// Number of Weeks.
        /// </summary>
        public static int NumberOfWeeks;

        #endregion Static members

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="WeekNumberGridPanel"/> class.  It overrides some dependency properties.
        /// </summary>
        static WeekNumberGridPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WeekNumberGridPanel), new FrameworkPropertyMetadata(typeof(WeekNumberGridPanel)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeekNumberGridPanel"/> class.
        /// </summary>
        public WeekNumberGridPanel()
            : base(DEFROWSCOUNT, DEFCOLUMNSCOUNT)
        {
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Initializes content of WeekNumberGridPanel.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="culture">Current culture.</param>
        /// <param name="calendar">Current calendar.</param>
        public override void Initialize(VisibleDate date, CultureInfo culture, Calendar calendar)
        {
            SetWeekNumber(date, culture, calendar);
            this.SetWeekCellContent();
            this.SetIsSelected(date);
            for (int i = 0; i < CellsCollection.Count; i++)
            {
                WeekNumberCellPanel wc = (WeekNumberCellPanel)CellsCollection[i];
                if (wc.IsSelected)
                {
                    FocusedCellIndex = i + 1;
                }
            }
        }

        /// <summary>
        /// Sets the <see>
        ///     <cref>WeekNumber</cref>
        /// </see>
        ///     property of the <see cref="Syncfusion.Windows.Shared.WeekNumberCellPanel"/>.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="culture">Current culture.</param>
        /// <param name="calendar">Current calendar.</param>
        protected internal void SetWeekNumber(VisibleDate date, CultureInfo culture, Calendar calendar)
        {
            try
            {
                // Gets the DTFI properties required by GetWeekOfYear.
                Thread.CurrentThread.CurrentCulture = culture;
                int currentYear = date.VisibleYear;
                CalendarWeekRule myCWR = Thread.CurrentThread.CurrentCulture.DateTimeFormat.CalendarWeekRule;
                int numberofmonths = culture.Calendar.GetMonthsInYear(currentYear, 0);
                DateTime firstDay = new DateTime(date.VisibleYear, 1, 1);
                int numberofdaysinlastmonth = culture.Calendar.GetDaysInMonth(currentYear, numberofmonths);
                DateTime lastDay = new System.DateTime(currentYear, numberofmonths, numberofdaysinlastmonth);
                NumberOfWeeks = Thread.CurrentThread.CurrentCulture.Calendar.GetWeekOfYear(lastDay, myCWR, firstDay.DayOfWeek);
                int k = 1;
                foreach (WeekNumberCellPanel wc in CellsCollection)
                {
                    if (k <= NumberOfWeeks)
                    {
                        string sk = System.Convert.ToString(k);
                        wc.WeekNumber = sk;
                    }

                    k++;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Sets the content for the <see>
        ///     <cref>WeekNumber</cref>
        /// </see>
        ///     property of the <see cref="Syncfusion.Windows.Shared.WeekNumberCellPanel"/>.
        /// </summary>
        protected internal void SetWeekCellContent()
        {
            foreach (WeekNumberCellPanel wc in CellsCollection)
            {
                if (wc.Visibility == Visibility.Visible)
                {
                    wc.Content = wc.WeekNumber;
                }
            }
        }

        /// <summary>
        /// Sets the selected cell.
        /// </summary>
        /// <param name="date">Current date.</param>
        public override void SetIsSelected(VisibleDate date)
        {
            foreach (WeekNumberCellPanel wc in CellsCollection)
            {
                string week = CalendarEdit.clickedweeknumber;
                if (wc.WeekNumber == week)
                {
                    wc.IsSelected = true;
                }
                else
                {
                    wc.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Creates the instance of the single cell.
        /// </summary>
        /// <returns>
        /// New instance of the WeekNumbercellPanel.
        /// </returns>
        protected override Cell CreateCell()
        {
            return new WeekNumberCellPanel();
        }

        #endregion Implementation
    }
}