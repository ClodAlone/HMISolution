// <copyright file="CalendarGrid.cs" company="Syncfusion">
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
using System.Windows.Data;
using System.Windows.Media;
using Calendar = System.Globalization.Calendar;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Implements the basic functionality required by the grid.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class CalendarEditGrid : FrameworkElement
    {
        #region Private members

        /// <summary>
        /// Inner grid for cells layout.
        /// </summary>
        private Grid minnerGrid;

        /// <summary>
        /// Number of columns.
        /// </summary>
        private int mcolumnsCount;

        /// <summary>
        /// Number of rows.
        /// </summary>
        private int mrowsCount;

        /// <summary>
        /// Collection of cells.
        /// </summary>
        private ArrayList mcellsCollection;

        /// <summary>
        /// Index of the cell in the <see cref="DayCell"/> collection that is focused.
        /// </summary>
        private int mfocusedCellIndex;

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the CalendarEditGrid class.
        /// </summary>
        /// <param name="rowsCount">Number of rows.</param>
        /// <param name="columnsCount">Number of columns.</param>
        public CalendarEditGrid(int rowsCount, int columnsCount)
        {
            this.minnerGrid = new Grid();
            this.minnerGrid.SnapsToDevicePixels = true;
            this.RowsCount = rowsCount;
            this.ColumnsCount = columnsCount;
            this.CellsCollection = new ArrayList();
            this.GenerateGrid();
            this.FillGrid();
            BindingUtils.SetRelativeBinding(this.minnerGrid, FrameworkElement.FlowDirectionProperty, typeof(CalendarEdit), FrameworkElement.FlowDirectionProperty, BindingMode.OneWay, 1);
            AddLogicalChild(this.minnerGrid);
            AddVisualChild(this.minnerGrid);

            //BindingUtils.SetRelativeBinding(this.minnerGrid, SkinStorage.VisualStyleProperty, typeof(CalendarEdit), SkinStorage.VisualStyleProperty, BindingMode.OneWay);
            //BindingUtils.SetRelativeBinding(this.minnerGrid, SkinStorage.VisualStylesListProperty, typeof(CalendarEdit), SkinStorage.VisualStylesListProperty, BindingMode.OneWay);
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets or sets index of the focused cell.
        /// </summary>
        /// <value>
        /// Type: <see cref="Syncfusion.Windows.Shared.CalendarEditGrid.FocusedCellIndex">Integer</see>
        /// </value>
        /// <seealso cref="Syncfusion.Windows.Shared.CalendarEditGrid">Integer</seealso>
        protected internal int FocusedCellIndex
        {
            get
            {
                return this.mfocusedCellIndex;
            }

            set
            {
                if (this.mfocusedCellIndex != value)
                {
                    this.mfocusedCellIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets count of the rows.
        /// </summary>
        /// <value>
        /// Type: <see cref="Syncfusion.Windows.Shared.CalendarEditGrid.RowsCount">Integer</see>
        /// </value>
        /// <seealso cref="Syncfusion.Windows.Shared.CalendarEditGrid">Integer</seealso>
        protected internal int RowsCount
        {
            get
            {
                return this.mrowsCount;
            }

            set
            {
                this.mrowsCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the columns count.
        /// </summary>
        /// <value>The columns count.</value>
        protected internal int ColumnsCount
        {
            get
            {
                return this.mcolumnsCount;
            }

            set
            {
                this.mcolumnsCount = value;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="DayCell"/> collection.
        /// </summary>
        /// <value>
        /// Type: <see cref="ArrayList"/>
        /// </value>
        /// <seealso cref="ArrayList"/>
        protected internal ArrayList CellsCollection
        {
            get
            {
                return this.mcellsCollection;
            }

            set
            {
                this.mcellsCollection = value;
            }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Sets the selected element.
        /// </summary>
        /// <param name="date">Current date.</param>
        public virtual void SetIsSelected(VisibleDate date)
        {
        }

        /// <summary>
        /// Initializes grid content.
        /// </summary>
        /// <param name="date">Current date.</param>
        /// <param name="culture">Current culture.</param>
        /// <param name="calendar">Current calendar.</param>
        public virtual void Initialize(VisibleDate date, CultureInfo culture, Calendar calendar)
        {
        }

        /// <summary>
        /// Adds element to the innerGrid children collection.
        /// </summary>
        /// <param name="element">Element to be added.</param>
        protected void AddToInnerGrid(UIElement element)
        {
            this.minnerGrid.Children.Add(element);
        }

        /// <summary>
        /// Creates instance of the single cell.
        /// </summary>
        /// <returns>
        /// New instance of the cell.
        /// </returns>
        protected abstract Cell CreateCell();

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The index of the visual object in the
        /// VisualCollection.</param>
        /// <exception cref="ArgumentOutOfRangeException">Index must be 0 because only one child element is present.</exception>
        /// <returns>
        /// The child in the VisualCollection at the specified index
        /// value.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("Only one child element is present.");
            }

            return (Visual)this.minnerGrid;
        }

        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size for the control.
        /// </summary>
        /// <param name="availableSize">The available size that this
        /// element can give to the child. Infinity can be specified as a
        /// value to indicate that the element will size to whatever
        /// content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout,
        /// based on its calculations of children's sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            this.minnerGrid.Measure(availableSize);
            return this.minnerGrid.DesiredSize;
        }

        /// <summary>
        /// Positions child elements and determines a size for the control.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.minnerGrid.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        /// <summary>
        /// Fills the grid with column definitions and row definitions.
        /// </summary>
        private void GenerateGrid()
        {
            // Define Columns
            ArrayList columns = new ArrayList();
            ArrayList rows = new ArrayList();

            for (int i = 0; i < this.ColumnsCount; i++)
            {
                ColumnDefinition cdcolumn = new ColumnDefinition();
                cdcolumn.Width = new GridLength(1, GridUnitType.Star);
                columns.Add(cdcolumn);
            }

            foreach (ColumnDefinition cd in columns)
            {
                this.minnerGrid.ColumnDefinitions.Add(cd);
            }

            // Define Rows
            for (int i = 0; i < this.RowsCount; i++)
            {
                RowDefinition rdrow = new RowDefinition();
                rdrow.Height = new GridLength(1, GridUnitType.Star);
                rows.Add(rdrow);
            }

            foreach (RowDefinition rd in rows)
            {
                this.minnerGrid.RowDefinitions.Add(rd);
            }
        }

        /// <summary>
        /// Adds day and dayName cells to the grid.
        /// </summary>
        private void FillGrid()
        {
            // adding DayCells
            for (int i = 0; i < this.ColumnsCount * this.RowsCount; i++)
            {
                Cell dccell = this.CreateCell();
                this.CellsCollection.Add(dccell);
            }

            for (int i = 0, k = 0; i < this.RowsCount; i++)
            {
                for (int j = 0; j < this.ColumnsCount; j++)
                {
                    Grid.SetRow((UIElement)this.CellsCollection[k], i);
                    Grid.SetColumn((UIElement)this.CellsCollection[k], j);
                    UIElement temp = (UIElement)this.CellsCollection[k];
                    this.minnerGrid.Children.Add(temp);
                    k++;
                }
            }
        }

        #endregion Implementation
    }
}