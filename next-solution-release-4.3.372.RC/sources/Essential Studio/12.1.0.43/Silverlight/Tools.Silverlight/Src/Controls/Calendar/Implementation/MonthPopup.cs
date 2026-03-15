#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Contains HidePopup event data.
    /// </summary>
    public class HidePopupEventArgs : EventArgs
    {
        /// <summary>
        /// Selected date.
        /// </summary>
        private Date mSelectedDate;

        /// <summary>
        /// Initializes a new instance of the HidePopupEventArgs class.
        /// </summary>
        /// <param name="selectedDate">Selected date.</param>
        public HidePopupEventArgs(Date selectedDate)
        {
            SelectedDate = selectedDate;
        }

        /// <summary>
        /// Gets or sets selected date.
        /// </summary>
        /// <value>
        /// Type: <see cref="Date"/>
        /// </value>
        /// <seealso cref="Date"/>
        public Date SelectedDate
        {
            get
            {
                return mSelectedDate;
            }

            set
            {
                mSelectedDate = value;
            }
        }
    }

    /// <summary>
    /// Used for working with month selector.
    /// </summary>
    public class MonthPopup
    {
        /// <summary>
        /// Timer that is responsible forDouble Click.
        /// </summary>
        private DispatcherTimer _timer;

        private string flag = string.Empty;

        /// <summary>
        /// Identifies calendar current visible date.
        /// </summary>
        private Date mCurrentDate;

        /// <summary>
        /// Identifies <see cref="System.Globalization.DateTimeFormatInfo"/>
        /// information.
        /// </summary>
        /// <remarks>
        /// Depends on calendar current culture.
        /// </remarks>
        protected internal DateTimeFormatInfo m_Format;

        /// <summary>
        /// Maximal date supported by the calendar.
        /// </summary>
        protected internal Date m_MaxDate;

        /// <summary>
        /// Minimal date supported by the calendar.
        /// </summary>
        protected internal Date mMinDate;

        /// <summary>
        /// Popup instance.
        /// </summary>
        private Popup mPopup;

        /// <summary>
        /// Helper collection that contains ItemIndex/date pairs.
        /// </summary>
        ////private Hashtable m_PopupDates;
        private List<Date> mPopupDates;

        /// <summary>
        /// ListBox month selector instance.
        /// </summary>
        private ListBox mSelector = new ListBox();

        /// <summary>
        /// Timer that is responsible for popup scrolling.
        /// </summary>
        private DispatcherTimer mtimer;
        /// <summary>
        /// 
        /// </summary>
        protected internal bool Monthabbreviated;

        Polygon pl1;
        Polygon pl2;

        internal Brush ScrollButtonFill = new SolidColorBrush(Color.FromArgb(0xFF, 0x24, 0x3A, 0x76));
        private StackPanel sp;

        /// <summary>
        /// Initializes new instance of the MonthPopup class.
        /// </summary>
        /// <param name="popup">Popup Instance.</param>
        /// <param name="date">Current visible date.</param>
        /// <param name="format">Calendar Control.</param>
        /// <param name="minDate">Get minimum date.</param>
        /// <param name="maxDate">Get maximum date.</param>
        public MonthPopup(Popup popup, Date date, DateTimeFormatInfo format, Date minDate, Date maxDate)
        {
            mPopup = new Popup();
            sp = new StackPanel();
            pl1 = new Polygon();
            Border b1 = new Border();
            b1.Background = new SolidColorBrush(Colors.Transparent);
            pl1.StrokeThickness = 1;
            pl1.Fill = ScrollButtonFill;
            pl1.VerticalAlignment = VerticalAlignment.Center;
            pl1.HorizontalAlignment = HorizontalAlignment.Center;
            PointCollection _pc = new PointCollection();
            _pc.Add(new Point(-1, 8));
            _pc.Add(new Point(6, 2));
            _pc.Add(new Point(13, 8));
            _pc.Add(new Point(-1, 8));
            pl1.Points = _pc;
            b1.Child = pl1;
            sp.Children.Add(b1);
            sp.Children.Add(mSelector);
            pl2 = new Polygon();
            pl2.Fill = ScrollButtonFill;
            pl2.VerticalAlignment = VerticalAlignment.Bottom;
            pl2.HorizontalAlignment = HorizontalAlignment.Center;
            _pc = new PointCollection();
            _pc.Add(new Point(9, 9));
            _pc.Add(new Point(16, 3));
            _pc.Add(new Point(3, 3));
            _pc.Add(new Point(9, 9));
            pl2.Points = _pc;
            Border b2 = new Border();
            b2.Background = new SolidColorBrush(Colors.Transparent);
            b2.Child = pl2;

            sp.Children.Add(b2);

            Border m_b1 = new Border();
            mSelector.Background = new SolidColorBrush(Colors.Transparent);
            mSelector.BorderBrush = new SolidColorBrush(Colors.Transparent);
            m_b1.BorderThickness = new Thickness(1);
            m_b1.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x24, 0x3a, 0x76));
            m_b1.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xeb, 0xf3, 0xfd));
            m_b1.Child = sp;
            mPopup.Child = m_b1;
            mMinDate = minDate;
            m_MaxDate = maxDate;
            mCurrentDate = date;

            m_Format = format;
            mSelector.Focus();
            b1.MouseEnter += new MouseEventHandler(pl1_MouseEnter);
            b2.MouseEnter += new MouseEventHandler(pl2_MouseEnter);
            b1.MouseLeave += new MouseEventHandler(pl1_MouseLeave);
            b2.MouseLeave += new MouseEventHandler(pl1_MouseLeave);
            sp.MouseLeave += new MouseEventHandler(m_Popup_MouseLeave);
            mPopupDates = new List<Date>();
            mSelector.KeyDown += new KeyEventHandler(m_Selector_KeyDown);
            mSelector.MouseLeftButtonUp += new MouseButtonEventHandler(m_Selector_MouseLeftButtonUp);
            // m_Selector.MouseMove += new System.Windows.Input.MouseEventHandler(m_Selector_MouseMove);
            ////m_Selector.MouseLeave += new MouseEventHandler(m_Selector_MouseLeave);
            mSelector.SelectionChanged += new SelectionChangedEventHandler(m_Selector_SelectionChanged);
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            _timer.Tick += new EventHandler(_timer_Tick);
            mtimer = new DispatcherTimer();
            mtimer.Interval = TimeSpan.FromMilliseconds(300);
            mtimer.Tick += new EventHandler(m_timer_Tick);
            mSelector.Width = 110;
        }

        /// <summary>
        /// Occurs when the popup hides.
        /// </summary>
        public event EventHandler<HidePopupEventArgs> HidePopup;

        /// <summary>
        /// Gets or sets the current date.
        /// </summary>
        /// <value>
        /// Type: <see cref="Date"/>
        /// </value>
        /// <seealso cref="Date"/>
        public Date CurrentDate
        {
            get
            {
                return mCurrentDate;
            }

            set
            {
                mCurrentDate = value;
            }
        }

        /// <summary>
        /// Gets or sets date time format.
        /// </summary>
        /// <value>
        /// Type: <see cref="DateTimeFormatInfo"/>
        /// </value>
        /// <seealso cref="DateTimeFormatInfo"/>
        public DateTimeFormatInfo Format
        {
            get
            {
                return m_Format;
            }

            set
            {
                m_Format = value;
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
                return m_MaxDate;
            }

            set
            {
                m_MaxDate = value;
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
                return mMinDate;
            }

            set
            {
                mMinDate = value;
            }
        }

        /// <summary>
        /// Gets the pop up.
        /// </summary>
        /// <value>The pop up.</value>
        public Popup PopUp
        {
            get
            {
                return mPopup;
            }
        }

        /// <summary>
        /// Handles the Tick event of the _timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
        }

        /// <summary>
        /// Addands the remove month.
        /// </summary>
        private void AddandRemoveMonth()
        {
            Date lastDate;
            Date nextDate;
            string monthName;
            List<Date> tmp;
            int count = mSelector.Items.Count - 1;
            if (flag == "DownArrow")
            {
                lastDate = (Date)mPopupDates[count];
                nextDate = lastDate.AddMonthToDate(1);
                if (nextDate <= m_MaxDate)
                {
                    monthName = m_Format.MonthNames[nextDate.Month - 1];
                    if (!Monthabbreviated)
                    {
                        monthName = m_Format.MonthNames[nextDate.Month - 1];
                    }
                    else
                    {
                        monthName = m_Format.AbbreviatedMonthNames[nextDate.Month - 1];
                    }

                    mSelector.Items.RemoveAt(0);
                    ListBoxItem item = new ListBoxItem();
                    item.KeyDown += new KeyEventHandler(li_KeyDown);
                    item.MouseEnter += new MouseEventHandler(li_MouseEnter);
                    item.Content = monthName + " " + nextDate.Year;
                    mSelector.Items.Add(item);
                    tmp = mPopupDates;
                    mPopupDates.RemoveAt(0);

                    count = mSelector.Items.Count - 1;
                    mPopupDates.Add(nextDate);
                }
            }
            else if (flag == "UpArrow")
            {
                lastDate = (Date)mPopupDates[0];
                nextDate = lastDate.AddMonthToDate(-1);
                if (nextDate >= mMinDate)
                {
                    monthName = m_Format.MonthNames[nextDate.Month - 1];
                    if (!Monthabbreviated)
                    {
                        monthName = m_Format.MonthNames[nextDate.Month - 1];
                    }
                    else
                    {
                        monthName = m_Format.AbbreviatedMonthNames[nextDate.Month - 1];
                    }

                    mSelector.Items.RemoveAt(count);
                    ListBoxItem item = new ListBoxItem();
                    item.KeyDown += new KeyEventHandler(li_KeyDown);
                    item.MouseEnter += new MouseEventHandler(li_MouseEnter);
                    item.Content = monthName + " " + nextDate.Year;
                    mSelector.Items.Insert(0, item);
                    mPopupDates.RemoveAt(count);

                    count = mSelector.Items.Count - 1;
                    mPopupDates.Insert(0, nextDate);
                }
            }
        }

        /// <summary>
        /// Displays the selected month year.
        /// </summary>
        /// <param name="selectedIndex">Index of the selected.</param>
        private void DisplaySelectedMonthYear(int selectedIndex)
        {
            if (mSelector.SelectedItem != null)
            {
                Date selectedDate = (Date)mPopupDates[selectedIndex];
                RaiseHidePopupEvent(new HidePopupEventArgs(selectedDate));
            }
        }

        /// <summary>
        /// Fills month selector content.
        /// </summary>
        /// <remarks>
        /// Current visible date will be in the middle of the ListBox.
        /// </remarks>
        private void FillMonthSelector(Date visibleDate, bool isMonthAbbreviated, CultureInfo cultureInfo)
        {
            mCurrentDate = visibleDate;
            Date startDate = mCurrentDate.AddMonthToDate(-3);
            Date endDate = mCurrentDate.AddMonthToDate(4);

            if (startDate < mMinDate)
            {
                startDate = new Date(mMinDate.Year, mMinDate.Month, 1);
                endDate = startDate.AddMonthToDate(7);
            }

            if (endDate > m_MaxDate)
            {
                endDate = new Date(m_MaxDate.Year, m_MaxDate.Month, 1).AddMonthToDate(1);
                startDate = endDate.AddMonthToDate(-7);
            }

            mSelector.Items.Clear();
            mPopupDates.Clear();
            int i = 0;
            while (startDate != endDate)
            {
                DateTimeFormatInfo format = cultureInfo.DateTimeFormat;
                string name = format.MonthNames[startDate.Month - 1];

                if (!isMonthAbbreviated)
                {
                    name = format.MonthNames[startDate.Month - 1];
                }
                else
                {
                    name = format.AbbreviatedMonthNames[startDate.Month - 1];
                }

                ListBoxItem li = new ListBoxItem();
                li.KeyDown += new KeyEventHandler(li_KeyDown);
                li.MouseEnter += new MouseEventHandler(li_MouseEnter);
                li.Content = name + " " + startDate.Year;
                if ((startDate.Month + startDate.Year).ToString() == (visibleDate.Month + visibleDate.Year).ToString())
                {
                    li.IsSelected = true;
                    mSelector.Focus();
                }

                mSelector.Items.Add(li);
                ////if (i == 0)
                ////    m_Selector.SelectedItem = (name + " " + startDate.Year).ToString();
                mPopupDates.Add(startDate);
                startDate = startDate.AddMonthToDate(1);
                i++;
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the li control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        void li_KeyDown(object sender, KeyEventArgs e)
        {
            int selectedIndex = (((ListBoxItem)sender).Parent as ListBox).SelectedIndex;
            if (e.Key == Key.Escape)
            {
                mPopup.IsOpen = false;
            }
            else if (e.Key == Key.Enter)
            {
                mPopup.IsOpen = false;
                DisplaySelectedMonthYear(selectedIndex);
            }
            else if (e.Key == Key.Down)
            {
                if (selectedIndex == 6)
                {
                    flag = "DownArrow";
                    AddandRemoveMonth();
                }
            }
            else if (e.Key == Key.Up)
            {
                if (selectedIndex == 0)
                {
                    flag = "UpArrow";
                    AddandRemoveMonth();
                }
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the li control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void li_MouseEnter(object sender, MouseEventArgs e)
        {
            ((ListBoxItem)sender).Focus();
        }

        /// <summary>
        /// Handles the MouseLeave event of the m_Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void m_Popup_MouseLeave(object sender, MouseEventArgs e)
        {
            mPopup.IsOpen = false;
        }

        /// <summary>
        /// Occurs when a key is pressed while focus is on this control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance that contains 
        /// the event data.</param>
        void m_Selector_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                mPopup.IsOpen = false;
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the m_Selector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void m_Selector_MouseLeave(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(mSelector);
        }

        /// <summary>
        /// Occurs when the left mouse button is released while the mouse pointer is
        /// over the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance that contains 
        /// the event data.</param>
        private void m_Selector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_timer.IsEnabled)
            {
                ////    It Represents For Double Click
                ////    stop the timer
                _timer.Stop();
                mtimer.Stop();
                mPopup.IsOpen = false;
                if (mSelector.SelectedItem != null)
                {
                    int selectedIndex = (sender as ListBox).SelectedIndex;
                    DisplaySelectedMonthYear(selectedIndex);
                }
            }
            else
            {
                _timer.Start();
            }
        }

        /// <summary>
        /// Invoked when the mouse pointer moves while over the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance that contains 
        /// the event data.</param>
        private void m_Selector_MouseMove(object sender, MouseEventArgs e)
        {
            // m_Selector.UnselectAll();
            Point position = e.GetPosition(mSelector);
            position.X = mSelector.ActualWidth / 2d;

            List<StackPanel> hits = VisualTreeHelper.FindElementsInHostCoordinates(position, mSelector) as List<StackPanel>;
            if (hits != null)
            {
                foreach (UIElement ui in hits)
                {
                    Type t = ui.GetType();
                    if (t.FullName == "System.Windows.Controls.ListBoxItem")
                    {
                        ListBoxItem item = (ListBoxItem)ui;

                        if (item != null)
                        {
                            if (position.Y > 0 && position.Y < mSelector.ActualHeight)
                            {
                                mtimer.Stop();

                                ////    item.IsSelected = true;
                                mSelector.SelectedItem = item.Content.ToString();
                                item.Focus();
                            }
                        }
                        else
                        {
                            mtimer.Start();
                        }

                        break;
                    }
                }
            }
            else
            {
                mtimer.Start();
            }
        }

        /// <summary>
        /// Invoked whenever selection is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance that contains 
        /// the event data.</param>
        private void m_Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ////ListBoxItem item = m_Selector.SelectedItem as ListBoxItem;
            ////string DataType = (string)item.Content;
            ////item.Focus();
        }

        /// <summary>
        /// Invoked when the timer interval has elapsed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance that contains 
        /// the event data.</param>
        private void m_timer_Tick(object sender, EventArgs e)
        {
            AddandRemoveMonth();
            mtimer.Interval = TimeSpan.FromMilliseconds(750);
        }

        /// <summary>
        /// Handles the MouseEnter event of the pl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void pl1_MouseEnter(object sender, MouseEventArgs e)
        {
            flag = "UpArrow";
            mtimer.Start();
        }

        /// <summary>
        /// Handles the MouseLeave event of the pl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void pl1_MouseLeave(object sender, MouseEventArgs e)
        {
            mtimer.Stop();
        }

        /// <summary>
        /// Handles the MouseEnter event of the pl2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void pl2_MouseEnter(object sender, MouseEventArgs e)
        {
            flag = "DownArrow";
            mtimer.Start();
        }

        /// <summary>
        /// Raises the HidePopup event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.HidePopupEventArgs"/> instance that contains 
        /// the event data.</param>
        private void RaiseHidePopupEvent(HidePopupEventArgs e)
        {
            if (HidePopup != null)
            {
                HidePopup(this, e);
            }
        }

        /// <summary>
        /// Refreshes month selector items.
        /// </summary>
        public void RefreshContent()
        {
            ////    FillMonthSelector(m_CurrentDate,4);
        }

        /// <summary>
        /// Shows the popup.
        /// </summary>
        public void Show(Date visibleDate, bool isMonthAbbreviated, CultureInfo cultureInfo, Date minDate, Date maxDate)
        {
            this.pl1.Fill = ScrollButtonFill;
            this.pl2.Fill = ScrollButtonFill;
            FillMonthSelector(visibleDate, isMonthAbbreviated, cultureInfo);
            DateTimeFormatInfo format = cultureInfo.DateTimeFormat;
            m_Format = format;
            mMinDate = minDate;
            m_MaxDate = maxDate;
            Monthabbreviated = isMonthAbbreviated;
            mPopup.IsOpen = true;
            mSelector.Focus();
            //// m_Selector.CaptureMouse();
        }
    }
}