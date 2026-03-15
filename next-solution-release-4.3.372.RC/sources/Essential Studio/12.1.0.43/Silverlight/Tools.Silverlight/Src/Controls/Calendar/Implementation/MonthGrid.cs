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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Month Grid Class.
    /// </summary>
    public class MonthGrid : FrameworkElement
    {
        /// <summary>
        /// Default number of columns.
        /// </summary>
        internal const int DEF_COLUMNS_COUNT = 4;

        /// <summary>
        /// Default number of rows.
        /// </summary>
        internal const int DEF_ROWS_COUNT = 3;

        /// <summary>
        /// Collection of Borders.
        /// </summary>
        private List<Border> mMonthBorderCollection;

        /// <summary>
        /// Collection of cells.
        /// </summary>
        private List<Cell> mMonthCellsCollection;

        /// <summary>
        /// Inner grid for cells layout.
        /// </summary>
        private Grid mMonthGrid;
        private VisibleDate visibledata;

        /// <summary>
        /// Default public constructor.
        /// Initializes new instance of the MonthGrid class.
        /// </summary>
        public MonthGrid()
        {
            mMonthGrid = new Grid();
            ////    m_MonthGrid.Name = "Month Grid";
            Cell_ForeColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x68, 0x93, 0xCF));
            mMonthCellsCollection = new List<Cell>();
            mMonthBorderCollection = new List<Border>();
            GenerateGrid();
            FillGrid();
        }

        /// <summary>
        /// Gets or sets the color of the cell_ fore.
        /// </summary>
        /// <value>The color of the cell_ fore.</value>
        protected internal Brush Cell_ForeColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the focused cell.
        /// </summary>
        /// <value>The focused cell.</value>
        protected internal MonthCell FocusedCell
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the m_ mouse hovered cell.
        /// </summary>
        /// <value>The m_ mouse hovered cell.</value>
        protected internal MonthCell m_MouseHoveredCell
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content of the focused cell.
        /// </summary>
        /// <value>The content of the focused cell.</value>
        protected internal int FocusedCellContent
        {
            get;
            set;
        }

        ////    with Border and Cell
        /// <summary>
        /// Gets or sets the m_ selected month border cell.
        /// </summary>
        /// <value>The m_ selected month border cell.</value>
        protected Border m_SelectedMonthBorderCell
        {   
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the m_ selected month cell.
        /// </summary>
        /// <value>The m_ selected month cell.</value>
        protected Cell m_SelectedMonthCell
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the month border collection.
        /// </summary>
        /// <value>The month border collection.</value>
        protected internal List<Border> MonthBorderCollection
        {
            get
            {
                return mMonthBorderCollection;
            }

            set
            {
                mMonthBorderCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets the month cells collection.
        /// </summary>
        /// <value>The month cells collection.</value>
        protected internal List<Cell> MonthCellsCollection
        {
            get
            {
                return mMonthCellsCollection;
            }

            set
            {
                mMonthCellsCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent calendar.
        /// </summary>
        /// <value>The parent calendar.</value>
        protected internal CalendarControl ParentCalendar
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the populated month grid.
        /// </summary>
        /// <value>The populated month grid.</value>
        public Grid PopulatedMonthGrid
        {
            get
            {
                return mMonthGrid;
            }
        }

        /// <summary>
        /// Gets or sets the selected month border.
        /// </summary>
        /// <value>The selected month border.</value>
        protected internal Border SelectedMonthBorder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date visible settings.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="VisibleDate"/>
        /// </value>
        /// <seealso cref="VisibleDate"/>
        protected internal VisibleDate VisibleData
        {
            get
            {
                return visibledata;
            }

            set
            {
                visibledata = value;
            }
        }

        /// <summary>
        /// Applies the border.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="b">The b.</param>
        private void ApplyBorder(MonthCell cell, Border b)
        {
            ////    cell.IsMouseHover = true;
            cell.IsSelected = true;
            cell.Focus();
            cell.Foreground = ParentCalendar.SelectedCellForeground;
            b.CornerRadius = ParentCalendar.SelectedCellCornerRadius;
            b.BorderBrush = ParentCalendar.SelectedCellBorderBrush;
            b.BorderThickness = ParentCalendar.SelectedCellBorderThickness;
            b.Background = ParentCalendar.SelectedCellBackground;
            b.UpdateLayout();
        }

        /// <summary>
        /// Applies the null border.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        private Border ApplyNullBorder(int row, int column)
        {
            Border b = new Border();
            ////    b.Name = "ApplyBorder" + row.ToString() + column.ToString();
            return b;
        }

        /// <summary>
        /// Applies the null border value.
        /// </summary>
        /// <param name="b">The b.</param>
        private void ApplyNullBorderValue(Border b)
        {
            b.Background = new SolidColorBrush();
            b.CornerRadius = new CornerRadius(0);
            b.BorderThickness = new Thickness(0);
            b.BorderBrush = new SolidColorBrush();
        }

        /// <summary>
        /// Handles the MouseEnter event of the br control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void br_MouseEnter(object sender, MouseEventArgs e)
        {
            Border b = (Border)sender;
            int iRow = Grid.GetRow((FrameworkElement)b);
            int iColumn = Grid.GetColumn((FrameworkElement)b);

            Cell cell = (Cell)b.Child;  ////    m_MonthGrid.FindName("MonthCell" + iRow.ToString() + iColumn.ToString());
            m_MouseHoveredCell = (MonthCell)cell;
            cell.Focus();
            if (!cell.IsSelected)
            {
                    m_MouseHoveredCell = (MonthCell)cell;
                ////    cell.IsMouseHover = true;

                cell.Focus();
                b.CornerRadius = ParentCalendar.MouseHoverCellCornerRadius;
                b.BorderBrush = ParentCalendar.MouseHoverCellBorderBrush;
                b.BorderThickness = ParentCalendar.MouseHoverCellBorderThickness;
                b.Background = ParentCalendar.MouseHoverCellBackground;
                cell.Foreground = ParentCalendar.MouseHoverCellForeground;
                b.UpdateLayout();
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the br control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void br_MouseLeave(object sender, MouseEventArgs e)
        {
            Border b = (Border)sender;
            int iRow = Grid.GetRow((FrameworkElement)b);
            int iColumn = Grid.GetColumn((FrameworkElement)b);
            m_MouseHoveredCell = null;
            Cell cell = (Cell)b.Child;  ////    m_MonthGrid.FindName("MonthCell" + iRow.ToString() + iColumn.ToString());
            if (!cell.IsSelected)
            {
                cell.Foreground = Cell_ForeColor;
                ApplyNullBorderValue(b);
                b.UpdateLayout();
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the br control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void br_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border b = (Border)sender;
            int iRow = Grid.GetRow((FrameworkElement)b);
            int iColumn = Grid.GetColumn((FrameworkElement)b);
            if (m_SelectedMonthBorderCell != null && m_SelectedMonthBorderCell != ((Border)sender))
            {
                ApplyNullBorderValue(m_SelectedMonthBorderCell);
                m_SelectedMonthCell.Foreground = Cell_ForeColor;
                m_SelectedMonthCell.IsSelected = false;
            }

            Cell cell = (Cell)b.Child;  ////    m_MonthGrid.FindName("MonthCell" + iRow.ToString() + iColumn.ToString());
            cell.Focus();
            if (!cell.IsSelected)
            {
                m_SelectedMonthCell = cell;
                m_SelectedMonthBorderCell = b;
                SelectedMonthBorder = b;
                cell.Foreground = ParentCalendar.SelectedCellForeground;
                cell.IsSelected = true;
                ////    cell.IsMouseHover = true;
            }
        }

        /// <summary>
        /// Creates new instance of the <see cref="Syncfusion.Windows.Tools.Controls.MonthCell"/> class.
        /// </summary>
        /// <returns>New instance of the <see cref="Syncfusion.Windows.Tools.Controls.MonthCell"/> class.</returns>
        protected Cell CreateCell()
        {
            return new MonthCell();
        }

        /// <summary>
        /// Adds day and dayName cells to the grid.
        /// </summary>
        private void FillGrid()
        {
            ////    m_innerGrid.k
            for (int i = 0, k = 0; i < DEF_ROWS_COUNT; i++)
            {
                for (int j = 0; j < DEF_COLUMNS_COUNT; j++)
                {
                    Border br = ApplyNullBorder(i, j);
                    br.MouseEnter += new MouseEventHandler(br_MouseEnter);
                    br.MouseLeave += new MouseEventHandler(br_MouseLeave);
                    br.MouseLeftButtonUp += new MouseButtonEventHandler(br_MouseLeftButtonUp);
                    Cell monthCell = CreateCell();
                    ////    monthCell.Name = "MonthCell" + i.ToString() + j.ToString();
                    monthCell.HorizontalAlignment = HorizontalAlignment.Center;
                    monthCell.VerticalAlignment = VerticalAlignment.Center;
                    monthCell.KeyDown += new KeyEventHandler(monthCell_KeyDown);
                    br.Child = monthCell;
                    Grid.SetRow((FrameworkElement)br, i);
                    Grid.SetColumn((FrameworkElement)br, j);
                    MonthCellsCollection.Add(monthCell);
                    MonthBorderCollection.Add(br);
                    mMonthGrid.Children.Add(br);

                    k++;
                }
            }
        }

        /// <summary>
        /// Fills the grid with column definitions and row definitions.
        /// </summary>
        private void GenerateGrid()
        {
            for (int i = 0; i < DEF_COLUMNS_COUNT; i++)
            {
                ColumnDefinition cdColumn = new ColumnDefinition();
                cdColumn.Width = new GridLength(1, GridUnitType.Star);
                mMonthGrid.ColumnDefinitions.Add(cdColumn);
            }
            // Define Rows
            for (int i = 0; i < DEF_ROWS_COUNT; i++)
            {
                RowDefinition rdRow = new RowDefinition();
                rdRow.Height = new GridLength(1, GridUnitType.Star);
                mMonthGrid.RowDefinitions.Add(rdRow);
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the monthCell control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        void monthCell_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Alt && Keyboard.Modifiers != ModifierKeys.Control && Keyboard.Modifiers != ModifierKeys.Shift && Keyboard.Modifiers != ModifierKeys.Windows)
            {
                if (!e.Handled)
                {
                    MonthCell mc;
                    if (FocusedCell == null)
                    {
                        mc = (MonthCell)sender;
                        FocusedCell = mc;
                        FocusedCellContent = (int)mc.MonthNumber;
                    }

                    int loc = NewFocusedCellIndex();
                    mc = (MonthCell)MonthCellsCollection[loc];
                    Border b = (Border)mc.Parent;
                    int iRow = Grid.GetRow((FrameworkElement)b);
                    int iColumn = Grid.GetColumn((FrameworkElement)b);

                    if (Keyboard.Modifiers == ModifierKeys.None)
                    {
                        bool changingEnabled = false;
                        switch (e.Key)
                        {
                            case Key.Down:
                                e.Handled = true;
                                break;

                            case Key.Left:
                                e.Handled = true;
                                break;

                            case Key.Right:
                                e.Handled = true;
                                break;

                            case Key.Up:
                                e.Handled = true;
                                break;

                            case Key.Enter:

                                changingEnabled = true;

                                e.Handled = true;
                                break;

                            default:
                                break;
                        }

                        OnMonthCellClick((Cell)mc, changingEnabled, e, loc);
                        ApplyBorderMouseHoveredCell();
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// News the index of the focused cell.
        /// </summary>
        /// <returns></returns>
        private int NewFocusedCellIndex()
        {
            int iNewFocusedCellIndex = -1;

            for (int i = 0; i < MonthCellsCollection.Count; i++)
            {
                MonthCell dc = (MonthCell)MonthCellsCollection[i];
                if (((MonthCell)dc).MonthNumber == FocusedCellContent)
                {
                    ////    (dc.IsSelected)
                    iNewFocusedCellIndex = i;
                }
                dc.IsSelected = false;
            }

            return iNewFocusedCellIndex;
        }

        /// <summary>
        /// Applies the border mouse hovered cell.
        /// </summary>
        void ApplyBorderMouseHoveredCell()
        {
            if (m_MouseHoveredCell != null && !((Cell)m_MouseHoveredCell).IsSelected)
            {
                    Cell cell = (Cell)m_MouseHoveredCell;
                    Border b = cell.Parent as Border;
                    b.CornerRadius = ParentCalendar.MouseHoverCellCornerRadius;
                    b.BorderBrush = ParentCalendar.MouseHoverCellBorderBrush;
                    b.BorderThickness = ParentCalendar.MouseHoverCellBorderThickness;
                    b.Background = ParentCalendar.MouseHoverCellBackground;
                    cell.Foreground = ParentCalendar.MouseHoverCellForeground;
                    b.UpdateLayout(); 
            }
        }

        /// <summary>
        /// Called when [month cell click].
        /// </summary>
        /// <param name="mc">The mc.</param>
        /// <param name="changingEnabled">if set to <c>true</c> [changing enabled].</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        /// <param name="loc">The loc.</param>
       private void OnMonthCellClick(Cell mc, bool changingEnabled, KeyEventArgs e, int loc)
        {
            Border b = (Border)mc.Parent;
            CultureInfo parentCul = null;
            VisibleDate visibleData = new VisibleDate();
            if (!changingEnabled)
            {
                if (e.Key == Key.Right || e.Key == Key.Left)
                {
                    // mc.IsMouseHover = false;
                    mc.IsSelected = false;
                    mc.Focus();
                    mc.Foreground = Cell_ForeColor;
                    ApplyNullBorderValue(b);
                    b.UpdateLayout();
                }

                if (e.Key == Key.Right)
                {
                    MonthCell c;
                    Border m_b;
                    if (loc == 11)
                    {
                        if (ParentCalendar.next.Fill != ParentCalendar.buttonDisabledColor)
                        {
                            c = (MonthCell)MonthCellsCollection[0];
                            m_b = (Border)c.Parent;
                            //// m_MouseHoveredCell = null;
                            ParentCalendar.ChangeVisibleData(1, 0);
                            ParentCalendar.NextMonthDisplay();
                            c.IsSelected = true;
                            FocusedCell = c;
                            SelectedMonthBorder = m_b;
                            FocusedCellContent = (int)c.MonthNumber;
                        }
                    }
                    else
                    {
                        c = (MonthCell)MonthCellsCollection[loc + 1];
                        m_b = (Border)c.Parent;
                        ParentCalendar.GetParetntCalendar(ref parentCul, ref visibleData);
                        ParentCalendar.SetMonthCellContent(parentCul, c.MonthNumber, visibleData.VisibleYear);
                        ApplyBorder(c, m_b);
                        c.IsSelected = true;
                        FocusedCell = c;
                        SelectedMonthBorder = m_b;
                        FocusedCellContent = (int)c.MonthNumber;
                    }

                    ////c.IsSelected = true;
                    ////FocusedCell = c;
                    ////SelectedMonthBorder = m_b;
                    ////FocusedCellContent = (int)c.MonthNumber;
                }
                else if (e.Key == Key.Left)
                {                  
                    MonthCell c;
                    Border m_b;
                    if (loc == 0)
                    {
                        if (ParentCalendar.previous.Fill != ParentCalendar.buttonDisabledColor)
                        {
                            c = (MonthCell)MonthCellsCollection[11];
                            m_b = (Border)c.Parent;
                            //// m_MouseHoveredCell = null;
                            ParentCalendar.ChangeVisibleData(12, 0);
                            ParentCalendar.PreviouMonthDisplay();

                            c.IsSelected = true;
                            FocusedCell = c;
                            SelectedMonthBorder = m_b;
                            FocusedCellContent = (int)c.MonthNumber;
                        }
                    }
                    else
                    {
                        c = (MonthCell)MonthCellsCollection[loc - 1];
                        m_b = (Border)c.Parent;
                        ParentCalendar.GetParetntCalendar(ref parentCul, ref visibleData);
                        ParentCalendar.SetMonthCellContent(parentCul, c.MonthNumber, visibleData.VisibleYear);
                        ApplyBorder(c, m_b);
                        c.IsSelected = true;
                        FocusedCell = c;
                        SelectedMonthBorder = m_b;
                        FocusedCellContent = (int)c.MonthNumber;
                    }
                }
                else if (e.Key == Key.Up)
                {
                    MonthCell c;
                    Border m_b;
                    if (loc >= 4 && loc <= 11)
                    {
                        ////    mc.IsMouseHover = false;
                        mc.IsSelected = false;
                        mc.Focus();
                        mc.Foreground = Cell_ForeColor;
                        ApplyNullBorderValue(b);
                        b.UpdateLayout();
                        c = (MonthCell)MonthCellsCollection[loc - 4];
                        m_b = (Border)c.Parent;
                        ParentCalendar.GetParetntCalendar(ref parentCul, ref visibleData);
                        ParentCalendar.SetMonthCellContent(parentCul, c.MonthNumber, visibleData.VisibleYear);
                        ApplyBorder(c, m_b);
                        c.IsSelected = true;
                        FocusedCell = c;
                        SelectedMonthBorder = m_b;
                        FocusedCellContent = (int)c.MonthNumber;
                    }
                    else if (loc < 4 && loc >= 0)
                    {
                        if (ParentCalendar.previous.Fill != ParentCalendar.buttonDisabledColor)
                        {
                            c = (MonthCell)MonthCellsCollection[8 + loc];
                            m_b = (Border)c.Parent;
                            //// m_MouseHoveredCell = null;
                            ParentCalendar.ChangeVisibleData(9 + loc, 0);
                            ParentCalendar.PreviouMonthDisplay();
                            c.IsSelected = true;
                            FocusedCell = c;
                            SelectedMonthBorder = m_b;
                            FocusedCellContent = (int)c.MonthNumber;
                        }
                    }
                }
                else if (e.Key == Key.Down)
                {
                    MonthCell c;
                    Border m_b;
                    if (loc >= 0 && loc < 8)
                    {
                        ////    mc.IsMouseHover = false;
                        mc.IsSelected = false;
                        mc.Focus();
                        mc.Foreground = Cell_ForeColor;
                        ApplyNullBorderValue(b);
                        b.UpdateLayout();
                        c = (MonthCell)MonthCellsCollection[loc + 4];
                        m_b = (Border)c.Parent;
                        ParentCalendar.GetParetntCalendar(ref parentCul, ref visibleData);
                        ParentCalendar.SetMonthCellContent(parentCul, c.MonthNumber, visibleData.VisibleYear);
                        ApplyBorder(c, m_b);
                        c.IsSelected = true;
                        FocusedCell = c;
                        SelectedMonthBorder = m_b;
                        FocusedCellContent = (int)c.MonthNumber;
                    }
                    else if (loc < 12 && loc >= 8)
                    {
                        if (ParentCalendar.next.Fill != ParentCalendar.buttonDisabledColor)
                        {
                            c = (MonthCell)MonthCellsCollection[loc-8];
                            m_b = (Border)c.Parent;
                            //// m_MouseHoveredCell = null;
                            ParentCalendar.ChangeVisibleData(loc-7, 0);
                            ParentCalendar.NextMonthDisplay();
                            c.IsSelected = true;
                            FocusedCell = c;
                            SelectedMonthBorder = m_b;
                            FocusedCellContent = (int)c.MonthNumber;
                        }
                    }
                }
            }
            else
            {
                ////    mc.IsMouseHover = false;
                mc.IsSelected = false;
                mc.Focus();
                mc.Foreground = Cell_ForeColor;
                ApplyNullBorderValue(b);
                b.UpdateLayout();
                int location = NewFocusedCellIndex();
                MonthCell c = (MonthCell)MonthCellsCollection[location];
                Border m_b = (Border)c.Parent;
                ApplyBorder(c, m_b);
                SelectedMonthBorder = m_b;
 
                ////m_MouseHoveredCell = null;

                ////    c.IsSelected = true;
                ////    FocusedCell = c;
                ////    FocusedCellContent = (int)c.MonthNumber;
 
                Border mb = MonthBorderCollection[location];
                ParentCalendar.ChangeVisibleData(FocusedCellContent, 0);
                ParentCalendar.GetParetntCalendar(ref parentCul, ref visibleData);
                ParentCalendar.SetMonthCellContent(parentCul, visibleData.VisibleMonth, visibleData.VisibleYear);
                ParentCalendar.StoryBoardForMonth(((FrameworkElement)mb), 2d, 2d, (FrameworkElement)ParentCalendar.DayNamesAndDateGrid.PopulatedDayGrid(), (FrameworkElement)ParentCalendar.MonthNameGrid.PopulatedMonthGrid);
            }
        }

        /// <summary>
        /// Sets the selected cell.
        /// </summary>
        /// <param name="date">Current date.</param>
        public void SetIsSelected(VisibleDate date)
        {
            ////foreach (MonthCell mc in CellsCollection)
            ////{
            ////    if (mc.MonthNumber == date.VisibleMonth)
            ////        mc.IsSelected = true;
            ////    else
            ////        mc.IsSelected = false;
            ////}
        }

        /// <summary>
        /// Sets the <see cref="T:Syncfusion.Windows.Tools.Controls.MonthCell.Content"/> property of the <see cref="Syncfusion.Windows.Tools.Controls.MonthCell"/>.
        /// </summary>
        /// <param name="culture">Current culture.</param>
        protected internal void SetMonthCellContent(CultureInfo culture)
        {
            DateTimeFormatInfo format = culture.DateTimeFormat;

            foreach (MonthCell mc in MonthCellsCollection)
            {
                if (mc.Visibility == Visibility.Visible)
                {
                    mc.Content = format.AbbreviatedMonthNames[mc.MonthNumber - 1];
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="MonthCell.MonthNumber"/> property of the <see cref="Syncfusion.Windows.Tools.Controls.MonthCell"/>.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="calendar">Current calendar.</param>
        /// <param name="culture">Culture</param>
        protected internal void SetMonthNumber(VisibleDate date, System.Globalization.Calendar calendar, CultureInfo culture)
        {
            if (m_SelectedMonthBorderCell != null)
            {
                Border m = (Border)m_SelectedMonthBorderCell;
                m.CornerRadius = new CornerRadius(0);
                m.BorderBrush = new SolidColorBrush();
                m.Background = new SolidColorBrush();
                m.BorderThickness = new Thickness(0);
                Cell c = m_SelectedMonthCell;
                c.Foreground = Cell_ForeColor;
                c.IsSelected = false;
            }

            int k = 1, i = 0;
            Date curDate;
            Date minDate = new Date(calendar.MinSupportedDateTime, calendar);
            Date maxDate = new Date(calendar.MaxSupportedDateTime, calendar);

            foreach (MonthCell mc in MonthCellsCollection)
            {
                mc.Foreground = Cell_ForeColor;
                ApplyNullBorderValue(MonthBorderCollection[i]);
                curDate = new Date(date.VisibleYear, k, 1);

                if (curDate > maxDate || curDate < minDate)
                {
                    mc.Visibility = Visibility.Collapsed;
                    mc.MonthNumber = -1;
                }
                else
                {
                    if (mc.Visibility == Visibility.Collapsed)
                    {
                        mc.Visibility = Visibility.Visible;
                    }

                    mc.MonthNumber = k;
                    if (k == date.VisibleMonth)
                    {
                        ////    insted of k we use i
                        Border b = MonthBorderCollection[i];
                        if (ParentCalendar != null)
                        {
                            b.CornerRadius = ParentCalendar.SelectedCellCornerRadius;
                            b.BorderBrush = ParentCalendar.SelectedCellBorderBrush;
                            b.BorderThickness = ParentCalendar.SelectedCellBorderThickness;
                            b.Background = ParentCalendar.SelectedCellBackground;
                            mc.Foreground = ParentCalendar.SelectedCellForeground;
                            b.UpdateLayout();
                        }

                        m_SelectedMonthCell = mc;
                        FocusedCell = mc;
                        FocusedCellContent = mc.MonthNumber;
                        mc.Focus();
                        m_SelectedMonthBorderCell = b;
                        SelectedMonthBorder = b;
                        mc.IsSelected = true;
                        //// mc.IsMouseHover = true;
                    }
                }

                k++;
                i++;
            }

            SetMonthCellContent(culture);
        }
    }
}