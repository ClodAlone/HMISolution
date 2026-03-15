#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
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
    /// Represents the Week Numbers Grid.
    /// </summary>
    public class WeekNumbersGrid : FrameworkElement
    {
        /// <summary>
        /// Default number of columns.
        /// </summary>
        private const int DEF_COLUMNS_COUNT = 1;

        /// <summary>
        /// Default number of rows.
        /// </summary>
        private const int DEF_ROWS_COUNT = 7;
        private StackPanel m_stack;

        /// <summary>
        /// Collection of Borders.
        /// </summary>
        private List<Border> m_weekNumberBorderCollection;

        /// <summary>
        /// Collection of cells.
        /// </summary>
        private List<Cell> m_weekNumberCells;

        /// <summary>
        /// Inner grid used for layout logic.
        /// </summary>
        private Grid m_weekNumberGrid;

        /// <summary>
        /// Initializes new instance of the WeekNumbersGrid class.
        /// </summary>
        public WeekNumbersGrid()
        {
            m_stack = new StackPanel();
            m_stack.Orientation = Orientation.Vertical;
            m_weekNumberGrid = new Grid();
            m_weekNumberCells = new List<Cell>();
            m_weekNumberBorderCollection = new List<Border>();
            GenerateGrid();
            FillGrid();
        }

        /// <summary>
        /// Gets or sets the parent calendar.
        /// </summary>
        /// <value>The parent calendar.</value>
        internal CalendarControl ParentCalendar
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the pop stack panel.
        /// </summary>
        /// <value>The pop stack panel.</value>
        public StackPanel popStackPanel
        {
            get
            {
                return m_stack;
            }
        }

        /// <summary>
        /// Gets the populatedweek number grid.
        /// </summary>
        /// <value>The populatedweek number grid.</value>
        public Grid PopulatedweekNumberGrid
        {
            get
            {
                return m_weekNumberGrid;
            }
        }

        /// <summary>
        /// Gets or sets the week number border collection.
        /// </summary>
        /// <value>The week number border collection.</value>
        protected internal List<Border> WeekNumberBorderCollection
        {
            get
            {
                return m_weekNumberBorderCollection;
            }

            set
            {
                m_weekNumberBorderCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets the week number cells collection.
        /// </summary>
        /// <value>The week number cells collection.</value>
        protected internal List<Cell> WeekNumberCellsCollection
        {
            get
            {
                return m_weekNumberCells;
            }

            set
            {
                m_weekNumberCells = value;
            }
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
            // b.Name = "ApplyBorder" + row.ToString() + column.ToString();
            return b;
        }

        /// <summary>
        /// Creates the instance of the single cell.
        /// </summary>
        /// <returns>
        /// New instance of the cell.
        /// </returns>
        protected Cell CreateCell()
        {
            return new WeekNumberCell();
        }

        /// <summary>
        /// Adds week number cell to the grid.
        /// </summary>
        protected void FillGrid()
        {
            for (int i = 0; i < DEF_ROWS_COUNT; i++)
            {
                Border br = ApplyNullBorder(i, 0);
                Cell weekNumberCell = CreateCell();
                ////weekNumberCell.Name = "weekNumberCell" + i.ToString() + "0";
                weekNumberCell.HorizontalAlignment = HorizontalAlignment.Center;
                weekNumberCell.VerticalAlignment = VerticalAlignment.Center;

                ////br.Child = weekNumberCell;
                Grid.SetRow((FrameworkElement)br, i);
                Grid.SetColumn((FrameworkElement)br, 0);
                WeekNumberCellsCollection.Add(weekNumberCell);
                WeekNumberBorderCollection.Add(br);
                ////m_weekNumberGrid.Children.Add(br);
                ////m_stack.Children.Add(br);
            }
        }

        /// <summary>
        /// Fills the grid with column definitions and row definitions.
        /// </summary>
        protected void GenerateGrid()
        {
            for (int i = 0; i < DEF_COLUMNS_COUNT; i++)
            {
                ColumnDefinition cdColumn = new ColumnDefinition();
                cdColumn.Width = new GridLength(1, GridUnitType.Star);
                m_weekNumberGrid.ColumnDefinitions.Add(cdColumn);
            }

            // Define Rows
            for (int i = 0; i < DEF_ROWS_COUNT; i++)
            {
                RowDefinition rdRow = new RowDefinition();
                rdRow.Height = new GridLength(1, GridUnitType.Star);
                m_weekNumberGrid.RowDefinitions.Add(rdRow);
            }
        }

        /// <summary>
        /// Sets the cell's content.
        /// </summary>
        protected internal void SetWeekNumbers(List<WeekNumberCell> list)
        {
            if (list.Count > 0)
            {
                try
                {
                    for (int i = 1, k = 0; i < WeekNumberCellsCollection.Count; i++)
                    {
                        (WeekNumberCellsCollection[i] as WeekNumberCell).Content = list[k].Content;
                        k++;
                        (WeekNumberCellsCollection[i] as WeekNumberCell).Foreground = ParentCalendar.DayForeground;
                    }
                }
                catch
                {
                }
            }
        }
    }
}
