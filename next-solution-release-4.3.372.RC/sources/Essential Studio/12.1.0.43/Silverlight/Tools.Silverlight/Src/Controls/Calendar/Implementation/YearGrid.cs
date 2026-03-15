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
    /// Represents the Year Grid class.
    /// </summary>
    public class YearGrid
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
        private Grid mYearGrid;

        private VisibleDate visibledata;

        /// <summary>
        /// Default public constructor.
        /// Initializes new instance of the MonthGrid class.
        /// </summary>
        public YearGrid()
        {
            mYearGrid = new Grid();
            ////m_YearGrid.Name = "Year Grid";
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
        /// Gets or sets the end year.
        /// </summary>
        /// <value>The end year.</value>
        public int EndYear
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the m_ mouse hovered cell.
        /// </summary>
        /// <value>The m_ mouse hovered cell.</value>
        protected internal YearCell m_MouseHoveredCell
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the focused cell.
        /// </summary>
        /// <value>The focused cell.</value>
        protected internal YearCell FocusedCell
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
        /// Gets or sets the m_ selected month cell.
        /// </summary>
        /// <value>The m_ selected month cell.</value>
        protected Cell m_SelectedMonthCell
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
        /// Gets the populated year grid.
        /// </summary>
        /// <value>The populated year grid.</value>
        public Grid PopulatedYearGrid
        {
            get
            {
                return mYearGrid;
            }
        }

        ////with Border and Cell
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
        /// Gets or sets the start year.
        /// </summary>
        /// <value>The start year.</value>
        public int StartYear
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

        /// <summary>
        /// Applieds the borderand focus.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="yc">The yc.</param>
        private void AppliedBorderandFocus(int i, YearCell yc)
        {
            Border b = YearBorderCollection[i];
            b.CornerRadius = ParentCalendar.SelectedCellCornerRadius;
            b.BorderBrush = ParentCalendar.SelectedCellBorderBrush;
            b.BorderThickness = ParentCalendar.SelectedCellBorderThickness;
            b.Background = ParentCalendar.SelectedCellBackground;
            b.UpdateLayout();
            m_SelectedMonthCell = yc;
            FocusedCell = yc;
            FocusedCellContent = (int)yc.Year;
            yc.Focus();
            yc.Foreground = ParentCalendar.SelectedCellForeground;
            m_SelectedMonthBorderCell = b;
            SelectedMonthBorder = b;
            yc.IsSelected = true;
        }

        /// <summary>
        /// Applies the border.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="b">The b.</param>
        private void ApplyBorder(YearCell cell, Border b)
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
        /// Handles the MouseEnter event of the br control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void br_MouseEnter(object sender, MouseEventArgs e)
        {
            Border b = (Border)sender;
            int iRow = Grid.GetRow((FrameworkElement)b);
            int iColumn = Grid.GetColumn((FrameworkElement)b);

            Cell cell = (Cell)b.Child;  ////m_YearGrid.FindName("YearCell" + iRow.ToString() + iColumn.ToString());
            m_MouseHoveredCell = (YearCell)cell;
            cell.Focus();
            if (!cell.IsSelected)
            {
                ////m_MouseHoverCell = cell;
                ////cell.IsMouseHover = true;
                m_MouseHoveredCell = (YearCell)cell;
                b.CornerRadius = ParentCalendar.MouseHoverCellCornerRadius;
                b.BorderBrush = ParentCalendar.MouseHoverCellBorderBrush;
                b.BorderThickness = ParentCalendar.MouseHoverCellBorderThickness;
                b.Background = ParentCalendar.MouseHoverCellBackground;
                cell.Foreground = ParentCalendar.MouseHoverCellForeground;
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
        /// Handles the MouseLeave event of the br control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void br_MouseLeave(object sender, MouseEventArgs e)
        {
            Border b = (Border)sender;
            int iRow = Grid.GetRow((FrameworkElement)b);
            int iColumn = Grid.GetColumn((FrameworkElement)b);

            Cell cell = (Cell)b.Child;  //// m_YearGrid.FindName("YearCell" + iRow.ToString() + iColumn.ToString());
            m_MouseHoveredCell = null;
            if (!cell.IsSelected)
            {
                cell.Foreground = Cell_ForeColor;
                b.CornerRadius = new CornerRadius(0);
                b.BorderBrush = new SolidColorBrush();
                b.Background = new SolidColorBrush();
                b.BorderThickness = new Thickness(0);
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

            Cell cell = (Cell)b.Child;  //// m_YearGrid.FindName("YearCell" + iRow.ToString() + iColumn.ToString());
            cell.Focus();
            if (!cell.IsSelected)
            {
                m_SelectedMonthCell = cell;
                m_SelectedMonthBorderCell = b;
                SelectedMonthBorder = b;
                cell.Foreground = ParentCalendar.SelectedCellForeground;
                cell.IsSelected = true;
            }
        }

        /// <summary>
        /// Creates new instance of the <see cref="Syncfusion.Windows.Tools.Controls.MonthCell"/> class.
        /// </summary>
        /// <returns>New instance of the <see cref="Syncfusion.Windows.Tools.Controls.MonthCell"/> class.</returns>
        protected Cell CreateCell()
        {
            return new YearCell();
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
                    mYearGrid.Children.Add(br);

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
                mYearGrid.ColumnDefinitions.Add(cdColumn);
            }

            //// Define Rows
            for (int i = 0; i < DEF_ROWS_COUNT; i++)
            {
                RowDefinition rdRow = new RowDefinition();
                rdRow.Height = new GridLength(1, GridUnitType.Star);
                mYearGrid.RowDefinitions.Add(rdRow);
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
                    YearCell mc;
                    if (FocusedCell == null)
                    {
                        mc = (YearCell)sender;
                        FocusedCell = mc;
                        FocusedCellContent = (int)mc.Year;
                    }

                    int loc = NewFocusedCellIndex();
                    if (loc >= 0)
                    {
                        mc = (YearCell)YearCellsCollection[loc];

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
                YearCell dc = (YearCell)YearCellsCollection[i];
                dc.IsSelected = false;
                if (((YearCell)dc).Year == FocusedCellContent)
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
            Border b = (Border)mc.Parent;
            CultureInfo parentCul = null;
            VisibleDate visibleData = new VisibleDate();
            if (!changingEnabled)
            {
                if (e.Key == Key.Right || e.Key == Key.Left)
                {
                    ////    mc.IsMouseHover = false;
                    mc.IsSelected = false;
                    mc.Focus();
                    mc.Foreground = Cell_ForeColor;
                    ApplyNullBorderValue(b);
                    b.UpdateLayout();
                }

                if (e.Key == Key.Right)
                {
                    YearCell c;
                    Border m_b;
                    if (loc == 10)
                    {
                        if (((YearCell)YearCellsCollection[loc + 1]).Visibility == Visibility.Visible && ParentCalendar.next.Fill != ParentCalendar.buttonDisabledColor)
                        {
                            c = (YearCell)YearCellsCollection[1];
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
                                Cell m_c = m_SelectedMonthCell;
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
                        if (((YearCell)YearCellsCollection[loc + 1]).Visibility == Visibility.Visible)
                        {
                            c = (YearCell)YearCellsCollection[loc + 1];
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
                    FocusedCellContent = (int)c.Year;
                }
                else if (e.Key == Key.Left)
                {
                    YearCell c;
                    Border m_b;
                    if (loc == 1)
                    {
                        if (((YearCell)YearCellsCollection[0]).Visibility == Visibility.Visible && ParentCalendar.next.Fill != ParentCalendar.buttonDisabledColor)                        
                        {
                            c = (YearCell)YearCellsCollection[10];
                            m_b = (Border)c.Parent;
                            ParentCalendar.PreviouMonthDisplay();
                            if (m_SelectedMonthBorderCell != null)
                            {
                                Border yb = (Border)m_SelectedMonthBorderCell;
                                yb.CornerRadius = new CornerRadius(0);
                                yb.BorderBrush = new SolidColorBrush();
                                yb.Background = new SolidColorBrush();
                                yb.BorderThickness = new Thickness(0);
                                Cell m_c = m_SelectedMonthCell;
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
                        if (((YearCell)YearCellsCollection[loc - 1]).Visibility == Visibility.Visible)
                        {
                            c = (YearCell)YearCellsCollection[loc - 1];
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
                    FocusedCellContent = (int)c.Year;
                }
                else if (e.Key == Key.Up)
                {
                    YearCell c;
                    Border m_b;
                    if (loc > 4 && loc <= 11)
                    {
                        ////mc.IsMouseHover = false;
                        mc.IsSelected = false;
                        mc.Focus();
                        mc.Foreground = Cell_ForeColor;
                        ApplyNullBorderValue(b);
                        b.UpdateLayout();
                        c = (YearCell)YearCellsCollection[loc - 4];
                        if (c.Visibility == Visibility.Visible)
                        {
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                            c.IsSelected = true;
                            FocusedCell = c;
                            SelectedMonthBorder = m_b;
                            FocusedCellContent = (int)c.Year;
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
                            c = (YearCell)YearCellsCollection[10];
                        }
                        else
                        {
                            c = (YearCell)YearCellsCollection[8 + loc];
                            if (loc == 3)
                            {
                                c = (YearCell)YearCellsCollection[7 + loc];
                            }
                        }
                        ////c = (YearCell)YearCellsCollection[10];
                        m_b = (Border)c.Parent;
                        ParentCalendar.PreviouMonthDisplay();
                        if (m_SelectedMonthBorderCell != null)
                        {
                            Border yb = (Border)m_SelectedMonthBorderCell;
                            yb.CornerRadius = new CornerRadius(0);
                            yb.BorderBrush = new SolidColorBrush();
                            yb.Background = new SolidColorBrush();
                            yb.BorderThickness = new Thickness(0);
                            Cell m_c = m_SelectedMonthCell;
                            m_c.Foreground = Cell_ForeColor;
                            m_c.IsSelected = false;
                        }

                        ApplyBorder(c, m_b);
                        FocusedCell = c;
                        SelectedMonthBorder = m_b;
                        FocusedCellContent = (int)c.Year;
                    }                     
                }
                else if (e.Key == Key.Down)
                {
                    YearCell c;
                    Border m_b;
                    if (loc >= 0 && loc < 7)
                    {
                        //// mc.IsMouseHover = false;
                        mc.IsSelected = false;
                        mc.Focus();
                        mc.Foreground = Cell_ForeColor;
                        ApplyNullBorderValue(b);
                        b.UpdateLayout();
                        c = (YearCell)YearCellsCollection[loc + 4];
                        if (c.Visibility == Visibility.Visible)
                        {
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                            c.IsSelected = true;
                            FocusedCell = c;
                            FocusedCellContent = (int)c.Year;
                        }
                        else
                        {
                            c = FocusedCell;
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                        }

                        SelectedMonthBorder = m_b;
                    }
                    else if (loc < 12 && loc >= 7)
                    {
                        ////((YearCell)YearCellsCollection[loc + 4]).Visibility == Visibility.Visible && 
                        if (ParentCalendar.next.Fill != ParentCalendar.buttonDisabledColor)
                        {
                            if (loc == 7)
                            {
                                c = (YearCell)YearCellsCollection[1];
                            }
                            else
                            {
                                c = (YearCell)YearCellsCollection[loc - 8];
                            }
                            ////c = (YearCell)YearCellsCollection[loc - 8];
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
                                Cell m_c = m_SelectedMonthCell;
                                m_c.Foreground = Cell_ForeColor;
                                m_c.IsSelected = false;
                            }

                            ApplyBorder(c, m_b);
                            FocusedCell = c;
                            SelectedMonthBorder = m_b;
                            FocusedCellContent = (int)c.Year;
                        }
                        else
                        {
                            c = FocusedCell;
                            m_b = (Border)c.Parent;
                            ApplyBorder(c, m_b);
                        }

                        SelectedMonthBorder = m_b;
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
                YearCell c = (YearCell)YearCellsCollection[location];
                Border m_b = (Border)c.Parent;
                ApplyBorder(c, m_b);
                SelectedMonthBorder = m_b;
                Border yb = YearBorderCollection[location];
                ParentCalendar.ChangeVisibleData(0, FocusedCellContent);
                ParentCalendar.GetParetntCalendar(ref parentCul, ref visibleData);
                ////ParentCalendar.SetMonthCellContent(ParentCul, visibleData.VisibleMonth, visibleData.VisibleYear);
                ParentCalendar.StoryBoardForMonth(((FrameworkElement)yb), 2d, 2d, (FrameworkElement)ParentCalendar.MonthNameGrid.PopulatedMonthGrid, (FrameworkElement)ParentCalendar.YearGrid.PopulatedYearGrid);
            }
        }

        /// <summary>
        /// Sets the <see cref="T:Syncfusion.Windows.Tools.Controls.YearCell.Year"/> property of the <see cref="Syncfusion.Windows.Tools.Controls.YearCell"/>.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="calendar">Current calendar.</param>
        /// <param name="type">Type</param>
        protected internal void SetYear(VisibleDate date, System.Globalization.Calendar calendar, string type)
        {
            int visibleYear = 0;
            if (m_SelectedMonthBorderCell != null)
            {
                Border yb = (Border)m_SelectedMonthBorderCell;
                yb.CornerRadius = new CornerRadius(0);
                yb.BorderBrush = new SolidColorBrush();
                yb.Background = new SolidColorBrush();
                yb.BorderThickness = new Thickness(0);
                Cell c = m_SelectedMonthCell;
                c.Foreground = Cell_ForeColor;
                c.IsSelected = false;
            }

            int i = 0;
            int startYear = date.VisibleYear;
            Date curDate;
            Date minDate = new Date(calendar.MinSupportedDateTime, calendar);
            Date maxDate = new Date(calendar.MaxSupportedDateTime, calendar);

            while (startYear % 10 != 0)
            {
                startYear--;
            }

            ////if (startYear == maxDate.Year)
            ////    startYear -= 10;

            startYear--;
            if (startYear > 0)
            {
                StartYear = startYear;
            }
            else
            {
                StartYear = 1;
            }

            if (type.ToLower() == "prev")
            {
                if (StartYear != 1)
                {
                    visibleYear = StartYear + 1;
                }
                else
                {
                    visibleYear = 1;
                }
            }
            else if (type.ToLower() == "next")
            {
                visibleYear = StartYear + YearCellsCollection.Count - 2;
            }
            else
            {
                if (StartYear != 1)
                {
                    visibleYear = date.VisibleYear;
                }
                else
                {
                    visibleYear = 1;
                }
            }

            bool appliedBorderLastDate = false;
            bool appliedBorderAnyDate = false;
            foreach (YearCell yc in YearCellsCollection)
            {
                ApplyNullBorderValue(YearBorderCollection[i]);
                YearCell tyc = (YearCell)YearCellsCollection[i];
                curDate = new Date(startYear, date.VisibleMonth, 1);
                yc.Foreground = Cell_ForeColor;
                if (curDate > maxDate || curDate < minDate || minDate.Year == curDate.Year)
                {
                    if (minDate.Year == curDate.Year)
                    {
                        if (yc.Visibility == Visibility.Collapsed)
                        {
                            yc.Visibility = Visibility.Visible;
                            YearBorderCollection[i].Visibility = Visibility.Visible;
                        }

                        yc.Year = startYear;
                        AppliedBorderandFocus(i, yc);
                    }
                    else
                    {
                        yc.Visibility = Visibility.Collapsed;
                        yc.Year = -1;
                        YearBorderCollection[i].Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (yc.Visibility == Visibility.Collapsed)
                    {
                        yc.Visibility = Visibility.Visible;
                        YearBorderCollection[i].Visibility = Visibility.Visible;
                    }

                    yc.Year = startYear;
                    if (startYear == visibleYear)
                    {
                        Border b = YearBorderCollection[i];
                        if (ParentCalendar != null)
                        {
                            b.CornerRadius = ParentCalendar.SelectedCellCornerRadius;
                            b.BorderBrush = ParentCalendar.SelectedCellBorderBrush;
                            b.BorderThickness = ParentCalendar.SelectedCellBorderThickness;
                            b.Background = ParentCalendar.SelectedCellBackground;
                            yc.Foreground = ParentCalendar.SelectedCellForeground;
                        }

                        b.UpdateLayout();
                        m_SelectedMonthCell = yc;
                        FocusedCell = yc;
                        FocusedCellContent = (int)yc.Year;
                        yc.Focus();

                        m_SelectedMonthBorderCell = b;
                        SelectedMonthBorder = b;
                        yc.IsSelected = true;
                        appliedBorderAnyDate = true;
                    }
                }

                startYear++;
                i++;
                if (yc.Visibility == Visibility.Collapsed && ((YearCell)YearCellsCollection[0]).Visibility == Visibility.Visible)
                {
                    if (!appliedBorderLastDate && !appliedBorderAnyDate)
                    {
                        int temp = i;
                        AppliedBorderandFocus(temp - 2, (YearCell)YearCellsCollection[temp - 2]);
                        appliedBorderLastDate = true;
                    }
                }
            }

            if (startYear > 0)
            {
                EndYear = startYear - 1;
            }
            else
            {
                EndYear = 1;
            }

            SetYearCellContent();
        }

        /// <summary>
        /// Sets the <see cref="T:Syncfusion.Windows.Tools.Controls.YearCell.Content"/> property of the <see cref="Syncfusion.Windows.Tools.Controls.YearCell"/>.
        /// </summary>
        protected internal void SetYearCellContent()
        {
            foreach (YearCell yc in YearCellsCollection)
            {
                if (yc.Visibility == Visibility.Visible)
                {
                    yc.Content = yc.Year;
                }
            }

            ((YearCell)YearCellsCollection[0]).Opacity = 0.5;
            ((YearCell)YearCellsCollection[YearCellsCollection.Count-1]).Opacity = 0.5;
        }
    }
}
