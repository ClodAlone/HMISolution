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
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Year Range Grid Class.
    /// </summary>
    public class YearRangeGrid
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
        /// Collection of cells.
        /// </summary>
        private List<Border> mYearBorderCollection;

        /// <summary>
        /// Collection of cells.
        /// </summary>
        private List<Cell> mYearCellsCollection;

        /// <summary>
        /// Inner grid for cells layout.
        /// </summary>
        private Grid mYearRangeGrid;

        private VisibleDate visibledata;

        /// <summary>
        /// Default public constructor.
        /// Initializes new instance of the MonthGrid class.
        /// </summary>
        public YearRangeGrid()
        {
            mYearRangeGrid = new Grid();
            ////m_YearRangeGrid.Name = "Year Grid";
            Cell_ForeColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x68, 0x93, 0xCF));
            mYearCellsCollection = new List<Cell>();
            mYearBorderCollection = new List<Border>();
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
        /// Gets or sets the end year range.
        /// </summary>
        /// <value>The end year range.</value>
        protected internal int EndYearRange
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the focused cell.
        /// </summary>
        /// <value>The focused cell.</value>
        protected internal YearRangeCell FocusedCell
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the m_ mouse hovered cell.
        /// </summary>
        /// <value>The m_ mouse hovered cell.</value>
        protected internal YearRangeCell m_MouseHoveredCell
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content of the focused cell.
        /// </summary>
        /// <value>The content of the focused cell.</value>
        protected internal YearsRange FocusedCellContent
        {
            get;
            set;
        }

        ////with Border and Cell
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
        /// Gets or sets the m_ selected year cell.
        /// </summary>
        /// <value>The m_ selected year cell.</value>
        protected Cell m_SelectedYearCell
        {
            get;
            set;
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
        /// Gets the populated year range grid.
        /// </summary>
        /// <value>The populated year range grid.</value>
        public Grid PopulatedYearRangeGrid
        {
            get
            {
                return mYearRangeGrid;
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
        /// Gets or sets the selected year range.
        /// </summary>
        /// <value>The selected year range.</value>
        public string SelectedYearRange
        {
            get;
            private set;
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
        /// Gets or sets the year border collection.
        /// </summary>
        /// <value>The year border collection.</value>
        protected internal List<Border> YearBorderCollection
        {
            get
            {
                return mYearBorderCollection;
            }

            set
            {
                mYearBorderCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets the year cells collection.
        /// </summary>
        /// <value>The year cells collection.</value>
        protected internal List<Cell> YearCellsCollection
        {
            get
            {
                return mYearCellsCollection;
            }

            set
            {
                mYearCellsCollection = value;
            }
        }

        private void ApplyBorder(YearRangeCell cell, Border b)
        {
            ////cell.IsMouseHover = true;
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
            ////b.Name = "ApplyBorder" + row.ToString() + column.ToString();
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
        /// Handles the KeyDown event of the br control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        void br_KeyDown(object sender, KeyEventArgs e)
        {
            ////throw new NotImplementedException();
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

            Cell cell = (Cell)b.Child;  //// m_YearRangeGrid.FindName("YearCell" + iRow.ToString() + iColumn.ToString());
            m_MouseHoveredCell = (YearRangeCell)cell;
            if (!cell.IsSelected)
            {
                ////m_MouseHoverCell = cell;
                m_MouseHoveredCell = (YearRangeCell)cell;
                cell.Focus();
                cell.IsMouseHover = true;
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
            Cell cell = (Cell)b.Child;  //// m_YearRangeGrid.FindName("YearCell" + iRow.ToString() + iColumn.ToString());
            if (!cell.IsSelected)
            {
                b.CornerRadius = new CornerRadius(0);
                b.BorderBrush = new SolidColorBrush();
                b.Background = new SolidColorBrush();
                b.BorderThickness = new Thickness(0);
                cell.Foreground = Cell_ForeColor;
                b.UpdateLayout();
            }
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
                m_SelectedYearCell.Foreground = Cell_ForeColor;
                m_SelectedYearCell.IsSelected = false;
            }

            Cell cell = (Cell)b.Child;  //// m_YearRangeGrid.FindName("YearCell" + iRow.ToString() + iColumn.ToString());
            cell.Focus();
            if (!cell.IsSelected)
            {
                m_SelectedYearCell = cell;
                cell.Foreground = ParentCalendar.SelectedCellForeground;
                SelectedYearRange = cell.Content.ToString().Replace('\n', ' ');
                m_SelectedMonthBorderCell = b;
                SelectedMonthBorder = b;
                cell.IsSelected = true;
            }
        }

        /// <summary>
        /// Determines whether [contains current year] [the specified date].
        /// </summary>
        /// <param name="date">The date.</param>
        protected void ContainsCurrentYear(VisibleDate date)
        {
            if (date.VisibleYear % 10 == 0)
            {
                SelectedYearRange = date.VisibleYear.ToString() + "-" + (date.VisibleYear + 9).ToString();
            }
            else
            {
                int r = date.VisibleYear % 10;
                int val = 0;
                if ((10 - r) < 0)
                {
                    val = -(10 - r);
                }
                else
                {
                    val = 10 - r;
                }

                SelectedYearRange = (date.VisibleYear - r).ToString() + "-" + (date.VisibleYear + val - 1).ToString();
            }

            if (SelectedYearRange.ToLower() == "0-9")
            {
                SelectedYearRange = "1-9";
            }
        }

        /// <summary>
        /// Creates new instance of the <see cref="Syncfusion.Windows.Tools.Controls.MonthCell"/> class.
        /// </summary>
        /// <returns>New instance of the <see cref="Syncfusion.Windows.Tools.Controls.MonthCell"/> class.</returns>
        protected Cell CreateCell()
        {
            return new YearRangeCell();
        }

        /// <summary>
        /// Adds day and dayName cells to the grid.
        /// </summary>
        private void FillGrid()
        {
            ////m_innerGrid.k
            for (int i = 0, k = 0; i < DEF_ROWS_COUNT; i++)
            {
                for (int j = 0; j < DEF_COLUMNS_COUNT; j++)
                {
                    Border br = ApplyNullBorder(i, j);
                    br.MouseEnter += new MouseEventHandler(br_MouseEnter);
                    br.MouseLeave += new MouseEventHandler(br_MouseLeave);
                    br.MouseLeftButtonDown += new MouseButtonEventHandler(br_MouseLeftButtonUp);
                    ////br.KeyDown += new KeyEventHandler(br_KeyDown);

                    Cell monthCell = CreateCell();
                    ////monthCell.Name = "YearCell" + i.ToString() + j.ToString();
                    monthCell.HorizontalAlignment = HorizontalAlignment.Center;
                    monthCell.VerticalAlignment = VerticalAlignment.Center;
                    monthCell.KeyDown += new KeyEventHandler(monthCell_KeyDown);
                    br.Child = monthCell;
                    Grid.SetRow((FrameworkElement)br, i);
                    Grid.SetColumn((FrameworkElement)br, j);
                    YearCellsCollection.Add(monthCell);
                    YearBorderCollection.Add(br);
                    mYearRangeGrid.Children.Add(br);
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
                mYearRangeGrid.ColumnDefinitions.Add(cdColumn);
            }

            //// Define Rows
            for (int i = 0; i < DEF_ROWS_COUNT; i++)
            {
                RowDefinition rdRow = new RowDefinition();
                rdRow.Height = new GridLength(1, GridUnitType.Star);
                mYearRangeGrid.RowDefinitions.Add(rdRow);
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
                    YearRangeCell mc;
                    if (FocusedCell == null)
                    {
                        mc = (YearRangeCell)sender;
                        FocusedCell = mc;
                        FocusedCellContent = mc.Years;
                    }

                    int loc = NewFocusedCellIndex();
                    if (loc >= 0)
                    {
                        mc = (YearRangeCell)YearCellsCollection[loc];

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

            for (int i = 0; i < YearCellsCollection.Count; i++)
            {
                YearRangeCell dc = (YearRangeCell)YearCellsCollection[i];
                dc.IsSelected = false;
                if (((YearRangeCell)dc).Years == FocusedCellContent)
                {
                    //// (dc.IsSelected)
                    iNewFocusedCellIndex = i;
                }
            }

            return iNewFocusedCellIndex;
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
            CultureInfo parentCul = null;
            VisibleDate visibleData = new VisibleDate();
            Border b = (Border)mc.Parent;
            if (!changingEnabled)
            {
                if (e.Key == Key.Right || e.Key == Key.Left)
                {
                    ////mc.IsMouseHover = false;
                    mc.IsSelected = false;
                    mc.Focus();
                    mc.Foreground = Cell_ForeColor;
                    ApplyNullBorderValue(b);
                    b.UpdateLayout();
                }

                if (e.Key == Key.Right)
                {
                    YearRangeCell c;
                    Border m_b;
                    if (loc == 10)
                    {
                        if (((YearRangeCell)YearCellsCollection[loc + 1]).Visibility == Visibility.Visible && ParentCalendar.next.Fill != ParentCalendar.buttonDisabledColor)
                        {
                            c = (YearRangeCell)YearCellsCollection[1];
                            m_b = (Border)c.Parent;
                            c.IsSelected = true;
                            FocusedCell = c;
                            ParentCalendar.NextMonthDisplay();
                            if (m_SelectedMonthBorderCell != null)
                            {
                                Border yb = (Border)m_SelectedMonthBorderCell;
                                yb.CornerRadius = new CornerRadius(0);
                                yb.BorderBrush = new SolidColorBrush();
                                yb.Background = new SolidColorBrush();
                                yb.BorderThickness = new Thickness(0);
                                Cell m_c = m_SelectedYearCell;
                                m_c.Foreground = Cell_ForeColor;
                                m_c.IsSelected = false;
                            }

                            ApplyBorder(c, m_b);
                        }
                        else
                        {
                            c = FocusedCell;
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                        }
                    }
                    else
                    {
                        if (((YearRangeCell)YearCellsCollection[loc + 1]).Visibility == Visibility.Visible)
                        {
                            c = (YearRangeCell)YearCellsCollection[loc + 1];
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                        }
                        else
                        {
                            c = FocusedCell;
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                        }
                    }

                    c.IsSelected = true;
                    FocusedCell = c;
                    SelectedMonthBorder = m_b;
                    FocusedCellContent = c.Years;
                }
                else if (e.Key == Key.Left)
                {
                    YearRangeCell c;
                    Border m_b;
                    if (loc == 1)
                    {
                        if (((YearRangeCell)YearCellsCollection[0]).Visibility == Visibility.Visible  && ParentCalendar.previous.Fill != ParentCalendar.buttonDisabledColor)
                        {
                            c = (YearRangeCell)YearCellsCollection[10];
                            m_b = (Border)c.Parent;
                            ParentCalendar.PreviouMonthDisplay();
                            if (m_SelectedMonthBorderCell != null)
                            {
                                Border yb = (Border)m_SelectedMonthBorderCell;
                                yb.CornerRadius = new CornerRadius(0);
                                yb.BorderBrush = new SolidColorBrush();
                                yb.Background = new SolidColorBrush();
                                yb.BorderThickness = new Thickness(0);
                                Cell m_c = m_SelectedYearCell;
                                m_c.Foreground = Cell_ForeColor;
                                m_c.IsSelected = false;
                            }

                            ApplyBorder(c, m_b);
                        }
                        else
                        {
                            c = FocusedCell;
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                        }
                    }
                    else
                    {
                        if (((YearRangeCell)YearCellsCollection[loc - 1]).Visibility == Visibility.Visible)
                        {
                            c = (YearRangeCell)YearCellsCollection[loc - 1];
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                        }
                        else
                        {
                            c = FocusedCell;
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                        }
                    }

                    c.IsSelected = true;
                    FocusedCell = c;
                    SelectedMonthBorder = m_b;
                    FocusedCellContent = c.Years;
                }
                else if (e.Key == Key.Up)
                {
                    YearRangeCell c;
                    Border m_b;
                    if (loc > 4 && loc <= 11)
                    {
                        ////mc.IsMouseHover = false;
                        mc.IsSelected = false;
                        mc.Focus();
                        mc.Foreground = Cell_ForeColor;
                        ApplyNullBorderValue(b);
                        b.UpdateLayout();
                        c = (YearRangeCell)YearCellsCollection[loc - 4];
                        if (c.Visibility == Visibility.Visible)
                        {
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                            c.IsSelected = true;
                            FocusedCell = c;
                            FocusedCellContent = c.Years;
                            SelectedMonthBorder = m_b;
                        }
                        else
                        {
                            c = FocusedCell;
                            m_b = (Border)c.Parent;
                            SelectedMonthBorder = m_b;
                            ApplyBorder(c, m_b);
                        }
                    }
                    else if ((loc <= 4 && loc >= 0) && ParentCalendar.previous.Fill != ParentCalendar.buttonDisabledColor)
                    {
                        if (loc == 4)
                        {
                            c = (YearRangeCell)YearCellsCollection[10];
                        }
                        else
                        {
                            c = (YearRangeCell)YearCellsCollection[8 + loc];
                            if (loc == 3)
                            {
                                c = (YearRangeCell)YearCellsCollection[7 + loc];
                            }
                        }
                        ////c = (YearRangeCell)YearCellsCollection[10];
                        m_b = (Border)c.Parent;
                        ParentCalendar.PreviouMonthDisplay();
                        if (m_SelectedMonthBorderCell != null)
                        {
                            Border yb = (Border)m_SelectedMonthBorderCell;
                            yb.CornerRadius = new CornerRadius(0);
                            yb.BorderBrush = new SolidColorBrush();
                            yb.Background = new SolidColorBrush();
                            yb.BorderThickness = new Thickness(0);
                            Cell m_c = m_SelectedYearCell;
                            m_c.Foreground = Cell_ForeColor;
                            m_c.IsSelected = false;
                        }

                        ApplyBorder(c, m_b);
                        FocusedCell = c;
                        SelectedMonthBorder = m_b;
                        FocusedCellContent = c.Years;
                    }
                    else
                    {
                        c = FocusedCell;
                        m_b = (Border)c.Parent;
                        SelectedMonthBorder = m_b;
                        ApplyBorder(c, m_b);
                    }
                }
                else if (e.Key == Key.Down)
                {
                    YearRangeCell c;
                    Border m_b;
                    if (loc >= 0 && loc < 7)
                    {
                        //// mc.IsMouseHover = false;
                        mc.IsSelected = false;
                        mc.Focus();
                        mc.Foreground = Cell_ForeColor;
                        ApplyNullBorderValue(b);
                        b.UpdateLayout();
                        c = (YearRangeCell)YearCellsCollection[loc + 4];
                        if (c.Visibility == Visibility.Visible)
                        {
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                            c.IsSelected = true;
                            FocusedCell = c;
                            FocusedCellContent = c.Years;
                            SelectedMonthBorder = m_b;
                        }
                        else
                        {
                            c = FocusedCell;
                            m_b = (Border)c.Parent;
                            SelectedMonthBorder = m_b;
                            ApplyBorder(c, m_b);
                        }
                    }
                    else if (loc < 12 && loc >= 7)
                    {
                        ////((YearCell)YearCellsCollection[loc + 4]).Visibility == Visibility.Visible && 
                        if (ParentCalendar.next.Fill != ParentCalendar.buttonDisabledColor)
                        {
                            if (loc == 7)
                            {
                                c = (YearRangeCell)YearCellsCollection[1];
                            }
                            else
                            {
                                c = (YearRangeCell)YearCellsCollection[loc - 8];
                            }
                            ////c = (YearRangeCell)YearCellsCollection[1];
                            m_b = (Border)c.Parent;
                            c.IsSelected = true;
                            FocusedCell = c;
                            ParentCalendar.NextMonthDisplay();
                            if (m_SelectedMonthBorderCell != null)
                            {
                                Border yb = (Border)m_SelectedMonthBorderCell;
                                yb.CornerRadius = new CornerRadius(0);
                                yb.BorderBrush = new SolidColorBrush();
                                yb.Background = new SolidColorBrush();
                                yb.BorderThickness = new Thickness(0);
                                Cell m_c = m_SelectedYearCell;
                                m_c.Foreground = Cell_ForeColor;
                                m_c.IsSelected = false;
                            }

                            ApplyBorder(c, m_b);
                            FocusedCell = c;
                            SelectedMonthBorder = m_b;
                            FocusedCellContent = c.Years;
                        }
                        else
                        {
                            c = FocusedCell;
                            m_b = (Border)c.Parent;
                            SelectedMonthBorder = m_b;
                            ApplyBorder(c, m_b);
                        }
                    }
                    else
                    {
                        c = FocusedCell;
                        m_b = (Border)c.Parent;
                        SelectedMonthBorder = m_b;
                        ApplyBorder(c, m_b);
                    }
                }
            }
            else
            {
                //// mc.IsMouseHover = false;
                mc.IsSelected = false;
                mc.Focus();
                mc.Foreground = Cell_ForeColor;
                ApplyNullBorderValue(b);
                b.UpdateLayout();
                int location = NewFocusedCellIndex();
                YearRangeCell c = (YearRangeCell)YearCellsCollection[location];
                Border m_b = (Border)c.Parent;
                SelectedMonthBorder = m_b;
                ApplyBorder(c, m_b);
                Border yb = YearBorderCollection[location];
                ParentCalendar.ChangeVisibleData(0, FocusedCellContent.StartYear);
                ParentCalendar.GetParetntCalendar(ref parentCul, ref visibleData);
                ////ParentCalendar.SetYearCellContent(ParentCul, FocusedCellContent.StartYear, FocusedCellContent.EndYear); 
                ParentCalendar.StoryBoardForMonth(((FrameworkElement)yb), 2d, 2d, (FrameworkElement)ParentCalendar.YearGrid.PopulatedYearGrid, (FrameworkElement)ParentCalendar.YearRangedGrid.PopulatedYearRangeGrid);
            }
        }

        /// <summary>
        /// Sets the <see cref="T:Syncfusion.Windows.Tools.Controls.YearRangeCell.YearRange"/> property of the <see cref="Syncfusion.Windows.Tools.Controls.YearRangeCell"/>.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="calendar">Current calendar.</param>
        protected internal void SetYearRange(VisibleDate date, System.Globalization.Calendar calendar)
        {
            int startYear = date.VisibleYear;
            Date startDate, endDate = new Date();
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
            int i = 0;
            foreach (YearRangeCell yrc in YearCellsCollection)
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
                    if (minDate < endDate && minDate > startDate)
                    {
                        if (yrc.Visibility == Visibility.Collapsed)
                        {
                            yrc.Visibility = Visibility.Visible;
                            YearBorderCollection[i].Visibility = Visibility.Visible;
                        }

                        yrc.Years = new YearsRange(startDate.Year, endDate.Year);
                    }
                    else
                    {
                        yrc.Visibility = Visibility.Collapsed;
                        yrc.Years = new YearsRange(-1, -1);
                        YearBorderCollection[i].Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (yrc.Visibility == Visibility.Collapsed)
                    {
                        yrc.Visibility = Visibility.Visible;
                        YearBorderCollection[i].Visibility = Visibility.Visible;
                    }

                    yrc.Years = new YearsRange(startDate.Year, endDate.Year);
                }

                startYear += 10;
                i++;
            }

            EndYearRange = endDate.Year;
            ContainsCurrentYear(date);
            SetYearRangeCellContent();
        }

        /// <summary>
        /// Sets the <see cref="T:Syncfusion.Windows.Tools.Controls.YearRangeCell.Content"/> property of the <see cref="Syncfusion.Windows.Tools.Controls.YearRangeCell"/>.
        /// </summary>
        protected internal void SetYearRangeCellContent()
        {
            if (m_SelectedMonthBorderCell != null)
            {
                Border yb = (Border)m_SelectedMonthBorderCell;
                yb.CornerRadius = new CornerRadius(0);
                yb.BorderBrush = new SolidColorBrush();
                yb.Background = new SolidColorBrush();
                yb.BorderThickness = new Thickness(0);
                Cell c = m_SelectedYearCell;
                c.Foreground = Cell_ForeColor;
                c.IsSelected = false;
            }

            YearsRange years;
            int startYear, endYear, i = 0;

            foreach (YearRangeCell yrc in YearCellsCollection)
            {
                yrc.Foreground = Cell_ForeColor;
                ApplyNullBorderValue(YearBorderCollection[i]);
                if (yrc.Visibility == Visibility.Visible)
                {
                    years = yrc.Years;
                    startYear = years.StartYear;
                    endYear = years.EndYear;
                    yrc.Content = startYear.ToString() + "-" + "\n" + endYear.ToString();

                    if ((startYear.ToString() + "-" + endYear.ToString()) == SelectedYearRange)
                    {
                        Border b = (Border)YearBorderCollection[i];
                        if (ParentCalendar != null)
                        {
                            b.CornerRadius = ParentCalendar.SelectedCellCornerRadius;
                            b.BorderBrush = ParentCalendar.SelectedCellBorderBrush;
                            b.BorderThickness = ParentCalendar.SelectedCellBorderThickness;
                            b.Background = ParentCalendar.SelectedCellBackground;
                            yrc.Foreground = ParentCalendar.SelectedCellForeground;
                            b.UpdateLayout();
                        }

                        m_SelectedYearCell = yrc;
                        m_SelectedMonthBorderCell = b;
                        SelectedMonthBorder = b;
                        yrc.IsSelected = true;
                        FocusedCell = yrc;
                        FocusedCellContent = yrc.Years;
                    }
                }

                i++;
            }
        }
    }
}
