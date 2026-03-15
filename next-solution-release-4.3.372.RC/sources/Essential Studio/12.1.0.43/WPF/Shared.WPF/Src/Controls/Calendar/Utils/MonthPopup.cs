// <copyright file="MonthPopup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Contains HidePopup event data.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class HidePopupEventArgs : EventArgs
    {
        #region Private members

        /// <summary>
        /// Selected date.
        /// </summary>
        private Date m_selectedDate;

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the HidePopupEventArgs class.
        /// </summary>
        /// <param name="selectedDate">Selected date.</param>
        public HidePopupEventArgs(Date selectedDate)
        {
            SelectedDate = selectedDate;
        }

        #endregion Initialization

        /// <summary>
        /// Gets or sets selected date.
        /// </summary>
        public Date SelectedDate
        {
            get
            {
                return m_selectedDate;
            }

            set
            {
                m_selectedDate = value;
            }
        }
    }

    /// <summary>
    /// Used for working with month selector.
    /// </summary>
    public class MonthPopup
    {
        #region Private members

        /// <summary>
        /// Popup instance.
        /// </summary>
        private Popup m_popup;

        /// <summary>
        /// ListBox month selector instance.
        /// </summary>
        private ListBox m_selector;

        /// <summary>
        /// Indicates whether date can be selected.
        /// </summary>
        private bool m_canSelect;

        /// <summary>
        /// Helper collection that contains ItemIndex/date pairs.
        /// </summary>
        private Hashtable m_popupDates;

        /// <summary>
        /// Timer that is responsible for popup scrolling.
        /// </summary>
        private DispatcherTimer m_timer;

        /// <summary>
        /// Identifies calendar current visible date.
        /// </summary>
        private Date m_currentDate;

        /// <summary>
        /// Identifies <see cref="System.Globalization.DateTimeFormatInfo"/>
        /// information.
        /// </summary>
        /// <remarks>
        /// Depends on calendar current culture.
        /// </remarks>
        private DateTimeFormatInfo m_format;

        /// <summary>
        /// Minimal date supported by the calendar.
        /// </summary>
        private Date m_minDate;

        /// <summary>
        /// Maximal date supported by the calendar.
        /// </summary>
        private Date m_maxDate;

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthPopup"/> class.
        /// </summary>
        /// <param name="popup">Popup instance.</param>
        /// <param name="date">Current visible date.</param>
        /// <param name="format"><see cref="System.Globalization.DateTimeFormatInfo"/> instance.</param>
        /// <param name="minDate">Minimum date.</param>
        /// <param name="maxDate">Maximum date.</param>
        public MonthPopup(Popup popup, Date date, DateTimeFormatInfo format, Date minDate, Date maxDate)
        {
            if (popup != null && format != null && popup.Child is ListBox)
            {
                m_popup = popup;
                m_selector = m_popup.Child as ListBox;
            }
            else
            {
                throw new ArgumentNullException("argument can not be null");
            }

            m_minDate = minDate;
            m_maxDate = maxDate;
            m_currentDate = date;
            m_canSelect = false;
            m_format = format;
            m_popupDates = new Hashtable();
            m_selector.KeyDown += new KeyEventHandler(Selector_KeyDown);
            m_selector.MouseLeftButtonUp += new MouseButtonEventHandler(Selector_MouseLeftButtonUp);
            m_selector.MouseMove += new System.Windows.Input.MouseEventHandler(Selector_MouseMove);
            m_selector.SelectionChanged += new SelectionChangedEventHandler(Selector_SelectionChanged);
            m_timer = new DispatcherTimer(DispatcherPriority.Input);
            m_timer.Interval = TimeSpan.FromMilliseconds(300);
            m_timer.Tick += new EventHandler(Timer_Tick);
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Shows the popup.
        /// </summary>
        public void Show()
        {
            FillMonthSelector();
            m_popup.IsOpen = true;
            m_selector.CaptureMouse();
        }

        /// <summary>
        /// Refreshes month selector items.
        /// </summary>
        public void RefreshContent()
        {
            FillMonthSelector();
        }

        /// <summary>
        /// Raises the HidePopup event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Shared.HidePopupEventArgs"/> instance that contains
        /// the event data.</param>
        private void RaiseHidePopupEvent(HidePopupEventArgs e)
        {
            if (HidePopup != null)
            {
                HidePopup(this, e);
            }
        }

        /// <summary>
        /// Fills month selector content.
        /// </summary>
        /// <remarks>
        /// Current visible date will be in the middle of the ListBox.
        /// </remarks>
        private void FillMonthSelector()
        {
            Date startDate = m_currentDate.AddMonthToDate(-3);
            Date endDate = m_currentDate.AddMonthToDate(4);

            if (startDate < m_minDate)
            {
                startDate = new Date(m_minDate.Year, m_minDate.Month, 1);
                endDate = startDate.AddMonthToDate(7);
            }

            if (endDate > m_maxDate)
            {
                endDate = new Date(m_maxDate.Year, m_maxDate.Month, 1).AddMonthToDate(1);
                startDate = endDate.AddMonthToDate(-7);
            }

            m_selector.Items.Clear();
            m_popupDates.Clear();

            while (startDate != endDate)
            {
                string name = m_format.MonthNames[startDate.Month - 1];
                m_selector.Items.Add(name + " " + startDate.Year);
                m_popupDates.Add(m_selector.Items.Count - 1, startDate);
                startDate = startDate.AddMonthToDate(1);
            }
        }

        /// <summary>
        /// Invoked whenever selection is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance that contains
        /// the event data.</param>
        private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!m_canSelect)
            {
                m_selector.UnselectAll();
            }

            m_canSelect = false;
        }

        /// <summary>
        /// Invoked when the mouse pointer moves while over the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance that contains
        /// the event data.</param>
        private void Selector_MouseMove(object sender, MouseEventArgs e)
        {
            m_selector.UnselectAll();
            Point position = e.GetPosition(m_selector);
            position.X = m_selector.ActualWidth / 2d;
            HitTestResult hitTest = VisualTreeHelper.HitTest(m_selector, position);

            if (hitTest != null)
            {
                FrameworkElement element = hitTest.VisualHit as FrameworkElement;

                if (element != null)
                {
                    ListBoxItem item = VisualUtils.FindAncestor(element, typeof(ListBoxItem)) as ListBoxItem;

                    if (item != null)
                    {
                        if (position.Y > 0 && position.Y < m_selector.ActualHeight)
                        {
                            m_timer.Stop();
                            m_canSelect = true;
                            m_selector.SelectedItem = item.Content;
                        }
                    }
                }
            }
            else
            {
                m_timer.Start();
            }
        }

        /// <summary>
        /// Invoked when the timer interval has elapsed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance that contains
        /// the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            double delta = 0;
            Point position = Mouse.GetPosition(m_selector);
            Date lastDate;
            Date nextDate;
            string monthName;
            Hashtable tmp;
            int count = m_selector.Items.Count - 1;

            if (position.Y > m_selector.ActualHeight)
            {
                delta = position.Y - m_selector.ActualHeight;
                lastDate = (Date)m_popupDates[count];
                nextDate = lastDate.AddMonthToDate(1);
                if (nextDate <= m_maxDate)
                {
                    monthName = m_format.MonthNames[nextDate.Month - 1];
                    m_selector.Items.RemoveAt(0);
                    m_selector.Items.Add(monthName + " " + nextDate.Year);
                    m_popupDates.Remove(0);
                    tmp = (Hashtable)m_popupDates.Clone();
                    m_popupDates.Clear();

                    foreach (int key in tmp.Keys)
                    {
                        m_popupDates.Add(key - 1, tmp[key]);
                    }

                    count = m_selector.Items.Count - 1;
                    m_popupDates.Add(count, nextDate);
                }
            }

            if (position.Y < 0)
            {
                delta = Math.Abs(position.Y);
                lastDate = (Date)m_popupDates[0];
                nextDate = lastDate.AddMonthToDate(-1);
                if (nextDate >= m_minDate)
                {
                    monthName = m_format.MonthNames[nextDate.Month - 1];
                    m_selector.Items.RemoveAt(count);
                    m_selector.Items.Insert(0, monthName + " " + nextDate.Year);
                    m_popupDates.Remove(count);
                    tmp = (Hashtable)m_popupDates.Clone();
                    m_popupDates.Clear();

                    foreach (int key in tmp.Keys)
                    {
                        m_popupDates.Add(key + 1, tmp[key]);
                    }

                    count = m_selector.Items.Count - 1;
                    m_popupDates.Add(0, nextDate);
                }
            }

            ////Scrolling speed setings
            if (delta <= 10)
            {
                m_timer.Interval = TimeSpan.FromMilliseconds(350);
            }

            if (delta > 10 && delta <= 30)
            {
                m_timer.Interval = TimeSpan.FromMilliseconds(300);
            }

            if (delta > 30 && delta <= 50)
            {
                m_timer.Interval = TimeSpan.FromMilliseconds(200);
            }

            if (delta > 50 && delta <= 70)
            {
                m_timer.Interval = TimeSpan.FromMilliseconds(150);
            }

            if (delta > 70 && delta <= 100)
            {
                m_timer.Interval = TimeSpan.FromMilliseconds(100);
            }

            if (delta > 100)
            {
                m_timer.Interval = TimeSpan.FromMilliseconds(30);
            }
        }

        /// <summary>
        /// Occurs when the left mouse button is released while the mouse pointer is
        /// over the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance that contains
        /// the event data.</param>
        private void Selector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_timer.Stop();
            m_popup.IsOpen = false;
            if (m_selector.SelectedItem != null)
            {
                int selectedIndex = (sender as ListBox).SelectedIndex;
                Date selectedDate = (Date)m_popupDates[selectedIndex];
                RaiseHidePopupEvent(new HidePopupEventArgs(selectedDate));
            }
        }

        /// <summary>
        /// Occurs when a key is pressed while focus is on this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance that contains
        /// the event data.</param>
        private void Selector_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                m_popup.IsOpen = false;
            }
        }

        #endregion Implementation

        #region Properties

        /// <summary>
        /// Gets or sets date time format.
        /// </summary>
        public DateTimeFormatInfo Format
        {
            get
            {
                return m_format;
            }

            set
            {
                m_format = value;
            }
        }

        /// <summary>
        /// Gets or sets the current date.
        /// </summary>
        public Date CurrentDate
        {
            get
            {
                return m_currentDate;
            }

            set
            {
                m_currentDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimal date.
        /// </summary>
        /// <value>
        /// Type: <see cref="Date"/>
        /// </value>
        /// <seealso cref="Date"/>
        public Date MinDate
        {
            get
            {
                return m_minDate;
            }

            set
            {
                m_minDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximal.
        /// </summary>
        /// <value>
        /// Type: <see cref="Date"/>
        /// </value>
        /// <seealso cref="Date"/>
        public Date MaxDate
        {
            get
            {
                return m_maxDate;
            }

            set
            {
                m_maxDate = value;
            }
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// Occurs when the popup hides.
        /// </summary>
        public event EventHandler<HidePopupEventArgs> HidePopup;

        #endregion Events
    }
}