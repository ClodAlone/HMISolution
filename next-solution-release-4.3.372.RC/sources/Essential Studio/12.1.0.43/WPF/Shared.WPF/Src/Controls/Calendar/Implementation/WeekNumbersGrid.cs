// <copyright file="WeekNumbersGrid.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a grid that consists of columns
    /// that contain week number cell elements.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class WeekNumbersGrid : CalendarEditGrid
    {
        #region Constants

        /// <summary>
        /// Default number of columns.
        /// </summary>
        private const int DEFCOLUMNSCOUNT = 1;

        /// <summary>
        /// Default number of rows.
        /// </summary>
        private const int DEFROWSCOUNT = 6;

        #endregion Constants

        #region Private members

        /// <summary>
        /// Inner grid used for layout logic.
        /// </summary>
        private Grid minnerGrid;

        /// <summary>
        /// Parent instance.
        /// </summary>
        private CalendarEdit mparentCalendar;

        /// <summary>
        /// Collection of cells.
        /// </summary>
        private ArrayList mweekNumberCells;

        #endregion Private members

        #region Initilization

        /// <summary>
        /// Initializes static members of the <see cref="WeekNumbersGrid"/> class.  It overrides some dependency properties.
        /// </summary>
        static WeekNumbersGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WeekNumbersGrid), new FrameworkPropertyMetadata(typeof(WeekNumbersGrid)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeekNumbersGrid"/> class.
        /// </summary>
        public WeekNumbersGrid()
            : base(DEFROWSCOUNT, DEFCOLUMNSCOUNT)
        {
            this.minnerGrid = new Grid();
            this.WeekNumberCells = new ArrayList();
            this.GenerateGrid();
            this.FillGrid();
            this.AddLogicalChild(this.minnerGrid);
            this.AddVisualChild(this.minnerGrid);
            BindingUtils.SetRelativeBinding(this.minnerGrid, FrameworkElement.FlowDirectionProperty, typeof(CalendarEdit), FrameworkElement.FlowDirectionProperty, BindingMode.OneWay, 1);
        }

        #endregion Initilization

        #region Properties

        /// <summary>
        /// Gets or sets the collection of cells.
        /// </summary>
        /// <value>
        /// Type: <see cref="ArrayList"/>
        /// </value>
        /// <seealso cref="ArrayList"/>
        public ArrayList WeekNumberCells
        {
            get
            {
                return this.mweekNumberCells;
            }

            set
            {
                this.mweekNumberCells = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// Type: <see cref="CalendarEdit"/>
        /// </value>
        /// <seealso cref="CalendarEdit"/>
        public CalendarEdit ParentCalendar
        {
            get
            {
                return this.mparentCalendar;
            }

            set
            {
                this.mparentCalendar = value;
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
        /// Sets the cell's content.
        /// </summary>
        /// <param name="list">The list of WeekNumberCell.</param>
        protected internal void SetWeekNumbers(List<WeekNumberCell> list)
        {
            if (this.ParentCalendar == null)
            {
                this.ParentCalendar = Parent as CalendarEdit;
            }

            if (this.ParentCalendar != null)
            {
                for (int i = 0; i < this.WeekNumberCells.Count && i < list.Count; i++)
                {
                    (this.WeekNumberCells[i] as WeekNumberCell).Content = list[i].Content;
                }
            }
        }

        /// <summary>
        /// Updates data template and data template selector on all
        /// cells.
        /// </summary>
        /// <param name="template">Data template to be set to the cell.
        /// If it is null the local value of data template would be cleared.</param>
        /// <param name="selector">Data template selector to be set to
        /// the cell. If it is null the local value would be cleared.</param>
        protected internal void UpdateTemplateAndSelector(DataTemplate template, DataTemplateSelector selector)
        {
            if (template != null && selector != null)
            {
                throw new ArgumentException("Both template and selector can not be set at one time.");
            }

            foreach (WeekNumberCell wnc in this.WeekNumberCells)
            {
                wnc.UpdateCellTemplateAndSelector(template, selector);
            }
        }

        /// <summary>
        /// Updates styles on all cells.
        /// </summary>
        /// <param name="style">Style to be set.</param>
        protected internal void UpdateStyles(Style style)
        {
            foreach (WeekNumberCell wnc in this.WeekNumberCells)
            {
                wnc.SetStyle(style);
            }
        }

        /// <summary>
        /// Fills the grid with column definitions and row definitions.
        /// </summary>
        protected void GenerateGrid()
        {
            ArrayList columns = new ArrayList();

            ColumnDefinition cdcolumn = new ColumnDefinition();
            cdcolumn.Width = new GridLength(1, GridUnitType.Star);
            columns.Add(cdcolumn);

            foreach (ColumnDefinition cd in columns)
            {
                this.minnerGrid.ColumnDefinitions.Add(cd);
            }

            for (int i = 0; i < DEFROWSCOUNT; i++)
            {
                RowDefinition rdrow = new RowDefinition();
                rdrow.Height = new GridLength(1, GridUnitType.Star);
                this.minnerGrid.RowDefinitions.Add(rdrow);
            }
        }

        /// <summary>
        /// Adds week number cell to the grid.
        /// </summary>
        protected void FillGrid()
        {
            for (int i = 0; i < DEFROWSCOUNT; i++)
            {
                WeekNumberCell wncCell = new WeekNumberCell();
                this.WeekNumberCells.Add(wncCell);

                Grid.SetRow((UIElement)this.WeekNumberCells[i], i);
                Grid.SetColumn((UIElement)this.WeekNumberCells[i], 0);

                this.minnerGrid.Children.Add((UIElement)this.WeekNumberCells[i]);
            }
        }

        /// <summary>
        /// Updates the parent.
        /// </summary>
        protected void UpdateParent()
        {
            if (this.Parent == null)
            {
                throw new NullReferenceException("Parent is null");
            }

            CalendarEdit calendar = this.Parent as CalendarEdit;

            if (calendar == null)
            {
                throw new NotSupportedException("Parent must be inherited from CalendarEdit type.");
            }

            this.ParentCalendar = calendar;
        }

        /// <summary>
        /// Creates the instance of the single cell.
        /// </summary>
        /// <returns>
        /// New instance of the cell.
        /// </returns>
        protected override Cell CreateCell()
        {
            return new WeekNumberCell();
        }

        #endregion Implementation

        #region Overrides

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
                throw new ArgumentOutOfRangeException("Only one child element is present");
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
        /// Invoked when the parent is changed.
        /// </summary>
        /// <param name="oldParent">The previous parent. Set to a null
        /// reference (Nothing in Visual Basic) if the DependencyObject did not have
        /// a previous parent.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            this.UpdateParent();
        }

        #endregion Overrides
    }
}